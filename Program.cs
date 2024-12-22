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

var buffer = new byte[MySerializer.ByteSize];
var lineCount = 0;

foreach(var item in list)
{
    MySerializer.Serialize(item, lineCount, buffer);
    writer.Write(buffer, 0, buffer.Length);
    lineCount++;
}

Console.WriteLine("converted");
writer.Dispose();

using var reader = new FileStream("binary.bin", FileMode.Open, FileAccess.Read);
using var writer2 = new StreamWriter("restored.csv");
writer2.WriteLine("Year,Month,Day,Hour,Temperature,Humidity");
int bytesToRead;
var bufferInfo = new Information();
// var infos = new List<Information>();
lineCount = 0;

do
{
    bytesToRead = reader.Read(buffer, 0, buffer.Length);

    if (bytesToRead >= buffer.Length)
    {
        try
        {
            MySerializer.Deserialize(buffer, lineCount, bufferInfo);
            writer2.WriteLine(ToCsvTextLine(bufferInfo));

            lineCount++;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }

} while(bytesToRead >= buffer.Length);


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

public class Information
{
    public int Year { get; set; }

    public int Month { get; set; }

    public int Day { get; set; }

    public int Hour { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }
}

