using System;
using System.Windows.Forms;

namespace New_Gui;

public static class Program
{
	public static Form1 mainform;

	[STAThread]
	private static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		mainform = new Form1();
		Application.Run(mainform);
	}
}
