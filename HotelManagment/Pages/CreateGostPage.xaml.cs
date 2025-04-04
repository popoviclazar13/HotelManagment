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
    /// Interaction logic for CreateGostPage.xaml
    /// </summary>
    public partial class CreateGostPage : Page
    {
        private readonly IKorisnikService _korisnikService;
        private readonly Action _reloadDataAction;
        public CreateGostPage(IKorisnikService korisnikService, Action reloadAction)
        {
            InitializeComponent();
            _korisnikService = korisnikService;
            _reloadDataAction = reloadAction;
            LoadCountries();
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validacija
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                CountryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Sva polja su obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var noviKorisnik = new Korisnik
            {
                imePrezime = FullNameTextBox.Text.Trim(),
                telefon = PhoneTextBox.Text.Trim(),
                email = EmailTextBox.Text.Trim(),
                zemlja = CountryComboBox.Text.Trim() // ako koristiš string za državu
                // Ako koristiš objekat za zemlju, može biti nešto kao:
                // drzava = (CountryComboBox.SelectedItem as Country)?.Name
            };

            try
            {
                await _korisnikService.AddKorisnik(noviKorisnik);
                MessageBox.Show("Korisnik je uspešno dodat!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);

                _reloadDataAction?.Invoke(); // Osvežavanje comboBox-a u prethodnoj stranici
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom dodavanja korisnika: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
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
                /*if (!string.IsNullOrEmpty(selectedKorisnik.zemlja) && countryList.Any(c => c.Name == selectedKorisnik.zemlja))
                {
                    CountryComboBox.SelectedItem = countryList.FirstOrDefault(c => c.Name == selectedKorisnik.zemlja);
                }*/
                /*else
                {
                    CountryComboBox.SelectedIndex = 0;  // Podrazumevano selektujte prvu zemlju
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom učitavanja zemalja: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

