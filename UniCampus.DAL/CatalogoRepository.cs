using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    /// <summary>
    /// Búsqueda de actividades con filtros opcionales.
    /// Patrón "(@Param IS NULL OR columna = @Param)": un solo SQL fijo y
    /// parametrizado, sin concatenar el texto que escribe el usuario.
    /// </summary>
    public class CatalogoRepository
    {
        public List<ActividadDto> Buscar(FiltroActividadDto filtro)
        {
            if (filtro == null) throw new ArgumentNullException(nameof(filtro));

            const string sql = ActividadSql.SelectBase + @"
            WHERE (@Texto IS NULL
                   OR a.Titulo LIKE @Texto ESCAPE '\'
                   OR a.Descripcion LIKE @Texto ESCAPE '\'
                   OR a.Lugar LIKE @Texto ESCAPE '\')
              AND (@TipoId IS NULL OR a.TipoId = @TipoId)
              AND (@FechaDesde IS NULL OR a.FechaInicio >= @FechaDesde)
              AND (@FechaHasta IS NULL OR a.FechaInicio <= @FechaHasta)
              AND (@Modalidad IS NULL OR a.Modalidad = @Modalidad)
              AND (@Estado IS NULL OR a.Estado = @Estado)
              AND (@SoloConCupos = 0
                   OR a.Cupos > (SELECT COUNT(1) FROM dbo.Inscripcion i2
                                 WHERE i2.ActividadId = a.ActividadId AND i2.Estado <> N'Cancelada'))
            ORDER BY a.FechaInicio, a.HoraInicio, a.Titulo
            OPTION (RECOMPILE);";

            var actividades = new List<ActividadDto>();

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                // El texto se envuelve en % para buscar "contiene"; los comodines
                // que escriba el usuario (% _ [) se escapan para tratarlos como texto.
                cmd.Parameters.Add("@Texto", SqlDbType.NVarChar, 102).Value =
                    string.IsNullOrWhiteSpace(filtro.Texto) ? (object)DBNull.Value : "%" + EscaparLike(filtro.Texto) + "%";
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = (object)filtro.TipoId ?? DBNull.Value;
                cmd.Parameters.Add("@FechaDesde", SqlDbType.Date).Value = (object)filtro.FechaDesde ?? DBNull.Value;
                cmd.Parameters.Add("@FechaHasta", SqlDbType.Date).Value = (object)filtro.FechaHasta ?? DBNull.Value;
                cmd.Parameters.Add("@Modalidad", SqlDbType.NVarChar, 20).Value = ActividadSql.ValorONulo(filtro.Modalidad);
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = ActividadSql.ValorONulo(filtro.Estado);
                cmd.Parameters.Add("@SoloConCupos", SqlDbType.Bit).Value = filtro.SoloConCupos;

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

        /// <summary>Escapa los comodines de LIKE usando '\' como carácter de escape.</summary>
        private static string EscaparLike(string texto)
        {
            return texto.Trim()
                .Replace(@"\", @"\\")
                .Replace("%", @"\%")
                .Replace("_", @"\_")
                .Replace("[", @"\[");
        }
    }
}
