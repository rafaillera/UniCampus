using System;
using System.Collections.Generic;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>
    /// Reglas de negocio de las inscripciones:
    ///  - Solo estudiantes autenticados se inscriben (el UsuarioId viene de la sesión).
    ///  - Solo en actividades publicadas que no hayan comenzado.
    ///  - Sin duplicados y sin superar los cupos (lo garantiza la transacción de la DAL).
    ///  - Se puede cancelar únicamente antes del día de inicio.
    /// </summary>
    public class InscripcionService
    {
        private readonly InscripcionRepository _repository;
        private readonly ActividadRepository _actividadRepository;

        public InscripcionService() : this(new InscripcionRepository(), new ActividadRepository())
        {
        }

        public InscripcionService(InscripcionRepository repository, ActividadRepository actividadRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _actividadRepository = actividadRepository ?? throw new ArgumentNullException(nameof(actividadRepository));
        }

        public ResultadoOperacion Inscribir(int actividadId, UsuarioDto usuario)
        {
            string error = ValidarEstudiante(usuario);
            if (error != null) return ResultadoOperacion.Fallo(error);

            ActividadDto actividad = actividadId > 0 ? _actividadRepository.ObtenerPorId(actividadId) : null;
            if (actividad == null)
                return ResultadoOperacion.Fallo("La actividad no existe.");

            if (actividad.Estado != EstadosActividad.Publicada)
                return ResultadoOperacion.Fallo("La actividad no está abierta a inscripciones.");

            if (actividad.FechaInicio.Date < DateTime.Today)
                return ResultadoOperacion.Fallo("No es posible inscribirse: la actividad ya comenzó.");

            CodigoInscripcion codigo = _repository.Inscribir(actividadId, usuario.UsuarioId);

            switch (codigo)
            {
                case CodigoInscripcion.Inscrito:
                case CodigoInscripcion.Reactivada:
                    return ResultadoOperacion.Ok("Te inscribiste correctamente en \"" + actividad.Titulo + "\".", actividadId);
                case CodigoInscripcion.YaInscrito:
                    return ResultadoOperacion.Fallo("Ya estás inscrito en esta actividad.");
                case CodigoInscripcion.SinCupos:
                    return ResultadoOperacion.Fallo("Lo sentimos, ya no hay cupos disponibles para esta actividad.");
                case CodigoInscripcion.NoDisponible:
                    return ResultadoOperacion.Fallo("La actividad ya no está abierta a inscripciones.");
                default:
                    return ResultadoOperacion.Fallo("La actividad no existe.");
            }
        }

        public ResultadoOperacion Cancelar(int actividadId, UsuarioDto usuario)
        {
            string error = ValidarEstudiante(usuario);
            if (error != null) return ResultadoOperacion.Fallo(error);

            ActividadDto actividad = actividadId > 0 ? _actividadRepository.ObtenerPorId(actividadId) : null;
            if (actividad == null)
                return ResultadoOperacion.Fallo("La actividad no existe.");

            if (!PermiteCancelar(EstadosInscripcion.Activa, actividad.FechaInicio))
                return ResultadoOperacion.Fallo("Solo puedes cancelar tu inscripción antes del día de inicio de la actividad.");

            if (!_repository.Cancelar(actividadId, usuario.UsuarioId))
                return ResultadoOperacion.Fallo("No tienes una inscripción activa en esta actividad.");

            return ResultadoOperacion.Ok("Cancelaste tu inscripción en \"" + actividad.Titulo + "\". Tu cupo quedó libre.", actividadId);
        }

        /// <summary>
        /// Decide qué botones ve el estudiante en el detalle de la actividad.
        /// </summary>
        public OpcionesInscripcionDto ObtenerOpciones(ActividadDto actividad, UsuarioDto usuario)
        {
            var opciones = new OpcionesInscripcionDto();
            if (actividad == null || usuario == null || !usuario.EsEstudiante) return opciones;

            string estado = _repository.ObtenerEstado(actividad.ActividadId, usuario.UsuarioId);
            bool inscrito = estado != null && estado != EstadosInscripcion.Cancelada;

            if (estado == EstadosInscripcion.Asistio)
            {
                opciones.Mensaje = "Asististe a esta actividad. ¡Cuéntanos qué te pareció en las reseñas!";
            }
            else if (inscrito)
            {
                opciones.Mensaje = "Ya estás inscrito en esta actividad.";
                opciones.PuedeCancelar = PermiteCancelar(estado, actividad.FechaInicio);
            }
            else if (actividad.Estado != EstadosActividad.Publicada)
            {
                opciones.Mensaje = "Esta actividad no está abierta a inscripciones.";
            }
            else if (actividad.FechaInicio.Date < DateTime.Today)
            {
                opciones.Mensaje = "Las inscripciones cerraron: la actividad ya comenzó.";
            }
            else if (actividad.CuposDisponibles == 0)
            {
                opciones.Mensaje = "No quedan cupos disponibles.";
            }
            else
            {
                opciones.PuedeInscribirse = true;
            }

            return opciones;
        }

        public List<InscripcionDto> ListarMisInscripciones(UsuarioDto usuario)
        {
            if (usuario == null) return new List<InscripcionDto>();

            List<InscripcionDto> inscripciones = _repository.ObtenerPorUsuario(usuario.UsuarioId);
            foreach (InscripcionDto inscripcion in inscripciones)
            {
                inscripcion.PuedeCancelar = PermiteCancelar(inscripcion.Estado, inscripcion.FechaInicio);
            }

            return inscripciones;
        }

        /// <summary>
        /// Lista de inscritos de una actividad. Solo para organizadores y administradores:
        /// a un estudiante se le devuelve una lista vacía (protege datos personales).
        /// </summary>
        public List<InscripcionDto> ListarInscritos(int actividadId, UsuarioDto usuario)
        {
            if (usuario == null || !usuario.EsGestor) return new List<InscripcionDto>();
            return _repository.ObtenerPorActividad(actividadId);
        }

        private static string ValidarEstudiante(UsuarioDto usuario)
        {
            if (usuario == null) return "Debes iniciar sesión para continuar.";
            if (!usuario.EsEstudiante) return "Solo los estudiantes pueden inscribirse en actividades.";
            return null;
        }

        /// <summary>Regla única: se cancela una inscripción activa antes del día de inicio.</summary>
        private static bool PermiteCancelar(string estadoInscripcion, DateTime fechaInicio)
        {
            return estadoInscripcion == EstadosInscripcion.Activa && fechaInicio.Date > DateTime.Today;
        }
    }
}
