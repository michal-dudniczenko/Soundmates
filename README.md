# Soundmates

### Back end API usage:

1. cd into .../api/Soundmates/

2. **start:** _docker compose up --build -d_

3. **stop:** _docker compose down_

---

api url: **http://localhost:5000**

swagger available at: **http://localhost:5000/swagger**

openapi available at: **http://localhost:5000/openapi/soundmates.json**

adminer database management at: **http://localhost:8080**

adminer login data: (**case-sensitive**)
- System: PostgreSQL
- Server: db
- Username: Soundmates
- Password: Soundmates
- Database: Soundmates

## API tutorial

#### TO DO:

-   potwierdzanie maila przy rejestracji, reset hasla
-   live eventy do chatu/powiadomien o nowych matchach

#### Ogólny flow:

1. Rejestrujemy sie, logujemy, dostajemy tokeny, access na 30 minut i refresh na 30 dni.
2. Po rejestracji profil jest pusty, musimy go wypelnic danymi (PUT) i dopiero wtedy bedziemy mogli korzystac z pozostalych funkcjonalnosci, dopoki tego nie zrobimy profil jest 'nieaktywny', mozna tylko sie logowac/wylogowac i deaktywowac konto.
3. Praktycznie wszystkie endpointy dzialaja w oparciu o access token, on okresla to co dany user moze zrobic a czego nie, np. nie moze wyslac wiadomosci do kogos z kim nie ma matcha, nie moze usunac czyjegos zdjecia/nutki.
4. Wszystkie endpointy typu aktualizacja profilu, deaktywacja, zmiana hasla itp beda dotyczyly tego usera ktory jest zakodowany w przeslanym access tokenie.
5. Mechanizm matchowania dziala tak, ze GET /matching/artists oraz GET /matching/bands zwraca uzytkownikow, ktorzy moga byc potencjalnym matchem, czyli tacy, ktorym nie dalismy wczesniej like/dislike. Przegladamy te profile i dajemy like/dislike i jak ktos tez dal nam like to pojawia sie match. Jak mamy matcha to mozemy napisac do tej osoby.

### Walidacje:

Wszystkie ograniczenia/stale zdefiniowane sa w pliku dostepnym [tutaj](./api/Soundmates/src/Soundmates.Domain/Constants/AppConstants.cs).

Dodatkowo dla hasła wymagane sa:
- mala litera,
- duza litera,
- cyfra,
- znak specjalny,
- tylko standardowe drukowalne znaki ascii
