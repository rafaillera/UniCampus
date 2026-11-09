using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    /// <summary>
    /// Acceso a datos de dbo.ActividadAcademica con ADO.NET.
    /// Todas las consultas usan SqlParameter tipados: nunca se concatenan
    /// valores ingresados por el usuario dentro del texto SQL.
    /// </summary>
    public class ActividadRepository
    {
        private const string SqlSelectBase = @"
            SELECT a.ActividadId, a.Titulo, a.Descripcion, a.TipoId, t.Nombre AS TipoNombre,
                   a.FechaInicio, a.FechaFin, a.HoraInicio, a.HoraFin,
                   a.Modalidad, a.Lugar, a.Cupos, a.Estado
            FROM dbo.ActividadAcademica a
            INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId";

        /// <summary>
        /// Devuelve las actividades con estado 'Publicada', ordenadas por fecha de inicio.
        /// </summary>
        public List<ActividadDto> ObtenerPublicadas()
        {
            const string sql = SqlSelectBase + @"
            WHERE a.Estado = @Estado
            ORDER BY a.FechaInicio, a.HoraInicio, a.Titulo;";

            var actividades = new List<ActividadDto>();

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = EstadosActividad.Publicada;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        actividades.Add(Mapear(dr));
                    }
                }
            }

            return actividades;
        }

        /// <summary>
        /// Inserta una nueva actividad y devuelve el ActividadId generado (IDENTITY).
        /// </summary>
        public int Insertar(ActividadDto nuevaActividad)
        {
            if (nuevaActividad == null) throw new ArgumentNullException(nameof(nuevaActividad));

            const string sql = @"
            INSERT INTO dbo.ActividadAcademica
                (Titulo, Descripcion, TipoId, FechaInicio, FechaFin, HoraInicio, HoraFin,
                 Modalidad, Lugar, Cupos, Estado)
            OUTPUT INSERTED.ActividadId
            VALUES
                (@Titulo, @Descripcion, @TipoId, @FechaInicio, @FechaFin, @HoraInicio, @HoraFin,
                 @Modalidad, @Lugar, @Cupos, @Estado);";

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 150).Value = nuevaActividad.Titulo;
                cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 1000).Value = ValorONulo(nuevaActividad.Descripcion);
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = nuevaActividad.TipoId;
                cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = nuevaActividad.FechaInicio.Date;
                cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = nuevaActividad.FechaFin.Date;
                cmd.Parameters.Add("@HoraInicio", SqlDbType.Time).Value = ValorONulo(nuevaActividad.HoraInicio);
                cmd.Parameters.Add("@HoraFin", SqlDbType.Time).Value = ValorONulo(nuevaActividad.HoraFin);
                cmd.Parameters.Add("@Modalidad", SqlDbType.NVarChar, 20).Value = nuevaActividad.Modalidad;
                cmd.Parameters.Add("@Lugar", SqlDbType.NVarChar, 150).Value = nuevaActividad.Lugar;
                cmd.Parameters.Add("@Cupos", SqlDbType.Int).Value = nuevaActividad.Cupos;
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = nuevaActividad.Estado;

                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        // ---------------------------------------------------------------------
        // Métodos auxiliares
        // ---------------------------------------------------------------------

        private static ActividadDto Mapear(SqlDataReader dr)
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
                Estado = dr.GetString(dr.GetOrdinal("Estado"))
            };
        }

        private static string LeerTexto(SqlDataReader dr, string columna)
        {
            int i = dr.GetOrdinal(columna);
            return dr.IsDBNull(i) ? null : dr.GetString(i);
        }

        private static TimeSpan? LeerHora(SqlDataReader dr, string columna)
        {
            int i = dr.GetOrdinal(columna);
            return dr.IsDBNull(i) ? (TimeSpan?)null : dr.GetTimeSpan(i);
        }

        private static object ValorONulo(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor;
        }

        private static object ValorONulo(TimeSpan? valor)
        {
            return valor.HasValue ? (object)valor.Value : DBNull.Value;
        }
    }
}
