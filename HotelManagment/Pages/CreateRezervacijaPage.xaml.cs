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
                MessageBox.Show("Molimo unesite validan iznos provizije!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                provizija = 0; // Postavi vrednost na 0 ako unos nije validan
            }

            // Provera da li je apartman slobodan za odabrani period
            if (!await IsApartmentAvailableAsync(selectedApartment, StartDatePicker.SelectedDate.Value, EndDatePicker.SelectedDate.Value))
            {
                // Pronađi sve slobodne apartmane za ovaj period
                var freeApartments = await GetFreeApartments(StartDatePicker.SelectedDate.Value, EndDatePicker.SelectedDate.Value);

                // Prikazivanje slobodnih apartmana
                string slobodniApartmani = string.Join("\n", freeApartments.Select(a => a.nazivApartmana));
                MessageBox.Show($"Ovaj apartman nije slobodan za odabrani period. Slobodni apartmani: {slobodniApartmani}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private async Task<List<Apartman>> GetFreeApartments(DateTime startDate, DateTime endDate)
        {
            var allApartments = await _apartmanService.GetAllApartman(); // Pretpostavljam da imaš ovu metodu koja vraća sve apartmane
            var allReservations = await _rezervacijaService.GetAllRezervacija(); // Sve rezervacije iz baze

            var freeApartments = new List<Apartman>();

            foreach (var apartment in allApartments)
            {
                bool isFree = true;

                var reservationsForApartment = allReservations.Where(r => r.apartmanId == apartment.apartmanId);

                foreach (var reservation in reservationsForApartment)
                {
                    // Ako se period preklapa, apartman nije slobodan
                    if (startDate <= reservation.krajnjiDatum && endDate >= reservation.pocetniDatum)
                    {
                        isFree = false;
                        break;
                    }
                }

                if (isFree)
                {
                    freeApartments.Add(apartment);
                }
            }

            return freeApartments;
        }

        private async Task<bool> IsApartmentAvailableAsync(Apartman apartment, DateTime startDate, DateTime endDate)
        {
            // Pretpostavljamo da postoji metoda koja vraća sve rezervacije za određeni apartman
            var allReservations = await _rezervacijaService.GetRezervacijeByApartmanId(apartment.apartmanId);

            foreach (var reservation in allReservations)
            {
                // Ako postoji preklapanje u terminima
                if ((startDate <= reservation.krajnjiDatum && endDate >= reservation.pocetniDatum))
                {
                    return false; // Apartman nije slobodan za ovaj period
                }
            }
            return true; // Apartman je slobodan za ovaj period
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the previous page
            NavigationService.GoBack();
        }

        private void ApartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadGuestCountComboBox();
            //PostaviCenuApartmana(); // Pozivanje metode da se ažurira cena
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded && StartDatePicker.SelectedDate.HasValue)
            {
                // Postavi minimalni datum za krajnji datum na vrednost početnog datuma
                EndDatePicker.DisplayDateStart = StartDatePicker.SelectedDate.Value;

                // Ako korisnik odabere loš datum za krajnji datum, poništi ga
                if (EndDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.Value < StartDatePicker.SelectedDate.Value)
                {
                    EndDatePicker.SelectedDate = null;
                }

                // Blokiraj sve datume pre početnog datuma na krajnjem DatePickeru
                EndDatePicker.BlackoutDates.Clear();
                EndDatePicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, StartDatePicker.SelectedDate.Value.AddDays(-1)));

                // Poziv metode za ažuriranje cene
                PostaviCenuApartmana();
            }
            //PostaviCenuApartmana(); // Pozivanje metode da se ažurira cena prilikom promene datuma
        }
        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                // Ako krajnji datum nije manji od početnog, ažuriraj cenu
                if (EndDatePicker.SelectedDate.Value >= StartDatePicker.SelectedDate.Value)
                {
                    // Poziv metode za ažuriranje cene
                    PostaviCenuApartmana();
                }
                else
                {
                    // Ako je krajnji datum manji, poništi ga
                    EndDatePicker.SelectedDate = null;
                }
            }
            //PostaviCenuApartmana(); // Pozivanje metode da se ažurira cena prilikom promene datuma
        }

        private void GuestCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                bool sveRezervacijeUspele = true; // Postavi na true pre početka

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
                            string nazivApartmana = worksheet.Cells[row, 12].Text.Trim();
                            string telefon = worksheet.Cells[row, 13].Text;
                            int brojGostiju = int.Parse(worksheet.Cells[row, 14].Text);

                            // 1. Pronađi ili dodaj korisnika
                            var korisnik = (await _korisnikService.GetAllKorisnik())
                                .FirstOrDefault(k => k.imePrezime == gost &&
                                                     k.telefon == telefon &&
                                                     k.email == "Nije ostavljen");

                            if (korisnik == null)
                            {
                                var noviKorisnik = new Korisnik
                                {
                                    imePrezime = gost,
                                    telefon = telefon,
                                    email = "Nije ostavljen",
                                    zemlja = zemlja
                                };

                                await _korisnikService.AddKorisnik(noviKorisnik);
                                korisnik = (await _korisnikService.GetAllKorisnik())
                                    .FirstOrDefault(k => k.imePrezime == gost &&
                                                         k.telefon == telefon &&
                                                         k.email == "Nije ostavljen");

                                if (korisnik == null)
                                {
                                    MessageBox.Show($"Greška prilikom dodavanja korisnika '{gost}' u redu {row}.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                                    sveRezervacijeUspele = false;
                                    continue;
                                }
                            }

                            // 2. Pronađi apartman
                            var sviApartmani = await _apartmanService.GetAllApartman();
                            var apartman = sviApartmani
                                .FirstOrDefault(a => a.nazivApartmana.Trim().StartsWith(nazivApartmana, StringComparison.OrdinalIgnoreCase));

                            if (apartman == null)
                            {
                                MessageBox.Show($"Nije pronađen apartman sa nazivom '{nazivApartmana}' u redu {row}.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                                sveRezervacijeUspele = false;
                                continue;
                            }

                            // 3. Pronađi agenciju "Bez Agencije"
                            var sveAgencije = await _agencijaService.GetAllAgencija();
                            var agencijaBezAgencije = sveAgencije
                                .FirstOrDefault(a => a.nazivAgencije.Trim().Equals("Bez Agencije", StringComparison.OrdinalIgnoreCase));

                            if (agencijaBezAgencije == null)
                            {
                                MessageBox.Show("Nije pronađena agencija sa nazivom 'Bez Agencije'.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                                sveRezervacijeUspele = false;
                                return;
                            }

                            // 4. Provera da li rezervacija već postoji
                            var postojeceRezervacije = await _rezervacijaService.GetRezervacijeByApartmanId(apartman.apartmanId);
                            bool rezervacijaVecPostoji = postojeceRezervacije.Any(r =>
                                r.korisnikId == korisnik.korisnikId &&
                                r.pocetniDatum.Date == dolazak.Date &&
                                r.krajnjiDatum.Date == odlazak.Date);

                            if (rezervacijaVecPostoji)
                            {
                                // Preskoči ako već postoji ista rezervacija
                                continue;
                            }

                            // 5. Provera da li je apartman zauzet u tom periodu
                            bool jeZauzet = postojeceRezervacije.Any(r => r.pocetniDatum < odlazak && r.krajnjiDatum > dolazak);

                            if (jeZauzet)
                            {
                                // Nađi slobodne apartmane - bez async u Where
                                var slobodniApartmani = new List<Apartman>();

                                foreach (var a in sviApartmani)
                                {
                                    var rezervacijeZaApartman = await _rezervacijaService.GetRezervacijeByApartmanId(a.apartmanId);
                                    bool apartmanZauzet = rezervacijeZaApartman.Any(r => r.pocetniDatum < odlazak && r.krajnjiDatum > dolazak);

                                    if (!apartmanZauzet)
                                    {
                                        slobodniApartmani.Add(a);
                                    }
                                }

                                if (slobodniApartmani.Any())
                                {
                                    var slobodniNazivi = string.Join(", ", slobodniApartmani.Select(a => a.nazivApartmana));
                                    MessageBox.Show($"Apartman '{nazivApartmana}' je zauzet u periodu {dolazak.ToShortDateString()} - {odlazak.ToShortDateString()}. " +
                                                    $"Slobodni apartmani: {slobodniNazivi}.", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show($"Apartman '{nazivApartmana}' je zauzet u periodu {dolazak.ToShortDateString()} - {odlazak.ToShortDateString()}. " +
                                                    "Nema slobodnih apartmana za taj period.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                                sveRezervacijeUspele = false;
                                continue;
                            }

                            // 6. Kreiraj rezervaciju
                            var rezervacija = new Rezervacija
                            {
                                pocetniDatum = dolazak,
                                krajnjiDatum = odlazak,
                                ukupnaCena = total,
                                cenaKonacna = total,
                                iznosProvizije = provizija,
                                placeno = false,
                                komentar = zahtevi,
                                brojGostiju = brojGostiju,
                                nacinPlacanja = "Keš",
                                apartmanId = apartman.apartmanId,
                                korisnikId = korisnik.korisnikId,
                                agencijaId = agencijaBezAgencije.agencijaId
                            };

                            try
                            {
                                await _rezervacijaService.AddRezervacija(rezervacija);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Greška prilikom dodavanja rezervacije za korisnika '{gost}' u redu {row}. Detalji: {ex.Message}",
                                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                                sveRezervacijeUspele = false;
                                continue;
                            }
                        }

                        if (sveRezervacijeUspele)
                        {
                            MessageBox.Show("Uvoz rezervacija iz Excel fajla uspešno završen!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Neke rezervacije nisu unesene. Proverite Excel fajl.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

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
        /*private async void PostaviCenuApartmana()
        {
            //Za GRANICNI DATUM 01.07-20.07 20.07-31.07 za 20.07 uzece vrednost od 01.07-20.07 intervala!!!
            if (ApartmentComboBox.SelectedItem is Apartman selectedApartman && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                DateTime datumOd = StartDatePicker.SelectedDate.Value;
                DateTime datumDo = EndDatePicker.SelectedDate.Value;

                if (datumDo <= datumOd)
                {
                    MessageBox.Show("Datum odlaska mora biti posle datuma dolaska.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    var cenaApartmanaList = await _cenaApartmanaService.GetAllCenaApartmana();

                    var relevantPrices = cenaApartmanaList
                        .Where(cena => cena.apartmanId == selectedApartman.apartmanId)
                        .ToList();

                    if (relevantPrices.Count == 0)
                    {
                        MessageBox.Show("Nema dostupnih cena za izabrani apartman.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    double ukupnaCena = 0;
                    DateTime trenutniDatum = datumOd;

                    while (trenutniDatum < datumDo)
                    {
                        var cenaZaDan = relevantPrices.FirstOrDefault(cena =>
                            cena.pocetniDatum.Date <= trenutniDatum.Date &&
                            cena.krajnjiDatum.Date >= trenutniDatum.Date);

                        if (cenaZaDan == null)
                        {
                            MessageBox.Show($"Nema definisane cene za datum {trenutniDatum:dd.MM.yyyy}.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        double cenaPoNoci = (double)cenaZaDan.cenaPoNoci;

                        if (!string.Equals(selectedApartman.tipApartmana?.nazivTipaApartmana, "Apartman", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!int.TryParse(GuestCountComboBox.Text, out int brojGostiju) || brojGostiju <= 0)
                            {
                                MessageBox.Show("Unesite validan broj gostiju.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            cenaPoNoci *= brojGostiju;
                        }

                        ukupnaCena += cenaPoNoci;

                        // DEBUG poruka
                        Console.WriteLine($"Datum: {trenutniDatum:dd.MM.yyyy}, Cena: {cenaPoNoci}");

                        trenutniDatum = trenutniDatum.AddDays(1);
                    }

                    // Primeni popust
                    string selectedDiscountText = (DiscountComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    double popustProcenat = 0;

                    if (!string.IsNullOrEmpty(selectedDiscountText) && double.TryParse(selectedDiscountText.TrimEnd('%'), out popustProcenat))
                    {
                        popustProcenat /= 100;
                    }

                    double iznosPopusta = ukupnaCena * popustProcenat;
                    double konacnaCena = ukupnaCena - iznosPopusta;

                    PriceTextBox.Text = konacnaCena.ToString("F2");

                    MessageBox.Show($"Konačna cena: {konacnaCena:F2}", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri dohvatanju cene apartmana: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }*/
        private async void PostaviCenuApartmana()
        {
            //Za GRANICNI DATUM 01.07-20.07 20.07-31.07 za 20.07 uzece vrednost od 01.07-20.07 intervala!!!
            if (ApartmentComboBox.SelectedItem is Apartman selectedApartman && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                DateTime datumOd = StartDatePicker.SelectedDate.Value;
                DateTime datumDo = EndDatePicker.SelectedDate.Value;

                if (datumDo <= datumOd)
                {
                    MessageBox.Show("Datum odlaska mora biti posle datuma dolaska.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    var cenaApartmanaList = await _cenaApartmanaService.GetAllCenaApartmana();
                    var relevantPrices = cenaApartmanaList
                        .Where(cena => cena.apartmanId == selectedApartman.apartmanId)
                        .ToList();

                    if (relevantPrices.Count == 0)
                    {
                        MessageBox.Show("Nema dostupnih cena za izabrani apartman.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    double ukupnaCena = 0;
                    DateTime trenutniDatum = datumOd;

                    while (trenutniDatum < datumDo)
                    {
                        var cenaZaDan = relevantPrices.FirstOrDefault(cena =>
                            cena.pocetniDatum.Date <= trenutniDatum.Date &&
                            cena.krajnjiDatum.Date >= trenutniDatum.Date);

                        if (cenaZaDan == null)
                        {
                            MessageBox.Show($"Nema definisane cene za datum {trenutniDatum:dd.MM.yyyy}.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        double cenaPoNoci = (double)cenaZaDan.cenaPoNoci;

                        if (!string.Equals(selectedApartman.tipApartmana?.nazivTipaApartmana, "Apartman", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!int.TryParse(GuestCountComboBox.Text, out int brojGostiju) || brojGostiju <= 0)
                            {
                                MessageBox.Show("Unesite validan broj gostiju.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            cenaPoNoci *= brojGostiju;
                        }

                        ukupnaCena += cenaPoNoci;
                        trenutniDatum = trenutniDatum.AddDays(1);
                    }

                    // Primeni popust
                    string selectedDiscountText = (DiscountComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    double popustProcenat = 0;

                    if (!string.IsNullOrEmpty(selectedDiscountText) && double.TryParse(selectedDiscountText.TrimEnd('%'), out popustProcenat))
                    {
                        popustProcenat /= 100;
                    }

                    double iznosPopusta = ukupnaCena * popustProcenat;
                    double konacnaCena = ukupnaCena - iznosPopusta;

                    // *** DODATO: Računanje cene izabranih usluga ***
                    double ukupnaCenaUsluga = 0;

                    if (ServicesComboBox.SelectedItems != null && ServicesComboBox.SelectedItems.Count > 0)
                    {
                        var sveUsluge = await _uslugaService.GetAllUsluga();

                        foreach (var selectedItem in ServicesComboBox.SelectedItems)
                        {
                            if (selectedItem is Usluga usluga)
                            {
                                ukupnaCenaUsluga += (double)usluga.cenaUsluge;
                            }
                            else if (selectedItem is string nazivUsluge) // Ako ListBox veže samo naziv
                            {
                                var usluga1 = sveUsluge.FirstOrDefault(u => u.nazivUsluge == nazivUsluge);
                                if (usluga1 != null)
                                {
                                    ukupnaCenaUsluga += (double)usluga1.cenaUsluge;
                                }
                            }
                        }
                    }

                    double ukupnaKonacnaCena = konacnaCena + ukupnaCenaUsluga;

                    // Postavi cenu
                    PriceTextBox.Text = ukupnaKonacnaCena.ToString("F2");

                    MessageBox.Show($"Konačna cena : {ukupnaKonacnaCena:F2} EUR", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri dohvatanju cene apartmana: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
