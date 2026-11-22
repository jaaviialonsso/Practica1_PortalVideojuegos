using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using Practica1_PortalVideojuegos.Data;

namespace Practica1_PortalVideojuegos
{
    public partial class F_Login : Window
    {
        // Contador de intentos fallidos (se mantiene de la práctica anterior)
        private int intentosFallidos = 0;

        public F_Login()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------------
        // LOGIN: comprueba usuario y contraseña contra la tabla Usuarios
        // ---------------------------------------------------------------
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Password;

            // 1. Campos vacíos: se comprueba ANTES de conectar, así que con los
            //    campos en blanco no se llega a consultar la base de datos.
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                MostrarMensaje("Introduzca datos", Colors.Red);
                return;
            }

            BloquearBotones(true);
            try
            {
                // 2. Consulta parametrizada. @usuario y @password se envían como datos,
                //    nunca como parte del SQL, por lo que un texto como ' OR '1'='1
                //    no puede cambiar la consulta (protección contra inyección SQL).
                const string sql = "SELECT COUNT(*) FROM Usuarios " +
                                   "WHERE nombre_usuario = @usuario AND password = @password";

                using MySqlConnection conexion = await DatabaseHelper.AbrirConexionAsync();
                using var comando = new MySqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@password", password);

                // COUNT(*) solo puede ser 0 o 1 porque nombre_usuario es UNIQUE
                long coincidencias = Convert.ToInt64(await comando.ExecuteScalarAsync());

                // 3. Navegación y mensajes
                if (coincidencias == 1)
                {
                    intentosFallidos = 0;
                    MostrarMensaje("¡Acceso concedido!", Colors.LightGreen);

                    MainWindow principal = new MainWindow();
                    principal.Show();
                    this.Hide();
                }
                else
                {
                    await RegistrarIntentoFallido();
                }
            }
            catch (MySqlException ex)
            {
                // Servidor parado, usuario de BD incorrecto... la aplicación sigue abierta
                MostrarErrorConexion(ex);
            }
            catch (Exception ex)
            {
                // Cualquier otro fallo inesperado tampoco debe cerrar el programa
                MostrarMensaje("Error inesperado: " + ex.Message, Colors.Red);
            }
            finally
            {
                BloquearBotones(false);
            }
        }

        // ---------------------------------------------------------------
        // NUEVO USUARIO: inserta en la tabla y avisa si ya existe
        // ---------------------------------------------------------------
        private async void btnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                MostrarMensaje("Introduzca datos", Colors.Red);
                return;
            }

            BloquearBotones(true);
            try
            {
                // También parametrizada: un INSERT concatenado sería igual de vulnerable
                const string sql = "INSERT INTO Usuarios (nombre_usuario, password) " +
                                   "VALUES (@usuario, @password)";

                using MySqlConnection conexion = await DatabaseHelper.AbrirConexionAsync();
                using var comando = new MySqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@password", password);
                await comando.ExecuteNonQueryAsync();

                MostrarMensaje($"Usuario \"{usuario}\" dado de alta correctamente", Colors.LightGreen);
                txtPassword.Clear();
            }
            // El duplicado lo detecta la propia base de datos gracias al UNIQUE de
            // nombre_usuario: MySQL devuelve el error 1062 (clave duplicada).
            // Así se hace con una sola consulta y sin riesgo de que dos altas
            // simultáneas del mismo nombre pasen una comprobación previa.
            catch (MySqlException ex) when (ex.Number == (int)MySqlErrorCode.DuplicateKeyEntry)
            {
                MostrarMensaje("El usuario ya existe", Colors.Red);
            }
            catch (MySqlException ex)
            {
                MostrarErrorConexion(ex);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error inesperado: " + ex.Message, Colors.Red);
            }
            finally
            {
                BloquearBotones(false);
            }
        }

        // Lógica del botón Salir
        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // ---------------------------------------------------------------
        // Métodos auxiliares
        // ---------------------------------------------------------------

        // Mismo mensaje tanto si el usuario no existe como si la contraseña es
        // incorrecta: así no se dan pistas de qué usuarios hay registrados.
        private async Task RegistrarIntentoFallido()
        {
            intentosFallidos++;

            if (intentosFallidos >= 3)
            {
                // Bloqueo temporal de la práctica anterior: 5 segundos sin poder entrar
                MostrarMensaje("Usuario bloqueado temporalmente", Colors.Red);
                BloquearBotones(true);
                await Task.Delay(5000);
                intentosFallidos = 0;
                lblMensaje.Text = "";
            }
            else
            {
                MostrarMensaje("Usuario o contraseña incorrectos", Colors.Red);
            }
        }

        private void MostrarErrorConexion(MySqlException ex)
        {
            // 1042 = no se puede conectar con el servidor (servicio parado o puerto mal)
            if (ex.Number == (int)MySqlErrorCode.UnableToConnectToHost)
            {
                MostrarMensaje("Error de conexión: el servidor MySQL no responde", Colors.Red);
            }
            else
            {
                MostrarMensaje("Error de base de datos: " + ex.Message, Colors.Red);
            }
        }

        private void MostrarMensaje(string texto, Color color)
        {
            lblMensaje.Foreground = new SolidColorBrush(color);
            lblMensaje.Text = texto;
        }

        // Mientras se consulta la BD se desactivan los botones para evitar dobles clics
        private void BloquearBotones(bool bloquear)
        {
            btnLogin.IsEnabled = !bloquear;
            btnNuevoUsuario.IsEnabled = !bloquear;
        }
    }
}
