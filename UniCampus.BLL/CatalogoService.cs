using System;
using System.Collections.Generic;
using System.Linq;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>
    /// Reglas del catálogo de actividades para estudiantes:
    /// valida los filtros y garantiza que solo se muestren actividades publicadas.
    /// </summary>
    public class CatalogoService
    {
        public const int LongitudMaximaTexto = 100;

        private readonly CatalogoRepository _repository;

        public CatalogoService() : this(new CatalogoRepository())
        {
        }

        public CatalogoService(CatalogoRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Valida los criterios. Devuelve la lista de errores (vacía si todo está bien).
        /// </summary>
        public List<string> ValidarFiltro(FiltroActividadDto filtro)
        {
            var errores = new List<string>();
            if (filtro == null) return errores;

            if (filtro.Texto != null && filtro.Texto.Trim().Length > LongitudMaximaTexto)
                errores.Add("El texto de búsqueda no puede superar " + LongitudMaximaTexto + " caracteres.");

            if (filtro.FechaDesde.HasValue && filtro.FechaHasta.HasValue
                && filtro.FechaDesde.Value.Date > filtro.FechaHasta.Value.Date)
                errores.Add("La fecha \"desde\" no puede ser posterior a la fecha \"hasta\".");

            if (!string.IsNullOrWhiteSpace(filtro.Modalidad) && !Modalidades.Todas.Contains(filtro.Modalidad))
                errores.Add("La modalidad seleccionada no es válida.");

            return errores;
        }

        /// <summary>
        /// Busca SOLO actividades publicadas (los estudiantes no deben ver borradores
        /// ni actividades inactivas, aunque manipulen el formulario).
        /// </summary>
        public List<ActividadDto> BuscarPublicadas(FiltroActividadDto filtro)
        {
            filtro = filtro ?? new FiltroActividadDto();

            if (ValidarFiltro(filtro).Count > 0)
            {
                return new List<ActividadDto>();
            }

            filtro.Texto = string.IsNullOrWhiteSpace(filtro.Texto) ? null : filtro.Texto.Trim();
            filtro.Modalidad = string.IsNullOrWhiteSpace(filtro.Modalidad) ? null : filtro.Modalidad;
            filtro.TipoId = filtro.TipoId.HasValue && filtro.TipoId.Value > 0 ? filtro.TipoId : null;
            filtro.Estado = EstadosActividad.Publicada;

            return _repository.Buscar(filtro);
        }
    }
}
