using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    /// <summary>
    /// Contrato MVP de Reportes.aspx.
    /// </summary>
    public interface IReportesView
    {
        void MostrarResumen(ResumenActividadesDto resumen);
        void MostrarReportePorTipo(IList<ReporteTipoDto> filas);
        void MostrarReportePorActividad(IList<ReporteActividadDto> filas);
        void MostrarError(string mensaje);
    }
}
