using System.Diagnostics;
//using System.Runtime.InteropServices;

//_ = ShowWindow(GetConsoleWindow(), 0);
_ = Process.Start(new ProcessStartInfo($"https://www.google.com.au/search?q={System.Web.HttpUtility.UrlEncode(string.Join(' ', args))}") { UseShellExecute = true });
Environment.Exit(0);

//[DllImport("kernel32.dll")]
//static extern IntPtr GetConsoleWindow();

//[DllImport("user32.dll")]
//static extern bool ShowWindow( IntPtr HWnd, int NCmdShow );