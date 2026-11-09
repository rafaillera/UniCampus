using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>
    /// Presenter del patrón MVP para la pantalla de actividades.
    /// Recibe las acciones de la Vista, llama a la capa de negocio (BLL)
    /// y le indica a la Vista qué mostrar. Nunca accede a SQL ni a controles web.
    /// </summary>
    public class ActividadesPresenter
    {
        private const string MensajeErrorGenerico =
            "Ocurrió un problema al procesar la solicitud. Intente de nuevo o contacte al administrador.";

        private readonly IActividadesView _view;
        private readonly ActividadService _actividadService;
        private readonly TipoActividadService _tipoService;

        public ActividadesPresenter(IActividadesView view)
            : this(view, new ActividadService(), new TipoActividadService())
        {
        }

        public ActividadesPresenter(IActividadesView view, ActividadService actividadService, TipoActividadService tipoService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _actividadService = actividadService ?? throw new ArgumentNullException(nameof(actividadService));
            _tipoService = tipoService ?? throw new ArgumentNullException(nameof(tipoService));
        }

        /// <summary>
        /// Carga inicial de la página: tipos de actividad y listado de actividades publicadas.
        /// </summary>
        public void Cargar()
        {
            try
            {
                _view.MostrarTipos(_tipoService.ListarTipos());
                CargarActividades();
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarActividades(new List<ActividadDto>());
                _view.MostrarError("No fue posible cargar la información de actividades en este momento.");
            }
        }

        /// <summary>
        /// Envía la actividad a la capa de negocio para validarla y guardarla.
        /// </summary>
        public void GuardarActividad(ActividadDto dto)
        {
            try
            {
                ResultadoOperacion resultado = _actividadService.RegistrarActividad(dto);

                if (resultado.Exito)
                {
                    _view.MostrarMensaje(resultado.Mensaje);
                    _view.LimpiarFormulario();
                    CargarActividades();
                }
                else
                {
                    _view.MostrarError(resultado.Mensaje);
                }
            }
            catch (Exception ex)
            {
                // El detalle técnico (por ejemplo, una SqlException) solo se registra en el log;
                // al usuario se le muestra un mensaje amigable.
                Registrar(ex);
                _view.MostrarError(MensajeErrorGenerico);
            }
        }

        private void CargarActividades()
        {
            _view.MostrarActividades(_actividadService.ListarActividades());
        }

        private static void Registrar(Exception ex)
        {
            Trace.TraceError("[UniCampus] " + DateTime.Now.ToString("s") + " " + ex);
        }
    }
}
