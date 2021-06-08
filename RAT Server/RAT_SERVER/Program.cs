using System;
using System.Windows.Forms;

namespace RAT_SERVER
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			Application.Run(new SplashScreen());
			Application.Run(new RemoteCommand());
		}
	}
}
