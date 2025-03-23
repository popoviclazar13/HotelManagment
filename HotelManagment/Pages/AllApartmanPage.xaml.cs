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
    /// Interaction logic for AllApartmanPage.xaml
    /// </summary>
    public partial class AllApartmanPage : Page
    {
        private readonly IApartmanService _apartmanService;
        private readonly IRezervacijaService _rezervacijaService;
        private List<Apartman> _sviApartmani = new List<Apartman>();

        public AllApartmanPage(IApartmanService apartmanService, IRezervacijaService rezervacijaService)
        {
            InitializeComponent();
            _apartmanService = apartmanService;
            _rezervacijaService = rezervacijaService;
            LoadApartmani();
        }
        private async void LoadApartmani()
        {
            try
            {
                var apartmani = await _apartmanService.GetAllApartman();  // Assuming GetAllApartmani returns a list of apartments
                _sviApartmani = apartmani.ToList();  // Save the fetched apartments
                ApartmaniDataGrid.ItemsSource = apartmani;  // Bind data to the DataGrid
                PopuniComboBoxTipApartmana();  // Populate filter combo box for apartment types (if needed)

                // Set default selection for the "Zauzet" filter
                ZauzetFilterComboz.SelectedIndex = 0;  // Set "Odaberite status" as the initial selection
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom učitavanja apartmana: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async Task FiltrirajApartmane()
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

            // Debug ispis selektovanih filtera
            MessageBox.Show($"Odabrani filteri:\n" +
                            $"Pocetni Datum: {PocetniDatumPicker.SelectedDate?.ToString("dd.MM.yyyy") ?? "Nije odabran"}\n" +
                            $"Krajnji Datum: {KrajnjiDatumPicker.SelectedDate?.ToString("dd.MM.yyyy") ?? "Nije odabran"}\n" +
                            $"Status: {selectedZauzetStatus ?? "Nije odabran"}",
                            "Debug - Filteri", MessageBoxButton.OK, MessageBoxImage.Information);

            DateTime? pocetniDatum = PocetniDatumPicker.SelectedDate;
            DateTime? krajnjiDatum = KrajnjiDatumPicker.SelectedDate;

            if (!string.IsNullOrEmpty(selectedZauzetStatus) && selectedZauzetStatus != "Odaberite status"
                && pocetniDatum.HasValue && krajnjiDatum.HasValue)
            {
                bool trazimoZauzete = selectedZauzetStatus == "Zauzet";

                try
                {
                    var sveRezervacije = await _rezervacijaService.GetAllRezervacija();

                    MessageBox.Show($"Učitano rezervacija: {sveRezervacije.Count}",
                                    "Debug - Rezervacije", MessageBoxButton.OK, MessageBoxImage.Information);

                    filtriraniApartmani = filtriraniApartmani.Where(a =>
                    {
                        bool apartmanJeZauzet = sveRezervacije.Any(r =>
                            r.apartmanId == a.apartmanId &&
                            ((pocetniDatum.Value >= r.pocetniDatum && pocetniDatum.Value <= r.krajnjiDatum) ||
                             (krajnjiDatum.Value >= r.pocetniDatum && krajnjiDatum.Value <= r.krajnjiDatum) ||
                             (pocetniDatum.Value <= r.pocetniDatum && krajnjiDatum.Value >= r.krajnjiDatum)));

                        return trazimoZauzete ? apartmanJeZauzet : !apartmanJeZauzet;
                    });

                    MessageBox.Show($"Broj filtriranih apartmana: {filtriraniApartmani.Count()}",
                                    "Debug - Filtrirani apartmani", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška prilikom učitavanja rezervacija: {ex.Message}",
                                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            ApartmaniDataGrid.ItemsSource = filtriraniApartmani.ToList();
            ApartmaniDataGrid.Items.Refresh();
        }

        private async void TipApartmanaFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                await FiltrirajApartmane();
        }

        private async void DatumFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                await FiltrirajApartmane();
        }

        private async void ZauzetFilterComboz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("Event SelectionChanged za zauzet filter je pokrenut!",
                    "Debug", MessageBoxButton.OK, MessageBoxImage.Information);
            if (IsLoaded)
                await FiltrirajApartmane();
        }

        private async void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            // Resetovanje filtera
            PocetniDatumPicker.SelectedDate = null;
            KrajnjiDatumPicker.SelectedDate = null;
            TipApartmanaFilterComboBox.SelectedIndex = 0;
            ZauzetFilterComboz.SelectedIndex = 0;

            await FiltrirajApartmane(); // Poziv filtriranja asinhrono
        }
    }
}
