using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Cheats.ForzaHorizon4;

public class CameraCheats : CheatsUtilities, ICheatsBase
{
    public UIntPtr ChaseAddress;
    public UIntPtr ChaseFarAddress;
    public UIntPtr DriverAddress;
    public UIntPtr HoodAddress;
    public UIntPtr BumperAddress;
    public bool WereLimitersScanned;

    public async Task CheatLimiters()
    {
        WereLimitersScanned = false;
        ChaseAddress = 0;
        ChaseFarAddress = 0;
        DriverAddress = 0;
        HoodAddress = 0;
        BumperAddress = 0;
        
        var processMainModule = GetInstance().MProc.Process.MainModule;
        if (processMainModule == null)
        {
            return;
        }

        var successCount = 0; 
        var minRange = processMainModule.BaseAddress;
        var maxRange = minRange + processMainModule.ModuleMemorySize;

        const string chaseSig = "90 40 CD CC 8C 40 1F 85 2B 3F 00 00 00 40";
        var chaseList = await GetInstance().AoBScan(minRange, maxRange, chaseSig, true);
        var chaseEnumerable = chaseList as UIntPtr[] ?? chaseList.ToArray();
        if (chaseEnumerable.Length != 2)
        {
            ShowError("Chase Camera Limiters", chaseSig);
            goto skipScans;
        }

        ChaseAddress = chaseEnumerable.FirstOrDefault() - 10;
        ChaseFarAddress = chaseEnumerable.LastOrDefault() - 10;
        ++successCount;

        var newScanStart = (long)(ChaseAddress - 0x1000);
        var newScanEnd = (long)(ChaseAddress + 0x1000);
        
        const string driverHoodSig = "CD CC 4C 3E 00 50 43 47 00 00 34 42 00 00 20";
        var driverHoodList = await GetInstance().AoBScan(newScanStart, newScanEnd, driverHoodSig, true);
        var driverHoodEnumerable = driverHoodList as UIntPtr[] ?? driverHoodList.ToArray();
        if (driverHoodEnumerable.Length != 2)
        {
            ShowError("Bumper/Hood Camera Limiters", driverHoodSig);
            goto skipScans;
        }

        HoodAddress = driverHoodEnumerable.FirstOrDefault() - 0x24;
        DriverAddress = driverHoodEnumerable.LastOrDefault() - 0x24;
        ++successCount;

        const string bumperSig = "00 CD CC 4C 3E ? ? ? 47 00 ? 54";
        var bumperList = await GetInstance().AoBScan(newScanStart, newScanEnd, bumperSig, true);
        var bumperEnumerable = bumperList as UIntPtr[] ?? bumperList.ToArray();
        if (bumperEnumerable.Length == 0)
        {
            ShowError("Bumper Camera Limiter", bumperSig);
            goto skipScans;
        }

        BumperAddress = bumperEnumerable.FirstOrDefault() - 0x23;
        ++successCount;

        skipScans:
        WereLimitersScanned = successCount == 3;
    }

    public void Cleanup()
    {
    }

    public void Reset()
    {
        var fields = typeof(CameraCheats).GetFields().Where(f => f.FieldType == typeof(UIntPtr));
        foreach (var field in fields)
        {
            field.SetValue(this, UIntPtr.Zero);
        }
    }
}