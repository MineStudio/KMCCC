namespace KMCCC.Launcher
{
	#region

	using System;
	using System.Runtime.InteropServices;

	#endregion

	public static class LaunchHandleExtensions
	{
		public static bool SetTitle(this LaunchHandle handle, string title)
		{
			try
			{
				SetWindowText(handle.Process.MainWindowHandle, title);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static string GetTile(this LaunchHandle handle)
		{
			try
			{
				return handle.Process.MainWindowTitle;
			}
			catch
			{
				return null;
			}
		}

		public static void Kill(this LaunchHandle handle)
		{
			handle.Process.Kill();
		}


		[DllImport("User32.dll")]
		public static extern int SetWindowText(IntPtr winHandle, string title);
	}
}