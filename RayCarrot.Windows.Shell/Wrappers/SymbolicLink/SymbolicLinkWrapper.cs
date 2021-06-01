using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace RayCarrot.Windows.Shell
{
    /// <summary>
    /// A wrapper for creating a symbolic link
    /// </summary>
    public static class SymbolicLinkWrapper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool _CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLinkFlags dwFlags);

        /// <summary>
        /// Creates a symbolic link. This requires administration privileges and is only supported on Windows Vista and above.
        /// </summary>
        /// <param name="symLinkPath">The path for the symbolic link. Can be relative or absolute.</param>
        /// <param name="targetPath">The path for the target of the symbolic link. Can be relative or absolute.</param>
        /// <param name="flags">The flags to use</param>
        /// <exception cref="IOException">An error occurred during the operation</exception>
        public static void CreateSymbolicLink(string symLinkPath, string targetPath, SymbolicLinkFlags flags)
        {
            if (_CreateSymbolicLink(symLinkPath, targetPath, flags))
                return;

            // Get reason for failure if it failed
            var ex = new Win32Exception();
            throw new IOException(ex.Message, ex);
        }
    }
}