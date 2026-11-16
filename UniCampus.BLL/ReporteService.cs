using System;
using System.Collections.Generic;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>
    /// Reportes y resúmenes para organizadores y administradores.
    /// </summary>
    public class ReporteService
    {
        private readonly ReporteRepository _repository;

        public ReporteService() : this(new ReporteRepository())
        {
        }

        public ReporteService(ReporteRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>Totales generales calculados con la fecha de hoy.</summary>
        public ResumenActividadesDto ObtenerResumen()
        {
            return _repository.ObtenerResumen(DateTime.Today);
        }

        public List<ReporteTipoDto> ObtenerReportePorTipo()
        {
            return _repository.ObtenerPorTipo();
        }

        /// <summary>Actividades ordenadas de mayor a menor número de inscritos.</summary>
        public List<ReporteActividadDto> ObtenerReportePorActividad()
        {
            return _repository.ObtenerPorActividad();
        }
    }
}
