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
    /// Interaction logic for Optimizacija.xaml
    /// </summary>
    public partial class Optimizacija : Page
    {
        private readonly IRezervacijaService _rezervacijaService;
        private readonly IApartmanService _apartmanService;
        private readonly ITipApartmanaService _tipartmanaService;
        private List<Apartman> _sviApartmani = new List<Apartman>();

        public Optimizacija(IRezervacijaService rezervacijaService, IApartmanService apartmanService, ITipApartmanaService tipApartmanaService)
        {
            _rezervacijaService = rezervacijaService;
            _apartmanService = apartmanService;   
            _tipartmanaService = tipApartmanaService;
            InitializeComponent();
            UcitajPodatke();

        }
        //pravi problem ako ucitavam za svaki ComboBox podatke iz posebne metode i onda stavljam u konstruktor!!
        //Zato kreirati samo jednu metodu unutar koje se pune svi !!
        private async void UcitajPodatke()
        {
            try
            {
                // Učitavanje svih apartmana
                var apartmani = await _apartmanService.GetAllApartman();
                _sviApartmani = apartmani.ToList();
                ApartmaniListBox.ItemsSource = _sviApartmani;

                // Popunjavanje ComboBox-a za kapacitete
                if (_sviApartmani == null || !_sviApartmani.Any())
                {
                    MessageBox.Show("Nema dostupnih apartmana za pretragu kapaciteta.", "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (_sviApartmani == null || !_sviApartmani.Any())
                        return;

                    BrojKrevetaComboBox.Items.Clear();
                    BrojKrevetaComboBox.Items.Add("Odaberite broj kreveta"); // Default opcija

                    var brojeviKreveta = _sviApartmani.Select(a => a.kapacitetOdrasli).Distinct().OrderBy(k => k).ToList();

                    foreach (var brojKreveta in brojeviKreveta)
                    {
                        BrojKrevetaComboBox.Items.Add(brojKreveta.ToString()); // Dodavanje broja kreveta kao string
                    }

                    BrojKrevetaComboBox.SelectedIndex = 0; // Postavljanje default selekcije
                }

                // Učitavanje tipova apartmana
                var tipovi = await _tipartmanaService.GetAllTipApartmana();

                var defaultTip = new TipApartmana
                {
                    tipApartmanaId = 0,
                    nazivTipaApartmana = "Odaberite tip apartmana"
                };

                tipovi.Insert(0, defaultTip);

                TipApartmanaComboBox.ItemsSource = tipovi;
                TipApartmanaComboBox.DisplayMemberPath = "nazivTipaApartmana";
                TipApartmanaComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju podataka: " + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OdaberiSve_Click(object sender, RoutedEventArgs e)
        {
            // Ako je ListBox popunjen, selektujemo sve apartmane
            if (ApartmaniListBox.ItemsSource != null)
            {
                // Selektujemo sve stavke u ListBox-u
                foreach (var item in ApartmaniListBox.Items)
                {
                    ApartmaniListBox.SelectedItems.Add(item);
                }
            }
        }
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            var selectedTip = TipApartmanaComboBox.SelectedItem as TipApartmana;
            var selectedBrojKreveta = BrojKrevetaComboBox.SelectedItem as string;

            // Filtriramo apartmane na osnovu selektovanih vrednosti
            var filteredApartmani = _sviApartmani.AsEnumerable();

            // Filtriramo prema tipu apartmana, ako je tip selektovan
            if (selectedTip != null && selectedTip.tipApartmanaId > 0) // Provera da odabrani tip nije 0
            {
                filteredApartmani = filteredApartmani.Where(a => a.tipApartmanaId == selectedTip.tipApartmanaId);
            }

            // Filtriramo prema broju kreveta, ako je broj kreveta selektovan
            if (!string.IsNullOrEmpty(selectedBrojKreveta) && selectedBrojKreveta != "Odaberite kapacitet")
            {
                if (int.TryParse(selectedBrojKreveta, out var brojKreveta))
                {
                    filteredApartmani = filteredApartmani.Where(a => a.kapacitetOdrasli == brojKreveta);
                }
            }

            // Ažuriramo listu apartmana u ListBox-u
            ApartmaniListBox.ItemsSource = filteredApartmani.ToList();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // Resetovanje datuma
            DatumOdPicker.SelectedDate = null;
            DatumDoPicker.SelectedDate = null;

            // Resetovanje ComboBox-ova
            if (TipApartmanaComboBox.Items.Count > 0)
                TipApartmanaComboBox.SelectedIndex = 0;

            if (BrojKrevetaComboBox.Items.Count > 0)
                BrojKrevetaComboBox.SelectedIndex = 0;

            // Poništavanje selekcije u ListBox-u
            ApartmaniListBox.SelectedItems.Clear();

            // Pražnjenje DataGrid-a s predlozima
            PredloziDataGrid.ItemsSource = null;
        }
        private async void GenerisiPredloge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var datumOd = DatumOdPicker.SelectedDate;
                var datumDo = DatumDoPicker.SelectedDate;

                if (datumOd == null || datumDo == null)
                {
                    MessageBox.Show("Molimo izaberite datumski opseg.");
                    return;
                }

                var selektovaniApartmani = ApartmaniListBox.SelectedItems.Cast<Apartman>().ToList();
                if (!selektovaniApartmani.Any())
                {
                    MessageBox.Show("Izaberite barem jedan apartman.");
                    return;
                }

                var predlozi = await _rezervacijaService.GenerisiPredlogeZaOptimizacijuAsync(                   
                    datumOd.Value,
                    datumDo.Value,
                    selektovaniApartmani.Select(a => a.apartmanId).ToList()
                );

                MessageBox.Show($"Broj predloga: {predlozi.Count}");

                foreach (var p in predlozi)
                {
                    MessageBox.Show($" ApartmanID: {p.ApartmanId}, Novi: {p.NoviPocetak} - {p.NoviKraj}");
                }

                // Priprema za prikaz u tabeli
                var prikazPredloga = predlozi
                .Where(p =>
                    selektovaniApartmani.Any(a => a.apartmanId == p.ApartmanId) &&
                    selektovaniApartmani.Any(a => a.apartmanId == p.ApartmanUKojiIde))
                .Select(p => new
                {
                    IzApartmana = selektovaniApartmani.FirstOrDefault(a => a.apartmanId == p.ApartmanId)?.nazivApartmana ?? "Nepoznat",
                    UApartman = selektovaniApartmani.FirstOrDefault(a => a.apartmanId == p.ApartmanUKojiIde)?.nazivApartmana ?? "Nepoznat",
                    StariPeriod = $"{p.StariPocetak:dd.MM.yyyy} - {p.StariKraj:dd.MM.yyyy}",
                    NoviPeriod = $"{p.NoviPocetak:dd.MM.yyyy} - {p.NoviKraj:dd.MM.yyyy}"
                }).ToList();

                PredloziDataGrid.ItemsSource = prikazPredloga;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri generisanju predloga: " + ex.Message);
            }
        }
    }
}
