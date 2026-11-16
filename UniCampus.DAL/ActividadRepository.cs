using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class ActividadRepository
    {
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

        public List<ActividadDto> ObtenerTodas()
        {
            const string sql = ActividadSql.SelectBase + @"
            ORDER BY a.FechaInicio DESC, a.Titulo;";

            var actividades = new List<ActividadDto>();

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
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

        public bool Actualizar(ActividadDto actividad)
        {
            if (actividad == null) throw new ArgumentNullException(nameof(actividad));

            const string sql = @"
            UPDATE dbo.ActividadAcademica
            SET Titulo      = @Titulo,
                Descripcion = @Descripcion,
                TipoId      = @TipoId,
                FechaInicio = @FechaInicio,
                FechaFin    = @FechaFin,
                HoraInicio  = @HoraInicio,
                HoraFin     = @HoraFin,
                Modalidad   = @Modalidad,
                Lugar       = @Lugar,
                Cupos       = @Cupos
            WHERE ActividadId = @ActividadId
              AND @Cupos >= (SELECT COUNT(1) FROM dbo.Inscripcion i
                             WHERE i.ActividadId = @ActividadId AND i.Estado <> N'Cancelada');";

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividad.ActividadId;
                cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 150).Value = actividad.Titulo;
                cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 1000).Value = ActividadSql.ValorONulo(actividad.Descripcion);
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = actividad.TipoId;
                cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = actividad.FechaInicio.Date;
                cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = actividad.FechaFin.Date;
                cmd.Parameters.Add("@HoraInicio", SqlDbType.Time).Value = ActividadSql.ValorONulo(actividad.HoraInicio);
                cmd.Parameters.Add("@HoraFin", SqlDbType.Time).Value = ActividadSql.ValorONulo(actividad.HoraFin);
                cmd.Parameters.Add("@Modalidad", SqlDbType.NVarChar, 20).Value = actividad.Modalidad;
                cmd.Parameters.Add("@Lugar", SqlDbType.NVarChar, 150).Value = actividad.Lugar;
                cmd.Parameters.Add("@Cupos", SqlDbType.Int).Value = actividad.Cupos;

                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public bool CambiarEstado(int actividadId, string nuevoEstado)
        {
            const string sql = @"
            UPDATE dbo.ActividadAcademica
            SET Estado = @Estado
            WHERE ActividadId = @ActividadId;";

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = nuevoEstado;

                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }
    }
}