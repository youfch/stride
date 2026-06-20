// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Stride.Core.IO;
using Stride.Core.Presentation.Services;
using Stride.Core.Presentation.Windows;
using DialogButtonInfo = Stride.Core.Presentation.Windows.DialogButtonInfo;

namespace Stride.Core.Presentation.Avalonia.Services
{
    public class AvaloniaDialogService : IDialogService
    {
        private readonly Window? _mainWindow;

        public AvaloniaDialogService()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _mainWindow = desktop.MainWindow as Window;
            }
        }

        public bool HasMainWindow => _mainWindow != null;

        public void Exit(int exitCode = 0)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown(exitCode);
            else
                Environment.Exit(exitCode);
        }

        public async Task<MessageBoxResult> MessageBoxAsync(string message, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None)
        {
            return await ShowMessageBoxWindow(message, GetButtonLabels(buttons), image);
        }

        public async Task<int> MessageBoxAsync(string message, IReadOnlyCollection<DialogButtonInfo> buttons, MessageBoxImage image = MessageBoxImage.None)
        {
            var labels = buttons.Select(b => b.Content?.ToString() ?? "OK").ToList();
            var result = await ShowMessageBoxWindow(message, labels, image);
            return result == MessageBoxResult.OK ? 0 : 1;
        }

        public async Task<CheckedMessageBoxResult> CheckedMessageBoxAsync(string message, bool? isChecked, string checkboxMessage, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None)
        {
            return await ShowCheckedMessageBoxWindow(message, isChecked ?? false, checkboxMessage, GetButtonLabels(buttons), image);
        }

        public async Task<CheckedMessageBoxResult> CheckedMessageBoxAsync(string message, bool? isChecked, string checkboxMessage, IReadOnlyCollection<DialogButtonInfo> buttons, MessageBoxImage image = MessageBoxImage.None)
        {
            var labels = buttons.Select(b => b.Content?.ToString() ?? "OK").ToList();
            return await ShowCheckedMessageBoxWindow(message, isChecked ?? false, checkboxMessage, labels, image);
        }

        public async Task<UFile?> OpenFilePickerAsync(UDirectory? initialPath = null, IReadOnlyList<FilePickerFilter>? filters = null)
        {
            var topLevel = GetTopLevel();
            if (topLevel == null) return null;

            var options = new FilePickerOpenOptions
            {
                AllowMultiple = false,
                SuggestedStartLocation = await GetSuggestedFolder(initialPath, topLevel),
                FileTypeFilter = ConvertFilters(filters),
            };
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);
            return files.Count > 0 ? new UFile(files[0].Path.LocalPath) : null;
        }

        public async Task<IReadOnlyList<UFile>> OpenMultipleFilesPickerAsync(UDirectory? initialPath = null, IReadOnlyList<FilePickerFilter>? filters = null)
        {
            var topLevel = GetTopLevel();
            if (topLevel == null) return Array.Empty<UFile>();

            var options = new FilePickerOpenOptions
            {
                AllowMultiple = true,
                SuggestedStartLocation = await GetSuggestedFolder(initialPath, topLevel),
                FileTypeFilter = ConvertFilters(filters),
            };
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);
            return files.Select(f => new UFile(f.Path.LocalPath)).ToList();
        }

        public async Task<UDirectory?> OpenFolderPickerAsync(UDirectory? initialPath = null)
        {
            var topLevel = GetTopLevel();
            if (topLevel == null) return null;

            var options = new FolderPickerOpenOptions
            {
                AllowMultiple = false,
                SuggestedStartLocation = await GetSuggestedFolder(initialPath, topLevel),
            };
            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
            return folders.Count > 0 ? new UDirectory(folders[0].Path.LocalPath) : null;
        }

        public async Task<UFile?> SaveFilePickerAsync(UDirectory? initialPath = null, IReadOnlyList<FilePickerFilter>? filters = null, string? defaultExtension = null, string? defaultFileName = null)
        {
            var topLevel = GetTopLevel();
            if (topLevel == null) return null;

            var options = new FilePickerSaveOptions
            {
                SuggestedFileName = defaultFileName,
                DefaultExtension = defaultExtension,
                SuggestedStartLocation = await GetSuggestedFolder(initialPath, topLevel),
                FileTypeChoices = ConvertFilters(filters),
            };
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(options);
            return file != null ? new UFile(file.Path.LocalPath) : null;
        }

        private TopLevel? GetTopLevel()
        {
            if (_mainWindow != null) return TopLevel.GetTopLevel(_mainWindow);
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return TopLevel.GetTopLevel(desktop.MainWindow as Visual);
            return null;
        }

        private static async Task<IStorageFolder?> GetSuggestedFolder(UDirectory? path, TopLevel? topLevel)
        {
            if (path == null || topLevel == null) return null;
            return await topLevel.StorageProvider.TryGetFolderFromPathAsync(path.FullPath);
        }

        private static IReadOnlyList<FilePickerFileType>? ConvertFilters(IReadOnlyList<FilePickerFilter>? filters)
        {
            if (filters == null || filters.Count == 0) return null;
            return filters.Select(f =>
            {
                var patterns = f.Patterns?.ToList() ?? new List<string> { "*.*" };
                return new FilePickerFileType(f.Name ?? "All files") { Patterns = patterns };
            }).ToList();
        }

        private static List<string> GetButtonLabels(MessageBoxButton buttons)
        {
            return buttons switch
            {
                MessageBoxButton.OK => new List<string> { "OK" },
                MessageBoxButton.OKCancel => new List<string> { "OK", "Cancel" },
                MessageBoxButton.YesNo => new List<string> { "Yes", "No" },
                MessageBoxButton.YesNoCancel => new List<string> { "Yes", "No", "Cancel" },
                _ => new List<string> { "OK" },
            };
        }

        private static MessageBoxResult LabelToResult(string label)
        {
            return label switch
            {
                "OK" => MessageBoxResult.OK,
                "Cancel" => MessageBoxResult.Cancel,
                "Yes" => MessageBoxResult.Yes,
                "No" => MessageBoxResult.No,
                _ => MessageBoxResult.OK
            };
        }

        private async Task<MessageBoxResult> ShowMessageBoxWindow(string message, IReadOnlyList<string> buttonLabels, MessageBoxImage image)
        {
            var window = new Window
            {
                Title = "Message",
                Width = 400,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
            };

            var panel = new StackPanel { Margin = new Thickness(20), Spacing = 15 };
            var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            header.Children.Add(new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 340,
                VerticalAlignment = VerticalAlignment.Center
            });
            panel.Children.Add(header);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 8
            };

            var tcs = new TaskCompletionSource<MessageBoxResult>();
            foreach (var label in buttonLabels)
            {
                var btn = new Button { Content = label, MinWidth = 75, Padding = new Thickness(12, 4) };
                var captured = label;
                btn.Click += (_, _) => { tcs.TrySetResult(LabelToResult(captured)); window.Close(); };
                buttonPanel.Children.Add(btn);
            }
            panel.Children.Add(buttonPanel);
            window.Content = panel;

            if (_mainWindow != null) await window.ShowDialog(_mainWindow);
            else window.Show();
            return await tcs.Task;
        }

        private async Task<CheckedMessageBoxResult> ShowCheckedMessageBoxWindow(string message, bool isChecked, string checkboxMessage, IReadOnlyList<string> buttonLabels, MessageBoxImage image)
        {
            var window = new Window
            {
                Title = "Message",
                Width = 400,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
            };

            var panel = new StackPanel { Margin = new Thickness(20), Spacing = 15 };
            panel.Children.Add(new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, MaxWidth = 360 });
            var cb = new CheckBox { Content = checkboxMessage, IsChecked = isChecked };
            panel.Children.Add(cb);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 8
            };

            var tcs = new TaskCompletionSource<CheckedMessageBoxResult>();
            foreach (var label in buttonLabels)
            {
                var btn = new Button { Content = label, MinWidth = 75, Padding = new Thickness(12, 4) };
                btn.Click += (_, _) =>
                {
                    tcs.TrySetResult(new CheckedMessageBoxResult(LabelToResult(label), cb.IsChecked ?? false));
                    window.Close();
                };
                buttonPanel.Children.Add(btn);
            }
            panel.Children.Add(buttonPanel);
            window.Content = panel;

            if (_mainWindow != null) await window.ShowDialog(_mainWindow);
            else window.Show();
            return await tcs.Task;
        }
    }
}