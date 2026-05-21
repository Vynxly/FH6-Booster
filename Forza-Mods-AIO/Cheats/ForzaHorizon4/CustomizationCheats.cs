namespace Forza_Mods_AIO.Cheats.ForzaHorizon4;

public class CustomizationCheats : CheatsUtilities, ICheatsBase
{
    private UIntPtr _paintAddress;
    public UIntPtr PaintDetourAddress;
    private UIntPtr _cleanlinessAddress;
    public UIntPtr CleanlinessDetourAddress;
    
    public async Task CheatGlowingPaint()
    {
        _paintAddress = 0;
        PaintDetourAddress = 0;

        const string sig = "41 0F ? ? ? 41 C6 02";
        _paintAddress = await SmartAobScan(sig);
        
        if (_paintAddress > 0)
        {
            var asm = new byte[]
            {
                0x80, 0x3D, 0x2D, 0x00, 0x00, 0x00, 0x01, 0x75, 0x21, 0x48, 0x83, 0xEC, 0x10, 0xF3, 0x0F, 0x7F, 0x14,
                0x24, 0xF3, 0x0F, 0x10, 0x15, 0x1B, 0x00, 0x00, 0x00, 0x0F, 0xC6, 0xD2, 0x00, 0x0F, 0x59, 0xCA, 0xF3,
                0x0F, 0x6F, 0x14, 0x24, 0x48, 0x83, 0xC4, 0x10, 0x41, 0x0F, 0x11, 0x4A, 0x10
            };

            PaintDetourAddress = Resources.Memory.GetInstance().CreateDetour(_paintAddress, asm, 5);
            return;
        }
        
        ShowError("Glowing paint", sig);
    }

    public async Task CheatCleanliness()
    {
        _cleanlinessAddress = 0;
        CleanlinessDetourAddress = 0;

        const string sig = "F3 0F ? ? ? ? ? ? F3 0F ? ? ? ? B9 ? ? ? ? E8";
        _cleanlinessAddress = await SmartAobScan(sig);

        if (_cleanlinessAddress > 0)
        {
            var asm = new byte[]
            {
                0x80, 0x3D, 0x30, 0x00, 0x00, 0x00, 0x01, 0x75, 0x0C, 0x8B, 0x0D, 0x29, 0x00, 0x00, 0x00, 0x89, 0x88,
                0x7C, 0x8C, 0x00, 0x00, 0x80, 0x3D, 0x20, 0x00, 0x00, 0x00, 0x01, 0x75, 0x0C, 0x8B, 0x0D, 0x19, 0x00,
                0x00, 0x00, 0x89, 0x88, 0x80, 0x8C, 0x00, 0x00, 0xF3, 0x0F, 0x10, 0x88, 0x84, 0x8C, 0x00, 0x00
            };

            CleanlinessDetourAddress = Resources.Memory.GetInstance().CreateDetour(_cleanlinessAddress, asm, 8);
            return;
        }
        
        ShowError("Cleanliness", sig);
    }

    public void Cleanup()
    {
        var mem = Resources.Memory.GetInstance();
        
        if (_paintAddress > 0)
        {
            mem.WriteArrayMemory(_paintAddress, new byte[] { 0x41, 0x0F, 0x11, 0x4A, 0x10 });
            Free(PaintDetourAddress);
        }

        if (_cleanlinessAddress <= 0) return;
        mem.WriteArrayMemory(_cleanlinessAddress, new byte[] { 0xF3, 0x0F, 0x10, 0x88, 0x84, 0x8C, 0x00, 0x00 });
        Free(CleanlinessDetourAddress);
    }

    public void Reset()
    {
        var fields = typeof(CustomizationCheats).GetFields().Where(f => f.FieldType == typeof(UIntPtr));
        foreach (var field in fields)
        {
            field.SetValue(this, UIntPtr.Zero);
        }
    }
}