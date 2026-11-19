using System;
using System.Diagnostics;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>
    /// Presenter del detalle de una actividad: muestra los datos, los cupos
    /// disponibles y coordina la inscripción / cancelación con la BLL.
    /// </summary>
    public class DetalleActividadPresenter
    {
        private const string MensajeErrorGenerico =
            "Ocurrió un problema al procesar la solicitud. Intente de nuevo.";

        private readonly IDetalleActividadView _view;
        private readonly ActividadService _actividadService;
        private readonly InscripcionService _inscripcionService;

        public DetalleActividadPresenter(IDetalleActividadView view)
            : this(view, new ActividadService(), new InscripcionService())
        {
        }

        public DetalleActividadPresenter(IDetalleActividadView view, ActividadService actividadService, InscripcionService inscripcionService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _actividadService = actividadService ?? throw new ArgumentNullException(nameof(actividadService));
            _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
        }

        public void Cargar()
        {
            try
            {
                UsuarioDto usuario = _view.UsuarioActual;
                ActividadDto actividad = _actividadService.ObtenerActividad(_view.ActividadId);

                // Un estudiante no puede ver borradores aunque escriba el Id en la URL.
                if (actividad == null || (!usuario.EsGestor && actividad.Estado == EstadosActividad.Borrador))
                {
                    _view.MostrarNoEncontrada();
                    return;
                }

                _view.MostrarActividad(actividad);
                _view.MostrarOpcionesInscripcion(_inscripcionService.ObtenerOpciones(actividad, usuario));
                _view.MostrarInscritos(usuario.EsGestor
                    ? _inscripcionService.ListarInscritos(actividad.ActividadId, usuario)
                    : null);
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarError("No fue posible cargar la actividad en este momento.");
            }
        }

        public void Inscribirse()
        {
            Ejecutar(() => _inscripcionService.Inscribir(_view.ActividadId, _view.UsuarioActual));
        }

        public void CancelarInscripcion()
        {
            Ejecutar(() => _inscripcionService.Cancelar(_view.ActividadId, _view.UsuarioActual));
        }

        /// <summary>Ejecuta la operación, muestra el resultado y recarga los cupos.</summary>
        private void Ejecutar(Func<ResultadoOperacion> operacion)
        {
            try
            {
                ResultadoOperacion resultado = operacion();

                if (resultado.Exito)
                    _view.MostrarMensaje(resultado.Mensaje);
                else
                    _view.MostrarError(resultado.Mensaje);

                Cargar();
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarError(MensajeErrorGenerico);
            }
        }

        private static void Registrar(Exception ex)
        {
            Trace.TraceError("[UniCampus] " + DateTime.Now.ToString("s") + " " + ex);
        }
    }
}
