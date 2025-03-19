using System;
using System.IO;
using System.Windows.Forms;
using EasyIO;

namespace XtafRewrite;

public class XtafIO
{
	public class PartitionIO
	{
		public long global_offset;

		public global::EasyIO.EasyIO.EndianIO nio;

		public global::EasyIO.EasyIO.EndianReader nr;

		public global::EasyIO.EasyIO.EndianWriter nw;

		private global::EasyIO.EasyIO.EndianIO[] iolist;

		private global::EasyIO.EasyIO.EndianReader[] rlist;

		private global::EasyIO.EasyIO.EndianWriter[] wlist;

		private DebugLog log;

		public long LengthOverride;

		public long Length;

		public bool usb_rootoffset
		{
			get
			{
				iolist[1].Stream.Position = 0L;
				if (rlist[1].ReadInt64() == 0L)
				{
					return true;
				}
				return false;
			}
		}

		private void calculate_length()
		{
			long num = 0L;
			global::EasyIO.EasyIO.EndianIO[] array = iolist;
			foreach (global::EasyIO.EasyIO.EndianIO endianIO in array)
			{
				num += endianIO.Stream.Length;
			}
			Length = num;
		}

		public PartitionIO(string fpath, bool bigendian, bool debug)
		{
			if (File.Exists(Application.StartupPath + "\\IO_log.txt"))
			{
				log = new DebugLog(Application.StartupPath + "\\IO_log2.txt");
			}
			else
			{
				log = new DebugLog(Application.StartupPath + "\\IO_log.txt");
			}
			log.log_on = debug;
			log.Log("Opening partition IO on " + fpath);
			iolist = new global::EasyIO.EasyIO.EndianIO[1];
			rlist = new global::EasyIO.EasyIO.EndianReader[1];
			wlist = new global::EasyIO.EasyIO.EndianWriter[1];
			if (bigendian)
			{
				iolist[0] = new global::EasyIO.EasyIO.EndianIO(fpath, global::EasyIO.EasyIO.EndianType.BigEndian);
				log.Log("Opening " + fpath);
				iolist[0].Open();
				log.Log("Opened");
				rlist[0] = new global::EasyIO.EasyIO.EndianReader(iolist[0].Stream, global::EasyIO.EasyIO.EndianType.BigEndian);
				wlist[0] = new global::EasyIO.EasyIO.EndianWriter(iolist[0].Stream, global::EasyIO.EasyIO.EndianType.BigEndian);
			}
			else
			{
				iolist[0] = new global::EasyIO.EasyIO.EndianIO(fpath, global::EasyIO.EasyIO.EndianType.LittleEndian);
				log.Log("Opening " + fpath);
				iolist[0].Open();
				log.Log("Opened");
				rlist[0] = new global::EasyIO.EasyIO.EndianReader(iolist[0].Stream, global::EasyIO.EasyIO.EndianType.LittleEndian);
				wlist[0] = new global::EasyIO.EasyIO.EndianWriter(iolist[0].Stream, global::EasyIO.EasyIO.EndianType.LittleEndian);
			}
			nio = iolist[0];
			nr = rlist[0];
			nw = wlist[0];
			calculate_length();
		}

		public PartitionIO(string[] files, bool bigendian, bool debug)
		{
			if (File.Exists(Application.StartupPath + "\\IO_log.txt"))
			{
				log = new DebugLog(Application.StartupPath + "\\IO_log2.txt");
			}
			else
			{
				log = new DebugLog(Application.StartupPath + "\\IO_log.txt");
			}
			log.log_on = debug;
			log.Log("Opening partition IO on: ");
			foreach (string text in files)
			{
				log.Log(text);
			}
			iolist = new global::EasyIO.EasyIO.EndianIO[files.Length];
			rlist = new global::EasyIO.EasyIO.EndianReader[files.Length];
			wlist = new global::EasyIO.EasyIO.EndianWriter[files.Length];
			for (int i = 0; i < files.Length; i++)
			{
				if (bigendian)
				{
					iolist[i] = new global::EasyIO.EasyIO.EndianIO(files[i], global::EasyIO.EasyIO.EndianType.BigEndian);
					log.Log("Opening " + files[i]);
					iolist[i].Open();
					log.Log("Opened");
					rlist[i] = new global::EasyIO.EasyIO.EndianReader(iolist[i].Stream, global::EasyIO.EasyIO.EndianType.BigEndian);
					wlist[i] = new global::EasyIO.EasyIO.EndianWriter(iolist[i].Stream, global::EasyIO.EasyIO.EndianType.BigEndian);
				}
				else
				{
					iolist[i] = new global::EasyIO.EasyIO.EndianIO(files[i], global::EasyIO.EasyIO.EndianType.LittleEndian);
					iolist[i].Open();
					rlist[i] = new global::EasyIO.EasyIO.EndianReader(iolist[i].Stream, global::EasyIO.EasyIO.EndianType.LittleEndian);
					wlist[i] = new global::EasyIO.EasyIO.EndianWriter(iolist[i].Stream, global::EasyIO.EasyIO.EndianType.LittleEndian);
				}
			}
			nio = iolist[0];
			nr = rlist[0];
			nw = wlist[0];
			calculate_length();
		}

		public PartitionIO(Stream stream)
		{
			iolist = new global::EasyIO.EasyIO.EndianIO[1];
			iolist[0] = new global::EasyIO.EasyIO.EndianIO(stream, global::EasyIO.EasyIO.EndianType.BigEndian);
			iolist[0].Open();
			rlist = new global::EasyIO.EasyIO.EndianReader[1];
			rlist[0] = new global::EasyIO.EasyIO.EndianReader(iolist[0].Stream, global::EasyIO.EasyIO.EndianType.BigEndian);
			wlist = new global::EasyIO.EasyIO.EndianWriter[1];
			wlist[0] = new global::EasyIO.EasyIO.EndianWriter(iolist[0].Stream, global::EasyIO.EasyIO.EndianType.BigEndian);
			nio = iolist[0];
			nr = rlist[0];
			nw = wlist[0];
			calculate_length();
		}

		public int SeekTo(long offset)
		{
			try
			{
				offset += global_offset;
				if (LengthOverride != 0L)
				{
					if (offset > LengthOverride)
					{
						throw new Exception("IO Overseek");
					}
				}
				else if (offset > Length)
				{
					throw new Exception("IO Overseek");
				}
				if (iolist.Length > 1)
				{
					int i;
					for (i = 0; offset >= iolist[i].Stream.Length; i++)
					{
						offset -= iolist[i].Stream.Length;
					}
					nio = iolist[i];
					nr = rlist[i];
					nw = wlist[i];
					nio.Stream.Position = offset;
					return 0;
				}
				nio.Stream.Position = offset;
				return 0;
			}
			catch
			{
				return -1;
			}
		}

		public long total_partition_length()
		{
			long num = 0L;
			for (int i = 0; i < iolist.Length; i++)
			{
				num += iolist[i].Stream.Length;
			}
			return num - global_offset;
		}

		public long total_fat_length(uint numofclusters, uint clustersize)
		{
			return iolist[0].Stream.Length - 4096 - numofclusters * clustersize;
		}

		public uint get_usb_numofc()
		{
			uint num = (uint)((iolist[0].Stream.Length - 4096) / 4);
			if (usb_rootoffset)
			{
				num += 1024;
			}
			return num;
		}

		public void Close()
		{
			nio = null;
			nr = null;
			nw = null;
			global::EasyIO.EasyIO.EndianIO[] array = iolist;
			for (int i = 0; i < array.Length; i++)
			{
				array[i]?.Close();
			}
		}
	}
}
