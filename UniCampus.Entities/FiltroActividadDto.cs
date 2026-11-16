using System;

namespace UniCampus.Entities
{
    /// <summary>
    /// Criterios de búsqueda del catálogo. Un criterio en null (o vacío)
    /// significa "no filtrar por ese campo".
    /// </summary>
    public class FiltroActividadDto
    {
        /// <summary>Texto a buscar en el título, la descripción o el lugar.</summary>
        public string Texto { get; set; }

        public int? TipoId { get; set; }

        /// <summary>Actividades que inician desde esta fecha (inclusive).</summary>
        public DateTime? FechaDesde { get; set; }

        /// <summary>Actividades que inician hasta esta fecha (inclusive).</summary>
        public DateTime? FechaHasta { get; set; }

        public string Modalidad { get; set; }

        /// <summary>Estado de la actividad. En el catálogo de estudiantes la BLL lo fija en Publicada.</summary>
        public string Estado { get; set; }

        /// <summary>true = solo actividades que todavía tienen cupos libres.</summary>
        public bool SoloConCupos { get; set; }
    }
}
