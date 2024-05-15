using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CircularBuffer
{
    public static byte size;
    public byte position;
    private byte[] buffer;
    public CircularBuffer()
    {
        size = 0;
        position = 0;
    }
    public CircularBuffer(byte bufferSize)
    {
        size = bufferSize;
        buffer = new byte[size];
        position = 0;
    }
    private void positionUpdate()
    {
        if (position == size)
        {
            position = 0;
        }
        else
        {
            position += 1;
        }
    }
    public void addByte(byte data)
    {
        buffer[position] = data;
        positionUpdate();
    }
    public void addSetBytes(byte[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            buffer[position] = data[i];
            positionUpdate();
        }
    }
    public byte getByte(byte index)
    {
        return buffer[index];
    }
    public byte[] getSetBytes(byte offset, byte length)
    {
        ArraySegment<byte> buffer = new ArraySegment<byte>(this.buffer, offset, length);
        return buffer.ToArray();
    }
}

