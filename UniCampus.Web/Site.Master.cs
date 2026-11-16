using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using UniCampus.Entities;
using UniCampus.Web.Seguridad;

namespace UniCampus.Web
{
    /// <summary>
    /// Página maestra: encabezado con el usuario y menú lateral según el rol.
    /// Solo presenta información; no contiene reglas de negocio.
    /// </summary>
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UsuarioDto usuario = SesionUsuario.Actual;
            if (usuario == null) return;

            // Literal no codifica HTML: por eso se usa Server.HtmlEncode.
            litNombre.Text = Server.HtmlEncode(usuario.NombreCompleto);
            litRol.Text = Server.HtmlEncode(usuario.Rol);
            litInicial.Text = Server.HtmlEncode(string.IsNullOrEmpty(usuario.Nombres) ? "?" : usuario.Nombres.Substring(0, 1));

            // Menú según el rol
            lnkActividades.Visible = usuario.EsGestor;
            lnkReportes.Visible = usuario.EsGestor;
            lnkMisInscripciones.Visible = usuario.EsEstudiante;

            MarcarActivo(lnkActividades, lnkCatalogo, lnkMisInscripciones, lnkReportes);
        }

        protected void lnkSalir_Click(object sender, EventArgs e)
        {
            SesionUsuario.Cerrar();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void MarcarActivo(params HyperLink[] enlaces)
        {
            string paginaActual = Request.AppRelativeCurrentExecutionFilePath;

            foreach (HyperLink enlace in enlaces)
            {
                bool activo = string.Equals(enlace.NavigateUrl, paginaActual, StringComparison.OrdinalIgnoreCase);
                enlace.CssClass = activo ? "activo" : string.Empty;
            }

            // Las páginas de detalle y reseñas se muestran dentro de "Catálogo".
            if (paginaActual.EndsWith("DetalleActividad.aspx", StringComparison.OrdinalIgnoreCase)
                || paginaActual.EndsWith("Resenas.aspx", StringComparison.OrdinalIgnoreCase))
            {
                lnkCatalogo.CssClass = "activo";
            }
        }
    }
}
