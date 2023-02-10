using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoMuteUsPortable.Shared.Utility
{
    public static class ProcessExtensions
    {
        [DllImport("kernel32.dll")]
        private static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool FreeConsole();

        public static void GracefullyClose(this Process process, int? timeout = null)
        {
            FreeConsole();
            AttachConsole((uint)process.Id);
            GenerateConsoleCtrlEvent(0, 0);
            if (timeout != null) process.WaitForExit((int)timeout);
            else process.WaitForExit();
        }

        public static async Task GracefullyCloseAsync(this Process process,
            CancellationToken cancellationToken = default)
        {
            FreeConsole();
            AttachConsole((uint)process.Id);
            GenerateConsoleCtrlEvent(0, 0);
            await process.WaitForExitAsync(cancellationToken);
        }
    }
}
