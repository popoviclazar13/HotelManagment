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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly IZgradaService _zgradaService;
        private readonly IApartmanService _apartmanService;
        private readonly IPopustService _popustService;
        private readonly IRezervacijaService _rezervacijaService;
        private readonly IKorisnikService _korisnikService;
        private readonly IAgencijaService _agencijaService;
        private readonly ICenaApartmanaService _cenaApartmanaService;
        private readonly IUslugaService _uslugaService;
        private readonly IOpremaService _opremaService;
        private readonly IApartmanPopustService _apartmanPopustService;
        public HomePage(IZgradaService zgradaService, IApartmanService apartmanService, IPopustService popustService, IRezervacijaService rezervacijaService, IKorisnikService korisnikService, IAgencijaService agencijaService, ICenaApartmanaService cenaApartmanaService, IUslugaService uslugaService, IOpremaService opremaService, IApartmanPopustService apartmanPopustService)
        {
            InitializeComponent();
            _zgradaService = zgradaService;
            _apartmanService = apartmanService;
            _popustService = popustService;
            _rezervacijaService = rezervacijaService;
            _korisnikService = korisnikService;
            _agencijaService = agencijaService;
            _cenaApartmanaService = cenaApartmanaService;
            _uslugaService = uslugaService;
            _opremaService = opremaService;
            _apartmanPopustService = apartmanPopustService;
            LoadBuildings();
        }

        private async void LoadBuildings()
        {
            //try
            //{
                // Dohvatanje svih Zgrada iz baze
                var zgrade = await _zgradaService.GetAllZgrada();

                // Kreiranje dugmadi za svaku zgradu
                foreach (var zgrada in zgrade)
                {
                    Button buildingButton = new Button
                    {
                        Width = 100,
                        Height = 100,
                        Margin = new Thickness(10),
                        Background = System.Windows.Media.Brushes.SkyBlue,
                        BorderBrush = System.Windows.Media.Brushes.Black,
                        BorderThickness = new Thickness(2),
                        Tag = zgrada.zgradaId // Čuvamo ID zgrade u Tag svojstvu
                    };

                    TextBlock textBlock = new TextBlock
                    {
                        Text = zgrada.naziv,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Foreground = System.Windows.Media.Brushes.Black
                    };

                    buildingButton.Content = textBlock;
                    buildingButton.Click += BuildingButton_Click;

                // Dodavanje dugmeta na WrapPanel
                BuildingsPanel.Children.Add(buildingButton);
                }
        }
        private void BuildingButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int zgradaId)
            {
                NavigationService?.Navigate(new ApartmaniPage(zgradaId, _apartmanService, _popustService, _rezervacijaService, _korisnikService, _agencijaService, _cenaApartmanaService));
            }
        }
        private void Rezervacije_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AllRezervacijePage(_rezervacijaService, _apartmanService, _korisnikService, _agencijaService, _popustService, _cenaApartmanaService, _apartmanPopustService));
        }
        private void Usluge_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AllUslugePage(_uslugaService));
        }
        private void Oprema_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AllOpremaPage(_opremaService));
        }
        private void Popusti_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AllPopustPage(_popustService));
        }
        private void Korisnici_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AllKorisniciPage(_korisnikService));
        }

    }
}
