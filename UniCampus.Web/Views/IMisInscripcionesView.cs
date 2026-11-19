using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    /// <summary>
    /// Contrato MVP de MisInscripciones.aspx.
    /// </summary>
    public interface IMisInscripcionesView
    {
        UsuarioDto UsuarioActual { get; }

        void MostrarInscripciones(IList<InscripcionDto> inscripciones);
        void MostrarMensaje(string mensaje);
        void MostrarError(string mensaje);
    }
}
