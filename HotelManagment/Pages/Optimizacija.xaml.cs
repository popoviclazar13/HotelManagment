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

        public Optimizacija(IRezervacijaService rezervacijaService, IApartmanService apartmanService)
        {
            _rezervacijaService = rezervacijaService;
            _apartmanService = apartmanService;            
            InitializeComponent();
            LoadApartmani();
        }
        private async void LoadApartmani()
        {
            try
            {
                var apartmani = await _apartmanService.GetAllApartman();
                ApartmaniListBox.ItemsSource = apartmani;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju apartmana: " + ex.Message);
            }
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
                    MessageBox.Show($"ID: {p.RezervacijaId}, ApartmanID: {p.ApartmanId}, Novi: {p.NoviPocetak} - {p.NoviKraj}");
                }

                // Priprema za prikaz u tabeli
                var prikazPredloga = predlozi
                .Where(p =>
                    selektovaniApartmani.Any(a => a.apartmanId == p.ApartmanId) &&
                    selektovaniApartmani.Any(a => a.apartmanId == p.ApartmanUKojiIde))
                .Select(p => new
                {
                    p.RezervacijaId,
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
