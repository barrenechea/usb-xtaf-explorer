using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace New_Gui;

public class updater
{
	public XmlTextReader reader;

	public updater(Form1 mainform)
	{
		new Thread(updater_run).Start();
	}

	public void updater_run()
	{
		Version version = null;
		string address = "";
		try
		{
			string url = "http://gruntmods.com/Projects/Downloads/USB%20XTAF/test2.xml";
			reader = new XmlTextReader(url);
			try
			{
				reader.MoveToContent();
			}
			catch
			{
			}
			string text = "";
			if (reader.NodeType == XmlNodeType.Element && reader.Name == "xtaf_gui")
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						text = reader.Name;
					}
					else if (reader.NodeType == XmlNodeType.Text && reader.HasValue)
					{
						switch (text)
						{
						case "version":
							version = new Version(reader.Value);
							break;
						case "url":
							address = reader.Value;
							break;
						case "motd":
							Program.mainform.toolStripStatusLabel1.Text = Program.mainform.toolStripStatusLabel1.Text + " ---- MOTD: " + reader.Value;
							break;
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			if (reader != null)
			{
				reader.Close();
			}
		}
		if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(version) < 0)
		{
			string caption = "New version detected.";
			string text2 = "Download the new version?";
			if (DialogResult.Yes == MessageBox.Show(Program.mainform, text2, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
			{
				new WebClient().DownloadFile(address, "USBXTAFGUI_v" + Convert.ToString(version.Revision) + ".exe");
				Process.Start(Application.StartupPath + "\\USBXTAFGUI_v" + Convert.ToString(version.Revision) + ".exe");
				Environment.Exit(0);
			}
		}
	}
}
