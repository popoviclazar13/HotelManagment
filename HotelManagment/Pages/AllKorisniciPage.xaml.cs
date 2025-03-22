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
    /// Interaction logic for AllKorisniciPage.xaml
    /// </summary>
    public partial class AllKorisniciPage : Page
    {
        private readonly IKorisnikService _korisnikService;
        public AllKorisniciPage(IKorisnikService korisnikService)
        {
            InitializeComponent();
            _korisnikService = korisnikService;
            LoadKorisnici();
        }
        private async void LoadKorisnici()
        {
            try
            {
                var korisnici = await _korisnikService.GetAllKorisnik();  
                KorisniciDataGrid.ItemsSource = korisnici;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom učitavanja korisnika: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void KorisniciDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KorisniciDataGrid.SelectedItem is Korisnik selectedKorisnik && selectedKorisnik.korisnikId > 0)
            {
                // Ako je potrebno da napravite novi prozor za editovanje korisnika
                // Ovdje možete pozvati novu stranicu ili prozor za editovanje korisnika
                Action reloadDataAction = LoadKorisnici; // Reload funkcionalnost nakon promjena
                NavigationService.Navigate(new KorisnikEditWindow(_korisnikService, selectedKorisnik, reloadDataAction));
                KorisniciDataGrid.SelectedItem = null; // Resetovanje selekcije
            }
        }
    }
}
