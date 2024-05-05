using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace CheckiAutoUpdater;

class Program
{
    static void Main(string[] args)
    {
        Dictionary<string, string>? currentVer = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("CAUconfig.json"));
        using var client = new WebClient();
        
        var checkNewVer = client.DownloadString(currentVer["urlCheck"]);

        byte[] readResponse = Encoding.UTF8.GetBytes(checkNewVer);
        Dictionary<string, string>? newVer = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(readResponse));

        if (currentVer["appVersion"] != newVer["appVersion"])
        {
            Console.Write($"[+] Найдены обновления:\n\n|\n|\t{currentVer["appVersion"]} -> {newVer["appVersion"]}\n|\n\nОбновить? [y/n]: ");
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Y || key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine($"\n\n[...] Скачивание с {currentVer["urlDownload"]}");
                client.DownloadFile(currentVer["urlDownload"], currentVer["downloadPath"]);
                Console.WriteLine($"[...] Установка. Путь: {currentVer["downloadPath"]}");
                currentVer["appVersion"] = newVer["appVersion"];
                var jsonData = JsonConvert.SerializeObject(currentVer, Formatting.Indented);
                File.WriteAllText("CAUconfig.json", jsonData);
                Console.WriteLine("[+] Обновление завершено!");
            }
            else if (key.Key == ConsoleKey.N)
            {
                Console.WriteLine("\n\n[-] Отмена");
            }
        }
        else if (currentVer["appVersion"] == newVer["appVersion"])
        {
            Console.WriteLine("[-] Обновления не найдены");
        }
    }
}