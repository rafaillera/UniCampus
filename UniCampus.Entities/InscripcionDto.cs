using System;

namespace UniCampus.Entities
{
    /// <summary>
    /// Una fila de dbo.Inscripcion con los datos de la actividad y del estudiante
    /// que se necesitan en pantalla (se llenan con JOIN según la consulta).
    /// </summary>
    public class InscripcionDto
    {
        public int InscripcionId { get; set; }
        public int ActividadId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaInscripcion { get; set; }

        /// <summary>Activa, Cancelada o Asistió.</summary>
        public string Estado { get; set; }

        // Datos de la actividad (para "Mis inscripciones")
        public string ActividadTitulo { get; set; }
        public string TipoNombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public string Lugar { get; set; }
        public string EstadoActividad { get; set; }

        // Datos del estudiante (para la lista de inscritos)
        public string NombreEstudiante { get; set; }
        public string Correo { get; set; }
        public string Programa { get; set; }

        /// <summary>Lo calcula la BLL: true si el estudiante todavía puede cancelar.</summary>
        public bool PuedeCancelar { get; set; }
    }

    /// <summary>
    /// Resultado de la transacción de inscripción en la capa DAL.
    /// </summary>
    public enum CodigoInscripcion
    {
        Inscrito,
        Reactivada,
        YaInscrito,
        SinCupos,
        NoDisponible,
        NoExiste
    }

    /// <summary>
    /// Qué botones ve el estudiante en el detalle de una actividad. Lo decide la BLL.
    /// </summary>
    public class OpcionesInscripcionDto
    {
        public bool PuedeInscribirse { get; set; }
        public bool PuedeCancelar { get; set; }

        /// <summary>Texto informativo, por ejemplo "Ya estás inscrito en esta actividad."</summary>
        public string Mensaje { get; set; }
    }
}
