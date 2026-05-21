using System.Collections.ObjectModel;

namespace Forza_Mods_AIO.Models;

public sealed class DebugSession
{
    public DebugSession(string name, ObservableCollection<DebugInfoReport> debugInfoReports)
    {
        Name = name;
        DebugInfoReports = debugInfoReports;
    }

    public string Name { get; }
    public ObservableCollection<DebugInfoReport> DebugInfoReports { get; }
}