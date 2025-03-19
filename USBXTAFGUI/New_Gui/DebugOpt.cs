using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace New_Gui;

public class DebugOpt : Form
{
	private IContainer components;

	private CheckBox checkBox1;

	private CheckBox checkBox2;

	private CheckBox checkBox3;

	public DebugOpt()
	{
		InitializeComponent();
		checkBox1.Checked = Program.mainform.CacheDisable;
		checkBox2.Checked = Program.mainform.endianfail;
		checkBox3.Checked = Program.mainform.OldSizeCalc;
	}

	private void checkBox1_CheckedChanged(object sender, EventArgs e)
	{
		Program.mainform.CacheDisable = checkBox1.Checked;
	}

	private void checkBox2_CheckedChanged(object sender, EventArgs e)
	{
		Program.mainform.endianfail = checkBox2.Checked;
	}

	private void checkBox3_CheckedChanged(object sender, EventArgs e)
	{
		Program.mainform.OldSizeCalc = checkBox3.Checked;
	}

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
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.checkBox2 = new System.Windows.Forms.CheckBox();
		this.checkBox3 = new System.Windows.Forms.CheckBox();
		base.SuspendLayout();
		this.checkBox1.AutoSize = true;
		this.checkBox1.Location = new System.Drawing.Point(13, 13);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(101, 17);
		this.checkBox1.TabIndex = 0;
		this.checkBox1.Text = "Disable Cache?";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.checkBox1.CheckedChanged += new System.EventHandler(checkBox1_CheckedChanged);
		this.checkBox2.AutoSize = true;
		this.checkBox2.Location = new System.Drawing.Point(13, 37);
		this.checkBox2.Name = "checkBox2";
		this.checkBox2.Size = new System.Drawing.Size(433, 17);
		this.checkBox2.TabIndex = 1;
		this.checkBox2.Text = "I am a se7ensins kiddie and would like it to  try to use littleendian instead of biggendian";
		this.checkBox2.UseVisualStyleBackColor = true;
		this.checkBox2.CheckedChanged += new System.EventHandler(checkBox2_CheckedChanged);
		this.checkBox3.AutoSize = true;
		this.checkBox3.Location = new System.Drawing.Point(13, 61);
		this.checkBox3.Name = "checkBox3";
		this.checkBox3.Size = new System.Drawing.Size(142, 17);
		this.checkBox3.TabIndex = 2;
		this.checkBox3.Text = "Use Old Size Calculation";
		this.checkBox3.UseVisualStyleBackColor = true;
		this.checkBox3.CheckedChanged += new System.EventHandler(checkBox3_CheckedChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(459, 97);
		base.Controls.Add(this.checkBox3);
		base.Controls.Add(this.checkBox2);
		base.Controls.Add(this.checkBox1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
		base.Name = "DebugOpt";
		this.Text = "DebugOpt";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
