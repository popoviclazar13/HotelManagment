using ClosedXML.Excel;
using HotelManagment.DTO.DTORezervacija;
using HotelManagment.Entitys;
using HotelManagment.Service;
using HotelManagment.ServiceRepository;
using Microsoft.Win32;
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
    /// Interaction logic for AllRezervacijePage.xaml
    /// </summary>
    public partial class AllRezervacijePage : Page
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
        private List<Rezervacija> _sveRezervacije = new List<Rezervacija>();
        public AllRezervacijePage(IRezervacijaService rezervacijaService, IApartmanService apartmanService, IKorisnikService korisnikService, IAgencijaService agencijaService, IPopustService popustService, ICenaApartmanaService cenaApartmanaService, IApartmanPopustService apartmanPopustService, IRezervacijaUslugaService rezervacijaUslugaService, IUslugaService uslugaService)
        {
            InitializeComponent();
            _rezervacijaService = rezervacijaService;
            _agencijaService = agencijaService;
            _korisnikService = korisnikService;
            _apartmanService = apartmanService;
            _popustService = popustService;
            _cenaApartmanaService = cenaApartmanaService;
            _apartmanPopustService = apartmanPopustService;
            _rezervacijaUslugaService = rezervacijaUslugaService;
            _uslugaService = uslugaService;
            LoadRezervacije();
        }
        public async void LoadRezervacije()
        {
            try
            {
                var rezervacije = await _rezervacijaService.GetAllRezervacija();
                _sveRezervacije = rezervacije.ToList(); // Save the fetched reservations
                RezervacijeDataGrid.ItemsSource = rezervacije;
                PopuniComboBoxAgencija(); // Populate the Agencija combo box here
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom učitavanja rezervacija: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RezervacijeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RezervacijeDataGrid.SelectedItem is Rezervacija selectedRezervacija && selectedRezervacija.rezervacijaId > 0)
            {
                Action reloadDataAction = LoadRezervacije; // ili neka druga metoda za ponovno učitavanje podataka, ako je potrebno
                NavigationService.Navigate(new RezervacijaEditWindow(_rezervacijaService, selectedRezervacija, reloadDataAction, _korisnikService, _agencijaService, _apartmanService, _rezervacijaUslugaService, _uslugaService));
                RezervacijeDataGrid.SelectedItem = null; // Resetovanje selekcije
            }
        }
        private void PopuniComboBoxPlaceno()
        {
            PlacenoFilterComboBox.Items.Clear();
            PlacenoFilterComboBox.Items.Add("Odaberite status plaćanja");
            PlacenoFilterComboBox.Items.Add("DA"); // For paid reservations
            PlacenoFilterComboBox.Items.Add("NE"); // For unpaid reservations
            PlacenoFilterComboBox.SelectedIndex = 0;
        }
        private void PopuniComboBoxAgencija()
        {
            // Get the list of distinct agency names from the reservations
            var agencije = _sveRezervacije
                .Where(r => r.agencija != null)
                .Select(r => r.agencija.nazivAgencije)
                .Distinct()
                .ToList();

            // Clear previous items in the combo box
            AgencijaFilterComboBox.Items.Clear();

            // Add a default option
            AgencijaFilterComboBox.Items.Add("Odaberite agenciju");

            // Add all distinct agency names to the combo box
            foreach (var agencija in agencije)
            {
                AgencijaFilterComboBox.Items.Add(agencija);
            }

            // Set the default selected index
            AgencijaFilterComboBox.SelectedIndex = 0;
        }
        private void FiltrirajRezervacije()
        {
            var filtriraneRezervacije = _sveRezervacije.AsEnumerable();

            // Filtriranje po Agenciji
            var selectedAgencija = AgencijaFilterComboBox.SelectedItem as string;
            if (selectedAgencija != null && selectedAgencija != "Odaberite agenciju")
            {
                filtriraneRezervacije = filtriraneRezervacije.Where(r => r.agencija != null && r.agencija.nazivAgencije == selectedAgencija);
            }

            // Filtriranje po statusu plaćanja
            var selectedPlaceno = PlacenoFilterComboBox.SelectedItem as ComboBoxItem;
            if (selectedPlaceno != null && selectedPlaceno.Content.ToString() != "Odaberite status plaćanja")
            {
                bool placeno = selectedPlaceno.Content.ToString() == "Plaćeno";
                filtriraneRezervacije = filtriraneRezervacije.Where(r => r.placeno == placeno);
            }

            // Filtriranje po načinu plaćanja
            var selectedNacinPlacanja = NacinPlacanjaFilterComboBox.SelectedItem as ComboBoxItem;
            if (selectedNacinPlacanja != null && selectedNacinPlacanja.Content.ToString() != "Odaberite način plaćanja")
            {
                filtriraneRezervacije = filtriraneRezervacije.Where(r => r.nacinPlacanja == selectedNacinPlacanja.Content.ToString());
            }

            // Filtriranje po osnovnom datumu (PocetniDatumPicker i KrajnjiDatumPicker)
            DateTime? pocetniDatum = PocetniDatumPicker.SelectedDate;
            DateTime? krajnjiDatum = KrajnjiDatumPicker.SelectedDate;

            if (pocetniDatum.HasValue && krajnjiDatum.HasValue)
            {
                // Filter po rasponu datuma
                filtriraneRezervacije = filtriraneRezervacije.Where(r =>
                    (r.pocetniDatum >= pocetniDatum && r.pocetniDatum <= krajnjiDatum) ||
                    (r.krajnjiDatum >= pocetniDatum && r.krajnjiDatum <= krajnjiDatum) ||
                    (r.pocetniDatum <= pocetniDatum && r.krajnjiDatum >= krajnjiDatum)
                );
            }
            else if (pocetniDatum.HasValue)
            {
                filtriraneRezervacije = filtriraneRezervacije.Where(r => r.pocetniDatum >= pocetniDatum);
            }
            else if (krajnjiDatum.HasValue)
            {
                filtriraneRezervacije = filtriraneRezervacije.Where(r => r.krajnjiDatum <= krajnjiDatum);
            }

            // Filtriranje po dodatnim datumima (PocetniDatumRezervacijePicker i KrajnjiDatumRezervacijePicker)
            DateTime? pocetniDatumRezervacije = PocetniDatumRezervacijePicker.SelectedDate;
            DateTime? krajnjiDatumRezervacije = KrajnjiDatumRezervacijePicker.SelectedDate;

            if (pocetniDatumRezervacije.HasValue && krajnjiDatumRezervacije.HasValue)
            {
                // Ako su oba datuma selektovana, filtriraj rezervacije koje imaju datum između
                filtriraneRezervacije = filtriraneRezervacije.Where(r =>
                    (r.pocetniDatum.Date == pocetniDatumRezervacije.Value.Date) ||
                    (r.krajnjiDatum.Date == krajnjiDatumRezervacije.Value.Date)
                );
            }
            else if (pocetniDatumRezervacije.HasValue)
            {
                // Filtriranje samo po početnom datumu rezervacije
                filtriraneRezervacije = filtriraneRezervacije.Where(r => r.pocetniDatum.Date == pocetniDatumRezervacije.Value.Date);
            }
            else if (krajnjiDatumRezervacije.HasValue)
            {
                // Filtriranje samo po krajnjem datumu rezervacije
                filtriraneRezervacije = filtriraneRezervacije.Where(r => r.krajnjiDatum.Date == krajnjiDatumRezervacije.Value.Date);
            }

            // Ažuriranje DataGrid-a sa filtriranim rezervacijama
            RezervacijeDataGrid.ItemsSource = filtriraneRezervacije.ToList();

            // Izračunavanje ukupne cene - treba da se ažurira svaki put kad se filtrira
            double ukupnaCena = filtriraneRezervacije.Sum(r => r.cenaKonacna);

            // Izračunavanje ukupne provizije
            double ukupnaProvizija = filtriraneRezervacije.Sum(r => r.iznosProvizije); // pretpostavljam da postoji atribut provizija

            // Izračunavanje bruto cene (Ukupna cena - ukupna provizija)
            double brutoCena = ukupnaCena - ukupnaProvizija;

            UkupnaCenaTextBlock.Text = $"Bruto cena: {ukupnaCena:F2} EUR";
            NetoCenaTextBlock.Text = $"Neto cena: {brutoCena:F2} EUR";
        }


        private void AgencijaFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                FiltrirajRezervacije();
        }

        private void PlacenoFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                FiltrirajRezervacije();
        }

        private void NacinPlacanjaFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                FiltrirajRezervacije();
        }

        private void KreirajRezervacijuButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming you have a page or dialog for creating a reservation.
            // Navigate to a page to create a new reservation.
            Action reloadDataAction = LoadRezervacije;
            NavigationService.Navigate(new CreateRezervacijaPage(_rezervacijaService, _korisnikService, _agencijaService, _apartmanService, _popustService, _cenaApartmanaService, _apartmanPopustService, _rezervacijaUslugaService, _uslugaService, reloadDataAction));
        }

        private void OcistiFiltereButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset all filter combo boxes to the default state
            AgencijaFilterComboBox.SelectedIndex = 0;
            PlacenoFilterComboBox.SelectedIndex = 0;
            NacinPlacanjaFilterComboBox.SelectedIndex = 0;

            // Reset date pickers
            PocetniDatumPicker.SelectedDate = null;
            KrajnjiDatumPicker.SelectedDate = null;
            //Reset date pickers
            PocetniDatumRezervacijePicker.SelectedDate = null;
            KrajnjiDatumRezervacijePicker.SelectedDate = null;

            // Refresh the reservation list without any filters applied
            FiltrirajRezervacije();
        }
        private void DatumFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                if (PocetniDatumPicker.SelectedDate.HasValue)
                {
                    // Postavi minimalni datum za krajnji datum na vrednost početnog datuma
                    KrajnjiDatumPicker.DisplayDateStart = PocetniDatumPicker.SelectedDate.Value;

                    // Ako korisnik odabere loš datum za krajnji datum, poništi ga
                    if (KrajnjiDatumPicker.SelectedDate.HasValue &&
                        KrajnjiDatumPicker.SelectedDate.Value < PocetniDatumPicker.SelectedDate.Value)
                    {
                        KrajnjiDatumPicker.SelectedDate = null;
                    }

                    // Blokiraj sve datume pre početnog datuma na krajnjem DatePickeru
                    KrajnjiDatumPicker.BlackoutDates.Clear();
                    KrajnjiDatumPicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, PocetniDatumPicker.SelectedDate.Value.AddDays(-1)));
                }

                // Poziv metode za filtriranje rezervacija
                FiltrirajRezervacije();
            }
        }
        private void PocetniDatumRezervacijePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if (PocetniDatumRezervacijePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = PocetniDatumRezervacijePicker.SelectedDate.Value;
                // Poziv metode za filtriranje rezervacija koje počinju na odabrani datum
                _ = FilterReservationsByStartDate(selectedDate);
            }*/
            if (PocetniDatumRezervacijePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = PocetniDatumRezervacijePicker.SelectedDate.Value;
                // Poziv metode za filtriranje rezervacija koje počinju na odabrani datum
                FiltrirajRezervacije();
            }
        }

        private void KrajnjiDatumRezervacijePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if (KrajnjiDatumRezervacijePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = KrajnjiDatumRezervacijePicker.SelectedDate.Value;
                // Poziv metode za filtriranje rezervacija koje se završavaju na odabrani datum
                _ = FilterReservationsByEndDate(selectedDate);
            }*/
            if (KrajnjiDatumRezervacijePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = KrajnjiDatumRezervacijePicker.SelectedDate.Value;
                // Poziv metode za filtriranje rezervacija koje se završavaju na odabrani datum
                FiltrirajRezervacije();
            }
        }

        private async Task FilterReservationsByStartDate(DateTime startDate)
        {
            // Logika za filtriranje rezervacija koje počinju na selectedDate
            var filteredReservations = await _rezervacijaService.GetRezervacijeByPocetniDatum(startDate);
            RezervacijeDataGrid.ItemsSource = filteredReservations;
        }

        private async Task FilterReservationsByEndDate(DateTime endDate)
        {
            // Logika za filtriranje rezervacija koje se završavaju na selectedDate
            var filteredReservations = await _rezervacijaService.GetRezervacijeByKrajnjiDatum(endDate);
            RezervacijeDataGrid.ItemsSource = filteredReservations;
        }
        private async void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel fajl (*.xlsx)|*.xlsx",
                FileName = "Rezervacije.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Rezervacije");

                        // Piši zaglavlja
                        worksheet.Cell(1, 1).Value = "Apartman";
                        worksheet.Cell(1, 2).Value = "Gost";
                        worksheet.Cell(1, 3).Value = "Dolazak";
                        worksheet.Cell(1, 4).Value = "Odlazak";
                        worksheet.Cell(1, 5).Value = "Broj noćenja";
                        worksheet.Cell(1, 6).Value = "Ukupna cena";
                        worksheet.Cell(1, 7).Value = "Provizija";
                        worksheet.Cell(1, 8).Value = "Zahtevi";
                        worksheet.Cell(1, 9).Value = "Zemlja";
                        worksheet.Cell(1, 10).Value = "SobaID";
                        worksheet.Cell(1, 11).Value = "Telefon";
                        worksheet.Cell(1, 12).Value = "Broj gostiju";
                        worksheet.Cell(1, 13).Value = "Placeno";

                        // Podaci iz DataGrida
                        var rezervacije = RezervacijeDataGrid.ItemsSource.Cast<Rezervacija>().ToList();

                        for (int i = 0; i < rezervacije.Count; i++)
                        {
                            var r = rezervacije[i];
                            worksheet.Cell(i + 2, 1).Value = r.apartman?.nazivApartmana ?? "";
                            worksheet.Cell(i + 2, 2).Value = r.korisnik?.imePrezime ?? "";
                            worksheet.Cell(i + 2, 3).Value = r.pocetniDatum.ToString("dd.MM.yyyy");
                            worksheet.Cell(i + 2, 4).Value = r.krajnjiDatum.ToString("dd.MM.yyyy");
                            worksheet.Cell(i + 2, 5).Value = (r.krajnjiDatum - r.pocetniDatum).Days;
                            worksheet.Cell(i + 2, 6).Value = r.ukupnaCena;
                            worksheet.Cell(i + 2, 7).Value = r.iznosProvizije;
                            worksheet.Cell(i + 2, 8).Value = r.komentar;
                            worksheet.Cell(i + 2, 9).Value = r.korisnik?.zemlja ?? "";
                            worksheet.Cell(i + 2, 10).Value = r.apartman?.apartmanId ?? 0;
                            worksheet.Cell(i + 2, 11).Value = r.korisnik?.telefon ?? "";
                            worksheet.Cell(i + 2, 12).Value = r.brojGostiju;
                            worksheet.Cell(i + 2, 13).Value = r.placeno ? "DA" : "NE";
                        }

                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Podaci uspešno izvezeni u Excel!", "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri izvozu: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
