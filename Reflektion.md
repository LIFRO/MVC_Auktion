## Struktur
Domänreglerna ligger nära modellen och kontrollerna: `AuctionModel` har dataannotations för ID-format (AAA000000), prisintervall och obligatoriska fält. `AuctionsController` hanterar affärslogik som startpris ≥ inköpskostnad, slutpris ≥ startpris och unik InventoryId, samt sätter `Seller` från inloggad användare. EF Core InMemory används via `ApplicationDbContext` för enkel körning utan migrations, och vyerna binder till strongly-typed modeller med kategorilistor från `PopulateCategoriesAsync`. Kategorier håller jag i egen tabell för att möta kravet på fria/ändringsbara kategorier.

## Säkerhet
Alla auktionssidor är skyddade med `[Authorize]`, och känsliga åtgärder (Sell, SoldEdit) har `[Authorize(Roles = "IsAResponsible")]`. `Index` filtrerar på `Seller` för vanliga användare så de bara ser sina egna objekt, medan ansvariga ser alla. Validering sker både via dataannotations och ModelState i kontrollerna, vilket stoppar felaktiga ID:n och prisrelationer innan sparning. Slutpris och såld-status kan bara sättas av ansvarig eftersom både routes och actions är rollskyddade.

## Användare
Två roller används: `User` (vanliga inköpare) och `IsAResponsible` (auktionsansvariga). Registreringsflödet har en checkbox som via `Register.cshtml.cs` lägger användaren i rätt roll med `AddToRoleAsync`, och grundrollen `User` sätts för dem som inte väljer ansvarig. Vanliga användare kan skapa och redigera sina egna objekt; auktionsansvariga kan dessutom se alla objekt och sätta slutpris/markera sålda i Sell/SoldEdit.

## Övrigt
En förbättring vore att ersätta checkboxen med en rollväljare (dropdown) så fler roller kan läggas till utan kodändring, och att begränsa valet till behöriga administratörer.
