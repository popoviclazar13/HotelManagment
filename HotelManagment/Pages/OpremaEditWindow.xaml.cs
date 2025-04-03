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
    /// Interaction logic for OpremaEditWindow.xaml
    /// </summary>
    public partial class OpremaEditWindow : Page
    {
        private readonly IOpremaService _opremaService;
        private readonly Oprema _oprema;
        private readonly Action _reloadDataAction;

        public OpremaEditWindow(IOpremaService opremaService, Oprema oprema, Action reloadDataAction)
        {
            InitializeComponent();
            _opremaService = opremaService;
            _oprema = oprema;
            _reloadDataAction = reloadDataAction;

            // Učitavanje podataka u polja
            NazivOpremeTextBox.Text = _oprema.nazivOprema;
        }
        // Metod za čuvanje izmena
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validacija unosa
                if (string.IsNullOrEmpty(NazivOpremeTextBox.Text))
                {
                    MessageBox.Show("Sva polja moraju biti popunjena.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ažuriranje objekta
                _oprema.nazivOprema = NazivOpremeTextBox.Text;

                // Pozivanje servisa za čuvanje podataka u bazi
                try
                {
                    await _opremaService.UpdateOprema(_oprema);
                    MessageBox.Show("Oprema je uspešno ažurirana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);

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
                MessageBox.Show($"Došlo je do greške prilikom čuvanja opreme: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metod za otkazivanje
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
