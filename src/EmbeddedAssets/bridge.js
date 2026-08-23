// ============================================================
//  C# ↔ JS 桥接 + 剪贴板图片粘贴
//  与 C# 端 ApiBridge.cs 配套
// ============================================================

(function () {
  'use strict';

  // -------------------- Promise 化的桥接 --------------------
  let __msgId = 0;
  const __pending = new Map();

  function call(method, ...args) {
    return new Promise((resolve, reject) => {
      const id = ++__msgId;
      __pending.set(id, { resolve, reject });
      try {
        // 关键：只把单个参数直接传；多个参数才包成数组。
        // C# 端大多数方法期望 args 是单个值（不是数组），多参方法（saveFile 等）才期望数组。
        // 0 个参数 → 不传 args 字段（C# TryGetProperty 走 default 分支）
        const argsValue = args.length <= 1 ? args[0] : args;
        window.chrome.webview.postMessage(JSON.stringify({ id, method, args: argsValue }));
      } catch (err) {
        __pending.delete(id);
        reject(err);
      }
    });
  }

  // 监听来自 C# 的响应
  if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', (e) => {
      try {
        const msg = typeof e.data === 'string' ? JSON.parse(e.data) : e.data;
        const p = __pending.get(msg.id);
        if (!p) return;
        __pending.delete(msg.id);
        if (msg.ok) p.resolve(msg.result);
        else p.reject(new Error(msg.error || 'Bridge error'));
      } catch (err) {
        console.error('bridge parse error', err);
      }
    });
  }

  // 让外部能用：window.bridge.call(...)
  window.bridge = { call };

  // -------------------- 拦截原生对话框 --------------------
  // 1) confirm
  const __origConfirm = window.confirm;
  window.confirm = async function (msg) {
    if (!window.chrome?.webview) return __origConfirm.call(window, msg);
    try { return await call('confirm', String(msg ?? ''), ''); }
    catch { return __origConfirm.call(window, msg); }
  };

  // 2) alert
  const __origAlert = window.alert;
  window.alert = async function (msg) {
    if (!window.chrome?.webview) return __origAlert.call(window, msg);
    try { await call('alert', String(msg ?? ''), ''); return; }
    catch { __origAlert.call(window, msg); }
  };

  // 3) prompt
  const __origPrompt = window.prompt;
  window.prompt = async function (msg, def = '') {
    if (!window.chrome?.webview) return __origPrompt.call(window, msg, def);
    try { return await call('prompt', String(msg ?? ''), String(def ?? ''), ''); }
    catch { return __origPrompt.call(window, msg, def); }
  };

  // -------------------- 拦截 navigator.clipboard --------------------
  // 在 WebView2 中 file:// 协议下 navigator.clipboard 不可用，走 C# 桥
  if (!navigator.clipboard && window.bridge) {
    Object.defineProperty(navigator, 'clipboard', {
      configurable: true,
      value: {
        readText: () => call('readClipboard'),
        writeText: (text) => call('writeClipboard', String(text ?? '')),
      },
    });
  } else if (navigator.clipboard && window.bridge) {
    // 优先用原生，不行再 fallback 到桥
    const __origRead = navigator.clipboard.readText?.bind(navigator.clipboard);
    const __origWrite = navigator.clipboard.writeText?.bind(navigator.clipboard);
    Object.defineProperty(navigator, 'clipboard', {
      configurable: true,
      value: {
        readText: async () => {
          try { return await __origRead(); } catch { return call('readClipboard'); }
        },
        writeText: async (text) => {
          try { await __origWrite(text); return; } catch { await call('writeClipboard', String(text ?? '')); }
        },
      },
    });
  }

  // -------------------- 拦截 a.click() 下载 --------------------
  // 原 HTML 里很多用 a.click() 触发下载，WebView2 拦截。改写到 Bridge。
  const __origClick = HTMLAnchorElement.prototype.click;
  HTMLAnchorElement.prototype.click = function () {
    if (this.download && this.href && window.bridge) {
      try {
        const url = new URL(this.href, location.href);
        // blob: / data: 协议
        if (url.protocol === 'blob:' || url.protocol === 'data:') {
          fetch(this.href).then(r => r.blob()).then(blob => {
            const reader = new FileReader();
            reader.onload = async () => {
              await call('saveBinary', reader.result, this.download);
            };
            reader.readAsDataURL(blob);
          }).catch(err => console.error('save download failed', err));
          return;
        }
      } catch (e) {
        console.error('a.click intercept', e);
      }
    }
    return __origClick.call(this);
  };

  // -------------------- 剪贴板图片粘贴（核心新功能） --------------------
  document.addEventListener('paste', async (e) => {
    if (!e.clipboardData || !window.bridge) return;
    // 已经有 textarea 内部的 paste，不要拦截
    const items = e.clipboardData.items;
    for (let i = 0; i < items.length; i++) {
      const item = items[i];
      if (item.kind === 'file' && item.type && item.type.startsWith('image/')) {
        e.preventDefault();
        e.stopPropagation();
        const file = item.getAsFile();
        if (!file) return;
        const reader = new FileReader();
        reader.onload = async () => {
          try {
            const ext = (file.type.split('/')[1] || 'png').replace('jpeg', 'jpg');
            const name = `pasted-${Date.now()}.${ext}`;
            const result = await call('saveImage', reader.result, name);
            if (result && result.relativePath) {
              insertImageAtCaret(file.name || 'pasted-image', result.relativePath);
              if (window.showToast) window.showToast('图片已插入', 'success');
            }
          } catch (err) {
            console.error('image paste failed', err);
            if (window.showToast) window.showToast('图片插入失败: ' + err.message, 'error');
          }
        };
        reader.readAsDataURL(file);
        return; // 只处理第一张
      }
    }
  }, true); // 捕获阶段

  // 在光标位置插入 markdown 图片
  function insertImageAtCaret(alt, relPath) {
    const editor = document.getElementById('editor');
    if (!editor) return;
    editor.focus();
    const start = editor.selectionStart;
    const end = editor.selectionEnd;
    const md = `![${alt}](${relPath})`;
    const before = editor.value.substring(0, start);
    const after = editor.value.substring(end);
    const needsNewlineBefore = before.length > 0 && !before.endsWith('\n');
    const insert = (needsNewlineBefore ? '\n' : '') + md + (after.length === 0 || after.startsWith('\n') ? '' : '\n');
    editor.value = before + insert + after;
    const newPos = (before + insert).length;
    editor.selectionStart = editor.selectionEnd = newPos;
    // 触发 input 事件让监听器更新预览
    editor.dispatchEvent(new Event('input', { bubbles: true }));
  }

  // -------------------- 启动信息 --------------------
  if (window.bridge && window.chrome?.webview) {
    window.__md_bridge_ready = true;
    // 让 C# 知道 JS 端就绪
    call('getStartupInfo').then(info => {
      window.__md_startup = info;
    }).catch(() => {});
  }
})();
