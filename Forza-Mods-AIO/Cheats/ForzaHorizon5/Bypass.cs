using Forza_Mods_AIO.Models;
using static Forza_Mods_AIO.Resources.Cheats;
using static Forza_Mods_AIO.Resources.Memory;
using Timer = System.Timers.Timer;

namespace Forza_Mods_AIO.Cheats.ForzaHorizon5;

public class Bypass : CheatsUtilities, ICheatsBase
{
    public readonly DebugSession BypassDebug = new("Bypass", []);

    public UIntPtr CallAddress;
    public UIntPtr XxhCheck;
    public UIntPtr OrigXxhCheck;
    public UIntPtr Ret;
    private bool _scanning;
    private Timer _antiCheatTimer = null!;
    
    public async Task DisableCrcChecks()
    {
        var wasScanning = _scanning;
        while (_scanning)
        {
            await Task.Delay(1);
        }

        if (wasScanning)
        {
            return;
        }

        _scanning = true;
        CallAddress = 0;

        Ret = await SmartAobScan("C3");
        if (Ret == 0)
        {
            ShowError("Bypass", "C3");
            return;
        }
        
        const string sig = "48 8B D9 48 8D 05 ? ? ? ? 48 89 01 E8 ? ? ? ? 48 8B CB 48 83 C4 20 5B E9";
        CallAddress = await SmartAobScan(sig);

        if (CallAddress > 3)
        {
            UIntPtr pXxhCheckPfns = CallAddress + 0x3;
            int pPfnRelative = GetInstance().ReadMemory<int>(pXxhCheckPfns + 0x3);
            UIntPtr xxhCheckPfns = (UIntPtr)(pPfnRelative + (IntPtr)pXxhCheckPfns + 0x7);
            XxhCheck = xxhCheckPfns + 0x30;
            OrigXxhCheck = GetInstance().ReadMemory<UIntPtr>(XxhCheck);

            
            _antiCheatTimer = new Timer();
            _antiCheatTimer.Interval = 10_000;
            _antiCheatTimer.Elapsed += async (_, _) =>
            {
                var mem = GetInstance();
                foreach (var pair in CachedInstances.Where(kv => typeof(IRevertBase).IsAssignableFrom(kv.Key)))
                {
                    ((IRevertBase)pair.Value).Revert();
                }
                mem.WriteMemory(XxhCheck, OrigXxhCheck);
                await Task.Delay(1_000);
                mem.WriteMemory(XxhCheck, Ret);
                foreach (var pair in CachedInstances.Where(kv => typeof(IRevertBase).IsAssignableFrom(kv.Key)))
                {
                    ((IRevertBase)pair.Value).Continue();
                }
            };
        
            GetInstance().WriteMemory(XxhCheck, Ret);
            _antiCheatTimer.Start();
            _scanning = false;
            return;
        }
        
        _scanning = false;
        ShowError("Bypass", sig);
    }
    
    public void Cleanup()
    {
        var mem = GetInstance();
        if (XxhCheck <= 3) return;
        _antiCheatTimer.Stop();
        mem.WriteMemory(XxhCheck, OrigXxhCheck);
    }

    public void Reset()
    {
        _scanning = false;
        if (_antiCheatTimer != null!)
        {
            _antiCheatTimer.Stop();
            _antiCheatTimer = null!;
        }
        var fields = typeof(Bypass).GetFields().Where(f => f.FieldType == typeof(UIntPtr));
        foreach (var field in fields)
        {
            field.SetValue(this, UIntPtr.Zero);
        }
    }
}