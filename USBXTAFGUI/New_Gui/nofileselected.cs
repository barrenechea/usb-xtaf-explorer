using System;
using System.Windows.Forms;

namespace New_Gui;

public class nofileselected : Exception
{
	public nofileselected()
	{
		MessageBox.Show("No File Selected");
	}
}
