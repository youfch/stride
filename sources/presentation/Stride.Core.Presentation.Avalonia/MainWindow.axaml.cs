using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace Stride.Core.Presentation.Avalonia;

public class MainWindow : Window
{
    public MainWindow()
    {
        Title = "Stride Game Studio (Avalonia)";
        Width = 1280;
        Height = 800;
        MinWidth = 800;
        MinHeight = 600;

        // TODO: Implement main window layout with docking
        Content = new TextBlock
        {
            Text = "Stride Game Studio - Avalonia UI (WIP)",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 24
        };
    }
}
