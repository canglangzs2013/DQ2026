using System;
using System.IO;
using System.Xml.Linq;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0) { Console.WriteLine("用法: ValidateResxBase64 <path to .resx>"); return 1; }
        var path = args[0];
        if (!File.Exists(path)) { Console.WriteLine("文件不存在: " + path); return 2; }
        var doc = XDocument.Load(path);
        foreach (var data in doc.Descendants("data"))
        {
            var mime = (string)data.Attribute("mimetype");
            if (mime != null && mime.IndexOf("base64", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var name = (string)data.Attribute("name") ?? "<unnamed>";
                var valueElem = data.Element("value");
                if (valueElem == null) { Console.WriteLine($"{name}: missing <value>"); continue; }
                var raw = valueElem.Value;
                var cleaned = raw.Replace("\r","").Replace("\n","").Replace(" ","");
                try
                {
                    var bytes = Convert.FromBase64String(cleaned);
                    Console.WriteLine($"{name}: OK, {bytes.Length} bytes");
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"{name}: INVALID BASE64 -> {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{name}: ERROR -> {ex.Message}");
                }
            }
        }
        return 0;
    }
}