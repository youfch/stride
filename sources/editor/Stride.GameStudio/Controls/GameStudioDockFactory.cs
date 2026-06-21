// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Dock.Avalonia.Controls;
using Dock.Model.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;

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
        public static void InitializeLayout(DockControl dockControl)
        {
            var solutionExplorer = new ToolDock
            {
                Id = "SolutionExplorer",
                Title = "Solution Explorer",
                CanClose = false,
                CanPin = true,
            };

            var assetView = new ToolDock
            {
                Id = "AssetView",
                Title = "Asset View",
                CanClose = false,
                CanPin = true,
            };

            var propertyGrid = new ToolDock
            {
                Id = "PropertyGrid",
                Title = "Property Grid",
                CanClose = false,
                CanPin = true,
            };

            var documentDock = new DocumentDock
            {
                Id = "Documents",
                Title = "Documents",
                CanClose = false,
                CanPin = false,
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