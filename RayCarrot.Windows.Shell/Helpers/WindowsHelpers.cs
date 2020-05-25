using IWshRuntimeLibrary;
using RayCarrot.Common;
using RayCarrot.IO;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using RayCarrot.Logging;

namespace RayCarrot.Windows.Shell
{
    /// <summary>
    /// Helper methods for managing Windows specific requests
    /// </summary>
    public static class WindowsHelpers
    {
        /// <summary>
        /// Returns true if the current program is running as administrator
        /// </summary>
        public static bool RunningAsAdmin => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        /// <summary>
        /// Opens Windows Explorer in specified path. Supports files and directories.
        /// </summary>
        /// <param name="path">Path to open</param>
        public static void OpenExplorerPath(FileSystemPath path)
        {
            if (path.FileExists)
                Process.Start("explorer.exe", "/select, \"" + path.FullPath + "\"")?.Dispose();
            else
                Process.Start(path)?.Dispose();
        }

        /// <summary>
        /// Opens a specified path in RegEdit
        /// </summary>
        /// <param name="path">The path to open</param>
        public static void OpenRegistryPath(string path)
        {
            // Set the last opened path to the specified path so that will be opened on launch
            RunCommandPromptScript(@"REG ADD " + @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit" + " /v LastKey /t REG_SZ /d \"" + path + "\" /f");

            // Launch the program
            Process.Start("regedit")?.Dispose();
        }

        /// <summary>
        /// Runs a command prompt script as a new process
        /// </summary>
        /// <param name="script">The script to run</param>
        /// <param name="elevated">True if the script should be run in elevated mode</param>
        public static void RunCommandPromptScript(string script, bool elevated = false)
        {
            new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/c" + script,
                    Verb = elevated ? "runas" : null
                }
            }.RunAndDispose(x =>
            {
                x.Start();

                RL.Logger?.LogDebugSource($"A CMD process has started with the script: {script}");

                x.WaitForExit();
            });
        }

        /// <summary>
        /// Creates a shortcut for a file
        /// </summary>
        /// <param name="ShortcutName">The name of the shortcut file</param>
        /// <param name="DestinationDirectory">The destination of the shortcut file</param>
        /// <param name="TargetFile">The file the shortcut targets</param>
        /// <param name="arguments">Optional launch arguments</param>
        public static void CreateFileShortcut(FileSystemPath ShortcutName, FileSystemPath DestinationDirectory, FileSystemPath TargetFile, string arguments = null)
        {
            IWshShortcut shortcut = (IWshShortcut)new WshShell().CreateShortcut(DestinationDirectory + ShortcutName.ChangeFileExtension(new FileExtension(".lnk")));

            shortcut.TargetPath = TargetFile;

            if (arguments != null)
                shortcut.Arguments = arguments;

            shortcut.WorkingDirectory = TargetFile.Parent;

            shortcut.Save();
        }

        /// <summary>
        /// Creates a shortcut for an URL
        /// </summary>
        /// <param name="shortcutName">The name of the shortcut file</param>
        /// <param name="destinationDirectory">The path of the directory</param>
        /// <param name="targetURL">The URL</param>
        public static void CreateURLShortcut(FileSystemPath shortcutName, FileSystemPath destinationDirectory, string targetURL)
        {
            using StreamWriter writer = new StreamWriter(destinationDirectory + shortcutName.ChangeFileExtension(new FileExtension(".url")));

            writer.WriteLine("[InternetShortcut]");
            writer.WriteLine("URL=" + targetURL);
            
            writer.Flush();
        }

        /// <summary>
        /// Gets the target arguments of a shortcut
        /// </summary>
        /// <param name="shortcutPath">The shortcut file</param>
        /// <returns></returns>
        public static string GetShortCutArguments(FileSystemPath shortcutPath) =>
            GetShortCutTargetInfo(shortcutPath).Arguments;

        /// <summary>
        /// Gets the target file/directory of a shortcut
        /// </summary>
        /// <param name="shortcutPath">The shortcut file</param>
        /// <returns></returns>
        public static FileSystemPath GetShortCutTarget(FileSystemPath shortcutPath) =>
            GetShortCutTargetInfo(shortcutPath).TargetPath;

        /// <summary>
        /// Gets the target info of a shortcut
        /// </summary>
        /// <param name="shortcutPath">The shortcut file</param>
        /// <returns>The target info</returns>
        public static IWshShortcut GetShortCutTargetInfo(FileSystemPath shortcutPath) =>
            ((IWshShortcut)new WshShell().CreateShortcut(shortcutPath));

        /// <summary>
        /// Gets the current Windows version
        /// </summary>
        /// <remarks>
        /// If the assembly manifest doesn't explicitly state that it is compatible with
        /// Windows 8.1 and 10 this will return the wrong Windows version
        /// </remarks>
        /// <returns>The Windows version</returns>
        public static WindowsVersion GetCurrentWindowsVersion()
        {
            var version = Environment.OSVersion;

            if (version.Platform == PlatformID.Win32Windows)
            {
                if (version.Version.Minor == 0)
                    return WindowsVersion.Win95;

                if (version.Version.Minor == 10)
                    return WindowsVersion.Win98;

                if (version.Version.Minor == 90)
                    return WindowsVersion.WinMe;
            }
            else if (version.Platform == PlatformID.Win32NT)
            {
                if (version.Version.Major == 4 && version.Version.Minor == 0)
                    return WindowsVersion.WinNT4;

                if (version.Version.Major == 5)
                {
                    if (version.Version.Minor == 0)
                        return WindowsVersion.Win2000;

                    if (version.Version.Minor == 1)
                        return WindowsVersion.WinXP;

                    if (version.Version.Minor == 2)
                        return WindowsVersion.Win2003;
                }

                if (version.Version.Major == 6)
                {
                    if (version.Version.Minor == 0)
                        return WindowsVersion.WinVista;

                    if (version.Version.Minor == 1)
                        return WindowsVersion.Win7;

                    if (version.Version.Minor == 2)
                        return WindowsVersion.Win8;

                    if (version.Version.Minor == 3)
                        return WindowsVersion.Win81;
                }

                if (version.Version.Major == 10 && version.Version.Minor == 0)
                    return WindowsVersion.Win10;
            }
            else
            {
                throw new PlatformNotSupportedException($"The current Windows version can not be retrieved on the platform {version.Platform}");
            }

            return WindowsVersion.Unknown;
        }

        /// <summary>
        /// Marks a file or directory to be deleted on reboot. This requires administration privileges.
        /// </summary>
        /// <param name="targetPath">The target to remove on reboot</param>
        /// <exception cref="Win32Exception"/>
        public static void DeleteOnReboot(FileSystemPath targetPath)
        {
            if (!MoveFileEx(targetPath, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT))
                throw new Win32Exception();
        }

        [Flags]
        private enum MoveFileFlags
        {
            //MOVEFILE_REPLACE_EXISTING = 1,
            //MOVEFILE_COPY_ALLOWED = 2,

            /// <summary>
            /// This value can be used only if the process is in the context of a user who belongs to the administrators group or the LocalSystem account
            /// </summary>
            MOVEFILE_DELAY_UNTIL_REBOOT = 4,
            //MOVEFILE_WRITE_THROUGH = 8
        }

        /// <summary>
        /// Marks the file for deletion during next system reboot
        /// </summary>
        /// <param name="lpExistingFileName">The current name of the file or directory on the local computer.</param>
        /// <param name="lpNewFileName">The new name of the file or directory on the local computer.</param>
        /// <param name="dwFlags">MoveFileFlags</param>
        /// <returns>bool</returns>
        /// <remarks>http://msdn.microsoft.com/en-us/library/aa365240(VS.85).aspx</remarks>
        [DllImport("kernel32.dll", EntryPoint = "MoveFileEx", SetLastError = true)]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);
    }
}