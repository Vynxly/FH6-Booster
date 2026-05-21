using Forza_Mods_AIO.Resources;
using Memory;
using static Forza_Mods_AIO.Resources.Cheats;
using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Cheats.ForzaHorizon4;

public class Sql : CheatsUtilities, ICheatsBase
{
    private UIntPtr _cDatabaseAddress, _ptr;
    public bool WereScansSuccessful;

    public async Task SqlExecAobScan()
    {
        WereScansSuccessful = false;
        _cDatabaseAddress = 0;
        _ptr = 0;
        
        const string sig = "0F 84 ? ? ? ? 48 8B ? ? ? ? ? 48 8D ? ? ? ? ? 66 0F";
        _cDatabaseAddress = await SmartAobScan(sig);

        if (_cDatabaseAddress > 0)
        {
            var relativeAddress = _cDatabaseAddress + 0x6 + 0x3;
            var relative = GetInstance().ReadMemory<int>(relativeAddress);
            var pCDataBaseAddress = _cDatabaseAddress + (nuint)relative + 0x6 + 0x7;
            _ptr = GetInstance().ReadMemory<nuint>(pCDataBaseAddress);
            WereScansSuccessful = true;
            return;
        }

        ShowError("Sql", sig);
    }
    
    private static nuint GetVirtualFunctionPtr(nuint ptr, int index)
    {
        var mem = GetInstance();
        var pVTable = mem.ReadMemory<UIntPtr>(ptr);
        var lpBaseAddress = pVTable + (nuint)nuint.Size * (nuint)index;
        var result = mem.ReadMemory<UIntPtr>(lpBaseAddress);
        return result;
    }
    
    public Task Query(string command)
    {
        var memory = GetInstance();
        var procHandle = memory.MProc.Handle;

        var rcx = _ptr;
        const int virtualFunctionIndex = 9;
        var callFunction = GetVirtualFunctionPtr(_ptr, virtualFunctionIndex);
        var shellCodeAddress = Imps.VirtualAllocEx(procHandle, 0, 0x1000, 0x3000, 0x40);
        var rdx = Imps.VirtualAllocEx(procHandle, 0, 0x1000, 0x3000, 0x40);
        var r8 = Imps.VirtualAllocEx(procHandle, 0, 0x1000, 0x3000, 0x40);
        var rdxBytes = BitConverter.GetBytes(rdx);
        var r8Bytes = BitConverter.GetBytes(r8);
        var callBytes = BitConverter.GetBytes(callFunction);
        
        byte[] shellCode =
        [
            0x48, 0xBA, rdxBytes[0], rdxBytes[1], rdxBytes[2], rdxBytes[3], rdxBytes[4], rdxBytes[5], rdxBytes[6],
            rdxBytes[7], 0x49, 0xB8, r8Bytes[0], r8Bytes[1], r8Bytes[2], r8Bytes[3], r8Bytes[4], r8Bytes[5], r8Bytes[6],
            r8Bytes[7], 0xFF, 0x25, 0x00, 0x00, 0x00, 0x00, callBytes[0], callBytes[1], callBytes[2], callBytes[3],
            callBytes[4], callBytes[5], callBytes[6], callBytes[7]
        ];
        
        memory.WriteStringMemory(r8, command + "\0");
        memory.WriteArrayMemory(shellCodeAddress, shellCode);
        
        GetClass<Bypass>().DisableCreateRemoteThreadChecks();
        var thread = Imports.CreateRemoteThread(procHandle, 0, 0, shellCodeAddress, rcx, 0, out _);
        _ = Imports.WaitForSingleObject(thread, int.MaxValue);
        Imports.CloseHandle(thread);
        GetClass<Bypass>().Cleanup();
        Free(shellCodeAddress);
        Free(r8);
        Free(rdx);
        return Task.CompletedTask;
    }

    public void Cleanup()
    {
    }

    public void Reset()
    {
        WereScansSuccessful = false;
        var fields = typeof(Sql).GetFields().Where(f => f.FieldType == typeof(UIntPtr));
        foreach (var field in fields)
        {
            field.SetValue(this, UIntPtr.Zero);
        }
    }
}