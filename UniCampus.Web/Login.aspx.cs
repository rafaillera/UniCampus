using System;
using System.Web.UI;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Seguridad;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    /// <summary>
    /// Vista (MVP) del inicio de sesión. Es la única página que no hereda de
    /// PaginaBase, porque se abre sin sesión.
    /// </summary>
    public partial class Login : Page, ILoginView
    {
        private LoginPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new LoginPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Si ya hay sesión, no tiene sentido mostrar el formulario.
            UsuarioDto usuario = SesionUsuario.Actual;
            if (!IsPostBack && usuario != null)
            {
                Response.Redirect(SesionUsuario.PaginaInicio(usuario.Rol), false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            _presenter.IniciarSesion();
        }

        // ---------------------------------------------------------------------
        // Implementación de ILoginView
        // ---------------------------------------------------------------------

        public string Correo
        {
            get { return txtCorreo.Text; }
        }

        public string Clave
        {
            get { return txtClave.Text; }
        }

        public void MostrarError(string mensaje)
        {
            lblMensaje.Text = Server.HtmlEncode(mensaje);
            lblMensaje.Visible = true;
        }

        public void IniciarSesion(UsuarioDto usuario, string urlInicio)
        {
            SesionUsuario.Iniciar(usuario);
            Response.Redirect(urlInicio, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
