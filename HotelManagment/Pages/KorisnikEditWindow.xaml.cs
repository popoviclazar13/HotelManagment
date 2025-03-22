using HotelManagment.Entitys;
using HotelManagment.ServiceRepository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for KorisnikEditWindow.xaml
    /// </summary>
    public partial class KorisnikEditWindow : Page
    {
        private readonly IKorisnikService _korisnikService;
        private readonly Korisnik selectedKorisnik;
        private readonly Action _reloadDataAction;
        public KorisnikEditWindow(IKorisnikService korisnikService, Korisnik SelectedKorisnik, Action reloadDataAction)
        {
            InitializeComponent();
            _korisnikService = korisnikService;
            selectedKorisnik = SelectedKorisnik;
            _reloadDataAction = reloadDataAction;

            // Popunjavaju se polja sa podacima korisnika
            FullNameTextBox.Text = selectedKorisnik.imePrezime;
            PhoneTextBox.Text = selectedKorisnik.telefon;
            EmailTextBox.Text = selectedKorisnik.email;
            // Učitavanje zemalja sa API-ja
            LoadCountries();
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ažuriramo korisnika sa novim podacima
                selectedKorisnik.imePrezime = FullNameTextBox.Text;
                selectedKorisnik.telefon = PhoneTextBox.Text;
                selectedKorisnik.email = EmailTextBox.Text;
                selectedKorisnik.zemlja = ((Country)CountryComboBox.SelectedItem).Name;

                // Sačuvaj korisnika
                await _korisnikService.UpdateKorisnik(selectedKorisnik);

                // Pozovite akciju za ponovno učitavanje podataka u prethodnoj stranici
                _reloadDataAction();

                MessageBox.Show("Korisnik je uspešno sačuvan.", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack(); // Vraćanje na prethodnu stranicu
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom čuvanja korisnika: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Funkcija koja preuzima sve zemlje sa RestCountries API-ja bez dodatnih klasa
        public class Country
        {
            public string Name { get; set; }
        }

        private void LoadCountries()
        {
            try
            {
                // Rucno definisana lista zemalja
                var countryList = new List<Country>
        {
            new Country { Name = "Sjedinjene Američke Države" },
            new Country { Name = "Srbija" },
            new Country { Name = "Hrvatska" },
            new Country { Name = "Bosna i Hercegovina" },
            new Country { Name = "Crna Gora" },
            new Country { Name = "Severna Makedonija" },
            new Country { Name = "Slovenija" },
            new Country { Name = "Francuska" },
            new Country { Name = "Nemačka" },
            new Country { Name = "Italija" },
            new Country { Name = "Španija" },
            new Country { Name = "Velika Britanija" },
            new Country { Name = "Australija" },
            new Country { Name = "Kanada" },
            new Country { Name = "Rusija" },
            new Country { Name = "Kina" },
            new Country { Name = "Indija" },
            new Country { Name = "Japan" },
            new Country { Name = "Brazil" },
            new Country { Name = "Meksiko" },
            new Country { Name = "Argentina" },
            new Country { Name = "Južna Koreja" },
            new Country { Name = "Egipat" },
            new Country { Name = "Turska" },
            new Country { Name = "Južnoafrička Republika" },
            new Country { Name = "Saudi Arabija" },
            new Country { Name = "Indonezija" },
            new Country { Name = "Sjedinjeni Arapski Emirati" },
            new Country { Name = "Tajland" },
            new Country { Name = "Nigerija" },
            new Country { Name = "Pakistan" },
            new Country { Name = "Bangladeš" },
            new Country { Name = "Iran" },
            new Country { Name = "Iraq" },
            new Country { Name = "Vijetnam" },
            new Country { Name = "Tanzanija" },
            new Country { Name = "Kostarika" },
            new Country { Name = "Portoriko" },
            new Country { Name = "Norveška" },
            new Country { Name = "Švedska" },
            new Country { Name = "Danska" },
            new Country { Name = "Finska" },
            new Country { Name = "Poljska" },
            new Country { Name = "Rumunija" },
            new Country { Name = "Belgija" },
            new Country { Name = "Holandija" },
            new Country { Name = "Austrija" },
            new Country { Name = "Švajcarska" },
            new Country { Name = "Grčka" },
            new Country { Name = "Novi Zeland" }
        };

                // Postavi ItemsSource na ComboBox
                CountryComboBox.ItemsSource = countryList;

                // Ako je zemlja korisnika postavljena, odaberi je u ComboBox
                if (!string.IsNullOrEmpty(selectedKorisnik.zemlja) && countryList.Any(c => c.Name == selectedKorisnik.zemlja))
                {
                    CountryComboBox.SelectedItem = countryList.FirstOrDefault(c => c.Name == selectedKorisnik.zemlja);
                }
                else
                {
                    CountryComboBox.SelectedIndex = 0;  // Podrazumevano selektujte prvu zemlju
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom učitavanja zemalja: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
