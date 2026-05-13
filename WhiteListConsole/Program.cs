using System.Net.Http.Json;

using var client = new HttpClient();

while (true)
{
    Console.Clear();
    Console.WriteLine("WYKAZ PODATNIKÓW");

    Console.Write("Podaj NIP: ");
    string nip = (Console.ReadLine() ?? "").Replace(" ", "").Replace("-", "");

    Console.Write("Podaj numer konta: ");
    string konto = (Console.ReadLine() ?? "").Replace(" ", "").Replace("-", "");

    if (nip.Length != 10 || konto.Length != 26)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"BŁĄD: NIP ma {nip.Length}/10 znaków, a konto {konto.Length}/26.");
    }
    else
    {
        try
        {
            string url = $"https://localhost:7043/sprawdz?nip={nip}&konto={konto}";
            var odpowiedz = await client.GetFromJsonAsync<WynikApi>(url);

            if (odpowiedz != null && odpowiedz.Znaleziono)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSUKCES! Status: {odpowiedz.Status}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nNIE ZNALEZIONO!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nBłąd połączenia: {ex.Message}");
        }
    }

    Console.ResetColor();
    Console.WriteLine("\nNaciśnij 'R', aby sprawdzić inny numer, lub dowolny inny klawisz, aby wyjść...");

    var klawisz = Console.ReadKey(true).Key;
    if (klawisz != ConsoleKey.R) break;
}
public record WynikApi(bool Znaleziono, string Status, string Wiadomosc);