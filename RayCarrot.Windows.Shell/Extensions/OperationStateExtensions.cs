using Microsoft.WindowsAPICodePack.Taskbar;
using RayCarrot.Common;

namespace RayCarrot.Windows.Shell
{
    /// <summary>
    /// Extensions methods for <see cref="OperationState"/>
    /// </summary>
    public static class OperationStateExtensions
    {
        /// <summary>
        /// Gets the <see cref="TaskbarProgressBarState"/> from an <see cref="OperationState"/>
        /// </summary>
        /// <param name="operationState">The operation state</param>
        /// <returns>The equivalent task bar progress state</returns>
        public static TaskbarProgressBarState GetTaskbarState(this OperationState operationState)
        {
            switch (operationState)
            {
                default:
                case OperationState.None:
                    return TaskbarProgressBarState.NoProgress;

                case OperationState.Running:
                    return TaskbarProgressBarState.Normal;

                case OperationState.Paused:
                    return TaskbarProgressBarState.Paused;

                case OperationState.Error:
                    return TaskbarProgressBarState.Error;
            }
        }
    }
}