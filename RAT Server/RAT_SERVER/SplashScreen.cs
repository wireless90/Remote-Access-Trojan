using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace RAT_SERVER
{
	public class SplashScreen : Form
	{
		private IContainer components = null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RAT_SERVER.SplashScreen));
			SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			BackgroundImage = (System.Drawing.Image)resources.GetObject("$this.BackgroundImage");
			base.ClientSize = new System.Drawing.Size(796, 262);
			base.ControlBox = false;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "SplashScreen";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "SplashScreen";
			base.Shown += new System.EventHandler(SplashScreen_Shown);
			ResumeLayout(false);
		}

		public SplashScreen()
		{
			InitializeComponent();
		}

		private void SplashScreen_Shown(object sender, EventArgs e)
		{
			Thread.Sleep(1500);
			Close();
		}
	}
}
