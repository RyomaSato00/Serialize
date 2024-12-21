using MessagePack;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("data.csv");

var list = new List<Information>();

string[] lineColumns;

foreach(var line in lines)
{
    lineColumns = line.Split(',');

    var date = lineColumns[0];
    var dateColumns = date.Split(' ');
    var dateColumns2 = dateColumns[0].Split('/');
    var dateColumns3 = dateColumns[1].Split(':');

    var info = new Information
    {
        Year = int.Parse(dateColumns2[0]),
        Month = int.Parse(dateColumns2[1]),
        Day = int.Parse(dateColumns2[2]),
        Hour = int.Parse(dateColumns3[0]),
        Temperature = double.Parse(lineColumns[1]),
        Humidity = double.Parse(lineColumns[2])
    };

    // Console.WriteLine($"{info.Year}, {info.Month}, {info.Day}, {info.Hour}, {info.Temperature}, {info.Humidity}");

    list.Add(info);
}

var writer = new FileStream("binary.bin", FileMode.Create, FileAccess.ReadWrite);

var buffer = new byte[10];

foreach(var item in list)
{
    ToBinaryData(item, buffer);
    writer.Write(buffer, 0, buffer.Length);
}

Console.WriteLine("converted");
writer.Dispose();

using var reader = new FileStream("binary.bin", FileMode.Open, FileAccess.Read);
int bytesToRead;
var infos = new List<Information>();

do
{
    bytesToRead = reader.Read(buffer, 0, buffer.Length);

    if (bytesToRead >= buffer.Length)
    {
        var bufferInfo = new Information();

        ToInformation(buffer, bufferInfo);

        infos.Add(bufferInfo);
    }

} while(bytesToRead >= buffer.Length);

var output = infos.Select(info => ToCsvTextLine(info)).Prepend("Year,Month,Day,Hour,Temperature,Humidity");
File.WriteAllLines("restored.csv", output);

Console.WriteLine("restored");

// var binary = MessagePackSerializer.Serialize(list);

// File.WriteAllBytes("output.bin", binary);

// var infoSize = sizeof(int) * 4 + sizeof(double) * 2;
// Console.WriteLine($"info size:{infoSize}");

// var reBinary = File.ReadAllBytes("output.bin");

// var reGain = MessagePackSerializer.Deserialize<List<Information>>(reBinary);

// using var writer = new StreamWriter("reGain.txt");

// foreach(var item in reGain)
// {
//     writer.WriteLine($"{item.Year},{item.Month},{item.Day},{item.Hour},{item.Temperature},{item.Humidity}");
// }

// BitConverter.GetBytes()
// var infoSize = 25;

// using var writer = new StreamWriter("output.bin");
// using var writer = new FileStream("binary.bin", FileMode.Create, FileAccess.ReadWrite);

// foreach(var item in list)
// {
//     try
//     {
//         // Console.WriteLine($"{item.Year}, {item.Month}, {item.Day}, {item.Hour}, {item.Temperature}, {item.Humidity}");

//         var binary = MessagePackSerializer.Serialize(item);

//         // Console.WriteLine($"size: {binary.Length}");

//         writer.Write(binary, 0, infoSize);
//     }
//     catch(Exception)
//     {

//     }
// }

// var text = File.ReadAllText("input.txt");

// var binary = MessagePackSerializer.Serialize(text);

// File.WriteAllBytes("output.bin", binary);

static string ToCsvTextLine(Information info)
{
    string text = string.Empty;

    text = $"{info.Year},{info.Month},{info.Day},{info.Hour},{info.Temperature:.#},{info.Humidity}";

    return text;
}


static void ToBinaryData(Information info, byte[] output)
{
    if (10 > output.Length) return;

    var u2_year = (ushort)info.Year;
    var u1_month = (byte)info.Month;
    var u1_day = (byte)info.Day;
    var u1_hour = (byte)info.Hour;
    var u4_temperature = BitConverter.SingleToInt32Bits((float)info.Temperature);
    var u1_humidity = (byte)info.Humidity;

    output[0] = (byte)(u2_year >> 8);
    output[1] = (byte)(u2_year & 0xFF);
    output[2] = u1_month;
    output[3] = u1_day;
    output[4] = u1_hour;
    output[5] = (byte)(u4_temperature >> 24);
    output[6] = (byte)((u4_temperature >> 16) & 0xFF);
    output[7] = (byte)((u4_temperature >> 8) & 0xFF);
    output[8] = (byte)(u4_temperature & 0xFF);
    output[9] = u1_humidity;
}

static void ToInformation(byte[] input, Information info)
{
    if (10 > input.Length) return;

    info.Year = (input[0] << 8) | input[1];
    info.Month = input[2];
    info.Day = input[3];
    info.Hour = input[4];
    info.Temperature = BitConverter.Int32BitsToSingle((input[5] << 24) | (input[6] << 16) | (input[7] << 8) | input[8]);
    info.Humidity = input[9];
}

public class Information
{
    public int Year { get; set; }

    public int Month { get; set; }

    public int Day { get; set; }

    public int Hour { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }
}

