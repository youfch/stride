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
    public partial class ProjectSelectionWindow : Window, IModalDialog
    {
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
            set
            {
                DataContext = value;
                if (value != null)
                {
                    var g = value.SelectedGroup;
                    if (g != null) value.SelectedGroup = g;
                }
            }
        }

        public Task<DialogResult> ShowModal()
        {
            // Handled by Program.Avalonia.cs via ShowDialog - kept for IModalDialog interface
            return Task.FromResult(DialogResult.Cancel);
        }

        public void RequestClose(DialogResult result)
        {
            Close(result);
        }

        private void OnSelectClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is NewOrOpenSessionTemplateCollectionViewModel vm && vm.SelectedTemplate != null)
            {
                CollectResult(vm);
                Close(DialogResult.Ok);
            }
        }

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            Close(DialogResult.Cancel);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void CollectResult(NewOrOpenSessionTemplateCollectionViewModel vm)
        {
            if (vm.SelectedTemplate is ExistingProjectViewModel existingProject)
            {
                ExistingSessionPath = existingProject.Path;
            }
            else if (vm.SelectedTemplate is ITemplateDescriptionViewModel templateDesc)
            {
                var template = templateDesc.GetTemplate();
                if (template != null)
                {
                    NewSessionParameters = new NewSessionParameters
                    {
                        TemplateDescription = template,
                        OutputName = vm.Name,
                        OutputDirectory = vm.Location,
                        SolutionName = vm.SolutionName,
                        SolutionLocation = vm.SolutionLocation,
                    };
                }
            }
        }
    }
}

#endif