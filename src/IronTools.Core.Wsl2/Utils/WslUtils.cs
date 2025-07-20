using IronTools.Core.Cmd.Utils;

namespace IronTools.Core.Wsl2.Utils;

public class WslUtils
{
    public static bool CheckIfWslIsInstalled()
    {
        string powershellCommand = "$vmPlatform = Get-WindowsOptionalFeature -Online -FeatureName VirtualMachinePlatform | Select-Object State; " +
                                   "$outputFile = Join-Path $env:TEMP 'vm_platform_status.txt'; " +
                                   "if ($vmPlatform -ne $null) { " +
                                   "    if ($vmPlatform.State -eq 'Enabled') { " +
                                   "        'Virtual Machine Platform feature is enabled.' | Out-File -FilePath $outputFile; " +
                                   "    } elseif ($vmPlatform.State -eq 'Disabled') { " +
                                   "        'Virtual Machine Platform feature is installed but disabled.' | Out-File -FilePath $outputFile; " +
                                   "    } " +
                                   "} else { " +
                                   "    'Virtual Machine Platform feature is not installed.' | Out-File -FilePath $outputFile; " +
                                   "} " +
                                   "$vmPlatform.State | Out-File -FilePath $outputFile -Append";
        PowershellUtils.RestartElevatedPowerShell([powershellCommand]);
        
        string tempFilePath = Path.Combine(Path.GetTempPath(), "vm_platform_status.txt");
        string[] fileLines = [];
        try
        {
            fileLines = File.ReadAllLines(tempFilePath);
            // Optional: Clean up the file after reading
            File.Delete(tempFilePath);
            // Use the content as needed
            // Console.WriteLine("File content:");
            // Console.WriteLine(fileLines.GetValue(0));
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("VM platform status file not found.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }
        if (fileLines[0].Contains("Virtual Machine Platform feature is enabled."))
        {
            Console.WriteLine("Virtual Machine Platform feature is enabled.");
            return true;
        }
        if (fileLines[0].Contains("Virtual Machine Platform feature is installed but disabled."))
        {
            Console.WriteLine("Virtual Machine Platform feature is installed but disabled.");
            return false;
        }
        if (fileLines[0].Contains("Virtual Machine Platform feature is not installed."))
        {
            Console.WriteLine("Virtual Machine Platform feature is not installed.");
            return false;
        }
        Console.WriteLine("Unknown state.");
        return false;
        
        
        // // PowerShell command to check VirtualMachinePlatform feature
        // string powerShellCommand =
        //     "$vmPlatform = Get-WindowsOptionalFeature -Online -FeatureName VirtualMachinePlatform | Select-Object State; " +
        //     "if ($vmPlatform -ne $null) { " +
        //     "    if ($vmPlatform.State -eq 'Enabled') { " +
        //     "        Write-Output 'Virtual Machine Platform feature is enabled.'; " +
        //     "    } elseif ($vmPlatform.State -eq 'Disabled') { " +
        //     "        Write-Output 'Virtual Machine Platform feature is installed but disabled.'; " +
        //     "    } " +
        //     "} else { " +
        //     "    Write-Output 'Virtual Machine Platform feature is not installed.'; " +
        //     "} " +
        //     "$vmPlatform.State";
        //
        // string result = ExecutePowerShellCommand(powerShellCommand);
        //
        // // Process the result
        // if (result.Contains("Virtual Machine Platform feature is enabled."))
        // {
        //     Console.WriteLine("Virtual Machine Platform feature is enabled.");
        //     return true;
        // }
        // if (result.Contains("Virtual Machine Platform feature is installed but disabled."))
        // {
        //     Console.WriteLine("Virtual Machine Platform feature is installed but disabled.");
        //     return false;
        // }
        // if (result.Contains("Virtual Machine Platform feature is not installed."))
        // {
        //     Console.WriteLine("Virtual Machine Platform feature is not installed.");
        //     return false;
        // }
        // Console.WriteLine("Unknown state.");
        // return false;
    }

    public static void InstallWsl2()
    {
        CmdUtils.RestartElevatedCmd(["wsl", "--install"]);
    }
}