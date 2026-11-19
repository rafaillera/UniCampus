using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    /// <summary>
    /// Acceso a datos de dbo.Inscripcion.
    /// La inscripción se hace dentro de una SqlTransaction que bloquea la fila
    /// de la actividad, para que dos estudiantes no tomen el último cupo a la vez.
    /// </summary>
    public class InscripcionRepository
    {
        /// <summary>
        /// Inscribe (o reactiva) a un estudiante controlando cupos y duplicados
        /// de forma atómica. Devuelve un código con el resultado.
        /// </summary>
        public CodigoInscripcion Inscribir(int actividadId, int usuarioId)
        {
            using (SqlConnection cn = ConexionDb.Crear())
            {
                cn.Open();

                using (SqlTransaction tx = cn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // 1. Leer y BLOQUEAR la actividad (UPDLOCK). Si otro estudiante se está
                        //    inscribiendo en la misma actividad, esta consulta espera a que termine.
                        int cupos;
                        string estadoActividad;

                        const string sqlActividad = @"
                        SELECT Cupos, Estado
                        FROM dbo.ActividadAcademica WITH (UPDLOCK, ROWLOCK)
                        WHERE ActividadId = @ActividadId;";

                        using (var cmd = new SqlCommand(sqlActividad, cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;

                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                if (!dr.Read())
                                {
                                    dr.Close();
                                    tx.Rollback();
                                    return CodigoInscripcion.NoExiste;
                                }

                                cupos = dr.GetInt32(0);
                                estadoActividad = dr.GetString(1);
                            }
                        }

                        if (estadoActividad != EstadosActividad.Publicada)
                        {
                            tx.Rollback();
                            return CodigoInscripcion.NoDisponible;
                        }

                        // 2. ¿El estudiante ya tiene una inscripción (activa o cancelada)?
                        const string sqlPrevia = @"
                        SELECT Estado FROM dbo.Inscripcion
                        WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId;";

                        string estadoPrevio;
                        using (var cmd = new SqlCommand(sqlPrevia, cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                            estadoPrevio = cmd.ExecuteScalar() as string;
                        }

                        if (estadoPrevio != null && estadoPrevio != EstadosInscripcion.Cancelada)
                        {
                            tx.Rollback();
                            return CodigoInscripcion.YaInscrito;
                        }

                        // 3. Contar cupos ocupados (inscripciones no canceladas).
                        const string sqlOcupados = @"
                        SELECT COUNT(1) FROM dbo.Inscripcion
                        WHERE ActividadId = @ActividadId AND Estado <> @Cancelada;";

                        int ocupados;
                        using (var cmd = new SqlCommand(sqlOcupados, cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            cmd.Parameters.Add("@Cancelada", SqlDbType.NVarChar, 20).Value = EstadosInscripcion.Cancelada;
                            ocupados = (int)cmd.ExecuteScalar();
                        }

                        if (ocupados >= cupos)
                        {
                            tx.Rollback();
                            return CodigoInscripcion.SinCupos;
                        }

                        // 4. Insertar la inscripción nueva o reactivar la cancelada
                        //    (la restricción UNIQUE no permite una segunda fila).
                        string sqlGuardar = estadoPrevio == null
                            ? @"INSERT INTO dbo.Inscripcion (ActividadId, UsuarioId, Estado)
                                VALUES (@ActividadId, @UsuarioId, @Activa);"
                            : @"UPDATE dbo.Inscripcion
                                SET Estado = @Activa, FechaInscripcion = SYSDATETIME()
                                WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId;";

                        using (var cmd = new SqlCommand(sqlGuardar, cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                            cmd.Parameters.Add("@Activa", SqlDbType.NVarChar, 20).Value = EstadosInscripcion.Activa;
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return estadoPrevio == null ? CodigoInscripcion.Inscrito : CodigoInscripcion.Reactivada;
                    }
                    catch
                    {
                        // Cualquier error deshace TODO lo hecho en la transacción.
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>Cancela una inscripción activa. Devuelve true si había una inscripción activa.</summary>
        public bool Cancelar(int actividadId, int usuarioId)
        {
            const string sql = @"
            UPDATE dbo.Inscripcion
            SET Estado = @Cancelada
            WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId AND Estado = @Activa;";

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                cmd.Parameters.Add("@Cancelada", SqlDbType.NVarChar, 20).Value = EstadosInscripcion.Cancelada;
                cmd.Parameters.Add("@Activa", SqlDbType.NVarChar, 20).Value = EstadosInscripcion.Activa;

                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        /// <summary>Estado de la inscripción del estudiante en la actividad, o null si nunca se inscribió.</summary>
        public string ObtenerEstado(int actividadId, int usuarioId)
        {
            const string sql = @"
            SELECT Estado FROM dbo.Inscripcion
            WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId;";

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

                cn.Open();
                return cmd.ExecuteScalar() as string;
            }
        }

        /// <summary>Inscripciones de un estudiante con los datos de cada actividad.</summary>
        public List<InscripcionDto> ObtenerPorUsuario(int usuarioId)
        {
            const string sql = @"
            SELECT i.InscripcionId, i.ActividadId, i.UsuarioId, i.FechaInscripcion, i.Estado,
                   a.Titulo, t.Nombre AS TipoNombre, a.FechaInicio, a.HoraInicio, a.Lugar,
                   a.Estado AS EstadoActividad
            FROM dbo.Inscripcion i
            INNER JOIN dbo.ActividadAcademica a ON a.ActividadId = i.ActividadId
            INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId
            WHERE i.UsuarioId = @UsuarioId
            ORDER BY a.FechaInicio DESC;";

            var inscripciones = new List<InscripcionDto>();

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int iHora = dr.GetOrdinal("HoraInicio");

                        inscripciones.Add(new InscripcionDto
                        {
                            InscripcionId = dr.GetInt32(dr.GetOrdinal("InscripcionId")),
                            ActividadId = dr.GetInt32(dr.GetOrdinal("ActividadId")),
                            UsuarioId = dr.GetInt32(dr.GetOrdinal("UsuarioId")),
                            FechaInscripcion = dr.GetDateTime(dr.GetOrdinal("FechaInscripcion")),
                            Estado = dr.GetString(dr.GetOrdinal("Estado")),
                            ActividadTitulo = dr.GetString(dr.GetOrdinal("Titulo")),
                            TipoNombre = dr.GetString(dr.GetOrdinal("TipoNombre")),
                            FechaInicio = dr.GetDateTime(dr.GetOrdinal("FechaInicio")),
                            HoraInicio = dr.IsDBNull(iHora) ? (TimeSpan?)null : dr.GetTimeSpan(iHora),
                            Lugar = dr.GetString(dr.GetOrdinal("Lugar")),
                            EstadoActividad = dr.GetString(dr.GetOrdinal("EstadoActividad"))
                        });
                    }
                }
            }

            return inscripciones;
        }

        /// <summary>Estudiantes inscritos en una actividad (para organizadores).</summary>
        public List<InscripcionDto> ObtenerPorActividad(int actividadId)
        {
            const string sql = @"
            SELECT i.InscripcionId, i.ActividadId, i.UsuarioId, i.FechaInscripcion, i.Estado,
                   u.Nombres + N' ' + u.Apellidos AS NombreEstudiante, u.Correo, u.Programa
            FROM dbo.Inscripcion i
            INNER JOIN dbo.Usuario u ON u.UsuarioId = i.UsuarioId
            WHERE i.ActividadId = @ActividadId
            ORDER BY CASE WHEN i.Estado = N'Cancelada' THEN 1 ELSE 0 END, u.Apellidos, u.Nombres;";

            var inscritos = new List<InscripcionDto>();

            using (SqlConnection cn = ConexionDb.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int iPrograma = dr.GetOrdinal("Programa");

                        inscritos.Add(new InscripcionDto
                        {
                            InscripcionId = dr.GetInt32(dr.GetOrdinal("InscripcionId")),
                            ActividadId = dr.GetInt32(dr.GetOrdinal("ActividadId")),
                            UsuarioId = dr.GetInt32(dr.GetOrdinal("UsuarioId")),
                            FechaInscripcion = dr.GetDateTime(dr.GetOrdinal("FechaInscripcion")),
                            Estado = dr.GetString(dr.GetOrdinal("Estado")),
                            NombreEstudiante = dr.GetString(dr.GetOrdinal("NombreEstudiante")),
                            Correo = dr.GetString(dr.GetOrdinal("Correo")),
                            Programa = dr.IsDBNull(iPrograma) ? null : dr.GetString(iPrograma)
                        });
                    }
                }
            }

            return inscritos;
        }
    }
}
