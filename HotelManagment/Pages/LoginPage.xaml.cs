using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HotelManagment.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Provera šifre
            if (PasswordBox.Password == "vilabojana123")
            {
                try
                {
                    var homePage = App.ServiceProvider.GetRequiredService<HomePage>();
                    NavigationService.Navigate(homePage);
                }
                catch (InvalidOperationException ex)
                {
                    // Ako servis nije registrovan, prikazivanje greške
                   MessageBox.Show($"Greška: {ex.Message}", "Greška u navigaciji", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Prikazivanje greške za pogrešnu šifru
                ErrorMessage.Text = "Pogrešna šifra!";
            }
        }
    }
}
