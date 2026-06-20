// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
#if AVALONIA
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
#else
using System.Windows;
#endif
using Stride.GameStudio.Helpers;

namespace Stride.GameStudio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
#if !AVALONIA
        private DataBindingExceptionRethrower exceptionRethrower;

        protected override void OnStartup(StartupEventArgs e)
        {
            exceptionRethrower = new DataBindingExceptionRethrower();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            exceptionRethrower?.Dispose();
        }
#endif
    }
}
