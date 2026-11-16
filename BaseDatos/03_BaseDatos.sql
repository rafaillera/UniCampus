/* =============================================================================
   UniCampus - Script de Base de Datos
   Versión: Entrega final (v2.0)
   Asignatura: Desarrollo de Aplicaciones Web II
   Motor: SQL Server 2016 o superior (usa DROP TABLE IF EXISTS)

   Contenido:
     1. Creación de la base de datos UniCampusDB (si no existe)
     2. Tablas: TipoActividad, Usuario, ActividadAcademica, Inscripcion, Resena
     3. Restricciones: PK, FK, UNIQUE, CHECK y DEFAULT
      4. Datos de prueba (las fechas se calculan a partir de HOY para que la
        demostración funcione el día en que se ejecute el script)

   Usuarios de prueba (todos con la misma contraseña de prueba, ver @ClaveDemo):
     admin@unicampus.edu.co             Administrador
     carlos.perez@unicampus.edu.co      Organizador
     laura.martinez@unicampus.edu.co    Estudiante
     andres.gomez@unicampus.edu.co      Estudiante
     valeria.rodriguez@unicampus.edu.co Estudiante
     sofia.ramirez@unicampus.edu.co     Estudiante

   IMPORTANTE: el script elimina y vuelve a crear las tablas para que sea
   reproducible desde cero. No lo ejecute sobre una base con datos reales.
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
DROP TABLE IF EXISTS dbo.Resena;
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
      - La contraseña NUNCA se guarda en texto plano: se guarda el hash
        SHA-256 de (ClaveSalt + contraseña). ClaveSalt es aleatorio por usuario.
      - Un usuario con ClaveHash NULL no puede iniciar sesión.
   ----------------------------------------------------------------------------- */
CREATE TABLE dbo.Usuario
(
    UsuarioId     INT IDENTITY(1,1) NOT NULL,
    Nombres       NVARCHAR(80)      NOT NULL,
    Apellidos     NVARCHAR(80)      NOT NULL,
    Correo        NVARCHAR(150)     NOT NULL,
    Programa      NVARCHAR(120)     NULL,
    Rol           NVARCHAR(20)      NOT NULL CONSTRAINT DF_Usuario_Rol DEFAULT (N'Estudiante'),
    ClaveSalt     NVARCHAR(36)      NOT NULL CONSTRAINT DF_Usuario_ClaveSalt DEFAULT (CONVERT(NVARCHAR(36), NEWID())),
    ClaveHash     VARBINARY(32)     NULL,
    FechaRegistro DATETIME2(0)      NOT NULL CONSTRAINT DF_Usuario_FechaRegistro DEFAULT (SYSDATETIME()),
    Activo        BIT               NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1),

    CONSTRAINT PK_Usuario PRIMARY KEY (UsuarioId),
    CONSTRAINT UQ_Usuario_Correo UNIQUE (Correo),
    CONSTRAINT CK_Usuario_Rol CHECK (Rol IN (N'Estudiante', N'Organizador', N'Administrador'))
);
GO

/* -----------------------------------------------------------------------------
   3. ActividadAcademica: actividades publicadas por la universidad
      - Cupos: capacidad TOTAL de la actividad (nunca negativa).
        Los cupos disponibles se calculan restando las inscripciones vigentes.
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
   4. Inscripcion: relación estudiante - actividad (N a N)
      UQ_Inscripcion_Actividad_Usuario impide la doble inscripción desde la BD.
      Si un estudiante cancela y se vuelve a inscribir, se reactiva la misma fila.
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

/* -----------------------------------------------------------------------------
   5. Resena: valoración (1 a 5 estrellas) y comentario de un estudiante
      - Una sola reseña por estudiante y actividad (UQ_Resena_Actividad_Usuario).
      - La regla "solo estudiantes inscritos" se valida en la capa BLL.
   ----------------------------------------------------------------------------- */
CREATE TABLE dbo.Resena
(
    ResenaId      INT IDENTITY(1,1) NOT NULL,
    ActividadId   INT               NOT NULL,
    UsuarioId     INT               NOT NULL,
    Calificacion  TINYINT           NOT NULL,
    Comentario    NVARCHAR(1000)    NULL,
    FechaCreacion DATETIME2(0)      NOT NULL CONSTRAINT DF_Resena_Fecha DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Resena PRIMARY KEY (ResenaId),
    CONSTRAINT FK_Resena_Actividad FOREIGN KEY (ActividadId)
        REFERENCES dbo.ActividadAcademica (ActividadId),
    CONSTRAINT FK_Resena_Usuario FOREIGN KEY (UsuarioId)
        REFERENCES dbo.Usuario (UsuarioId),
    CONSTRAINT UQ_Resena_Actividad_Usuario UNIQUE (ActividadId, UsuarioId),
    CONSTRAINT CK_Resena_Calificacion CHECK (Calificacion BETWEEN 1 AND 5)
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

/* Usuarios: el salt se genera con el DEFAULT y luego se calcula el hash.
   HASHBYTES sobre NVARCHAR usa UTF-16LE, igual que Encoding.Unicode en C#. */
DECLARE @ClaveDemo NVARCHAR(50) = N'UniCampus2026*';

INSERT INTO dbo.Usuario (Nombres, Apellidos, Correo, Programa, Rol) VALUES
    (N'Ana',     N'Suárez',    N'admin@unicampus.edu.co',             NULL,                         N'Administrador'),
    (N'Carlos',  N'Pérez',     N'carlos.perez@unicampus.edu.co',      NULL,                         N'Organizador'),
    (N'Laura',   N'Martínez',  N'laura.martinez@unicampus.edu.co',    N'Ingeniería de Sistemas',    N'Estudiante'),
    (N'Andrés',  N'Gómez',     N'andres.gomez@unicampus.edu.co',      N'Ingeniería de Sistemas',    N'Estudiante'),
    (N'Valeria', N'Rodríguez', N'valeria.rodriguez@unicampus.edu.co', N'Administración de Empresas', N'Estudiante'),
    (N'Sofía',   N'Ramírez',   N'sofia.ramirez@unicampus.edu.co',     N'Diseño Gráfico',            N'Estudiante');

UPDATE dbo.Usuario
SET ClaveHash = HASHBYTES('SHA2_256', ClaveSalt + @ClaveDemo);
GO

/* Actividades: fechas relativas a hoy para que siempre haya actividades
   próximas, una en curso y otras ya realizadas (para probar reseñas). */
DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

INSERT INTO dbo.ActividadAcademica
    (Titulo, Descripcion, TipoId, FechaInicio, FechaFin, HoraInicio, HoraFin, Modalidad, Lugar, Cupos, Estado)
VALUES
    /* 1 */ (N'Introducción a ASP.NET Web Forms',
     N'Primeros pasos con páginas .aspx, controles de servidor y ciclo de vida de la página.',
     1, DATEADD(DAY, 4, @Hoy), DATEADD(DAY, 4, @Hoy), '14:00', '17:00', N'Presencial', N'Laboratorio de Sistemas 2', 25, N'Publicada'),

    /* 2 */ (N'Conferencia: Seguridad en aplicaciones web',
     N'Inyección SQL, XSS y buenas prácticas de validación en servidor.',
     2, DATEADD(DAY, 11, @Hoy), DATEADD(DAY, 11, @Hoy), '10:00', '12:00', N'Híbrida', N'Auditorio Principal', 120, N'Publicada'),

    /* 3 */ (N'Seminario de bases de datos relacionales',
     N'Modelado entidad-relación, normalización y restricciones de integridad.',
     3, DATEADD(DAY, 17, @Hoy), DATEADD(DAY, 18, @Hoy), '08:00', '12:00', N'Virtual', N'Microsoft Teams', 40, N'Publicada'),

    /* 4 */ (N'Curso corto de Git y GitHub',
     N'Actividad en preparación: aún no visible para los estudiantes.',
     4, DATEADD(DAY, 24, @Hoy), DATEADD(DAY, 26, @Hoy), '15:00', '18:00', N'Presencial', N'Sala 305', 20, N'Borrador'),

    /* 5 */ (N'Taller de diseño UX/UI',
     N'Prototipado rápido y pruebas de usabilidad. Solo 2 cupos para probar el control de cupos.',
     1, DATEADD(DAY, 6, @Hoy), DATEADD(DAY, 6, @Hoy), '09:00', '11:00', N'Presencial', N'Sala de diseño', 2, N'Publicada'),

    /* 6 */ (N'Conferencia de Inteligencia Artificial',
     N'Aplicaciones de la IA en la educación superior.',
     2, DATEADD(DAY, -10, @Hoy), DATEADD(DAY, -10, @Hoy), '16:00', '18:00', N'Presencial', N'Auditorio Principal', 100, N'Finalizada'),

    /* 7 */ (N'Encuentro de semilleros de investigación',
     N'Socialización de proyectos de los semilleros de la facultad.',
     3, DATEADD(DAY, -3, @Hoy), DATEADD(DAY, -2, @Hoy), '08:00', '17:00', N'Híbrida', N'Bloque C', 80, N'Publicada'),

    /* 8 */ (N'Taller de Excel avanzado',
     N'Actividad cancelada por el organizador.',
     1, DATEADD(DAY, 9, @Hoy), DATEADD(DAY, 9, @Hoy), '14:00', '16:00', N'Virtual', N'Zoom', 30, N'Cancelada');
GO

/* UsuarioId: 1 admin, 2 organizador, 3 Laura, 4 Andrés, 5 Valeria, 6 Sofía */
INSERT INTO dbo.Inscripcion (ActividadId, UsuarioId, Estado) VALUES
    (1, 3, N'Activa'),
    (1, 4, N'Activa'),
    (2, 5, N'Activa'),
    (5, 3, N'Activa'),   -- la actividad 5 queda sin cupos (2 de 2)
    (5, 5, N'Activa'),
    (6, 3, N'Asistió'),
    (6, 4, N'Asistió'),
    (6, 6, N'Asistió'),
    (7, 4, N'Activa'),
    (7, 6, N'Cancelada');
GO

INSERT INTO dbo.Resena (ActividadId, UsuarioId, Calificacion, Comentario) VALUES
    (6, 3, 5, N'Excelente conferencia, muy bien explicada.'),
    (6, 4, 4, N'Buen contenido, faltó tiempo para preguntas.');
GO

/* =============================================================================
   CONSULTAS DE VERIFICACIÓN (para las capturas de pruebas en SSMS)
   ============================================================================= */

-- Actividades con cupos disponibles
SELECT a.ActividadId, a.Titulo, t.Nombre AS Tipo, a.FechaInicio, a.Estado, a.Cupos,
       (SELECT COUNT(1) FROM dbo.Inscripcion i
        WHERE i.ActividadId = a.ActividadId AND i.Estado <> N'Cancelada') AS Inscritos
FROM dbo.ActividadAcademica a
INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId
ORDER BY a.FechaInicio;

-- Usuarios (sin mostrar el hash)
SELECT UsuarioId, Nombres, Apellidos, Correo, Rol, Activo FROM dbo.Usuario;

-- Reseñas
SELECT r.ResenaId, a.Titulo, u.Nombres, r.Calificacion, r.Comentario
FROM dbo.Resena r
INNER JOIN dbo.ActividadAcademica a ON a.ActividadId = r.ActividadId
INNER JOIN dbo.Usuario u ON u.UsuarioId = r.UsuarioId;
GO
