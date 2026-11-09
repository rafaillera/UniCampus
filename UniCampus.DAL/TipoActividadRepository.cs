using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    /// <summary>
    /// Acceso a datos del catálogo dbo.TipoActividad.
    /// </summary>
    public class TipoActividadRepository
    {
        public List<TipoActividadDto> ObtenerActivos()
        {
            const string sql = @"
            SELECT TipoId, Nombre
            FROM dbo.TipoActividad
            WHERE Activo = @Activo
            ORDER BY Nombre;";

            var tipos = new List<TipoActividadDto>();

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = true;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        tipos.Add(new TipoActividadDto
                        {
                            TipoId = dr.GetInt32(0),
                            Nombre = dr.GetString(1)
                        });
                    }
                }
            }

            return tipos;
        }

        public bool Existe(int tipoId)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.TipoActividad WHERE TipoId = @TipoId AND Activo = 1;";

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = tipoId;
                cn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}
