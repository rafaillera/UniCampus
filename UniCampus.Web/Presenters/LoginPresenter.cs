using System;
using System.Diagnostics;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Seguridad;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>
    /// Presenter de Login.aspx: pide a la BLL que valide las credenciales
    /// y le indica a la Vista si debe mostrar un error o iniciar la sesión.
    /// </summary>
    public class LoginPresenter
    {
        private readonly ILoginView _view;
        private readonly AutenticacionService _service;

        public LoginPresenter(ILoginView view) : this(view, new AutenticacionService())
        {
        }

        public LoginPresenter(ILoginView view, AutenticacionService service)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void IniciarSesion()
        {
            try
            {
                ResultadoAutenticacion resultado = _service.Autenticar(_view.Correo, _view.Clave);

                if (!resultado.Exito)
                {
                    _view.MostrarError(resultado.Mensaje);
                    return;
                }

                _view.IniciarSesion(resultado.Usuario, SesionUsuario.PaginaInicio(resultado.Usuario.Rol));
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UniCampus] " + DateTime.Now.ToString("s") + " " + ex);
                _view.MostrarError("No fue posible iniciar sesión en este momento. Intente de nuevo.");
            }
        }
    }
}
