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
                new Country { Name = "Afganistan" },
                new Country { Name = "Albanija" },
                new Country { Name = "Alžir" },
                new Country { Name = "Andora" },
                new Country { Name = "Angola" },
                new Country { Name = "Antigva i Barbuda" },
                new Country { Name = "Argentina" },
                new Country { Name = "Armenija" },
                new Country { Name = "Australija" },
                new Country { Name = "Austrija" },
                new Country { Name = "Azerbejdžan" },
                new Country { Name = "Bahami" },
                new Country { Name = "Bahrein" },
                new Country { Name = "Bangladeš" },
                new Country { Name = "Barbados" },
                new Country { Name = "Belgija" },
                new Country { Name = "Beliz" },
                new Country { Name = "Belorusija" },
                new Country { Name = "Benin" },
                new Country { Name = "Bocvana" },
                new Country { Name = "Bolivija" },
                new Country { Name = "Bosna i Hercegovina" },
                new Country { Name = "Brazil" },
                new Country { Name = "Brunej" },
                new Country { Name = "Bugarska" },
                new Country { Name = "Burkina Faso" },
                new Country { Name = "Burundi" },
                new Country { Name = "Butan" },
                new Country { Name = "Centralnoafrička Republika" },
                new Country { Name = "Crna Gora" },
                new Country { Name = "Čad" },
                new Country { Name = "Češka" },
                new Country { Name = "Čile" },
                new Country { Name = "Danska" },
                new Country { Name = "Dominika" },
                new Country { Name = "Dominikanska Republika" },
                new Country { Name = "Džibuti" },
                new Country { Name = "Egipat" },
                new Country { Name = "Ekvador" },
                new Country { Name = "Ekvatorska Gvineja" },
                new Country { Name = "Eritreja" },
                new Country { Name = "Estonija" },
                new Country { Name = "Etiopija" },
                new Country { Name = "Fidži" },
                new Country { Name = "Filipini" },
                new Country { Name = "Finska" },
                new Country { Name = "Francuska" },
                new Country { Name = "Gabon" },
                new Country { Name = "Gambija" },
                new Country { Name = "Gana" },
                new Country { Name = "Grčka" },
                new Country { Name = "Grenada" },
                new Country { Name = "Gruzija" },
                new Country { Name = "Gvajana" },
                new Country { Name = "Gvatemala" },
                new Country { Name = "Gvineja" },
                new Country { Name = "Gvineja-Bisau" },
                new Country { Name = "Haiti" },
                new Country { Name = "Holandija" },
                new Country { Name = "Honduras" },
                new Country { Name = "Hrvatska" },
                new Country { Name = "Indija" },
                new Country { Name = "Indonezija" },
                new Country { Name = "Irak" },
                new Country { Name = "Iran" },
                new Country { Name = "Irska" },
                new Country { Name = "Island" },
                new Country { Name = "Istočni Timor" },
                new Country { Name = "Izrael" },
                new Country { Name = "Italija" },
                new Country { Name = "Jamajka" },
                new Country { Name = "Japan" },
                new Country { Name = "Jemen" },
                new Country { Name = "Jordan" },
                new Country { Name = "Južna Koreja" },
                new Country { Name = "Južni Sudan" },
                new Country { Name = "Južnoafrička Republika" },
                new Country { Name = "Kambodža" },
                new Country { Name = "Kamerun" },
                new Country { Name = "Kanada" },
                new Country { Name = "Katar" },
                new Country { Name = "Kazahstan" },
                new Country { Name = "Kenija" },
                new Country { Name = "Kina" },
                new Country { Name = "Kipar" },
                new Country { Name = "Kirgistan" },
                new Country { Name = "Kiribati" },
                new Country { Name = "Kolumbija" },
                new Country { Name = "Komori" },
                new Country { Name = "Kongo" },
                new Country { Name = "Kongo, Demokratska Republika" },
                new Country { Name = "Kostarika" },
                new Country { Name = "Kuba" },
                new Country { Name = "Kuvajt" },
                new Country { Name = "Laos" },
                new Country { Name = "Latvija" },
                new Country { Name = "Lesoto" },
                new Country { Name = "Liban" },
                new Country { Name = "Liberija" },
                new Country { Name = "Libija" },
                new Country { Name = "Lihtenštajn" },
                new Country { Name = "Litvanija" },
                new Country { Name = "Luksemburg" },
                new Country { Name = "Madagaskar" },
                new Country { Name = "Mađarska" },
                new Country { Name = "Malavi" },
                new Country { Name = "Maldivi" },
                new Country { Name = "Malezija" },
                new Country { Name = "Mali" },
                new Country { Name = "Malta" },
                new Country { Name = "Maroko" },
                new Country { Name = "Maršalska Ostrva" },
                new Country { Name = "Mauricijus" },
                new Country { Name = "Mauritanija" },
                new Country { Name = "Meksiko" },
                new Country { Name = "Mikronezija" },
                new Country { Name = "Mjanmar" },
                new Country { Name = "Moldavija" },
                new Country { Name = "Monako" },
                new Country { Name = "Mongolija" },
                new Country { Name = "Mozambik" },
                new Country { Name = "Namibija" },
                new Country { Name = "Nepal" },
                new Country { Name = "Niger" },
                new Country { Name = "Nigerija" },
                new Country { Name = "Nikaragva" },
                new Country { Name = "Norveška" },
                new Country { Name = "Novi Zeland" },
                new Country { Name = "Nemačka" },
                new Country { Name = "Obala Slonovače" },
                new Country { Name = "Oman" },
                new Country { Name = "Pakistan" },
                new Country { Name = "Palau" },
                new Country { Name = "Panama" },
                new Country { Name = "Papua Nova Gvineja" },
                new Country { Name = "Paragvaj" },
                new Country { Name = "Peru" },
                new Country { Name = "Poljska" },
                new Country { Name = "Portugalija" },
                new Country { Name = "Ruanda" },
                new Country { Name = "Rumunija" },
                new Country { Name = "Rusija" },
                new Country { Name = "Salomonska Ostrva" },
                new Country { Name = "Salvador" },
                new Country { Name = "Samoa" },
                new Country { Name = "San Marino" },
                new Country { Name = "Saudijska Arabija" },
                new Country { Name = "Sejšeli" },
                new Country { Name = "Senegal" },
                new Country { Name = "Severna Koreja" },
                new Country { Name = "Severna Makedonija" },
                new Country { Name = "Sijera Leone" },
                new Country { Name = "Singapur" },
                new Country { Name = "Sirija" },
                new Country { Name = "Sjedinjene Američke Države" },
                new Country { Name = "Sjedinjeni Arapski Emirati" },
                new Country { Name = "Slovačka" },
                new Country { Name = "Slovenija" },
                new Country { Name = "Somalija" },
                new Country { Name = "Srbija" },
                new Country { Name = "Sudan" },
                new Country { Name = "Surinam" },
                new Country { Name = "Sveta Lucija" },
                new Country { Name = "Sveti Kristofor i Nevis" },
                new Country { Name = "Sveti Toma i Princip" },
                new Country { Name = "Sveti Vincent i Grenadini" },
                new Country { Name = "Španija" },
                new Country { Name = "Šri Lanka" },
                new Country { Name = "Švedska" },
                new Country { Name = "Švajcarska" },
                new Country { Name = "Tadžikistan" },
                new Country { Name = "Tanzanija" },
                new Country { Name = "Tajland" },
                new Country { Name = "Togo" },
                new Country { Name = "Tonga" },
                new Country { Name = "Trinidad i Tobago" },
                new Country { Name = "Tunis" },
                new Country { Name = "Turkmenistan" },
                new Country { Name = "Turska" },
                new Country { Name = "Tuvalu" },
                new Country { Name = "Uganda" },
                new Country { Name = "Ukrajina" },
                new Country { Name = "Urugvaj" },
                new Country { Name = "Uzbekistan" },
                new Country { Name = "Vanuatu" },
                new Country { Name = "Venecuela" },
                new Country { Name = "Vijetnam" },
                new Country { Name = "Zambija" },
                new Country { Name = "Zelenortska Ostrva" },
                new Country { Name = "Zimbabve" }
        };

                // Postavi ItemsSource na ComboBox
                CountryComboBox.ItemsSource = countryList;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom učitavanja zemalja: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

