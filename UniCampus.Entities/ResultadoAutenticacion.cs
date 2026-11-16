namespace UniCampus.Entities
{
    /// <summary>
    /// Resultado del inicio de sesión que devuelve la BLL al Presenter.
    /// </summary>
    public class ResultadoAutenticacion
    {
        public bool Exito { get; private set; }
        public string Mensaje { get; private set; }
        public UsuarioDto Usuario { get; private set; }

        private ResultadoAutenticacion() { }

        public static ResultadoAutenticacion Ok(UsuarioDto usuario)
        {
            return new ResultadoAutenticacion { Exito = true, Usuario = usuario, Mensaje = "Bienvenido(a), " + usuario.Nombres + "." };
        }

        public static ResultadoAutenticacion Fallo(string mensaje)
        {
            return new ResultadoAutenticacion { Exito = false, Mensaje = mensaje };
        }
    }
}
