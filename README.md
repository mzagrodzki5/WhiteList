# WhiteList
System do weryfikacji statusu podatnika VAT na podstawie publicznych plików płaskich udostępnianych przez Ministerstwo Finansów

#Tech Stack
C# (.NET 10)
API: ASP.NET Core
Baza danych: MS SQL Server (LocalDB)

#Architektura:
WhiteList (API): Serwis, który przyjmuje zapytania, przetwarza dane (SHA-512), obsługuje maskowanie kont wirtualnych i komunikuje się z bazą danych
WhiteListConsole (Client): Aplikacja konsolowa służąca do interakcji z użytkownikiem i testowania endpointów API

#Start
W pliku appsettings.json zaktualizuj ConnectionStrings
Uruchom projekt WhiteList (API), a następnie WhiteListConsole.
