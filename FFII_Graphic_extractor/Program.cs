using System.Text;
using System.Linq;
using System.Diagnostics.Metrics;

class Program : Coder
{
    static void strToByte(string str)
    {
        string _byte = "";
        int i = 0;
        foreach (char c in str)
        {
            _byte += c;
            if (_byte.Length == 8)
            {
                Console.Write(Convert.ToString(Convert.ToInt32(_byte, 2), 16).PadLeft(2, '0') + " ");
                _byte = "";
                i++;
            }
            if (i == 16)
            {
                Console.WriteLine();
                i = 0;
            }
        }
    }
    static void Main(String[] args)
    {
        string gamePath = "C:\\Users\\test\\Desktop\\out_25159A";
        string decompressedFilePath = "C:\\Users\\test\\Desktop\\out_g";
        strToByte(codeGraphic(gamePath, decompressedFilePath));
        //codeGraphic(gamePath, decompressedFilePath);
        //decodeGraphic(args[0], args[1]);
        //decodeGraphic(gamePath, decompressedFilePath);
    }
}
/*
 string compressedFilePath = "C:\\Users\\test\\Desktop\\extracting\\title";
        string decompressedFilePath = "C:\\Users\\test\\Desktop\\extracting\\outt2.bin";
        CircularBuffer buffer = new CircularBuffer(0x100);
        CircularBuffer bitBuffer = new CircularBuffer(8);

        using (FileStream compStream = new FileStream(compressedFilePath, FileMode.Open))
        using (FileStream decompStream = new FileStream(decompressedFilePath, FileMode.Create))
        {
            byte[] header = new byte[8];
            compStream.Read(header, 0, 8);
            int decodedGraphicSize = Convert.ToInt16(Convert.ToString(header[7], 16) +
                                                     Convert.ToString(header[6], 16), 16);

            bitUpdate(bitBuffer, compStream);
            byte[] byteBuilder = new byte[8];

            do
            {
                //flag checking
                switch (readBit(bitBuffer, compStream))
                {
                    case 1:
                        for (int i = 0; i < 8; i++)
                        {
                            byteBuilder[i] = readBit(bitBuffer, compStream);
                        }

                        buffer.addByte(buildByte(byteBuilder));
                        decompStream.WriteByte(buildByte(byteBuilder));
                        break;
                    case 0:
                        byte offset;
                        byte length;

                        for (int i = 0; i < 8; i++)
                        {
                            byteBuilder[i] = readBit(bitBuffer, compStream);
                        }
                        offset = buildByte(byteBuilder);
                        length = (byte)(buildByte(new byte[]{
                                readBit(bitBuffer, compStream),
                                readBit(bitBuffer, compStream),
                                readBit(bitBuffer, compStream)
                        }) + 3);

                        buffer.byteRefresh(offset, length);
                        decompStream.Write(buffer.getSetBytes((byte)(offset - 1), length), 0, length);
                        decompStream.Flush();
                        break;
                }
            } while (decompStream.Position != decodedGraphicSize);
        }
*/