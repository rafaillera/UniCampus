using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    /// <summary>
    /// Contrato MVP de la página Actividades.aspx.
    /// El Presenter solo conoce esta interfaz, nunca los controles web
    /// (TextBox, GridView...). Así la lógica de presentación queda desacoplada
    /// de ASP.NET Web Forms.
    /// </summary>
    public interface IActividadesView
    {
        /// <summary>Muestra el listado de actividades en la tabla.</summary>
        void MostrarActividades(IList<ActividadDto> actividades);

        /// <summary>Muestra un mensaje de confirmación al usuario.</summary>
        void MostrarMensaje(string mensaje);

        /// <summary>Muestra un mensaje de error o de validación al usuario.</summary>
        void MostrarError(string mensaje);

        /// <summary>Carga las opciones del selector de tipo de actividad.</summary>
        void MostrarTipos(IList<TipoActividadDto> tipos);

        /// <summary>Deja el formulario en blanco después de un registro exitoso.</summary>
        void LimpiarFormulario();

        // ---- Entrega final: editar, ver detalle y cambiar estado ----

        /// <summary>Llena el formulario con los datos de una actividad para editarla.</summary>
        void CargarEnFormulario(ActividadDto actividad);

        /// <summary>Cambia títulos y botones entre "registrar" y "editar".</summary>
        void MostrarModoEdicion(bool editando);

        /// <summary>Muestra el panel de detalle de una actividad (null lo oculta).</summary>
        void MostrarDetalle(ActividadDto actividad);
    }
}
