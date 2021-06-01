using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace RayCarrot.Windows.Shell
{
    /// <summary>
    /// A wrapper class for CopyFileEx
    /// </summary>
    /// <remarks>
    /// Complete documentation: https://docs.microsoft.com/en-us/windows/desktop/api/winbase/nc-winbase-lpprogress_routine
    /// </remarks>
    public class CopyFileExWrapper
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data">The data for this operation</param>
        public CopyFileExWrapper(CopyFileExWrapperData data)
        {
            Data = data;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies a file
        /// </summary>
        /// <exception cref="FileNotFoundException">The source does not exist</exception>
        /// <exception cref="IOException">An error occurred during the operation</exception>
        public void CopyFile()
        {
            if (!File.Exists(Data.Source))
                throw new FileNotFoundException("source does not exist");

            // Create the handler if we got a callback
            CopyProgressRoutine cpr = Data.Callback == null ? null : new CopyProgressRoutine(CallbackHandler);

            // Create the reference boolean
            bool cancel = false;

            // Change the boolean to false if the cancellation token is called
            Data.CancellationToken.Register(() => cancel = true);

            // Run the operation
            if (!CopyFileEx(Data.Source, Data.Destination, cpr, IntPtr.Zero, ref cancel, (int)Data.Options))
            {
                // Get reason for failure if it failed
                var ex = new Win32Exception();
                throw new IOException(ex.Message, ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The complete callback handler, calling the specified callback in the data
        /// </summary>
        /// <param name="totalFileSize">The total size of the file, in bytes</param>
        /// <param name="totalBytesTransferred">The total number of bytes transferred from the source file to the destination file since the copy operation began</param>
        /// <param name="streamSize">The total size of the current file stream, in bytes</param>
        /// <param name="streamBytesTransferred">The total number of bytes in the current stream that have been transferred from the source file to the destination file since the copy operation began</param>
        /// <param name="streamNumber">A handle to the current stream. The first time <see cref="CopyProgressRoutine"/> is called, the stream number is 1.</param>
        /// <param name="callbackReason">The reason that CopyProgressRoutine was called</param>
        /// <param name="sourceFile">A handle to the source file</param>
        /// <param name="destinationFile">A handle to the destination file</param>
        /// <param name="data">The state object passed in</param>
        /// <returns>The callback action</returns>
        private int CallbackHandler(long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred,
            int streamNumber, int callbackReason, IntPtr sourceFile, IntPtr destinationFile, IntPtr data)
        {
            return (int)Data.Callback.Invoke(Data.Source, Data.Destination, Data.State, totalFileSize, totalBytesTransferred);
        }

        #endregion

        #region External Methods

        /// <summary>
        /// Copies an existing file to a new file, notifying the application of its progress through a callback function
        /// </summary>
        /// <param name="lpExistingFileName">The name of an existing file</param>
        /// <param name="lpNewFileName">The name of the new file</param>
        /// <param name="lpProgressRoutine">The address of a callback function of type LPPROGRESS_ROUTINE that is called each time another portion of the file has been copied. This parameter can be NULL.</param>
        /// <param name="lpData">The argument to be passed to the callback function. This parameter can be NULL.</param>
        /// <param name="pbCancel">If this flag is set to TRUE during the copy operation, the operation is canceled. Otherwise, the copy operation will continue to completion.</param>
        /// <param name="dwCopyFlags">Flags that specify how the file is to be copied. This parameter can be a combination of the following values.</param>
        /// <returns>True if succeeded, false if not</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName,
            CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref bool pbCancel, int dwCopyFlags);

        #endregion

        #region Public Properties

        /// <summary>
        /// The data for this operation
        /// </summary>
        public CopyFileExWrapperData Data { get; }

        #endregion

        #region Private Delegates

        /// <summary>
        /// The callback signature for an IO operation
        /// </summary>
        /// <param name="totalFileSize">The total size of the file, in bytes</param>
        /// <param name="totalBytesTransferred">The total number of bytes transferred from the source file to the destination file since the copy operation began</param>
        /// <param name="streamSize">The total size of the current file stream, in bytes</param>
        /// <param name="streamBytesTransferred">The total number of bytes in the current stream that have been transferred from the source file to the destination file since the copy operation began</param>
        /// <param name="streamNumber">A handle to the current stream. The first time <see cref="CopyProgressRoutine"/> is called, the stream number is 1.</param>
        /// <param name="callbackReason">The reason that CopyProgressRoutine was called</param>
        /// <param name="sourceFile">A handle to the source file</param>
        /// <param name="destinationFile">A handle to the destination file</param>
        /// <param name="data">The state object passed in</param>
        /// <returns>The callback action</returns>
        private delegate int CopyProgressRoutine(long totalFileSize, long totalBytesTransferred, long streamSize,
            long streamBytesTransferred, int streamNumber, int callbackReason, IntPtr sourceFile, IntPtr destinationFile, IntPtr data);

        #endregion
    }
}