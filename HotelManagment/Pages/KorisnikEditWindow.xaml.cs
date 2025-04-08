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
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
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
