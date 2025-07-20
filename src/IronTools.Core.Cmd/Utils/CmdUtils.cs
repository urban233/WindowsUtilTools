using System.Diagnostics;
using System.Security.Principal;

namespace IronTools.Core.Cmd.Utils;

public static class CmdUtils
{
    /// <summary>
    /// Restarts the current application with elevated (administrator) privileges.
    /// </summary>
    /// <param name="args">The command-line arguments to pass to the application.</param>
    public static void RestartElevatedCmd(string[] args)
    {
        string arguments = string.Join(" ", args);

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {arguments}",
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
    
    /// <summary>
    /// Checks if the current user has administrator privileges.
    /// </summary>
    /// <returns>True if the current user is an administrator; otherwise, false.</returns>
    public static bool IsAdministrator()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}