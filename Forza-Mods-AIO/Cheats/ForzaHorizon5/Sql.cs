using Forza_Mods_AIO.Resources;
using Memory;
using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Cheats.ForzaHorizon5;

public class Sql : CheatsUtilities, ICheatsBase
{
    private static readonly string[] DbSigs =
    [
        "48 8B 0D ? ? ? ? 48 8B 01 4C 8D 45 ? 48 8D 55 ? FF 50 48 90 48 8B 4D ? 48 85 C9",
        "0F 84 ? ? ? ? 48 8B 35 ? ? ? ? 48 85 F6 74",
        "0F 85 ? ? ? ? 48 8B 35 ? ? ? ? 48 85 F6 74",
        "48 8B 35 ? ? ? ? 48 85 F6 74",
        "48 8B 35 ? ? ? ? 48 85 F6 0F 84",
        "48 8B 35 ? ? ? ? 48 85 F6 0F 85"
    ];

    private UIntPtr _cDatabaseAddress, _ptr;
    public bool WereScansSuccessful;

    public async Task SqlExecAobScan()
    {
        WereScansSuccessful = false;
        _cDatabaseAddress = 0;
        _ptr = 0;

        foreach (var sig in DbSigs)
        {
            _cDatabaseAddress = await SmartAobScan(sig);
            if (_cDatabaseAddress == 0 || !TryResolveDatabasePointer(_cDatabaseAddress))
            {
                continue;
            }

            WereScansSuccessful = true;
            return;
        }

        ShowError("Sql", string.Join(" | ", DbSigs));
    }

    private bool TryResolveDatabasePointer(nuint matchAddress)
    {
        var memory = GetInstance();

        for (nuint offset = 0; offset < 24; offset++)
        {
            try
            {
                if (memory.ReadMemory<byte>(matchAddress + offset) != 0x48 ||
                    memory.ReadMemory<byte>(matchAddress + offset + 1) != 0x8B)
                {
                    continue;
                }

                var registerByte = memory.ReadMemory<byte>(matchAddress + offset + 2);
                if (registerByte is not 0x0D and not 0x35)
                {
                    continue;
                }

                var relative = memory.ReadMemory<int>(matchAddress + offset + 3);
                var instructionEnd = (long)(matchAddress + offset + 7);
                var pointerAddress = (nuint)(instructionEnd + relative);
                var databasePointer = memory.ReadMemory<nuint>(pointerAddress);
                if (databasePointer == 0 || GetVirtualFunctionPtr(databasePointer, 9) == 0)
                {
                    continue;
                }

                _ptr = databasePointer;
                return true;
            }
            catch (Exception)
            {
            }
        }

        return false;
    }
    
    private static nuint GetVirtualFunctionPtr(nuint ptr, int index)
    {
        var mem = GetInstance();
        var pVTable = mem.ReadMemory<UIntPtr>(ptr);
        var lpBaseAddress = pVTable + (nuint)nuint.Size * (nuint)index;
        var result = mem.ReadMemory<UIntPtr>(lpBaseAddress);
        return result;
    }
    
    public async Task Query(string command)
    {
        var memory = GetInstance();
        var procHandle = memory.MProc.Handle;

        var rcx = _ptr;
        const int virtualFunctionIndex = 9;
        var callFunction = GetVirtualFunctionPtr(_ptr, virtualFunctionIndex);
        if (callFunction <= 0)
        {
            return;
        }
        
        var shellCodeAddress = Imps.VirtualAllocEx(procHandle, 0, 0x1000, 0x3000, 0x40);
        var rdx = Imps.VirtualAllocEx(procHandle, 0, 0x1000, 0x3000, 0x40);
        var r8 = Imps.VirtualAllocEx(procHandle, 0, 0x1000, 0x3000, 0x40);
        var rdxBytes = BitConverter.GetBytes(rdx.ToUInt64());
        var r8Bytes = BitConverter.GetBytes(r8.ToUInt64());
        var callBytes = BitConverter.GetBytes(callFunction.ToUInt64());
        
        byte[] shellCode =
        [
            0x48, 0xBA, rdxBytes[0], rdxBytes[1], rdxBytes[2], rdxBytes[3], rdxBytes[4], rdxBytes[5], rdxBytes[6],
            rdxBytes[7], 0x49, 0xB8, r8Bytes[0], r8Bytes[1], r8Bytes[2], r8Bytes[3], r8Bytes[4], r8Bytes[5], r8Bytes[6],
            r8Bytes[7], 0xFF, 0x25, 0x00, 0x00, 0x00, 0x00, callBytes[0], callBytes[1], callBytes[2], callBytes[3],
            callBytes[4], callBytes[5], callBytes[6], callBytes[7]
        ];
        
        memory.WriteStringMemory(r8, command + "\0");
        memory.WriteArrayMemory(shellCodeAddress, shellCode);
        memory.WriteArrayMemory(callFunction + 41, new byte[] { 0xE9, 0xB6, 0x00, 0x00, 0x00, 0x90 });
        var thread = Imports.CreateRemoteThread(procHandle, 0, 0, shellCodeAddress, rcx, 0, out _);
        await Task.Delay(5);
        memory.WriteArrayMemory(callFunction + 41, new byte[] { 0x0F, 0x85, 0xB5, 0x00, 0x00, 0x00 });
        _ = Imports.WaitForSingleObject(thread, int.MaxValue);
        Imports.CloseHandle(thread);
        Free(shellCodeAddress);
        Free(r8);
        Free(rdx);
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