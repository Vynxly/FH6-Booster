using static Forza_Mods_AIO.Resources.Imports;
using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Cheats.ForzaHorizon4;

// ReSharper disable once ClassNeverInstantiated.Global
public class Bypass : ICheatsBase
{
    private static readonly byte[] RtlUserThreadStartPatch = [0x48, 0x83, 0xEC, 0x78, 0x4C, 0x8B, 0xC2];
    private static readonly byte[] NtCreateThreadExPatch = [0x4C, 0x8B, 0xD1, 0xB8, 0xC7, 0x00, 0x00, 0x00];

    private byte[] _rtlUserThreadStartOrig = null!;
    private byte[] _ntCreateThreadExOrig = null!;
    
    public void DisableCreateRemoteThreadChecks()
    {
        var mem = GetInstance();
        var ntDll = GetModuleHandle("ntdll.dll");
        var rtlUserThreadStart = GetProcAddress(ntDll, "RtlUserThreadStart");
        var ntCreateThreadEx = GetProcAddress(ntDll, "NtCreateThreadEx");
        _rtlUserThreadStartOrig = mem.ReadArrayMemory<byte>(rtlUserThreadStart, 7);
        _ntCreateThreadExOrig = mem.ReadArrayMemory<byte>(ntCreateThreadEx, 8);
        mem.WriteArrayMemory(rtlUserThreadStart, RtlUserThreadStartPatch);
        mem.WriteArrayMemory(ntCreateThreadEx, NtCreateThreadExPatch);
    }
    
    public void Cleanup()
    {
        var ntDll = GetModuleHandle("ntdll.dll");
        var mem = GetInstance();

        if (_rtlUserThreadStartOrig != null!)
        {
            var rtlUserThreadStart = GetProcAddress(ntDll, "RtlUserThreadStart");
            mem.WriteArrayMemory(rtlUserThreadStart, _rtlUserThreadStartOrig);
        }

        if (_ntCreateThreadExOrig == null!) return;
        var ntCreateThreadEx = GetProcAddress(ntDll, "NtCreateThreadEx");
        mem.WriteArrayMemory(ntCreateThreadEx, _ntCreateThreadExOrig);
    }

    public void Reset()
    {        
        _rtlUserThreadStartOrig = null!;
        _ntCreateThreadExOrig = null!;
    }
}