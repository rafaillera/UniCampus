using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    /// <summary>
    /// Contrato MVP de Catalogo.aspx (búsqueda y filtros de actividades).
    /// </summary>
    public interface ICatalogoView
    {
        /// <summary>Criterios escritos por el usuario, ya convertidos a tipos .NET.</summary>
        FiltroActividadDto Filtro { get; }

        void MostrarTipos(IList<TipoActividadDto> tipos);
        void MostrarActividades(IList<ActividadDto> actividades);
        void MostrarError(string mensaje);
        void LimpiarFiltros();
    }
}
