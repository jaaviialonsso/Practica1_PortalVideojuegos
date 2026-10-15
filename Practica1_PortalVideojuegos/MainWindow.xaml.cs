using System.Windows;

namespace Practica1_PortalVideojuegos
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Esta es la definición exacta que tu programa está buscando
        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            F_Login login = new F_Login();
            login.Show();
        }
    }
}