using System;
using System.Globalization;
using System.IO;

namespace EasyIO;

public class EasyIO
{
	public class conversions
	{
		public static byte[] AsciiToHex(string ascii)
		{
			byte[] array = new byte[ascii.Length / 2];
			for (int i = 0; i < ascii.Length / 2; i++)
			{
				array[i] = byte.Parse(ascii.Substring(i * 2, 2), NumberStyles.HexNumber);
			}
			return array;
		}

		public static string HexToAscii(byte[] hex)
		{
			string text = "";
			for (int i = 0; i < hex.Length; i++)
			{
				string text2 = hex[i].ToString("X");
				if (text2.Length == 1)
				{
					text2 = "0" + text2;
				}
				text += text2;
			}
			return text;
		}
	}

	public class EndianIO
	{
		private EndianReader _in;

		private EndianWriter _out;

		private EndianType endiantype;

		private string filepath;

		private bool isfile;

		private bool isOpen;

		private Stream stream;

		public bool Closed => !isOpen;

		public EndianReader In => _in;

		public bool Opened => isOpen;

		public EndianWriter Out => _out;

		public Stream Stream => stream;

		public EndianIO(MemoryStream MemoryStream, EndianType EndianStyle)
		{
			filepath = "";
			endiantype = EndianType.LittleEndian;
			endiantype = EndianStyle;
			stream = MemoryStream;
			isfile = false;
		}

		public EndianIO(Stream Stream, EndianType EndianStyle)
		{
			filepath = "";
			endiantype = EndianType.LittleEndian;
			endiantype = EndianStyle;
			stream = Stream;
			isfile = false;
		}

		public EndianIO(string FilePath, EndianType EndianStyle)
		{
			filepath = "";
			endiantype = EndianType.LittleEndian;
			endiantype = EndianStyle;
			filepath = FilePath;
			isfile = true;
		}

		public EndianIO(byte[] Buffer, EndianType EndianStyle)
		{
			filepath = "";
			endiantype = EndianType.LittleEndian;
			endiantype = EndianStyle;
			stream = new MemoryStream(Buffer);
			isfile = false;
		}

		public void Close()
		{
			if (isOpen)
			{
				stream.Close();
				_in.Close();
				_out.Close();
				isOpen = false;
			}
		}

		public void Open()
		{
			if (!isOpen)
			{
				if (isfile)
				{
					stream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
				}
				_in = new EndianReader(stream, endiantype);
				_out = new EndianWriter(stream, endiantype);
				isOpen = true;
			}
		}
	}

	public class EndianReader : BinaryReader
	{
		public EndianType endianstyle;

		public EndianReader(Stream stream, EndianType endianstyle)
			: base(stream)
		{
			this.endianstyle = endianstyle;
		}

		public string ReadAsciiString(int Length)
		{
			return ReadAsciiString(Length, endianstyle);
		}

		public string ReadAsciiString(int Length, EndianType EndianType)
		{
			string text = "";
			int num = 0;
			for (int i = 0; i < Length; i++)
			{
				char c = (char)ReadByte();
				num++;
				if (c == '\0')
				{
					break;
				}
				text += c;
			}
			int num2 = Length - num;
			BaseStream.Seek(num2, SeekOrigin.Current);
			return text;
		}

		public override double ReadDouble()
		{
			return ReadDouble(endianstyle);
		}

		public double ReadDouble(EndianType EndianType)
		{
			byte[] array = base.ReadBytes(4);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToDouble(array, 0);
		}

		public override int ReadInt32()
		{
			return ReadInt32(endianstyle);
		}

		public int ReadInt32(EndianType EndianType)
		{
			byte[] array = base.ReadBytes(4);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToInt32(array, 0);
		}

		public override float ReadSingle()
		{
			return ReadSingle(endianstyle);
		}

		public float ReadSingle(EndianType EndianType)
		{
			byte[] array = base.ReadBytes(4);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToSingle(array, 0);
		}

		public override ushort ReadUInt16()
		{
			return ReadUInt16(endianstyle);
		}

		public ushort ReadUInt16(EndianType EndianType)
		{
			byte[] array = base.ReadBytes(2);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToUInt16(array, 0);
		}

		public override uint ReadUInt32()
		{
			return ReadUInt32(endianstyle);
		}

		public uint ReadUInt32(EndianType EndianType)
		{
			byte[] array = base.ReadBytes(4);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToUInt32(array, 0);
		}

		public override ulong ReadUInt64()
		{
			return ReadUInt64(endianstyle);
		}

		public ulong ReadUInt64(EndianType EndianType)
		{
			byte[] array = base.ReadBytes(8);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToUInt64(array, 0);
		}

		public string ReadUnicodeString(int Length)
		{
			return ReadUnicodeString(Length, endianstyle);
		}

		public string ReadUnicodeString(int Length, EndianType EndianType)
		{
			string text = "";
			int num = 0;
			for (int i = 0; i < Length; i++)
			{
				char c = (char)ReadUInt16(EndianType);
				num++;
				if (c == '\0')
				{
					break;
				}
				text += c;
			}
			int num2 = (Length - num) * 2;
			BaseStream.Seek(num2, SeekOrigin.Current);
			return text;
		}
	}

	public enum EndianType
	{
		BigEndian,
		LittleEndian
	}

	public class EndianWriter : BinaryWriter
	{
		private EndianType endianstyle;

		public EndianWriter(Stream stream, EndianType endianstyle)
			: base(stream)
		{
			this.endianstyle = endianstyle;
		}

		public override void Write(double value)
		{
			Write(value, endianstyle);
		}

		public override void Write(short value)
		{
			Write(value, endianstyle);
		}

		public override void Write(int value)
		{
			Write(value, endianstyle);
		}

		public override void Write(long value)
		{
			Write(value, endianstyle);
		}

		public override void Write(float value)
		{
			Write(value, endianstyle);
		}

		public override void Write(ushort value)
		{
			Write(value, endianstyle);
		}

		public override void Write(uint value)
		{
			Write(value, endianstyle);
		}

		public override void Write(ulong value)
		{
			Write(value, endianstyle);
		}

		public void Write(double value, EndianType EndianType)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(bytes);
			}
			base.Write(bytes);
		}

		public void Write(short value, EndianType EndianType)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(bytes);
			}
			base.Write(bytes);
		}

		public void Write(int value, EndianType EndianType)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(bytes);
			}
			base.Write(bytes);
		}

		public void Write(long value, EndianType EndianType)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(bytes);
			}
			base.Write(bytes);
		}

		public void Write(float value, EndianType EndianType)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(bytes);
			}
			base.Write(bytes);
		}

		public void Write(ushort value, EndianType EndianType)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(bytes);
			}
			base.Write(bytes);
		}

		public void Write(uint value, EndianType EndianType)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(bytes);
			}
			base.Write(bytes);
		}

		public void Write(ulong value, EndianType EndianType)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (EndianType == EndianType.BigEndian)
			{
				Array.Reverse(bytes);
			}
			base.Write(bytes);
		}

		public void WriteAsciiString(string String, int Length)
		{
			WriteAsciiString(String, Length, endianstyle);
		}

		public void WriteAsciiString(string String, int Length, EndianType EndianType)
		{
			int length = String.Length;
			for (int i = 0; i < length && i <= Length; i++)
			{
				byte value = (byte)String[i];
				Write(value);
			}
			int num = Length - length;
			if (num > 0)
			{
				Write(new byte[num]);
			}
		}

		public void WriteUnicodeString(string String, int Length)
		{
			WriteUnicodeString(String, Length, endianstyle);
		}

		public void WriteUnicodeString(string String, int Length, EndianType EndianType)
		{
			int length = String.Length;
			for (int i = 0; i < length && i <= Length; i++)
			{
				ushort value = String[i];
				Write(value, EndianType);
			}
			int num = (Length - length) * 2;
			if (num > 0)
			{
				Write(new byte[num]);
			}
		}
	}

	public static byte[] FlipBytesBy8(byte[] imput)
	{
		byte[] array = new byte[imput.Length];
		int num = imput.Length - 8;
		int num2 = 0;
		for (int i = 0; i < imput.Length / 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				array[num2 + j] = imput[num + j];
			}
			num -= 8;
			num2 += 8;
		}
		return array;
	}
}
