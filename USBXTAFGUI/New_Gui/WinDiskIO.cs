using System;
using System.IO;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using New_Gui.Win32;

namespace New_Gui;

public class WinDiskIO : Stream
{
	private SafeFileHandle handle;

	private API.DISK_GEOMETRY diskgeo;

	private uint suboffset;

	private uint oldsector;

	private uint sector;

	private bool open;

	private long pos;

	private long length;

	private byte[] diskbuffer = new byte[512];

	private FileStream diskStream;

	public override long Position
	{
		get
		{
			return pos + suboffset;
		}
		set
		{
			pos = value;
			long num = pos / 512 * 512;
			suboffset = (uint)(pos - num);
			pos = num;
			_ = oldsector;
			_ = sector;
		}
	}

	public override long Length => diskgeo.DiskSize;

	public override bool CanWrite => open;

	public override bool CanSeek => open;

	public override bool CanRead => open;

	public WinDiskIO(int devicenumber)
	{
		handle = API.CreateFile("\\\\.\\PhysicalDrive" + devicenumber, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, API.FlagsAndAttributes.Device | API.FlagsAndAttributes.Write_Through | API.FlagsAndAttributes.NoBuffering, IntPtr.Zero);
		if (!handle.IsInvalid)
		{
			new API().GetDriveGeometry(ref diskgeo, devicenumber, handle);
			diskStream = new FileStream(handle, FileAccess.ReadWrite);
			diskStream.Position = 0L;
			loadbuffer();
			Position = 0L;
			open = true;
			length = diskgeo.DiskSize;
			return;
		}
		throw new Exception("Failed to open HDD");
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		long position = Position;
		int num = count;
		int num2 = 0;
		do
		{
			Position = position + num2;
			int num3 = (int)(512 - suboffset);
			if (num3 > num)
			{
				num3 = num;
			}
			loadbuffer();
			Array.Copy(buffer, offset + num2, diskbuffer, suboffset, num3);
			Position = position + num2;
			syncbuffer();
			num -= num3;
			num2 += num3;
		}
		while (num > 0);
		Position = position;
		Position += count;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		long position = Position;
		int num = count;
		int num2 = 0;
		do
		{
			Position = position + num2;
			int num3 = (int)(512 - suboffset);
			if (num3 > num)
			{
				num3 = num;
			}
			loadbuffer();
			Array.Copy(diskbuffer, suboffset, buffer, offset + num2, num3);
			num -= num3;
			num2 += num3;
		}
		while (num > 0);
		Position = position;
		Position += count;
		return count;
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		switch (origin)
		{
		case SeekOrigin.Begin:
			Position = offset;
			break;
		case SeekOrigin.Current:
			Position += offset;
			break;
		default:
			Position = diskStream.Length;
			break;
		}
		return Position;
	}

	public override void Flush()
	{
		throw new NotImplementedException();
	}

	public void loadbuffer()
	{
		oldsector = sector;
		sector = (uint)pos / 512;
		if (oldsector != sector)
		{
			do
			{
				Thread.Sleep(0);
			}
			while (!diskStream.CanRead);
			if (diskStream.Position != pos)
			{
				diskStream.Position = pos;
			}
			diskStream.Read(diskbuffer, 0, 512);
		}
	}

	public void syncbuffer()
	{
		do
		{
			Thread.Sleep(0);
		}
		while (!diskStream.CanWrite);
		diskStream.Seek(pos, SeekOrigin.Begin);
		diskStream.Write(diskbuffer, 0, 512);
		diskStream.Flush();
	}
}
