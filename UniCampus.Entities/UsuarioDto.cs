using System;

namespace UniCampus.Entities
{
    /// <summary>
    /// Datos públicos del usuario autenticado. Es lo que se guarda en la sesión,
    /// por eso NO incluye el hash ni el salt de la contraseña.
    /// </summary>
    [Serializable]
    public class UsuarioDto
    {
        public int UsuarioId { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Programa { get; set; }

        /// <summary>Estudiante, Organizador o Administrador.</summary>
        public string Rol { get; set; }

        public string NombreCompleto
        {
            get { return (Nombres + " " + Apellidos).Trim(); }
        }

        public bool EsEstudiante
        {
            get { return Rol == Roles.Estudiante; }
        }

        public bool EsGestor
        {
            get { return Rol == Roles.Organizador || Rol == Roles.Administrador; }
        }
    }

    /// <summary>
    /// Usuario con su credencial. Solo viaja entre la DAL y la BLL para
    /// comprobar la contraseña; nunca llega a la capa Web ni a la sesión.
    /// </summary>
    public class UsuarioCredencialDto : UsuarioDto
    {
        public string ClaveSalt { get; set; }
        public byte[] ClaveHash { get; set; }
    }
}
