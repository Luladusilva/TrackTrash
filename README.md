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

2. Configurar los secretos de desarrollo (no guardarlos en `appsettings.json`):

   ```json
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=tracktrash_db;Username=postgres;Password=<tu-password>" --project TrackTrashAPI
   dotnet user-secrets set "Jwt:Key" "<secreto-aleatorio-de-al-menos-32-bytes>" --project TrackTrashAPI
   ```

3. Compilar, probar y ejecutar:

   ```bash
   dotnet build
   dotnet test
   dotnet run --project TrackTrashAPI
   ```

4. Abrir Swagger en `http://localhost:5145/swagger`.

Ejecutar siempre con `dotnet run` o el perfil del IDE. Si se lanza el `.exe` directamente, el entorno no es `Development`: no se carga `appsettings.Development.json` ni se publica Swagger.

## Despliegue en Render

1. Crea un **Web Service** conectado a este repositorio. Selecciona la rama `main`, el runtime **Docker** y deja vacío **Root Directory** para usar el `Dockerfile` de la raíz.
2. En **Environment**, configura estas variables antes del primer despliegue.

   | Variable | Valor |
   | --- | --- |
   | `ConnectionStrings__DefaultConnection` | Cadena de conexión de PostgreSQL en formato Npgsql, con host, base de datos, usuario, contraseña y configuración TLS de Neon. |
   | `Jwt__Key` | Secreto aleatorio de producción de al menos 32 bytes. La API requiere esta clave para arrancar. |

3. Pulsa **Deploy Web Service**. El contenedor escucha en el puerto que Render asigna mediante `PORT` y usa `10000` por defecto. Configura **Health Check Path** con `/api/health`.

## Base de datos

`database/01_schema.sql` se generó desde el modelo de EF Core (`dotnet ef dbcontext script`). Contiene las tablas, claves e índices. No contiene las tres vistas que usa el módulo de indicadores (`vw_gatherer_grades`, `vw_general_indicators`, `vw_residues_by_type`); su definición hay que agregarla desde la base de datos original.

Los cambios de esquema van en scripts nuevos dentro de `database/`, nunca editando uno ya publicado.

Después de `01_schema.sql`, ejecutar `03_add_auth_and_applications.sql` y luego `04_seed_residue_types.sql`.
El script 03 crea las sesiones de refresh y las postulaciones de recolectores; el 04 agrega el catálogo inicial de
materiales asociados a las categorías fijas `Recyclable`, `Organic` y `General`.

## Autenticación y autorización

- `POST /api/Auth/Login` recibe `{ "email", "password" }` y devuelve `accessToken`, `refreshToken`, `tokenType`, `expiresAt`, `expiresIn` y `refreshAfterSeconds` (1800).
- El access token dura 4 horas. `POST /api/Auth/Refresh` recibe `{ "refreshToken" }`, rota el refresh token y devuelve un nuevo par; el refresh token tiene una vigencia móvil de 7 días desde cada renovación.
- `POST /api/Auth/Logout` revoca la familia de refresh tokens de esa sesión. Reutilizar un refresh token ya rotado revoca también esa familia.
- `GET /api/User/me` devuelve el perfil asociado al access token. Las respuestas de autenticación no incluyen el perfil.
- Las solicitudes protegidas usan `Authorization: Bearer <accessToken>`. Un access token vencido responde `401` con `code: "ACCESS_TOKEN_EXPIRED"`; el cliente debe renovar la sesión solo mientras haya actividad y guardar el refresh token rotado.
- La API exige autenticación por defecto. Login, refresh, logout y registro de usuario permiten acceso anónimo (logout se valida con el refresh token); los permisos por rol se aplican por endpoint y los cambios de datos sensibles comprueban la propiedad del recurso.
- `refreshAfterSeconds` indica el intervalo mínimo antes de renovar si el cliente sigue activo. La API expone el refresh y rota los tokens; el conteo de actividad de usuario debe aplicarse en cada cliente al integrarlo.

## Matriz de endpoints por rol

Todos los endpoints requieren Bearer token salvo `POST /api/Auth/Login`, `POST /api/Auth/Refresh` y `POST /api/User`.

| Recurso/endpoint | Generator | Organization | Gatherer | Admin |
| --- | --- | --- | --- | --- |
| `GET /api/User/me`, `PUT /api/User/{id}` propio, cambio de password propio | Sí | Sí | Sí | Sí |
| `GET /api/CollectRequest/user/{id}` propio, crear/editar/cancelar solicitud | Sí | Sí | No | Sí |
| `GET /api/CollectRequest/pending`, ver detalle compatible, postularse | No | No | Sí | Consulta completa |
| `GET /api/Gatherer`, perfiles y mapa filtrados por residuo o solicitud | Sí | Sí | No | Sí |
| Aplicaciones de solicitud: listar/aceptar/rechazar | Propietario | Propietario | Postular/ver propias | Supervisar |
| `PUT /api/Gatherer/me/residue-types` | No | No | Sí | `PUT /api/Gatherer/{id}/residue-types` |
| Perfil de organizacion/solicitante | Propio | Propio | Lectura segura de solicitudes visibles | Lectura completa |
| `GET /api/GathererGrade/gatherer/{id}` calificaciones | Sí | Sí | Sí | Sí |
| Historial de solicitud y servicio | Participante | Participante | Participante | Todos |
| Usuarios, reportes, indicadores y datos globales | No | No | No | Sí |
| Gestionar catalogo `ResidueType` | No | No | No | Sí |

Una solicitud contiene `details[]`, cada elemento con `residueTypeId`, `estimatedQuantity`, `unitId` y `observations`. El material debe existir, estar activo y ser reciclable. `ResidueCategory` permanece como enum; `ResidueType` es un catalogo administrable en base de datos para no requerir despliegues cuando se agreguen materiales.

## Convenciones

- Una rama por módulo: `feature/<modulo>`. Un commit por tarea: `tipo(modulo): descripción`.
- Un archivo por operación: el `record` del Command o Query y su handler `internal sealed`.
- Los handlers lanzan `InvalidOperationException` (400), `KeyNotFoundException` (404) o `UnauthorizedAccessException` (401). `ExceptionHandlingMiddleware` los convierte en la respuesta HTTP.
- Un `InvalidOperationException` que envuelve otra excepción (por ejemplo, un fallo de conexión a la base de datos) se responde como 500 con mensaje genérico, no como 400.
- Los controladores solo envían el mensaje con `ISender`; no llevan `try/catch`.
- Código e identificadores en inglés, sin comentarios.
- `Program.cs`, los `.csproj`, el `DbContext`, los registros de dependencias, el repositorio genérico y `IUnitOfWork` son archivos compartidos: los cambios se piden a quien mantiene la base.
