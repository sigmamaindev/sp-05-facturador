using System.Text;

namespace Infrastructure.Services;

public static class SriLogHelper
{
    public static string CurrentSessionFolder { get; set; } = "";
    public static string CreateSessionFolder()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "logs", "sri");
        Directory.CreateDirectory(root);

        var folder = Path.Combine(root, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        Directory.CreateDirectory(folder);

        return folder;
    }

    public static void Write(string folder, string fileName, string content)
    {
        try
        {
            var path = Path.Combine(folder, fileName);

            File.WriteAllText(path, content, Encoding.UTF8);

            Console.WriteLine($"📝 Archivo generado: {path}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("⛔ Error guardando archivo del SRI:");
            Console.WriteLine(ex);
        }
    }

    public static void Append(string folder, string fileName, string content)
    {
        try
        {
            var path = Path.Combine(folder, fileName);
            File.AppendAllText(path, content + Environment.NewLine, Encoding.UTF8);
            Console.WriteLine($"📝 Log actualizado: {path}");
        }
        catch { }
    }
}
