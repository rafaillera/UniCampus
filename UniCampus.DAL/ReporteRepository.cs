using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    /// <summary>
    /// Consultas de resumen (agregados) para la página de reportes.
    /// </summary>
    public class ReporteRepository
    {
        public ResumenActividadesDto ObtenerResumen(DateTime hoy)
        {
            const string sql = @"
            SELECT
                COUNT(1) AS TotalActividades,
                ISNULL(SUM(CASE WHEN a.Estado = N'Publicada' THEN 1 ELSE 0 END), 0) AS Publicadas,
                ISNULL(SUM(CASE WHEN a.Estado = N'Publicada' AND a.FechaInicio >= @Hoy THEN 1 ELSE 0 END), 0) AS Proximas,
                ISNULL(SUM(CASE WHEN a.Estado = N'Finalizada'
                                  OR (a.Estado = N'Publicada' AND a.FechaFin < @Hoy) THEN 1 ELSE 0 END), 0) AS Realizadas,
                ISNULL(SUM(CASE WHEN a.Estado IN (N'Inactiva', N'Cancelada') THEN 1 ELSE 0 END), 0) AS Inactivas,
                (SELECT COUNT(1) FROM dbo.Inscripcion i WHERE i.Estado <> N'Cancelada') AS Inscripciones,
                (SELECT AVG(CAST(r.Calificacion AS DECIMAL(4, 2))) FROM dbo.Resena r) AS PromedioCalificacion
            FROM dbo.ActividadAcademica a;";

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Hoy", SqlDbType.Date).Value = hoy.Date;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    dr.Read();
                    int iPromedio = dr.GetOrdinal("PromedioCalificacion");

                    return new ResumenActividadesDto
                    {
                        TotalActividades = dr.GetInt32(dr.GetOrdinal("TotalActividades")),
                        Publicadas = dr.GetInt32(dr.GetOrdinal("Publicadas")),
                        Proximas = dr.GetInt32(dr.GetOrdinal("Proximas")),
                        Realizadas = dr.GetInt32(dr.GetOrdinal("Realizadas")),
                        Inactivas = dr.GetInt32(dr.GetOrdinal("Inactivas")),
                        Inscripciones = dr.GetInt32(dr.GetOrdinal("Inscripciones")),
                        PromedioCalificacion = dr.IsDBNull(iPromedio) ? (decimal?)null : dr.GetDecimal(iPromedio)
                    };
                }
            }
        }

        public List<ReporteTipoDto> ObtenerPorTipo()
        {
            const string sql = @"
            SELECT t.Nombre AS Tipo,
                   COUNT(a.ActividadId) AS Actividades,
                   ISNULL(SUM(a.Cupos), 0) AS Cupos,
                   ISNULL(SUM(x.Inscritos), 0) AS Inscritos
            FROM dbo.TipoActividad t
            LEFT JOIN dbo.ActividadAcademica a ON a.TipoId = t.TipoId
            OUTER APPLY (SELECT COUNT(1) AS Inscritos
                         FROM dbo.Inscripcion i
                         WHERE i.ActividadId = a.ActividadId AND i.Estado <> N'Cancelada') x
            GROUP BY t.Nombre
            ORDER BY t.Nombre;";

            var filas = new List<ReporteTipoDto>();

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        filas.Add(new ReporteTipoDto
                        {
                            Tipo = dr.GetString(0),
                            Actividades = dr.GetInt32(1),
                            Cupos = dr.GetInt32(2),
                            Inscritos = dr.GetInt32(3)
                        });
                    }
                }
            }

            return filas;
        }

        public List<ReporteActividadDto> ObtenerPorActividad()
        {
            const string sql = @"
            SELECT a.ActividadId, a.Titulo, t.Nombre AS Tipo, a.FechaInicio, a.Estado, a.Cupos,
                   (SELECT COUNT(1) FROM dbo.Inscripcion i
                    WHERE i.ActividadId = a.ActividadId AND i.Estado <> N'Cancelada') AS Inscritos,
                   (SELECT COUNT(1) FROM dbo.Resena r WHERE r.ActividadId = a.ActividadId) AS TotalResenas,
                   (SELECT AVG(CAST(r.Calificacion AS DECIMAL(4, 2))) FROM dbo.Resena r
                    WHERE r.ActividadId = a.ActividadId) AS PromedioCalificacion
            FROM dbo.ActividadAcademica a
            INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId
            ORDER BY Inscritos DESC, a.FechaInicio;";

            var filas = new List<ReporteActividadDto>();

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int iPromedio = dr.GetOrdinal("PromedioCalificacion");

                        filas.Add(new ReporteActividadDto
                        {
                            ActividadId = dr.GetInt32(dr.GetOrdinal("ActividadId")),
                            Titulo = dr.GetString(dr.GetOrdinal("Titulo")),
                            Tipo = dr.GetString(dr.GetOrdinal("Tipo")),
                            FechaInicio = dr.GetDateTime(dr.GetOrdinal("FechaInicio")),
                            Estado = dr.GetString(dr.GetOrdinal("Estado")),
                            Cupos = dr.GetInt32(dr.GetOrdinal("Cupos")),
                            Inscritos = dr.GetInt32(dr.GetOrdinal("Inscritos")),
                            TotalResenas = dr.GetInt32(dr.GetOrdinal("TotalResenas")),
                            PromedioCalificacion = dr.IsDBNull(iPromedio) ? (decimal?)null : dr.GetDecimal(iPromedio)
                        });
                    }
                }
            }

            return filas;
        }
    }
}
