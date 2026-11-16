using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    /// <summary>
    /// Contrato MVP de Login.aspx.
    /// </summary>
    public interface ILoginView
    {
        string Correo { get; }
        string Clave { get; }

        void MostrarError(string mensaje);

        /// <summary>Guarda el usuario en la sesión y lo lleva a su página de inicio.</summary>
        void IniciarSesion(UsuarioDto usuario, string urlInicio);
    }
}
