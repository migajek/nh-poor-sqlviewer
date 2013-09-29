/*
 * Created by SharpDevelop.
 * User: michal
 * Date: 2013-09-27
 * Time: 17:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace UdpLogListener
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.textBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
			this.tbLog = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(3, 12);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(408, 303);
			this.listBox1.TabIndex = 0;
			this.listBox1.Click += new System.EventHandler(this.ListBox1Click);
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.ListBox1SelectedIndexChanged);
			// 
			// textBox1
			// 
			this.textBox1.AutoScrollMinSize = new System.Drawing.Size(25, 15);
			this.textBox1.BackBrush = null;
			this.textBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.textBox1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.textBox1.Language = FastColoredTextBoxNS.Language.SQL;
			this.textBox1.Location = new System.Drawing.Point(417, 12);
			this.textBox1.Name = "textBox1";
			this.textBox1.Paddings = new System.Windows.Forms.Padding(0);
			this.textBox1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.textBox1.Size = new System.Drawing.Size(448, 303);
			this.textBox1.TabIndex = 1;
			// 
			// tbLog
			// 
			this.tbLog.Location = new System.Drawing.Point(3, 336);
			this.tbLog.Multiline = true;
			this.tbLog.Name = "tbLog";
			this.tbLog.Size = new System.Drawing.Size(862, 84);
			this.tbLog.TabIndex = 2;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(877, 451);
			this.Controls.Add(this.tbLog);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.listBox1);
			this.Name = "MainForm";
			this.Text = "Simple NHibernate SQL Viewer / Copyright Michal migajek Gajek";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TextBox tbLog;
		private FastColoredTextBoxNS.FastColoredTextBox textBox1;
		private System.Windows.Forms.ListBox listBox1;
	}
}
