using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    /// <summary>
    /// Acceso a datos de dbo.Resena.
    /// </summary>
    public class ResenaRepository
    {
        /// <summary>Reseñas de una actividad, de la más reciente a la más antigua.</summary>
        public List<ResenaDto> ObtenerPorActividad(int actividadId)
        {
            const string sql = @"
            SELECT r.ResenaId, r.ActividadId, r.UsuarioId, r.Calificacion, r.Comentario, r.FechaCreacion,
                   u.Nombres + N' ' + LEFT(u.Apellidos, 1) + N'.' AS Autor
            FROM dbo.Resena r
            INNER JOIN dbo.Usuario u ON u.UsuarioId = r.UsuarioId
            WHERE r.ActividadId = @ActividadId
            ORDER BY r.FechaCreacion DESC, r.ResenaId DESC;";

            var resenas = new List<ResenaDto>();

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int iComentario = dr.GetOrdinal("Comentario");

                        resenas.Add(new ResenaDto
                        {
                            ResenaId = dr.GetInt32(dr.GetOrdinal("ResenaId")),
                            ActividadId = dr.GetInt32(dr.GetOrdinal("ActividadId")),
                            UsuarioId = dr.GetInt32(dr.GetOrdinal("UsuarioId")),
                            Calificacion = dr.GetByte(dr.GetOrdinal("Calificacion")),
                            Comentario = dr.IsDBNull(iComentario) ? null : dr.GetString(iComentario),
                            FechaCreacion = dr.GetDateTime(dr.GetOrdinal("FechaCreacion")),
                            Autor = dr.GetString(dr.GetOrdinal("Autor"))
                        });
                    }
                }
            }

            return resenas;
        }

        /// <summary>true si el estudiante tiene una inscripción vigente (Activa o Asistió).</summary>
        public bool EstaInscrito(int actividadId, int usuarioId)
        {
            const string sql = @"
            SELECT COUNT(1) FROM dbo.Inscripcion
            WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId AND Estado <> N'Cancelada';";

            return Contar(sql, actividadId, usuarioId) > 0;
        }

        /// <summary>true si el estudiante ya publicó una reseña para la actividad.</summary>
        public bool ExisteResena(int actividadId, int usuarioId)
        {
            const string sql = @"
            SELECT COUNT(1) FROM dbo.Resena
            WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId;";

            return Contar(sql, actividadId, usuarioId) > 0;
        }

        /// <summary>
        /// Inserta la reseña. Devuelve false si ya existía una del mismo estudiante
        /// (la restricción UNIQUE de la BD lo impide aunque se envíe dos veces).
        /// </summary>
        public bool Insertar(ResenaDto resena)
        {
            if (resena == null) throw new ArgumentNullException(nameof(resena));

            const string sql = @"
            INSERT INTO dbo.Resena (ActividadId, UsuarioId, Calificacion, Comentario)
            VALUES (@ActividadId, @UsuarioId, @Calificacion, @Comentario);";

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = resena.ActividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = resena.UsuarioId;
                cmd.Parameters.Add("@Calificacion", SqlDbType.TinyInt).Value = (byte)resena.Calificacion;
                cmd.Parameters.Add("@Comentario", SqlDbType.NVarChar, 1000).Value = ActividadSql.ValorONulo(resena.Comentario);

                cn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                {
                    // 2627 / 2601 = violación de UNIQUE: reseña duplicada.
                    return false;
                }
            }
        }

        private static int Contar(string sql, int actividadId, int usuarioId)
        {
            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
