using static Forza_Mods_AIO.Resources.Cheats;
using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Cheats.ForzaHorizon5;

public class ValueEncryption : CheatsUtilities, ICheatsBase, IRevertBase
{
    private UIntPtr _encryptAddress;
    
    public async Task CheatDisableValueEncryption()
    {
        _encryptAddress = 0;

        const string sig = "48 8B ? 48 89 ? ? 48 89 ? ? 48 89 ? ? 55 41 ? 41 ? 48 8D ? ? 48 81 EC ? ? ? ? 48 8B ? ? ? ? ? 48 33 ? 48 89 ? ? 4C 8B ? 48 89";
        _encryptAddress = await SmartAobScan(sig);

        if (_encryptAddress > 0)
        {
            if (GetClass<Bypass>().CallAddress <= 3)
            {
                await GetClass<Bypass>().DisableCrcChecks();
            }
            
            if (GetClass<Bypass>().CallAddress <= 3) return;

            GetInstance().WriteArrayMemory(_encryptAddress, new byte[] { 0xC3, 0x90, 0x90 });
            return;
        }
        
        ShowError("Value Encryption", sig);
    }
    
    public void Cleanup()
    {
        if (_encryptAddress > 0)
        {
            GetInstance().WriteArrayMemory(_encryptAddress, new byte[] { 0x48, 0x8B, 0xC4 });
        }
    }

    public void Reset()
    {
        var fields = typeof(ValueEncryption).GetFields().Where(f => f.FieldType == typeof(UIntPtr));
        foreach (var field in fields)
        {
            field.SetValue(this, UIntPtr.Zero);
        }
    }

    public void Revert()
    {
        if (_encryptAddress > 0)
        {
            GetInstance().WriteArrayMemory(_encryptAddress, new byte[] { 0x48, 0x8B, 0xC4 });
        }
    }

    public void Continue()
    {
        if (_encryptAddress > 0)
        {
            GetInstance().WriteArrayMemory(_encryptAddress, new byte[] { 0xC3, 0x90, 0x90 });
        }
    }
}