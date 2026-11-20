# UniCampus — Gestión de Actividades Académicas

Proyecto de la asignatura **Desarrollo de Aplicaciones Web II** (Ing. Cindy Fuentes) — Corporación Universitaria Americana.

Aplicación web en **ASP.NET Web Forms (.NET Framework 4.8)** con arquitectura de **tres capas** (`Web → BLL → DAL` + `Entities`) y patrón **MVP** (Model-View-Presenter), conectada a **SQL Server** mediante **ADO.NET**.

> **Versión actual:** Entrega final (`v2.0-final`): gestión completa de actividades, catálogo con búsqueda y filtros, inscripciones con control de cupos transaccional, reseñas y valoraciones, reportes, e inicio de sesión con roles.

---

## Estructura de la solución

```
UniCampus.sln
├── UniCampus.Entities   DTOs y catálogos de valores (estados, roles, modalidades)
├── UniCampus.DAL        Repositorios ADO.NET (SqlConnection, SqlCommand, SqlParameter, SqlTransaction)
├── UniCampus.BLL        Servicios con reglas de negocio y validaciones
├── UniCampus.Web        Páginas (.aspx), contratos de vista (Views/), Presenters/ y Seguridad/
└── BaseDatos/03_BaseDatos.sql
```

| Proyecto | Tipo | Referencias |
| :--- | :--- | :--- |
| `UniCampus.Entities` | Biblioteca de clases | Ninguna |
| `UniCampus.DAL` | Biblioteca de clases | `UniCampus.Entities`, `System.Configuration`, `System.Data` |
| `UniCampus.BLL` | Biblioteca de clases | `UniCampus.DAL`, `UniCampus.Entities` |
| `UniCampus.Web` | Aplicación web ASP.NET Web Forms | `UniCampus.BLL`, `UniCampus.Entities` |

**Flujo de una petición** (ejemplo, inscripción):
`DetalleActividad.aspx.cs` → `DetalleActividadPresenter` → `InscripcionService` (BLL) → `InscripcionRepository` (DAL, con `SqlTransaction`) → SQL Server.

Cada página sigue el mismo patrón: la Vista implementa una interfaz `I...View`, el Presenter solo conoce esa interfaz y llama a un servicio de la BLL, y el servicio usa un repositorio de la DAL. Ninguna capa se salta a otra.

---

## Páginas

| Página | Quién la usa | Qué hace | Responsable |
| :--- | :--- | :--- | :--- |
| `Login.aspx` | Todos | Inicio de sesión con roles | Cristian |
| `Actividades.aspx` | Organizador / Administrador | Registrar, ver detalle, editar, desactivar y publicar actividades | Rafael |
| `Catalogo.aspx` | Todos | Búsqueda por texto y filtros por tipo, modalidad, fechas y cupos | Alber |
| `Reportes.aspx` | Organizador / Administrador | Totales, ocupación por tipo y por actividad, promedio de reseñas | Alber |
| `DetalleActividad.aspx` | Todos | Detalle, cupos disponibles, inscripción y cancelación; lista de inscritos para organizadores | Samuel |
| `MisInscripciones.aspx` | Estudiante | Inscripciones propias y cancelación | Samuel |
| `Resenas.aspx` | Todos | Reseñas de 1 a 5 estrellas con comentario (anti-XSS) | Cristian |

---

## Requisitos

- Visual Studio 2022 o superior con la carga de trabajo **Desarrollo de ASP.NET y web**
- **.NET Framework 4.8 Developer Pack**
- SQL Server 2016 o superior (Express sirve) y SQL Server Management Studio (SSMS)

## Instalación y configuración

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/rafaillera/UniCampus.git
   ```
2. **Crear la base de datos:** en SSMS, **Archivo → Abrir → Archivo** → `BaseDatos/03_BaseDatos.sql` → **F5**. Crea `UniCampusDB`, las 5 tablas, sus restricciones y los datos de prueba. Las fechas de las actividades se calculan a partir del día en que se ejecuta el script, así siempre hay actividades próximas y ya realizadas.
   - Si SSMS muestra *"Incorrect syntax near"* en la línea 1, borre el carácter invisible del inicio de la línea 1 (tecla Supr) y ejecute de nuevo.
   - Alternativa por consola: `sqlcmd -S .\SQLEXPRESS -E -C -f 65001 -i BaseDatos/03_BaseDatos.sql`
3. **Revisar la cadena de conexión** en `UniCampus.Web/Web.config`:
   ```xml
   <add name="UniCampusDb"
        connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=UniCampusDB;Integrated Security=True;TrustServerCertificate=True"
        providerName="System.Data.SqlClient" />
   ```
   Si su instancia no es `.\SQLEXPRESS`, cambie `Data Source` (por ejemplo `.` o `(localdb)\MSSQLLocalDB`). Se usa autenticación de Windows: no se guarda ninguna contraseña.
4. **Abrir `UniCampus.sln`** (no "Abrir carpeta"), clic derecho sobre `UniCampus.Web` → *Establecer como proyecto de inicio*.
5. **Compilar** (Ctrl+Shift+B) y **ejecutar** (F5). Se abre `Login.aspx`.

## Usuarios de prueba

| Correo | Rol | Para probar |
| :--- | :--- | :--- |
| `admin@unicampus.edu.co` | Administrador | Gestión de actividades y reportes |
| `carlos.perez@unicampus.edu.co` | Organizador | Gestión, reportes y lista de inscritos |
| `laura.martinez@unicampus.edu.co` | Estudiante | Catálogo, inscripción y reseñas |
| `andres.gomez@unicampus.edu.co` | Estudiante | Catálogo, inscripción y reseñas |
| `valeria.rodriguez@unicampus.edu.co` | Estudiante | Catálogo e inscripción |
| `sofia.ramirez@unicampus.edu.co` | Estudiante | Puede reseñar la conferencia de IA (asistió y aún no la ha reseñado) |

La contraseña de todos es la definida en `@ClaveDemo` dentro de `BaseDatos/03_BaseDatos.sql`. Es solo para pruebas: en la base de datos se guarda el hash SHA-256 con un salt aleatorio por usuario, nunca la contraseña.

---

## Reglas de negocio (BLL)

**Actividades**

| Campo / operación | Regla |
| :--- | :--- |
| Título | Obligatorio, máximo 150 caracteres |
| Descripción | Opcional, máximo 1000 caracteres |
| Tipo | Obligatorio y existente en `TipoActividad` |
| Fecha de inicio | Obligatoria, no anterior a hoy (al editar, se permite si no se cambia) |
| Fecha de fin | Obligatoria, no anterior a la fecha de inicio |
| Horas | Si la actividad dura un día, la hora de fin debe ser posterior a la de inicio |
| Modalidad | Presencial, Virtual o Híbrida |
| Lugar | Obligatorio, máximo 150 caracteres |
| Cupos | Entero de 1 a 1000; al editar, nunca menor que los estudiantes inscritos |
| Desactivar | Borrado lógico (`Estado = Inactiva`): se conservan inscripciones y reseñas |
| Publicar | No se puede publicar una actividad que ya terminó |

**Catálogo:** los estudiantes solo ven actividades publicadas; el texto de búsqueda tiene máximo 100 caracteres y la fecha "desde" no puede ser posterior a "hasta".

**Inscripciones:** solo estudiantes, solo en actividades publicadas que no hayan comenzado, sin duplicados y sin superar los cupos. Se puede cancelar antes del día de inicio; si el estudiante se vuelve a inscribir, se reactiva la misma inscripción.

**Reseñas:** solo estudiantes inscritos (o que asistieron), cuando la actividad ya comenzó, una por estudiante y actividad, calificación de 1 a 5 y comentario de máximo 1000 caracteres.

## Seguridad aplicada

- **Sin inyección SQL:** todas las consultas usan `SqlParameter` tipados (`SqlDbType`). En la búsqueda, los comodines de `LIKE` que escribe el usuario se escapan.
- **Contraseñas:** hash SHA-256 con salt aleatorio por usuario, comparación en tiempo constante y el mismo mensaje para correo o contraseña incorrectos.
- **Sesión y roles:** todas las páginas heredan de `PaginaBase`, que exige sesión iniciada y el rol permitido. El usuario de inscripciones y reseñas sale de la sesión, nunca de un campo oculto ni de la URL.
- **Control de cupos concurrente:** la inscripción se hace en una `SqlTransaction` que bloquea la fila de la actividad (`UPDLOCK`), así dos estudiantes no pueden tomar el último cupo al mismo tiempo.
- **Anti-XSS:** los mensajes se muestran con `Server.HtmlEncode()`, los `GridView` usan `BoundField` y los `Repeater` usan `<%#: %>`. Un comentario con `<script>` se muestra como texto.
- **Separación de capas:** ni la Vista ni la BLL contienen SQL; ni la BLL, ni la DAL, ni Entities referencian `System.Web`.
- **Errores controlados:** se registran con `Trace` y al usuario se le muestra un mensaje amigable o la página `Error.html`.
- **Defensa en la base de datos:** PK, FK, `CHECK` (cupos, fechas, horas, estados, calificación 1–5) y `UNIQUE` (una inscripción y una reseña por estudiante y actividad).
- `Web.config` no contiene contraseñas (Integrated Security) y las cookies son `HttpOnly`.

---

## Avance del proyecto

### Avance 1 (9 de noviembre de 2026)
- [x] Solución con 4 proyectos y referencias entre capas
- [x] Cadena de conexión `UniCampusDb` en `Web.config`
- [x] Script `03_BaseDatos.sql` con PK, FK, CHECK, UNIQUE y datos de prueba
- [x] Registrar actividad con validaciones de servidor
- [x] Listar actividades publicadas (`GridView` con `EmptyDataText`)
- [x] Patrón MVP: `IActividadesView` + `ActividadesPresenter`

### Entrega final (23 de noviembre de 2026)
- [x] Ver detalle, editar y desactivar actividades (`ObtenerPorId`, `Actualizar`, `CambiarEstado`)
- [x] Búsqueda por texto y filtros por tipo, modalidad, fechas y cupos disponibles
- [x] Inscripciones con control de cupos transaccional (`SqlTransaction`)
- [x] Reseñas y valoraciones de 1 a 5 estrellas con protección anti-XSS
- [x] Usuario tomado de la sesión autenticada, con roles
- [x] Reportes de actividades, ocupación y valoraciones
- [x] Manual de instalación y matriz de pruebas (en el informe técnico)

---

## Integrantes

| Nombre | Cuenta de GitHub | Aporte |
| :--- | :--- | :--- |
| Cristian Camilo Rodríguez Lugo | [Cristian-rl](https://github.com/Cristian-rl) | Login con sesión y roles, página maestra, script de base de datos v2, reseñas y valoraciones, documentación final |
| Rafael Illera Luna | [rafaillera](https://github.com/rafaillera) | Arquitectura del Avance 1; gestión de actividades: ver detalle, editar, desactivar y publicar |
| Alber Palacio Díaz | [adbe58](https://github.com/adbe58) | Catálogo con búsqueda y filtros, y página de reportes |
| Samuel Balaguera | [samuelbalaguera7](https://github.com/samuelbalaguera7) | Detalle de actividad, inscripciones con transacción y "Mis inscripciones" |

Repositorio: [github.com/rafaillera/UniCampus](https://github.com/rafaillera/UniCampus)
