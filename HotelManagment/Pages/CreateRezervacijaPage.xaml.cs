using HotelManagment.Entitys;
using HotelManagment.ServiceRepository;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IRezervacijaUslugaService _rezervacijaUslugaService;
        private readonly IUslugaService _uslugaService;
        private readonly Action _reloadDataAction;

        public CreateRezervacijaPage(IRezervacijaService rezervacijaService, IKorisnikService korisnikService, IAgencijaService agencijaService, IApartmanService apartmanService, IPopustService popustService, ICenaApartmanaService cenaApartmanaService, IApartmanPopustService apartmanPopustService, IRezervacijaUslugaService rezervacijaUslugaService, IUslugaService uslugaService, Action reloadDataAction)
        {
            InitializeComponent();
            _rezervacijaService = rezervacijaService;
            _korisnikService = korisnikService;
            _agencijaService = agencijaService;
            _apartmanService = apartmanService;
            _popustService = popustService;
            _cenaApartmanaService = cenaApartmanaService;
            _apartmanPopustService = apartmanPopustService;
            _rezervacijaUslugaService = rezervacijaUslugaService;
            _uslugaService = uslugaService;
            _reloadDataAction = reloadDataAction;
            LoadComboBoxData();
        }
        private async void LoadComboBoxData()
        {
            try
            {
                var korisnici = await _korisnikService.GetAllKorisnik();
                UserComboBox.ItemsSource = korisnici.OrderByDescending(k => k.korisnikId).ToList();

                var agencije = await _agencijaService.GetAllAgencija();
                AgencyComboBox.ItemsSource = agencije;

                var apartmani = await _apartmanService.GetAllApartman();
                ApartmentComboBox.ItemsSource = apartmani;

                var usluge = await _uslugaService.GetAllUsluga();
                ServicesComboBox.ItemsSource = usluge;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju podataka: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
            double provizija;
            if (!double.TryParse(CommissionAmountTextBox.Text, out provizija))
            {
                // Ako nije validan broj, možeš da prikažeš poruku ili podesiš vrednost na 0
                MessageBox.Show("Molimo unesite validan iznos provizije!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                provizija = 0; // Postavi vrednost na 0 ako unos nije validan
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
                iznosProvizije = provizija, // Ako se primenjuje provizija
                placeno = PaidCheckBox.IsChecked ?? false,
                komentar = CommentTextBox.Text,
            };

            try
            {
                await _rezervacijaService.AddRezervacija(newRezervacija);
                // Prolazimo kroz sve selektovane usluge i dodajemo torke u RezervacijaUsluga
                var selectedServices = ServicesComboBox.SelectedItems.Cast<Usluga>().ToList();

                foreach (var service in selectedServices)
                {
                    var rezervacijaUsluga = new RezervacijaUsluga
                    {
                        rezervacijaId = newRezervacija.rezervacijaId,
                        uslugaId = service.uslugaId,
                        kolicina = 0,
                        datum = DateTime.Now
                    };

                    // Kreiranje torke u bazi za svaku uslugu
                    await _rezervacijaUslugaService.AddRezervacijaUsluga(rezervacijaUsluga);
                }

                MessageBox.Show("Rezervacija je uspešno kreirana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                _reloadDataAction();

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
        private void AddGuestButton_Click(object sender, RoutedEventArgs e)
        {
            Action reloadDataAction = LoadComboBoxData;
            NavigationService.Navigate(new CreateGostPage(_korisnikService, reloadDataAction)); // ili naziv tvoje stranice
        }
        private void DiscountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Pozivanje postojeće metode koja računa cenu sa popustom
            PostaviCenuApartmana();
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
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the previous page
            ImportFromExcel_Click();
        }
        private async void ImportFromExcel_Click()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel fajl (*.xlsx)|*.xlsx"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var package = new ExcelPackage(new FileInfo(openFileDialog.FileName)))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // preskače zaglavlje
                        {
                            string gost = worksheet.Cells[row, 4].Text;
                            DateTime dolazak = DateTime.Parse(worksheet.Cells[row, 5].Text);
                            DateTime odlazak = DateTime.Parse(worksheet.Cells[row, 6].Text);
                            double total = double.Parse(worksheet.Cells[row, 8].Text);
                            double provizija = double.TryParse(worksheet.Cells[row, 9].Text, out var p) ? p : 0;
                            string zahtevi = worksheet.Cells[row, 10].Text;
                            string zemlja = worksheet.Cells[row, 11].Text;
                            if (!int.TryParse(worksheet.Cells[row, 12].Text, out int sobaId))
                            {
                                MessageBox.Show($"Nevalidan ID sobe u redu {row}.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                                continue; // preskoči ovaj red ako nije validan ID
                            }
                            string telefon = worksheet.Cells[row, 13].Text;

                            // Pronađi korisnika po imenu, telefonu i emailu
                            var korisnik = (await _korisnikService.GetAllKorisnik())
                                .FirstOrDefault(k => k.imePrezime == gost &&
                                                     k.telefon == telefon &&
                                                     k.email == "Nije ostavljen");

                            if (korisnik == null)
                            {
                                // Ako ne postoji, dodaj ga
                                var noviKorisnik = new Korisnik
                                {
                                    imePrezime = gost,
                                    telefon = telefon,
                                    email = "Nije ostavljen",
                                    zemlja = zemlja
                                };

                                await _korisnikService.AddKorisnik(noviKorisnik);

                                // Ponovno preuzimanje korisnika
                                korisnik = (await _korisnikService.GetAllKorisnik())
                                    .FirstOrDefault(k => k.imePrezime == gost &&
                                                         k.telefon == telefon &&
                                                         k.email == "Nije ostavljen");

                                if (korisnik == null)
                                {
                                    MessageBox.Show($"Greška prilikom dodavanja korisnika '{gost}' u redu {row}.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                                    continue;
                                }
                            }

                            // 2. Pronađi apartman i agenciju (po imenu)
                            var apartman = await _apartmanService.GetByIdApartman(sobaId);
                            var sveAgencije = await _agencijaService.GetAllAgencija();

                            var agencijaBezAgencije = sveAgencije
                                .FirstOrDefault(a => a.nazivAgencije.Trim().Equals("Bez Agencije", StringComparison.OrdinalIgnoreCase));

                            if (agencijaBezAgencije == null)
                            {
                                MessageBox.Show("Nije pronađena agencija sa nazivom 'Bez Agencije'.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            // 3. Kreiraj rezervaciju
                            var rezervacija = new Rezervacija
                            {
                                pocetniDatum = dolazak,
                                krajnjiDatum = odlazak,
                                ukupnaCena = total,
                                cenaKonacna = total,
                                iznosProvizije = provizija,
                                placeno = false,
                                komentar = zahtevi,
                                nacinPlacanja = "Keš",

                                apartmanId = apartman?.apartmanId ?? 0,
                                korisnikId = korisnik.korisnikId,
                                agencijaId = agencijaBezAgencije.agencijaId
                            };

                            await _rezervacijaService.AddRezervacija(rezervacija);
                        }

                        MessageBox.Show("Uvoz rezervacija iz Excel fajla uspešno završen!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                        _reloadDataAction();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Došlo je do greške: {ex.Message}\n" +
                        $"Detalji: {ex.InnerException?.Message}",
                        "Greška",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
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
                    // Dobijanje popusta iz ComboBoxa
                    string selectedDiscountText = (DiscountComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    double popustProcenat = 0;

                    // Ako je odabrana vrednost popusta u ComboBoxu, pretvori je u broj
                    if (!string.IsNullOrEmpty(selectedDiscountText) && double.TryParse(selectedDiscountText.TrimEnd('%'), out popustProcenat))
                    {
                        // Ako je validan popust, pretvori ga u decimalnu vrednost
                        popustProcenat /= 100;
                        MessageBox.Show($"Popust primenjen: {popustProcenat * 100}%", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nema popusta za izabrani period.", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    // Računanje cene sa popustom
                    double iznosPopusta = ukupnaCena * popustProcenat;
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
