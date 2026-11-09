using System.Configuration;
using System.Data.SqlClient;

namespace UniCampus.DAL
{
    /// <summary>
    /// Centraliza la lectura de la cadena de conexión "UniCampusDb" definida
    /// en el Web.config del proyecto UniCampus.Web.
    /// </summary>
    internal static class ConexionDb
    {
        private const string NombreCadena = "UniCampusDb";

        public static SqlConnection Crear()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[NombreCadena];
            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "No se encontró la cadena de conexión '" + NombreCadena + "' en Web.config.");
            }

            return new SqlConnection(settings.ConnectionString);
        }
    }
}
