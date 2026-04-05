using System;
using System.Runtime.InteropServices;

namespace MouseOptimizer.Models
{
    /// <summary>
    /// Interop helpers for reading and writing Windows mouse settings via the registry and Win32 APIs.
    /// No game injection, macro, or exploit functionality — only standard Windows API calls.
    /// </summary>
    public static class SystemMouseHelper
    {
        // Win32: SystemParametersInfo action codes
        private const uint SPI_SETMOUSE = 0x0004;
        private const uint SPI_GETMOUSE = 0x0003;
        private const uint SPI_SETMOUSESPEED = 0x0071;
        private const uint SPI_GETMOUSESPEED = 0x0070;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDCHANGE = 0x02;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, int[] pvParam, uint fWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

        /// <summary>
        /// Disables or enables Windows pointer acceleration (Enhance Pointer Precision).
        /// </summary>
        public static bool SetPointerAcceleration(bool enable)
        {
            try
            {
                // Mouse params: [threshold1, threshold2, acceleration]
                // acceleration=0 → off, acceleration=1 → standard
                int[] mouseParams = enable ? new[] { 6, 10, 1 } : new[] { 0, 0, 0 };
                return SystemParametersInfo(SPI_SETMOUSE, 0, mouseParams,
                    SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads current Windows mouse speed (1–20).
        /// </summary>
        public static int GetMouseSpeed()
        {
            try
            {
                int speed = 10;
                SystemParametersInfo(SPI_GETMOUSESPEED, 0, ref speed, 0);
                return speed;
            }
            catch
            {
                return 10;
            }
        }

        /// <summary>
        /// Sets Windows mouse speed (1–20). Maps our 0.1–10 range to 1–20.
        /// </summary>
        public static bool SetMouseSpeed(double sensitivityValue)
        {
            try
            {
                // Map 0.1–10.0 → 1–20
                int winSpeed = (int)Math.Round(sensitivityValue * 2.0);
                winSpeed = Math.Clamp(winSpeed, 1, 20);
                int param = winSpeed;
                return SystemParametersInfo(SPI_SETMOUSESPEED, 0, ref param,
                    SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if pointer acceleration is currently enabled.
        /// </summary>
        public static bool IsPointerAccelerationEnabled()
        {
            try
            {
                int[] mouseParams = new int[3];
                SystemParametersInfo(SPI_GETMOUSE, 0, mouseParams, 0);
                return mouseParams[2] != 0;
            }
            catch
            {
                return true;
            }
        }
    }
}
