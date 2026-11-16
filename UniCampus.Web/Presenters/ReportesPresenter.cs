using System;
using System.Diagnostics;
using UniCampus.BLL;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>
    /// Presenter de la página de reportes.
    /// </summary>
    public class ReportesPresenter
    {
        private readonly IReportesView _view;
        private readonly ReporteService _service;

        public ReportesPresenter(IReportesView view) : this(view, new ReporteService())
        {
        }

        public ReportesPresenter(IReportesView view, ReporteService service)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void Cargar()
        {
            try
            {
                _view.MostrarResumen(_service.ObtenerResumen());
                _view.MostrarReportePorTipo(_service.ObtenerReportePorTipo());
                _view.MostrarReportePorActividad(_service.ObtenerReportePorActividad());
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UniCampus] " + DateTime.Now.ToString("s") + " " + ex);
                _view.MostrarError("No fue posible generar los reportes en este momento.");
            }
        }
    }
}
