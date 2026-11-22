using System.Data;
using MySql.Data.MySqlClient;

namespace Practica1_PortalVideojuegos.Data
{
    /// <summary>
    /// Clase encargada de todo lo relacionado con la conexión a MySQL.
    /// La cadena de conexión solo está aquí: si cambia el servidor, el puerto
    /// o la contraseña, se modifica en un único sitio y ninguna ventana la repite.
    /// </summary>
    public static class DatabaseHelper
    {
        // Me conecto con el usuario 'catalogo_app' (creado en database.sql), que solo
        // puede hacer SELECT e INSERT sobre la tabla Usuarios. Así la aplicación nunca
        // usa root: si alguien saca esta cadena del programa, no puede borrar nada.
        // Connection Timeout=5 -> si MySQL está parado, el error salta a los 5 segundos
        // en lugar de esperar los 15 que vienen por defecto.
        private const string CadenaConexion =
            "Server=localhost;Port=3306;Database=catalogo_videojuegos;" +
            "Uid=catalogo_app;Pwd=Catalogo_2025!;" +
            "Connection Timeout=5;CharSet=utf8mb4;";

        /// <summary>
        /// Crea una conexión y la abre. Hay que usarla dentro de un 'using'
        /// para que se cierre sola al terminar, incluso si salta una excepción.
        /// Si el servidor no responde, lanza MySqlException y la ventana que la
        /// pidió se encarga de mostrar el mensaje (aquí no se toca la interfaz).
        /// </summary>
        public static async Task<MySqlConnection> AbrirConexionAsync()
        {
            var conexion = new MySqlConnection(CadenaConexion);
            try
            {
                // Abrir en asíncrono evita que la ventana se quede congelada
                // mientras se intenta conectar (se nota mucho con el servidor parado).
                await conexion.OpenAsync();
                return conexion;
            }
            catch
            {
                // Si no se ha podido abrir, se libera el objeto antes de relanzar el error.
                conexion.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Cierra la conexión si sigue abierta. Con 'using' no hace falta llamarlo
        /// (Dispose ya la cierra), pero lo dejo para usarlo de forma explícita.
        /// </summary>
        public static void CerrarConexion(MySqlConnection? conexion)
        {
            if (conexion != null && conexion.State != ConnectionState.Closed)
            {
                conexion.Close();
            }
        }
    }
}
