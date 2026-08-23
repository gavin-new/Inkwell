// 简洁干净的 i18n（去除原作者信息，保留核心双语）
const i18n = {
  'zh-CN': {
    // 品牌
    brand: 'Inkwell',
    brandSubtitle: '简洁优雅的写作工具',
    filenameTitle: '文件名',
    filenameDefault: '未命名文档.md',
    editorPlaceholder: '开始输入 Markdown...',
    previewSourcePlaceholder: '在此编辑 Markdown 源码...',
    editorPaneTitle: '编辑',
    editorPaneHint: '支持 Markdown 语法',
    previewPaneTitle: '实时预览',
    previewTab: '预览',
    sourceTab: '源码',

    // 菜单（顶栏）
    menuFile: '文件',
    menuEdit: '编辑',
    menuInsert: '插入',
    menuView: '视图',
    menuHelp: '帮助',
    menuFormat: '格式',
    menuHeading: '标题',
    menuList: '列表',
    menuExportAs: '导出为',
    menuAbout: '关于',
    menuToggleTheme: '切换主题',

    // 文件操作
    fileNew: '新建文档',
    fileOpen: '打开文件…',
    fileRecent: '最近文档',
    fileSave: '保存',
    fileSaveAs: '另存为…',
    filePrint: '打印…',
    importBtn: '导入',
    exportBtn: '导出',
    clear: '清空文档',

    // 编辑
    menuUndo: '撤销',
    menuRedo: '重做',
    menuFind: '查找',
    menuReplace: '替换',
    menuSelectAll: '全选',
    menuCut: '剪切',
    menuCopy: '复制',
    menuPaste: '粘贴',
    menuCopyMd: '复制为 Markdown',
    menuCopyHtml: '复制为 HTML',
    menuClearFormat: '清除格式',

    // 格式
    save: '保存',
    boldTitle: '加粗',
    italicTitle: '斜体',
    underlineTitle: '下划线',
    strikethroughTitle: '删除线',
    subscriptTitle: '下标',
    superscriptTitle: '上标',
    inlineCode: '行内代码',
    codeBlock: '代码块',
    link: '链接',
    image: '图片',
    table: '表格',
    find: '查找',
    mermaid: 'Mermaid',

    // 标题
    headingH1: '一级标题',
    headingH2: '二级标题',
    headingH3: '三级标题',
    headingH4: '四级标题',
    headingH5: '五级标题',
    headingH6: '六级标题',
    quote: '引用',
    unordered: '无序列表',
    ordered: '有序列表',
    task: '任务列表',

    // 视图
    view: '视图',
    viewBoth: '编辑 + 预览',
    viewEdit: '仅编辑',
    viewPreview: '仅预览',
    pageFullscreen: '页面全屏',
    systemFullscreen: '系统全屏',
    theme: '主题',

    // 状态 / 提示
    langLabel: '语言',
    webToMd: '网页转 MD',
    help: '帮助',
    helpTitle: '使用帮助',
    helpOk: '知道了',
    saved: '已保存',
    autosaveEnabled: '自动保存已启用',
    statusShortcuts: 'Ctrl+S 保存 · Ctrl+B 加粗 · Ctrl+I 斜体 · Ctrl+U 下划线 · Ctrl+Z 撤销 · Ctrl+Y 重做',

    // 导出
    exportMd: 'Markdown (.md)',
    exportWord: 'Word (.doc)',
    exportPdf: 'PDF（打印另存为）',
    exportHtml: 'HTML (.html)',
    exportImage: '图片 (.png)',

    // 模态框
    urlTitle: '网页转 Markdown',
    urlLabel: '网页地址',
    urlPlaceholder: 'https://example.com/article',
    useProxy: '使用本地代理（推荐，可解决知乎/微信公众号）',
    proxyPlaceholder: '本地代理地址',
    fetchBtn: '尝试获取',
    cancel: '取消',
    convertInsert: '转换并插入',
    manualLabel: '网页 HTML 源码（手动粘贴）',
    manualPlaceholder: '在浏览器中打开目标网页 → 右键「查看网页源代码」→ 全选复制 → 粘贴到此处',
    findTitle: '查找与替换',
    findLabel: '查找',
    findPlaceholder: '要查找的内容',
    replaceLabel: '替换为',
    replacePlaceholder: '替换后的内容',
    replaceAll: '全部替换',
    replaceOne: '替换',
    findNext: '查找下一个',
    exportImageTitle: '导出图片',
    ratioLabel: '分享比例',
    ratio9_16: '手机竖屏/故事',
    ratio4_5: '小红书/IG',
    ratio3_4: '竖图',
    ratio1_1: '方形',
    ratio16_9: '横图',
    cropLabel: '过长时裁剪为固定比例（默认生成长图）',
    previewLabel: '预览',
    exportImageHint: '内容较长时将按内容高度生成长图，方便手机阅读。',
    exportImagePreviewAlt: '导出预览',
    refreshPreview: '刷新预览',
    downloadPng: '下载 PNG',
    imageTitle: '插入图片',
    imageTabUrl: '链接',
    imageTabUpload: '本地上传',
    imageUrlLabel: '图片链接',
    imageUrlPlaceholder: 'https://example.com/image.png',
    imageAltLabel: '描述文字',
    imageAltPlaceholder: '图片描述',
    uploadArea: '点击选择或拖拽图片到此处',
    imageHint: '本地图片将以 Base64 嵌入文档，文件过大可能影响性能和保存。',
    insert: '插入',
    mermaidTitle: '插入 Mermaid 图表',
    mermaidTypeLabel: '图表类型',
    mermaidTypeMindmap: '思维导图',
    mermaidTypeFlowchart: '流程图',
    mermaidCodeLabel: 'Mermaid 源码',
    mermaidCodePlaceholder: 'mindmap\n  root((主题))\n    子主题 A\n    子主题 B',
    mermaidHint: '支持 Mermaid 语法，插入后将在预览区自动渲染。',

    // 数字
    wordCount: '{0} 字',
    tableSizeLabel: '{0} 行 × {1} 列',
    statusReplacedCount: '已替换 {0} 处',
    imageLargeWarning: '图片大小 {0}MB 较大，确定要插入吗？',

    // 弹窗
    confirmClear: '确定要清空当前文档吗？此操作不可撤销。',
    confirmNew: '当前文档有内容，确定要新建文档吗？',
    promptLinkUrl: '输入链接地址：',
    promptLinkDefault: 'https://',

    // 按钮提示
    dropMessage: '释放以打开文件或插入图片',
    expandEditor: '展开编辑区',
    collapseEditor: '收起编辑区',
    expandPreview: '展开预览区',
    collapsePreview: '收起预览区',

    // 状态消息
    urlStatusEmptyUrl: '请输入网页地址',
    urlStatusFetching: '正在获取网页内容…',
    urlStatusLocalSuccess: '✓ 通过本地代理获取成功',
    urlStatusLocalFailed: '✗ 本地代理获取失败：{0}',
    urlStatusPublicSuccess: '✓ 已通过公共代理获取',
    urlStatusPublicFailed: '✗ 所有代理都失败了：{0}',
    statusNoMatch: '未找到匹配项',
    statusFoundMatch: '✓ 已找到',
    aboutVersion: 'Inkwell Ver 0.11a · 3 列 5 区 + AI 大模型 + AI 编辑选区',
    aboutDesc: '一款简洁优雅的 Markdown 编辑器，支持实时预览、数学公式、Mermaid 图表，并可将文档导出为 Markdown、HTML、Word、PDF 或图片。支持 HTTP→MD 网页转换、25+ 文本格式、多厂商 AI 对话（OpenAI / DeepSeek / 智谱 / 通义 / Kimi / Ollama）。',

    // V0.11: 文件树 + 大纲
    fileTree: '文件',
    fileTreeEmpty: '点击 📁 打开文件夹',
    refresh: '刷新',
    openFolder: '打开文件夹',
    closeWorkspace: '关闭工作区',
    collapsePane: '折叠面板',
    currentFile: '当前文件',
    outline: '大纲',
    outlineEmpty: '输入标题（# 开头）后自动生成',

    // V0.13: AI 大模型（多厂商，OpenAI 协议 + Ollama 协议）
    aiPaneTitle: 'AI 大模型',
    aiDisconnected: '未配置',
    aiConnected: '已配置',
    aiError: '连接失败',
    aiSelectModel: '选择模型',
    aiModels: '个模型',
    aiChatEmpty: '输入内容与 AI 模型对话（润色 / 翻译 / 续写）',
    aiInputPlaceholder: '问 AI... (Shift+Enter 换行 / Enter 发送)',
    send: '发送',
    sending: '发送中...',
    clearChat: '清空对话',
    aiRoleUser: '你',
    aiRoleAssistant: 'AI',
    aiSelectModelFirst: '请先在顶部选择模型',

    // V0.13: AI 设置面板
    aiSettings: 'AI 大模型设置',
    aiSettingsTitle: 'AI 大模型设置',
    aiProvider: '服务商（Provider）',
    aiProviderHint: '选服务商会自动填上 baseUrl 和默认模型。OpenAI 协议覆盖 80% 厂商。',
    aiApiUrl: 'API 端点（Base URL）',
    aiApiUrlHint: '填服务商的基础 URL，OpenAI 协议通常以 <code>/v1</code> 结尾。选"Ollama"会写 <code>http://localhost:11434/v1</code>。',
    aiApiKey: 'API Key',
    aiApiKeyHint: '必填（除本地 Ollama）。OpenAI 用 <code>sk-...</code>，智谱用 <code>{id}.{secret}</code>，各家格式不同看官方文档。',
    aiModel: '模型名',
    aiTemperature: '温度（0-2）',
    aiSystemPrompt: '系统提示词（System Prompt）',
    aiSystemPromptPlaceholder: '你是一位严谨的写作助手，擅长中英文翻译、润色和续写。',
    aiCustomHeaders: '自定义请求头（JSON，可选）',
    aiCustomHeadersPlaceholder: '{"X-Custom-Header": "value"}',
    aiCustomHeadersHint: '高级选项，标准 JSON 格式。用于需要特殊 Header 的代理服务。',
    aiTestBtn: '测试连接',
    aiReset: '恢复默认',
    aiConfigSaved: '已保存配置',
    aiConfigReset: '已恢复默认配置',
    aiConfigResetConfirm: '确定要恢复 AI 默认配置吗？',
    aiConfigFile: '配置文件：',
    aiConfigFileHint: '改动后立即保存到该路径。可用任意编辑器直接修改（JSON 格式）。',

    // V0.14: AI 编辑选区（润色 / 翻译 / 续写 / 总结）
    aiEdit: 'AI',
    aiPolish: '✨ 润色选区',
    aiTranslate: '🌐 翻译（中↔英）',
    aiContinue: '📝 续写',
    aiSummarize: '📋 总结',

    // 通知
    toastSaved: '✓ 已保存',
    toastNewDoc: '已新建文档',
    toastExported: '已导出',
    toastWordExported: '已导出 Word',
    toastHtmlExported: '已导出 HTML',
    toastCopied: '已复制',
    toastCut: '已剪切',
    toastMdCopied: '已复制 Markdown 源码',
    toastHtmlCopied: '已复制 HTML 源码',
    toastClipboardEmpty: '剪贴板为空',
    toastSelectFirst: '请先选中要清除格式的内容',
    confirm: '确认',
    input: '输入',
    fileRecent: '最近文档',
    fileRecentEmpty: '暂无最近文档',
    toastUndone: '已撤销',
    toastRedone: '已重做',
    toastPageFullscreenOn: '已进入页面全屏',
    toastPageFullscreenOff: '已退出页面全屏',
    toastNoFullscreenApi: '当前浏览器不支持系统全屏 API',
    toastFileImported: '已打开文件',
    toastImageInserted: '已插入图片',
    toastDropUnsupported: '不支持的文件类型',
    toastImageLibMissing: '图片导出库未加载',
    toastPreviewGenerated: '预览已生成',
    toastImageGenFailed: '图片生成失败：{0}',
    toastImageDownloaded: '图片已下载',
    toastGeneratePreviewFirst: '请先生成预览',
    toastSelectImageFile: '请选择图片文件',
    toastImageTooLarge: '图片超过 5MB，无法插入',
    toastImageReadFailed: '读取图片失败',
    toastEnterImageUrl: '请输入图片链接',
    toastSelectImageFirst: '请先选择图片',
    toastMermaidEmpty: 'Mermaid 源码不能为空',
    toastMermaidInserted: '已插入 Mermaid 图表',
    toastChoosePdf: '请在打印对话框选择"另存为 PDF"',
    toastNoContent: '没有可转换的内容',
    toastExtractFailed: '未能从 HTML 中提取到正文',
    toastConvertFailed: '转换失败：{0}',
    toastInsertedMd: '已转换为 Markdown 并插入',

    // 欢迎文档
    welcomeDoc: `# 欢迎使用 Inkwell

一款简洁优雅的 Markdown 编辑器，支持实时预览、数学公式、Mermaid 图表与多格式导出。

## 快速开始

- **左侧编辑，右侧实时预览**
- 使用顶部菜单快速插入格式、文件、视图设置
- 在编辑器中**右键**可呼出快捷菜单
- 拖入文件即可打开，支持 .md / .markdown / .txt

## 格式示例

### 文字格式

**加粗**、*斜体*、~~删除线~~、<u>下划线</u>、\`行内代码\`

### 列表

- 无序列表项
- 另一个项目
  - 嵌套子项
- [x] 已完成任务
- [ ] 未完成任务

### 引用

> 这是一段引用文本，可以用来强调某些内容。
> 支持多行引用。

### 代码块

\`\`\`javascript
function hello() {
  console.log('Hello, Inkwell!');
}
\`\`\`

### 数学公式

行内公式：$E = mc^2$

块级公式：

$$
\\int_{-\\infty}^{\\infty} e^{-x^2} dx = \\sqrt{\\pi}
$$

### Mermaid 图表

\`\`\`mermaid
mindmap
  root((Markdown))
    写作
      标题
      列表
      引用
    代码
      行内
      块级
    图表
      Mermaid
\`\`\`

### 表格

| 功能 | 快捷键 | 说明 |
| --- | --- | --- |
| 加粗 | Ctrl+B | **B**old |
| 斜体 | Ctrl+I | *I*talic |
| 链接 | Ctrl+K | 插入链接 |

---

按 \`Ctrl+S\` 保存，按顶部"文件"菜单可导出为多种格式。祝写作愉快！`,

    helpHtml: `<p>这是一款即开即用的 Markdown 编辑器：左侧写作，右侧实时预览。内容会自动保存到本地。</p>
      <p><b>快速上手</b></p>
      <ul>
        <li>在左侧输入 Markdown，右侧同步渲染。</li>
        <li>使用顶部菜单（文件 / 编辑 / 插入 / 视图 / 帮助）完成所有操作。</li>
        <li>在编辑区<strong>右键</strong>可呼出快捷菜单，剪贴板、格式操作一应俱全。</li>
      </ul>
      <p><b>常用快捷键</b>：<code>Ctrl+S</code> 保存，<code>Ctrl+B</code> 加粗，<code>Ctrl+I</code> 斜体，<code>Ctrl+U</code> 下划线，<code>Ctrl+K</code> 链接，<code>Ctrl+F</code> 查找，<code>Ctrl+H</code> 替换，<code>Ctrl+Z</code> 撤销，<code>Ctrl+Y</code> 重做。</p>
      <p><b>AI 大模型</b>：右下角面板可与 OpenAI / DeepSeek / 智谱 GLM / 通义千问 / Kimi / 本地 Ollama 等多家厂商对话。点击齿轮按钮配置 API Key 和模型名。</p>
      <p><b>导出</b>：支持 Markdown、HTML、Word、PDF（通过浏览器打印）、图片（长图 / 方形 / 竖图 / 横图）。</p>
      <p style="color:var(--text-muted);font-size:12px;">简洁优雅的 Markdown 编辑器，支持实时预览、公式、图表与多格式导出。</p>`
  },

  'en': {
    brand: 'Inkwell',
    brandSubtitle: 'A clean writing tool',
    filenameTitle: 'Filename',
    filenameDefault: 'untitled.md',
    editorPlaceholder: 'Start typing Markdown...',
    previewSourcePlaceholder: 'Edit Markdown source here...',
    editorPaneTitle: 'Editor',
    editorPaneHint: 'Markdown supported',
    previewPaneTitle: 'Live Preview',
    previewTab: 'Preview',
    sourceTab: 'Source',

    // V0.13: AI (multi-provider, OpenAI-compatible + Ollama)
    fileTree: 'Files',
    fileTreeEmpty: 'Click 📁 to open a folder',
    refresh: 'Refresh',
    openFolder: 'Open folder',
    closeWorkspace: 'Close workspace',
    collapsePane: 'Collapse pane',
    outline: 'Outline',
    outlineEmpty: 'Type # headings to generate outline',
    currentFile: 'Current file',
    aiPaneTitle: 'AI Chat',
    aiDisconnected: 'Not configured',
    aiConnected: 'Configured',
    aiError: 'Connection failed',
    aiSelectModel: 'Select model',
    aiModels: 'models',
    aiChatEmpty: 'Chat with AI to polish / translate / continue your text',
    aiInputPlaceholder: 'Ask AI... (Shift+Enter newline / Enter to send)',
    send: 'Send',
    sending: 'Sending...',
    clearChat: 'Clear chat',
    aiRoleUser: 'You',
    aiRoleAssistant: 'AI',
    aiSelectModelFirst: 'Please select a model first',

    // V0.13: AI Settings Panel
    aiSettings: 'AI Settings',
    aiSettingsTitle: 'AI Settings',
    aiProvider: 'Provider',
    aiProviderHint: 'Pick a provider to auto-fill baseUrl and default model. OpenAI-compatible protocol covers 80% of vendors.',
    aiApiUrl: 'API Endpoint (Base URL)',
    aiApiUrlHint: 'Provider base URL. OpenAI-compatible usually ends with <code>/v1</code>. "Ollama" uses <code>http://localhost:11434/v1</code>.',
    aiApiKey: 'API Key',
    aiApiKeyHint: 'Required (except local Ollama). OpenAI uses <code>sk-...</code>, Zhipu uses <code>{id}.{secret}</code>. Check vendor docs.',
    aiModel: 'Model name',
    aiTemperature: 'Temperature (0-2)',
    aiSystemPrompt: 'System Prompt',
    aiSystemPromptPlaceholder: 'You are a rigorous writing assistant, skilled in translation, polishing and continuation.',
    aiCustomHeaders: 'Custom Headers (JSON, optional)',
    aiCustomHeadersPlaceholder: '{"X-Custom-Header": "value"}',
    aiCustomHeadersHint: 'Advanced. Standard JSON. Use for proxy services requiring special headers.',
    aiTestBtn: 'Test connection',
    aiReset: 'Reset to defaults',
    aiConfigSaved: 'Configuration saved',
    aiConfigReset: 'Configuration reset',
    aiConfigResetConfirm: 'Reset AI configuration to defaults?',
    aiConfigFile: 'Config file:',
    aiConfigFileHint: 'Saved to this path on every change. You can also edit the JSON directly with any editor.',

    // V0.14: AI Edit Selection (polish / translate / continue / summarize)
    aiEdit: 'AI',
    aiPolish: '✨ Polish selection',
    aiTranslate: '🌐 Translate (ZH↔EN)',
    aiContinue: '📝 Continue writing',
    aiSummarize: '📋 Summarize',

    menuFile: 'File',
    menuEdit: 'Edit',
    menuInsert: 'Insert',
    menuView: 'View',
    menuHelp: 'Help',
    menuFormat: 'Format',
    menuHeading: 'Heading',
    menuList: 'List',
    menuExportAs: 'Export as',
    menuAbout: 'About',
    menuToggleTheme: 'Toggle theme',

    fileNew: 'New',
    fileOpen: 'Open file…',
    fileRecent: 'Recent',
    fileSave: 'Save',
    fileSaveAs: 'Save as…',
    filePrint: 'Print…',
    importBtn: 'Import',
    exportBtn: 'Export',
    clear: 'Clear',

    menuUndo: 'Undo',
    menuRedo: 'Redo',
    menuFind: 'Find',
    menuReplace: 'Replace',
    menuSelectAll: 'Select all',
    menuCut: 'Cut',
    menuCopy: 'Copy',
    menuPaste: 'Paste',
    menuCopyMd: 'Copy as Markdown',
    menuCopyHtml: 'Copy as HTML',
    menuClearFormat: 'Clear formatting',

    save: 'Save',
    boldTitle: 'Bold',
    italicTitle: 'Italic',
    underlineTitle: 'Underline',
    strikethroughTitle: 'Strikethrough',
    subscriptTitle: 'Subscript',
    superscriptTitle: 'Superscript',
    inlineCode: 'Inline code',
    codeBlock: 'Code block',
    link: 'Link',
    image: 'Image',
    table: 'Table',
    find: 'Find',
    mermaid: 'Mermaid',

    headingH1: 'Heading 1',
    headingH2: 'Heading 2',
    headingH3: 'Heading 3',
    headingH4: 'Heading 4',
    headingH5: 'Heading 5',
    headingH6: 'Heading 6',
    quote: 'Quote',
    unordered: 'Bulleted list',
    ordered: 'Numbered list',
    task: 'Task list',

    view: 'View',
    viewBoth: 'Editor + Preview',
    viewEdit: 'Editor only',
    viewPreview: 'Preview only',
    pageFullscreen: 'Page fullscreen',
    systemFullscreen: 'System fullscreen',
    theme: 'Theme',

    langLabel: 'Language',
    webToMd: 'Web to MD',
    help: 'Help',
    helpTitle: 'Help',
    helpOk: 'Got it',
    saved: 'Saved',
    autosaveEnabled: 'Auto-save enabled',
    statusShortcuts: 'Ctrl+S Save · Ctrl+B Bold · Ctrl+I Italic · Ctrl+U Underline · Ctrl+Z Undo · Ctrl+Y Redo',

    exportMd: 'Markdown (.md)',
    exportWord: 'Word (.doc)',
    exportPdf: 'PDF (print)',
    exportHtml: 'HTML (.html)',
    exportImage: 'Image (.png)',

    urlTitle: 'Web to Markdown',
    urlLabel: 'URL',
    urlPlaceholder: 'https://example.com/article',
    useProxy: 'Use local proxy (for Zhihu / WeChat)',
    proxyPlaceholder: 'Local proxy URL',
    fetchBtn: 'Fetch',
    cancel: 'Cancel',
    convertInsert: 'Convert & insert',
    manualLabel: 'HTML source (manual paste)',
    manualPlaceholder: 'Open the page → View Source → Copy → Paste here',
    findTitle: 'Find & Replace',
    findLabel: 'Find',
    findPlaceholder: 'Find what',
    replaceLabel: 'Replace with',
    replacePlaceholder: 'Replacement',
    replaceAll: 'Replace all',
    replaceOne: 'Replace',
    findNext: 'Find next',
    exportImageTitle: 'Export image',
    ratioLabel: 'Aspect ratio',
    ratio9_16: 'Story / vertical',
    ratio4_5: 'IG / XHS',
    ratio3_4: 'Portrait',
    ratio1_1: 'Square',
    ratio16_9: 'Landscape',
    cropLabel: 'Crop to ratio when too tall (default: long image)',
    previewLabel: 'Preview',
    exportImageHint: 'A long image is generated for tall content.',
    exportImagePreviewAlt: 'Preview',
    refreshPreview: 'Refresh',
    downloadPng: 'Download PNG',
    imageTitle: 'Insert image',
    imageTabUrl: 'URL',
    imageTabUpload: 'Upload',
    imageUrlLabel: 'Image URL',
    imageUrlPlaceholder: 'https://example.com/image.png',
    imageAltLabel: 'Alt text',
    imageAltPlaceholder: 'Description',
    uploadArea: 'Click or drop image here',
    imageHint: 'Local images are embedded as Base64.',
    insert: 'Insert',
    mermaidTitle: 'Insert Mermaid',
    mermaidTypeLabel: 'Type',
    mermaidTypeMindmap: 'Mindmap',
    mermaidTypeFlowchart: 'Flowchart',
    mermaidCodeLabel: 'Mermaid code',
    mermaidCodePlaceholder: 'mindmap\n  root((Topic))\n    Sub A\n    Sub B',
    mermaidHint: 'Mermaid syntax supported.',

    wordCount: '{0} chars',
    tableSizeLabel: '{0} rows × {1} cols',
    statusReplacedCount: 'Replaced {0}',
    imageLargeWarning: 'Image is {0}MB. Insert anyway?',

    confirmClear: 'Clear the current document? This cannot be undone.',
    confirmNew: 'Current content will be lost. Create a new document?',
    promptLinkUrl: 'Enter link URL:',
    promptLinkDefault: 'https://',

    dropMessage: 'Drop to open file or insert image',
    expandEditor: 'Expand editor',
    collapseEditor: 'Collapse editor',
    expandPreview: 'Expand preview',
    collapsePreview: 'Collapse preview',

    urlStatusEmptyUrl: 'Please enter a URL',
    urlStatusFetching: 'Fetching…',
    urlStatusLocalSuccess: '✓ Local proxy succeeded',
    urlStatusLocalFailed: '✗ Local proxy failed: {0}',
    urlStatusPublicSuccess: '✓ Public proxy succeeded',
    urlStatusPublicFailed: '✗ All proxies failed: {0}',
    statusNoMatch: 'No match',
    statusFoundMatch: '✓ Found',
    aboutVersion: 'Inkwell Ver 0.11a · 3-column 5-zone + AI + AI edit selection',
    aboutDesc: 'A clean Markdown editor with live preview, math formulas, Mermaid diagrams and multi-format export. Multi-provider AI chat (OpenAI / DeepSeek / Zhipu / Qwen / Kimi / Ollama).',

    toastSaved: '✓ Saved',
    toastNewDoc: 'New document created',
    toastExported: 'Exported',
    toastWordExported: 'Word exported',
    toastHtmlExported: 'HTML exported',
    toastCopied: 'Copied',
    toastCut: 'Cut',
    toastMdCopied: 'Markdown copied',
    toastHtmlCopied: 'HTML copied',
    toastClipboardEmpty: 'Clipboard is empty',
    toastSelectFirst: 'Select content to clear formatting',
    confirm: 'Confirm',
    input: 'Input',
    fileRecent: 'Recent',
    fileRecentEmpty: 'No recent files',
    toastUndone: 'Undone',
    toastRedone: 'Redone',
    toastPageFullscreenOn: 'Page fullscreen on',
    toastPageFullscreenOff: 'Page fullscreen off',
    toastNoFullscreenApi: 'Fullscreen API not supported',
    toastFileImported: 'File opened',
    toastImageInserted: 'Image inserted',
    toastDropUnsupported: 'Unsupported file type',
    toastImageLibMissing: 'Image library not loaded',
    toastPreviewGenerated: 'Preview generated',
    toastImageGenFailed: 'Image generation failed: {0}',
    toastImageDownloaded: 'Image downloaded',
    toastGeneratePreviewFirst: 'Generate preview first',
    toastSelectImageFile: 'Please select an image',
    toastImageTooLarge: 'Image exceeds 5MB',
    toastImageReadFailed: 'Failed to read image',
    toastEnterImageUrl: 'Please enter image URL',
    toastSelectImageFirst: 'Please select an image first',
    toastMermaidEmpty: 'Mermaid code is empty',
    toastMermaidInserted: 'Mermaid inserted',
    toastChoosePdf: 'In the print dialog choose "Save as PDF"',
    toastNoContent: 'Nothing to convert',
    toastExtractFailed: 'Failed to extract content',
    toastConvertFailed: 'Conversion failed: {0}',
    toastInsertedMd: 'Converted and inserted',

    welcomeDoc: `# Welcome to Inkwell

A clean Markdown editor with live preview, math, Mermaid diagrams, multi-format export and multi-provider AI chat.

## Quick start

- **Edit on the left, see live preview on the right**
- Use the top menus (File / Edit / Insert / View / Help)
- **Right-click** the editor for a quick menu
- Drag in a .md / .markdown / .txt file to open

## Features

### Text

**Bold**, *italic*, ~~strikethrough~~, <u>underline</u>, \`inline code\`

### Lists

- Bulleted
- Another
  - Nested
- [x] Done
- [ ] Todo

### Quote

> Multi-line quote.

### Code

\`\`\`javascript
function hello() {
  console.log('Hi!');
}
\`\`\`

### Math

Inline: $E = mc^2$

Block:

$$
\\int_{-\\infty}^{\\infty} e^{-x^2} dx = \\sqrt{\\pi}
$$

### Mermaid

\`\`\`mermaid
mindmap
  root((Markdown))
    Writing
    Code
    Charts
\`\`\`

### Table

| Shortcut | Action |
| --- | --- |
| Ctrl+B | **Bold** |
| Ctrl+I | *Italic* |
| Ctrl+K | Link |

### AI Chat

Use the bottom-right panel to chat with OpenAI, DeepSeek, Zhipu, Qwen, Kimi or a local Ollama model. Click the gear icon to configure.

---

Press \`Ctrl+S\` to save. Enjoy!`,

    helpHtml: `<p>A ready-to-use Markdown editor: write on the left, see live preview on the right. Content is auto-saved locally.</p>
      <p><b>Quick start</b></p>
      <ul>
        <li>Type Markdown on the left; the right side renders in real time.</li>
        <li>Use the top menu (File / Edit / Insert / View / Help) for all actions.</li>
        <li><strong>Right-click</strong> the editor for a quick context menu (clipboard, formatting, etc.).</li>
      </ul>
      <p><b>Shortcuts</b>: <code>Ctrl+S</code> save, <code>Ctrl+B</code> bold, <code>Ctrl+I</code> italic, <code>Ctrl+U</code> underline, <code>Ctrl+K</code> link, <code>Ctrl+F</code> find, <code>Ctrl+H</code> replace, <code>Ctrl+Z</code> undo, <code>Ctrl+Y</code> redo.</p>
      <p><b>AI</b>: the bottom-right panel supports OpenAI, DeepSeek, Zhipu GLM, Qwen, Kimi, or local Ollama. Click the gear to configure your API key and model.</p>
      <p><b>Export</b>: Markdown, HTML, Word, PDF (via browser print), or image (long / square / vertical / landscape).</p>
      <p style="color:var(--text-muted);font-size:12px;">A clean Markdown editor with live preview, math, diagrams and multi-format export.</p>`
  }
};
