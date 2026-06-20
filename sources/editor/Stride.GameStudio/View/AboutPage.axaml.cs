// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Stride.Core.Assets;
using Stride.Core.Assets.Editor.Services;
using Stride.Core.Extensions;

namespace Stride.GameStudio.View
{
    /// <summary>
    /// Avalonia About dialog for Stride Game Studio.
    /// </summary>
    public partial class AboutPage : Window
    {
        private const string MarkdownNotLoaded = "Unable to load the file.";
        private static readonly StyledProperty<string> MarkdownBackersProperty =
            AvaloniaProperty.Register<AboutPage, string>(nameof(MarkdownBackers));

        public AboutPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadBackers().Forget();
        }

        public AboutPage(IEditorDialogService service)
            : this()
        {
            Service = service;
        }

        public string MarkdownBackers
        {
            get => GetValue(MarkdownBackersProperty);
            set => SetValue(MarkdownBackersProperty, value);
        }

        private IEditorDialogService? Service { get; }

        /// <summary>
        /// Shows the about page as a modal dialog (called from StrideDialogService).
        /// </summary>
        public Task ShowModal()
        {
            return ShowDialog<Window?>(Owner as Window ?? null);
        }

        private void ButtonCloseClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void License_OnClick(object? sender, RoutedEventArgs e)
        {
            var message = await LoadMarkdown("LICENSE.md");
            Service?.MessageBoxAsync(message).Forget();
        }

        private async void ThirdParty_OnClick(object? sender, RoutedEventArgs e)
        {
            var message = await LoadMarkdown("THIRD PARTY.md");
            Service?.MessageBoxAsync(message).Forget();
        }

        private static async Task<string> LoadMarkdown(string file)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var filePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", file);
                    if (!File.Exists(filePath))
                        filePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", @"..\..\content", file);
                    using var reader = new StreamReader(new FileStream(filePath, FileMode.Open));
                    return reader.ReadToEnd();
                });
            }
            catch
            {
                return MarkdownNotLoaded;
            }
        }

        private async Task LoadBackers()
        {
            MarkdownBackers = await LoadMarkdown("BACKERS.md");
        }
    }
}

#endif