using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    /// <summary>
    /// Contrato MVP de DetalleActividad.aspx (detalle + inscripción).
    /// </summary>
    public interface IDetalleActividadView
    {
        /// <summary>Id de la actividad leído de la URL (?id=). 0 si no es válido.</summary>
        int ActividadId { get; }

        /// <summary>Usuario de la sesión (lo implementa PaginaBase).</summary>
        UsuarioDto UsuarioActual { get; }

        void MostrarActividad(ActividadDto actividad);
        void MostrarNoEncontrada();
        void MostrarOpcionesInscripcion(OpcionesInscripcionDto opciones);

        /// <summary>Lista de inscritos (solo organizadores). Null la oculta.</summary>
        void MostrarInscritos(IList<InscripcionDto> inscritos);

        void MostrarMensaje(string mensaje);
        void MostrarError(string mensaje);
    }
}
