// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Stride.Core.Assets.Editor.Components.TemplateDescriptions.ViewModels;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.IO;
using Stride.Core.Presentation.Services;
using Stride.GameStudio.ViewModels;

namespace Stride.GameStudio.View
{
    /// <summary>
    /// Avalonia version of the project selection window.
    /// </summary>
    public partial class ProjectSelectionWindow : Window, IModalDialog
    {
        public ProjectSelectionWindow()
        {
            InitializeComponent();
            Title = $"Project selection - Game Studio";
        }

        public NewSessionParameters? NewSessionParameters { get; private set; }

        public UFile? ExistingSessionPath { get; private set; }

        public NewOrOpenSessionTemplateCollectionViewModel? Templates
        {
            get => DataContext as NewOrOpenSessionTemplateCollectionViewModel;
            set => DataContext = value;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
            if (Templates != null)
            {
                // Save settings
                // InternalSettings.TemplatesWindowDialogLastNewSessionTemplateDirectory.SetValue(Templates.Location.FullPath);
            }
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
            
            if (Templates != null && DataContext is NewOrOpenSessionTemplateCollectionViewModel vm)
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
        }

        // IModalDialog implementation
        private TaskCompletionSource<DialogResult>? _showModalTcs;

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
    }
}

#endif
