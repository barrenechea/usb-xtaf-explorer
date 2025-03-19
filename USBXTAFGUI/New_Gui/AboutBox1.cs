using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace New_Gui;

internal class AboutBox1 : Form
{
	private IContainer components;

	private TableLayoutPanel tableLayoutPanel;

	private PictureBox logoPictureBox;

	private Label labelProductName;

	private Label labelVersion;

	private Label labelCopyright;

	private Label labelCompanyName;

	private Button okButton;

	private RichTextBox richTextBox1;

	public string AssemblyTitle
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), inherit: false);
			if (customAttributes.Length != 0)
			{
				AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute)customAttributes[0];
				if (assemblyTitleAttribute.Title != "")
				{
					return assemblyTitleAttribute.Title;
				}
			}
			return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
		}
	}

	public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString();

	public string AssemblyDescription
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyDescriptionAttribute)customAttributes[0]).Description;
		}
	}

	public string AssemblyProduct
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyProductAttribute)customAttributes[0]).Product;
		}
	}

	public string AssemblyCopyright
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
		}
	}

	public string AssemblyCompany
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyCompanyAttribute)customAttributes[0]).Company;
		}
	}

	public AboutBox1()
	{
		InitializeComponent();
		Text = "About USB XTAF Explorer";
		labelProductName.Text = "Usb Xtaf Xplorer Mach3";
		labelVersion.Text = $"Version {AssemblyVersion}" ?? "";
		labelCopyright.Text = AssemblyCopyright;
		labelCompanyName.Text = "Slasher - Darkjump";
		richTextBox1.Text = AssemblyDescription + "\n\nCredits:\nGruntmods:Testing, inspiration, server and code maintenance\nGruntParty:Icon\nCommunity:Bug reports, feedback\nCLK Rebellion: Game name(now broken), inspiration";
	}

	private void okButton_Click(object sender, EventArgs e)
	{
		Hide();
	}

	private void richTextBox1_TextChanged(object sender, EventArgs e)
	{
	}

	private void labelProductName_Click(object sender, EventArgs e)
	{
	}

	private void labelCompanyName_Click(object sender, EventArgs e)
	{
	}

	private void AboutBox1_Load(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(New_Gui.AboutBox1));
		this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.logoPictureBox = new System.Windows.Forms.PictureBox();
		this.labelProductName = new System.Windows.Forms.Label();
		this.labelVersion = new System.Windows.Forms.Label();
		this.labelCopyright = new System.Windows.Forms.Label();
		this.labelCompanyName = new System.Windows.Forms.Label();
		this.okButton = new System.Windows.Forms.Button();
		this.richTextBox1 = new System.Windows.Forms.RichTextBox();
		this.tableLayoutPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.logoPictureBox).BeginInit();
		base.SuspendLayout();
		this.tableLayoutPanel.ColumnCount = 2;
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67f));
		this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
		this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
		this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
		this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 2);
		this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 3);
		this.tableLayoutPanel.Controls.Add(this.okButton, 1, 5);
		this.tableLayoutPanel.Controls.Add(this.richTextBox1, 1, 4);
		this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		this.tableLayoutPanel.RowCount = 6;
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.92453f));
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.83019f));
		this.tableLayoutPanel.Size = new System.Drawing.Size(455, 265);
		this.tableLayoutPanel.TabIndex = 0;
		this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.logoPictureBox.Image = (System.Drawing.Image)resources.GetObject("logoPictureBox.Image");
		this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
		this.logoPictureBox.Name = "logoPictureBox";
		this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
		this.logoPictureBox.Size = new System.Drawing.Size(144, 259);
		this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.logoPictureBox.TabIndex = 12;
		this.logoPictureBox.TabStop = false;
		this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelProductName.Location = new System.Drawing.Point(156, 0);
		this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
		this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
		this.labelProductName.Name = "labelProductName";
		this.labelProductName.Size = new System.Drawing.Size(296, 17);
		this.labelProductName.TabIndex = 19;
		this.labelProductName.Text = "USB XTAF Explorer";
		this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.labelProductName.Click += new System.EventHandler(labelProductName_Click);
		this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelVersion.Location = new System.Drawing.Point(156, 26);
		this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
		this.labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
		this.labelVersion.Name = "labelVersion";
		this.labelVersion.Size = new System.Drawing.Size(296, 17);
		this.labelVersion.TabIndex = 0;
		this.labelVersion.Text = "Version";
		this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelCopyright.Location = new System.Drawing.Point(156, 52);
		this.labelCopyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
		this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
		this.labelCopyright.Name = "labelCopyright";
		this.labelCopyright.Size = new System.Drawing.Size(296, 17);
		this.labelCopyright.TabIndex = 21;
		this.labelCopyright.Text = "Copyright 2015";
		this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelCompanyName.Location = new System.Drawing.Point(156, 78);
		this.labelCompanyName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
		this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 17);
		this.labelCompanyName.Name = "labelCompanyName";
		this.labelCompanyName.Size = new System.Drawing.Size(296, 17);
		this.labelCompanyName.TabIndex = 22;
		this.labelCompanyName.Text = "Slasherking823";
		this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.labelCompanyName.Click += new System.EventHandler(labelCompanyName_Click);
		this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.okButton.Location = new System.Drawing.Point(377, 239);
		this.okButton.Name = "okButton";
		this.okButton.Size = new System.Drawing.Size(75, 23);
		this.okButton.TabIndex = 24;
		this.okButton.Text = "&OK";
		this.okButton.Click += new System.EventHandler(okButton_Click);
		this.richTextBox1.Location = new System.Drawing.Point(153, 107);
		this.richTextBox1.Name = "richTextBox1";
		this.richTextBox1.ReadOnly = true;
		this.richTextBox1.Size = new System.Drawing.Size(299, 120);
		this.richTextBox1.TabIndex = 25;
		this.richTextBox1.Text = "";
		this.richTextBox1.TextChanged += new System.EventHandler(richTextBox1_TextChanged);
		base.AcceptButton = this.okButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(473, 283);
		base.Controls.Add(this.tableLayoutPanel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AboutBox1";
		base.Padding = new System.Windows.Forms.Padding(9);
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "AboutBox1";
		base.Load += new System.EventHandler(AboutBox1_Load);
		this.tableLayoutPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.logoPictureBox).EndInit();
		base.ResumeLayout(false);
	}
}
