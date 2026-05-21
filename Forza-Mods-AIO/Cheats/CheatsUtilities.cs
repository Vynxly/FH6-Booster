using System.Windows;
using Forza_Mods_AIO.Models;
using MahApps.Metro.Controls;
using Memory;
using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Cheats;

public class CheatsUtilities
{
    protected static async Task<nuint> SmartAobScan(string search, UIntPtr? start = null, UIntPtr? end = null)
    {
        Imps.GetSystemInfo(out var info);

        var handle = GetInstance().MProc.Handle;
        var minRange = (long)GetInstance().MProc.Process.MainModule!.BaseAddress;
        var maxRange = minRange + GetInstance().MProc.Process.MainModule!.ModuleMemorySize;

        if (start != null)
        {
            minRange = (long)start;
        }
        
        if (end != null)
        {
            maxRange = (long)end;
        }
        
        var scanStartAddr = minRange;
        var address = (UIntPtr)minRange;

        try
        {
            while (address < (ulong)maxRange)
            {
                Imps.Native_VirtualQueryEx(handle, address, out Imps.MemoryBasicInformation64 memInfo, info.PageSize);
                if (address == memInfo.BaseAddress + memInfo.RegionSize)
                {
                    break;
                }

                var scanEndAddr = (long)memInfo.BaseAddress + (long)memInfo.RegionSize;

                nuint retAddress;
                if (scanEndAddr - scanStartAddr > 500000000)
                {
                    retAddress = await ScanRange(search, scanStartAddr, scanEndAddr);
                }
                else
                {
                    retAddress = (await GetInstance().AoBScan(scanStartAddr, scanEndAddr, search)).FirstOrDefault();
                }

                if (retAddress != 0)
                {
                    return retAddress;
                }

                scanStartAddr = scanEndAddr;
                address = memInfo.BaseAddress + checked((UIntPtr)memInfo.RegionSize);
            }
        }
        catch
        {
            // ignored
        }

        return 0;
    }

    private static async Task<nuint> ScanRange(string search, long startAddr, long endAddr)
    {
        var end = startAddr + (endAddr - startAddr) / 2;
        var retAddress = (await GetInstance().AoBScan(startAddr, end, search)).FirstOrDefault();
        return retAddress;
    }
    
    protected static void ShowError(string feature, string sig)
    {
        MessageBox.Show(
            $"Address for this feature wasn't found!\nPlease try to activate the cheat again or try to restart the game and the tool.\n\nIf this error still occurs, please (Press Ctrl+C) to copy, and make an issue on the github repository or post the copied text on the Vynx.dev support page in our discord server (vynx.dev).\n\nFeature: {feature}\nSignature: {sig}\n\nTool Version: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}\nGame: {GameVerPlat.GetInstance().Name}\nGame Version: {GameVerPlat.GetInstance().Update}\nPlatform: {GameVerPlat.GetInstance().Platform}",
            $"{App.GetRequiredService<MetroWindow>().Title} - Error", 0, MessageBoxImage.Error);
    }

    protected static void Free(UIntPtr address)
    {
        if (address == 0) return;
        var handle = GetInstance().MProc.Handle;
        Imps.VirtualFreeEx(handle, address,0, Imps.MemRelease);
    }
    
    protected static byte[] CalculateDetour(nuint address, nuint target, int replaceCount)
    {
        var detourBytes = new byte[replaceCount];
        detourBytes[0] = 0xE9;
        BitConverter.GetBytes((int)((long)target - (long)address - 5)).CopyTo(detourBytes, 1);
        
        for (var i = 5; i < detourBytes.Length; i++)
        {
            detourBytes[i] = 0x90;
        }

        return detourBytes;
    }
}
