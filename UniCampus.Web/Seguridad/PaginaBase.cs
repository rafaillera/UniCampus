using System;
using System.Web;
using System.Web.UI;
using UniCampus.Entities;

namespace UniCampus.Web.Seguridad
{
    /// <summary>
    /// Clase base de todas las páginas que requieren sesión iniciada.
    /// - Sin sesión: redirige a Login.aspx.
    /// - Con un rol no permitido: redirige a la página de inicio de su rol.
    /// Cada página indica sus roles sobrescribiendo RolesPermitidos.
    /// </summary>
    public abstract class PaginaBase : Page
    {
        /// <summary>Roles que pueden abrir la página. Null = cualquier usuario autenticado.</summary>
        protected virtual string[] RolesPermitidos
        {
            get { return null; }
        }

        /// <summary>Usuario autenticado (lo usan las Vistas MVP para pasarlo al Presenter).</summary>
        public UsuarioDto UsuarioActual
        {
            get { return SesionUsuario.Actual; }
        }

        protected override void OnInit(EventArgs e)
        {
            UsuarioDto usuario = SesionUsuario.Actual;

            if (usuario == null)
            {
                Response.Redirect("~/Login.aspx", true);
                return;
            }

            if (RolesPermitidos != null && Array.IndexOf(RolesPermitidos, usuario.Rol) < 0)
            {
                Response.Redirect(SesionUsuario.PaginaInicio(usuario.Rol), true);
                return;
            }

            // Evita que con el botón "Atrás" se vean páginas privadas después de cerrar sesión.
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            base.OnInit(e);
        }

        /// <summary>
        /// Redirige sin lanzar ThreadAbortException (seguro dentro de un try/catch del Presenter).
        /// </summary>
        protected void RedirigirA(string url)
        {
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
