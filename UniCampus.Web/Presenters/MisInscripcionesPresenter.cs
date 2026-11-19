using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>
    /// Presenter de "Mis inscripciones" del estudiante.
    /// </summary>
    public class MisInscripcionesPresenter
    {
        private readonly IMisInscripcionesView _view;
        private readonly InscripcionService _service;

        public MisInscripcionesPresenter(IMisInscripcionesView view) : this(view, new InscripcionService())
        {
        }

        public MisInscripcionesPresenter(IMisInscripcionesView view, InscripcionService service)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void Cargar()
        {
            try
            {
                _view.MostrarInscripciones(_service.ListarMisInscripciones(_view.UsuarioActual));
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarInscripciones(new List<InscripcionDto>());
                _view.MostrarError("No fue posible cargar tus inscripciones en este momento.");
            }
        }

        public void Cancelar(int actividadId)
        {
            try
            {
                ResultadoOperacion resultado = _service.Cancelar(actividadId, _view.UsuarioActual);

                if (resultado.Exito)
                    _view.MostrarMensaje(resultado.Mensaje);
                else
                    _view.MostrarError(resultado.Mensaje);

                Cargar();
            }
            catch (Exception ex)
            {
                Registrar(ex);
                _view.MostrarError("No fue posible cancelar la inscripción. Intente de nuevo.");
            }
        }

        private static void Registrar(Exception ex)
        {
            Trace.TraceError("[UniCampus] " + DateTime.Now.ToString("s") + " " + ex);
        }
    }
}
