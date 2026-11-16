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

    /// <summary>
    /// Roles de usuario. Coinciden con la restricción CK_Usuario_Rol.
    /// </summary>
    public static class Roles
    {
        public const string Estudiante = "Estudiante";
        public const string Organizador = "Organizador";
        public const string Administrador = "Administrador";

        /// <summary>Roles que pueden crear, editar y desactivar actividades.</summary>
        public static readonly string[] Gestores = { Organizador, Administrador };
    }

    /// <summary>
    /// Estados de una inscripción. Coinciden con la restricción CK_Inscripcion_Estado.
    /// Una inscripción "vigente" es cualquiera que no esté Cancelada.
    /// </summary>
    public static class EstadosInscripcion
    {
        public const string Activa = "Activa";
        public const string Cancelada = "Cancelada";
        public const string Asistio = "Asistió";
    }
}
