
using u1 = System.Byte;
using u2 = System.UInt16;
using f4 = System.Single;
using System.Runtime.InteropServices;

public class MySerializer
{
    public static readonly int ByteSize = (int)SerialID.ByteSize;

    public static void Serialize(Information info, int lineCount, byte[] output)
    {
        if ((int)SerialID.ByteSize > output.Length) return;

        output[(int)SerialID.LineCount] = (u1)(lineCount & 0xFF);
        output[(int)SerialID.Year_H] = (u1)(((u2)info.Year) >> 8);
        output[(int)SerialID.Year_L] = (u1)(((u2)info.Year) & 0xFF);
        output[(int)SerialID.Month] = (u1)info.Month;
        output[(int)SerialID.Day] = (u1)info.Day;
        output[(int)SerialID.Hour] = (u1)info.Hour;
        var s4_temperature = BitConverter.SingleToInt32Bits((f4)info.Temperature);
        output[(int)SerialID.Temp_H] = (u1)(s4_temperature >> 24);
        output[(int)SerialID.Temp_MH] = (u1)((s4_temperature >> 16) & 0xFF);
        output[(int)SerialID.Temp_ML] = (u1)((s4_temperature >> 8) & 0xFF);
        output[(int)SerialID.Temp_L] = (u1)(s4_temperature & 0xFF);
        output[(int)SerialID.Humid] = (u1)info.Humidity;
        output[(int)SerialID.CheckSum] = (u1)(output.Take((int)SerialID.CheckSum).Sum(b => b) & 0xFF);
    }

    public static void Deserialize(byte[] input, int lineCount, Information info)
    {
        if ((int)SerialID.ByteSize > input.Length) throw new IndexOutOfRangeException("insuffic buffer size");

        if (input[(int)SerialID.LineCount] != (lineCount & 0xFF)) throw new ArgumentException($"No. {lineCount} is mismatched.");

        if (input[(int)SerialID.CheckSum] != (input.Take((int)SerialID.CheckSum).Sum(b => b) & 0xFF)) throw new InvalidDataException("Check sum is mismatched.");

        info.Year = (input[(int)SerialID.Year_H] << 8) | input[(int)SerialID.Year_L];
        info.Month = input[(int)SerialID.Month];
        info.Day = input[(int)SerialID.Day];
        info.Hour = input[(int)SerialID.Hour];
        info.Temperature = BitConverter.Int32BitsToSingle((input[(int)SerialID.Temp_H] << 24) | (input[(int)SerialID.Temp_MH] << 16) | (input[(int)SerialID.Temp_ML] << 8) | input[(int)SerialID.Temp_L]);
        info.Humidity = input[(int)SerialID.Humid];
    }


    private enum SerialID
    {
        LineCount,
        Year_H,
        Year_L,
        Month,
        Day,
        Hour,
        Temp_H,
        Temp_MH,
        Temp_ML,
        Temp_L,
        Humid,
        CheckSum,
        ByteSize
    }

}



