namespace RayCarrot.Windows.Shell
{
    /// <summary>
    /// Available actions for a copy file callback
    /// </summary>
    public enum CopyFileCallbackAction
    {
        /// <summary>
        /// Continue the copy operation
        /// </summary>
        Continue = 0,

        /// <summary>
        /// Cancel the copy operation and delete the destination file
        /// </summary>
        Cancel = 1,

        /// <summary>
        /// Stop the copy operation. It can be restarted at a later time.
        /// </summary>
        Stop = 2,

        /// <summary>
        /// Continue the copy operation, but stop invoking <see cref="CopyFileExWrapperData.CopyFileCallback"/> to report progress
        /// </summary>
        Quiet = 3
    }
}