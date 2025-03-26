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
    /// Interaction logic for AllApartmanPage.xaml
    /// </summary>
    public partial class AllApartmanPage : Page
    {
        private readonly IApartmanService _apartmanService;
        private readonly IRezervacijaService _rezervacijaService;
        private List<Apartman> _sviApartmani = new List<Apartman>();
        private List<Apartman> _filtriraniApartmani = new List<Apartman>();

        public AllApartmanPage(IApartmanService apartmanService, IRezervacijaService rezervacijaService)
        {
            InitializeComponent();
            _apartmanService = apartmanService;
            _rezervacijaService = rezervacijaService;
            LoadApartmani();
        }
        /*private async void LoadApartmani()
        {

                var apartmani = await _apartmanService.GetAllApartman();  // Assuming GetAllApartmani returns a list of apartments
                _sviApartmani = apartmani.ToList();  // Save the fetched apartments
                ApartmaniDataGrid.ItemsSource = apartmani;  // Bind data to the DataGrid
                PopuniComboBoxTipApartmana();  // Populate filter combo box for apartment types (if needed)

                // Set default selection for the "Zauzet" filter
                ZauzetFilterComboz.SelectedIndex = 0;  // Set "Odaberite status" as the initial selection
            
        }*/
        private async void LoadApartmani()
        {
            try
            {
                var apartmani = await _apartmanService.GetAllApartman().ConfigureAwait(false);  // Prekida asinhrono čekanje na glavnoj niti
                _sviApartmani = apartmani.ToList();  // Spremi učitane apartmane

                // Korišćenje Dispatcher.Invoke da bi ažurirao UI na glavnoj niti
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ApartmaniDataGrid.ItemsSource = apartmani;  // Binduj podatke na DataGrid
                    PopuniComboBoxTipApartmana();  // Popuni filter combo box (ako je potrebno)
                    PopuniComboBoxApartmana();

                    // Setuj podrazumevani odabir za "Zauzet" filter
                    ZauzetFilterComboz.SelectedIndex = 0;  // Setuj "Odaberite status" kao početni izbor
                });
            }
            catch (Exception ex)
            {
                // Obrada greške, ako nešto pođe po zlu
                Console.WriteLine($"Greška pri učitavanju apartmana: {ex.Message}");
            }
        }

        private void PopuniComboBoxTipApartmana()
        {
            // Assuming you have a list of apartment types to filter by (this could come from the service)
            var tipoviApartmana = _sviApartmani.Select(a => a.tipApartmana.nazivTipaApartmana).Distinct().ToList();

            TipApartmanaFilterComboBox.Items.Clear();
            TipApartmanaFilterComboBox.Items.Add("Odaberite tip apartmana");  // Default filter option

            foreach (var tip in tipoviApartmana)
            {
                TipApartmanaFilterComboBox.Items.Add(tip);  // Add distinct apartment types
            }
            TipApartmanaFilterComboBox.SelectedIndex = 0;
        }
        private void PopuniComboBoxApartmana()
        {
            if (_sviApartmani == null || !_sviApartmani.Any())
                return;

            ApartmentFilterComboBox.Items.Clear();
            ApartmentFilterComboBox.Items.Add("Odaberite apartman"); // Default option

            foreach (var apartman in _sviApartmani)
            {
                ApartmentFilterComboBox.Items.Add(apartman.nazivApartmana); // Dodavanje naziva apartmana
            }

            ApartmentFilterComboBox.SelectedIndex = 0; // Postavljanje default selekcije
        }

        /*private async Task FiltrirajApartmane()
        {
            var filtriraniApartmani = _sviApartmani.AsEnumerable();

            // Filter po tipu apartmana
            var selectedTipApartmana = TipApartmanaFilterComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedTipApartmana) && selectedTipApartmana != "Odaberite tip apartmana")
            {
                filtriraniApartmani = filtriraniApartmani.Where(a => a.tipApartmana.nazivTipaApartmana == selectedTipApartmana);
            }

            // Filter po zauzetosti
            var selectedZauzetItem = ZauzetFilterComboz.SelectedItem as ComboBoxItem;
            string selectedZauzetStatus = selectedZauzetItem?.Content.ToString();

            DateTime? pocetniDatum = PocetniDatumPicker.SelectedDate;
            DateTime? krajnjiDatum = KrajnjiDatumPicker.SelectedDate;

            if (!string.IsNullOrEmpty(selectedZauzetStatus) && selectedZauzetStatus != "Odaberite status"
                && pocetniDatum.HasValue && krajnjiDatum.HasValue)
            {
                bool trazimoZauzete = selectedZauzetStatus == "Zauzet";

                    var sveRezervacije = await _rezervacijaService.GetAllRezervacija();

                    filtriraniApartmani = filtriraniApartmani.Where(a =>
                    {
                        bool apartmanJeZauzet = sveRezervacije.Any(r =>
                            r.apartmanId == a.apartmanId &&
                            ((pocetniDatum.Value >= r.pocetniDatum && pocetniDatum.Value <= r.krajnjiDatum) ||
                             (krajnjiDatum.Value >= r.pocetniDatum && krajnjiDatum.Value <= r.krajnjiDatum) ||
                             (pocetniDatum.Value <= r.pocetniDatum && krajnjiDatum.Value >= r.krajnjiDatum)));

                        return trazimoZauzete ? apartmanJeZauzet : !apartmanJeZauzet;
                    });

                
            }
            // Ažuriraj listu filtriranih apartmana
            _filtriraniApartmani = filtriraniApartmani.ToList();

            ApartmaniDataGrid.ItemsSource = filtriraniApartmani.ToList();
            ApartmaniDataGrid.Items.Refresh();

        }*/
        private async Task FiltrirajApartmane()
        {
            var filtriraniApartmani = _sviApartmani.AsEnumerable();

            // **Filter po tipu apartmana**
            var selectedTipApartmana = TipApartmanaFilterComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedTipApartmana) && selectedTipApartmana != "Odaberite tip apartmana")
            {
                filtriraniApartmani = filtriraniApartmani.Where(a => a.tipApartmana.nazivTipaApartmana == selectedTipApartmana);
            }

            // **Filter po zauzetosti**
            var selectedZauzetItem = ZauzetFilterComboz.SelectedItem as ComboBoxItem;
            string selectedZauzetStatus = selectedZauzetItem?.Content.ToString();

            if (string.IsNullOrEmpty(selectedZauzetStatus) || selectedZauzetStatus == "Odaberite status")
            {
                selectedZauzetStatus = null;
            }

            // **Filter po selektovanom apartmanu iz ComboBox-a**
            var selectedApartmentName = ApartmentFilterComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedApartmentName) && selectedApartmentName != "Odaberite apartman")
            {
                filtriraniApartmani = filtriraniApartmani.Where(a => a.nazivApartmana == selectedApartmentName);
            }

            DateTime? pocetniDatum = PocetniDatumPicker.SelectedDate;
            DateTime? krajnjiDatum = KrajnjiDatumPicker.SelectedDate;

            List<Rezervacija> sveRezervacije = null;

            // **Učitavanje rezervacija pre filtriranja**
            try
            {
                sveRezervacije = await _rezervacijaService.GetAllRezervacija();
                if (sveRezervacije == null)
                {
                    Console.WriteLine("Greška: Nema rezervacija");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom učitavanja rezervacija: {ex.Message}");
                return;
            }

            // **Filter po datumu i zauzetosti**
            if (!string.IsNullOrEmpty(selectedZauzetStatus) && pocetniDatum.HasValue && krajnjiDatum.HasValue)
            {
                bool trazimoZauzete = selectedZauzetStatus == "Zauzet";

                filtriraniApartmani = filtriraniApartmani.Where(a =>
                {
                    bool apartmanJeZauzet = sveRezervacije.Any(r =>
                        r.apartmanId == a.apartmanId &&
                        ((pocetniDatum.Value >= r.pocetniDatum && pocetniDatum.Value <= r.krajnjiDatum) ||
                         (krajnjiDatum.Value >= r.pocetniDatum && krajnjiDatum.Value <= r.krajnjiDatum) ||
                         (pocetniDatum.Value <= r.pocetniDatum && krajnjiDatum.Value >= r.krajnjiDatum)));

                    return trazimoZauzete ? apartmanJeZauzet : !apartmanJeZauzet;
                });
            }

            // **Poziv metode PopuniTermine samo ako su datumi selektovani**
            if (pocetniDatum.HasValue && krajnjiDatum.HasValue)
            {
                foreach (var apartman in filtriraniApartmani)
                {
                    await PopuniTermine(apartman, sveRezervacije);
                }
            }

            // **Ažuriranje filtriranih apartmana**
            _filtriraniApartmani = filtriraniApartmani.ToList();

            if (_filtriraniApartmani.Count == 0)
            {
                Console.WriteLine("Nema filtriranih apartmana");
            }

            // **Ažuriranje prikaza u DataGrid-u**
            ApartmaniDataGrid.ItemsSource = _filtriraniApartmani;
            ApartmaniDataGrid.Items.Refresh();
        }
        /*private async Task FiltrirajApartmane()
        {
            var filtriraniApartmani = _sviApartmani.AsEnumerable();

            // Filter po tipu apartmana
            var selectedTipApartmana = TipApartmanaFilterComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedTipApartmana) && selectedTipApartmana != "Odaberite tip apartmana")
            {
                filtriraniApartmani = filtriraniApartmani.Where(a => a.tipApartmana.nazivTipaApartmana == selectedTipApartmana);
            }

            // Filter po zauzetosti
            var selectedZauzetItem = ZauzetFilterComboz.SelectedItem as ComboBoxItem;
            string selectedZauzetStatus = selectedZauzetItem?.Content.ToString();

            // Proveriti da li je selectedZauzetStatus validan
            if (string.IsNullOrEmpty(selectedZauzetStatus) || selectedZauzetStatus == "Odaberite status")
            {
                // Ako nije selektovan validan status, samo vraćamo bez filtriranja po zauzetosti
                selectedZauzetStatus = null;
            }

            DateTime? pocetniDatum = PocetniDatumPicker.SelectedDate;
            DateTime? krajnjiDatum = KrajnjiDatumPicker.SelectedDate;

            if (!string.IsNullOrEmpty(selectedZauzetStatus) && pocetniDatum.HasValue && krajnjiDatum.HasValue)
            {
                bool trazimoZauzete = selectedZauzetStatus == "Zauzet";

                var sveRezervacije = await _rezervacijaService.GetAllRezervacija();
                if (sveRezervacije == null)
                {
                    // Ako nema rezervacija, loguj grešku
                    Console.WriteLine("Greška: Nema rezervacija");
                    return;
                }

                filtriraniApartmani = filtriraniApartmani.Where(a =>
                {
                    bool apartmanJeZauzet = sveRezervacije.Any(r =>
                        r.apartmanId == a.apartmanId &&
                        ((pocetniDatum.Value >= r.pocetniDatum && pocetniDatum.Value <= r.krajnjiDatum) ||
                         (krajnjiDatum.Value >= r.pocetniDatum && krajnjiDatum.Value <= r.krajnjiDatum) ||
                         (pocetniDatum.Value <= r.pocetniDatum && krajnjiDatum.Value >= r.krajnjiDatum)));

                    return trazimoZauzete ? apartmanJeZauzet : !apartmanJeZauzet;
                });
            }

            // Ako je filtriranje uspešno, ažuriraj listu filtriranih apartmana
            _filtriraniApartmani = filtriraniApartmani.ToList();

            // Prikazivanje greške ako lista filtriranih apartmana nije popunjena
            if (_filtriraniApartmani.Count == 0)
            {
                Console.WriteLine("Nema filtriranih apartmana");
            }

            // Ažuriraj prikaz u DataGridu
            ApartmaniDataGrid.ItemsSource = _filtriraniApartmani;
            ApartmaniDataGrid.Items.Refresh();
        }*/

        private async Task PopuniTermine(Apartman apartman, List<Rezervacija> sveRezervacije)
        {
            if (apartman == null || sveRezervacije == null)
                return;

            // Dobijamo selektovane datume iz DatePicker-a
            DateTime? pocetniDatum = PocetniDatumPicker.SelectedDate;
            DateTime? krajnjiDatum = KrajnjiDatumPicker.SelectedDate;

            if (!pocetniDatum.HasValue || !krajnjiDatum.HasValue)
            {
                // Ako nisu selektovani datumi, nemoj raditi ništa
                apartman.ZauzetiTermini = "Nema selektovanog perioda";
                apartman.SlobodniTermini = "Nema selektovanog perioda";
                return;
            }

            // Filtriramo rezervacije koje padaju unutar selektovanog perioda
            var rezervacijeApartmana = sveRezervacije
                .Where(r => r.apartmanId == apartman.apartmanId && r.pocetniDatum != null && r.krajnjiDatum != null)
                .Where(r => (r.pocetniDatum <= krajnjiDatum && r.krajnjiDatum >= pocetniDatum))  // Rezervacija mora da pada unutar selektovanog perioda
                .OrderBy(r => r.pocetniDatum)
                .ToList();

            List<(DateTime, DateTime)> zauzetiPeriodi = rezervacijeApartmana
                .Select(r => (r.pocetniDatum, r.krajnjiDatum))
                .Where(p => p.Item1 <= p.Item2) // Osigurava validne periode
                .ToList();

            List<(DateTime, DateTime)> slobodniPeriodi = new List<(DateTime, DateTime)>();

            DateTime trenutniPocetak = pocetniDatum.Value;

            // Sada dodajemo slobodne periode unutar selektovanog perioda
            foreach (var (zPocetak, zKraj) in zauzetiPeriodi)
            {
                if (trenutniPocetak < zPocetak)
                {
                    // Ako je početak slobodan period pre nego što počinje zauzeti period
                    slobodniPeriodi.Add((trenutniPocetak, zPocetak.AddDays(-1)));
                }
                trenutniPocetak = zKraj.AddDays(1);
            }

            if (trenutniPocetak <= krajnjiDatum.Value)
            {
                // Dodajemo poslednji slobodan period ako postoji
                slobodniPeriodi.Add((trenutniPocetak, krajnjiDatum.Value));
            }

            // Formiramo stringove za zauzete i slobodne termine
            apartman.ZauzetiTermini = zauzetiPeriodi.Any()
                ? string.Join("\n", zauzetiPeriodi.Select(p => $"{p.Item1:dd.MM.}-{p.Item2:dd.MM.}"))
                : "Nema rezervacija";

            apartman.SlobodniTermini = slobodniPeriodi.Any()
                ? string.Join("\n", slobodniPeriodi.Select(p => $"{p.Item1:dd.MM.}-{p.Item2:dd.MM.}"))
                : "Nema slobodnih termina";
        }

        /*private async Task PopuniTermine(Apartman apartman, List<Rezervacija> sveRezervacije)
        {
            if (apartman == null || sveRezervacije == null)
                return;

            var rezervacijeApartmana = sveRezervacije
                .Where(r => r.apartmanId == apartman.apartmanId && r.pocetniDatum != null && r.krajnjiDatum != null)
                .OrderBy(r => r.pocetniDatum)
                .ToList();

            DateTime prviDanMeseca = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime poslednjiDanMeseca = prviDanMeseca.AddMonths(1).AddDays(-1);

            List<(DateTime, DateTime)> zauzetiPeriodi = rezervacijeApartmana
                .Select(r => (r.pocetniDatum, r.krajnjiDatum))
                .Where(p => p.Item1 <= p.Item2) // Osigurava validne periode
                .ToList();

            List<(DateTime, DateTime)> slobodniPeriodi = new List<(DateTime, DateTime)>();

            DateTime trenutniPocetak = prviDanMeseca;

            foreach (var (zPocetak, zKraj) in zauzetiPeriodi)
            {
                if (trenutniPocetak < zPocetak)
                {
                    slobodniPeriodi.Add((trenutniPocetak, zPocetak.AddDays(-1)));
                }
                trenutniPocetak = zKraj.AddDays(1);
            }

            if (trenutniPocetak <= poslednjiDanMeseca)
            {
                slobodniPeriodi.Add((trenutniPocetak, poslednjiDanMeseca));
            }

            apartman.ZauzetiTermini = zauzetiPeriodi.Any()
                ? string.Join("\n", zauzetiPeriodi.Select(p => $"{p.Item1:dd.MM.}-{p.Item2:dd.MM.}"))
                : "Nema rezervacija";

            apartman.SlobodniTermini = slobodniPeriodi.Any()
                ? string.Join("\n", slobodniPeriodi.Select(p => $"{p.Item1:dd.MM.}-{p.Item2:dd.MM.}"))
                : "Nema slobodnih termina";
        }*/

        private async void TipApartmanaFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded && TipApartmanaFilterComboBox.SelectedItem != null)
            {
                await FiltrirajApartmane();
            }
        }

        private async void DatumFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded && PocetniDatumPicker.SelectedDate.HasValue && KrajnjiDatumPicker.SelectedDate.HasValue)
            {
                await FiltrirajApartmane();
            }
        }

        private async void ZauzetFilterComboz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded && ZauzetFilterComboz.SelectedItem != null)
            {
                await FiltrirajApartmane();
            }
        }
        private async void ApartmentFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded && ApartmentFilterComboBox.SelectedItem != null)
                await FiltrirajApartmane();
        }

        private async void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            // Resetovanje filtera
            PocetniDatumPicker.SelectedDate = null;
            KrajnjiDatumPicker.SelectedDate = null;
            TipApartmanaFilterComboBox.SelectedIndex = 0;
            ZauzetFilterComboz.SelectedIndex = 0;
            ApartmentFilterComboBox.SelectedIndex = 0;

            await FiltrirajApartmane(); // Poziv filtriranja asinhrono
        }
        // Dodaj kod za izvoz u Excel
        private async void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Prvo, filtriraj apartmane sa selektovanim filterima
                await FiltrirajApartmane();

                // Kreiraj Excel paket
                using (var package = new ExcelPackage())
                {
                    // Dodaj radni list
                    var worksheet = package.Workbook.Worksheets.Add("Apartmani");

                    // Add header for filter details
                    worksheet.Cells[1, 1].Value = "Filter Details";
                    worksheet.Cells[2, 1].Value = "Pocetni Datum:";
                    worksheet.Cells[2, 2].Value = PocetniDatumPicker.SelectedDate?.ToString("yyyy-MM-dd") ?? "N/A";
                    worksheet.Cells[3, 1].Value = "Krajnji Datum:";
                    worksheet.Cells[3, 2].Value = KrajnjiDatumPicker.SelectedDate?.ToString("yyyy-MM-dd") ?? "N/A";

                    // Add status filter (Zauzet or Slobodan)
                    var selectedZauzetItem = ZauzetFilterComboz.SelectedItem as ComboBoxItem;
                    string selectedZauzetStatus = selectedZauzetItem?.Content.ToString() ?? "N/A";
                    worksheet.Cells[4, 1].Value = "Status:";
                    worksheet.Cells[4, 2].Value = selectedZauzetStatus;

                    // Add a space line between filter details and apartments list
                    worksheet.Cells[6, 1].Value = "Naziv Apartmana";
                    worksheet.Cells[6, 2].Value = "Sprat";
                    worksheet.Cells[6, 3].Value = "Kapacitet";
                    worksheet.Cells[6, 4].Value = "Zauzet";

                    // Populate filtered apartment data starting from row 7
                    int row = 7;
                    foreach (var apartman in _filtriraniApartmani)
                    {
                        worksheet.Cells[row, 1].Value = apartman.nazivApartmana;
                        worksheet.Cells[row, 2].Value = apartman.brojSprata;
                        worksheet.Cells[row, 3].Value = apartman.ukupniKapacitet;
                        worksheet.Cells[row, 4].Value = apartman.zauzet ? "Zauzet" : "Slobodan";
                        row++;
                    }

                    // Spremi Excel fajl
                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Excel Files (*.xlsx)|*.xlsx",
                        FileName = "Apartmani_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        var filePath = saveFileDialog.FileName;
                        FileInfo fileInfo = new FileInfo(filePath);

                        // Spremi fajl
                        package.SaveAs(fileInfo);

                        MessageBox.Show("Podaci su uspešno izvezeni u Excel!", "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom izvoza: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
