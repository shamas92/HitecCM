# HitecCM — University Content Management

An ASP.NET MVC 5 administration application for managing the content and operational data used by the [Hitec University Website](https://github.com/shamas92/HitecWebsite). The codebase includes management areas for departments, admissions, events, facilities, faculty, downloads, policies, quality assurance, and user accounts.

## Technology

- C# and ASP.NET MVC 5
- .NET Framework 4.7.2
- Entity Framework 6
- SQL Server
- Razor views, Bootstrap, jQuery

## Run locally

1. Install Visual Studio with the **ASP.NET and web development** workload and the .NET Framework 4.7.2 developer pack.
2. Install or connect to SQL Server and prepare the application database/schema.
3. Open `HitecCM.sln` in Visual Studio.
4. Set a local SQL Server connection string for the `Model1` Entity Framework context in `Web.config`. Keep credentials and machine-specific values out of Git.
5. Restore NuGet packages, then run the `HitecCM` project with IIS Express.

The application depends on a compatible SQL Server database populated with the expected tables. Database setup and seed data are not documented or bundled here, so the project cannot be fully exercised from a clean clone without them.

## Project areas

- `Controllers/` and `Views/`: administration pages and workflows
- `Models/`: Entity Framework data model
- `App_Start/`: MVC routing, filters, and bundles
- `Content/` and `Scripts/`: user interface assets

## Security and deployment

Configure authentication, authorization, database access, request validation, and production error handling for your environment before deployment. Do not publish local connection strings or Visual Studio cache files. This is a legacy ASP.NET MVC application for Windows/Visual Studio.

