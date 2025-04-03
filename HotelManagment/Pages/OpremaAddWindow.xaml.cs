using HotelManagment.Entitys;
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
    /// Interaction logic for OpremaAddWindow.xaml
    /// </summary>
    public partial class OpremaAddWindow : Page
    {
        private readonly IOpremaService _opremaService;
        private readonly Action _reloadDataAction;
        public OpremaAddWindow(IOpremaService opremaService, Action reloadDataAction)
        {
            InitializeComponent();
            _opremaService = opremaService;
            _reloadDataAction = reloadDataAction;

        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string nazivOpreme = NazivOpremeTextBox.Text.Trim();

            // Provera da li je unos prazan
            if (string.IsNullOrWhiteSpace(nazivOpreme))
            {
                MessageBox.Show("Naziv opreme ne može biti prazan.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Kreiranje novog objekta opreme
                var novaOprema = new Oprema
                {
                    nazivOprema = nazivOpreme
                };

                // Dodavanje nove opreme u bazu (provera duplikata već obavljena u AddOprema metodi)
                await _opremaService.AddOprema(novaOprema);

                MessageBox.Show("Oprema uspešno dodata.", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);

                // Osvježavanje podataka na prethodnoj stranici
                _reloadDataAction?.Invoke();

                // Vraćanje na prethodnu stranicu
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                // Ako dođe do greške (ako oprema već postoji)
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metod za otkazivanje
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
