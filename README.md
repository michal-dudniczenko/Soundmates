# Soundmates

### Back end API usage:

1. cd into .../api/Soundmates/

2. **start:** _docker compose up --build -d_

3. **stop:** _docker compose down_

---

api url: **http://localhost:5000**

openapi available at: **http://localhost:5000/openapi/soundmates.json**

adminer database management at: **http://localhost:8080**

### API tutorial

TO DO:

-   obecnie chat idzie przez http wiec mozna wysylac wiadomosci, ale odswiezanie skrzynki tylko przez jakis long polling, docelowo przez websockety
-   obecnie nie trzeba potwierdzac emaila, email jest z automatu 'potwierdzony', dodam to jako krok niezbedny zeby sie zalogowac
-   dodac jakis algorytm proponowania matchy, na podstawie odleglosci, wieku, czy online, idk jakis innych skladowych. obecnie zwracane sa w takiej kolejnosci w jakiej sa w bazie.
-   jak juz ogarne te potwierdzenie emaila to dodam jeszcze logowanie przez kodzik na maila i ogolnie 2FA i reset hasla
-   usuwanie matchy
-   jakies eventy przez web sockety zeby lista matchy aktualizowala sie live, jak dalismy komus wczesniej like i to z jego strony pojawi sie match
-   zmienic rok urodzenia w profilu na date urodzenia, zeby prawidlowo pokazywac wiek

#### Ogólny flow:

1. Rejestrujemy sie, logujemy, dostajemy tokeny, access na 30 minut i refresh na 30 dni.
2. Po rejestracji profil jest pusty, musimy go wypelnic danymi (PUT) i dopiero wtedy bedziemy mogli korzystac z pozostalych funkcjonalnosci, dopoki tego nie zrobimy profil jest 'nieaktywny', mozna tylko sie logowac/wylogowac i deaktywowac konto.
3. Praktycznie wszystkie endpointy dzialaja w oparciu o access token, on okresla to co dany user moze zrobic a czego nie, np. nie moze wyslac wiadomosci do kogos z kim nie ma matcha, nie moze usunac czyjegos zdjecia/nutki.
4. Wszystkie endpointy typu aktualizacja profilu, deaktywacja, zmiana hasla itp beda dotyczyly tego usera ktory jest zakodowany w przeslanym access tokenie.
5. Mechanizm matchowania dziala tak, ze GET /users zwraca uzytkownikow, ktorzy moga byc potencjalnym matchem, czyli tacy, ktorym nie dalismy wczesniej like/dislike. Przegladamy te profile i dajemy like/dislike i jak ktos tez dal nam like to pojawia sie match. Jak mamy matcha to mozemy napisac do tej osoby.

### Walidacje (wiadomo wszystko mozna zmieniac, podaje stan obecny):

1. email standardowo + max 100 znakow
2. hasło 8-32 znakow, jedna mala, duza litera, cyfra, znak specjalny, tylko standardowe drukowalne znaki ascii
3. wiadomosc max 4000 znakow
4. profil: data urodzenia miedzy 1900 a dzisiaj, imie max 50 znakow, opis max 500 - opcjonalny, miasto, kraj max 100
5. nutka max 5 mb, tylko .mp3, kazdy user max 5 nutek
6. zdjecie max 5 mb, tylko .jpeg/.jpg, kazdy user max 5 zdjec

/auth
    POST /auth/register - rejestracja
    POST /auth/login - logowanie, zwraca access + refresh token na 30 dni
    POST /auth/refresh - przesylamy refresh token zeby dostac nowy access token (zeby odnowic sesje i nie trzeba bylo sie logowac ponownie, przechowujemy tylko tokeny na kliencie)
    POST /auth/logout - uniewaznia obecne tokeny, trzeba sie zalogowac zeby dostac nowe

/users
    GET /users/profile - zwraca profil usera
    GET /users/{id} - zwraca profil innego uzytkownika, to samo co wyzej, ale bez emaila bo RODO
    GET /users - zwraca profile innych uzytkownikow, ktorzy sa potencjalnymi matchami dla usera
    POST /users/change-password - zmiana hasla usera
    PUT /users - aktualizacja profilu usera
    DELETE /users - deaktywacja konta usera, nie da sie cofnac, chyba ze recznie w db

/matching
    POST /matching/like - daje lajka uzytkownikowi
    POST /matching/dislike - daje dislajka uzytkownikowi
    GET /matching/matches - lista matchy usera

/messages
    GET /messages/{userId} - zwraca konwersacje usera z uzytkownikiem o userId
    GET /messages/preview - zwraca 'preview' konwersacji usera, po jednej ostatniej wiadomosci w kazdej z konwersacji
    POST /messages - wyslanie wiadomosci, mozna wyslac tylko do uzytkownika, z ktorym mamy matcha

/profile-pictures
    GET /profile-pictures - zwraca linki do zdjec usera
    GET /profile-pictures/{userId} - zwraca linki do zdjec uzytkownika o userId
    POST /profile-pictures - dodaje zdjecie
    DEL /profile-pictures/{pictureId} - usuwa zdjecie
    POST /profile-pictures/move-display-order-up/{pictureId} - zmienia kolejnosc wyswietlania zdjecia o pictureId o jedno do przodu
    POST /profile-pictures/move-display-order-down/{pictureId} - zmienia kolejnosc wyswietlania zdjecia o pictureId o jedno do tylu

/music-samples
    GET /music-samples - zwraca linki do nutek usera
    GET /music-samples/{userId} - zwraca linki do nutek uzytkownika o userId
    POST /music-samples - dodaje nutke
    DEL /music-samples/{pictureId} - usuwa nutke
    POST /music-samples/move-display-order-up/{musicSampleId} - zmienia kolejnosc wyswietlania nutki o musicSampleId o jedno do przodu
    POST /music-samples/move-display-order-down/{musicSampleId} - zmienia kolejnosc wyswietlania nutki o musicSampleId o jedno do tylu
