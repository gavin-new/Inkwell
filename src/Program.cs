// =============================================================================
//  Inkwell — A single-file Markdown editor for Windows
//  Version: v0.16c
//  Original release: Ver 0.10
//  Author:  Gavin (gavin.zhang815@gmail.com)
//  License: MIT — see LICENSE in the repository root
//  Repo:    https://github.com/gavin-new/Inkwell
// =============================================================================

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Inkwell;

internal static class Program
{
    // .NET 10 桌面运行时下载页
    private const string DotnetDownloadUrl = "https://dotnet.microsoft.com/download/dotnet/10.0";
    private const string WebView2DownloadUrl = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";

    [STAThread]
    static void Main(string[] args)
    {
        // 全局未捕获异常兜底 → 写 crash.log + 诊断
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            HandleCrash(e.ExceptionObject as Exception);
        };
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (s, e) =>
        {
            HandleCrash(e.Exception);
        };

        // 启动优化
        ApplicationConfiguration.Initialize();
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        // 解析命令行参数
        string? initialFile = null;
        if (args.Length > 0 && System.IO.File.Exists(args[0]))
        {
            initialFile = args[0];
        }

        Application.Run(new MainForm(initialFile));
    }

    // 崩溃诊断：写日志 + 提示用户缺什么
    private static void HandleCrash(Exception? ex)
    {
        try
        {
            string logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Inkwell");
            Directory.CreateDirectory(logDir);
            File.AppendAllText(
                Path.Combine(logDir, "crash.log"),
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}\n\n");
        }
        catch { }

        if (ex == null) return;

        // 判定 1：是不是 .NET 桌面运行时的问题（TypeLoadException / FileNotFoundException
        //         且错误信息含 WindowsDesktop / WindowsBase 等关键词）
        bool isNetIssue =
            (ex is TypeLoadException || ex is FileNotFoundException || ex is TypeInitializationException)
            && (
                ex.Message.Contains("WindowsDesktop", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("WindowsBase", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("System.Windows.Forms", StringComparison.OrdinalIgnoreCase)
                || ex.StackTrace?.Contains("WindowsDesktop") == true
            );
        // 触发条件不严格时也兜底：任何 .NET-Desktop 相关的 dll-not-found
        if (!isNetIssue && ex is FileNotFoundException fnf)
        {
            string fn = fnf.Message ?? "";
            if (fn.Contains("Microsoft.WindowsDesktop.App")
                || fn.Contains("PresentationFramework")
                || fn.Contains("System.Windows.Forms"))
            {
                isNetIssue = true;
            }
        }

        // 判定 2：是不是 WebView2 的问题
        bool isWebView2Issue =
            ex.Message?.Contains("WebView2", StringComparison.OrdinalIgnoreCase) == true
            || ex.StackTrace?.Contains("WebView2", StringComparison.OrdinalIgnoreCase) == true
            || ex.Message?.Contains("EdgeWebView", StringComparison.OrdinalIgnoreCase) == true;

        if (isNetIssue)
        {
            // 先做一次确认检查，避免误报（用户环境可能只是 .NET 版本不对但 WindowsDesktop 有装）
            if (!IsNetDesktop10Installed(out _))
            {
                ShowNetMissingDialog();
            }
            else
            {
                ShowGenericCrashDialog(ex, "检测到 .NET 桌面相关异常，但 WindowsDesktop.App 已装。请把 crash.log 提交给开发者。");
            }
        }
        else if (isWebView2Issue)
        {
            if (!IsWebView2Installed())
            {
                ShowWebView2MissingDialog();
            }
            else
            {
                ShowGenericCrashDialog(ex, "检测到 WebView2 相关异常，但 Runtime 已装。请把 crash.log 提交给开发者。");
            }
        }
        else
        {
            ShowGenericCrashDialog(ex, "Inkwell 启动时发生未捕获异常。");
        }
    }

    private static void ShowNetMissingDialog()
    {
        var r = MessageBox.Show(
            "Inkwell 启动失败，疑似缺少 .NET 10 桌面运行时。\n\n" +
            "• Win11 24H2+ 通常已自带\n" +
            "• Win10 / 老版 Win11 需要手动下载安装\n" +
            "• 安装包约 50 MB，一次性安装，永久使用\n\n" +
            "是否打开 .NET 10 下载页面？",
            "Inkwell - 缺少 .NET 10 桌面运行时",
            MessageBoxButtons.YesNo, MessageBoxIcon.Error);
        if (r == DialogResult.Yes)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = DotnetDownloadUrl,
                    UseShellExecute = true,
                });
            }
            catch { }
        }
    }

    private static void ShowWebView2MissingDialog()
    {
        var r = MessageBox.Show(
            "Inkwell 启动失败，疑似缺少 WebView2 Runtime。\n\n" +
            "• Win11 通常已自带\n" +
            "• Win10 一般需要手动下载安装\n" +
            "• 安装包约 100 MB，一次性安装，永久使用\n\n" +
            "是否打开 WebView2 下载页面？",
            "Inkwell - 缺少 WebView2 运行时",
            MessageBoxButtons.YesNo, MessageBoxIcon.Error);
        if (r == DialogResult.Yes)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = WebView2DownloadUrl,
                    UseShellExecute = true,
                });
            }
            catch { }
        }
    }

    private static void ShowGenericCrashDialog(Exception ex, string hint)
    {
        try
        {
            MessageBox.Show(
                hint + "\n\n" +
                "错误：\n" + ex.GetType().Name + "\n" +
                ex.Message + "\n\n" +
                "详细日志已写入：\n" + Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Inkwell", "crash.log"),
                "Inkwell - 启动失败",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch { }
    }

    // 检查 .NET 10 桌面运行时（Microsoft.WindowsDesktop.App 10.x）
    // 仅在崩溃诊断时调用，**不**做启动时检测
    private static bool IsNetDesktop10Installed(out string version)
    {
        version = "";

        // 1) 优先查文件系统（最稳，不依赖 PATH）
        string[] roots =
        {
            @"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App",
            @"C:\Program Files (x86)\dotnet\shared\Microsoft.WindowsDesktop.App",
        };
        Version? best = null;
        foreach (var root in roots)
        {
            if (!Directory.Exists(root)) continue;
            try
            {
                foreach (var dir in Directory.GetDirectories(root))
                {
                    var name = Path.GetFileName(dir);
                    if (Version.TryParse(name, out var v) && (best == null || v > best))
                        best = v;
                }
            }
            catch { }
        }
        if (best != null && best.Major >= 10)
        {
            version = best.ToString();
            return true;
        }

        // 2) 兜底：用 dotnet --list-runtimes
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--list-runtimes",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var p = Process.Start(psi);
            if (p == null) return false;
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit(5000);
            Version? best2 = null;
            foreach (var line in output.Split('\n'))
            {
                var trimmed = line.Trim();
                if (!trimmed.StartsWith("Microsoft.WindowsDesktop.App")) continue;
                var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && Version.TryParse(parts[1], out var v) && (best2 == null || v > best2))
                    best2 = v;
            }
            if (best2 != null && best2.Major >= 10)
            {
                version = best2.ToString();
                return true;
            }
        }
        catch { }

        return false;
    }

    // 检查 WebView2 Runtime（启动时检测，因为 WebView2 是独立可选组件，
    // 缺失会导致 Inkwell 启动到一半就崩，而 .NET 是 .NET 整体的事）
    private static bool IsWebView2Installed()
    {
        const string keyPath = @"SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}";
        try
        {
            using (var k1 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\WOW6432Node\" + keyPath))
                if (k1 != null) return true;
            using (var k2 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyPath))
                if (k2 != null) return true;
            using (var k3 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keyPath))
                if (k3 != null) return true;

            string userPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Microsoft\EdgeWebView\Application");
            return Directory.Exists(userPath);
        }
        catch
        {
            return true;
        }
    }
}
