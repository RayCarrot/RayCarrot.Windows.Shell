namespace RayCarrot.Windows.Shell
{
    /// <summary>
    /// Defines the available flags to use when creating a symbolic link
    /// </summary>
    public enum SymbolicLinkFlags
    {
        /// <summary>
        /// The link target is a file
        /// </summary>
        File = 0,

        /// <summary>
        /// The link target is a directory
        /// </summary>
        Directory = 1,

        /// <summary>
        /// Specify this flag to allow creation of symbolic links when the process is not elevated.
        /// Developer Mode must first be enabled on the machine before this option will function.
        /// </summary>
        Unprivileged = 2
    }
}