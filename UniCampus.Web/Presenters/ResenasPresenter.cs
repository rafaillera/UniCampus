using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>
    /// Presenter de las reseñas de una actividad.
    /// </summary>
    public class ResenasPresenter
    {
        private readonly IResenasView _view;
        private readonly ActividadService _actividadService;
        private readonly ResenaService _resenaService;

        public ResenasPresenter(IResenasView view)
            : this(view, new ActividadService(), new ResenaService())
        {
        }

        public ResenasPresenter(IResenasView view, ActividadService actividadService, ResenaService resenaService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _actividadService = actividadService ?? throw new ArgumentNullException(nameof(actividadService));
            _resenaService = resenaService ?? throw new ArgumentNullException(nameof(resenaService));
        }

        public void Cargar()
        {
            try
            {
                UsuarioDto usuario = _view.UsuarioActual;
                ActividadDto actividad = _actividadService.ObtenerActividad(_view.ActividadId);

                if (actividad == null || (!usuario.EsGestor && actividad.Estado == EstadosActividad.Borrador))
                {
                    _view.MostrarNoEncontrada();
                    return;
                }

                List<ResenaDto> resenas = _resenaService.ListarResenas(actividad.ActividadId);

                _view.MostrarActividad(actividad);
                _view.MostrarResenas(resenas, _resenaService.CalcularPromedio(resenas));
                _view.MostrarFormulario(_resenaService.ObtenerPermiso(actividad, usuario));
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarError("No fue posible cargar las reseñas en este momento.");
            }
        }

        public void Publicar()
        {
            try
            {
                ResultadoOperacion resultado = _resenaService.Publicar(
                    _view.ActividadId, _view.Calificacion, _view.Comentario, _view.UsuarioActual);

                if (resultado.Exito)
                {
                    _view.MostrarMensaje(resultado.Mensaje);
                    _view.LimpiarFormulario();
                    Cargar();
                }
                else
                {
                    _view.MostrarError(resultado.Mensaje);
                }
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarError("No fue posible publicar la reseña. Intente de nuevo.");
            }
        }

        private static void Registrar(Exception ex)
        {
            Trace.TraceError("[UniCampus] " + DateTime.Now.ToString("s") + " " + ex);
        }
    }
}
