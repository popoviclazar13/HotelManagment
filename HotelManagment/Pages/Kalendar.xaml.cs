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
    /// Interaction logic for Kalendar.xaml
    /// </summary>
    public partial class Kalendar : Page
    {
        private readonly IRezervacijaService _rezervacijaService;
        private readonly IApartmanService _apartmanService;
        public Kalendar(IRezervacijaService rezervacijaService, IApartmanService apartmanService)
        {
            InitializeComponent();
            _rezervacijaService = rezervacijaService;
            _apartmanService = apartmanService;
            UcitajKalendar(6);
        }
        private async Task UcitajKalendar(int izabraniMesec)
        {
            var rezervacije = await _rezervacijaService.GetAllRezervacija();
            var apartmani = await _apartmanService.GetAllApartman();

            DateTime startDate, endDate;

            if (izabraniMesec >= 6 && izabraniMesec <= 9)
            {
                startDate = new DateTime(DateTime.Now.Year, izabraniMesec, 1);
                endDate = startDate.AddMonths(1).AddDays(-1); // kraj meseca
            }
            else
            {
                startDate = new DateTime(DateTime.Now.Year, 6, 1);
                endDate = new DateTime(DateTime.Now.Year, 9, 30);
            }

            int totalDays = (endDate - startDate).Days + 1;

            KalendarGrid.Columns.Clear();

            // Statične kolone
            KalendarGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Sprat",
                Binding = new Binding("BrojSprata"),
                Width = 60
            });

            KalendarGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Apartman",
                Binding = new Binding("Naziv"),
                Width = 150
            });

            KalendarGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Kreveti",
                Binding = new Binding("Kapacitet"),
                Width = 60
            });

            // Dinamičke kolone po danima
            for (int i = 0; i < totalDays; i++)
            {
                var date = startDate.AddDays(i);
                var dateKey = date.ToString("dd.MM");

                var column = new DataGridTextColumn
                {
                    Header = dateKey,
                    Binding = new Binding($"RezervacijePoDatumu[{dateKey}]"),
                    Width = 60
                };

                var cellStyle = new Style(typeof(DataGridCell));
                cellStyle.Setters.Add(new Setter(DataGridCell.BorderThicknessProperty, new Thickness(0)));
                cellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.Orange));
                cellStyle.Setters.Add(new Setter(DataGridCell.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
                cellStyle.Setters.Add(new Setter(DataGridCell.VerticalContentAlignmentProperty, VerticalAlignment.Center));

                var triggerPrazna = new DataTrigger
                {
                    Binding = new Binding($"RezervacijePoDatumu[{dateKey}]"),
                    Value = ""
                };
                triggerPrazna.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.Transparent));
                triggerPrazna.Setters.Add(new Setter(DataGridCell.BorderThicknessProperty, new Thickness(1)));
                triggerPrazna.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, Brushes.LightGray));

                var triggerSredina = new DataTrigger
                {
                    Binding = new Binding($"RezervacijePoDatumu[{dateKey}]"),
                    Value = " "
                };
                triggerSredina.Setters.Add(new Setter(DataGridCell.BorderThicknessProperty, new Thickness(1)));
                triggerSredina.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, Brushes.Black));

                var triggerKraj = new DataTrigger
                {
                    Binding = new Binding($"JeKrajRezervacijePoDatumu[{dateKey}]"),
                    Value = true
                };
                triggerKraj.Setters.Add(new Setter(DataGridCell.BorderThicknessProperty, new Thickness(1, 1, 3, 1)));
                triggerKraj.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, Brushes.Black));

                cellStyle.Triggers.Add(triggerPrazna);
                cellStyle.Triggers.Add(triggerSredina);
                cellStyle.Triggers.Add(triggerKraj);

                column.CellStyle = cellStyle;

                KalendarGrid.Columns.Add(column);
            }

            var kalendarData = new List<KalendarViewModel>();

            foreach (var apartman in apartmani)
            {
                var kalendar = new KalendarViewModel
                {
                    BrojSprata = apartman.brojSprata,
                    Naziv = apartman.nazivApartmana,
                    Kapacitet = apartman.kapacitetOdrasli,
                    RezervacijePoDatumu = new Dictionary<string, string>(),
                    JeKrajRezervacijePoDatumu = new Dictionary<string, bool>()
                };

                for (int i = 0; i < totalDays; i++)
                {
                    var currentDate = startDate.AddDays(i);
                    var key = currentDate.ToString("dd.MM");

                    var rezervacijeApartmana = rezervacije
                        .Where(r => r.apartmanId == apartman.apartmanId &&
                                    currentDate >= r.pocetniDatum &&
                                    currentDate <= r.krajnjiDatum)
                        .ToList();

                    if (rezervacijeApartmana.Any())
                    {
                        var rezervacija = rezervacijeApartmana.First();
                        int trajanje = (rezervacija.krajnjiDatum - rezervacija.pocetniDatum).Days + 1;

                        if (currentDate.Date == rezervacija.pocetniDatum.AddDays(trajanje / 2).Date)
                        {
                            kalendar.RezervacijePoDatumu[key] = rezervacija.korisnik.imePrezime;
                        }
                        else
                        {
                            kalendar.RezervacijePoDatumu[key] = " ";
                        }

                        kalendar.JeKrajRezervacijePoDatumu[key] = currentDate.Date == rezervacija.krajnjiDatum.Date;
                    }
                    else
                    {
                        kalendar.RezervacijePoDatumu[key] = "";
                        kalendar.JeKrajRezervacijePoDatumu[key] = false;
                    }
                }

                kalendarData.Add(kalendar);
            }

            KalendarGrid.ItemsSource = kalendarData;
        }
        private async void MesecComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MesecComboBox.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Tag.ToString(), out int mesec))
            {
                await UcitajKalendar(mesec); // Prosljeđujemo izabrani mesec
            }
        }
    }

}
