using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XtafRewrite;

namespace New_Gui;

public class Form3 : Form
{
	private Form1 mainform;

	private Xtaf.XtafFileSystem.XtafDirent d;

	private bool leftside;

	private IContainer components;

	private Button button1;

	private TextBox textBox1;

	public Form3(Form1 mainform, Xtaf.XtafFileSystem.XtafDirent d, bool leftside)
	{
		InitializeComponent();
		this.leftside = leftside;
		this.mainform = mainform;
		this.d = d;
		textBox1.Text = d.name;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		d.rename(textBox1.Text);
		mainform.reload_cnode();
		mainform.reload();
		Hide();
		if (leftside)
		{
			mainform.reload_pnode();
			mainform.reload();
		}
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
		this.button1 = new System.Windows.Forms.Button();
		this.textBox1 = new System.Windows.Forms.TextBox();
		base.SuspendLayout();
		this.button1.Location = new System.Drawing.Point(197, 13);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 0;
		this.button1.Text = "OK";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.textBox1.Location = new System.Drawing.Point(13, 13);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(178, 20);
		this.textBox1.TabIndex = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 50);
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.button1);
		base.Name = "Form3";
		this.Text = "Rename";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
