// =============================================================================
//  Inkwell — A single-file Markdown editor for Windows
//  Version: Ver 0.11a (first public release)
//  Original release: Ver 0.10
//  Author:  Gavin (gavin.zhang815@gmail.com)
//  License: MIT — see LICENSE in the repository root
//  Repo:    https://github.com/gavin-new/Inkwell
// =============================================================================

using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Inkwell;

public sealed class MainForm : Form
{
    private readonly WebView2 _webView = null!;
    private ApiBridge? _bridge;
    public string? InitialFilePath { get; }

    private const string VirtualHost = "app.local";
    // V0.12: wwwroot 放到 %LocalAppData%\Inkwell\wwwroot（不污染 EXE 所在目录，
    //        避免绿色版 / 便携版用户在桌面上看到一堆解压文件）
    private static readonly string AppDataDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Inkwell");
    private static readonly string WwwRoot = Path.Combine(AppDataDir, "wwwroot");

    // 资源版本号：版本变化时强制重新释放
    // V0.11a: 12（关闭工作区按钮）
    // V0.15:  16（磁吸卡片布局 + 状态栏统计；13-15 为排查编码损坏的中间版本，
    //         实际根因是 editor.html 源文件被以 ANSI 误读后重存，与解压无关）
    // V0.15a: 17（JS 修复：confirm/prompt 异步化后所有调用点补 await；
    //         下拉菜单 z-index，避免被卡片遮挡）
    // V0.15b: 18（移除「视图」菜单改为全屏按钮；状态栏磁吸卡开关居中并强化样式）
    // V0.15c: 19（列表按钮修复：占位文字按类型显示；有序列表自动编号并延续上一行序号）
    // V0.15d: 20（工具栏移除「无序列表」按钮，保留有序列表与任务列表）
    // V0.15e: 21（本地图片预览：docs.local 虚拟域映射文档目录，相对路径图片可显示）
    //         22（改用桥接读取：虚拟主机动态重映射不生效，改 readResource→dataURL 方案）
    // V0.15f: 23（保存流程修复：欢迎文档/清空文档时清掉残留路径，新文档保存必弹另存为；
    //         状态栏左侧显示文档路径，点击可在资源管理器中定位）
    private const string ResourcesVersion = "23";
    private const string VersionFile = ".version";
    private const string WwwRootZipResource = "Inkwell.wwwroot.zip";

    public MainForm(string? initialFile)
    {
        InitialFilePath = initialFile;

        Text = "Inkwell Ver 0.11a";
        Width = 1280;
        Height = 800;
        MinimumSize = new Size(720, 480);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(251, 251, 253);

        // 图标（从嵌入资源释放到临时目录，然后加载）
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream("Inkwell.Resources.app.ico");
            if (stream != null)
            {
                string tmpIco = Path.Combine(Path.GetTempPath(), $"md-editor-{Guid.NewGuid():N}.ico");
                using (var fs = File.Create(tmpIco)) stream.CopyTo(fs);
                Icon = new Icon(tmpIco);
            }
        }
        catch { }

        _webView = new WebView2
        {
            Dock = DockStyle.Fill,
        };
        Controls.Add(_webView);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        string logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Inkwell");
        Directory.CreateDirectory(logDir);
        string logPath = Path.Combine(logDir, "startup.log");
        void Log(string msg) { try { File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\n"); } catch { } }

        try
        {
            Log("=== Starting ===");

            // 0. 注册 CodePages 编码 provider
            EncodingRegistration.EnsureRegistered();

            // 1. 释放 wwwroot 资源
            ExtractEmbeddedWwwRoot();
            Log($"WwwRoot: {WwwRoot}, exists={Directory.Exists(WwwRoot)}");
            if (Directory.Exists(WwwRoot))
            {
                var files = Directory.GetFiles(WwwRoot, "*", SearchOption.AllDirectories);
                Log($"Files: {files.Length}");
            }

            // 2. WebView2 用户数据目录
            string userDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Inkwell", "WebView2");
            Directory.CreateDirectory(userDataDir);
            Log($"UserDataDir: {userDataDir}");

            // 3. 创建环境
            Log("Creating WebView2 environment...");
            var env = await CoreWebView2Environment.CreateAsync(
                null, userDataDir,
                new CoreWebView2EnvironmentOptions
                {
                    AdditionalBrowserArguments = "--disable-features=msSmartScreenProtection --no-sandbox",
                });
            Log("Environment created");

            await _webView.EnsureCoreWebView2Async(env);
            Log("CoreWebView2 initialized");

            // 4. 虚拟主机映射
            if (Directory.Exists(WwwRoot))
            {
                _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    VirtualHost, WwwRoot,
                    CoreWebView2HostResourceAccessKind.Allow);
                Log($"Virtual host '{VirtualHost}' -> {WwwRoot}");
            }
            else
            {
                Log("WARNING: WwwRoot not found!");
            }

            // 5. 桥接
            _bridge = new ApiBridge(_webView.CoreWebView2, this);
            _webView.CoreWebView2.WebMessageReceived += _bridge.OnWebMessageReceived;
            _webView.CoreWebView2.WindowCloseRequested += OnWindowCloseRequested;
            Log("Bridge attached");

            // 6. 加载 HTML
            string url = $"https://{VirtualHost}/editor.html";
            Log($"Navigating to {url}");
            _webView.CoreWebView2.Navigate(url);
            Log("Navigate called");
        }
        catch (Exception ex)
        {
            Log($"ERROR: {ex}");
            MessageBox.Show(this,
                $"启动失败：{ex.Message}\n\n{ex.StackTrace}\n\n请确认系统已安装 WebView2 Runtime。\n下载地址：https://developer.microsoft.com/microsoft-edge/webview2/",
                "Markdown 编辑器", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 把嵌入到 EXE 的 wwwroot.zip 解压到 %LocalAppData%\Inkwell\wwwroot\。
    /// 首次启动稍慢（解压），后续启动毫秒级（版本号命中跳过）。
    /// V0.12: 不再解压到 EXE 同目录（避免污染桌面/绿色版目录）
    /// </summary>
    private static void ExtractEmbeddedWwwRoot()
    {
        Directory.CreateDirectory(AppDataDir);
        string versionFile = Path.Combine(WwwRoot, VersionFile);
        if (File.Exists(versionFile) && File.ReadAllText(versionFile).Trim() == ResourcesVersion)
            return;

        var asm = Assembly.GetExecutingAssembly();
        using var stream = asm.GetManifestResourceStream(WwwRootZipResource)
            ?? throw new InvalidOperationException("找不到 wwwroot 资源");

        if (Directory.Exists(WwwRoot))
        {
            try { Directory.Delete(WwwRoot, recursive: true); } catch { }
        }
        Directory.CreateDirectory(WwwRoot);

        using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
        foreach (var entry in zip.Entries)
        {
            // 跳过目录条目
            if (string.IsNullOrEmpty(entry.Name)) continue;

            string outPath = Path.Combine(WwwRoot, entry.FullName);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
            entry.ExtractToFile(outPath, overwrite: true);
        }

        File.WriteAllText(versionFile, ResourcesVersion);
    }

    private async void OnWindowCloseRequested(object? sender, dynamic e)
    {
        try
        {
            var deferral = e.GetDeferral();
            var json = await _webView.CoreWebView2.ExecuteScriptAsync(
                "window.__md_isDirty ? window.__md_isDirty() : false");
            bool dirty = json.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);

            if (dirty)
            {
                var r = MessageBox.Show(this,
                    "当前文档有未保存的修改，是否保存？",
                    "Markdown 编辑器",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (r == DialogResult.Cancel)
                {
                    deferral.Complete();
                    return;
                }
                if (r == DialogResult.Yes)
                {
                    await _webView.CoreWebView2.ExecuteScriptAsync("window.__md_save && window.__md_save()");
                }
            }
            deferral.Complete();
        }
        catch
        {
            try { e.GetDeferral().Complete(); } catch { }
        }
    }

    public void SetWindowTitle(string? fileName = null)
    {
        if (string.IsNullOrEmpty(fileName))
            Invoke(() => Text = "Inkwell Ver 0.11a");
        else
            Invoke(() => Text = $"{Path.GetFileName(fileName)} - Inkwell");
    }
}
