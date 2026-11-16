using System;
using System.Web;
using UniCampus.Entities;

namespace UniCampus.Web.Seguridad
{
    /// <summary>
    /// Único lugar donde se lee y escribe el usuario autenticado en la sesión.
    /// El UsuarioId que usan inscripciones y reseñas sale SIEMPRE de aquí,
    /// nunca de un campo oculto o de la URL.
    /// </summary>
    public static class SesionUsuario
    {
        private const string ClaveSesion = "UniCampus.Usuario";

        public static UsuarioDto Actual
        {
            get
            {
                HttpContext contexto = HttpContext.Current;
                if (contexto == null || contexto.Session == null) return null;
                return contexto.Session[ClaveSesion] as UsuarioDto;
            }
        }

        public static void Iniciar(UsuarioDto usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));

            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session[ClaveSesion] = usuario;
        }

        public static void Cerrar()
        {
            HttpContext contexto = HttpContext.Current;
            contexto.Session.Clear();
            contexto.Session.Abandon();

            // Se expira la cookie de sesión para que el siguiente inicio use un Id nuevo.
            contexto.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty)
            {
                Expires = DateTime.Now.AddDays(-1),
                HttpOnly = true
            });
        }

        /// <summary>Página a la que se envía al usuario después de iniciar sesión.</summary>
        public static string PaginaInicio(string rol)
        {
            return rol == Roles.Organizador || rol == Roles.Administrador
                ? "~/Actividades.aspx"
                : "~/Catalogo.aspx";
        }
    }
}
