using System;

namespace UniCampus.Entities
{
    /// <summary>
    /// Objeto de transferencia de datos que representa una fila de la tabla
    /// dbo.ActividadAcademica. Viaja entre las capas Web, BLL y DAL.
    /// No contiene lógica ni referencias a System.Web.
    /// </summary>
    public class ActividadDto
    {
        public int ActividadId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        /// <summary>Clave foránea a dbo.TipoActividad.</summary>
        public int TipoId { get; set; }

        /// <summary>Nombre del tipo (se llena al consultar con JOIN; solo lectura en pantalla).</summary>
        public string TipoNombre { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }

        /// <summary>Presencial, Virtual o Híbrida.</summary>
        public string Modalidad { get; set; }

        public string Lugar { get; set; }

        /// <summary>Capacidad total de la actividad.</summary>
        public int Cupos { get; set; }

        /// <summary>Borrador, Publicada, Cancelada, Finalizada o Inactiva.</summary>
        public string Estado { get; set; }

        /// <summary>Inscripciones vigentes (no canceladas). Se calcula al consultar.</summary>
        public int Inscritos { get; set; }

        /// <summary>Cupos libres = Cupos - Inscritos (nunca negativo).</summary>
        public int CuposDisponibles
        {
            get { return Math.Max(0, Cupos - Inscritos); }
        }
    }
}
