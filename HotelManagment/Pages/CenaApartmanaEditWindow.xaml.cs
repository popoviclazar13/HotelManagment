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
    /// Interaction logic for CenaApartmanaEditWindow.xaml
    /// </summary>
    public partial class CenaApartmanaEditWindow : Page
    {
        private readonly ICenaApartmanaService _cenaApartmanaService;
        private dynamic _selectedPrice;
        private readonly Action _reloadDataAction;

        public CenaApartmanaEditWindow(ICenaApartmanaService cenaApartmanaService, dynamic selectedPrice, Action reloadDataAction)
        {
            InitializeComponent();
            _selectedPrice = selectedPrice;
            _cenaApartmanaService = cenaApartmanaService;
            _reloadDataAction = reloadDataAction;

            // Popunjavanje polja sa postojećim podacima
            PocetniDatumPicker.SelectedDate = selectedPrice.pocetniDatum;
            KrajnjiDatumPicker.SelectedDate = selectedPrice.krajnjiDatum;
            CenaPoNociBox.Text = selectedPrice.cenaPoNoci.ToString();
            CenaPoOsobiBox.Text = selectedPrice.cenaPoOsobi.ToString();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Ažuriranje podataka
            _selectedPrice.pocetniDatum = PocetniDatumPicker.SelectedDate ?? _selectedPrice.pocetniDatum;
            _selectedPrice.krajnjiDatum = KrajnjiDatumPicker.SelectedDate ?? _selectedPrice.krajnjiDatum;

            // Konverzija iz decimal u double
            _selectedPrice.cenaPoNoci = (double)(decimal.TryParse(CenaPoNociBox.Text, out decimal cenaPoNoci) ? cenaPoNoci : (decimal)_selectedPrice.cenaPoNoci);
            _selectedPrice.cenaPoOsobi = (double)(decimal.TryParse(CenaPoOsobiBox.Text, out decimal cenaPoOsobi) ? cenaPoOsobi : (decimal)_selectedPrice.cenaPoOsobi);

            try
            {
                await _cenaApartmanaService.UpdateCenaApartmana(_selectedPrice);
                MessageBox.Show("Cena apartmana uspešno ažurirana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                _reloadDataAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom ažuriranja cene: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Vraćanje na prethodnu stranicu
            NavigationService.GoBack();
        }
    }
}
