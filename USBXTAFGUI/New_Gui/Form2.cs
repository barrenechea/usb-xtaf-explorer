using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XtafRewrite;

namespace New_Gui;

public class Form2 : Form
{
	public static string returns = "";

	private Xtaf.XtafFileSystem.XtafDirectory dir;

	private Form1 mainForm;

	private IContainer components;

	private TextBox textBox1;

	private Button button1;

	public Form2(Form1 mainForm, Xtaf.XtafFileSystem.XtafDirectory dir)
	{
		this.mainForm = mainForm;
		InitializeComponent();
		this.dir = dir;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		if (!dir.isInvalid)
		{
			dir.newfolder(textBox1.Text);
			dir.Reload();
			mainForm.reload_cnode();
			mainForm.reload();
		}
		Hide();
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
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.button1 = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.textBox1.Location = new System.Drawing.Point(12, 12);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(183, 20);
		this.textBox1.TabIndex = 0;
		this.button1.Location = new System.Drawing.Point(201, 12);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(71, 23);
		this.button1.TabIndex = 1;
		this.button1.Text = "OK";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 43);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.textBox1);
		base.Name = "Form2";
		this.Text = "New Folder";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
