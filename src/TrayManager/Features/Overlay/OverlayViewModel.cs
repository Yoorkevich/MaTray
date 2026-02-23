using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TrayManager.Features.KillProcess;
using TrayManager.Features.ListProcesses;
using TrayManager.Infrastructure.Wpf;

namespace TrayManager.Features.Overlay;

public class OverlayViewModel : INotifyPropertyChanged
{
    private readonly ListProcessesHandler _listHandler;
    private readonly KillProcessHandler _killHandler;

    public ObservableCollection<AppProcessInfo> Processes { get; } = [];

    public RelayCommand<AppProcessInfo> KillCommand { get; }

    public OverlayViewModel(ListProcessesHandler listHandler, KillProcessHandler killHandler)
    {
        _listHandler = listHandler;
        _killHandler = killHandler;

        KillCommand = new RelayCommand<AppProcessInfo>(OnKill);
    }

    public void Refresh()
    {
        Processes.Clear();
        foreach (var p in _listHandler.Handle())
            Processes.Add(p);
    }

    private void OnKill(AppProcessInfo? info)
    {
        if (info is null) return;

        var result = _killHandler.Handle(new KillProcessRequest(info.Pids, info.Name));
        if (result.Success)
            Processes.Remove(info);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
