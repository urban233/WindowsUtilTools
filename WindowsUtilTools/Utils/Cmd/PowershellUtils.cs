using System.Diagnostics;

namespace Utils.Cmd;

public static class PowershellUtils
{
    public static void RestartElevatedPowerShell(string[] args)
    {
        string arguments = string.Join(" ", args);

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-Command \"{arguments}\"",
            Verb = "runas",
            UseShellExecute = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        try
        {
            Process process = Process.Start(startInfo);
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine("The process could not be started: " + ex.Message);
        }
    }
}