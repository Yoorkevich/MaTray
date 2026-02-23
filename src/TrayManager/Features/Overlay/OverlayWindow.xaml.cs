using System.Windows;
using System.Windows.Input;

namespace TrayManager.Features.Overlay;

public partial class OverlayWindow : Window
{
    public OverlayWindow()
    {
        InitializeComponent();
        ContentRendered += OnContentRendered;
        Deactivated += (_, _) => Close();
        KeyDown += (_, e) => { if (e.Key == Key.Escape) Close(); };
    }

    private void OnContentRendered(object? sender, EventArgs e)
    {
        var workArea = SystemParameters.WorkArea;
        Left = workArea.Left + (workArea.Width - ActualWidth) / 2;
        Top = workArea.Top + (workArea.Height - ActualHeight) / 2;
    }
}
