using HotelManagment.Entitys;
using HotelManagment.Service;
using HotelManagment.ServiceRepository;
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
    /// Interaction logic for UslugaEditWindow.xaml
    /// </summary>
    public partial class UslugaEditWindow : Page
    {
        private readonly IUslugaService _uslugaService;
        private readonly Usluga _usluga;
        private readonly Action _reloadDataAction;

        public UslugaEditWindow(IUslugaService uslugaService, Usluga usluga, Action reloadDataAction)
        {
            InitializeComponent();
            _uslugaService = uslugaService;
            _usluga = usluga;
            _reloadDataAction = reloadDataAction;

            // Učitavanje podataka u polja
            NazivUslugeTextBox.Text = _usluga.nazivUsluge;
            CenaUslugeTextBox.Text = _usluga.cenaUsluge.ToString();
        }

        // Metod za čuvanje izmena
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validacija unosa
                if (string.IsNullOrEmpty(NazivUslugeTextBox.Text) || string.IsNullOrEmpty(CenaUslugeTextBox.Text))
                {
                    MessageBox.Show("Sva polja moraju biti popunjena.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ažuriranje objekta
                _usluga.nazivUsluge = NazivUslugeTextBox.Text;
                _usluga.cenaUsluge = double.Parse(CenaUslugeTextBox.Text);

                // Pozivanje servisa za čuvanje podataka u bazi

                try
                {
                    await _uslugaService.UpdateUsluga(_usluga);
                    MessageBox.Show("Usluga je uspešno ažurirana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);

                    _reloadDataAction();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Došlo je do greške: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Zatvaranje prozora
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom čuvanja usluge: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metod za otkazivanje
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
