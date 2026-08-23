# Inkwell

> **简洁优雅、可扩展的 Markdown 编辑器**
> 单文件 Windows EXE · 3 列 5 区布局 · 内置多厂商 AI 大模型 · 完全离线可用

Inkwell 是一款为写作者打造的 Markdown 编辑器，把"打开就能写"做到极致：
一个 EXE 搞定所有事——不依赖浏览器、不需要登录、不上传你的文档到任何云端。

---

## 📸 主界面

![Inkwell Ver 0.11a](docs/screenshots/v011a-main.png)

---

## ✨ 核心功能

### 📝 写作者友好的编辑器

- **实时预览** — 左右两栏同步渲染，所见即所得
- **多模式预览** — 渲染模式 / 源码模式一键切换
- **大纲面板** — 自动解析 H1-H6 标题，点击跳转，编辑时高亮跟随
- **文件树** — 打开文件夹，整套 Markdown 工程一起管
- **25+ 文件格式** — `.md` `.txt` `.csv` `.py` `.adoc` `.rst` `.org` `.tex` `.js` `.ts` `.json` `.yaml` `.toml` `.xml` `.html` `.css` `.sh` `.ps1` ……

### 🤖 内置 AI 大模型（多厂商）

不止支持 OpenAI，主流厂商开箱即用：

| 服务商 | 适用场景 |
|---|---|
| **OpenAI** | gpt-4o-mini、gpt-4o 等 |
| **DeepSeek** | 国内可用、价格低 |
| **智谱 GLM** | glm-4-flash 等 |
| **通义千问** | qwen-turbo 等 |
| **Kimi 月之暗面** | 长文本 |
| **本地 Ollama** | 隐私优先、零成本 |
| **自定义** | OneAPI / LiteLLM / Open WebUI 等代理 |

**AI 选区编辑** — 选中一段文字 → 一键润色 / 翻译 / 续写 / 总结，AI 流式替换选区，所见即所得。
**AI 自由对话** — 右下角面板，常规聊天、解释概念、问问题都顺手。

### 🎨 视觉设计

- **现代毛玻璃风格** — 系统字体、`#007AFF` 主色、流畅过渡动画
- **浮动卡片布局** — 右侧预览/AI 独立卡片，边界清晰、视觉聚焦
- **明暗双主题** — 跟随系统或手动切换
- **可拖动分隔条** — 5 个区域任意调整比例，记忆到下次启动

### 🔧 工程级细节

- **多编码自动检测** — UTF-8 / UTF-8 BOM / UTF-16 / GB18030 (GBK/GB2312) / Shift-JIS
- **保留行尾符** — 改 Python/JS 文件时 CRLF / LF / 末尾换行 原样保留
- **剪贴板图片粘贴** — `Ctrl+V` 直接插入截图，自动存到 `assets/`
- **25+ 导出格式** — Markdown / Word (.doc) / HTML / PDF（打印） / PNG（长图/方形/竖图/横图）
- **网页转 Markdown** — 公共代理 / 本地代理 / 手动粘贴三种方式

---

## 🏆 优点

### 对比传统 Markdown 工具

| | 传统工具 | Inkwell |
|---|---|---|
| 安装 | 装 Node、VS Code、扩展 | 下载 EXE，双击运行 |
| 体积 | 几百 MB | **3.16 MB** 单文件 |
| 离线 | 部分功能要联网 | **完全离线**（AI 可选） |
| 数据 | 上传云端或浏览器本地存储 | **本地文件**，所见即所得 |
| 隐私 | 笔记/草稿可能被收集 | 永远不离开你的电脑 |

### 对比在线编辑器

- **Typora 替代** — Typora 收费且已停更，Inkwell 免费开源
- **Notion 替代** — 不用注册、不用联网、不用担心内容审核
- **语雀/飞书文档替代** — 数据在自己电脑上，永远可读

### 对比同类 AI 编辑器

- **Cursor 类** — Cursor 强但重、面向代码。Inkwell 轻、面向写作
- **Notion AI 类** — 订阅制、按月收费。Inkwell 一次下载永久用
- **国产 AI 工具** — 普遍锁定自家模型。Inkwell **6 个主流厂商 + 自定义**，不被绑架

---

## 🎯 适合谁用

### ✅ 强烈推荐

- **📝 个人写作者 / 博主** — 写公众号、博客、长文、专栏，需要所见即所得又要保持纯文本可移植
- **📚 学术研究者 / 学生** — 写论文、笔记、读书记录，公式、图表、引用都要
- **💻 程序员** — 写技术文档、README、API 文档，CRLF/LF、代码块、文件名高亮都是刚需
- **📒 知识管理爱好者** — 第二大脑、卡片笔记、Obsidian 用户但想要"开箱即用"

### ✅ 适合

- **🧠 思考者 / 规划者** — 每天写日记、做计划、整理思路的人
- **📋 内容创作者** — 视频脚本、播客大纲、小红书/知乎文案的草稿
- **🗂️ 文档维护者** — 团队/项目的内部文档，需要 Markdown 源文件

### ❌ 不适合

- 需要多人实时协作（Inkwell 是单人本地工具）
- 需要云同步多设备（Inkwell 没有自带云，建议用 Git/Syncthing 自己同步）
- 需要富文本所见即所得编辑（Inkwell 是纯 Markdown，所见即所得只在预览侧）

---

## 🚀 快速开始

### 安装

1. 下载 [dist/Inkwell.exe](dist/Inkwell.exe)（3.16 MB）
2. 双击运行，**第一次**会弹出 .NET 10 / WebView2 Runtime 检测（缺哪个就装哪个，都是一次性的）
3. 开始写

### 系统要求

- Windows 10 1809+ / Windows 11
- .NET 10 Desktop Runtime（约 50 MB，[下载](https://dotnet.microsoft.com/download/dotnet/10.0)）
- WebView2 Runtime（约 100 MB，Win11 必带，[下载](https://go.microsoft.com/fwlink/p/?LinkId=2124703)）

> **完全可选**：AI 大模型需要联网（用 OpenAI / DeepSeek / 智谱 等云端 API），不配也不影响其他功能。

### 配置 AI（可选）

1. 右下角 AI 面板点齿轮 ⚙
2. 选服务商（下拉自动填 baseUrl 和默认模型）
3. 填 API Key → 测试连接 → 保存
4. 配置存到 `~/.Inkwell/ai-config.json`，下次启动自动加载

### 日常使用

| 操作 | 快捷键 |
|---|---|
| 保存 | `Ctrl+S` |
| 加粗 / 斜体 / 下划线 | `Ctrl+B` / `Ctrl+I` / `Ctrl+U` |
| 查找 / 替换 | `Ctrl+F` / `Ctrl+H` |
| 插入链接 / 图片 | `Ctrl+K` / `Ctrl+Shift+K` |
| 撤销 / 重做 | `Ctrl+Z` / `Ctrl+Y` |
| AI 选区润色 | 选中文本 → 工具栏 AI 按钮 |
| AI 自由对话 | 右下角面板 |

---

## 🛠️ 技术栈

- **C# / .NET 10 / WinForms** — 原生 Windows 体验
- **WebView2 (Edge 内核)** — 现代 Web 渲染，无浏览器依赖
- **Web 标准前端** — HTML / CSS / JavaScript
- **marked + KaTeX + Mermaid** — Markdown / 公式 / 图表

**完全开源**（MIT）— 任何人可以审阅、修改、重新分发。

---

## 📦 下载

| 文件 | 大小 | 说明 |
|---|---|---|
| [Inkwell.exe](dist/Inkwell.exe) | 3.16 MB | 单文件 EXE，直接运行 |

---

## 📄 协议

MIT License — 自由使用、修改、商用。

---

**Inkwell Ver 0.11a** — 你的下一篇文章，从这里开始。
