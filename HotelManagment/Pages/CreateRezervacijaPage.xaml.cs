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
    /// Interaction logic for CreateRezervacijaPage.xaml
    /// </summary>
    public partial class CreateRezervacijaPage : Page
    {
        private readonly IRezervacijaService _rezervacijaService;
        private readonly IKorisnikService _korisnikService;
        private readonly IAgencijaService _agencijaService;
        private readonly IApartmanService _apartmanService;
        private readonly IPopustService _popustService;
        private readonly ICenaApartmanaService _cenaApartmanaService;
        private readonly IApartmanPopustService _apartmanPopustService;

        public CreateRezervacijaPage(IRezervacijaService rezervacijaService, IKorisnikService korisnikService, IAgencijaService agencijaService, IApartmanService apartmanService, IPopustService popustService, ICenaApartmanaService cenaApartmanaService, IApartmanPopustService apartmanPopustService)
        {
            InitializeComponent();
            _rezervacijaService = rezervacijaService;
            _korisnikService = korisnikService;
            _agencijaService = agencijaService;
            _apartmanService = apartmanService;
            _popustService = popustService;
            _cenaApartmanaService = cenaApartmanaService;
            _apartmanPopustService = apartmanPopustService;
            LoadComboBoxData();
        }
        private async void LoadComboBoxData()
        {
            try
            {
                var korisnici = await _korisnikService.GetAllKorisnik();
                UserComboBox.ItemsSource = korisnici;

                var agencije = await _agencijaService.GetAllAgencija();
                AgencyComboBox.ItemsSource = agencije;

                var apartmani = await _apartmanService.GetAllApartman();
                ApartmentComboBox.ItemsSource = apartmani;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju podataka: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Molimo unesite datume!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ApartmentComboBox.SelectedItem == null)
            {
                MessageBox.Show("Molimo izaberite apartman!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UserComboBox.SelectedItem == null)
            {
                MessageBox.Show("Molimo izaberite korisnika!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedApartment = ApartmentComboBox.SelectedItem as Apartman;
            var selectedUser = UserComboBox.SelectedItem as Korisnik;
            var selectedAgency = AgencyComboBox.SelectedItem as Agencija;
            var selectedPayment = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            var newRezervacija = new Rezervacija
            {
                pocetniDatum = StartDatePicker.SelectedDate.Value,
                krajnjiDatum = EndDatePicker.SelectedDate.Value,
                brojGostiju = (int)(GuestCountComboBox.SelectedItem ?? 1),
                nacinPlacanja = selectedPayment ?? "Keš", // Default na keš ako nije izabrano

                // Povezivanje sa apartmanom i korisnikom
                apartmanId = selectedApartment.apartmanId,
                korisnikId = selectedUser.korisnikId,
                agencijaId = selectedAgency?.agencijaId, // Može biti null

                // Ostale vrednosti (treba ih dodatno obračunati)
                ukupnaCena = 0,  // Treba izračunati
                cenaKonacna = 0, // Treba izračunati
                iznosProvizije = 0, // Ako se primenjuje provizija
                placeno = false,
                komentar = ""
            };

            try
            {
                await _rezervacijaService.AddRezervacija(newRezervacija);
                MessageBox.Show("Rezervacija je uspešno kreirana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške: {ex.Message}\nDetalji: {ex.InnerException?.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }*/
        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Molimo unesite datume!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ApartmentComboBox.SelectedItem == null)
            {
                MessageBox.Show("Molimo izaberite apartman!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UserComboBox.SelectedItem == null)
            {
                MessageBox.Show("Molimo izaberite korisnika!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedApartment = ApartmentComboBox.SelectedItem as Apartman;
            var selectedUser = UserComboBox.SelectedItem as Korisnik;
            var selectedAgency = AgencyComboBox.SelectedItem as Agencija;
            var selectedPayment = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Uzimanje vrednosti iz PriceTextBox-a
            double konacnaCena = 0;
            if (!double.TryParse(PriceTextBox.Text, out konacnaCena))
            {
                MessageBox.Show("Molimo unesite validnu cenu!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newRezervacija = new Rezervacija
            {
                pocetniDatum = StartDatePicker.SelectedDate.Value,
                krajnjiDatum = EndDatePicker.SelectedDate.Value,
                brojGostiju = (int)(GuestCountComboBox.SelectedItem ?? 1),
                nacinPlacanja = selectedPayment ?? "Keš", // Default na keš ako nije izabrano

                // Povezivanje sa apartmanom i korisnikom
                apartmanId = selectedApartment.apartmanId,
                korisnikId = selectedUser.korisnikId,
                agencijaId = selectedAgency?.agencijaId, // Može biti null

                // Ostale vrednosti (treba ih dodatno obračunati)
                ukupnaCena = konacnaCena,  // Postavljanje konačne cene
                cenaKonacna = konacnaCena, // Postavljanje konačne cene
                iznosProvizije = 0, // Ako se primenjuje provizija
                placeno = PaidCheckBox.IsChecked ?? false,
                komentar = ""
            };

            try
            {
                await _rezervacijaService.AddRezervacija(newRezervacija);
                MessageBox.Show("Rezervacija je uspešno kreirana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                // Pozivanje metode za osvežavanje rezervacija ako postoji
                if (NavigationService.Content is AllRezervacijePage rezervacijePage)
                {
                    rezervacijePage.LoadRezervacije();
                }

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške: {ex.Message}\nDetalji: {ex.InnerException?.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the previous page
            NavigationService.GoBack();
        }

        private void ApartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadGuestCountComboBox();
            PostaviCenuApartmana(); // Pozivanje metode da se ažurira cena
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            PostaviCenuApartmana(); // Pozivanje metode da se ažurira cena prilikom promene datuma
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            PostaviCenuApartmana(); // Pozivanje metode da se ažurira cena prilikom promene datuma
        }

        private void LoadGuestCountComboBox()
        {
            if (ApartmentComboBox.SelectedItem is Apartman selectedApartman)
            {
                // Računanje maksimalnog kapaciteta
                int maxKapacitet = selectedApartman.kapacitetDeca + selectedApartman.kapacitetOdrasli;
                GuestCountComboBox.ItemsSource = Enumerable.Range(1, maxKapacitet); // Brojevi od 1 do maxKapacitet
                GuestCountComboBox.SelectedItem = 1; // Postavi početnu vrednost na 1
            }
        }
        private async void PostaviCenuApartmana()
        {
            if (ApartmentComboBox.SelectedItem is Apartman selectedApartman && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                DateTime datumOd = StartDatePicker.SelectedDate.Value;
                DateTime datumDo = EndDatePicker.SelectedDate.Value;

                if (datumDo <= datumOd)
                {
                    MessageBox.Show("Datum odlaska mora biti posle datuma dolaska.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int brojNocenja = (datumDo - datumOd).Days;

                // Prikazujemo broja noćenja
                MessageBox.Show($"Broj noćenja: {brojNocenja} ", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);

                double ukupnaCena = 0;

                try
                {
                    // Dohvatanje cena za izabrani apartman i datum
                    var cenaApartmanaList = await _cenaApartmanaService.GetAllCenaApartmana();

                    // Prikazivanje svih dostupnih cena za period
                    MessageBox.Show($"Broj dostupnih cena: {cenaApartmanaList.Count}", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Filtriranje relevantnih cena
                    var relevantPrices = cenaApartmanaList
                        .Where(cena => cena.apartmanId == selectedApartman.apartmanId &&
                            (
                                (cena.pocetniDatum.Date <= datumDo.Date && cena.krajnjiDatum.Date >= datumOd.Date) || // Cena je unutar datuma
                                (cena.pocetniDatum.Date <= datumDo.Date && cena.krajnjiDatum.Date >= datumDo.Date) || // Početak cene je pre nego što rezervacija završava
                                (cena.pocetniDatum.Date <= datumOd.Date && cena.krajnjiDatum.Date >= datumOd.Date) // Kraj cene je posle nego što rezervacija počinje
                            ))
                        .ToList();

                    // Prikazivanje rezultata
                    MessageBox.Show($"Broj relevantnih cena: {relevantPrices.Count}", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Provera da li je pronađena relevantna cena
                    if (relevantPrices.Count == 0)
                    {
                        MessageBox.Show("Nema dostupnih cena za izabrani period.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    foreach (var cena in relevantPrices)
                    {
                        // Računanje cene na osnovu broja noćenja
                        int brojNocenjaZaTajInterval = (datumDo - datumOd).Days;
                        double cenaZaTajPeriod = (double)cena.cenaPoNoci * brojNocenjaZaTajInterval;

                        // Prikazivanje cene za taj period
                        MessageBox.Show($"Cena za period: {cenaZaTajPeriod:F2}", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);

                        ukupnaCena += cenaZaTajPeriod;
                    }

                    // Dohvatanje primenljivih popusta
                    var allPopusti = await _popustService.GetAllPopust();
                    var relevantPopust = allPopusti
                        .Where(p => p.pocetniDatum <= datumDo && p.krajnjiDatum >= datumOd)  // Popust koji se poklapa sa periodom rezervacije
                        .FirstOrDefault();

                    // Prikazivanje popusta
                    if (relevantPopust != null)
                    {
                        MessageBox.Show($"Popust primenjen: {relevantPopust.vrednost}%", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nema popusta za izabrani period.", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    double popustProcenat = relevantPopust?.vrednost ?? 0;  // Ako nije pronađen popust, vrednost će biti 0
                    double iznosPopusta = ukupnaCena * (popustProcenat / 100);
                    double konacnaCena = ukupnaCena - iznosPopusta;

                    // Ažuriranje polja sa finalnom cenom
                    PriceTextBox.Text = konacnaCena.ToString("F2");

                    // Prikazivanje konačne cene
                    MessageBox.Show($"Konačna cena: {konacnaCena:F2}", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri dohvatanju cene apartmana: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
