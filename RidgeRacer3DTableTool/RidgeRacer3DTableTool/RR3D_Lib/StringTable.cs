using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RidgeRacer3DTableTool.RR3D_Lib
{
    class Header
    {
        public string magic = "TBL ";
        public int dataPtr;
        //public int unk08h;
        //public int unk0Ch;
        public short count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream">read</param>
        /// <param name="headerFileStream">write</param>
        /// <returns></returns>
        public bool Unpack(FileStream fileStream, FileStream headerFileStream)
        {

            byte[] bytes = new byte[4];
            fileStream.Read(bytes, 0x00, bytes.Length);

            Encoding enc = Encoding.UTF8;
            string readMagic = enc.GetString(bytes);

            if (!magic.Equals(readMagic))
            {
                return true;
            }

            fileStream.Read(bytes, 0x00, bytes.Length);
            dataPtr = BitConverter.ToInt32(bytes, 0x00);

            //fileStream.Read(bytes, 0x00, bytes.Length);
            //unk08h = BitConverter.ToInt32(bytes, 0x00);
            
            //fileStream.Read(bytes, 0x00, bytes.Length);
            //unk0Ch = BitConverter.ToInt32(bytes, 0x00);

            fileStream.Seek(0x10, SeekOrigin.Begin);
            bytes = new byte[2];
            fileStream.Read(bytes, 0x00, bytes.Length);
            count = BitConverter.ToInt16(bytes, 0x00);

            fileStream.Seek(0x04, SeekOrigin.Begin);
            bytes = new byte[dataPtr-0x04];
            fileStream.Read(bytes, 0x00, bytes.Length);
            headerFileStream.Write(bytes, 0x00, bytes.Length);

            return false;
        }

        public bool Pack(FileStream fileStream, FileStream headerFileStream)
        {

            Encoding enc = Encoding.UTF8;
            byte[] _magic = enc.GetBytes(magic);

            byte[] bytes = new byte[headerFileStream.Length];
            headerFileStream.Read(bytes, 0x00, bytes.Length);

            byte[] writeBytes = new byte[_magic.Length + bytes.Length];

            int _pos = 0x00;
            Array.Copy(_magic, 0x00, writeBytes, _pos, _magic.Length);
            _pos += _magic.Length;
            Array.Copy(bytes, 0x00, writeBytes, _pos, bytes.Length);
            _pos += _magic.Length;

            fileStream.Write(writeBytes, 0x00, writeBytes.Length);
            return false;
        }


    }

    class StringElement
    {
        public string id;
        public int totalLength;
        public int unk14h = 0x1C;
        public short textLength;
        public short unk1Ah = 0x100;
        // byte terminator1
        public string text;
        // byte terminator2

        public bool Unpack(FileStream fileStream)
        {

            byte[] bytes = new byte[0x10];
            fileStream.Read(bytes, 0x00, bytes.Length);
            Encoding enc = Encoding.UTF8;
            string _str = "";
            _str = enc.GetString(bytes);
            id = _str.TrimEnd('\0');

            bytes = new byte[4];
            fileStream.Read(bytes, 0x00, bytes.Length);
            totalLength = BitConverter.ToInt32(bytes, 0x00);
            fileStream.Read(bytes, 0x00, bytes.Length);
            unk14h = BitConverter.ToInt32(bytes, 0x00);
            bytes = new byte[2];
            fileStream.Read(bytes, 0x00, bytes.Length);
            textLength = BitConverter.ToInt16(bytes, 0x00);
            fileStream.Read(bytes, 0x00, bytes.Length);
            unk1Ah = BitConverter.ToInt16(bytes, 0x00);

            fileStream.Seek(0x01, SeekOrigin.Current);

            bytes = new byte[textLength];
            fileStream.Read(bytes, 0x00, bytes.Length);
            _str = enc.GetString(bytes);
            _str = _str.TrimEnd('\0');
            _str = _str.Replace("\r", @"\r");
            text = _str.Replace("\n", @"\n");

            fileStream.Seek(0x01, SeekOrigin.Current);

            return false;
        }

        public bool Pack(FileStream fileStream)
        {
            Encoding enc = Encoding.UTF8;
            string writeText = text.Replace("\\n", "\n");
            writeText = writeText.Replace("\\r", "\r");
            byte[] bytes = enc.GetBytes(writeText);
            byte[] textBytes = new byte[bytes.Length + 1];
            Array.Copy(bytes, 0x00, textBytes, 0x00, bytes.Length);
            int _val = textBytes.Length;
            if (_val > ushort.MaxValue)
            {
                return true;
            }
            textLength = (short)_val;
            byte[] textLengthBytes = BitConverter.GetBytes(textLength);

            byte[] idBytes = new byte[0x10];
            bytes = enc.GetBytes(id);
            Array.Copy(bytes, 0x00, idBytes, 0x00, bytes.Length);

            byte[] unk14hBytes = BitConverter.GetBytes(unk14h);
            byte[] unk1AhBytes = BitConverter.GetBytes(unk1Ah);

            totalLength = idBytes.Length
                + 0x04 + unk14hBytes.Length + textLengthBytes.Length + unk1AhBytes.Length + 0x01
                + textLength + 0x01;
            byte[] totaLengthBytes = BitConverter.GetBytes(totalLength);

            byte[] writeBytes = new byte[totalLength];
            int _pos = 0x00;
            Array.Copy(idBytes, 0x00, writeBytes, _pos, idBytes.Length);
            _pos += idBytes.Length;
            Array.Copy(totaLengthBytes, 0x00, writeBytes, _pos, totaLengthBytes.Length);
            _pos += totaLengthBytes.Length;
            Array.Copy(unk14hBytes, 0x00, writeBytes, _pos, unk14hBytes.Length);
            _pos += unk14hBytes.Length;
            Array.Copy(textLengthBytes, 0x00, writeBytes, _pos, textLengthBytes.Length);
            _pos += textLengthBytes.Length;
            Array.Copy(unk1AhBytes, 0x00, writeBytes, _pos, unk1AhBytes.Length);
            _pos += unk1AhBytes.Length;
            writeBytes[_pos] = 0x00;
            _pos += 0x01;
            Array.Copy(textBytes, 0x00, writeBytes, _pos, textBytes.Length);
            _pos += textBytes.Length;
            writeBytes[_pos] = 0x00;
            _pos += 0x01;
            fileStream.Write(writeBytes, 0x00, writeBytes.Length);

            return false;
        }

    }

}
