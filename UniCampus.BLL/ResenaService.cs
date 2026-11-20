using System;
using System.Collections.Generic;
using System.Linq;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>
    /// Reglas de las reseñas:
    ///  - Solo estudiantes inscritos en la actividad (Activa o Asistió).
    ///  - Solo cuando la actividad ya comenzó (no se califica lo que no ha pasado).
    ///  - Una reseña por estudiante y actividad.
    ///  - Calificación entera de 1 a 5; comentario opcional de máximo 1000 caracteres.
    /// </summary>
    public class ResenaService
    {
        public const int CalificacionMinima = 1;
        public const int CalificacionMaxima = 5;
        public const int LongitudMaximaComentario = 1000;

        private readonly ResenaRepository _repository;
        private readonly ActividadRepository _actividadRepository;

        public ResenaService() : this(new ResenaRepository(), new ActividadRepository())
        {
        }

        public ResenaService(ResenaRepository repository, ActividadRepository actividadRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _actividadRepository = actividadRepository ?? throw new ArgumentNullException(nameof(actividadRepository));
        }

        public List<ResenaDto> ListarResenas(int actividadId)
        {
            return actividadId > 0 ? _repository.ObtenerPorActividad(actividadId) : new List<ResenaDto>();
        }

        /// <summary>Promedio de calificaciones redondeado a 1 decimal; null si no hay reseñas.</summary>
        public decimal? CalcularPromedio(IList<ResenaDto> resenas)
        {
            if (resenas == null || resenas.Count == 0) return null;
            return Math.Round((decimal)resenas.Average(r => r.Calificacion), 1);
        }

        /// <summary>Indica si el usuario puede reseñar la actividad y, si no, el motivo.</summary>
        public PermisoResenaDto ObtenerPermiso(ActividadDto actividad, UsuarioDto usuario)
        {
            if (actividad == null)
                return Denegar("La actividad no existe.");

            if (usuario == null || !usuario.EsEstudiante)
                return Denegar("Solo los estudiantes inscritos pueden publicar reseñas.");

            if (actividad.FechaInicio.Date > DateTime.Today)
                return Denegar("Podrás calificar esta actividad cuando se haya realizado.");

            if (!_repository.EstaInscrito(actividad.ActividadId, usuario.UsuarioId))
                return Denegar("Solo los estudiantes que se inscribieron pueden reseñar esta actividad.");

            if (_repository.ExisteResena(actividad.ActividadId, usuario.UsuarioId))
                return Denegar("Ya publicaste tu reseña para esta actividad. ¡Gracias!");

            return new PermisoResenaDto { Permitido = true };
        }

        public ResultadoOperacion Publicar(int actividadId, int calificacion, string comentario, UsuarioDto usuario)
        {
            ActividadDto actividad = actividadId > 0 ? _actividadRepository.ObtenerPorId(actividadId) : null;

            PermisoResenaDto permiso = ObtenerPermiso(actividad, usuario);
            if (!permiso.Permitido)
                return ResultadoOperacion.Fallo(permiso.Motivo);

            comentario = comentario == null ? null : comentario.Trim();

            var errores = new List<string>();

            if (calificacion < CalificacionMinima || calificacion > CalificacionMaxima)
                errores.Add("Seleccione una calificación de 1 a 5 estrellas.");

            if (comentario != null && comentario.Length > LongitudMaximaComentario)
                errores.Add("El comentario no puede superar " + LongitudMaximaComentario + " caracteres.");

            if (errores.Count > 0)
                return ResultadoOperacion.Fallo(string.Join(" ", errores));

            var resena = new ResenaDto
            {
                ActividadId = actividadId,
                UsuarioId = usuario.UsuarioId,
                Calificacion = calificacion,
                Comentario = string.IsNullOrEmpty(comentario) ? null : comentario
            };

            if (!_repository.Insertar(resena))
                return ResultadoOperacion.Fallo("Ya publicaste tu reseña para esta actividad.");

            return ResultadoOperacion.Ok("¡Gracias! Tu reseña se publicó correctamente.");
        }

        private static PermisoResenaDto Denegar(string motivo)
        {
            return new PermisoResenaDto { Permitido = false, Motivo = motivo };
        }
    }
}
