// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.Presentation.Windows;
using Stride.GameStudio.Controls;
using Stride.GameStudio.Helpers;
using Stride.GameStudio.ViewModels;

namespace Stride.GameStudio.View
{
    /// <summary>
    /// Avalonia-specific implementation of GameStudioWindow.
    /// </summary>
    public partial class GameStudioWindow : Window
    {
        /// <summary>
        /// Parameterless constructor required by Avalonia XAML compiler.
        /// </summary>
        public GameStudioWindow()
        {
            InitializeComponent();
        }

        public GameStudioWindow(EditorViewModel editor)
        {
            if (editor == null) throw new ArgumentNullException(nameof(editor));
            DataContext = editor;
            InitializeComponent();
            InitializeDocking();
        }

        public EditorViewModel Editor => (EditorViewModel)DataContext;

        public string EditorTitle => Editor.Session.SolutionPath != null
            ? $"{Editor.Session.SolutionPath.GetFileName()} - Game Studio"
            : "Game Studio";

        public Task<bool> TryClose()
        {
            Close();
            return Task.FromResult(true);
        }

        private void InitializeDocking()
        {
            // Pass the Session ViewModel to the dock factory for data binding
            GameStudioDockFactory.InitializeLayout(MainDockControl, Editor.Session);
        }
    }
}

#endif