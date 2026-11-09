namespace UniCampus.Entities
{
    /// <summary>
    /// Representa una fila de dbo.TipoActividad. Se usa para llenar el
    /// DropDownList de tipos en el formulario de registro.
    /// </summary>
    public class TipoActividadDto
    {
        public int TipoId { get; set; }
        public string Nombre { get; set; }
    }
}
