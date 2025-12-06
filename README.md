# BrainBox

**BrainBox** è un'applicazione di gestione idee composta da un backend API REST e un frontend desktop WPF. Permette di organizzare idee creative associandole a temi/categorie personalizzabili.

## Prerequisiti

Prima di iniziare, assicurati di avere installato:

- Visual Studio 2022 (Community o superiore)
- .NET 8.0 SDK
- SQL Server Express (o versione completa)
- Git

## Installazione

### 1. Clone del Repository

```bash
git clone https://github.com/alessandro-sartini/BrainBox.git
cd BrainBox
```

### 2. Configurazione Database

Modifica il connection string in `BrainBox/appsettings.json` se usi un'istanza di SQL Server diversa da Express:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=.\\sqlexpress;Initial Catalog=BrainBoxDb;Integrated Security=True;TrustServerCertificate=True;"
}
```

### 3. Creazione Database

Apri Package Manager Console in Visual Studio e esegui:

```powershell
Update-Database
```

### 4. Importazione Dump Dati

Se desideri importare i dati di esempio:

1. Apri SQL Server Management Studio
2. Connettiti al server SQL Server
3. Fai clic destro su **BrainBoxDb** > **Tasks** > **Restore**
4. Importa il file dump fornito

**Attenzione**: Se esegui `Update-Database` dopo aver importato un dump, potrebbe verificarsi un conflitto se il dump contiene versioni diverse delle migrations. In tal caso:
- Esegui prima il dump
- Poi sincronizza le migrations eseguendo `Update-Database`

### 5. Configurazione Startup Projects

1. Click destro sulla **Solution** nel Solution Explorer
2. Seleziona **Set Startup Projects...**
3. Scegli **Multiple startup projects**
4. Imposta sia `BrainBox` che `BrainBox.Desktop` su **Start**

### 6. Avvio Applicazione

Premi **F5** per avviare entrambi i progetti.

## Architettura

### Backend: BrainBox (Web API)

Web API REST costruita con ASP.NET Core 8.0.

**Endpoint Principali**

- `GET /api/Ideas` - Lista tutte le idee
- `GET /api/Ideas/{id}` - Dettaglio idea specifica
- `POST /api/Ideas` - Crea nuova idea
- `PUT /api/Ideas/{id}` - Aggiorna idea
- `DELETE /api/Ideas/{id}` - Elimina idea
- `GET /api/Themes` - Lista tutti i temi
- `POST /api/Themes` - Crea nuovo tema
- `DELETE /api/Themes/{id}` - Elimina tema

**Modello Dati**

- **Idea**: Id, Title, Description, CreatedAt, LastModifiedAt, IdeaThemes
- **Theme**: Id, Name, IdeaThemes
- **IdeaTheme**: Tabella di join per relazione many-to-many

### Frontend: BrainBox.Desktop (WPF)

Applicazione desktop Windows Presentation Foundation.

**Funzionalità**

- MainWindow: Schermata principale con pulsanti di navigazione
- IdeasWindow: CRUD completo per idee con associazione a temi
- ThemesWindow: Gestione categorie/temi

## Dipendenze

**Backend**
- Microsoft.EntityFrameworkCore.SqlServer (8.0.22)
- Microsoft.EntityFrameworkCore.Tools (8.0.22)
- Swashbuckle.AspNetCore (6.6.2)

**Frontend**
- WPF (.NET 8.0)

## Comandi Utili

```powershell
# Creare una nuova migration
Add-Migration NomeMigration

# Applicare migrations
Update-Database

# Rimuovere ultima migration non applicata
Remove-Migration
```

## Testing API

Durante lo sviluppo puoi testare le API usando:

1. **Swagger UI** - Naviga a `https://localhost:[porta]/swagger` quando l'API è in esecuzione
2. **Postman** - Importa gli endpoint dalla documentazione Swagger
3. **File .http** - Usa `BrainBox/BrainBox.http` con Visual Studio

## Link Utili

- Repository GitHub: https://github.com/alessandro-sartini/BrainBox
- Documentazione .NET 8.0: https://learn.microsoft.com/dotnet/
- Entity Framework Core: https://learn.microsoft.com/ef/core/

## Autore

**Alessandro Sartini** - [@alessandro-sartini](https://github.com/alessandro-sartini)
