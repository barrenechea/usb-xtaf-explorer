using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EasyIO;

namespace XtafRewrite;

public class Xtaf
{
	public enum DirentFlags : byte
	{
		Normal = 0,
		ReadOnly = 1,
		Hidden = 2,
		System = 4,
		Directory = 0x10,
		Archive = 0x20,
		Device = 0x40
	}

	public struct FileSystemHeader
	{
		public string ident;

		public uint volid;

		public uint spc;

		public uint nfat;

		public ushort zero;
	}

	public struct FileSystemInfo
	{
		public FileSystemHeader fsheader;

		public long pwd;

		public byte fatmult;

		public uint fatstart;

		public uint fatsize;

		public long rootstart;

		public uint cluster_size;

		public long calcadd;

		public uint NumOfClusters;
	}

	public class XtafFileSystem
	{
		public class XtafDirectory
		{
			private uint cluster;

			public bool isInvalid;

			public XtafDirent dirent;

			public XtafDirectory Parent_Directory;

			private uint[] clusters;

			public string name;

			public long pwd;

			public XtafDirent[] dirents;

			public List<XtafDirectory> SubDirectories = new List<XtafDirectory>();

			public List<XtafDirent> SubFiles = new List<XtafDirent>();

			public XtafFileSystem parent;

			public void enter()
			{
				parent.curDir = this;
			}

			public void leave()
			{
				if (Parent_Directory != null)
				{
					parent.curDir = Parent_Directory;
				}
			}

			public void Reload()
			{
				clusters = parent.Read_Fatchain(cluster);
				XtafDirectory xtafDirectory = new XtafDirectory(parent, name, clusters[0], Parent_Directory);
				dirents = xtafDirectory.dirents;
				SubDirectories = xtafDirectory.SubDirectories;
				SubFiles = xtafDirectory.SubFiles;
				parent.curDir = xtafDirectory;
			}

			public XtafDirectory(XtafFileSystem parent, string name, uint cluster, XtafDirectory parentdir)
			{
				Parent_Directory = parentdir;
				this.parent = parent;
				pwd = parent.seek_to_offset(cluster);
				this.cluster = cluster;
				try
				{
					clusters = parent.Read_Fatchain(cluster);
				}
				catch
				{
					isInvalid = true;
					return;
				}
				this.name = name;
				dirents = new XtafDirent[clusters.Length * parent.fsinfo.cluster_size / 64];
				for (int i = 0; i < clusters.Length; i++)
				{
					Array.Copy(LoadDirentsForCluster(clusters[i]), 0L, dirents, i * (parent.fsinfo.cluster_size / 64), parent.fsinfo.cluster_size / 64);
				}
				XtafDirent[] array = dirents;
				foreach (XtafDirent xtafDirent in array)
				{
					if (xtafDirent.isDir & !xtafDirent.open)
					{
						XtafDirectory xtafDirectory = new XtafDirectory(parent, xtafDirent.name, xtafDirent.fstart, this)
						{
							dirent = xtafDirent
						};
						SubDirectories.Add(xtafDirectory);
						xtafDirent.AssignedDir = xtafDirectory;
						if (xtafDirent.AssignedDir.isInvalid)
						{
							xtafDirent.isValid = false;
						}
					}
					else if (!xtafDirent.open)
					{
						SubFiles.Add(xtafDirent);
					}
				}
			}

			public XtafDirent[] LoadDirentsForCluster(uint cluster)
			{
				XtafDirent[] array = new XtafDirent[parent.fsinfo.cluster_size / 64];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new XtafDirent(parent.seek_to_offset(cluster) + i * 64, parent);
				}
				return array;
			}

			public long find_open_dirent()
			{
				int num = clusters.Length;
				for (int i = 0; i < parent.fsinfo.cluster_size * num / 64; i++)
				{
					if (dirents[i].open)
					{
						return get_dirent_offset(i);
					}
				}
				uint num2 = parent.find_open_fat(2u);
				parent.seek_to_fat(clusters[clusters.Length - 1]);
				if (parent.fsinfo.fatmult == 2)
				{
					parent.pio.nw.Write((ushort)num2);
				}
				else
				{
					parent.pio.nw.Write(num2);
				}
				parent.seek_to_fat(num2);
				if (parent.fsinfo.fatmult == 2)
				{
					parent.pio.nw.Write(ushort.MaxValue);
				}
				else
				{
					parent.pio.nw.Write(uint.MaxValue);
				}
				parent.seek_to_offset(num2);
				byte[] array = new byte[parent.fsinfo.cluster_size];
				for (int j = 0; j < parent.fsinfo.cluster_size; j++)
				{
					array[j] = Convert.ToByte(255);
				}
				parent.pio.nw.Write(array);
				Reload();
				return find_open_dirent();
			}

			public long get_dirent_offset(int direntnum)
			{
				uint[] array = clusters;
				int num = 0;
				while (direntnum >= parent.fsinfo.cluster_size / 64)
				{
					direntnum = (int)(direntnum - parent.fsinfo.cluster_size / 64);
					num++;
				}
				return parent.seek_to_offset(array[num]) + direntnum * 64;
			}

			public void injectfile(string fpath)
			{
				if (isInvalid)
				{
					return;
				}
				if (find_by_name(Path.GetFileName(fpath)) == null)
				{
					if (File.Exists(fpath))
					{
						global::EasyIO.EasyIO.EndianIO endianIO = new global::EasyIO.EasyIO.EndianIO(fpath, global::EasyIO.EasyIO.EndianType.BigEndian);
						endianIO.Open();
						global::EasyIO.EasyIO.EndianReader endianReader = new global::EasyIO.EasyIO.EndianReader(endianIO.Stream, global::EasyIO.EasyIO.EndianType.BigEndian);
						FileInfo fileInfo = new FileInfo(fpath);
						long num = fileInfo.Length / parent.fsinfo.cluster_size;
						if (num * parent.fsinfo.cluster_size < fileInfo.Length)
						{
							num++;
						}
						if (num == 0L)
						{
							num = 1L;
						}
						uint[] array = parent.create_open_fat_chain(Convert.ToInt32(num));
						parent.link_cluster_chain(array);
						byte[] array2 = new byte[parent.fsinfo.cluster_size];
						for (int i = 0; i <= array.Length - 1; i++)
						{
							endianIO.Stream.Position = i * parent.fsinfo.cluster_size;
							array2 = endianReader.ReadBytes(Convert.ToInt32(parent.fsinfo.cluster_size));
							endianIO.Stream.Position = i * parent.fsinfo.cluster_size;
							parent.seek_to_offset(array[i]);
							parent.pio.nw.Write(array2);
						}
						long offset = find_open_dirent();
						XtafDirent xtafDirent = dirents[0];
						xtafDirent.adate = 0;
						xtafDirent.atime = 0;
						xtafDirent.cdate = 0;
						xtafDirent.ctime = 0;
						xtafDirent.flags = 0;
						xtafDirent.fsize = Convert.ToUInt32(fileInfo.Length);
						Console.WriteLine("New file size is " + Convert.ToString(xtafDirent.fsize) + " Bytes");
						xtafDirent.fstart = array[0];
						xtafDirent.name = Path.GetFileName(fpath);
						xtafDirent.namelen = Convert.ToByte(xtafDirent.name.Length);
						xtafDirent.udate = 0;
						xtafDirent.utime = 0;
						xtafDirent.offset = offset;
						xtafDirent.write();
						endianIO.Close();
					}
					else
					{
						recurisive_inject(fpath);
					}
					return;
				}
				throw new File_Exists();
			}

			public void recurisive_inject(string filepath)
			{
				newfolder(Path.GetFileName(filepath));
				Reload();
				XtafDirent xtafDirent = find_by_name(Path.GetFileName(filepath));
				xtafDirent.AssignedDir.enter();
				string[] files = Directory.GetFiles(filepath);
				foreach (string fpath in files)
				{
					parent.curDir.injectfile(fpath);
					parent.curDir.Reload();
				}
				files = Directory.GetDirectories(filepath);
				foreach (string path in files)
				{
					parent.curDir.recurisive_inject(filepath + "\\" + Path.GetFileName(path));
					parent.curDir.Reload();
				}
				xtafDirent.AssignedDir.leave();
			}

			public XtafDirent find_by_name(string name)
			{
				name = name.ToLower();
				XtafDirent[] array = parent.curDir.dirents;
				foreach (XtafDirent xtafDirent in array)
				{
					if ((xtafDirent.name.ToLower() == name) & !xtafDirent.open)
					{
						return xtafDirent;
					}
				}
				return null;
			}

			public void newfolder(string name)
			{
				long num = find_open_dirent();
				if (num != -1)
				{
					Console.WriteLine("New folder at " + Convert.ToString(num));
					uint fstart = parent.find_open_fat(2u);
					XtafDirent xtafDirent = dirents[0];
					xtafDirent.adate = 0;
					xtafDirent.atime = 0;
					xtafDirent.cdate = 0;
					xtafDirent.ctime = 0;
					xtafDirent.flags = 16;
					xtafDirent.fsize = 0u;
					xtafDirent.fstart = fstart;
					xtafDirent.name = name;
					xtafDirent.namelen = Convert.ToByte(name.Length);
					xtafDirent.udate = 0;
					xtafDirent.utime = 0;
					parent.seek_to_fat(fstart);
					if (parent.fsinfo.fatmult == 4)
					{
						uint value = uint.MaxValue;
						parent.pio.nw.Write(value);
					}
					else
					{
						ushort value2 = ushort.MaxValue;
						parent.pio.nw.Write(value2);
					}
					byte[] array = new byte[parent.fsinfo.cluster_size];
					for (int i = 0; i < parent.fsinfo.cluster_size; i++)
					{
						array[i] = Convert.ToByte(255);
					}
					parent.seek_to_offset(fstart);
					parent.pio.nw.Write(array);
					xtafDirent.offset = num;
					xtafDirent.write();
				}
			}
		}

		public class XtafDirent
		{
			public byte namelen;

			public byte flags;

			public string name;

			public uint fstart;

			public uint fsize;

			public ushort cdate;

			public ushort ctime;

			public ushort adate;

			public ushort atime;

			public ushort udate;

			public ushort utime;

			public bool open;

			public bool isDir;

			public long offset;

			public bool isValid = true;

			public XtafDirectory AssignedDir;

			public XtafFileSystem parent;

			public void rename(string name)
			{
				parent.pio.SeekTo(offset);
				parent.pio.nw.Write((byte)name.Length);
				parent.pio.SeekTo(offset + 2);
				parent.pio.nw.WriteAsciiString(name, name.Length);
			}

			public void write()
			{
				parent.pio.SeekTo(offset);
				parent.pio.nw.Write(namelen);
				parent.pio.nw.Write(flags);
				parent.pio.SeekTo(offset + 1);
				parent.pio.nw.Write(name);
				parent.pio.SeekTo(offset + 1);
				parent.pio.nw.Write(flags);
				parent.pio.SeekTo(offset + 44);
				parent.pio.nw.Write(fstart);
				parent.pio.nw.Write(fsize);
				parent.pio.nw.Write(cdate);
				parent.pio.nw.Write(ctime);
				parent.pio.nw.Write(adate);
				parent.pio.nw.Write(atime);
				parent.pio.nw.Write(udate);
				parent.pio.nw.Write(utime);
			}

			public XtafDirent(long offset, XtafFileSystem parent)
			{
				this.parent = parent;
				if (parent.pio.SeekTo(offset) == -1)
				{
					isValid = false;
				}
				namelen = parent.pio.nr.ReadByte();
				flags = parent.pio.nr.ReadByte();
				name = parent.pio.nr.ReadAsciiString(namelen);
				parent.pio.SeekTo(offset + 44);
				fstart = parent.pio.nr.ReadUInt32();
				fsize = parent.pio.nr.ReadUInt32();
				cdate = parent.pio.nr.ReadUInt16();
				ctime = parent.pio.nr.ReadUInt16();
				adate = parent.pio.nr.ReadUInt16();
				atime = parent.pio.nr.ReadUInt16();
				udate = parent.pio.nr.ReadUInt16();
				utime = parent.pio.nr.ReadUInt16();
				if (namelen == byte.MaxValue)
				{
					open = true;
				}
				else
				{
					open = false;
				}
				if (namelen == 229)
				{
					open = true;
				}
				this.offset = offset;
				if ((flags & 0x10) == 16)
				{
					isDir = true;
				}
				else
				{
					isDir = false;
				}
			}

			public void extract(string filepath)
			{
				if (isDir)
				{
					recurisive_extract(this, filepath);
					return;
				}
				if (File.Exists(filepath))
				{
					File.Delete(filepath);
				}
				global::EasyIO.EasyIO.EndianIO endianIO = new global::EasyIO.EasyIO.EndianIO(filepath, global::EasyIO.EasyIO.EndianType.BigEndian);
				endianIO.Open();
				global::EasyIO.EasyIO.EndianWriter endianWriter = new global::EasyIO.EasyIO.EndianWriter(endianIO.Stream, global::EasyIO.EasyIO.EndianType.BigEndian);
				uint[] array = parent.Read_Fatchain(fstart);
				int num = 0;
				for (num = 0; num < array.Length - 2; num++)
				{
					parent.seek_to_offset(array[num]);
					byte[] buffer = parent.pio.nr.ReadBytes((int)parent.fsinfo.cluster_size);
					endianWriter.Write(buffer);
				}
				parent.seek_to_offset(array[num]);
				byte[] buffer2 = parent.pio.nr.ReadBytes((int)(fsize - num * parent.fsinfo.cluster_size));
				endianWriter.Write(buffer2);
				endianWriter.Flush();
				endianWriter.Close();
				endianIO.Close();
			}

			public void recurisive_extract(XtafDirent d, string fpath)
			{
				Directory.CreateDirectory(fpath);
				d.AssignedDir.enter();
				XtafDirent[] dirents = parent.curDir.dirents;
				foreach (XtafDirent xtafDirent in dirents)
				{
					if (!xtafDirent.open)
					{
						if (xtafDirent.isDir)
						{
							recurisive_extract(xtafDirent, fpath + "\\" + xtafDirent.name);
						}
						else
						{
							xtafDirent.extract(fpath + "\\" + xtafDirent.name);
						}
					}
				}
				d.AssignedDir.leave();
			}

			public void delete()
			{
				parent.pio.SeekTo(offset);
				parent.pio.nw.Write((byte)229);
				parent.unlink_cluster_chain(fstart);
				open = true;
			}
		}

		public XtafDirectory curDir;

		private FileSystemHeader fsheader;

		public FileSystemInfo fsinfo;

		public XtafIO.PartitionIO pio;

		public XtafDirectory rootdir;

		private bool isusb;

		public XtafFileSystem(XtafIO.PartitionIO io, long offset, bool isusb, uint numofc, bool debug)
		{
			File.Exists(Application.StartupPath + "\\XtafLog.txt");
			pio = io;
			pio.global_offset = offset;
			this.isusb = isusb;
			readheader(numofc);
			rootdir = new XtafDirectory(this, "Root", 1u, null);
		}

		private void readheader(uint numofc)
		{
			pio.SeekTo(0L);
			fsheader.ident = pio.nr.ReadAsciiString(4);
			fsheader.volid = pio.nr.ReadUInt32();
			fsheader.spc = pio.nr.ReadUInt32();
			fsheader.nfat = pio.nr.ReadUInt32();
			fsheader.zero = pio.nr.ReadUInt16();
			fsinfo.fsheader = fsheader;
			fsinfo.cluster_size = fsheader.spc * 512;
			fsinfo.fatstart = 4096u;
			fsinfo.NumOfClusters = numofc;
			if (fsinfo.NumOfClusters >= 65524)
			{
				fsinfo.fatmult = 4;
			}
			else
			{
				fsinfo.fatmult = 2;
			}
			if (isusb)
			{
				fsinfo.fatmult = 4;
			}
			fsinfo.fatsize = numofc * fsinfo.fatmult;
			fsinfo.rootstart = fsinfo.fatstart + fsinfo.fatsize;
			fsinfo.pwd = fsinfo.rootstart;
			fsinfo.calcadd = 0L;
			fsinfo.calcadd = fsinfo.fatsize - (fsinfo.cluster_size - 4096);
		}

		public long seek_to_offset(uint cluster)
		{
			pio.SeekTo((long)fsinfo.cluster_size * (long)cluster + fsinfo.calcadd);
			return (long)fsinfo.cluster_size * (long)cluster + fsinfo.calcadd;
		}

		public void seek_to_fat(uint cluster)
		{
			pio.SeekTo(fsinfo.fatmult * cluster + fsinfo.fatstart);
		}

		public uint[] Read_Fatchain(uint startc)
		{
			uint[] array = new uint[1];
			uint cluster = startc;
			seek_to_fat(cluster);
			uint num = pio.nr.ReadUInt32();
			int num2 = 1;
			bool flag = false;
			array[0] = startc;
			uint num3 = 0u;
			do
			{
				seek_to_fat(cluster);
				num = ((fsinfo.fatmult != 4) ? pio.nr.ReadUInt16() : pio.nr.ReadUInt32());
				Array.Resize(ref array, num2 + 1);
				if (fsinfo.fatmult == 4)
				{
					if (num == uint.MaxValue)
					{
						flag = true;
						continue;
					}
					array[num2] = num;
					cluster = num;
					num2++;
					if (num == num3)
					{
						throw new Exception("Fat chain read infininte looped, likely that the partition is corrupted, sorry");
					}
					num3 = num;
					flag = false;
				}
				else if (num == 65535)
				{
					flag = true;
				}
				else
				{
					array[num2] = num;
					cluster = num;
					num2++;
					flag = false;
				}
			}
			while (!flag);
			Array.Resize(ref array, num2);
			return array;
		}

		public uint find_open_fat(uint startc)
		{
			bool flag = false;
			uint num = startc;
			do
			{
				seek_to_fat(num);
				_ = pio.nio.Stream.Position;
				_ = fsinfo.fatstart + fsinfo.fatsize;
				if (((fsinfo.fatmult != 2) ? pio.nr.ReadUInt32() : pio.nr.ReadUInt16()) == 0)
				{
					flag = true;
				}
				else
				{
					num++;
				}
			}
			while (!flag);
			return num;
		}

		public uint[] create_open_fat_chain(int number)
		{
			uint startc = 2u;
			uint[] array = new uint[number];
			for (int i = 0; i < number; i++)
			{
				array[i] = find_open_fat(startc);
				startc = array[i] + 1;
			}
			return array;
		}

		public void link_cluster_chain(uint[] clusters)
		{
			int num = 0;
			for (num = 0; num <= clusters.Length - 2; num++)
			{
				seek_to_fat(clusters[num]);
				if (fsinfo.fatmult == 4)
				{
					uint value = clusters[num + 1];
					pio.nw.Write(value);
				}
				else
				{
					ushort value2 = (ushort)clusters[num + 1];
					pio.nw.Write(value2);
				}
			}
			seek_to_fat(clusters[num]);
			if (fsinfo.fatmult == 4)
			{
				pio.nw.Write(uint.MaxValue);
				return;
			}
			ushort value3 = ushort.MaxValue;
			pio.nw.Write(value3);
		}

		public void unlink_cluster_chain(uint startc)
		{
			Console.WriteLine("Unlinking");
			uint num = startc;
			bool flag = false;
			uint num2 = 0u;
			do
			{
				if (fsinfo.fatmult == 4)
				{
					seek_to_fat(num);
					if (num == 0)
					{
						break;
					}
					uint num3 = pio.nr.ReadUInt32();
					seek_to_fat(num);
					if (num3 != uint.MaxValue)
					{
						num = num3;
						pio.nw.Write(num2);
					}
					else
					{
						pio.nw.Write(num2);
						flag = true;
					}
				}
				else
				{
					seek_to_fat(num);
					if (num == 0)
					{
						break;
					}
					uint num3 = pio.nr.ReadUInt16();
					seek_to_fat(num);
					if (num3 != 65535)
					{
						num = num3;
						pio.nw.Write((ushort)num2);
					}
					else
					{
						pio.nw.Write((ushort)num2);
						flag = true;
					}
				}
			}
			while (!flag);
		}
	}
}
