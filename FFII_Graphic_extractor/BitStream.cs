using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BitStream
{
    private protected static byte bitPosition = 0;

    private protected static void bitUpdate(CircularBufferR buffer, byte newByte)
    {
        foreach (var item in Convert.ToString(newByte, 2).PadLeft(8, '0'))
        {
            buffer.addByte(byte.Parse(item.ToString()));
        }
    }
    private protected static void bitUpdate(CircularBufferR buffer, Stream streamBit)
    {
        byte bit = (byte)streamBit.ReadByte();
        foreach (var item in Convert.ToString(bit, 2).PadLeft(8, '0'))
        {
            buffer.addByte(byte.Parse(item.ToString()));
        }
    }

    private protected static byte readBit(CircularBufferR buffer, FileStream stream)
    {
        if (bitPosition == buffer.size)
        {
            bitPosition = 0;
            bitUpdate(buffer, stream);
        }
        bitPosition++;
        return buffer.getByte((byte)(bitPosition - 1));
    }

    private protected static byte getNextByte(CircularBufferR buffer, FileStream stream) {
        byte[] byteBuilder = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            byteBuilder[i] = readBit(buffer, stream);
        }
        return buildByte(byteBuilder);
    }

    private protected static byte buildByte(byte[] bits)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var item in bits)
        {
            sb.Append(item.ToString());
        }
        return Convert.ToByte(sb.ToString(), 2);
    }
}

