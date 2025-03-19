using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using XtafRewrite;

namespace New_Gui;

public class Form1 : Form
{
	public class TreeTag
	{
		public Xtaf.XtafFileSystem.XtafDirectory d;

		public TreeTag(Xtaf.XtafFileSystem.XtafDirectory d)
		{
			this.d = d;
		}
	}

	public class ListTag
	{
		public Xtaf.XtafFileSystem.XtafDirent d;

		public ListTag(Xtaf.XtafFileSystem.XtafDirent d)
		{
			this.d = d;
		}
	}

	private delegate void set_lvi_subitem(int lvi_index, int subindex, string name);

	private delegate string get_lvi_subitem(int lvi_index, int subindex);

	private delegate string get_node_text(int nindex);

	private delegate void set_node_text(int nindex, string name);

	public DebugLog log;

	public Xtaf.XtafFileSystem DataPartition;

	public Xtaf.XtafFileSystem CachePartition;

	public XtafIO.PartitionIO DataIO;

	public XtafIO.PartitionIO CacheIO;

	public Xtaf.XtafFileSystem CompatPartition;

	public bool endianfail;

	public bool CacheDisable;

	public bool OldSizeCalc = true;

	private bool isLinux;

	public XmlTextReader reader;

	private bool deviceopen;

	private bool killnameget;

	private bool dirdone;

	private string[] dircachetitleids = new string[200];

	private string[] dircachenames = new string[200];

	private int cachecount;

	private bool nodesloaded;

	private IContainer components;

	private StatusStrip statusStrip1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem extractToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	public ToolStripStatusLabel toolStripStatusLabel1;

	private ToolStripMenuItem injectToolStripMenuItem;

	private ToolStripMenuItem newFolderToolStripMenuItem;

	private ToolStripMenuItem renameToolStripMenuItem;

	private TreeView treeView1;

	private ToolStripMenuItem diskToolStripMenuItem;

	private ToolStripMenuItem openDriveToolStripMenuItem;

	private MenuStrip menuStrip1;

	private SplitContainer splitContainer1;

	private ToolStripMenuItem closeToolStripMenuItem;

	private ToolStripMenuItem openManuallyToolStripMenuItem;

	private ToolStripMenuItem helpToolStripMenuItem;

	private ToolStripMenuItem supportSiteToolStripMenuItem;

	private ToolStripMenuItem readmeFileToolStripMenuItem;

	private ToolStripMenuItem aboutToolStripMenuItem;

	private ListView listView1;

	private ListViewColumnSorter lvwColumnSorter;

	private ColumnHeader columnFile;

	private ColumnHeader columnSize;

	private ImageList imageList1;

	private BackgroundWorker backgroundWorker1;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem extractToolStripMenuItem1;

	private ToolStripMenuItem deleteToolStripMenuItem1;

	private ToolStripMenuItem injectToolStripMenuItem1;

	private ToolStripMenuItem newFolderToolStripMenuItem1;

	private ToolStripMenuItem renameToolStripMenuItem1;

	private ToolStripMenuItem oldGUIToolStripMenuItem;

	private ToolStripMenuItem emailMeToolStripMenuItem;

	private ColumnHeader columnRealName;

	private BackgroundWorker backgroundWorker2;

	private ToolStripMenuItem injectFolderToolStripMenuItem;

	private ToolStripMenuItem resiToolStripMenuItem;

	private ToolStripMenuItem optionsToolStripMenuItem;

	private TabControl tabControl1;

	private TabPage tabPage1;

	private TabPage tabPage2;

	private WebBrowser webBrowser1;

	private ToolStripMenuItem openHDDImageToolStripMenuItem;

	private ToolStripMenuItem experimentalToolStripMenuItem;

	private ToolStripMenuItem openHDDToolStripMenuItem1;

	public Form1()
	{
		InitializeComponent();
		Version version = Assembly.GetExecutingAssembly().GetName().Version;
		Text = ("Usb Xtaf Gui Version " + version.Revision) ?? "";
		lvwColumnSorter = new ListViewColumnSorter();
		listView1.ListViewItemSorter = lvwColumnSorter;
		backgroundWorker1.RunWorkerAsync();
		backgroundWorker2.RunWorkerAsync();
		listView1.Hide();
		treeView1.Hide();
		log = new DebugLog(Application.StartupPath + "\\MainLog.txt");
		if (Directory.GetLogicalDrives()[0] == "/")
		{
			isLinux = true;
		}
	}

	public string GetGameName(string titleID)
	{
		try
		{
			return "";
		}
		catch
		{
			return "";
		}
	}

	private void openDriveToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (!deviceopen)
		{
			if (start_io_and_initialize_partitions(null) == 0)
			{
				LoadPartitions();
				deviceopen = true;
				listView1.Show();
				treeView1.Show();
			}
			else
			{
				MessageBox.Show("No drive found, try opening manually");
			}
		}
	}

	public static string autodetect_xbox360folder()
	{
		string[] logicalDrives = Directory.GetLogicalDrives();
		if (logicalDrives[0] != "/")
		{
			string[] array = logicalDrives;
			foreach (string text in array)
			{
				if (text != "A:\\" && Directory.Exists(text + "Xbox360"))
				{
					return text + "Xbox360";
				}
			}
		}
		else
		{
			DirectoryInfo[] directories = new DirectoryInfo("/media").GetDirectories();
			foreach (DirectoryInfo directoryInfo in directories)
			{
				if (Directory.Exists("/media/" + directoryInfo.Name + "/Xbox360"))
				{
					return "/media/" + directoryInfo.Name + "/Xbox360";
				}
			}
		}
		return null;
	}

	public string autodetect_cache(string xbox360path)
	{
		if (File.Exists(xbox360path + "/Data0000"))
		{
			return xbox360path + "/Data0000";
		}
		return null;
	}

	public string[] autodetect_data(string xbox360path)
	{
		string[] files = Directory.GetFiles(xbox360path);
		List<string> list = new List<string>();
		ushort num = 0;
		string[] array = files;
		foreach (string path in array)
		{
			if ((Path.GetFileName(path).Substring(0, 4) == "Data") & (Path.GetFileName(path) != "Data0000"))
			{
				num++;
			}
		}
		for (int i = 0; i < num; i++)
		{
			list.Add(xbox360path + "/" + data_filename_create((ushort)(i + 1)));
		}
		return list.ToArray();
	}

	private string data_filename_create(ushort datanum)
	{
		string text = Convert.ToString(datanum);
		while (text.Length < 4)
		{
			text = "0" + text;
		}
		return "Data" + text;
	}

	public int start_io_and_initialize_partitions(string overidepath)
	{
		string text = ((overidepath != null) ? overidepath : autodetect_xbox360folder());
		log.Log("Drive detected is " + text);
		if (text == null)
		{
			return -1;
		}
		string text2 = autodetect_cache(text);
		log.Log("Cahcpath is " + text2);
		string[] array = autodetect_data(text);
		log.Log("Datapath is " + array);
		try
		{
			if (!CacheDisable)
			{
				log.Log("Initializing CacheIO");
				if (!endianfail)
				{
					CacheIO = new XtafIO.PartitionIO(text2, bigendian: true, log.log_on);
				}
				else
				{
					CacheIO = new XtafIO.PartitionIO(text2, bigendian: false, log.log_on);
				}
			}
			log.Log("Initializing DataIO");
			if (!endianfail)
			{
				DataIO = new XtafIO.PartitionIO(array, bigendian: true, log.log_on);
			}
			else
			{
				DataIO = new XtafIO.PartitionIO(array, bigendian: false, log.log_on);
			}
		}
		catch
		{
			if (!CacheDisable && CacheIO != null)
			{
				CacheIO.Close();
			}
			if (DataIO != null)
			{
				DataIO.Close();
			}
			return -1;
		}
		if (!CacheDisable)
		{
			log.Log("Initializing Cache Partiton");
			CachePartition = new Xtaf.XtafFileSystem(CacheIO, 134218752L, isusb: false, 18432u, log.log_on);
		}
		log.Log("Initializing Data Partiton");
		DataPartition = new Xtaf.XtafFileSystem(DataIO, 0L, isusb: true, DataIO.get_usb_numofc(), log.log_on);
		return 0;
	}

	public int Autodetect_Hdd_Windows()
	{
		int num = 0;
		for (num = 0; num < 15; num++)
		{
			try
			{
				WinDiskIO winDiskIO = new WinDiskIO(num);
				BinaryReader binaryReader = new BinaryReader(winDiskIO);
				winDiskIO.Seek(5115674624L, SeekOrigin.Begin);
				if (Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) == "XTAF")
				{
					return num;
				}
			}
			catch
			{
			}
		}
		return -1;
	}

	public string Autodetect_HDD_Linux()
	{
		for (int i = 0; i < 16; i++)
		{
			string text = "/dev/sd" + (char)(i + 97);
			if (!File.Exists("/dev/sd" + (char)(i + 97)))
			{
				continue;
			}
			try
			{
				FileStream fileStream = new FileStream(text, FileMode.Open);
				fileStream.Position = 5115674624L;
				BinaryReader binaryReader = new BinaryReader(fileStream);
				if (Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) == "XTAF")
				{
					binaryReader.Close();
					fileStream.Close();
					fileStream = null;
					return text;
				}
			}
			catch
			{
			}
		}
		return "";
	}

	public void HDD_image_start(string fpath)
	{
		new WinDiskIO(0);
		DataIO = new XtafIO.PartitionIO(fpath, bigendian: true, debug: false);
		DataIO.SeekTo(0L);
		long num = (DataIO.nio.Stream.Length - 5115674624L) / 16384 * 4;
		long num2 = 4096 - num % 4096;
		num += num2;
		if (!OldSizeCalc)
		{
			num += 4096;
		}
		DataPartition = new Xtaf.XtafFileSystem(DataIO, 5115674624L, isusb: false, (uint)num / 4, debug: false);
		_ = CacheDisable;
	}

	public void HDD_start(int disknumber)
	{
		WinDiskIO winDiskIO = new WinDiskIO(disknumber);
		DataIO = new XtafIO.PartitionIO(winDiskIO);
		CacheIO = new XtafIO.PartitionIO(winDiskIO);
		DataIO.SeekTo(0L);
		CacheIO.SeekTo(0L);
		DataIO.LengthOverride = winDiskIO.Length;
		CacheIO.LengthOverride = winDiskIO.Length;
		long num = (winDiskIO.Length - 5115674624L) / 16384 * 4;
		long num2 = 4096 - num % 4096;
		num += num2;
		if (!OldSizeCalc)
		{
			num += 4096;
		}
		DataPartition = new Xtaf.XtafFileSystem(DataIO, 5115674624L, isusb: false, (uint)(num / 4), debug: false);
		if (!CacheDisable)
		{
			CompatPartition = new Xtaf.XtafFileSystem(CacheIO, 4847239168L, isusb: false, 18432u, debug: false);
		}
	}

	public void HDD_start_linux(string device)
	{
		XtafIO.PartitionIO partitionIO = new XtafIO.PartitionIO(device, bigendian: true, debug: false);
		partitionIO.SeekTo(0L);
		FileStream fileStream = new FileStream("/sys/block/" + device.Substring(5) + "/size", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		fileStream.Position = 0L;
		long num = Convert.ToInt64(Encoding.ASCII.GetString(binaryReader.ReadBytes((int)fileStream.Length)));
		long num2 = ((partitionIO.LengthOverride = num * 512) - 5115674624L) / 16384 * 4;
		long num3 = 4096 - num2 % 4096;
		num2 += num3;
		if (!OldSizeCalc)
		{
			num2 += 4096;
		}
		DataPartition = new Xtaf.XtafFileSystem(partitionIO, 5115674624L, isusb: false, (uint)(num2 / 4), debug: false);
		_ = CacheDisable;
	}

	public void LoadPartitions()
	{
		TreeNode treeNode = new TreeNode();
		treeNode.Text = "Data Partition";
		treeNode.Name = "Data";
		treeNode.Tag = new TreeTag(DataPartition.rootdir);
		treeNode = LoadChildDirectories(treeNode);
		if (!CacheDisable & (CachePartition != null))
		{
			TreeNode treeNode2 = new TreeNode();
			treeNode2.Text = "Cache Partition";
			treeNode2.Name = "Cache";
			treeNode2.Tag = new TreeTag(CachePartition.rootdir);
			treeNode2 = LoadChildDirectories(treeNode2);
			treeView1.Nodes.Add(treeNode2);
		}
		if (CompatPartition != null)
		{
			TreeNode treeNode3 = new TreeNode();
			treeNode3.Text = "Compatibility Partition";
			treeNode3.Name = "Compat";
			treeNode3.Tag = new TreeTag(CompatPartition.rootdir);
			treeNode3 = LoadChildDirectories(treeNode3);
			treeView1.Nodes.Add(treeNode3);
		}
		treeView1.Nodes.Add(treeNode);
	}

	private TreeNode LoadChildDirectories(TreeNode xNode)
	{
		xNode.Nodes.Clear();
		foreach (Xtaf.XtafFileSystem.XtafDirectory subDirectory in ((TreeTag)xNode.Tag).d.SubDirectories)
		{
			if (subDirectory.dirent.isValid)
			{
				if (!subDirectory.isInvalid)
				{
					TreeNode treeNode = new TreeNode();
					treeNode.Text = subDirectory.name;
					treeNode.Name = subDirectory.name;
					treeNode.Tag = new TreeTag(subDirectory);
					treeNode = LoadChildDirectories(treeNode);
					xNode.Nodes.Add(treeNode);
				}
				else
				{
					TreeNode treeNode2 = new TreeNode();
					treeNode2.Text = "Invalid Directory";
					treeNode2.Name = "Invalid Directory";
					treeNode2.Tag = null;
					xNode.Nodes.Add(treeNode2);
				}
			}
			else
			{
				TreeNode treeNode3 = new TreeNode();
				treeNode3.Text = "invalid directory";
				treeNode3.Name = subDirectory.name;
				treeNode3.Tag = new TreeTag(subDirectory);
				xNode.Nodes.Add(treeNode3);
			}
		}
		return xNode;
	}

	public void loadfiles(TreeNode xNode)
	{
		if (((TreeTag)xNode.Tag).d.isInvalid)
		{
			listView1.Items.Clear();
			return;
		}
		killnameget = true;
		dirdone = false;
		listView1.Items.Clear();
		Xtaf.XtafFileSystem.XtafDirent[] dirents = ((TreeTag)xNode.Tag).d.dirents;
		foreach (Xtaf.XtafFileSystem.XtafDirent xtafDirent in dirents)
		{
			if (xtafDirent.open)
			{
				continue;
			}
			if (xtafDirent.isValid)
			{
				ListViewItem listViewItem = new ListViewItem(new string[3]
				{
					xtafDirent.name,
					get_freindly_size(xtafDirent.fsize),
					""
				});
				listViewItem.Tag = new ListTag(xtafDirent);
				if (xtafDirent.isDir)
				{
					listViewItem.ImageIndex = 0;
				}
				else
				{
					listViewItem.ImageIndex = filetype(xtafDirent) + 1;
				}
				listView1.Items.Add(listViewItem);
				continue;
			}
			string[] array = new string[3] { null, "Invalid", "" };
			if (!xtafDirent.isDir)
			{
				array[0] = "Invalid File";
			}
			else
			{
				array[0] = "Invalid Directory";
			}
			ListViewItem listViewItem2 = new ListViewItem(array);
			listViewItem2.Tag = null;
			listView1.Items.Add(listViewItem2);
		}
		((TreeTag)xNode.Tag).d.parent.curDir = ((TreeTag)xNode.Tag).d;
	}

	public int filetype(Xtaf.XtafFileSystem.XtafDirent d)
	{
		d.parent.seek_to_offset(d.fstart);
		string @string = Encoding.ASCII.GetString(d.parent.pio.nr.ReadBytes(3));
		if (@string == "CON")
		{
			return 1;
		}
		if ((@string == "LIV") | (@string == "PIR"))
		{
			return 2;
		}
		return 0;
	}

	private void after_select(object sender, TreeViewEventArgs e)
	{
		if (treeView1.SelectedNode != null)
		{
			killnameget = true;
			loadfiles(treeView1.SelectedNode);
			dirdone = false;
		}
	}

	private void extractToolStripMenuItem_Click(object sender, EventArgs e)
	{
		foreach (ListViewItem selectedItem in listView1.SelectedItems)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = ((ListTag)selectedItem.Tag).d.name;
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				((ListTag)selectedItem.Tag).d.extract(saveFileDialog.FileName);
			}
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		foreach (ListViewItem selectedItem in listView1.SelectedItems)
		{
			if (!((ListTag)selectedItem.Tag).d.isDir)
			{
				((ListTag)selectedItem.Tag).d.delete();
			}
			else
			{
				delete_directory(((ListTag)selectedItem.Tag).d);
			}
		}
		reload_cnode();
		reload();
	}

	public void delete_directory(Xtaf.XtafFileSystem.XtafDirent d)
	{
		d.AssignedDir.enter();
		Xtaf.XtafFileSystem.XtafDirent[] dirents = d.parent.curDir.dirents;
		foreach (Xtaf.XtafFileSystem.XtafDirent xtafDirent in dirents)
		{
			if (!xtafDirent.open)
			{
				if (xtafDirent.isDir)
				{
					delete_directory(xtafDirent);
				}
				else
				{
					xtafDirent.delete();
				}
			}
		}
		d.AssignedDir.leave();
		d.delete();
	}

	public void reload()
	{
		loadfiles(treeView1.SelectedNode);
	}

	private void injectToolStripMenuItem_Click(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			try
			{
				((TreeTag)treeView1.SelectedNode.Tag).d.injectfile(openFileDialog.FileName);
			}
			catch (File_Exists)
			{
				MessageBox.Show("File already exists");
			}
		}
		((TreeTag)treeView1.SelectedNode.Tag).d.Reload();
		reload();
		reload_cnode();
	}

	private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
	{
		new Form2(this, ((TreeTag)treeView1.SelectedNode.Tag).d).Show();
	}

	private void renameToolStripMenuItem_Click(object sender, EventArgs e)
	{
		foreach (ListViewItem selectedItem in listView1.SelectedItems)
		{
			ListTag listTag = (ListTag)selectedItem.Tag;
			new Form3(this, listTag.d, leftside: false).Show();
		}
	}

	public void reload_cnode()
	{
		((TreeTag)treeView1.SelectedNode.Tag).d.Reload();
		treeView1.SelectedNode = LoadChildDirectories(treeView1.SelectedNode);
	}

	public void reload_pnode()
	{
		if (treeView1.SelectedNode.Name != "root")
		{
			treeView1.SelectedNode = treeView1.SelectedNode.Parent;
		}
		reload_cnode();
	}

	private void closeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (DataIO != null)
		{
			DataIO.Close();
			DataIO = null;
		}
		if (CacheIO != null)
		{
			CacheIO.Close();
			CacheIO = null;
		}
		DataPartition = null;
		CachePartition = null;
		CompatPartition = null;
		listView1.Items.Clear();
		treeView1.Nodes.Clear();
		listView1.Hide();
		treeView1.Hide();
		deviceopen = false;
	}

	private void openManuallyToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (deviceopen)
		{
			return;
		}
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
		{
			if (start_io_and_initialize_partitions(folderBrowserDialog.SelectedPath) == 0)
			{
				LoadPartitions();
				deviceopen = true;
				listView1.Show();
				treeView1.Show();
			}
			else if (start_io_and_initialize_partitions(folderBrowserDialog.SelectedPath + "\\Xbox360") == 0)
			{
				LoadPartitions();
				deviceopen = true;
				listView1.Show();
				treeView1.Show();
			}
			else
			{
				MessageBox.Show("Could not open, make sure you selected the correct Xbox360 folder");
			}
		}
	}

	private void lb_dragenter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			e.Effect = DragDropEffects.Move;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void lb_dragdrop(object sender, DragEventArgs e)
	{
		string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
		for (int i = 0; i <= array.Length - 1; i++)
		{
			toolStripStatusLabel1.Text = "Injecting...";
			Update();
			try
			{
				((TreeTag)treeView1.SelectedNode.Tag).d.injectfile(array[i]);
			}
			catch (File_Exists)
			{
				MessageBox.Show("File Exists Already");
			}
			toolStripStatusLabel1.Text = "Done...";
		}
		reload_cnode();
		reload();
	}

	private void supportSiteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Process.Start("http://gruntmods.com/?page_id=17");
	}

	private void readmeFileToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Process.Start("http://gruntmods.com/Projects/Downloads/USB%20XTAF/Readme.txt");
	}

	private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
	{
		new AboutBox1().Show();
	}

	private string get_freindly_size(long length)
	{
		if (length == 0L)
		{
			return "";
		}
		string text = "Bytes";
		if (length >= 1024)
		{
			text = "Kb";
			length /= 1024;
		}
		if (length >= 1024)
		{
			text = "Mb";
			length /= 1024;
		}
		if (length >= 1024)
		{
			text = "Gb";
			length /= 1024;
		}
		return Convert.ToString(length) + text;
	}

	private void Column_Clicked(object sender, ColumnClickEventArgs e)
	{
		if (e.Column == lvwColumnSorter.SortColumn)
		{
			if (lvwColumnSorter.Order == SortOrder.Ascending)
			{
				lvwColumnSorter.Order = SortOrder.Descending;
			}
			else
			{
				lvwColumnSorter.Order = SortOrder.Ascending;
			}
		}
		else
		{
			lvwColumnSorter.SortColumn = e.Column;
			lvwColumnSorter.Order = SortOrder.Ascending;
		}
		listView1.Sort();
	}

	private void ListView1_ItemDrag(object sender, ItemDragEventArgs e)
	{
		ListTag listTag = (ListTag)(e.Item as ListViewItem).Tag;
		listTag.d.extract(Path.GetTempPath() + listTag.d.name);
		string[] array = new string[1] { Path.GetTempPath() + listTag.d.name };
		if (array != null)
		{
			DoDragDrop(new DataObject(DataFormats.FileDrop, array), DragDropEffects.Copy | DragDropEffects.Move);
		}
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
			if (DialogResult.Yes == MessageBox.Show(text2, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
			{
				new WebClient().DownloadFile(address, "USBXTAFGUI_v" + Convert.ToString(version.Revision) + ".exe");
				Process.Start(Application.StartupPath + "\\USBXTAFGUI_v" + Convert.ToString(version.Revision) + ".exe");
				Environment.Exit(0);
			}
		}
	}

	private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
	{
		updater_run();
	}

	private void extractToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode == null)
		{
			return;
		}
		TreeTag treeTag = (TreeTag)treeView1.SelectedNode.Tag;
		if (treeTag.d.dirent == null)
		{
			return;
		}
		Xtaf.XtafFileSystem.XtafDirectory curDir = treeTag.d.parent.curDir;
		if (treeTag.d.dirent != null)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = treeTag.d.dirent.name;
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				treeTag.d.dirent.extract(saveFileDialog.FileName);
			}
		}
		treeTag.d.parent.curDir = curDir;
	}

	private void tv1_mouseup(object sender, MouseEventArgs e)
	{
		treeView1.SelectedNode = treeView1.GetNodeAt(e.Location);
	}

	private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode != null)
		{
			TreeTag treeTag = (TreeTag)treeView1.SelectedNode.Tag;
			if (treeTag.d.dirent != null)
			{
				treeView1.SelectedNode = treeView1.SelectedNode.Parent;
				((TreeTag)treeView1.SelectedNode.Tag).d.find_by_name(treeTag.d.name).delete();
				reload_cnode();
				reload();
			}
		}
	}

	private void injectToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode != null)
		{
			injectToolStripMenuItem_Click(sender, e);
		}
	}

	private void newFolderToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode != null)
		{
			newFolderToolStripMenuItem_Click(sender, e);
		}
	}

	private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode != null)
		{
			TreeTag treeTag = (TreeTag)treeView1.SelectedNode.Tag;
			if (treeTag.d.dirent != null && treeTag.d.dirent != null)
			{
				new Form3(this, treeTag.d.dirent, leftside: true).Show();
			}
		}
	}

	private void oldGUIToolStripMenuItem_Click(object sender, EventArgs e)
	{
		MessageBox.Show("If you can, please post a bug report of why the new version doesent work on the support site, or email me. If you are getting the array out of range/file access issue then I will need a compressed archive of the files in UsbDrive://Xbox360(without data0000) to fix the issue");
		Process.Start("http://gruntmods.com/Projects/Downloads/USB%20XTAF/endline.exe");
	}

	private void emailMeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		MessageBox.Show("You can contact the original author at the following email: slasherking823@gmail.com");
	}

	public void populate_realname()
	{
		for (int i = 0; i < listView1.Items.Count; i++)
		{
			if (killnameget)
			{
				killnameget = false;
				dirdone = false;
				return;
			}
			if (!base.InvokeRequired)
			{
				listView1.Items[i].SubItems[2].Text = GetGameName(listView1.Items[i].SubItems[0].Text);
			}
			else
			{
				get_lvi_subitem method = get_lvi_subitem_function;
				set_lvi_subitem method2 = set_lvi_subitem_function;
				object[] args = new object[2] { i, 0 };
				if (killnameget)
				{
					killnameget = false;
					dirdone = false;
					return;
				}
				string text = (string)Invoke(method, args);
				object[] array = new object[3] { i, 2, null };
				string[] array2 = new string[7] { "00000001", "00020000", "00030000", "00080000", "00090000", "00004000", "00000002" };
				string[] array3 = new string[7] { "Data", "Gamerpics", "Themes", "Demos", "Videos", "Game Installs", "Extra Content" };
				if (!array2.Contains(text))
				{
					if (dircachetitleids.Contains(text))
					{
						array[2] = dircachenames[Array.IndexOf(dircachetitleids, text)];
					}
					else
					{
						array[2] = GetGameName(text);
						if (cachecount < 200)
						{
							dircachetitleids[cachecount] = text;
							dircachenames[cachecount] = (string)array[2];
							cachecount++;
						}
					}
				}
				else
				{
					array[2] = array3[Array.IndexOf(array2, text)];
				}
				if (killnameget)
				{
					killnameget = false;
					dirdone = false;
					return;
				}
				try
				{
					Invoke(method2, array);
				}
				catch
				{
				}
			}
			if (killnameget)
			{
				killnameget = false;
				dirdone = false;
				return;
			}
		}
		dirdone = true;
	}

	private void set_lvi_subitem_function(int lvi_index, int subindex, string name)
	{
		listView1.Items[lvi_index].SubItems[subindex].Text = name;
	}

	private string get_lvi_subitem_function(int lvi_index, int subindex)
	{
		return listView1.Items[lvi_index].SubItems[subindex].Text;
	}

	private string get_node_text_f(int nindex)
	{
		return treeView1.Nodes[nindex].Text;
	}

	private void set_node_text_f(int nindex, string name)
	{
		treeView1.Nodes[nindex].Text = name;
	}

	private void listView1_SelectedIndexChanged(object sender, EventArgs e)
	{
	}

	private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
	{
		while (true)
		{
			if (!nodesloaded)
			{
				load_node_names();
			}
			if (!dirdone)
			{
				populate_realname();
			}
			else
			{
				Thread.Sleep(500);
			}
		}
	}

	private void load_node_names()
	{
		for (int i = 0; i < treeView1.Nodes.Count; i++)
		{
			get_node_text method = get_node_text_f;
			set_node_text method2 = set_node_text_f;
			string text = (string)Invoke(method, i);
			object[] array = new object[2] { i, null };
			if (dircachetitleids.Contains(text))
			{
				array[1] = dircachenames[Array.IndexOf(dircachetitleids, text)];
			}
			else
			{
				array[1] = GetGameName(text);
				if (cachecount < 200)
				{
					dircachetitleids[cachecount] = text;
					dircachenames[cachecount] = (string)array[1];
					cachecount++;
				}
			}
			if ((string)array[1] != "")
			{
				Invoke(method2, array);
			}
		}
		if (treeView1.Nodes.Count != 0)
		{
			nodesloaded = true;
		}
	}

	private void enterDirectory(Xtaf.XtafFileSystem.XtafDirent d)
	{
		treeView1.SelectedNode = treeView1.SelectedNode.Nodes[d.name];
	}

	private void injectFolderToolStripMenuItem_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
		{
			((TreeTag)treeView1.SelectedNode.Tag).d.injectfile(folderBrowserDialog.SelectedPath);
		}
		reload_cnode();
		reload();
	}

	private void resiToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		new DebugOpt().Show();
	}

	private void tab_clicked(object sender, EventArgs e)
	{
		try
		{
			webBrowser1.Navigate("http://gruntmods.com/Projects/Downloads/USB%20XTAF/News.php");
		}
		catch
		{
			MessageBox.Show("Unable to load webbrowser, if on linux install libgluezilla");
		}
	}

	private void openHDDImageToolStripMenuItem_Click(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			HDD_image_start(openFileDialog.FileName);
			LoadPartitions();
			deviceopen = true;
			listView1.Show();
			treeView1.Show();
			deviceopen = true;
		}
	}

	private void openHDDToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (!isLinux)
		{
			int num = Autodetect_Hdd_Windows();
			if (num != -1)
			{
				HDD_start(num);
				LoadPartitions();
				deviceopen = true;
				listView1.Show();
				treeView1.Show();
				deviceopen = true;
			}
			else
			{
				MessageBox.Show("Could not find a Xbox360 HDD, try running as administrator, and make sure the drive is detected");
			}
		}
		else
		{
			string text = Autodetect_HDD_Linux();
			if (text != "")
			{
				HDD_start_linux(text);
				LoadPartitions();
				deviceopen = true;
				listView1.Show();
				treeView1.Show();
				deviceopen = true;
			}
			else
			{
				MessageBox.Show("Could not find a Xbox360 HDD, try running as root");
			}
		}
	}

	private void toolStripStatusLabel1_Click(object sender, EventArgs e)
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(New_Gui.Form1));
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.injectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.injectFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.resiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.statusStrip1 = new System.Windows.Forms.StatusStrip();
		this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
		this.treeView1 = new System.Windows.Forms.TreeView();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.extractToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.injectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.newFolderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.imageList1 = new System.Windows.Forms.ImageList(this.components);
		this.diskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.openDriveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.openHDDImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.openManuallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.experimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.openHDDToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.menuStrip1 = new System.Windows.Forms.MenuStrip();
		this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.supportSiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.readmeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.oldGUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.emailMeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.listView1 = new System.Windows.Forms.ListView();
		this.columnFile = new System.Windows.Forms.ColumnHeader();
		this.columnSize = new System.Windows.Forms.ColumnHeader();
		this.columnRealName = new System.Windows.Forms.ColumnHeader();
		this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
		this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.tabPage2 = new System.Windows.Forms.TabPage();
		this.webBrowser1 = new System.Windows.Forms.WebBrowser();
		this.contextMenuStrip1.SuspendLayout();
		this.statusStrip1.SuspendLayout();
		this.contextMenuStrip2.SuspendLayout();
		this.menuStrip1.SuspendLayout();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		this.tabControl1.SuspendLayout();
		this.tabPage1.SuspendLayout();
		this.tabPage2.SuspendLayout();
		base.SuspendLayout();
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.extractToolStripMenuItem, this.deleteToolStripMenuItem, this.injectToolStripMenuItem, this.injectFolderToolStripMenuItem, this.newFolderToolStripMenuItem, this.renameToolStripMenuItem, this.resiToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(149, 158);
		this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
		this.extractToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.extractToolStripMenuItem.Text = "Extract...";
		this.extractToolStripMenuItem.Click += new System.EventHandler(extractToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.deleteToolStripMenuItem.Text = "Delete...";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(deleteToolStripMenuItem_Click);
		this.injectToolStripMenuItem.Name = "injectToolStripMenuItem";
		this.injectToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.injectToolStripMenuItem.Text = "Inject...";
		this.injectToolStripMenuItem.Click += new System.EventHandler(injectToolStripMenuItem_Click);
		this.injectFolderToolStripMenuItem.Name = "injectFolderToolStripMenuItem";
		this.injectFolderToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.injectFolderToolStripMenuItem.Text = "Inject Folder...";
		this.injectFolderToolStripMenuItem.Click += new System.EventHandler(injectFolderToolStripMenuItem_Click);
		this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
		this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.newFolderToolStripMenuItem.Text = "New Folder...";
		this.newFolderToolStripMenuItem.Click += new System.EventHandler(newFolderToolStripMenuItem_Click);
		this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
		this.renameToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.renameToolStripMenuItem.Text = "Rename...";
		this.renameToolStripMenuItem.Click += new System.EventHandler(renameToolStripMenuItem_Click);
		this.resiToolStripMenuItem.Name = "resiToolStripMenuItem";
		this.resiToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.resiToolStripMenuItem.Text = "Resign CON...";
		this.resiToolStripMenuItem.Visible = false;
		this.resiToolStripMenuItem.Click += new System.EventHandler(resiToolStripMenuItem_Click);
		this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripStatusLabel1 });
		this.statusStrip1.Location = new System.Drawing.Point(0, 479);
		this.statusStrip1.Name = "statusStrip1";
		this.statusStrip1.Size = new System.Drawing.Size(1008, 22);
		this.statusStrip1.TabIndex = 2;
		this.statusStrip1.Text = "statusStrip1";
		this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
		this.toolStripStatusLabel1.Size = new System.Drawing.Size(125, 17);
		this.toolStripStatusLabel1.Text = "Slashr - Open a Device";
		this.toolStripStatusLabel1.Click += new System.EventHandler(toolStripStatusLabel1_Click);
		this.treeView1.ContextMenuStrip = this.contextMenuStrip2;
		this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.treeView1.ImageIndex = 0;
		this.treeView1.ImageList = this.imageList1;
		this.treeView1.Location = new System.Drawing.Point(0, 0);
		this.treeView1.Name = "treeView1";
		this.treeView1.SelectedImageIndex = 0;
		this.treeView1.Size = new System.Drawing.Size(310, 423);
		this.treeView1.TabIndex = 0;
		this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(after_select);
		this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(tv1_mouseup);
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.extractToolStripMenuItem1, this.deleteToolStripMenuItem1, this.injectToolStripMenuItem1, this.newFolderToolStripMenuItem1, this.renameToolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(144, 114);
		this.extractToolStripMenuItem1.Name = "extractToolStripMenuItem1";
		this.extractToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.extractToolStripMenuItem1.Text = "Extract...";
		this.extractToolStripMenuItem1.Click += new System.EventHandler(extractToolStripMenuItem1_Click);
		this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
		this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.deleteToolStripMenuItem1.Text = "Delete...";
		this.deleteToolStripMenuItem1.Click += new System.EventHandler(deleteToolStripMenuItem1_Click);
		this.injectToolStripMenuItem1.Name = "injectToolStripMenuItem1";
		this.injectToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.injectToolStripMenuItem1.Text = "Inject...";
		this.injectToolStripMenuItem1.Click += new System.EventHandler(injectToolStripMenuItem1_Click);
		this.newFolderToolStripMenuItem1.Name = "newFolderToolStripMenuItem1";
		this.newFolderToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.newFolderToolStripMenuItem1.Text = "New Folder...";
		this.newFolderToolStripMenuItem1.Click += new System.EventHandler(newFolderToolStripMenuItem1_Click);
		this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
		this.renameToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.renameToolStripMenuItem1.Text = "Rename...";
		this.renameToolStripMenuItem1.Click += new System.EventHandler(renameToolStripMenuItem1_Click);
		this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream");
		this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
		this.imageList1.Images.SetKeyName(0, "folder.ico");
		this.imageList1.Images.SetKeyName(1, "file.ico");
		this.imageList1.Images.SetKeyName(2, "New_Con.ico");
		this.imageList1.Images.SetKeyName(3, "New_Live.ico");
		this.diskToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.openDriveToolStripMenuItem, this.openHDDImageToolStripMenuItem, this.openManuallyToolStripMenuItem, this.closeToolStripMenuItem, this.optionsToolStripMenuItem, this.experimentalToolStripMenuItem });
		this.diskToolStripMenuItem.Name = "diskToolStripMenuItem";
		this.diskToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
		this.diskToolStripMenuItem.Text = "File";
		this.openDriveToolStripMenuItem.Name = "openDriveToolStripMenuItem";
		this.openDriveToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
		this.openDriveToolStripMenuItem.Text = "Open USB Drive";
		this.openDriveToolStripMenuItem.Click += new System.EventHandler(openDriveToolStripMenuItem_Click);
		this.openHDDImageToolStripMenuItem.Name = "openHDDImageToolStripMenuItem";
		this.openHDDImageToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
		this.openHDDImageToolStripMenuItem.Text = "Open HDD Image";
		this.openHDDImageToolStripMenuItem.Click += new System.EventHandler(openHDDImageToolStripMenuItem_Click);
		this.openManuallyToolStripMenuItem.Name = "openManuallyToolStripMenuItem";
		this.openManuallyToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
		this.openManuallyToolStripMenuItem.Text = "Open USB Manually...";
		this.openManuallyToolStripMenuItem.Click += new System.EventHandler(openManuallyToolStripMenuItem_Click);
		this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
		this.closeToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
		this.closeToolStripMenuItem.Text = "Close  Current Drive";
		this.closeToolStripMenuItem.Click += new System.EventHandler(closeToolStripMenuItem_Click);
		this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
		this.optionsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
		this.optionsToolStripMenuItem.Text = "Options";
		this.optionsToolStripMenuItem.Click += new System.EventHandler(optionsToolStripMenuItem_Click);
		this.experimentalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.openHDDToolStripMenuItem1 });
		this.experimentalToolStripMenuItem.Name = "experimentalToolStripMenuItem";
		this.experimentalToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
		this.experimentalToolStripMenuItem.Text = "Experimental";
		this.openHDDToolStripMenuItem1.Name = "openHDDToolStripMenuItem1";
		this.openHDDToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
		this.openHDDToolStripMenuItem1.Text = "Open HDD";
		this.openHDDToolStripMenuItem1.Click += new System.EventHandler(openHDDToolStripMenuItem1_Click);
		this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
		this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.diskToolStripMenuItem, this.helpToolStripMenuItem });
		this.menuStrip1.Location = new System.Drawing.Point(0, 0);
		this.menuStrip1.Name = "menuStrip1";
		this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
		this.menuStrip1.TabIndex = 0;
		this.menuStrip1.Text = "menuStrip1";
		this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.supportSiteToolStripMenuItem, this.readmeFileToolStripMenuItem, this.aboutToolStripMenuItem, this.oldGUIToolStripMenuItem, this.emailMeToolStripMenuItem });
		this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
		this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
		this.helpToolStripMenuItem.Text = "Help";
		this.supportSiteToolStripMenuItem.Name = "supportSiteToolStripMenuItem";
		this.supportSiteToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.supportSiteToolStripMenuItem.Text = "Support Site";
		this.supportSiteToolStripMenuItem.Click += new System.EventHandler(supportSiteToolStripMenuItem_Click);
		this.readmeFileToolStripMenuItem.Name = "readmeFileToolStripMenuItem";
		this.readmeFileToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.readmeFileToolStripMenuItem.Text = "Readme File";
		this.readmeFileToolStripMenuItem.Click += new System.EventHandler(readmeFileToolStripMenuItem_Click);
		this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
		this.aboutToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.aboutToolStripMenuItem.Text = "About";
		this.aboutToolStripMenuItem.Click += new System.EventHandler(aboutToolStripMenuItem_Click);
		this.oldGUIToolStripMenuItem.Name = "oldGUIToolStripMenuItem";
		this.oldGUIToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.oldGUIToolStripMenuItem.Text = "Old GUI";
		this.oldGUIToolStripMenuItem.Click += new System.EventHandler(oldGUIToolStripMenuItem_Click);
		this.emailMeToolStripMenuItem.Name = "emailMeToolStripMenuItem";
		this.emailMeToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.emailMeToolStripMenuItem.Text = "Email Me...";
		this.emailMeToolStripMenuItem.Click += new System.EventHandler(emailMeToolStripMenuItem_Click);
		this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer1.Location = new System.Drawing.Point(3, 3);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Panel1.Controls.Add(this.treeView1);
		this.splitContainer1.Panel2.Controls.Add(this.listView1);
		this.splitContainer1.Size = new System.Drawing.Size(994, 423);
		this.splitContainer1.SplitterDistance = 310;
		this.splitContainer1.TabIndex = 1;
		this.listView1.AllowDrop = true;
		this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.columnFile, this.columnSize, this.columnRealName });
		this.listView1.ContextMenuStrip = this.contextMenuStrip1;
		this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.listView1.Location = new System.Drawing.Point(0, 0);
		this.listView1.Name = "listView1";
		this.listView1.Size = new System.Drawing.Size(680, 423);
		this.listView1.SmallImageList = this.imageList1;
		this.listView1.TabIndex = 0;
		this.listView1.UseCompatibleStateImageBehavior = false;
		this.listView1.View = System.Windows.Forms.View.Details;
		this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(Column_Clicked);
		this.listView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(ListView1_ItemDrag);
		this.listView1.SelectedIndexChanged += new System.EventHandler(listView1_SelectedIndexChanged);
		this.listView1.DragDrop += new System.Windows.Forms.DragEventHandler(lb_dragdrop);
		this.listView1.DragEnter += new System.Windows.Forms.DragEventHandler(lb_dragenter);
		this.columnFile.Text = "File";
		this.columnFile.Width = 125;
		this.columnSize.DisplayIndex = 2;
		this.columnSize.Text = "Size";
		this.columnSize.Width = 82;
		this.columnRealName.DisplayIndex = 1;
		this.columnRealName.Text = "Real Name";
		this.columnRealName.Width = 120;
		this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker1_DoWork);
		this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker2_DoWork);
		this.tabControl1.Controls.Add(this.tabPage1);
		this.tabControl1.Controls.Add(this.tabPage2);
		this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabControl1.Location = new System.Drawing.Point(0, 24);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabControl1.Size = new System.Drawing.Size(1008, 455);
		this.tabControl1.TabIndex = 3;
		this.tabControl1.SelectedIndexChanged += new System.EventHandler(tab_clicked);
		this.tabPage1.Controls.Add(this.splitContainer1);
		this.tabPage1.Location = new System.Drawing.Point(4, 22);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(1000, 429);
		this.tabPage1.TabIndex = 0;
		this.tabPage1.Text = "FileSystem";
		this.tabPage1.UseVisualStyleBackColor = true;
		this.tabPage2.Controls.Add(this.webBrowser1);
		this.tabPage2.Location = new System.Drawing.Point(4, 22);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(1000, 429);
		this.tabPage2.TabIndex = 1;
		this.tabPage2.Text = "Xtaf Today";
		this.tabPage2.UseVisualStyleBackColor = true;
		this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.webBrowser1.Location = new System.Drawing.Point(3, 3);
		this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
		this.webBrowser1.Name = "webBrowser1";
		this.webBrowser1.Size = new System.Drawing.Size(994, 423);
		this.webBrowser1.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1008, 501);
		base.Controls.Add(this.tabControl1);
		base.Controls.Add(this.statusStrip1);
		base.Controls.Add(this.menuStrip1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MainMenuStrip = this.menuStrip1;
		this.MinimumSize = new System.Drawing.Size(859, 540);
		base.Name = "Form1";
		this.Text = "USB XTAF Explorer";
		this.contextMenuStrip1.ResumeLayout(false);
		this.statusStrip1.ResumeLayout(false);
		this.statusStrip1.PerformLayout();
		this.contextMenuStrip2.ResumeLayout(false);
		this.menuStrip1.ResumeLayout(false);
		this.menuStrip1.PerformLayout();
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		this.splitContainer1.ResumeLayout(false);
		this.tabControl1.ResumeLayout(false);
		this.tabPage1.ResumeLayout(false);
		this.tabPage2.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
