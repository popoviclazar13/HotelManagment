using HotelManagment.Entitys;
using HotelManagment.Service;
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
    /// Interaction logic for ApartmaniPage.xaml
    /// </summary>
    public partial class ApartmaniPage : Page
    {
        private readonly IApartmanService _apartmanService;
        private readonly IPopustService _popustService;
        private readonly IRezervacijaService _rezervacijaService;
        private readonly IKorisnikService _korisnikService;
        private readonly IAgencijaService _agencijaService;
        private readonly ICenaApartmanaService _cenaApartmanaService;
        private readonly IRezervacijaUslugaService _rezervacijaUslugaService;
        private readonly IUslugaService _uslugaService;
        private readonly int _zgradaId;
        public ApartmaniPage(int zgradaId, IApartmanService apartmanService, IPopustService popustService, IRezervacijaService rezervacijaService, IKorisnikService korisnikService, IAgencijaService agencijaService, ICenaApartmanaService cenaApartmanaService, IRezervacijaUslugaService rezervacijaUslugaService, IUslugaService uslugaService)
        {
            InitializeComponent();
            _apartmanService = apartmanService; // Servis dobijamo preko Dependency Injection-a
            _zgradaId = zgradaId;
            _popustService = popustService;
            _rezervacijaService = rezervacijaService;
            _korisnikService = korisnikService;
            _agencijaService = agencijaService;
            _cenaApartmanaService = cenaApartmanaService;
            _rezervacijaUslugaService = rezervacijaUslugaService;
            _uslugaService = uslugaService;
            LoadApartmani();
            
        }
        private async void LoadApartmani()
        {
            var apartmani = await _apartmanService.GetApartmaniByZgradaId(_zgradaId);

            // Grupisanje apartmana po spratu
            var apartmaniPoSpratu = apartmani.GroupBy(a => a.brojSprata).OrderBy(g => g.Key);

            // Čistimo panel pre dodavanja novih podataka
            ApartmaniPanel.Children.Clear();

            foreach (var grupa in apartmaniPoSpratu)
            {
                // Naslov za sprat
                TextBlock spratTitle = new TextBlock
                {
                    Text = $"Sprat {grupa.Key}",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 5),
                    TextAlignment = TextAlignment.Center
                };

                // StackPanel za grupisanje apartmana na istom spratu
                StackPanel spratPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                foreach (var apartman in grupa)
                {
                    // Proveravamo da li je apartman slobodan na osnovu trenutnog datuma
                    //bool isAvailable = await _apartmanService.IsApartmanAvailable(apartman.apartmanId);
                    bool isAvailable = await IsApartmanAvailable(apartman.apartmanId);

                    Button apartmanButton = new Button
                    {
                        Width = 200,
                        Height = 50,
                        Margin = new Thickness(10),
                        Background = isAvailable ? Brushes.LightGreen : Brushes.Red, // Boja zavisi od dostupnosti
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(2),
                        Tag = apartman.apartmanId
                    };

                    TextBlock textBlock = new TextBlock
                    {
                        Text = apartman.nazivApartmana,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontWeight = FontWeights.Bold,
                        FontSize = 14,
                        Foreground = Brushes.Black
                    };

                    apartmanButton.Content = textBlock;
                    // Dodajemo event handler za navigaciju
                    apartmanButton.Click += ApartmanButton_Click;

                    // Dodavanje dugmadi za apartmane na odgovarajući sprat
                    spratPanel.Children.Add(apartmanButton);
                }

                // Dodajemo naslov i apartmane u glavni panel
                ApartmaniPanel.Children.Add(spratTitle);
                ApartmaniPanel.Children.Add(spratPanel);
            }
        }
        private void ApartmanButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int apartmanId)
            {
                // Navigacija na detalje apartmana
                NavigationService?.Navigate(new ApartmaniDetailsPage(apartmanId, _apartmanService, _popustService, _rezervacijaService, _korisnikService, _agencijaService, _cenaApartmanaService, _rezervacijaUslugaService, _uslugaService));
            }
        }
        public async Task<bool> IsApartmanAvailable(int apartmanId)
        {
            var rezervacije = await _rezervacijaService.GetRezervacijeByApartmanId(apartmanId);
            var currentDate = DateTime.Now;

            // Proverite sve rezervacije za apartman i proverite da li je apartman zauzet na trenutni datum
            return !rezervacije.Any(r => r.pocetniDatum <= currentDate && r.krajnjiDatum >= currentDate);
        }
    }
}
