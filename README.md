# TrackTrash API

Backend de TrackTrash: API ASP.NET Core (.NET 10) con arquitectura limpia, CQRS con MediatR y PostgreSQL.

## Proyectos

| Proyecto | Contiene | Depende de |
| --- | --- | --- |
| `TrackTrash.Domain` | Entidades, enums, puertos de repositorio, `IUnitOfWork`. Sin paquetes. | Nada |
| `TrackTrash.Application` | Casos de uso (`UseCases/<Area>/Commands` y `Queries`), DTOs, puertos de servicios externos | Domain |
| `TrackTrash.Infraestructure` | `DbContext`, repositorios, servicios, registro de dependencias | Domain, Application |
| `TrackTrashAPI` | Controladores, middleware, Swagger, `Program.cs` | Application, Infraestructure |
| `TrackTrash.Tests` | Pruebas xUnit | Todos |

## Requisitos

- .NET SDK 10
- PostgreSQL 15 o superior

## Puesta en marcha

1. Crear la base de datos y cargar el esquema:

   ```bash
   createdb tracktrash_db
   psql -d tracktrash_db -f database/01_schema.sql
   ```

2. Crear `TrackTrashAPI/appsettings.Development.json`. Este archivo no se versiona; cada persona pone ahí sus valores reales:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=tracktrash_db;Username=postgres;Password=<tu-password>"
     }
   }
   ```

3. Compilar, probar y ejecutar:

   ```bash
   dotnet build
   dotnet test
   dotnet run --project TrackTrashAPI
   ```

4. Abrir Swagger en `http://localhost:5145/swagger`.

Ejecutar siempre con `dotnet run` o el perfil del IDE. Si se lanza el `.exe` directamente, el entorno no es `Development`: no se carga `appsettings.Development.json` ni se publica Swagger.

## Base de datos

`database/01_schema.sql` se generó desde el modelo de EF Core (`dotnet ef dbcontext script`). Contiene las tablas, claves e índices. No contiene las tres vistas que usa el módulo de indicadores (`vw_gatherer_grades`, `vw_general_indicators`, `vw_residues_by_type`); su definición hay que agregarla desde la base de datos original.

Los cambios de esquema van en scripts nuevos dentro de `database/`, nunca editando uno ya publicado.

## Convenciones

- Una rama por módulo: `feature/<modulo>`. Un commit por tarea: `tipo(modulo): descripción`.
- Un archivo por operación: el `record` del Command o Query y su handler `internal sealed`.
- Los handlers lanzan `InvalidOperationException` (400), `KeyNotFoundException` (404) o `UnauthorizedAccessException` (401). `ExceptionHandlingMiddleware` los convierte en la respuesta HTTP.
- Un `InvalidOperationException` que envuelve otra excepción (por ejemplo, un fallo de conexión a la base de datos) se responde como 500 con mensaje genérico, no como 400.
- Los controladores solo envían el mensaje con `ISender`; no llevan `try/catch`.
- Código e identificadores en inglés, sin comentarios.
- `Program.cs`, los `.csproj`, el `DbContext`, los registros de dependencias, el repositorio genérico y `IUnitOfWork` son archivos compartidos: los cambios se piden a quien mantiene la base.
