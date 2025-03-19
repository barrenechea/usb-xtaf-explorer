using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace New_Gui.Win32;

internal class API
{
	public struct DISK_GEOMETRY
	{
		private long Cylinders;

		private MEDIA_TYPE type;

		private uint TracksPerCylinder;

		private uint SectorsPerTrack;

		private uint BytesPerSector;

		public long DiskSize => Cylinders * TracksPerCylinder * SectorsPerTrack * BytesPerSector;
	}

	public enum EIOControlCode
	{
		IOCTL_DISK_GET_DRIVE_GEOMETRY = 458752
	}

	[Flags]
	public enum MEDIA_TYPE : uint
	{
		Unknown = 0u,
		F5_1Pt2_512 = 1u,
		F3_1Pt44_512 = 2u,
		F3_2Pt88_512 = 3u,
		F3_20Pt8_512 = 4u,
		F3_720_512 = 5u,
		F5_360_512 = 6u,
		F5_320_512 = 7u,
		F5_320_1024 = 8u,
		F5_180_512 = 9u,
		F5_160_512 = 0xAu,
		RemovableMedia = 0xBu,
		FixedMedia = 0xCu,
		F3_120M_512 = 0xDu,
		F3_640_512 = 0xEu,
		F5_640_512 = 0xFu,
		F5_720_512 = 0x10u,
		F3_1Pt2_512 = 0x11u,
		F3_1Pt23_1024 = 0x12u,
		F5_1Pt23_1024 = 0x13u,
		F3_128Mb_512 = 0x14u,
		F3_230Mb_512 = 0x15u,
		F8_256_128 = 0x16u,
		F3_200Mb_512 = 0x17u,
		F3_240M_512 = 0x18u,
		F3_32M_512 = 0x19u
	}

	[Flags]
	public enum EFileAccess : uint
	{
		GenericRead = 0x80000000u,
		GenericWrite = 0x40000000u,
		GenericExecute = 0x20000000u,
		GenericAll = 0x10000000u
	}

	[Flags]
	public enum EFileShare : uint
	{
		None = 0u,
		Read = 1u,
		Write = 2u,
		Delete = 4u
	}

	public enum ECreationDisposition : uint
	{
		New = 1u,
		CreateAlways,
		OpenExisting,
		OpenAlways,
		TruncateExisting
	}

	[Flags]
	public enum FlagsAndAttributes : uint
	{
		Readonly = 1u,
		Hidden = 2u,
		System = 4u,
		Directory = 0x10u,
		Archive = 0x20u,
		Device = 0x40u,
		Normal = 0x80u,
		Temporary = 0x100u,
		SparseFile = 0x200u,
		ReparsePoint = 0x400u,
		Compressed = 0x800u,
		Offline = 0x1000u,
		NotContentIndexed = 0x2000u,
		Encrypted = 0x4000u,
		Write_Through = 0x80000000u,
		Overlapped = 0x40000000u,
		NoBuffering = 0x20000000u,
		MiscAccess = 0x10000000u,
		SequentialScan = 0x8000000u,
		DeleteOnClose = 0x4000000u,
		BackupSemantics = 0x2000000u,
		PosixSemantics = 0x1000000u,
		OpenReparsePoint = 0x200000u,
		OpenNoRecall = 0x100000u,
		FirstPipeInstance = 0x80000u
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern SafeFileHandle CreateFile(string lpFileName, FileAccess dwDesiredAccess, FileShare dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition, FlagsAndAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

	[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
	public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

	[DllImport("kernel32.dll")]
	private static extern bool DeviceIoControl(SafeHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, ref DISK_GEOMETRY lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern int CloseHandle(IntPtr hObject);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern int CloseHandle(SafeFileHandle hObject);

	internal bool GetDriveGeometry(ref DISK_GEOMETRY diskGeo, int driveID, SafeFileHandle handle)
	{
		_ = IntPtr.Zero;
		uint lpBytesReturned;
		return DeviceIoControl(handle, 458752u, IntPtr.Zero, 0u, ref diskGeo, (uint)Marshal.SizeOf(typeof(DISK_GEOMETRY)), out lpBytesReturned, IntPtr.Zero);
	}
}
