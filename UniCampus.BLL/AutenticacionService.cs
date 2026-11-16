using System;
using System.Security.Cryptography;
using System.Text;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>
    /// Inicio de sesión. Comprueba la contraseña comparando hashes:
    /// la contraseña nunca se guarda ni se compara en texto plano.
    /// </summary>
    public class AutenticacionService
    {
        public const int LongitudMaximaCorreo = 150;
        public const int LongitudMaximaClave = 100;

        /// <summary>
        /// Mensaje único para correo inexistente o contraseña errada:
        /// así no se revela qué correos están registrados.
        /// </summary>
        public const string MensajeCredencialesInvalidas = "Correo o contraseña incorrectos.";

        private readonly UsuarioRepository _repository;

        public AutenticacionService() : this(new UsuarioRepository())
        {
        }

        public AutenticacionService(UsuarioRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ResultadoAutenticacion Autenticar(string correo, string clave)
        {
            correo = correo == null ? null : correo.Trim();

            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrEmpty(clave))
                return ResultadoAutenticacion.Fallo("Ingrese su correo y su contraseña.");

            if (correo.Length > LongitudMaximaCorreo || clave.Length > LongitudMaximaClave || !correo.Contains("@"))
                return ResultadoAutenticacion.Fallo(MensajeCredencialesInvalidas);

            UsuarioCredencialDto credencial = _repository.ObtenerCredencialPorCorreo(correo);

            if (credencial == null || credencial.ClaveHash == null)
                return ResultadoAutenticacion.Fallo(MensajeCredencialesInvalidas);

            byte[] hashIngresado = CalcularHash(credencial.ClaveSalt, clave);
            if (!SonIguales(hashIngresado, credencial.ClaveHash))
                return ResultadoAutenticacion.Fallo(MensajeCredencialesInvalidas);

            // Se devuelve un UsuarioDto "limpio", sin salt ni hash.
            var usuario = new UsuarioDto
            {
                UsuarioId = credencial.UsuarioId,
                Nombres = credencial.Nombres,
                Apellidos = credencial.Apellidos,
                Correo = credencial.Correo,
                Programa = credencial.Programa,
                Rol = credencial.Rol
            };

            return ResultadoAutenticacion.Ok(usuario);
        }

        /// <summary>
        /// SHA-256 de (salt + contraseña) codificado en UTF-16LE (Encoding.Unicode),
        /// que es exactamente lo que hace HASHBYTES('SHA2_256', NVARCHAR) en SQL Server.
        /// </summary>
        public static byte[] CalcularHash(string salt, string clave)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.Unicode.GetBytes((salt ?? string.Empty) + clave));
            }
        }

        /// <summary>Comparación en tiempo constante para no filtrar información por tiempos de respuesta.</summary>
        private static bool SonIguales(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;

            int diferencia = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diferencia |= a[i] ^ b[i];
            }
            return diferencia == 0;
        }
    }
}
