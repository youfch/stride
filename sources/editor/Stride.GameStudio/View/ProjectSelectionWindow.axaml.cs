// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Stride.Core.Assets.Editor.Components.TemplateDescriptions.ViewModels;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.IO;
using Stride.Core.Presentation.Services;

namespace Stride.GameStudio.View
{
    /// <summary>
    /// Avalonia version of the project selection window.
    /// </summary>
    public partial class ProjectSelectionWindow : Window, IModalDialog
    {
        private TaskCompletionSource<DialogResult>? _showModalTcs;

        public ProjectSelectionWindow()
        {
            InitializeComponent();
            Title = "Project selection - Stride Game Studio";
        }

        public NewSessionParameters? NewSessionParameters { get; private set; }

        public UFile? ExistingSessionPath { get; private set; }

        public NewOrOpenSessionTemplateCollectionViewModel? Templates
        {
            get => DataContext as NewOrOpenSessionTemplateCollectionViewModel;
            set => DataContext = value;
        }

        // IModalDialog implementation
        public Task<DialogResult> ShowModal()
        {
            _showModalTcs = new TaskCompletionSource<DialogResult>();
            Show();
            return _showModalTcs.Task;
        }

        public void RequestClose(DialogResult result)
        {
            _showModalTcs?.TrySetResult(result);
            Close();
        }

        private void OnSelectClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            _showModalTcs?.TrySetResult(DialogResult.Cancel);
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Collect result from ViewModel
            if (DataContext is NewOrOpenSessionTemplateCollectionViewModel vm && vm.SelectedTemplate != null)
            {
                if (vm.SelectedTemplate is ExistingProjectViewModel existingProject)
                {
                    ExistingSessionPath = existingProject.Path;
                }
                else if (vm.SelectedTemplate is TemplateDescriptionViewModel templateDesc)
                {
                    NewSessionParameters = new NewSessionParameters
                    {
                        TemplateDescription = templateDesc.GetTemplate(),
                        OutputName = vm.Name,
                        OutputDirectory = vm.Location,
                        SolutionName = vm.SolutionName,
                        SolutionLocation = vm.SolutionLocation,
                    };
                }
            }

            _showModalTcs?.TrySetResult(ExistingSessionPath != null || NewSessionParameters != null
                ? DialogResult.Ok
                : DialogResult.Cancel);
        }
    }
}

#endif
