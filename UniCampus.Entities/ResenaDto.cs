using System;

namespace UniCampus.Entities
{
    /// <summary>
    /// Una fila de dbo.Resena: valoración de 1 a 5 estrellas y comentario.
    /// </summary>
    public class ResenaDto
    {
        public int ResenaId { get; set; }
        public int ActividadId { get; set; }
        public int UsuarioId { get; set; }

        /// <summary>Nombre corto del autor, por ejemplo "Laura M." (se arma en la consulta).</summary>
        public string Autor { get; set; }

        /// <summary>Entero de 1 a 5 (CK_Resena_Calificacion).</summary>
        public int Calificacion { get; set; }

        /// <summary>Opcional, máximo 1000 caracteres. Se muestra SIEMPRE codificado (anti-XSS).</summary>
        public string Comentario { get; set; }

        public DateTime FechaCreacion { get; set; }

        /// <summary>Representación visual, por ejemplo ★★★★☆.</summary>
        public string Estrellas
        {
            get
            {
                int valor = Math.Max(0, Math.Min(5, Calificacion));
                return new string('★', valor) + new string('☆', 5 - valor);
            }
        }
    }

    /// <summary>
    /// Indica si el usuario puede publicar una reseña y, si no, por qué.
    /// </summary>
    public class PermisoResenaDto
    {
        public bool Permitido { get; set; }
        public string Motivo { get; set; }
    }
}
