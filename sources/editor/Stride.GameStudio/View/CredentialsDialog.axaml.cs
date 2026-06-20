// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Stride.Core.Assets.Editor.Services;
using Stride.Core.Extensions;
using Stride.Core.Presentation.Services;
using Stride.GameStudio.Services;
using Stride.Core.Translation;

namespace Stride.GameStudio.View
{
    /// <summary>
    /// Avalonia credentials dialog for remote host connection.
    /// </summary>
    public partial class CredentialsDialog : Window, ICredentialsDialog
    {
        public CredentialsDialog()
        {
            InitializeComponent();
        }

        public CredentialsDialog(IEditorDialogService service)
            : this()
        {
            Service = service;
        }

        public bool AreCredentialsValid { get; private set; }
        private IEditorDialogService Service { get; }

        private void OnTestSettings(object? sender, RoutedEventArgs e)
        {
            Service?.MessageBoxAsync("Test connection - Avalonia placeholder", MessageBoxButton.OK, MessageBoxImage.Information).Forget();
        }

        private void OnOk(object? sender, RoutedEventArgs e)
        {
            AreCredentialsValid = true;
            Close();
        }

        private void OnCancel(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        // IModalDialog implementation
        async Task<DialogResult> IModalDialog.ShowModal()
        {
            await ShowDialog<Window>(Owner as Window ?? null);
            return AreCredentialsValid ? DialogResult.Ok : DialogResult.Cancel;
        }

        void IModalDialog.RequestClose(DialogResult result)
        {
            Close();
        }

        object IModalDialog.DataContext
        {
            get => DataContext;
            set => DataContext = value;
        }
    }
}

#endif