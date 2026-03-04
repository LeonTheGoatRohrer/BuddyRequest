# Wochenbericht - BuddyRequest Messenger App

## Projektübernahme von Felix

**Stand Felix, wo ich übernommen habe:**

**ORM**
- User-Model mit Basis-Properties
- DbManager-Klasse erstellt
- MySQL-Connection-String konfiguriert
- Erste Migration angelegt

**WebAPI**
- UsersController mit Register-Endpoint (POST)
- Login-Endpoint (POST)
- Basis-Passwort-Handling
- Swagger läuft

**MAUI**
- RegistrationPage mit einfachen Entry-Feldern
- RegistrationViewModel mit Properties und Basic-Binding
- ApiService für HTTP-POST
- Einfache Validierung (Pflichtfelder prüfen)
- AppShell-Navigation eingerichtet

---

## Woche [5]: 17.12.2025

### ✅ ORM
- ✅ Friends-Model um CreatedAt-Property erweitert (DateTime)
- ✅ DbManager: 7 neue Methoden für Freunde-Verwaltung implementiert:
  - SucheBenutzerAsync() - User nach Username/Key suchen
  - SendeFriendRequestAsync() - Freundschaftsanfrage senden
  - GetPendingRequestsAsync() - Offene Anfragen abrufen
  - GetFreundeAsync() - Freundesliste laden
  - AcceptRequestAsync() - Anfrage akzeptieren
  - DeclineRequestAsync() - Anfrage ablehnen
- ✅ Migration AddCreatedAtToFriends erstellt und auf Datenbank angewendet

### ✅ WebAPI
- ✅ FriendsController mit 6 REST-Endpoints erstellt:
  - GET /api/friends/search?query=... - User suchen
  - POST /api/friends/request - Anfrage senden
  - GET /api/friends/pending/{userId} - Offene Anfragen
  - GET /api/friends/list/{userId} - Freundesliste
  - PUT /api/friends/accept/{requestId} - Anfrage akzeptieren
  - DELETE /api/friends/decline/{requestId} - Anfrage ablehnen
- ✅ FriendRequestDto Klasse für API-Request erstellt
- ✅ Backend mit Swagger getestet - funktioniert
- ✅ Datenbank-Test durchgeführt: Anfrage wurde erfolgreich gespeichert (ID 1, User 1 → User 2)

### ✅ MAUI
- ✅ apiService um 6 Freunde-Methoden erweitert
- ✅ Bessere Exception-Handling in SendFriendRequestAsync() implementiert
- ✅ FriendsPage.xaml erstellt mit:
  - Suchfeld für User-Suche
  - CollectionView für Suchergebnisse
  - "Anfrage senden"-Button
  - Freundesliste-Bereich
  - Offene-Anfragen-Bereich
- ✅ FriendsViewModel mit 4 Commands implementiert:
  - SearchCommand - funktioniert
  - SendRequestCommand - Problem: Fehlermeldung trotz Erfolg
  - LoadFriendsCommand - implementiert
  - LoadPendingRequestsCommand - implementiert
- ✅ FriendsPage.xaml.cs Code-Behind erstellt
- ✅ FriendsPage in AppShell-Navigation eingebunden
- ✅ Debug-Logging hinzugefügt für bessere Fehleranalyse

### Probleme & offene Punkte
- ❌ Hauptproblem: MAUI-Frontend zeigt "Anfrage konnte nicht gesendet werden", obwohl Backend die Anfrage erfolgreich speichert
- ❌ Offene Anfragen-Liste: UI fehlt noch (Accept/Decline-Buttons)
- ❌ Session-Management: Aktuell wird User-ID hardcoded, sollte aus Login übernommen werden
- ❌ Freundesliste: Anzeige funktioniert, aber noch nicht getestet

---

## Woche [6]: 19.12.2025

### ✅ ORM
- ✅ User-Model um AvatarUrl, Bio und CreatedAt erweitert
- ✅ Migration AddAvatarAndBioToUser erstellt und auf Datenbank angewendet
- ✅ SQL-Update: AvatarUrl für alle User automatisch gesetzt (DiceBear-URL)
- ✅ FriendRequestWithUser-Model für Anfragen mit Userdaten erstellt
- ✅ DbManager: Methode GetPendingRequestsWithUserAsync implementiert (liefert offene Anfragen inkl. Senderdaten)

### ✅ WebAPI
- ✅ UsersController: GET /api/users/all um UserId erweitert (zeigt nur Nicht-Freunde)
- ✅ FriendsController: GET /api/friends/pending/{userId} liefert jetzt Anfragen mit Userdaten
- ✅ Endpoints für Accept/Decline (PUT/DELETE) getestet
- ✅ Backend mit Swagger getestet – funktioniert
- ✅ Datenbank-Test: Avatar-URLs und Anfragen korrekt gespeichert

### ✅ MAUI
- ✅ UserSession eingeführt: User-ID wird nach Login gespeichert und überall verwendet
- ✅ apiService um Methoden für Avatar, Anfragen, Accept/Decline erweitert
- ✅ FriendsViewModel:
  - PendingRequests als ObservableCollection hinzugefügt
  - AcceptRequestCommand und DeclineRequestCommand implementiert
  - LoadPendingRequestsAsync zeigt offene Anfragen mit Senderdaten
- ✅ FriendsPage.xaml:
  - Offene Anfragen werden als Liste angezeigt
  - Avatar, Username, Key, Accept- und Decline-Button pro Anfrage
  - UI für Freundesliste und Suchergebnisse verbessert
- ✅ RegistrationPage und LoginPage: Avatar-Vorschau integriert
- ✅ Debug-Logging für Fehleranalyse verbessert

### Probleme & offene Punkte
- ❌ Avatare werden in MAUI nicht immer angezeigt (URL in DB jetzt korrekt, UI muss ggf. neu geladen werden)
- ❌ Freundesliste: Anzeige funktioniert, aber noch nicht vollständig getestet
- ❌ Nachrichten-System: Noch nicht implementiert

---

## Woche [9]: 21.01.2026

### ✅ ORM
- ✅ Messages-Model erstellt (Id, SenderId, EmpfaengerId, Message, SentAt)
- ✅ Migration für Messages-Tabelle angelegt und angewendet

### ✅ WebAPI
- ✅ MessagesController mit Endpoints für Nachricht senden und Chatverlauf abrufen
- ✅ API mit Swagger getestet – Nachrichten werden korrekt gespeichert und abgerufen

### ✅ MAUI
- ✅ ChatPage und ChatViewModel erstellt
- ✅ Nachrichten werden beim Senden in der Datenbank gespeichert
- ✅ Nachrichten werden beim Öffnen des Chats aus der Datenbank geladen
- ✅ Anzeige der Nachrichten im Chatfenster mit CollectionView (Fehler mit BindableLayout/ScrollView behoben)
- ✅ Senden von Nachrichten und sofortige Anzeige im Chatfenster funktioniert
- ✅ Debug-Logging für Nachrichten-Ladevorgang integriert

### Probleme & offene Punkte
- ❌ Nachrichten-Layout: Eigene und empfangene Nachrichten werden noch nicht unterschiedlich ausgerichtet
- ❌ Automatisches Scrollen zum neuesten Eintrag fehlt noch
- ❌ UI-Optimierung für Chat (z.B. Farben, Abstände) noch offen

---

## Woche [10]: 28.01.2026

### ✅ Debugging & Fehlerbehebung
- ✅ HTTPS-Zertifikat-Problem behoben: Server von HTTPS auf HTTP (localhost:5231) umgestellt
- ✅ ApiService baseUrl auf HTTP aktualisiert
- ✅ UseHttpsRedirection() im WebApi_Server deaktiviert
- ✅ Build-Fehler durch gesperrte Dateien behoben (bin/obj-Cache gelöscht)
- ✅ Prozess-Konflikte bei WebApi_Server.exe gelöst (Port-Belegung)
- ✅ Git-Repository auf GitHub eingerichtet (LeonTheGoatRohrer/BuddyRequest)
- ✅ Erster Push mit vollständigem Codestand durchgeführt

### Probleme & offene Punkte
- ❌ Navigation zwischen Views funktioniert noch nicht zuverlässig
- ❌ Dark/Light Mode nur auf SettingsPage aktiv, nicht auf allen Seiten

---

## Woche [11]: 04.02.2026

### ✅ Debugging & Fehlerbehebung
- ✅ Shell-Navigation repariert: FlyoutItem Route-Konflikte behoben
- ✅ Doppelte Route-Registrierung entfernt (XAML Route + Routing.RegisterRoute Konflikt)
- ✅ GoToAsync("///MainPage") durch Shell.Current.CurrentItem ersetzt (WinUI Shell-Bug)
- ✅ LoginViewModel: Navigation nach Login funktioniert jetzt zuverlässig
- ✅ MainPage-Buttons (Chats, Freunde, Profil) navigieren jetzt korrekt
- ✅ Dark/Light Mode DynamicResource auf alle Views erweitert (ChatPage, FriendsPage, ProfilPage)

### Probleme & offene Punkte
- ❌ MapPage crasht still wegen fehlender UseMauiMaps()-Initialisierung
- ❌ Sprachumschaltung nur auf SettingsPage wirksam

---

## Woche [12]: 12.02.2026

**Semesterferien – keine Arbeit am Projekt**

---

## Woche [13]: 19.02.2026

### ✅ Debugging & Fehlerbehebung
- ✅ Multi-Language Support (DE/EN) mit TranslateExtension auf LoginPage und MainPage eingebaut
- ✅ LocalizationService mit Dictionary-basiertem Übersetzungssystem erweitert
- ✅ TranslateExtension für XAML-Binding erstellt und getestet
- ✅ StringFormat-Problem mit MultiBinding gelöst (MAUI-Kompatibilität)
- ✅ AppShell.xaml.cs bereinigt: Nur noch ChatPage wird manuell registriert

### Probleme & offene Punkte
- ❌ Übersetzungen noch nicht auf allen Seiten aktiv (FriendsPage, ProfilPage ausstehend)
- ❌ MapPage Maps-Control verursacht Probleme auf Windows

---

## Woche [14]: 26.02.2026

### ✅ Location Sharing Feature implementiert
- ✅ User-Model um Latitude, Longitude und LastLocationUpdate erweitert
- ✅ Datenbank-Migration für Location-Spalten erstellt und manuell angewendet (ALTER TABLE)
- ✅ 3 neue API-Endpoints im UsersController:
  - POST /api/users/share-location/{id} – Standort speichern
  - GET /api/users/friends-locations/{id} – Freunde-Standorte abrufen
  - GET /api/users/my-location/{id} – Eigenen Standort abrufen
- ✅ LocationDto und FriendLocationDto Models erstellt
- ✅ ApiService um ShareLocationAsync, GetFriendsLocationsAsync, GetMyLocationAsync erweitert
- ✅ MapPage: Maps-Control durch OpenStreetMap/Leaflet.js WebView ersetzt (kein API Key nötig)
- ✅ Interaktive Karte mit Markern für eigenen Standort und Freunde
- ✅ Live-Update alle 10 Sekunden wenn Standortfreigabe aktiv
- ✅ Marker-Design: Weißer Text auf farbigem Hintergrund mit Schatten (gut lesbar auf Karte)
- ✅ Alle Änderungen zu GitHub gepusht

### Probleme & offene Punkte
- ❌ Karte zeigt Standardposition (Innsbruck) wenn GPS nicht verfügbar
- ❌ Freunde-Marker nur sichtbar wenn beide gleichzeitig Location teilen
