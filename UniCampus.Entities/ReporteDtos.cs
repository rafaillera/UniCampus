using System;

namespace UniCampus.Entities
{
    /// <summary>
    /// Totales generales para las tarjetas de la página de reportes.
    /// </summary>
    public class ResumenActividadesDto
    {
        public int TotalActividades { get; set; }
        public int Publicadas { get; set; }

        /// <summary>Publicadas que todavía no han comenzado.</summary>
        public int Proximas { get; set; }

        /// <summary>Finalizadas, o publicadas cuya fecha de fin ya pasó.</summary>
        public int Realizadas { get; set; }

        /// <summary>Inactivas o canceladas.</summary>
        public int Inactivas { get; set; }

        /// <summary>Inscripciones vigentes (no canceladas) en todas las actividades.</summary>
        public int Inscripciones { get; set; }

        /// <summary>Promedio de todas las reseñas; null si aún no hay reseñas.</summary>
        public decimal? PromedioCalificacion { get; set; }
    }

    /// <summary>
    /// Una fila del reporte por tipo de actividad.
    /// </summary>
    public class ReporteTipoDto
    {
        public string Tipo { get; set; }
        public int Actividades { get; set; }
        public int Cupos { get; set; }
        public int Inscritos { get; set; }

        /// <summary>Porcentaje de cupos ocupados (0 a 100).</summary>
        public decimal PorcentajeOcupacion
        {
            get { return Cupos == 0 ? 0 : Math.Round(Inscritos * 100m / Cupos, 1); }
        }
    }

    /// <summary>
    /// Una fila del reporte por actividad: ocupación y valoración.
    /// </summary>
    public class ReporteActividadDto
    {
        public int ActividadId { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaInicio { get; set; }
        public string Estado { get; set; }
        public int Cupos { get; set; }
        public int Inscritos { get; set; }
        public int TotalResenas { get; set; }

        /// <summary>Promedio de calificaciones; null si no tiene reseñas.</summary>
        public decimal? PromedioCalificacion { get; set; }

        public decimal PorcentajeOcupacion
        {
            get { return Cupos == 0 ? 0 : Math.Round(Inscritos * 100m / Cupos, 1); }
        }
    }
}
