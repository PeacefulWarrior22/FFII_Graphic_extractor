using System.Text;

class Program
{
    static byte bitPosition = 0;
    static void bitUpdate(CircularBuffer buffer, Stream streamBit)
    {
        byte bit = (byte)streamBit.ReadByte();
        foreach (var item in Convert.ToString(bit, 2).PadLeft(8, '0'))
        {
            buffer.addByte(byte.Parse(item.ToString()));
        }
    }

    static byte readBit(CircularBuffer buffer, FileStream stream)
    {
        if (bitPosition == buffer.size)
        {
            bitPosition = 0;
            bitUpdate(buffer, stream);
        }
        bitPosition++;
        return buffer.getByte((byte)(bitPosition - 1));
    }

    static byte buildByte(byte[] bits)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var item in bits)
        {
            sb.Append(item.ToString());
        }
        return Convert.ToByte(sb.ToString(), 2);
    }

    static void Main(String[] args)
    {
        string compressedFilePath = "C:\\Users\\test\\Desktop\\title3";
        string decompressedFilePath = "C:\\Users\\test\\Desktop\\out2.bin";
        CircularBuffer buffer = new CircularBuffer(0x100);
        CircularBuffer bitBuffer = new CircularBuffer(8);

        using (FileStream compStream = new FileStream(compressedFilePath, FileMode.Open))
        using (FileStream decompStream = new FileStream(decompressedFilePath, FileMode.Create))
        {
            //first filling
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
            } while (compStream.Position < compStream.Length || bitPosition != 8);
        }
    }
}