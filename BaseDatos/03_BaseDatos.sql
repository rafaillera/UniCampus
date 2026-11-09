/* =============================================================================
   UniCampus - Script de Base de Datos
   Versión: Avance 1 (v1.0)
   Asignatura: Desarrollo de Aplicaciones Web II
   Motor: SQL Server 2016 o superior (usa DROP TABLE IF EXISTS)

   Contenido:
     1. Creación de la base de datos UniCampusDB (si no existe)
     2. Tablas: TipoActividad, Usuario, ActividadAcademica, Inscripcion
     3. Restricciones: PK, FK, UNIQUE, CHECK y DEFAULT
     4. Datos de prueba

   IMPORTANTE: el script elimina y vuelve a crear las tablas para que sea
   reproducible desde cero. No lo ejecute sobre una base con datos reales.
   Si su equipo ya tiene la base aprobada, ajuste los nombres de tablas y
   columnas a los de su DER antes de ejecutarlo.
   ============================================================================= */

IF DB_ID(N'UniCampusDB') IS NULL
BEGIN
    CREATE DATABASE UniCampusDB;
END
GO

USE UniCampusDB;
GO

SET NOCOUNT ON;
GO

/* -----------------------------------------------------------------------------
   Limpieza (orden inverso a las dependencias de claves foráneas)
   ----------------------------------------------------------------------------- */
DROP TABLE IF EXISTS dbo.Inscripcion;
DROP TABLE IF EXISTS dbo.ActividadAcademica;
DROP TABLE IF EXISTS dbo.Usuario;
DROP TABLE IF EXISTS dbo.TipoActividad;
GO

/* -----------------------------------------------------------------------------
   1. TipoActividad: catálogo de tipos (Taller, Conferencia, Seminario, ...)
   ----------------------------------------------------------------------------- */
CREATE TABLE dbo.TipoActividad
(
    TipoId      INT IDENTITY(1,1) NOT NULL,
    Nombre      NVARCHAR(80)      NOT NULL,
    Descripcion NVARCHAR(250)     NULL,
    Activo      BIT               NOT NULL CONSTRAINT DF_TipoActividad_Activo DEFAULT (1),

    CONSTRAINT PK_TipoActividad PRIMARY KEY (TipoId),
    CONSTRAINT UQ_TipoActividad_Nombre UNIQUE (Nombre)
);
GO

/* -----------------------------------------------------------------------------
   2. Usuario: estudiantes, organizadores y administradores
   ----------------------------------------------------------------------------- */
CREATE TABLE dbo.Usuario
(
    UsuarioId     INT IDENTITY(1,1) NOT NULL,
    Nombres       NVARCHAR(80)      NOT NULL,
    Apellidos     NVARCHAR(80)      NOT NULL,
    Correo        NVARCHAR(150)     NOT NULL,
    Programa      NVARCHAR(120)     NULL,
    Rol           NVARCHAR(20)      NOT NULL CONSTRAINT DF_Usuario_Rol DEFAULT (N'Estudiante'),
    FechaRegistro DATETIME2(0)      NOT NULL CONSTRAINT DF_Usuario_FechaRegistro DEFAULT (SYSDATETIME()),
    Activo        BIT               NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1),

    CONSTRAINT PK_Usuario PRIMARY KEY (UsuarioId),
    CONSTRAINT UQ_Usuario_Correo UNIQUE (Correo),
    CONSTRAINT CK_Usuario_Rol CHECK (Rol IN (N'Estudiante', N'Organizador', N'Administrador'))
);
GO

/* -----------------------------------------------------------------------------
   3. ActividadAcademica: actividades publicadas por la universidad
      - Cupos: capacidad total de la actividad (nunca negativa)
      - Estado: 'Publicada' por defecto
   ----------------------------------------------------------------------------- */
CREATE TABLE dbo.ActividadAcademica
(
    ActividadId   INT IDENTITY(1,1) NOT NULL,
    Titulo        NVARCHAR(150)     NOT NULL,
    Descripcion   NVARCHAR(1000)    NULL,
    TipoId        INT               NOT NULL,
    FechaInicio   DATE              NOT NULL,
    FechaFin      DATE              NOT NULL,
    HoraInicio    TIME(0)           NULL,
    HoraFin       TIME(0)           NULL,
    Modalidad     NVARCHAR(20)      NOT NULL CONSTRAINT DF_Actividad_Modalidad DEFAULT (N'Presencial'),
    Lugar         NVARCHAR(150)     NOT NULL,
    Cupos         INT               NOT NULL,
    Estado        NVARCHAR(20)      NOT NULL CONSTRAINT DF_Actividad_Estado DEFAULT (N'Publicada'),
    FechaCreacion DATETIME2(0)      NOT NULL CONSTRAINT DF_Actividad_FechaCreacion DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_ActividadAcademica PRIMARY KEY (ActividadId),
    CONSTRAINT FK_Actividad_TipoActividad FOREIGN KEY (TipoId)
        REFERENCES dbo.TipoActividad (TipoId),
    CONSTRAINT CK_Actividad_Cupos CHECK (Cupos >= 0),
    CONSTRAINT CK_Actividad_Titulo CHECK (LEN(LTRIM(RTRIM(Titulo))) > 0),
    CONSTRAINT CK_Actividad_Fechas CHECK (FechaFin >= FechaInicio),
    CONSTRAINT CK_Actividad_Horas CHECK (
        HoraInicio IS NULL OR HoraFin IS NULL
        OR FechaFin > FechaInicio
        OR HoraFin > HoraInicio),
    CONSTRAINT CK_Actividad_Modalidad CHECK (Modalidad IN (N'Presencial', N'Virtual', N'Híbrida')),
    CONSTRAINT CK_Actividad_Estado CHECK (Estado IN (N'Borrador', N'Publicada', N'Cancelada', N'Finalizada', N'Inactiva'))
);
GO

CREATE INDEX IX_Actividad_Estado_Fecha ON dbo.ActividadAcademica (Estado, FechaInicio);
GO

/* -----------------------------------------------------------------------------
   4. Inscripcion: relación estudiante - actividad
      UQ_Inscripcion_Actividad_Usuario impide la doble inscripción desde la BD.
      (El módulo de inscripciones se implementa en la entrega final.)
   ----------------------------------------------------------------------------- */
CREATE TABLE dbo.Inscripcion
(
    InscripcionId    INT IDENTITY(1,1) NOT NULL,
    ActividadId      INT               NOT NULL,
    UsuarioId        INT               NOT NULL,
    FechaInscripcion DATETIME2(0)      NOT NULL CONSTRAINT DF_Inscripcion_Fecha DEFAULT (SYSDATETIME()),
    Estado           NVARCHAR(20)      NOT NULL CONSTRAINT DF_Inscripcion_Estado DEFAULT (N'Activa'),

    CONSTRAINT PK_Inscripcion PRIMARY KEY (InscripcionId),
    CONSTRAINT FK_Inscripcion_Actividad FOREIGN KEY (ActividadId)
        REFERENCES dbo.ActividadAcademica (ActividadId),
    CONSTRAINT FK_Inscripcion_Usuario FOREIGN KEY (UsuarioId)
        REFERENCES dbo.Usuario (UsuarioId),
    CONSTRAINT UQ_Inscripcion_Actividad_Usuario UNIQUE (ActividadId, UsuarioId),
    CONSTRAINT CK_Inscripcion_Estado CHECK (Estado IN (N'Activa', N'Cancelada', N'Asistió'))
);
GO

/* =============================================================================
   DATOS DE PRUEBA
   ============================================================================= */

INSERT INTO dbo.TipoActividad (Nombre, Descripcion) VALUES
    (N'Taller',      N'Sesión práctica guiada con ejercicios'),
    (N'Conferencia', N'Charla magistral de un invitado'),
    (N'Seminario',   N'Espacio de discusión académica'),
    (N'Curso corto', N'Formación de varias sesiones');
GO

INSERT INTO dbo.Usuario (Nombres, Apellidos, Correo, Programa, Rol) VALUES
    (N'Carlos',  N'Pérez',     N'carlos.perez@unicampus.edu.co',    NULL,                         N'Organizador'),
    (N'Laura',   N'Martínez',  N'laura.martinez@unicampus.edu.co',  N'Ingeniería de Sistemas',    N'Estudiante'),
    (N'Andrés',  N'Gómez',     N'andres.gomez@unicampus.edu.co',    N'Ingeniería de Sistemas',    N'Estudiante'),
    (N'Valeria', N'Rodríguez', N'valeria.rodriguez@unicampus.edu.co', N'Administración de Empresas', N'Estudiante');
GO

INSERT INTO dbo.ActividadAcademica
    (Titulo, Descripcion, TipoId, FechaInicio, FechaFin, HoraInicio, HoraFin, Modalidad, Lugar, Cupos, Estado)
VALUES
    (N'Introducción a ASP.NET Web Forms',
     N'Primeros pasos con páginas .aspx, controles de servidor y ciclo de vida de la página.',
     1, '2026-11-20', '2026-11-20', '14:00', '17:00', N'Presencial', N'Laboratorio de Sistemas 2', 25, N'Publicada'),

    (N'Conferencia: Seguridad en aplicaciones web',
     N'Inyección SQL, XSS y buenas prácticas de validación en servidor.',
     2, '2026-11-27', '2026-11-27', '10:00', '12:00', N'Híbrida', N'Auditorio Principal', 120, N'Publicada'),

    (N'Seminario de bases de datos relacionales',
     N'Modelado entidad-relación, normalización y restricciones de integridad.',
     3, '2026-12-03', '2026-12-04', '08:00', '12:00', N'Virtual', N'Microsoft Teams', 40, N'Publicada'),

    (N'Curso corto de Git y GitHub',
     N'Actividad en preparación: aún no visible para los estudiantes.',
     4, '2026-12-10', '2026-12-12', '15:00', '18:00', N'Presencial', N'Sala 305', 20, N'Borrador');
GO

INSERT INTO dbo.Inscripcion (ActividadId, UsuarioId) VALUES
    (1, 2),
    (1, 3),
    (2, 4);
GO

/* =============================================================================
   CONSULTAS DE VERIFICACIÓN (para las capturas de pruebas en SSMS)
   ============================================================================= */

-- Actividades publicadas (lo que muestra el GridView)
SELECT a.ActividadId, a.Titulo, t.Nombre AS Tipo, a.FechaInicio, a.Lugar, a.Cupos, a.Estado
FROM dbo.ActividadAcademica a
INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId
WHERE a.Estado = N'Publicada'
ORDER BY a.FechaInicio;

-- Todas las actividades (incluye borradores)
SELECT * FROM dbo.ActividadAcademica ORDER BY ActividadId;

/* Prueba 4 (consulta sin datos): para ver el mensaje EmptyDataText del GridView
   ejecute temporalmente:
       UPDATE dbo.ActividadAcademica SET Estado = N'Borrador';
   y luego restaure:
       UPDATE dbo.ActividadAcademica SET Estado = N'Publicada' WHERE ActividadId IN (1, 2, 3);
*/
GO
