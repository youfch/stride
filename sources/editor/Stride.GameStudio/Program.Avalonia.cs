// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Media;
using Avalonia.Threading;
using Stride.Assets;
using Stride.Assets.Presentation;
using Stride.Core.Assets;
using Stride.Core.Assets.Editor;
using Stride.Core.Assets.Editor.Components.TemplateDescriptions.ViewModels;
using Stride.Core.Assets.Editor.Components.TemplateDescriptions.Views;
using Stride.Core.Assets.Editor.Services;
using Stride.Core.Assets.Editor.Settings;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.Diagnostics;
using Stride.Core.Extensions;
using Stride.Core.IO;
using Stride.Core.MostRecentlyUsedFiles;
using Stride.Core.Presentation.Services;
using Stride.Core.Presentation.Avalonia.Services;
using Stride.Core.Presentation.View;
using Stride.Core.Presentation.ViewModels;
using Stride.Core.Translation;
using Stride.Core.Translation.Providers;
using Stride.Editor.Build;
using Stride.Editor.Preview;
using Stride.GameStudio.Helpers;
using Stride.GameStudio.Plugin;
using Stride.GameStudio.Services;
using Stride.GameStudio.View;
using Stride.GameStudio.ViewModels;
using Stride.Graphics;
using Stride.Metrics;
using Stride.PrivacyPolicy;
using EditorSettings = Stride.Core.Assets.Editor.Settings.EditorSettings;

namespace Stride.GameStudio;

/// <summary>
/// Avalonia Program entry point.
/// </summary>
public static class Program
{
    private static App? app;
    private static bool terminating;
    private static readonly Dispatcher MainDispatcher;
    private static RenderDocManager? renderDocManager;
    private static readonly System.Collections.Concurrent.ConcurrentQueue<string> LogRingbuffer = new();
    private static bool enableThumbnailServices = true;

    // Startup checkpoints; shared file with the AutoTesting runner.
    private static readonly string DiagLogPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "gs-diag.log");
    private static void DiagLog(string message)
    {
        try { System.IO.File.AppendAllText(DiagLogPath, $"{DateTime.UtcNow:HH:mm:ss.fff} [tid={Thread.CurrentThread.ManagedThreadId}] GS: {message}\n"); }
        catch { /* best-effort */ }
    }

    static Program()
    {
        MainDispatcher = Dispatcher.UIThread;
    }

    /// <summary>
    /// Avalonia entry point.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        Run(Environment.GetCommandLineArgs().Skip(1).ToList());
    }

    /// <summary>
    /// Editor entry point body.
    /// </summary>
    public static void Run(IList<string> args)
    {
        DiagLog($"Run entered. args=[{string.Join(", ", args)}]");
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        EditorPath.EditorTitle = StrideGameStudio.EditorName;

        EditorSettings.Initialize();
        Thread.CurrentThread.Name = "Main thread";

        // Install Metrics for the editor
        using (StrideGameStudio.MetricsClient = EditorSettings.EnableMetrics.GetValue() ? new MetricsClient(CommonApps.StrideEditorAppId) : null)
        {
            try
            {
                var startupSessionPath = StrideEditorSettings.StartupSession.GetValue();
                var mru = new MostRecentlyUsedFileCollection(InternalSettings.LoadProfileCopy, InternalSettings.MostRecentlyUsedSessions, InternalSettings.WriteFile);
                mru.LoadFromSettings();
                var lastSessionPath = EditorSettings.ReloadLastSession.GetValue() ? mru.MostRecentlyUsedFiles.FirstOrDefault() : null;
                var initialSessionPath = !UPath.IsNullOrEmpty(startupSessionPath) ? startupSessionPath : lastSessionPath?.FilePath;

                // Handle arguments
                for (var i = 0; i < args.Count; i++)
                {
                    if (args[i] == "/NewProject")
                    {
                        initialSessionPath = null;
                    }
                    else if (args[i] == "/DebugEditorGraphics")
                    {
                        StrideConfig.GraphicsDebugMode = true;
                    }
                    else if (args[i] == "/DisableThumbnails")
                    {
                        enableThumbnailServices = false;
                    }
                    else if (args[i] == "/DisablePreview")
                    {
                        GameStudioPreviewService.DisablePreview = true;
                    }
                    else if (args[i] == "/RenderDoc")
                    {
                        GameStudioPreviewService.DisablePreview = true;
                        renderDocManager = new RenderDocManager();
                        renderDocManager.Initialize();
                    }
                    else if (args[i] == "/RecordEffects")
                    {
                        GameStudioBuilderService.GlobalEffectLogPath = args[++i];
                    }
                    else
                    {
                        initialSessionPath = args[i];
                    }
                }
                RuntimeHelpers.RunModuleConstructor(typeof(Asset).Module.ModuleHandle);

                // Listen to logger for crash report
                GlobalLogger.GlobalMessageLogged += GlobalLoggerOnGlobalMessageLogged;
                GlobalLogger.GlobalMessageLogged += new DebugLogListener { MinimumLevel = LogMessageType.Warning };

                MainDispatcher.InvokeAsync(() => Startup(initialSessionPath));

                // Build Avalonia app
                var builder = AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .UseSkia();

                app = (App)builder.Instance;
                StrideGameStudio.MetricsClient?.SetActiveState(true);

                DiagLog("Avalonia app configured");

                // Start the application main loop
                builder.StartWithClassicDesktopLifetime(args.ToArray());
            }
            catch (Exception e)
            {
                HandleException(e, 0);
            }
        }
    }

    private static void GlobalLoggerOnGlobalMessageLogged(ILogMessage logMessage)
    {
        if (logMessage.Type <= LogMessageType.Warning) return;

        LogRingbuffer.Enqueue(logMessage.ToString());
        while (LogRingbuffer.Count > 5)
        {
            LogRingbuffer.TryDequeue(out var msg);
        }
    }

    private sealed record CrashReportArgs(int Location, Exception Exception, string[] Log, string ThreadName);
    private static void CrashReport(object data)
    {
        var args = (CrashReportArgs)data;

        MainDispatcher?.InvokeAsync(() => Thread.CurrentThread.Join());

        CrashReportHelper.SendReport(args.Exception.FormatFull(), args.Location, args.Log, args.ThreadName);

        Environment.Exit(0);
    }

    private static void HandleException(Exception exception, int location)
    {
        if (exception == null) return;

        if (terminating) return;
        terminating = true;

        NuGetAssemblyResolver.DisableAssemblyResolve();

        var englishCulture = new CultureInfo("en-US");
        var crashLogThread = new Thread(CrashReport) { CurrentUICulture = englishCulture, CurrentCulture = englishCulture };
        crashLogThread.SetApartmentState(ApartmentState.STA);
        crashLogThread.Start(new CrashReportArgs(location, exception, LogRingbuffer.ToArray(), Thread.CurrentThread.Name));
        crashLogThread.Join();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.IsTerminating)
        {
            HandleException(e.ExceptionObject as Exception, 1);
        }
    }

    private static async void Startup(UFile initialSessionPath)
    {
        try
        {
            InitializeLanguageSettings();
            var serviceProvider = InitializeServiceProvider();

            try
            {
                PackageSessionPublicHelper.FindAndSetMSBuildVersion();
            }
            catch (Exception e)
            {
                var message = "Could not find a compatible version of MSBuild.\r\n\r\n" +
                              "Check that you have a valid installation with the required workloads, or go to [www.visualstudio.com/downloads](https://www.visualstudio.com/downloads) to install a new one.\r\n" +
                              $"Also make sure you have the latest [.NET {PackageSessionPublicHelper.NetMajorVersion} SDK](https://dotnet.microsoft.com/) \r\n\r\n" +
                              e;
                await serviceProvider.Get<IDialogService>().MessageBoxAsync(message, Core.Presentation.Services.MessageBoxButton.OK, Core.Presentation.Services.MessageBoxImage.Error);
                if (app?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    desktop.Shutdown();
                return;
            }

            var mru = new MostRecentlyUsedFileCollection(InternalSettings.LoadProfileCopy, InternalSettings.MostRecentlyUsedSessions, InternalSettings.WriteFile);
            mru.LoadFromSettings();
            var editor = new GameStudioViewModel(serviceProvider, mru);
            AssetsPlugin.RegisterPlugin(typeof(StrideDefaultAssetsPlugin));
            var strideEditorPlugin = (StrideEditorPlugin)AssetsPlugin.RegisterPlugin(typeof(StrideEditorPlugin));
            strideEditorPlugin.EnableThumbnailService = enableThumbnailServices;

            if (!UPath.IsNullOrEmpty(initialSessionPath))
            {
                var sessionLoaded = await editor.OpenInitialSession(initialSessionPath);
                if (sessionLoaded == true)
                {
                    var mainWindow = new GameStudioWindow(editor);
                    if (app?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.MainWindow = mainWindow;
                        mainWindow.Show();
                    }
                    return;
                }
            }

            // No session successfully loaded, open the new/open project window
            var startupWindow = new Stride.GameStudio.View.ProjectSelectionWindow();
            var viewModel = new NewOrOpenSessionTemplateCollectionViewModel(serviceProvider, startupWindow);
            startupWindow.Templates = viewModel;
            
            // Show as modal dialog using a hidden owner window
            var ownerWindow = new Window
            {
                Width = 0,
                Height = 0,
                WindowState = WindowState.Minimized,
                ShowInTaskbar = false,
                TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent },
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Transparent),
            };
            ownerWindow.Show();
            
            var dialogResult = await startupWindow.ShowDialog<DialogResult>(ownerWindow);
            
            System.IO.File.AppendAllText(
                System.IO.Path.Combine(System.IO.Path.GetTempPath(), "gs-crash.log"),
                $"[{DateTime.Now:HH:mm:ss.fff}] After ShowDialog, result={dialogResult}, NewSessionParams={startupWindow.NewSessionParameters != null}\n");

            try
            {
                if (startupWindow.NewSessionParameters != null)
                {
                    var directory = startupWindow.NewSessionParameters.OutputDirectory;
                    var name = startupWindow.NewSessionParameters.OutputName;
                    var mruData = new MRUAdditionalDataCollection(InternalSettings.LoadProfileCopy, GameStudioInternalSettings.MostRecentlyUsedSessionsData, InternalSettings.WriteFile);
                    mruData.RemoveFile(UFile.Combine(UDirectory.Combine(directory, name), new UFile(name + SessionViewModel.SolutionExtension)));

                    var completed = await editor.NewSession(startupWindow.NewSessionParameters);
                    System.IO.File.AppendAllText(
                        System.IO.Path.Combine(System.IO.Path.GetTempPath(), "gs-crash.log"),
                        $"[{DateTime.Now:HH:mm:ss.fff}] After NewSession, completed={completed}, Session={editor.Session != null}\n");
                    if (!completed)
                    {
                        System.IO.File.AppendAllText(
                            System.IO.Path.Combine(System.IO.Path.GetTempPath(), "gs-crash.log"),
                            $"[{DateTime.Now:HH:mm:ss.fff}] editor.NewSession returned false\n");
                    }
                }
                else if (startupWindow.ExistingSessionPath != null)
                {
                    var completed = await editor.OpenSession(startupWindow.ExistingSessionPath);
                }
                else
                {
                    // User cancelled, exit
                    ShutdownApp();
                    return;
                }

                if (editor.Session != null)
                {
                    try
                    {
                        var mainWindow = new GameStudioWindow(editor);
                        if (app?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        {
                            desktop.MainWindow = mainWindow;
                            mainWindow.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the crash details for debugging
                        System.IO.File.AppendAllText(
                            System.IO.Path.Combine(System.IO.Path.GetTempPath(), "gs-crash.log"),
                            $"[{DateTime.Now:HH:mm:ss.fff}] GameStudioWindow error: {ex}\n");
                        throw; // Let the outer catch handle it
                    }
                }
                else
                {
                    ShutdownApp();
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(System.IO.Path.GetTempPath(), "gs-crash.log"),
                    $"[{DateTime.Now:HH:mm:ss.fff}] Startup error: {ex}\n");
                ShutdownApp();
            }
        }
        catch (Exception ex)
        {
            System.IO.File.AppendAllText(
                System.IO.Path.Combine(System.IO.Path.GetTempPath(), "gs-crash.log"),
                $"[{DateTime.Now:HH:mm:ss.fff}] Startup outer error: {ex}\n");
            ShutdownApp();
        }
    }

    static void ShutdownApp()
        {
            if (app?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown();
        }

        private static IViewModelServiceProvider InitializeServiceProvider()
    {
        var dispatcherService = new AvaloniaDispatcherService(MainDispatcher);
        var dialogService = new AvaloniaDialogService();
        var strideDialogService = new StrideDialogService(dispatcherService, StrideGameStudio.EditorName);
        var pluginService = new PluginService();
        var services = new List<object> { dispatcherService, dialogService, strideDialogService, pluginService };
        if (renderDocManager != null)
            services.Add(renderDocManager);
        var serviceProvider = new ViewModelServiceProvider(services);
        return serviceProvider;
    }

    /// <summary>
    /// Avalonia-compatible dispatcher service wrapping <see cref="Avalonia.Threading.Dispatcher"/>.
    /// </summary>
    private class AvaloniaDispatcherService : IDispatcherService
    {
        private readonly Dispatcher _dispatcher;

        public AvaloniaDispatcherService(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Invoke(Action callback) => _dispatcher.Invoke(callback);

        public TResult Invoke<TResult>(Func<TResult> callback) => _dispatcher.Invoke(callback);

        public Task InvokeAsync(Action callback, CancellationToken token = default)
        {
            var tcs = new TaskCompletionSource();
            _dispatcher.Post(() =>
            {
                callback();
                tcs.SetResult();
            });
            return tcs.Task;
        }

        public Task LowPriorityInvokeAsync(Action callback, CancellationToken token = default)
        {
            var tcs = new TaskCompletionSource();
            _dispatcher.Post(() =>
            {
                callback();
                tcs.SetResult();
            }, DispatcherPriority.Background);
            return tcs.Task;
        }

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> callback, CancellationToken token = default)
        {
            var tcs = new TaskCompletionSource<TResult>();
            _dispatcher.Post(() =>
            {
                tcs.SetResult(callback());
            });
            return tcs.Task;
        }

        public Task InvokeTask(Func<Task> task, CancellationToken token = default)
        {
            var tcs = new TaskCompletionSource();
            _dispatcher.Post(async () =>
            {
                await task();
                tcs.SetResult();
            });
            return tcs.Task;
        }

        public Task<TResult> InvokeTask<TResult>(Func<Task<TResult>> task, CancellationToken token = default)
        {
            var tcs = new TaskCompletionSource<TResult>();
            _dispatcher.Post(async () =>
            {
                var result = await task();
                tcs.SetResult(result);
            });
            return tcs.Task;
        }

        public bool CheckAccess() => _dispatcher.CheckAccess();

        public void EnsureAccess(bool inDispatcherThread = true)
        {
            if (inDispatcherThread && !CheckAccess())
                throw new InvalidOperationException("Access to the dispatcher thread is required.");
            if (!inDispatcherThread && CheckAccess())
                throw new InvalidOperationException("Access to a non-dispatcher thread is required.");
        }
    }

    private static void InitializeLanguageSettings()
    {
        TranslationManager.Instance.RegisterProvider(new GettextTranslationProvider());
        TranslationManager.Instance.CurrentLanguage = EditorSettings.Language.GetValue() switch
        {
            SupportedLanguage.MachineDefault => CultureInfo.InstalledUICulture,
            SupportedLanguage.English => new CultureInfo("en-US"),
            SupportedLanguage.French => new CultureInfo("fr-FR"),
            SupportedLanguage.Japanese => new CultureInfo("ja-JP"),
            SupportedLanguage.Russian => new CultureInfo("ru-RU"),
            SupportedLanguage.German => new CultureInfo("de-DE"),
            SupportedLanguage.Spanish => new CultureInfo("es-ES"),
            SupportedLanguage.ChineseSimplified => new CultureInfo("zh-Hans"),
            SupportedLanguage.Italian => new CultureInfo("it-IT"),
            SupportedLanguage.Korean => new CultureInfo("ko-KR"),
            _ => throw new ArgumentException("Invalid language option"),
        };
    }
}

#endif