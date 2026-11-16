using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    /// <summary>
    /// Acceso a datos de dbo.Usuario.
    /// </summary>
    public class UsuarioRepository
    {
        /// <summary>
        /// Busca un usuario ACTIVO por correo y devuelve sus datos con la credencial
        /// (salt + hash) para que la BLL compruebe la contraseña. Null si no existe.
        /// </summary>
        public UsuarioCredencialDto ObtenerCredencialPorCorreo(string correo)
        {
            const string sql = @"
            SELECT UsuarioId, Nombres, Apellidos, Correo, Programa, Rol, ClaveSalt, ClaveHash
            FROM dbo.Usuario
            WHERE Correo = @Correo AND Activo = 1;";

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = correo;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        return null;
                    }

                    int iPrograma = dr.GetOrdinal("Programa");
                    int iHash = dr.GetOrdinal("ClaveHash");

                    return new UsuarioCredencialDto
                    {
                        UsuarioId = dr.GetInt32(dr.GetOrdinal("UsuarioId")),
                        Nombres = dr.GetString(dr.GetOrdinal("Nombres")),
                        Apellidos = dr.GetString(dr.GetOrdinal("Apellidos")),
                        Correo = dr.GetString(dr.GetOrdinal("Correo")),
                        Programa = dr.IsDBNull(iPrograma) ? null : dr.GetString(iPrograma),
                        Rol = dr.GetString(dr.GetOrdinal("Rol")),
                        ClaveSalt = dr.GetString(dr.GetOrdinal("ClaveSalt")),
                        ClaveHash = dr.IsDBNull(iHash) ? null : (byte[])dr[iHash]
                    };
                }
            }
        }
    }
}
