using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var connString = builder.Configuration.GetConnectionString("WhiteListDb");
string data = "20260511";

app.MapGet("/sprawdz", async (string nip, string konto) =>
{
    var n = nip.Replace(" ", "").Replace("-", "");
    var k = konto.Replace(" ", "").Replace("-", "");

    if (n.Length != 10 || k.Length != 26)
        return Results.BadRequest(new { Znaleziono = false, Wiadomosc = "Błędna długość danych" });

    using var conn = new SqlConnection(connString);
    await conn.OpenAsync();

    object? wynik = await Szukajhash(data + n + k, conn);

    if (wynik == null)
    {
        var maski = new List<string>();
        var cmd = new SqlCommand("SELECT Wzorzec FROM Maski WHERE BankID = @b", conn);
        cmd.Parameters.AddWithValue("@b", k.Substring(2, 8));

        using (var reader = await cmd.ExecuteReaderAsync())
            while (await reader.ReadAsync()) maski.Add(reader.GetString(0));

        foreach (var m in maski)
        {
            var zmaskowane = ZastosujMaske(k, m);
            wynik = await Szukajhash(data + n + zmaskowane, conn);
            if (wynik != null) break;
        }
    }

    return Results.Ok(new
    {
        Znaleziono = wynik != null,
        Status = wynik != null ? ((bool)wynik ? "Czynny" : "Zwolniony") : null
    });

    async Task<object?> Szukajhash(string tekst, SqlConnection c)
    {
        var h = Hashowanie(tekst);
        return await new SqlCommand("SELECT Status FROM Podatnicy WHERE hash = @h", c)
        { Parameters = { new("@h", h) } }.ExecuteScalarAsync();
    }
});

string Hashowanie(string t)
{
    string h = Convert.ToHexString(SHA512.HashData(Encoding.UTF8.GetBytes(t))).ToLower();
    for (int i = 1; i < 5000; i++)
        h = Convert.ToHexString(SHA512.HashData(Encoding.UTF8.GetBytes(h))).ToLower();
    return h;
}

string ZastosujMaske(string nrb, string m)
{
    char[] w = new char[26];
    for (int i = 0; i < 26; i++)
        w[i] = m[i] == 'Y' ? nrb[i] : (m[i] == 'X' ? 'X' : m[i]);
    return new string(w);
}

app.Run();