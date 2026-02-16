# Helpdesk Management System (ASP.NET Core)

A Helpdesk management web application built using ASP.NET Core and Entity Framework Core.

This project follows a layered architecture with:
- DAL (Data Access Layer)
- ViewModels
- Web API + Static Frontend (HTML/CSS/JS)

The data layer was generated using **Entity Framework Core Database-First (Scaffold-DbContext)** from an existing SQL Server database.

---

## Features

- Employee management (CRUD)
- Department and Problem lookup
- Call logging and tracking
- Reporting pages
- Swagger API documentation
- Static frontend pages served from wwwroot

---

## Tech Stack

- C#
- ASP.NET Core Web API
- Entity Framework Core (Database-First)
- SQL Server LocalDB
- HTML, CSS, JavaScript

---

## Architecture

- HelpdeskDAL → Entities + DbContext (scaffolded)
- HelpdeskViewModels → Data transfer models
- HelpdeskWebsite → Controllers + Frontend + API

---

## Database Setup

This project uses **SQL Server LocalDB**.

The database was created manually and then scaffolded into the project using:

Scaffold-DbContext

To run this project, you must create a local database named:

HelpdeskDb

Connection string used:

Server=(localdb)\MSSQLLocalDB;Database=HelpdeskDb;Trusted_Connection=True;

If the database does not exist, the application will not run correctly.

---

## How to Run

1. Clone the repository
2. Open the solution in Visual Studio
3. Ensure SQL Server LocalDB is installed
4. Make sure a database named `HelpdeskDb` exists
5. Run the HelpdeskWebsite project
6. Access the UI pages via:

   https://localhost:xxxx/Home.html

Swagger API documentation is available at:

   https://localhost:xxxx/swagger

---

## Notes

- This project was developed for academic purposes.
- The database schema was designed first, then scaffolded into Entity Framework Core.
- The connection string currently points to LocalDB.


## Screenshots
<img width="2414" height="1399" alt="image" src="https://github.com/user-attachments/assets/3690be72-40b5-41f6-bd98-4d07af1615b2" />

<img width="2442" height="1383" alt="image" src="https://github.com/user-attachments/assets/94f085e0-5b5d-4c8e-83de-75a6e7cb6050" />
<img width="2423" height="1401" alt="image" src="https://github.com/user-attachments/assets/70606c2e-7c2c-46cb-ab8c-2f8652f88627" />

