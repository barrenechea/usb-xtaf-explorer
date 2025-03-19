using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace New_Gui;

public class Progress : Form
{
	private IContainer components;

	public ProgressBar progressBar1;

	public Progress()
	{
		InitializeComponent();
		progressBar1.Hide();
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
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		base.SuspendLayout();
		this.progressBar1.Location = new System.Drawing.Point(13, 13);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(259, 23);
		this.progressBar1.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 50);
		base.Controls.Add(this.progressBar1);
		base.Name = "Progress";
		this.Text = "Form4";
		base.ResumeLayout(false);
	}
}
