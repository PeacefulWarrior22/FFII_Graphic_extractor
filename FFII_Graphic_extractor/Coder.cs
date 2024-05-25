using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Coder : BitStream
{
    private static byte[] graphicHead = new byte[] { 0x10, 0x00, 0x00, 0x00, 0x05, 0x00 };

    private static object slidingWindow(CircularBufferR dictionary, CircularBuffer buffer)
    {
        int[] resault = new int[2];
        int offset;
        int length = 0;
        for (int i = dictionary.position; i != 0; i--)
        {
            if (dictionary.getByte(i - 1) == buffer.getByte())
            {
                offset = i - 1;
                length++;
                for (int j = 1; j < 10; j++)
                {
                    if (dictionary.getSetBytes((byte)offset, (byte)(length + 1)).SequenceEqual(buffer.getSetBytes((byte)(length + 1))))
                    {
                        length++;
                    }
                    else
                    {
                        if (resault[1] < length) resault = new int[] { offset, length };
                        length = 0;
                        break;
                    }
                }
                if (resault[1] < length) resault[1] = length;
                if (resault[1] == 10) break;
            }
        }
        if (resault[1] <= 2)
        {
            return false;
        }
        else
        {
            return resault;
        }
    }
    private protected static void decodeGraphic(string gamePath, string decompressedFilePath)
    {
        using (FileStream compStream = new FileStream(gamePath, FileMode.Open))
        {
            byte[] header = new byte[6];
            compStream.Read(header, 0, 6);

            do
            {
                if (!header.SequenceEqual(graphicHead))
                {
                    for (int i = 0; i < header.Length - 1; i++)
                    {
                        header[i] = header[i + 1];
                    }
                    header[5] = (byte)compStream.ReadByte();
                }
                else
                {
                    using (FileStream decompStream = new FileStream($"{decompressedFilePath}_" +
                        $"{Convert.ToString(compStream.Position - header.Length, 16).ToUpper()}", FileMode.Create))
                    {
                        CircularBufferR buffer = new CircularBufferR(0x100);
                        CircularBufferR bitBuffer = new CircularBufferR(8);

                        byte[] outSize = new byte[] { (byte)compStream.ReadByte(), (byte)compStream.ReadByte() };
                        int decodedGraphicSize = Convert.ToInt32($"{Convert.ToString(outSize[1], 16).PadLeft(2, '0')}" +
                            $"{Convert.ToString(outSize[0], 16).PadLeft(2, '0')}", 16);

                        bitUpdate(bitBuffer, compStream);
                        byte[] byteBuilder = new byte[8];

                        do
                        {
                            //flag checking
                            switch (readBit(bitBuffer, compStream))
                            {
                                case 1:
                                    buffer.addByte(getNextByte(bitBuffer, compStream));
                                    decompStream.WriteByte(buffer.getByte((byte)(buffer.position - 1)));
                                    break;
                                case 0:
                                    byte offset = getNextByte(bitBuffer, compStream);
                                    byte length = (byte)(buildByte(new byte[]{
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
                    compStream.Read(header, 0, 6);
                }
                bitPosition = 0;
            } while (compStream.Position < compStream.Length);
        }
    }
    private protected static string codeGraphic(string decompressedFilePath, string compressedFilePath)
    {
        StringBuilder compressedFileBits = new StringBuilder();
        using (FileStream decompStream = new FileStream(decompressedFilePath, FileMode.Open))
        {
            using (FileStream compStream = new FileStream(compressedFilePath, FileMode.Create))
            {
                byte[] reserv = new byte[10];
                decompStream.Read(reserv, 0, 10);

                //в словаре будут находится данные что уже прошли проверку
                //именно отсюда будут браться данные для вставки в файл
                //при условии что нашлось совпадение больше 2х байт
                CircularBufferR dictionary = new CircularBufferR(0x100);
                //буфер куда будут помещаться данные для кодировки, т.к 
                //максимальный размер длины 10 то он равен 10
                //первые 10 байт присвоены сразу 
                CircularBuffer buffer = new CircularBuffer(10, reserv);

                //сразу добавляю в файл первый байт т.к он всегда будет с флагом 1
                compressedFileBits.Append('1');
                foreach (var item in Convert.ToString(buffer.getByte(), 2).PadLeft(8, '0'))
                {
                    compressedFileBits.Append(item);
                }

                dictionary.addByte(buffer.getByte());
                buffer.addByte((byte)decompStream.ReadByte());

                do
                {
                    if (compressedFileBits.Length > 1000) {
                        Console.WriteLine();
                    }
                    switch (slidingWindow(dictionary, buffer))
                    {
                        case bool b:
                            compressedFileBits.Append('1');
                            foreach (var item in Convert.ToString(buffer.getByte(), 2).PadLeft(8, '0'))
                            {
                                compressedFileBits.Append(item);
                            }
                            dictionary.addByte(buffer.getByte());
                            buffer.addByte((byte)decompStream.ReadByte());
                            break;
                        case int[] j:
                            compressedFileBits.Append('0');
                            foreach (var item in Convert.ToString(j[0] + 1, 2).PadLeft(8, '0'))
                            {
                                compressedFileBits.Append(item);
                            }
                            foreach (var item in Convert.ToString(j[1] - 3, 2).PadLeft(3, '0'))
                            {
                                compressedFileBits.Append(item);
                            }
                            dictionary.addSetBytes(buffer.getSetBytes((byte)j[1]));
                            buffer.addSetFromStream(decompStream, j[1]);
                            break;
                    }

                } while (decompStream.Position < decompStream.Length);
            }
        }
        return compressedFileBits.ToString();
    }
}
