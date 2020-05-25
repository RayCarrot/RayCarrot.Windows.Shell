using Microsoft.WindowsAPICodePack.Taskbar;
using RayCarrot.Common;
using System.Windows.Forms;

namespace RayCarrot.Windows.Shell
{
    /// <summary>
    /// Extension methods for <see cref="Form"/>
    /// </summary>
    public static class FormExtensions
    {
        /// <summary>
        /// Displays or updates a progress bar hosted in a taskbar button of the given Windows Forms
        /// form to show the specific percentage completed of the full operation.
        /// </summary>
        /// <param name="form">The form whose associated taskbar button is being used as a progress indicator.
        /// This window belong to a calling process associated with the button's application
        /// and must be already loaded.</param>
        /// <param name="currentValue">An application-defined value that indicates the proportion of the operation that
        /// has been completed at the time the method is called.</param>
        /// <param name="maximumValue">An application-defined value that specifies the value currentValue will have
        /// when the operation is complete.</param>
        public static void SetTaskbarProgressValue(this Form form, int currentValue, int maximumValue)
        {
            TaskbarManager.Instance.SetProgressValue(currentValue, maximumValue, form.Handle);
        }

        /// <summary>
        /// Displays or updates a progress bar hosted in a taskbar button of the given Windows Forms
        /// form to show the specific percentage completed of the full operation.
        /// </summary>
        /// <param name="form">The form whose associated taskbar button is being used as a progress indicator.
        /// This window belong to a calling process associated with the button's application
        /// and must be already loaded.</param>
        /// <param name="progress">The progress to display</param>
        public static void SetTaskbarProgressValue(this Form form, Progress progress)
        {
            TaskbarManager.Instance.SetProgressValue(progress, form.Handle);
        }

        /// <summary>
        /// Sets the type and state of the progress indicator displayed on a taskbar button
        /// of the given Windows Forms form
        /// </summary>
        /// <param name="form">The form whose associated taskbar button is being used as a progress indicator.
        /// This window belong to a calling process associated with the button's application
        /// and must be already loaded.</param>
        /// <param name="taskbarProgressBar">Progress state of the progress button</param>
        public static void SetTaskbarProgressState(this Form form, TaskbarProgressBarState taskbarProgressBar)
        {
            TaskbarManager.Instance.SetProgressState(taskbarProgressBar, form.Handle);
        }
    }
}