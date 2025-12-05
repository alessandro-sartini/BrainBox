# BrainBox

**BrainBox** è un'applicazione di gestione idee composta da un backend API REST e un frontend desktop WPF. Permette di organizzare idee creative associandole a temi/categorie personalizzabili.

## 📋 Prerequisiti

Prima di iniziare, assicurati di avere installato:

- **Visual Studio 2022** (Community o superiore)
  - Workload: "ASP.NET e sviluppo web"
  - Workload: ".NET Desktop Development"
- **.NET 8.0 SDK**
- **SQL Server Express** (o versione completa)
- **Git**

## 🚀 Installazione

### 1. Clone del Repository

```bash
git clone https://github.com/alessandro-sartini/BrainBox.git
cd BrainBox
```

### 2. Configurazione Database

Il progetto usa SQL Server con Entity Framework Core. Il connection string di default è configurato per SQL Server Express:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=.\\sqlexpress;Initial Catalog=BrainBoxDb;Integrated Security=True;TrustServerCertificate=True;"
}
```

**Se usi un'istanza diversa di SQL Server**, modifica il connection string in `BrainBox/appsettings.json`.

### 3. Creazione Database

Apri la **Package Manager Console** in Visual Studio e esegui:

```powershell
Update-Database
```

Questo comando applica le migrations e crea il database `BrainBoxDb` con tutte le tabelle necessarie.

### 4. Configurazione Startup Projects

1. In Visual Studio, click destro sulla **Solution** nel Solution Explorer
2. Seleziona **Set Startup Projects...**
3. Scegli **Multiple startup projects**
4. Imposta sia `BrainBox` che `BrainBox.Desktop` su **Start**

### 5. Avvio Applicazione

Premi **F5** per avviare entrambi i progetti contemporaneamente.

## 🏗️ Architettura

### Backend: BrainBox (Web API)

Web API REST costruita con **ASP.NET Core 8.0** che espone endpoint per la gestione di idee e temi.

#### Endpoint Principali

**Ideas Controller** (`/api/Ideas`)
- `GET /api/Ideas` - Lista tutte le idee
- `GET /api/Ideas/{id}` - Dettaglio idea specifica
- `POST /api/Ideas` - Crea nuova idea
- `PUT /api/Ideas/{id}` - Aggiorna idea esistente
- `DELETE /api/Ideas/{id}` - Elimina idea

**Themes Controller** (`/api/Themes`)
- `GET /api/Themes` - Lista tutti i temi
- `POST /api/Themes` - Crea nuovo tema
- `DELETE /api/Themes/{id}` - Elimina tema

#### Modello Dati

**Idea**
- `Id` (int) - Identificativo univoco
- `Title` (string, max 200 caratteri) - Titolo dell'idea
- `Description` (string) - Descrizione dettagliata
- `CreatedAt` (DateTime) - Data creazione
- `LastModifiedAt` (DateTime) - Data ultima modifica
- `IdeaThemes` (ICollection) - Relazione many-to-many con i temi

**Theme**
- `Id` (int) - Identificativo univoco
- `Name` (string, max 100 caratteri) - Nome del tema
- `IdeaThemes` (ICollection) - Relazione many-to-many con le idee

**IdeaTheme**
- Tabella di join per relazione many-to-many tra Idea e Theme

#### Tecnologie Backend

- **ASP.NET Core 8.0** - Framework web
- **Entity Framework Core 8.0** - ORM per SQL Server
- **SQL Server** - Database relazionale
- **Swagger/OpenAPI** - Documentazione API interattiva (disponibile in development mode)

### Frontend: BrainBox.Desktop (WPF)

Applicazione desktop **Windows Presentation Foundation (WPF)** che consuma le API del backend.

#### Struttura Progetto

```
BrainBox.Desktop/
├── Controls/          # Controlli personalizzati riutilizzabili
├── Model/             # Modelli dati (DTOs)
├── Services/          # Servizi per comunicazione HTTP con API
├── MainWindow.xaml    # Finestra principale con menu navigazione
├── IdeasWindow.xaml   # Gestione idee (CRUD)
└── ThemesWindow.xaml  # Gestione temi (CRUD)
```

#### Funzionalità Frontend

- **MainWindow**: Schermata principale con pulsanti di navigazione
  - "Gestisci Idee" → Apre IdeasWindow
  - "Gestisci Temi" → Apre ThemesWindow

- **IdeasWindow**: Interfaccia completa per gestire idee
  - Creazione nuove idee con titolo e descrizione
  - Modifica idee esistenti
  - Eliminazione idee
  - Associazione idee a temi multipli
  - Visualizzazione date creazione/modifica

- **ThemesWindow**: Gestione categorie/temi
  - Creazione nuovi temi
  - Visualizzazione lista temi esistenti
  - Eliminazione temi

## 🔄 Flusso di Funzionamento

1. Il **backend API** si avvia ed espone endpoint REST (es. `https://localhost:7000`)
2. L'**app WPF** si connette all'API tramite `HttpClient` nei Services
3. L'utente interagisce con l'**interfaccia grafica WPF**
4. Le operazioni CRUD vengono inviate come richieste HTTP all'**API**
5. L'API processa le richieste, interagisce con **SQL Server** tramite EF Core
6. I dati aggiornati ritornano al **client WPF** che aggiorna la UI

```
┌─────────────────┐         HTTP REST API        ┌──────────────┐
│  BrainBox.      │  ◄─────────────────────────► │   BrainBox   │
│  Desktop (WPF)  │      (JSON DTOs)              │   Web API    │
└─────────────────┘                               └──────┬───────┘
                                                         │
                                                         │ EF Core
                                                         ▼
                                                  ┌──────────────┐
                                                  │  SQL Server  │
                                                  │  BrainBoxDb  │
                                                  └──────────────┘
```

## 🧪 Testing API

Durante lo sviluppo, puoi testare le API usando:

1. **Swagger UI** - Naviga a `https://localhost:[porta]/swagger` quando l'API è in esecuzione
2. **Postman** - Importa gli endpoint dalla documentazione Swagger
3. **File .http** - Usa `BrainBox/BrainBox.http` con Visual Studio

## 📦 Dipendenze NuGet

### Backend (BrainBox)
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.22)
- `Microsoft.EntityFrameworkCore.Tools` (8.0.22)
- `Swashbuckle.AspNetCore` (6.6.2)
- `Microsoft.VisualStudio.Web.CodeGeneration.Design` (8.0.7)

### Frontend (BrainBox.Desktop)
- Framework: **WPF (.NET 8.0)**

## 🛠️ Comandi Utili

### Entity Framework Migrations

```powershell
# Creare una nuova migration
Add-Migration NomeMigration

# Applicare migrations al database
Update-Database

# Rimuovere ultima migration non applicata
Remove-Migration
```

## 📝 Note Tecniche

- **Pattern Architetturale**: Client-Server con separazione netta tra backend e frontend
- **Data Transfer**: DTOs per disaccoppiare entità database da oggetti API
- **Database**: Code-First approach con EF Core Migrations
- **UI Pattern**: XAML/Code-behind nel frontend WPF
- **API Design**: RESTful con convenzioni standard HTTP

## 🔗 Link Utili

- Repository GitHub: [https://github.com/alessandro-sartini/BrainBox](https://github.com/alessandro-sartini/BrainBox)
- Documentazione .NET 8.0: [https://learn.microsoft.com/dotnet/](https://learn.microsoft.com/dotnet/)
- Entity Framework Core: [https://learn.microsoft.com/ef/core/](https://learn.microsoft.com/ef/core/)

## 👤 Autore

**Alessandro Sartini**
- GitHub: [@alessandro-sartini](https://github.com/alessandro-sartini)

---

**BrainBox** - Organizza le tue idee creative 💡
