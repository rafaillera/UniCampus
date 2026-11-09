using System.Collections.Generic;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>
    /// Servicio del catálogo de tipos de actividad.
    /// </summary>
    public class TipoActividadService
    {
        private readonly TipoActividadRepository _repository;

        public TipoActividadService() : this(new TipoActividadRepository())
        {
        }

        public TipoActividadService(TipoActividadRepository repository)
        {
            _repository = repository;
        }

        public List<TipoActividadDto> ListarTipos()
        {
            return _repository.ObtenerActivos();
        }
    }
}
