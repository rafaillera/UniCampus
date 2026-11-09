# UniCampus — Gestión de Actividades Académicas

Proyecto de la asignatura **Desarrollo de Aplicaciones Web II** (Ing. Cindy Fuentes).

Aplicación web en **ASP.NET Web Forms (.NET Framework 4.8.1)** con arquitectura de **tres capas** (`Web → BLL → DAL` + `Entities`) y patrón **MVP** (Model-View-Presenter), conectada a **SQL Server** mediante **ADO.NET**.

> **Versión actual:** Avance 1 (`v1.0-avance1`): registrar y listar actividades académicas con validaciones en servidor.

---

## Estructura de la solución

```
UniCampus.sln
├── UniCampus.Entities   DTOs: ActividadDto, TipoActividadDto, ResultadoOperacion, catálogos
├── UniCampus.DAL        Repositorios ADO.NET (SqlConnection, SqlCommand, SqlParameter)
├── UniCampus.BLL        Servicios con reglas de negocio y validaciones
├── UniCampus.Web        Vista (.aspx), contratos (Views/) y Presenters (Presenters/)
└── BaseDatos/03_BaseDatos.sql
```

| Proyecto | Tipo | Referencias |
| :--- | :--- | :--- |
| `UniCampus.Entities` | Biblioteca de clases | Ninguna |
| `UniCampus.DAL` | Biblioteca de clases | `UniCampus.Entities`, `System.Configuration`, `System.Data` |
| `UniCampus.BLL` | Biblioteca de clases | `UniCampus.DAL`, `UniCampus.Entities` |
| `UniCampus.Web` | Aplicación web ASP.NET Web Forms | `UniCampus.BLL`, `UniCampus.Entities` |

**Flujo de una petición:** `Actividades.aspx.cs` → `ActividadesPresenter` → `ActividadService` (BLL) → `ActividadRepository` (DAL) → SQL Server.

---

## Requisitos

- Visual Studio 2022 con la carga de trabajo **Desarrollo de ASP.NET y web**
- **.NET Framework 4.8.1 Developer Pack**
- SQL Server 2016 o superior (Express sirve) y SQL Server Management Studio (SSMS)

## Instalación y configuración

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/<usuario>/<repositorio>.git
   ```
2. **Crear la base de datos:** abrir `BaseDatos/03_BaseDatos.sql` en SSMS y ejecutarlo (F5). Crea `UniCampusDB`, las tablas, las restricciones y los datos de prueba.
3. **Revisar la cadena de conexión** en `UniCampus.Web/Web.config`:
   ```xml
   <add name="UniCampusDb"
        connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=UniCampusDB;Integrated Security=True;TrustServerCertificate=True"
        providerName="System.Data.SqlClient" />
   ```
   Si tu instancia no es `.\SQLEXPRESS`, cambia `Data Source` (por ejemplo `(localdb)\MSSQLLocalDB` o `.`). Se usa autenticación de Windows, así que no se guarda ninguna contraseña.
4. **Abrir `UniCampus.sln`**, clic derecho sobre `UniCampus.Web` → *Establecer como proyecto de inicio*.
5. **Compilar** (Ctrl+Shift+B) y **ejecutar** (F5). Se abre `Actividades.aspx`.

---

## Reglas de seguridad aplicadas

- Todas las consultas usan `SqlParameter` tipados (`SqlDbType`): no se concatena texto del usuario en SQL.
- Ni la Vista (`.aspx.cs`) ni la BLL contienen `SqlConnection` ni SQL.
- Ni la BLL, ni la DAL, ni Entities referencian `System.Web`.
- Los mensajes se muestran con `Server.HtmlEncode()`; el `GridView` usa `BoundField` (codifica HTML por defecto).
- Los errores técnicos se registran con `Trace` y al usuario solo se le muestra un mensaje amigable.
- `Web.config` no contiene contraseñas (Integrated Security).

## Validaciones de servidor (BLL)

| Campo | Regla |
| :--- | :--- |
| Título | Obligatorio, máximo 150 caracteres |
| Descripción | Opcional, máximo 1000 caracteres |
| Tipo | Obligatorio y existente en `TipoActividad` |
| Fecha de inicio | Obligatoria, no anterior a hoy |
| Fecha de fin | Obligatoria, no anterior a la fecha de inicio |
| Horas | Si la actividad dura un día, la hora fin debe ser posterior a la de inicio |
| Modalidad | Presencial, Virtual o Híbrida |
| Lugar | Obligatorio, máximo 150 caracteres |
| Cupos | Entero mayor que 0 (máximo 1000) |
| Estado | Borrador o Publicada (por defecto Publicada) |

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
- [ ] Ver detalle, editar y desactivar actividades (`ObtenerPorId`, `Actualizar`, `CambiarEstado`)
- [ ] Búsqueda por texto y filtros por tipo, fecha y estado
- [ ] Inscripciones con control de cupos transaccional (`SqlTransaction`)
- [ ] Reseñas y valoraciones de 1 a 5 estrellas con protección anti-XSS
- [ ] Usuario tomado de la sesión autenticada
- [ ] Matriz completa de pruebas y manual de instalación final

---

## Git: comandos de la entrega

```bash
git init
git add .
git commit -m "Avance 1: arquitectura 3 capas + MVP, registro y listado de actividades"
git branch -M main
git remote add origin https://github.com/<usuario>/<repositorio>.git
git push -u origin main

# Marcar el Avance 1 y seguir trabajando en la versión final
git tag v1.0-avance1
git push origin v1.0-avance1
git checkout -b desarrollo-final
```

## Integrantes

| Nombre | Rol / aporte |
| :--- | :--- |
| _Integrante 1_ | |
| _Integrante 2_ | |
| _Integrante 3_ | |
