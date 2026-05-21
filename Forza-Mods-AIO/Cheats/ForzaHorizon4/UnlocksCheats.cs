using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Cheats.ForzaHorizon4;

public class UnlocksCheats : CheatsUtilities, ICheatsBase
{
    private UIntPtr _creditsAddress;
    public UIntPtr CreditsDetourAddress;
    private UIntPtr _xpPointsAddress;
    public UIntPtr XpPointsDetourAddress;
    private UIntPtr _xpAddress;
    public UIntPtr XpDetourAddress;
    private UIntPtr _spinsAddress;
    public UIntPtr SpinsDetourAddress;
    private UIntPtr _skillPointsAddress;
    public UIntPtr SkillPointsDetourAddress;
    
    public async Task CheatCredits()
    {
        _creditsAddress = 0;
        CreditsDetourAddress = 0;
        
        const string sig = "89 84 ? ? ? ? ? 4C 8D ? ? ? ? ? 48 8B ? 48 8D";
        _creditsAddress = await SmartAobScan(sig);

        if (_creditsAddress > 0)
        {
            var asm = new byte[]
            {
                0x80, 0x3D, 0x1D, 0x00, 0x00, 0x00, 0x01, 0x75, 0x0F, 0x81, 0x7F, 0xB4, 0x43, 0x72, 0x65, 0x64, 0x75,
                0x06, 0x8B, 0x05, 0x0D, 0x00, 0x00, 0x00, 0x89, 0x84, 0x24, 0x80, 0x00, 0x00, 0x00
            };

            CreditsDetourAddress = GetInstance().CreateDetour(_creditsAddress, asm, 7);
            return;
        }
        
        ShowError("Credits", sig);
    }

    public async Task CheatXp()
    {
        _xpPointsAddress = 0;
        XpPointsDetourAddress = 0;
        _xpAddress = 0;
        XpDetourAddress = 0;

        const string sig = "44 89 ? ? 8B 89 ? ? ? ? 85 C9";
        _xpPointsAddress = await SmartAobScan(sig) + 4;
        if (_xpPointsAddress > 4)
        {
            _xpAddress = _xpPointsAddress + 14;
            var pointsAsm = new byte[]
            {
                0x80, 0x3D, 0x14, 0x00, 0x00, 0x00, 0x01, 0x75, 0x07, 0xC6, 0x81, 0xC0, 0x00, 0x00, 0x00, 0x01, 0x8B,
                0x89, 0xC0, 0x00, 0x00, 0x00
            };

            var asm = new byte[]
            {
                0x41, 0x8B, 0x85, 0xE8, 0x00, 0x00, 0x00, 0x80, 0x3D, 0x0D, 0x00, 0x00, 0x00, 0x01, 0x75, 0x06, 0x8B,
                0x05, 0x06, 0x00, 0x00, 0x00
            };

            XpPointsDetourAddress = GetInstance().CreateDetour(_xpPointsAddress, pointsAsm, 6);
            XpDetourAddress = GetInstance().CreateDetour(_xpAddress, asm, 7);
            return;
        }
        
        ShowError("Xp", sig);
    }

    public async Task CheatSpins()
    {
        _spinsAddress = 0;
        SpinsDetourAddress = 0;

        const string sig = "85 C0 89 44 ? ? 48 8D ? ? ? 48 8D ? ? ? 48 0F ? ? 83 FB";
        _spinsAddress = await SmartAobScan(sig);

        if (_spinsAddress > 0)
        {
            var asm = new byte[]
            {
                0x80, 0x3D, 0x13, 0x00, 0x00, 0x00, 0x01, 0x75, 0x06, 0x8B, 0x05, 0x0C, 0x00, 0x00, 0x00, 0x85, 0xC0,
                0x89, 0x44, 0x24, 0x38
            };

            SpinsDetourAddress = GetInstance().CreateDetour(_spinsAddress, asm, 6);
            return;
        }
        
        ShowError("Spins", sig);
    }
    
    public async Task CheatSkillPoints()
    {
        _skillPointsAddress = 0;
        SkillPointsDetourAddress = 0;

        const string sig = "85 D2 78 ? 89 54 ? ? 48 83 EC ? 48 83 E9";
        _skillPointsAddress = await SmartAobScan(sig) + 4;

        if (_skillPointsAddress > 4)
        {
            var asm = new byte[]
            {
                0x80, 0x3D, 0x15, 0x00, 0x00, 0x00, 0x01, 0x75, 0x06, 0x8B, 0x15, 0x0E, 0x00, 0x00, 0x00, 0x89, 0x54,
                0x24, 0x10, 0x48, 0x83, 0xEC, 0x28
            };

            SkillPointsDetourAddress = GetInstance().CreateDetour(_skillPointsAddress, asm, 8);
            return;
        }
        
        ShowError("Skill points", sig);
    }
    
    public void Cleanup()
    {
        var mem = GetInstance();
        
        if (_creditsAddress > 0)
        {
            mem.WriteArrayMemory(_creditsAddress, new byte[] { 0x89, 0x84, 0x24, 0x80, 0x00, 0x00, 0x00 });
            Free(CreditsDetourAddress);
        }

        if (_xpPointsAddress > 4)
        {
            mem.WriteArrayMemory(_xpPointsAddress, new byte[] { 0x8B, 0x89, 0xC0, 0x00, 0x00, 0x00 });
            Free(XpPointsDetourAddress);
        }

        if (_xpAddress > 0)
        {
            mem.WriteArrayMemory(_xpAddress, new byte[] { 0x41, 0x8B, 0x85, 0xE8, 0x00, 0x00, 0x00 });
            Free(XpDetourAddress);
        }

        if (_spinsAddress > 0)
        {
            mem.WriteArrayMemory(_spinsAddress, new byte[] { 0x85, 0xC0, 0x89, 0x44, 0x24, 0x38 });
            Free(SpinsDetourAddress);
        }
        
        if (_skillPointsAddress <= 4) return;
        mem.WriteArrayMemory(_skillPointsAddress, new byte[] { 0x89, 0x54, 0x24, 0x10, 0x48, 0x83, 0xEC, 0x28 });
        Free(SkillPointsDetourAddress);
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