using Microsoft.WindowsAPICodePack.Shell;
using RayCarrot.IO;
using System.Drawing;

namespace RayCarrot.Windows.Shell
{
    /// <summary>
    /// Extension methods for <see cref="FileSystemPath"/>
    /// </summary>
    public static class FileSystemPathExtensions
    {
        /// <summary>
        /// Gets the icon or thumbnail for a file or directory
        /// </summary>
        /// <param name="path">The path of the file or directory to get the icon or thumbnail for</param>
        /// <param name="shellThumbnailSize">The size of the icon or thumbnail</param>
        /// <param name="getIcon">True if the icon should be returned or false if the thumbnail should be returned</param>
        /// <returns>The icon or thumbnail</returns>
        public static Bitmap GetIconOrThumbnail(this FileSystemPath path, ShellThumbnailSize shellThumbnailSize, bool getIcon = true)
        {
            using (var shellObject = ShellObject.FromParsingName(path))
            {
                var thumb = shellObject.Thumbnail;
                thumb.FormatOption = getIcon ? ShellThumbnailFormatOption.IconOnly : ShellThumbnailFormatOption.ThumbnailOnly;
                return thumb.GetTransparentBitmap(shellThumbnailSize);
            }
        }
    }
}