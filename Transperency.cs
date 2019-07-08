using System;
using System.Runtime.InteropServices;

namespace WARTLS
{
	internal class Transperency
	{
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		[DllImport("user32.dll")]
		private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GetConsoleWindow();
		internal static void Set(byte Alpha = 227)
		{
			int nIndex = -20;
			int num = 524288;
			uint dwFlags = 2u;
			IntPtr consoleWindow = GetConsoleWindow();
			SetWindowLong(consoleWindow, nIndex, GetWindowLong(consoleWindow, nIndex).ToInt32() | num);
			SetLayeredWindowAttributes(consoleWindow, 0u, Alpha, dwFlags);
		}
	}
}
