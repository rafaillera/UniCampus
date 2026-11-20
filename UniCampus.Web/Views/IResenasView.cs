using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    /// <summary>
    /// Contrato MVP de Resenas.aspx.
    /// </summary>
    public interface IResenasView
    {
        /// <summary>Id de la actividad leído de la URL (?id=). 0 si no es válido.</summary>
        int ActividadId { get; }

        UsuarioDto UsuarioActual { get; }

        /// <summary>Calificación elegida (0 si no eligió ninguna).</summary>
        int Calificacion { get; }

        string Comentario { get; }

        void MostrarActividad(ActividadDto actividad);
        void MostrarNoEncontrada();
        void MostrarResenas(IList<ResenaDto> resenas, decimal? promedio);

        /// <summary>Muestra el formulario o el motivo por el que no se puede reseñar.</summary>
        void MostrarFormulario(PermisoResenaDto permiso);

        void LimpiarFormulario();
        void MostrarMensaje(string mensaje);
        void MostrarError(string mensaje);
    }
}
