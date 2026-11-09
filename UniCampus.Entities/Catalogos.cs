namespace UniCampus.Entities
{
    /// <summary>
    /// Valores permitidos para Estado. Coinciden con la restricción
    /// CK_Actividad_Estado de la base de datos.
    /// </summary>
    public static class EstadosActividad
    {
        public const string Borrador = "Borrador";
        public const string Publicada = "Publicada";
        public const string Cancelada = "Cancelada";
        public const string Finalizada = "Finalizada";
        public const string Inactiva = "Inactiva";

        public static readonly string[] Todos = { Borrador, Publicada, Cancelada, Finalizada, Inactiva };
    }

    /// <summary>
    /// Valores permitidos para Modalidad. Coinciden con la restricción
    /// CK_Actividad_Modalidad de la base de datos.
    /// </summary>
    public static class Modalidades
    {
        public const string Presencial = "Presencial";
        public const string Virtual = "Virtual";
        public const string Hibrida = "Híbrida";

        public static readonly string[] Todas = { Presencial, Virtual, Hibrida };
    }
}
