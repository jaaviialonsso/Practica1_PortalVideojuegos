using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Practica1_PortalVideojuegos
{
    public partial class F_Login : Window
    {
        // Añadimos un contador global para los intentos fallidos
        private int intentosFallidos = 0;

        public F_Login()
        {
            InitializeComponent();
        }

        // Añadimos "async" al método para poder usar el bloqueo temporal
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // 1. Verificación de que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                lblMensaje.Foreground = new SolidColorBrush(Colors.Red);
                lblMensaje.Text = "Introduzca datos";
                return;
            }

            // 2. Simulación de una lista de usuarios en memoria
            string[,] usuarios = new string[,]
            {
                { "admin", "1234" },
                { "alumno", "dam" }
            };

            bool usuarioExiste = false;
            bool loginCorrecto = false;

            // 3. Comparación de credenciales
            for (int i = 0; i < usuarios.GetLength(0); i++)
            {
                if (txtUsuario.Text == usuarios[i, 0])
                {
                    usuarioExiste = true;
                    if (txtPassword.Password == usuarios[i, 1])
                    {
                        loginCorrecto = true;
                    }
                    break;
                }
            }

            // 4. Navegación y mensajes dinámicos
            if (loginCorrecto)
            {
                intentosFallidos = 0; // Reiniciamos el contador si entra con éxito
                lblMensaje.Foreground = new SolidColorBrush(Colors.LightGreen);
                lblMensaje.Text = "¡Acceso concedido!";

                MainWindow principal = new MainWindow();
                principal.Show();
                this.Hide();
            }
            else
            {
                // Si falla (por usuario o contraseña), sumamos un intento
                intentosFallidos++;

                if (intentosFallidos >= 3)
                {
                    // TC05: Bloqueo temporal del botón
                    lblMensaje.Foreground = new SolidColorBrush(Colors.Red);
                    lblMensaje.Text = "Usuario bloqueado temporalmente";
                    btnLogin.IsEnabled = false; // Desactiva el botón

                    // Espera de 5 segundos
                    await Task.Delay(5000);

                    // Tras 5 segundos, se reinicia y vuelve a dejar intentar
                    intentosFallidos = 0;
                    btnLogin.IsEnabled = true;
                    lblMensaje.Text = "";
                }
                else if (usuarioExiste)
                {
                    lblMensaje.Foreground = new SolidColorBrush(Colors.Red);
                    lblMensaje.Text = "La contraseña introducida no es correcta";
                }
                else
                {
                    lblMensaje.Foreground = new SolidColorBrush(Colors.Red);
                    lblMensaje.Text = "El usuario introducido no existe";
                }
            }
        }

        // Lógica del botón Salir
        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
