using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CircularBuffer
{
    public int size;
    public byte position;
    private byte[] buffer;

    public CircularBuffer(int bufferSize, byte[] buffer)
    {
        size = bufferSize;
        this.buffer = buffer;
        position = (byte)(bufferSize - 1);
    }
    public void addByte(byte data)
    {
        for (int i = 0; i < size - 1; i++)
        {
            buffer[i] = buffer[i + 1];
        }
        buffer[position] = data;
    }

    public void addSetFromStream(FileStream stream, int length)
    {
        byte[] reserv = new byte[length];
        stream.Read(reserv, 0, length);
        byteRefresh(reserv);
    }
    public void byteRefresh(byte[] array)
    {
        foreach (byte b in array) {
            addByte(b);
        }
    }
    public byte getByte()
    {
        return buffer[0];
    }
    public byte[] getSetBytes(byte length)
    {
        byte[] buffer = new byte[length];
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = this.buffer[i];
        }
        return buffer;
    }
}