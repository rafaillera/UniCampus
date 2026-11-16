using System;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    /// <summary>
    /// SELECT base y mapeo de actividades, compartidos por todos los
    /// repositorios que devuelven ActividadDto (actividades, catálogo, reportes).
    /// Así la conversión fila → DTO está escrita una sola vez.
    /// </summary>
    internal static class ActividadSql
    {
        /// <summary>
        /// Columnas de la actividad + nombre del tipo + número de inscripciones vigentes.
        /// Se completa con WHERE / ORDER BY en cada consulta.
        /// </summary>
        public const string SelectBase = @"
            SELECT a.ActividadId, a.Titulo, a.Descripcion, a.TipoId, t.Nombre AS TipoNombre,
                   a.FechaInicio, a.FechaFin, a.HoraInicio, a.HoraFin,
                   a.Modalidad, a.Lugar, a.Cupos, a.Estado,
                   (SELECT COUNT(1) FROM dbo.Inscripcion i
                    WHERE i.ActividadId = a.ActividadId AND i.Estado <> N'Cancelada') AS Inscritos
            FROM dbo.ActividadAcademica a
            INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId";

        public static ActividadDto Mapear(SqlDataReader dr)
        {
            return new ActividadDto
            {
                ActividadId = dr.GetInt32(dr.GetOrdinal("ActividadId")),
                Titulo = dr.GetString(dr.GetOrdinal("Titulo")),
                Descripcion = LeerTexto(dr, "Descripcion"),
                TipoId = dr.GetInt32(dr.GetOrdinal("TipoId")),
                TipoNombre = dr.GetString(dr.GetOrdinal("TipoNombre")),
                FechaInicio = dr.GetDateTime(dr.GetOrdinal("FechaInicio")),
                FechaFin = dr.GetDateTime(dr.GetOrdinal("FechaFin")),
                HoraInicio = LeerHora(dr, "HoraInicio"),
                HoraFin = LeerHora(dr, "HoraFin"),
                Modalidad = dr.GetString(dr.GetOrdinal("Modalidad")),
                Lugar = dr.GetString(dr.GetOrdinal("Lugar")),
                Cupos = dr.GetInt32(dr.GetOrdinal("Cupos")),
                Estado = dr.GetString(dr.GetOrdinal("Estado")),
                Inscritos = dr.GetInt32(dr.GetOrdinal("Inscritos"))
            };
        }

        public static string LeerTexto(SqlDataReader dr, string columna)
        {
            int i = dr.GetOrdinal(columna);
            return dr.IsDBNull(i) ? null : dr.GetString(i);
        }

        public static TimeSpan? LeerHora(SqlDataReader dr, string columna)
        {
            int i = dr.GetOrdinal(columna);
            return dr.IsDBNull(i) ? (TimeSpan?)null : dr.GetTimeSpan(i);
        }

        public static object ValorONulo(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor;
        }

        public static object ValorONulo(TimeSpan? valor)
        {
            return valor.HasValue ? (object)valor.Value : DBNull.Value;
        }
    }
}
