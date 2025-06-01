# Backend - Projeto Bludata

## Tecnologias utilizadas

- .NET 8
- ASP.NET Core WebAPI
- Entity Framework Core
- PostgreSQL

---

## Requisitos para rodar

- [.NET 8 SDK]
- [PostgreSQL]
- Ferramenta de linha de comando do EF Core:


## Como rodar o projeto:

- Instalar o .NET 8 SDK
- Instalar PostgreSQL e deixar rodando
- Instalar ferramenta EF Core com: dotnet tool install --global dotnet-ef
- Clonar repositório e acessar a pasta backend
- Configurar a connection string no appsettings.json com Host, Port, Database, Username e Password
- Restaurar dependências com dotnet restore
- Compilar projeto com dotnet build
- Aplicar migrations com dotnet ef database update
- Rodar API com dotnet run
