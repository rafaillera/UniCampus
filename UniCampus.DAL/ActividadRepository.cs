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
        /// <summary>
        /// Devuelve las actividades con estado 'Publicada', ordenadas por fecha de inicio.
        /// </summary>
        public List<ActividadDto> ObtenerPublicadas()
        {
            const string sql = ActividadSql.SelectBase + @"
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
                        actividades.Add(ActividadSql.Mapear(dr));
                    }
                }
            }

            return actividades;
        }

        /// <summary>
        /// Devuelve una actividad por su Id (cualquier estado) o null si no existe.
        /// </summary>
        public ActividadDto ObtenerPorId(int actividadId)
        {
            const string sql = ActividadSql.SelectBase + @"
            WHERE a.ActividadId = @ActividadId;";

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read() ? ActividadSql.Mapear(dr) : null;
                }
            }
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
                cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 1000).Value = ActividadSql.ValorONulo(nuevaActividad.Descripcion);
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = nuevaActividad.TipoId;
                cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = nuevaActividad.FechaInicio.Date;
                cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = nuevaActividad.FechaFin.Date;
                cmd.Parameters.Add("@HoraInicio", SqlDbType.Time).Value = ActividadSql.ValorONulo(nuevaActividad.HoraInicio);
                cmd.Parameters.Add("@HoraFin", SqlDbType.Time).Value = ActividadSql.ValorONulo(nuevaActividad.HoraFin);
                cmd.Parameters.Add("@Modalidad", SqlDbType.NVarChar, 20).Value = nuevaActividad.Modalidad;
                cmd.Parameters.Add("@Lugar", SqlDbType.NVarChar, 150).Value = nuevaActividad.Lugar;
                cmd.Parameters.Add("@Cupos", SqlDbType.Int).Value = nuevaActividad.Cupos;
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = nuevaActividad.Estado;

                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
