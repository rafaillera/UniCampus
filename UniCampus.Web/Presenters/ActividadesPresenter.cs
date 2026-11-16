using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>
    /// Presenter del patrón MVP para la pantalla de gestión de actividades.
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
        /// Carga inicial de la página: tipos de actividad y listado de todas las actividades.
        /// </summary>
        public void Cargar()
        {
            try
            {
                _view.MostrarTipos(_tipoService.ListarTipos());
                _view.MostrarModoEdicion(false);
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
        /// Registra (ActividadId = 0) o actualiza (ActividadId > 0) una actividad.
        /// </summary>
        public void GuardarActividad(ActividadDto dto)
        {
            try
            {
                bool esEdicion = dto != null && dto.ActividadId > 0;

                ResultadoOperacion resultado = esEdicion
                    ? _actividadService.ActualizarActividad(dto)
                    : _actividadService.RegistrarActividad(dto);

                if (resultado.Exito)
                {
                    _view.MostrarMensaje(resultado.Mensaje);
                    _view.LimpiarFormulario();
                    _view.MostrarModoEdicion(false);
                    _view.MostrarDetalle(null);
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

        /// <summary>Carga una actividad en el formulario para editarla.</summary>
        public void EditarActividad(int actividadId)
        {
            try
            {
                ActividadDto actividad = _actividadService.ObtenerActividad(actividadId);
                if (actividad == null)
                {
                    _view.MostrarError("La actividad seleccionada no existe.");
                    CargarActividades();
                    return;
                }

                _view.MostrarDetalle(null);
                _view.CargarEnFormulario(actividad);
                _view.MostrarModoEdicion(true);
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarError(MensajeErrorGenerico);
            }
        }

        /// <summary>Descarta la edición y vuelve al modo de registro.</summary>
        public void CancelarEdicion()
        {
            _view.LimpiarFormulario();
            _view.MostrarModoEdicion(false);
        }

        /// <summary>Muestra el panel de detalle de una actividad.</summary>
        public void VerDetalle(int actividadId)
        {
            try
            {
                ActividadDto actividad = _actividadService.ObtenerActividad(actividadId);
                if (actividad == null)
                {
                    _view.MostrarError("La actividad seleccionada no existe.");
                    return;
                }

                _view.MostrarDetalle(actividad);
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarError(MensajeErrorGenerico);
            }
        }

        /// <summary>Desactiva, publica o cancela una actividad.</summary>
        public void CambiarEstado(int actividadId, string nuevoEstado)
        {
            try
            {
                ResultadoOperacion resultado = _actividadService.CambiarEstado(actividadId, nuevoEstado);

                if (resultado.Exito)
                    _view.MostrarMensaje(resultado.Mensaje);
                else
                    _view.MostrarError(resultado.Mensaje);

                _view.MostrarDetalle(null);
                CargarActividades();
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarError(MensajeErrorGenerico);
            }
        }

        private void CargarActividades()
        {
            _view.MostrarActividades(_actividadService.ListarTodas());
        }

        private static void Registrar(Exception ex)
        {
            Trace.TraceError("[UniCampus] " + DateTime.Now.ToString("s") + " " + ex);
        }
    }
}
