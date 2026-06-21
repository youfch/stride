// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dock.Avalonia.Controls;
using Dock.Model.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.GameStudio.View.Panels;

namespace Stride.GameStudio.Controls
{
    /// <summary>
    /// Creates the dock layout for Game Studio panels.
    /// </summary>
    public class GameStudioDockFactory
    {
        /// <summary>
        /// Initializes the dock layout with Solution Explorer, Asset View, and Property Grid panels.
        /// </summary>
        /// <param name="dockControl">The DockControl to initialize.</param>
        /// <param name="session">The current session ViewModel for data binding.</param>
        public static void InitializeLayout(DockControl dockControl, SessionViewModel session)
        {
            // Create panel Views with DataContext bound to session
            var solutionExplorerView = new SolutionExplorerView
            {
                DataContext = session
            };

            var assetView = new AssetView
            {
                DataContext = session
            };

            var propertyGridView = new PropertyGridView
            {
                DataContext = session
            };

            var solutionExplorer = new ToolDock
            {
                Id = "SolutionExplorer",
                Title = "Solution Explorer",
                CanClose = false,
                CanPin = true,
                Content = solutionExplorerView
            };

            var assetViewDock = new ToolDock
            {
                Id = "AssetView",
                Title = "Asset View",
                CanClose = false,
                CanPin = true,
                Content = assetView
            };

            var propertyGrid = new ToolDock
            {
                Id = "PropertyGrid",
                Title = "Property Grid",
                CanClose = false,
                CanPin = true,
                Content = propertyGridView
            };

            var documentDock = new DocumentDock
            {
                Id = "Documents",
                Title = "Documents",
                CanClose = false,
                CanPin = false,
                Content = new TextBlock
                {
                    Text = "Documents (not yet implemented)",
                    HorizontalAlignment = Avalonia.Media.VerticalAlignment.Center,
                    VerticalAlignment = Avalonia.Media.VerticalAlignment.Center,
                    Foreground = Avalonia.Media.Brushes.Gray
                }
            };

            var leftPanel = new ProportionalDock
            {
                Id = "LeftPanel",
                Orientation = Orientation.Vertical,
                ActiveDockable = solutionExplorer,
                VisibleDockables = new List<IDockable> { solutionExplorer }
            };

            var rightPanel = new ProportionalDock
            {
                Id = "RightPanel",
                Orientation = Orientation.Vertical,
                ActiveDockable = propertyGrid,
                VisibleDockables = new List<IDockable> { propertyGrid }
            };

            var centerPanel = new ProportionalDock
            {
                Id = "CenterPanel",
                Orientation = Orientation.Vertical,
                ActiveDockable = documentDock,
                VisibleDockables = new List<IDockable> { documentDock }
            };

            var rootDock = new RootDock
            {
                Id = "Root",
                ActiveDockable = centerPanel,
                DefaultDockable = centerPanel,
                VisibleDockables = new List<IDockable>
                {
                    leftPanel,
                    centerPanel,
                    rightPanel
                }
            };

            dockControl.Layout = rootDock;
        }
    }
}

#endif