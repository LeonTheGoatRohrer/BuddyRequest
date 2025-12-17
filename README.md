# ?? BuddyRequest - Messenger App

Eine mobile Messenger-Anwendung für schnelle Alltagsanfragen zwischen Freunden und Familie.

## ?? Über das Projekt

BuddyRequest ist eine .NET MAUI App, die es Nutzern ermöglicht, kleine Alltagsfragen 
("Hast du Wasser dabei?", "Ist noch Brot zuhause?") strukturiert zu stellen und zu beantworten.

## ??? Technologie-Stack

- **Frontend:** .NET MAUI 10 (Multi-platform App UI)
- **Backend:** ASP.NET Core 10 WebAPI
- **Datenbank:** MySQL / MariaDB
- **ORM:** Entity Framework Core 9.0
- **API-Kommunikation:** HttpClient mit JSON
- **Authentifizierung:** ASP.NET Identity mit Password Hashing

## ? Features

### ? Implementiert
- Benutzerregistrierung & Login
- User-Verwaltung (Username, Email, eindeutiger User-Key)
- Freunde-System (Anfragen senden/annehmen/ablehnen)
- User-Suche (nach Username oder Key)
- Session-Management mit lokaler Speicherung
- Freundesliste anzeigen

### ?? In Entwicklung
- Offene Anfragen mit Accept/Decline UI
- Anfragen-Verwaltung vervollständigen
- Nachrichtensystem

### ?? Geplant
- Push-Benachrichtigungen (Firebase)
- Gruppen-Funktionalität
- Standort & Treffpunkt
- Gamification (Badges, Punkte)
- Admin Dashboard

## ??? Projektstruktur

```
Messanger/
??? Messanger/          # .NET MAUI Frontend
?   ??? Views/          # XAML Pages (Login, Registration, Friends, Main)
?   ??? ViewModels/     # MVVM ViewModels
?   ??? Services/       # API Service, UserSession
??? WebApi_Server/      # ASP.NET Core Backend
?   ??? Controllers/    # REST API Controllers (Users, Friends)
??? Models/             # Shared Data Models (User, Friends, Messages)
??? ORM/                # Entity Framework DbContext & Migrations
    ??? Services/       # DbManager
```

## ?? Installation & Setup

### Voraussetzungen
- .NET 10 SDK
- Visual Studio 2022 (mit MAUI Workload)
- MySQL Server 8.0+

### Setup-Schritte

1. **Repository klonen**
   ```bash
   git clone [REPO-URL]
   cd Messanger
   ```

2. **MySQL-Datenbank erstellen**
   ```sql
   CREATE DATABASE Messanger;
   ```

3. **Verbindungsstring anpassen**
   - Datei: `ORM/Services/DbManager.cs`
   - Connection-String mit eigenen MySQL-Credentials anpassen

4. **Entity Framework Migrations ausführen**
   ```bash
   cd ORM
   dotnet ef database update --startup-project ../WebApi_Server
   ```

5. **Projekte starten**
   - In Visual Studio: Beide Projekte als Startup Projects konfigurieren
   - Oder manuell:
     ```bash
     # Terminal 1 - Backend
     cd WebApi_Server
     dotnet run
     
     # Terminal 2 - Frontend
     cd Messanger
     dotnet build -t:Run -f net10.0-windows10.0.19041.0
     ```

6. **API testen**
   - Swagger UI: `https://localhost:7246/swagger`

## ?? API Endpoints

### Users
- `POST /api/users/register` - Neuen User registrieren
- `POST /api/users/login` - User einloggen
- `GET /api/users/{id}` - User nach ID abrufen
- `GET /api/users/byKey/{key}` - User nach Key abrufen

### Friends
- `GET /api/friends/search?query={query}` - User suchen
- `POST /api/friends/request` - Freundschaftsanfrage senden
- `GET /api/friends/pending/{userId}` - Offene Anfragen abrufen
- `GET /api/friends/list/{userId}` - Freundesliste abrufen
- `PUT /api/friends/accept/{requestId}` - Anfrage akzeptieren
- `DELETE /api/friends/decline/{requestId}` - Anfrage ablehnen

## ?? Datenbank-Schema

### User
- Id (Primary Key)
- Username (unique)
- Email
- Password (hashed)
- Key (eindeutiger User-Key, z.B. "MA1234")

### Friends
- Id (Primary Key)
- UserId (Foreign Key ? User)
- FriendUserId (Foreign Key ? User)
- Angenommen (bool)
- CreatedAt (DateTime)

### Messages
- Id (Primary Key)
- SenderId (Foreign Key ? User)
- EmpfaengerId (Foreign Key ? User)
- Message (Text)
- SentAt (DateTime)

## ?? Projekt-Kontext

Schulprojekt - 5AHWII SWP (Software-Projekt)  
Entwickelt als Teil des Lehrplans für moderne Full-Stack-Entwicklung

## ????? Entwickler

Mario Rohrer - HTL Projekt 2024/2025

## ?? Lizenz

Dieses Projekt ist für Bildungszwecke erstellt.
