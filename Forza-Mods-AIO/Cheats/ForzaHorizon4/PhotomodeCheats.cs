using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Cheats.ForzaHorizon4;

public class PhotomodeCheats : CheatsUtilities, ICheatsBase
{
    private UIntPtr _noClipAddress;
    public UIntPtr NoClipDetourAddress;
    private UIntPtr _noHeightLimitAddress;
    public UIntPtr NoHeightLimitDetourAddress;
    private UIntPtr _increasedZoomAddress;
    public UIntPtr IncreasedZoomDetourAddress;
    public UIntPtr MainModifiersAddress;
    public UIntPtr SpeedAddress;
    public bool WasModifiersScanSuccessful;
    
    public async Task CheatNoClip()
    {
        _noClipAddress = 0;
        NoClipDetourAddress = 0;

        const string sig = "88 44 ? ? 48 8D ? ? ? 48 89 ? ? 48 C7 45 90";
        _noClipAddress = await SmartAobScan(sig);

        if (_noClipAddress > 0)
        {
            _noClipAddress -= 84;
            var asm = new byte[]
            {
                0x80, 0x3D, 0x14, 0x00, 0x00, 0x00, 0x01, 0x75, 0x08, 0xC7, 0x44, 0x24, 0x30, 0x00, 0x00, 0x00, 0x00,
                0x0F, 0x11, 0x44, 0x24, 0x34
            };

            NoClipDetourAddress = GetInstance().CreateDetour(_noClipAddress, asm, 5);
            return;
        }
        
        ShowError("Photo mode no clip", sig);
    }

    public async Task CheatNoHeightLimits()
    {
        _noHeightLimitAddress = 0;
        NoHeightLimitDetourAddress = 0;

        const string sig = "F2 0F ? ? ? ? ? ? 66 0F ? ? 0F 2F ? 76";
        _noHeightLimitAddress = await SmartAobScan(sig);

        if (_noHeightLimitAddress > 0)
        {
            var asm = new byte[]
            {
                0x80, 0x3D, 0x1D, 0x00, 0x00, 0x00, 0x01, 0x75, 0x0E, 0x68, 0xF9, 0x02, 0x15, 0x50, 0xF3, 0x0F, 0x10,
                0x14, 0x24, 0x48, 0x83, 0xC4, 0x08, 0xF2, 0x0F, 0x10, 0x9E, 0xE0, 0x02, 0x00, 0x00
            };
            NoHeightLimitDetourAddress = GetInstance().CreateDetour(_noHeightLimitAddress, asm, 8);
            return;
        }
        
        ShowError("Photo mode no height limits", sig);
    }
    
    public async Task CheatIncreasedZoom()
    {
        _increasedZoomAddress = 0;
        IncreasedZoomDetourAddress = 0;

        const string sig = "FF 90 ? ? ? ? F3 0F ? ? ? ? ? ? 0F 28 ? F3 0F ? ? F3 0F ? ? ? ? ? ? F3 0F ? ? F3 0F ? ? F3 0F ? ? ? ? ? ? E8";
        _increasedZoomAddress = await SmartAobScan(sig);

        if (_increasedZoomAddress > 0)
        {
            var asm = new byte[]
            {
                0xFF, 0x90, 0x30, 0x03, 0x00, 0x00, 0x80, 0x3D, 0x0A, 0x00, 0x00, 0x00, 0x01, 0x75, 0x03, 0x0F, 0x57,
                0xC0
            };
            
            IncreasedZoomDetourAddress = GetInstance().CreateDetour(_increasedZoomAddress, asm, 6);
            return;
        }
        
        ShowError("Photo mode increased zoom", sig);
    }
    
    public async Task CheatModifiers()
    {
        MainModifiersAddress = 0;
        SpeedAddress = 0;

        var successCount = 0;
        
        const string mainModifiersSig = "3B 1D ? ? ? ? 0F 4E";
        MainModifiersAddress = await SmartAobScan(mainModifiersSig);
        if (MainModifiersAddress <= 0)
        {
            ShowError("Photo mode main modifiers", mainModifiersSig);
            goto skipScans;
        }

        var mainRelative = GetInstance().ReadMemory<int>(MainModifiersAddress + 2);
        MainModifiersAddress = (nuint)((nint)MainModifiersAddress + mainRelative + 6);
        ++successCount;

        const string speedSig = "F3 0F ? ? ? ? ? ? F3 0F ? ? ? ? ? ? 44 0F ? ? ? ? ? ? 44 0F ? ? ? ? ? ? 0F C6 C9";
        SpeedAddress = await SmartAobScan(speedSig);
        if (SpeedAddress <= 0)
        {
            ShowError("Photo mode speed modifiers", speedSig);
            goto skipScans;
        }

        var speedRelative = GetInstance().ReadMemory<int>(SpeedAddress + 4);
        SpeedAddress = (nuint)((nint)SpeedAddress + speedRelative + 8);
        ++successCount;
        
        skipScans:
        WasModifiersScanSuccessful = successCount == 2;
    }
    
    public void Cleanup()
    {
        var mem = GetInstance();
        
        if (_noHeightLimitAddress > 0)
        {
            mem.WriteArrayMemory(_noHeightLimitAddress, new byte[] { 0xF2, 0x0F, 0x10, 0x9E, 0xE0, 0x02, 0x00, 0x00 });
            Free(NoHeightLimitDetourAddress);
        }

        if (_noClipAddress > 0)
        {
            mem.WriteArrayMemory(_noClipAddress, new byte[] { 0x0F, 0x11, 0x44, 0x24, 0x34 });
            Free(NoClipDetourAddress);
        }

        if (_increasedZoomAddress > 0)
        {
            mem.WriteArrayMemory(_increasedZoomAddress, new byte[] { 0xFF, 0x90, 0x30, 0x03, 0x00, 0x00 });
            Free(IncreasedZoomDetourAddress);
        }
    }

    public void Reset()
    {
        WasModifiersScanSuccessful = false;
        var fields = typeof(PhotomodeCheats).GetFields().Where(f => f.FieldType == typeof(UIntPtr));
        foreach (var field in fields)
        {
            field.SetValue(this, UIntPtr.Zero);
        }
    }
}