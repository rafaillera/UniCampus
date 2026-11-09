namespace UniCampus.Entities
{
    /// <summary>
    /// Resultado que devuelve la capa de negocio (BLL) a la capa de presentación.
    /// Permite informar si la operación fue exitosa y el mensaje a mostrar,
    /// sin lanzar excepciones para errores de validación.
    /// </summary>
    public class ResultadoOperacion
    {
        public bool Exito { get; private set; }
        public string Mensaje { get; private set; }

        /// <summary>Id generado por la base de datos (por ejemplo, el ActividadId insertado).</summary>
        public int IdGenerado { get; private set; }

        private ResultadoOperacion() { }

        public static ResultadoOperacion Ok(string mensaje, int idGenerado = 0)
        {
            return new ResultadoOperacion { Exito = true, Mensaje = mensaje, IdGenerado = idGenerado };
        }

        public static ResultadoOperacion Fallo(string mensaje)
        {
            return new ResultadoOperacion { Exito = false, Mensaje = mensaje };
        }
    }
}
