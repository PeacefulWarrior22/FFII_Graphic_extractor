using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CircularBufferR
{
    public int size;
    public byte position;
    private byte[] buffer;

    public CircularBufferR(int bufferSize)
    {
        size = bufferSize;
        buffer = new byte[size];
        position = 0;
    }
    public CircularBufferR(int bufferSize, byte[] arr)
    {
        size = bufferSize;
        buffer = new byte[size];
        position = 0;

        foreach (var item in arr)
        {
            buffer[position] = item;
            position++;
        }
    }
    private void positionUpdate()
    {
        if (position == size - 1)
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
        foreach (byte b in data)
        {
            addByte(b);
        }
    }

    public void byteRefresh(byte offset, byte length)
    {
        for (int i = 0; i < length; i++)
        {
            buffer[position] = buffer[(offset - 1 + i) % size];
            //почему % ломает данные??/
            //buffer[position] = buffer[(offset - 1 + i) % size - 1];
            positionUpdate();
        }
    }
    public byte getByte(int index)
    {
        return buffer[index];
    }
    public byte[] getSetBytes(byte offset, byte length)
    {

        byte[] buffer = new byte[length];
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = this.buffer[(offset + i) % (size - 1)];
        }

        return buffer;
    }
}

