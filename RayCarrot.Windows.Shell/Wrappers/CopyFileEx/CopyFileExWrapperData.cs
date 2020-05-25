using RayCarrot.IO;
using System.Threading;

namespace RayCarrot.Windows.Shell
{
    /// <summary>
    /// The data for a <see cref="CopyFileExWrapper"/> operation
    /// </summary>
    public class CopyFileExWrapperData
    {
        /// <summary>
        /// Creates a new instance of <see cref="CopyFileExWrapperData"/>
        /// </summary>
        /// <param name="source">The source file</param>
        /// <param name="destination">The destination file</param>
        /// <param name="options">The options</param>
        /// <param name="callback">The operation callback</param>
        /// <param name="state">The operation state</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public CopyFileExWrapperData(FileSystemPath source, FileSystemPath destination, CopyFileOptions options, CopyFileCallback callback, object state, CancellationToken cancellationToken)
        {
            Source = source;
            Destination = destination;
            Options = options;
            Callback = callback;
            State = state;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// The source file
        /// </summary>
        public FileSystemPath Source { get; }

        /// <summary>
        /// The destination file
        /// </summary>
        public FileSystemPath Destination { get; }

        /// <summary>
        /// The options
        /// </summary>
        public CopyFileOptions Options { get; }

        /// <summary>
        /// The operation callback
        /// </summary>
        public CopyFileCallback Callback { get; }

        /// <summary>
        /// The operation state
        /// </summary>
        public object State { get; }

        /// <summary>
        /// The cancellation token
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// The callback for when copying a file using <see cref="CopyFileExWrapper"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="state"></param>
        /// <param name="totalFileSize"></param>
        /// <param name="totalBytesTransferred"></param>
        /// <returns></returns>
        public delegate CopyFileCallbackAction CopyFileCallback(FileSystemPath source, FileSystemPath destination, object state, long totalFileSize, long totalBytesTransferred);
    }
}