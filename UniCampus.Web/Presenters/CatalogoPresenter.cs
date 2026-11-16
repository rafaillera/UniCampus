using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>
    /// Presenter del catálogo: valida los filtros con la BLL y muestra los resultados.
    /// </summary>
    public class CatalogoPresenter
    {
        private readonly ICatalogoView _view;
        private readonly CatalogoService _catalogoService;
        private readonly TipoActividadService _tipoService;

        public CatalogoPresenter(ICatalogoView view)
            : this(view, new CatalogoService(), new TipoActividadService())
        {
        }

        public CatalogoPresenter(ICatalogoView view, CatalogoService catalogoService, TipoActividadService tipoService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _catalogoService = catalogoService ?? throw new ArgumentNullException(nameof(catalogoService));
            _tipoService = tipoService ?? throw new ArgumentNullException(nameof(tipoService));
        }

        /// <summary>Carga inicial: tipos para el filtro y todas las actividades publicadas.</summary>
        public void Cargar()
        {
            try
            {
                _view.MostrarTipos(_tipoService.ListarTipos());
                _view.MostrarActividades(_catalogoService.BuscarPublicadas(new FiltroActividadDto()));
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarActividades(new List<ActividadDto>());
                _view.MostrarError("No fue posible cargar el catálogo en este momento.");
            }
        }

        public void Buscar()
        {
            try
            {
                FiltroActividadDto filtro = _view.Filtro;
                List<string> errores = _catalogoService.ValidarFiltro(filtro);

                if (errores.Count > 0)
                {
                    _view.MostrarError(string.Join(" ", errores));
                    _view.MostrarActividades(new List<ActividadDto>());
                    return;
                }

                _view.MostrarActividades(_catalogoService.BuscarPublicadas(filtro));
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarError("No fue posible realizar la búsqueda. Intente de nuevo.");
            }
        }

        public void Limpiar()
        {
            _view.LimpiarFiltros();
            Buscar();
        }

        private static void Registrar(Exception ex)
        {
            Trace.TraceError("[UniCampus] " + DateTime.Now.ToString("s") + " " + ex);
        }
    }
}
