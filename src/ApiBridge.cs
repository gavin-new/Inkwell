// =============================================================================
//  Inkwell — A single-file Markdown editor for Windows
//  Version: Ver 0.11a (first public release)
//  Original release: Ver 0.10
//  Author:  Gavin (gavin.zhang815@gmail.com)
//  License: MIT — see LICENSE in the repository root
//  Repo:    https://github.com/gavin-new/Inkwell
// =============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.Web.WebView2.Core;

namespace Inkwell;

/// <summary>
/// 注册 CodePages 编码 provider（GB18030 / GBK / GB2312 / Shift-JIS / Big5 等）。
/// 必须在使用这些编码前调用一次。
/// </summary>
public static class EncodingRegistration
{
    private static bool _registered;
    public static void EnsureRegistered()
    {
        if (_registered) return;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _registered = true;
    }
}

/// <summary>
/// JS ↔ C# 桥接：处理所有 native 操作。
/// JS 端通过 window.chrome.webview.postMessage({id, method, args}) 调用，
/// C# 端处理后通过 PostMessageAsJson 回复 {id, ok, result|error}。
/// </summary>
internal sealed class ApiBridge
{
    private readonly CoreWebView2 _webView;
    private readonly MainForm _form;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public string? CurrentFilePath { get; set; }

    public ApiBridge(CoreWebView2 webView, MainForm form)
    {
        _webView = webView;
        _form = form;
    }

    public void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        _ = HandleAsync(e);
    }

    private async Task HandleAsync(CoreWebView2WebMessageReceivedEventArgs e)
    {
        int id = -1;
        try
        {
            var raw = e.TryGetWebMessageAsString();
            if (string.IsNullOrEmpty(raw)) return;
            using var doc = JsonDocument.Parse(raw);
            id = doc.RootElement.GetProperty("id").GetInt32();
            var method = doc.RootElement.GetProperty("method").GetString() ?? "";
            var args = doc.RootElement.TryGetProperty("args", out var a) ? a : default;

            object? result = method switch
            {
                "openFile" => await OpenFile(args),
                "openFileDialog" => await OpenFileDialog(),
                "saveFile" => await SaveFile(args),
                "saveAsDialog" => await SaveAsDialog(args),
                "readClipboard" => ReadClipboardText(),
                "writeClipboard" => WriteClipboardText(args),
                "saveImage" => await SaveImage(args),
                "saveBinary" => await SaveBinary(args),
                "readFileWithEncoding" => await ReadFileWithEncoding(args),
                "listDirectory" => ListDirectory(args),
                "chooseFolder" => await ChooseFolder(),
                "pickSavePath" => await PickSavePath(args),
                "pickOpenPath" => await PickOpenPath(args),
                "confirm" => ConfirmDialog(args),
                "alert" => AlertDialog(args),
                "prompt" => PromptDialog(args),
                "getInitialFile" => _form.InitialFilePath,
                "setCurrentFile" => SetCurrentFile(args),
                "readResource" => ReadResource(args),
                "associateFileTypes" => AssociateFileTypes(),
                "openAssociationWizard" => await OpenAssociationWizard(),
                "getStartupInfo" => GetStartupInfo(),
                "setTitle" => SetWindowTitle(args),
                "showInFolder" => ShowInFolder(args),
                "getAiConfig" => GetAiConfig(),
                "saveAiConfig" => SaveAiConfig(args),
                "getAiConfigPath" => GetAiConfigPath(),
                "revealAiConfig" => RevealAiConfigFile(),
                _ => throw new InvalidOperationException($"Unknown method: {method}"),
            };

            PostOk(id, result);
        }
        catch (Exception ex)
        {
            if (id >= 0) PostErr(id, ex.Message);
        }
    }

    private void PostOk(int id, object? result) =>
        _webView.PostWebMessageAsJson(JsonSerializer.Serialize(new { id, ok = true, result }, JsonOpts));

    private void PostErr(int id, string error) =>
        _webView.PostWebMessageAsJson(JsonSerializer.Serialize(new { id, ok = false, error }, JsonOpts));

    // ========== 文件操作 ==========

    private async Task<object?> OpenFile(JsonElement args)
    {
        string path = args.GetString() ?? "";
        if (!File.Exists(path)) throw new FileNotFoundException("文件不存在", path);
        var (content, encoding, lineEnding, hasFinalNewline) = await ReadFileAutoDetectAsync(path);
        CurrentFilePath = path;
        return new { path, content, encoding, lineEnding, hasFinalNewline };
    }

    private async Task<object?> OpenFileDialog()
    {
        string? path = await PickOpenPath(JsonDocument.Parse("[\"所有支持的格式|*.md;*.markdown;*.txt;*.csv;*.py;*.adoc;*.rst;*.org;*.rtf;*.textile;*.mediawiki;*.opml;*.tex;*.js;*.ts;*.json;*.yaml;*.yml;*.toml;*.xml;*.html;*.css;*.sh;*.ps1|Markdown 文件|*.md;*.markdown|纯文本|*.txt|CSV|*.csv|Python|*.py|JavaScript/TypeScript|*.js;*.ts|配置文件|*.json;*.yaml;*.yml;*.toml;*.xml|HTML/CSS|*.html;*.css|脚本|*.sh;*.ps1|AsciiDoc|*.adoc|reStructuredText|*.rst|Org-mode|*.org|RTF|*.rtf|所有文件|*.*\"]").RootElement);
        if (path == null) return null;
        if (!File.Exists(path)) throw new FileNotFoundException("文件不存在", path);
        var (content, encoding, lineEnding, hasFinalNewline) = await ReadFileAutoDetectAsync(path);
        CurrentFilePath = path;
        return new { path, content, encoding, lineEnding, hasFinalNewline };
    }

    private async Task<object?> SaveFile(JsonElement args)
    {
        string path = args[0].GetString() ?? throw new ArgumentException("path required");
        string content = args[1].GetString() ?? "";
        // 行尾符：参数 2 (可选)；末尾换行：参数 3 (可选)
        string lineEnding = args[2].ValueKind == JsonValueKind.String ? args[2].GetString() ?? "lf" : "lf";
        bool ensureFinalNewline = args[3].ValueKind == JsonValueKind.False ? false : true;
        await WriteFileWithFormatAsync(path, content, lineEnding, ensureFinalNewline);
        CurrentFilePath = path;
        return new { path, encoding = "utf-8", lineEnding, hasFinalNewline = ensureFinalNewline };
    }

    private async Task<object?> SaveAsDialog(JsonElement args)
    {
        // 参数：content, suggestedName, lineEnding, hasFinalNewline
        string content = args[0].GetString() ?? "";
        string suggestedName = args[1].ValueKind == JsonValueKind.String ? args[1].GetString() ?? "" : "";
        string lineEnding = args[2].ValueKind == JsonValueKind.String ? args[2].GetString() ?? "lf" : "lf";
        bool ensureFinalNewline = args[3].ValueKind == JsonValueKind.False ? false : true;

        string? path = await PickSavePath(JsonDocument.Parse("[\"所有支持的格式|*.md;*.markdown;*.txt;*.csv;*.py;*.adoc;*.rst;*.org;*.rtf;*.textile;*.mediawiki;*.opml;*.tex;*.js;*.ts;*.json;*.yaml;*.yml;*.toml;*.xml;*.html;*.css;*.sh;*.ps1|Markdown 文件|*.md;*.markdown|纯文本|*.txt|CSV|*.csv|Python|*.py|JavaScript/TypeScript|*.js;*.ts|配置文件|*.json;*.yaml;*.yml;*.toml;*.xml|HTML/CSS|*.html;*.css|脚本|*.sh;*.ps1|AsciiDoc|*.adoc|reStructuredText|*.rst|Org-mode|*.org|RTF|*.rtf|所有文件|*.*\"]").RootElement);
        if (path == null) return null;
        await WriteFileWithFormatAsync(path, content, lineEnding, ensureFinalNewline);
        CurrentFilePath = path;
        return new { path, encoding = "utf-8", lineEnding, hasFinalNewline = ensureFinalNewline };
    }

    /// <summary>
    /// 写入文件，按指定行尾符 + 末尾换行设置。
    /// 编码用 UTF-8（无 BOM）。Python 等代码文件原 CRLF 风格会被保留。
    /// </summary>
    private static async Task WriteFileWithFormatAsync(string path, string content, string lineEnding, bool ensureFinalNewline)
    {
        string outText = ConvertLineEnding(content, lineEnding, ensureFinalNewline);
        // 统一用 UTF-8 无 BOM
        await File.WriteAllTextAsync(path, outText, new UTF8Encoding(false));
    }

    /// <summary>
    /// 自动检测文件编码：
    /// 1) BOM 头（UTF-8 / UTF-16 LE/BE / UTF-32）
    /// 2) 严格 UTF-8 解码（不能失败）
    /// 3) 中文 fallback: GB18030（覆盖 GBK / GB2312）
    /// 4) 全部失败: UTF-8 + 替换错误字符
    /// 返回 (内容, 编码名)
    /// </summary>
    private static async Task<(string content, string encoding, string lineEnding, bool hasFinalNewline)> ReadFileAutoDetectAsync(string path)
    {
        byte[] bytes = await File.ReadAllBytesAsync(path);

        // 先扫一遍字节，记录行尾符 + 末尾是否有换行
        var (lineEnding, hasFinalNewline) = DetectLineEnding(bytes);

        // 1) BOM 头
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return (Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3), "utf-8-bom", lineEnding, hasFinalNewline);
        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            return (Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2), "utf-16le", lineEnding, hasFinalNewline);
        if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            return (Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2), "utf-16be", lineEnding, hasFinalNewline);
        if (bytes.Length >= 4 && bytes[0] == 0xFF && bytes[1] == 0xFE && bytes[2] == 0x00 && bytes[3] == 0x00)
            return (Encoding.UTF32.GetString(bytes, 4, bytes.Length - 4), "utf-32le", lineEnding, hasFinalNewline);

        // 2) 严格 UTF-8 验证
        try
        {
            var strictUtf8 = Encoding.GetEncoding("utf-8",
                new EncoderExceptionFallback(),
                new DecoderExceptionFallback());
            return (strictUtf8.GetString(bytes), "utf-8", lineEnding, hasFinalNewline);
        }
        catch (DecoderFallbackException) { }

        // 3) 中文 fallback: GB18030
        try
        {
            var gb18030 = Encoding.GetEncoding("GB18030");
            // 用严格解码（替换错误字符）以避免异常
            var strictGb = Encoding.GetEncoding("GB18030",
                new EncoderExceptionFallback(),
                new DecoderReplacementFallback("?"));
            string text = strictGb.GetString(bytes);
            // 如果包含太多替换符（>5%），认为不是 GB 系列
            int qm = 0;
            foreach (char c in text) if (c == '?') qm++;
            if (qm < text.Length / 20)
                return (text, "gb18030", lineEnding, hasFinalNewline);
        }
        catch { }

        // 4) 兜底: UTF-8 + 替换
        return (Encoding.UTF8.GetString(bytes), "utf-8-replaced", lineEnding, hasFinalNewline);
    }

    /// <summary>
    /// 扫描字节，检测行尾符风格。
    /// 返回 (lineEnding, hasFinalNewline):
    ///   lineEnding: "crlf" | "lf" | "cr" | "lf+crlf" | "none"
    ///   hasFinalNewline: 文件最后是否以换行符结尾
    /// </summary>
    private static (string lineEnding, bool hasFinalNewline) DetectLineEnding(byte[] bytes)
    {
        if (bytes.Length == 0) return ("none", false);

        int crlf = 0, lf = 0, cr = 0;
        bool finalLf = false, finalCr = false;
        bool finalIsCrlf = false;

        for (int i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            bool isLast = (i == bytes.Length - 1);

            if (b == 0x0D) // \r
            {
                if (i + 1 < bytes.Length && bytes[i + 1] == 0x0A) // \r\n
                {
                    crlf++;
                    if (isLast) { finalIsCrlf = true; } // 不可能，\r 是最后一个
                    i++; // 跳过 \n
                }
                else
                {
                    cr++;
                    if (isLast) finalCr = true;
                }
            }
            else if (b == 0x0A) // \n
            {
                lf++;
                if (isLast) finalLf = true;
            }
        }

        bool endsWithLf = finalLf;
        bool endsWithCr = finalCr;
        bool endsWithCrlf = finalIsCrlf;

        // 行尾符风格：哪种多就用哪种
        string style;
        if (crlf > 0 && lf == 0 && cr == 0) style = "crlf";
        else if (lf > 0 && crlf == 0 && cr == 0) style = "lf";
        else if (cr > 0 && lf == 0 && crlf == 0) style = "cr";
        else if (crlf > 0 || lf > 0 || cr > 0) style = "lf"; // 混合或空 → 默认 lf
        else style = "none";

        bool hasFinal = endsWithLf || endsWithCr || endsWithCrlf;
        return (style, hasFinal);
    }

    /// <summary>
    /// 按指定行尾符风格转换字符串（仅在 lineEnding 不是 "lf" 时调用）。
    /// lineEnding: "crlf" | "lf" | "cr" | "none"
    /// </summary>
    private static string ConvertLineEnding(string text, string lineEnding, bool ensureFinalNewline)
    {
        // 先把所有行尾符规范化成 \n
        string normalized = text.Replace("\r\n", "\n").Replace("\r", "\n");

        // 按目标风格转换
        string result = lineEnding switch
        {
            "crlf" => normalized.Replace("\n", "\r\n"),
            "cr" => normalized.Replace("\n", "\r"),
            _ => normalized, // "lf" 或 "none" 保持 \n
        };

        // 处理末尾换行
        if (ensureFinalNewline && result.Length > 0 && result[^1] != '\n' && result[^1] != '\r')
        {
            result += lineEnding == "crlf" ? "\r\n"
                    : lineEnding == "cr" ? "\r"
                    : "\n";
        }
        else if (!ensureFinalNewline && result.Length > 0)
        {
            // 去掉末尾换行
            if (result.EndsWith("\r\n")) result = result[..^2];
            else if (result.EndsWith("\n") || result.EndsWith("\r")) result = result[..^1];
        }

        return result;
    }

    /// <summary>
    /// 用指定编码读取文件（用于"重新打开（选编码）"）。
    /// encoding: utf-8 | utf-8-bom | utf-16le | utf-16be | gb18030
    /// </summary>
    private async Task<object?> ReadFileWithEncoding(JsonElement args)
    {
        string path = args[0].GetString() ?? throw new ArgumentException("path required");
        string encoding = args[1].GetString() ?? "utf-8";
        if (!File.Exists(path)) throw new FileNotFoundException("文件不存在", path);

        byte[] bytes = await File.ReadAllBytesAsync(path);
        string content;
        int bomOffset = 0;

        Encoding enc = encoding switch
        {
            "utf-8" => new UTF8Encoding(false),
            "utf-8-bom" => Encoding.UTF8,
            "utf-16le" => Encoding.Unicode,
            "utf-16be" => Encoding.BigEndianUnicode,
            "gb18030" => Encoding.GetEncoding("GB18030"),
            _ => Encoding.UTF8,
        };

        // 跳过 BOM
        if (encoding == "utf-8-bom" && bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            bomOffset = 3;
        else if (encoding == "utf-16le" && bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            bomOffset = 2;
        else if (encoding == "utf-16be" && bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            bomOffset = 2;

        content = enc.GetString(bytes, bomOffset, bytes.Length - bomOffset);
        return new { content, encoding };
    }

    /// <summary>
    /// 弹出"选择文件夹"对话框。
    /// 注意：不能在 WebView2 的 WebMessageReceived 回调里直接 ShowDialog ——
    /// 宿主窗口不会被禁用，对话框会落到主窗口后面（表现为"点了没反应"）。
    /// 必须用 BeginInvoke 推迟到桥接消息处理结束后再弹，模态关系才能正确建立。
    /// </summary>
    private async Task<object?> ChooseFolder()
    {
        var tcs = new TaskCompletionSource<string?>();
        _form.BeginInvoke(() =>
        {
            try
            {
                using var dlg = new FolderBrowserDialog
                {
                    Description = "选择工作区文件夹",
                    UseDescriptionForTitle = true,
                    ShowNewFolderButton = false,
                };
                tcs.SetResult(dlg.ShowDialog(_form) == DialogResult.OK ? dlg.SelectedPath : null);
            }
            catch (Exception ex) { tcs.SetException(ex); }
        });
        return await tcs.Task;
    }

    /// <summary>
    /// 列出目录树（用于左侧文件树 UI）
    /// </summary>
    private object ListDirectory(JsonElement args)
    {
        // 兼容两种调用：bridge.call('listDirectory', path) → args 是 String
        // 旧调用：bridge.call('listDirectory', [path]) → args 是 Array (length 1)
        string rootPath = args.ValueKind == JsonValueKind.String
            ? args.GetString() ?? ""
            : (args.GetArrayLength() > 0 ? args[0].GetString() ?? "" : "");
        if (string.IsNullOrEmpty(rootPath)) throw new ArgumentException("path required");
        if (!Directory.Exists(rootPath)) throw new DirectoryNotFoundException("目录不存在");

        var tree = BuildDirTree(rootPath, maxDepth: 8);
        return new { root = rootPath, tree };
    }

    private static object BuildDirTree(string path, int maxDepth, int currentDepth = 0)
    {
        var dirInfo = new DirectoryInfo(path);
        // 跳过的目录
        var skipDirs = new[] { ".git", "node_modules", ".vs", ".idea", "bin", "obj", ".vscode", "dist", "build" };
        var name = dirInfo.Name;
        if (currentDepth == 0) name = dirInfo.FullName; // 根显示完整路径

        var children = new List<object>();
        try
        {
            // 子目录
            foreach (var sub in dirInfo.GetDirectories().OrderBy(d => d.Name))
            {
                if (skipDirs.Any(s => string.Equals(s, sub.Name, StringComparison.OrdinalIgnoreCase))) continue;
                if ((sub.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                children.Add(BuildDirTree(sub.FullName, maxDepth, currentDepth + 1));
            }
            // 文件
            foreach (var f in dirInfo.GetFiles().OrderBy(f => f.Name))
            {
                if ((f.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                // 大于 5MB 跳过
                if (f.Length > 5 * 1024 * 1024) continue;
                children.Add(new
                {
                    name = f.Name,
                    path = f.FullName,
                    type = "file",
                    size = f.Length,
                });
            }
        }
        catch (UnauthorizedAccessException) { }

        return new
        {
            name,
            path = dirInfo.FullName,
            type = "dir",
            children,
        };
    }

    private async Task<string?> PickOpenPath(JsonElement args)
    {
        var tcs = new TaskCompletionSource<string?>();
        // BeginInvoke：不能在 WebMessageReceived 回调内直接弹模态对话框（会掉到主窗口后面）
        _form.BeginInvoke(() =>
        {
            try
            {
                using var dlg = new OpenFileDialog
                {
                    Title = "打开文件",
                    Filter = args.ValueKind == JsonValueKind.String ? args.GetString() : "所有文件|*.*",
                    CheckFileExists = true,
                };
                tcs.SetResult(dlg.ShowDialog(_form) == DialogResult.OK ? dlg.FileName : null);
            }
            catch (Exception ex) { tcs.SetException(ex); }
        });
        return await tcs.Task;
    }

    private async Task<string?> PickSavePath(JsonElement args)
    {
        var tcs = new TaskCompletionSource<string?>();
        _form.BeginInvoke(() =>
        {
            try
            {
                using var dlg = new SaveFileDialog
                {
                    Title = "另存为",
                    Filter = args.ValueKind == JsonValueKind.String ? args.GetString() : "Markdown 文件|*.md;*.markdown",
                    DefaultExt = "md",
                    AddExtension = true,
                };
                tcs.SetResult(dlg.ShowDialog(_form) == DialogResult.OK ? dlg.FileName : null);
            }
            catch (Exception ex) { tcs.SetException(ex); }
        });
        return await tcs.Task;
    }

    // ========== 剪贴板 ==========

    private string? ReadClipboardText()
    {
        try { return Clipboard.ContainsText() ? Clipboard.GetText() : null; }
        catch { return null; }
    }

    private bool WriteClipboardText(JsonElement args)
    {
        try
        {
            string text = args.GetString() ?? "";
            Clipboard.SetText(text);
            return true;
        }
        catch { return false; }
    }

    // ========== 图片保存（剪贴板图片粘贴） ==========

    /// <summary>
    /// 接收 base64 dataURL，保存到：
    ///   - 如果当前有打开的 MD 文件，存到 "{MD 文件目录}/assets/{时间戳}.{ext}"
    ///   - 否则存到 "{EXE 目录}/assets/{时间戳}.{ext}"
    /// 返回：{relativePath, absolutePath}
    /// </summary>
    private async Task<object?> SaveImage(JsonElement args)
    {
        string dataUrl = args[0].GetString() ?? throw new ArgumentException("dataUrl required");
        string? suggestedName = args[1].ValueKind == JsonValueKind.String ? args[1].GetString() : null;

        // 解析 dataURL
        var (bytes, ext) = ParseDataUrl(dataUrl);

        // 决定目录
        string baseDir;
        if (!string.IsNullOrEmpty(CurrentFilePath))
            baseDir = Path.GetDirectoryName(CurrentFilePath) ?? AppContext.BaseDirectory;
        else
            baseDir = AppContext.BaseDirectory;

        string assetsDir = Path.Combine(baseDir, "assets");
        Directory.CreateDirectory(assetsDir);

        // 文件名：时间戳 + 4 位随机
        string name = suggestedName ?? $"img-{DateTime.Now:yyyyMMdd-HHmmss}-{Random.Shared.Next(1000, 9999)}";
        if (!name.EndsWith("." + ext, StringComparison.OrdinalIgnoreCase))
            name = $"{name}.{ext}";
        string fileName = SanitizeFileName(name);
        string absPath = Path.Combine(assetsDir, fileName);

        await File.WriteAllBytesAsync(absPath, bytes);

        // 相对路径（Web 端用正斜杠）
        string relPath = "assets/" + fileName;

        return new { relativePath = relPath, absolutePath = absPath };
    }

    private static (byte[] bytes, string ext) ParseDataUrl(string dataUrl)
    {
        // data:image/png;base64,xxxxx
        const string prefix = "data:";
        if (!dataUrl.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Invalid dataURL");

        int semi = dataUrl.IndexOf(';');
        int comma = dataUrl.IndexOf(',');
        string mime = dataUrl.Substring(prefix.Length, semi - prefix.Length);
        string ext = mime switch
        {
            "image/png" => "png",
            "image/jpeg" or "image/jpg" => "jpg",
            "image/gif" => "gif",
            "image/webp" => "webp",
            "image/bmp" => "bmp",
            "image/svg+xml" => "svg",
            _ => "png",
        };
        string b64 = dataUrl.Substring(comma + 1);
        byte[] bytes = Convert.FromBase64String(b64);
        return (bytes, ext);
    }

    /// <summary>
    /// 从 base64 dataURL 保存任意二进制文件（用于 a.click() 拦截的下载）。
    /// </summary>
    private async Task<object?> SaveBinary(JsonElement args)
    {
        string dataUrl = args[0].GetString() ?? throw new ArgumentException("dataUrl required");
        string fileName = args[1].GetString() ?? "download.bin";

        var (bytes, ext) = ParseDataUrl(dataUrl);
        if (!fileName.EndsWith("." + ext, StringComparison.OrdinalIgnoreCase))
            fileName = $"{Path.GetFileNameWithoutExtension(fileName)}.{ext}";

        // 决定目录
        string baseDir = !string.IsNullOrEmpty(CurrentFilePath)
            ? Path.GetDirectoryName(CurrentFilePath) ?? AppContext.BaseDirectory
            : AppContext.BaseDirectory;

        // 用 Save As 对话框让用户选位置
        var tcs = new TaskCompletionSource<string?>();
        _form.BeginInvoke(() =>
        {
            try
            {
                using var dlg = new SaveFileDialog
                {
                    Title = "保存文件",
                    FileName = SanitizeFileName(fileName),
                    Filter = "所有文件|*.*",
                    AddExtension = true,
                    InitialDirectory = baseDir,
                };
                tcs.SetResult(dlg.ShowDialog(_form) == DialogResult.OK ? dlg.FileName : null);
            }
            catch (Exception ex) { tcs.SetException(ex); }
        });

        string? targetPath = await tcs.Task;
        if (targetPath == null) return null;

        await File.WriteAllBytesAsync(targetPath, bytes);
        return new { path = targetPath };
    }

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sb = new StringBuilder(name.Length);
        foreach (var c in name) sb.Append(Array.IndexOf(invalid, c) < 0 ? c : '_');
        return sb.ToString();
    }

    // ========== 对话框 ==========

    private bool ConfirmDialog(JsonElement args)
    {
        string message = args[0].GetString() ?? "";
        string? title = args[1].ValueKind == JsonValueKind.String ? args[1].GetString() : null;
        var tcs = new TaskCompletionSource<bool>();
        _form.Invoke(() =>
        {
            var r = MessageBox.Show(_form, message, title ?? "Inkwell",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            tcs.SetResult(r == DialogResult.Yes);
        });
        return tcs.Task.GetAwaiter().GetResult();
    }

    private bool AlertDialog(JsonElement args)
    {
        string message = args[0].GetString() ?? "";
        string? title = args[1].ValueKind == JsonValueKind.String ? args[1].GetString() : null;
        var tcs = new TaskCompletionSource<bool>();
        _form.Invoke(() =>
        {
            MessageBox.Show(_form, message, title ?? "Inkwell",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            tcs.SetResult(true);
        });
        return tcs.Task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// 简单输入对话框（用 VB Interaction.InputBox 走 .NET 内置）。
    /// 返回用户输入的字符串，取消返回 null。
    /// </summary>
    private string? PromptDialog(JsonElement args)
    {
        string message = args[0].GetString() ?? "";
        string defaultValue = args[1].ValueKind == JsonValueKind.String ? args[1].GetString() ?? "" : "";
        string? title = args[2].ValueKind == JsonValueKind.String ? args[2].GetString() : null;

        var tcs = new TaskCompletionSource<string?>();
        _form.Invoke(() =>
        {
            // 用 .NET 内置的 VisualBasic Interaction.InputBox（最简单，无需自定义 WinForms 控件）
            try
            {
                string? result = Microsoft.VisualBasic.Interaction.InputBox(
                    message, title ?? "输入", defaultValue, -1, -1);
                tcs.SetResult(string.IsNullOrEmpty(result) ? null : result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        return tcs.Task.GetAwaiter().GetResult();
    }

    // ========== 状态 ==========

    private object SetWindowTitle(JsonElement args)
    {
        string? title = args.GetString();
        _form.Invoke(() => _form.SetWindowTitle(title));
        return new { };
    }

    /// <summary>
    /// 文件关联（V0.15i）：把 Inkwell 注册到 .md / .markdown / .json 的
    /// 「打开方式」列表（HKCU，无需管理员）。写入 ProgId + OpenWithProgids +
    /// RegisteredApplications 应用能力（供系统关联向导展示）。
    /// </summary>
    private object AssociateFileTypes()
    {
        string exe = Environment.ProcessPath ?? AppContext.BaseDirectory;
        using (var progId = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Classes\Inkwell.Document"))
        {
            progId.SetValue("", "Inkwell 文档");
            using (var icon = progId.CreateSubKey("DefaultIcon"))
                icon.SetValue("", $"{exe},0");
            using (var cmd = progId.CreateSubKey(@"shell\open\command"))
                cmd.SetValue("", $"\"{exe}\" \"%1\"");
        }
        foreach (var ext in new[] { ".md", ".markdown", ".json" })
        {
            using var extKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                @"Software\Classes\" + ext + @"\OpenWithProgids");
            extKey.SetValue("Inkwell.Document", new byte[0], Microsoft.Win32.RegistryValueKind.None);
        }
        // 应用能力注册：系统「默认应用」向导据此列出 Inkwell 支持的扩展名
        using (var regApps = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\RegisteredApplications"))
            regApps.SetValue("Inkwell", @"Software\Inkwell\Capabilities");
        using (var cap = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Inkwell\Capabilities"))
        {
            cap.SetValue("ApplicationName", "Inkwell");
            cap.SetValue("ApplicationDescription", "一款本地优先的 Markdown / JSON 编辑器");
            using (var fa = cap.CreateSubKey("FileAssociations"))
                foreach (var ext in new[] { ".md", ".markdown", ".json" })
                    fa.SetValue(ext, "Inkwell.Document");
        }
        return new { ok = true };
    }

    /// <summary>
    /// 关联向导（V0.15j）：弹系统「设置程序关联」窗口（IApplicationAssociationRegistrationUI，
    /// 与 VS Code 安装器同款），用户勾选 .md/.json 点保存即完成默认关联；
    /// 失败时降级为直接打开 设置 → 默认应用 页面。
    /// </summary>
    private async Task<object> OpenAssociationWizard()
    {
        AssociateFileTypes();   // 先确保注册最新（exe 路径可能变化）
        var tcs = new TaskCompletionSource<bool>();
        // 模态 UI 必须推迟到消息处理结束后（同文件对话框，不能在回调里直接弹）
        _form.BeginInvoke(() =>
        {
            try
            {
                var ui = (IApplicationAssociationRegistrationUI)new ApplicationAssociationRegistrationUIClass();
                ui.LaunchAdvancedAssociationUI("Inkwell");
                tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"LaunchAdvancedAssociationUI failed: {ex.Message}");
                // Win10/11 的系统限制：老关联向导已被移除/受限。降级为直达设置页：
                // 优先用 deep link 打开 Inkwell 的专属默认应用页（免搜索），
                // 不支持时退回通用「默认应用」页
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "ms-settings:defaultapps?registeredApplicationName=Inkwell",
                        UseShellExecute = true,
                    });
                }
                catch
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "ms-settings:defaultapps",
                            UseShellExecute = true,
                        });
                    }
                    catch { }
                }
                tcs.SetResult(false);
            }
        });
        bool usedDialog = await tcs.Task;
        return new { ok = true, mode = usedDialog ? "dialog" : "settings" };
    }

    private object SetCurrentFile(JsonElement args)
    {
        CurrentFilePath = args.ValueKind == JsonValueKind.String ? args.GetString() : null;
        return new { };
    }

    /// <summary>
    /// 读取文档相对资源并转成 dataURL（预览本地图片用）：
    /// 相对路径依次在「当前 MD 文档目录」「EXE 目录」解析，命中即读盘返回。
    /// 路径被限制在对应目录内（拒绝 ".." 与盘符），防止逃逸读取任意文件。
    /// （V0.15e：虚拟主机无法动态重映射，改用桥接读文件方案）
    /// </summary>
    private object? ReadResource(JsonElement args)
    {
        string rel = args.GetString() ?? "";
        if (rel.Length == 0 || rel.Contains(':') || rel.Contains(".."))
            throw new ArgumentException("invalid resource path: " + rel);
        rel = rel.Replace('/', '\\').TrimStart('\\');

        string? docDir = !string.IsNullOrEmpty(CurrentFilePath)
            ? Path.GetDirectoryName(CurrentFilePath)
            : null;
        string? hit = null;
        foreach (var root in new[] { docDir, AppContext.BaseDirectory })
        {
            if (string.IsNullOrEmpty(root)) continue;
            string fullRoot = Path.GetFullPath(root);
            string candidate = Path.GetFullPath(Path.Combine(root, rel));
            if (!candidate.StartsWith(fullRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                continue;
            if (File.Exists(candidate)) { hit = candidate; break; }
        }
        if (hit is null)
            throw new FileNotFoundException("resource not found: " + rel);

        string mime = Path.GetExtension(hit).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream",
        };
        byte[] bytes = File.ReadAllBytes(hit);
        return new { dataUrl = $"data:{mime};base64,{Convert.ToBase64String(bytes)}" };
    }

    private object GetStartupInfo()
    {
        return new
        {
            version = "1.0.0",
            platform = "windows",
            isPackaged = false,
            hasWebView2 = true,
        };
    }

    private bool ShowInFolder(JsonElement args)
    {
        string? path = args.GetString();
        if (string.IsNullOrEmpty(path)) return false;
        try
        {
            string? dir = File.Exists(path) ? Path.GetDirectoryName(path) : path;
            if (string.IsNullOrEmpty(dir)) return false;
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = File.Exists(path) ? $"/select,\"{path}\"" : $"\"{dir}\"",
                UseShellExecute = true,
            });
            return true;
        }
        catch { return false; }
    }

    // ========== AI 大模型配置（持久化到 %USERPROFILE%\.Inkwell\ai-config.json） ==========

    private static readonly string AiConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".Inkwell");
    private static readonly string AiConfigFile = Path.Combine(AiConfigDir, "ai-config.json");

    public static string GetAiConfigPath() => AiConfigFile;

    /// <summary>
    /// 读 AI 配置。返回 JSON 字符串（若文件不存在返回 null）。
    /// </summary>
    private object? GetAiConfig()
    {
        try
        {
            if (!File.Exists(AiConfigFile)) return null;
            string content = File.ReadAllText(AiConfigFile, Encoding.UTF8);
            return new { path = AiConfigFile, content };
        }
        catch (Exception ex)
        {
            return new { path = AiConfigFile, error = ex.Message };
        }
    }

    /// <summary>
    /// 写 AI 配置。参数是 JSON 字符串。
    /// </summary>
    private object SaveAiConfig(JsonElement args)
    {
        try
        {
            string json = args.ValueKind == JsonValueKind.String ? args.GetString() ?? "" : args.GetRawText();
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("配置 JSON 不能为空");

            // 校验：是合法 JSON
            using (JsonDocument.Parse(json)) { }

            Directory.CreateDirectory(AiConfigDir);
            // 原子写：先写 .tmp，再 rename，避免半路崩溃把配置写坏
            string tmp = AiConfigFile + ".tmp";
            File.WriteAllText(tmp, json, new UTF8Encoding(false));  // 不写 BOM
            if (File.Exists(AiConfigFile)) File.Replace(tmp, AiConfigFile, null);
            else File.Move(tmp, AiConfigFile);
            return new { ok = true, path = AiConfigFile };
        }
        catch (Exception ex)
        {
            return new { ok = false, error = ex.Message };
        }
    }

    /// <summary>
    /// 在资源管理器里打开 .Inkwell 文件夹
    /// </summary>
    private bool RevealAiConfigFile()
    {
        try
        {
            Directory.CreateDirectory(AiConfigDir);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"\"{AiConfigDir}\"",
                UseShellExecute = true,
            });
            return true;
        }
        catch { return false; }
    }
}

/// <summary>
/// Windows 系统关联向导（shell32）：
/// LaunchAdvancedAssociationUI 弹出「设置程序关联」窗口，按 ProgId 列出
/// 该应用支持的扩展名，用户勾选保存即设为默认打开方式。
/// </summary>
[ComImport, Guid("1968106d-f3b5-44cf-890e-116fcb9ecef1")]
internal class ApplicationAssociationRegistrationUIClass { }

[ComImport, Guid("1f76a169-fa4c-4b62-979e-cf677ac0ff2f"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IApplicationAssociationRegistrationUI
{
    [PreserveSig]
    int LaunchAdvancedAssociationUI([MarshalAs(UnmanagedType.LPWStr)] string pszAppRegistryName);
}
