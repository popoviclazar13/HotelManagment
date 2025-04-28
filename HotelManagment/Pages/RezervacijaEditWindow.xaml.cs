using HotelManagment.Entitys;
using HotelManagment.Service;
using HotelManagment.ServiceRepository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for RezervacijaEditWindow.xaml
    /// </summary>
    public partial class RezervacijaEditWindow : Page
    {
        private readonly IRezervacijaService _rezervacijaService;
        private readonly IKorisnikService _korisnikService;
        private readonly IAgencijaService _agencijaService;
        private readonly IApartmanService _apartmanService;
        private readonly IRezervacijaUslugaService _rezervacijaUslugaService;
        private readonly IUslugaService _uslugaService;
        public Rezervacija SelectedReservation { get; set; }
        private readonly Action _reloadDataAction;
        public RezervacijaEditWindow(IRezervacijaService rezervacijaService, Rezervacija selectedReservation, Action reloadDataAction, IKorisnikService korisnikService, IAgencijaService agencijaService, IApartmanService apartmanService, IRezervacijaUslugaService rezervacijaUslugaService, IUslugaService uslugaService)
        {
            InitializeComponent();
            _rezervacijaService = rezervacijaService;
            _korisnikService = korisnikService;
            _agencijaService = agencijaService;
            SelectedReservation = selectedReservation;
            _reloadDataAction = reloadDataAction;
            _apartmanService = apartmanService;
            _rezervacijaUslugaService = rezervacijaUslugaService;
            _uslugaService = uslugaService;
            LoadComboBoxData();
            LoadGuestCountComboBox();

            // Postavljanje početnih vrednosti
            StartDatePicker.SelectedDate = selectedReservation.pocetniDatum;
            EndDatePicker.SelectedDate = selectedReservation.krajnjiDatum;
            GuestCountComboBox.SelectedItem = selectedReservation.brojGostiju;
            PriceTextBox.Text = selectedReservation.cenaKonacna.ToString("F2");
            PaymentMethodComboBox.SelectedItem = PaymentMethodComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == selectedReservation.nacinPlacanja);
            CommissionAmountTextBox.Text = selectedReservation.iznosProvizije.ToString("F2");
            PaidCheckBox.IsChecked = selectedReservation.placeno;
            CommentTextBox.Text = selectedReservation.komentar;

            // Postavljanje selektovanih vrednosti za korisnika i agenciju
            UserComboBox.SelectedValue = selectedReservation.korisnik?.korisnikId;
            AgencyComboBox.SelectedValue = selectedReservation.agencija?.agencijaId;
        }
        private async void LoadComboBoxData()
        {
            try
            {
                var korisnici = await _korisnikService.GetAllKorisnik();
                UserComboBox.ItemsSource = korisnici;
                Debug.WriteLine($"Učitaj korisnike: {korisnici.Count()}");

                var agencije = await _agencijaService.GetAllAgencija();
                AgencyComboBox.ItemsSource = agencije;
                Debug.WriteLine($"Učitaj agencije: {agencije.Count()}");

                var apartmani = await _apartmanService.GetAllApartman();
                ApartmanComboBox.ItemsSource = apartmani;
                Debug.WriteLine($"Učitaj apartmane: {apartmani.Count()}");

                var usluge = await _uslugaService.GetAllUsluga();
                ServicesComboBox.ItemsSource = usluge;
                Debug.WriteLine($"Učitaj usluge: {apartmani.Count()}");

                // Sada kada su podaci učitani, postavi selektovane vrednosti
                UserComboBox.SelectedValue = SelectedReservation.korisnik?.korisnikId;
                AgencyComboBox.SelectedValue = SelectedReservation.agencija?.agencijaId;
                ApartmanComboBox.SelectedValue = SelectedReservation.apartman?.apartmanId;

                var rezervacijaUsluge = await _rezervacijaUslugaService.GetUslugeByRezervacijaId(SelectedReservation.rezervacijaId);
                var sveUsluge = (List<Usluga>)ServicesComboBox.ItemsSource;

                var selektovaneUsluge = sveUsluge
                    .Where(usluga => rezervacijaUsluge.Any(ru => ru.uslugaId == usluga.uslugaId))
                    .ToList();

                // Postavljamo selektovane elemente
                foreach (var usluga in selektovaneUsluge)
                {
                    ServicesComboBox.SelectedItems.Add(usluga);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju podataka: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /*private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedReservation.pocetniDatum = StartDatePicker.SelectedDate ?? SelectedReservation.pocetniDatum;
            SelectedReservation.krajnjiDatum = EndDatePicker.SelectedDate ?? SelectedReservation.krajnjiDatum;
            SelectedReservation.brojGostiju = (int)(GuestCountComboBox.SelectedItem ?? SelectedReservation.brojGostiju);
            SelectedReservation.cenaKonacna = double.TryParse(PriceTextBox.Text, out double cenaKonacna) ? cenaKonacna : SelectedReservation.cenaKonacna;
            SelectedReservation.nacinPlacanja = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            SelectedReservation.iznosProvizije = double.TryParse(CommissionAmountTextBox.Text, out double iznosProvizije) ? iznosProvizije : SelectedReservation.iznosProvizije;
            SelectedReservation.placeno = PaidCheckBox.IsChecked ?? SelectedReservation.placeno;
            SelectedReservation.komentar = CommentTextBox.Text ?? SelectedReservation.komentar;

            // Postavljanje selektovanog korisnika i agencije
            SelectedReservation.korisnik = UserComboBox.SelectedItem as Korisnik;
            SelectedReservation.agencija = AgencyComboBox.SelectedItem as Agencija;
            SelectedReservation.apartman = ApartmanComboBox.SelectedItem as Apartman;

            // Prvo, pronalazimo sve postojeće RezervacijaUsluga torke povezane sa ovom rezervacijom
            var postojeceRezervacijaUsluge = await _rezervacijaUslugaService.GetRezervacijaUslugaByRezervacijaId(SelectedReservation.rezervacijaId);

            // Selektovane usluge
            var selectedServices = ServicesComboBox.SelectedItems.Cast<Usluga>().ToList();

            // Brišemo usluge koje više nisu selektovane
            foreach (var rezervacijaUsluga in postojeceRezervacijaUsluge)
            {
                if (!selectedServices.Any(s => s.uslugaId == rezervacijaUsluga.uslugaId))
                {
                    // Brišemo torku iz RezervacijaUsluga prema rezervacijaUslugaId
                    await _rezervacijaUslugaService.DeleteRezervacijaUsluga(rezervacijaUsluga.rezervacijaUslugaId);
                }
            }

            // Dodajemo nove selektovane usluge koje nisu već povezane sa rezervacijom
            foreach (var service in selectedServices)
            {
                // Ako usluga nije već povezana sa rezervacijom, dodajemo je
                if (!postojeceRezervacijaUsluge.Any(r => r.uslugaId == service.uslugaId))
                {
                    var rezervacijaUsluga = new RezervacijaUsluga
                    {
                        rezervacijaId = SelectedReservation.rezervacijaId,
                        uslugaId = service.uslugaId,
                        kolicina = 0, // Postavi količinu, možeš promeniti vrednost ako je potrebno
                        datum = DateTime.Now
                    };

                    // Dodajemo novu uslugu u RezervacijaUsluga
                    await _rezervacijaUslugaService.AddRezervacijaUsluga(rezervacijaUsluga);
                }
            }

            try
            {
                await _rezervacijaService.UpdateRezervacija(SelectedReservation);
                MessageBox.Show("Rezervacija je uspešno ažurirana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);

                _reloadDataAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            NavigationService.GoBack();
        }*/
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Prvo, pripremimo vrednosti koje bi korisnik hteo da upiše
            var noviPocetniDatum = StartDatePicker.SelectedDate ?? SelectedReservation.pocetniDatum;
            var noviKrajnjiDatum = EndDatePicker.SelectedDate ?? SelectedReservation.krajnjiDatum;
            var noviApartman = ApartmanComboBox.SelectedItem as Apartman ?? SelectedReservation.apartman;

            // Provera da li novi period zauzima apartman
            var rezervacijeZaApartman = await _rezervacijaService.GetRezervacijeByApartmanId(noviApartman.apartmanId);

            // Isključujemo trenutnu rezervaciju iz provere
            rezervacijeZaApartman = rezervacijeZaApartman.Where(r => r.rezervacijaId != SelectedReservation.rezervacijaId).ToList();

            bool zauzeto = rezervacijeZaApartman.Any(r =>
                (noviPocetniDatum < r.krajnjiDatum) && (noviKrajnjiDatum > r.pocetniDatum)
            );

            if (zauzeto)
            {
                MessageBox.Show("Apartman je zauzet u izabranom periodu.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Prekidamo dalje izvršavanje ako ima preklapanja
            }

            // Ako je sve u redu, sad ažuriramo podatke
            SelectedReservation.pocetniDatum = noviPocetniDatum;
            SelectedReservation.krajnjiDatum = noviKrajnjiDatum;
            SelectedReservation.brojGostiju = (int)(GuestCountComboBox.SelectedItem ?? SelectedReservation.brojGostiju);
            SelectedReservation.cenaKonacna = double.TryParse(PriceTextBox.Text, out double cenaKonacna) ? cenaKonacna : SelectedReservation.cenaKonacna;
            SelectedReservation.nacinPlacanja = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            SelectedReservation.iznosProvizije = double.TryParse(CommissionAmountTextBox.Text, out double iznosProvizije) ? iznosProvizije : SelectedReservation.iznosProvizije;
            SelectedReservation.placeno = PaidCheckBox.IsChecked ?? SelectedReservation.placeno;
            SelectedReservation.komentar = CommentTextBox.Text ?? SelectedReservation.komentar;

            SelectedReservation.korisnik = UserComboBox.SelectedItem as Korisnik;
            SelectedReservation.agencija = AgencyComboBox.SelectedItem as Agencija;
            SelectedReservation.apartman = noviApartman;

            try
            {
                // Ažuriranje usluga povezane sa rezervacijom
                var postojeceRezervacijaUsluge = await _rezervacijaUslugaService.GetRezervacijaUslugaByRezervacijaId(SelectedReservation.rezervacijaId);
                var selectedServices = ServicesComboBox.SelectedItems.Cast<Usluga>().ToList();

                // Brišemo usluge koje više nisu selektovane
                foreach (var rezervacijaUsluga in postojeceRezervacijaUsluge)
                {
                    if (!selectedServices.Any(s => s.uslugaId == rezervacijaUsluga.uslugaId))
                    {
                        await _rezervacijaUslugaService.DeleteRezervacijaUsluga(rezervacijaUsluga.rezervacijaUslugaId);
                    }
                }

                // Dodajemo nove usluge koje nisu bile povezane
                foreach (var service in selectedServices)
                {
                    if (!postojeceRezervacijaUsluge.Any(r => r.uslugaId == service.uslugaId))
                    {
                        var rezervacijaUsluga = new RezervacijaUsluga
                        {
                            rezervacijaId = SelectedReservation.rezervacijaId,
                            uslugaId = service.uslugaId,
                            kolicina = 0, // Ovde možeš postaviti default kolicinu
                            datum = DateTime.Now
                        };

                        await _rezervacijaUslugaService.AddRezervacijaUsluga(rezervacijaUsluga);
                    }
                }

                // Konačno ažuriramo rezervaciju u bazi
                await _rezervacijaService.UpdateRezervacija(SelectedReservation);

                MessageBox.Show("Rezervacija je uspešno ažurirana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);

                _reloadDataAction(); // Osvežavanje prikaza
                NavigationService.GoBack(); // Povratak nazad
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteReservationAsync();
        }
        private async Task DeleteReservationAsync()
        {
            // Prikazivanje potvrde za brisanje
            var result = MessageBox.Show("Da li ste sigurni da želite da obrišete ovu rezervaciju?",
                                         "Potvrda brisanja",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Pozivanje metode za brisanje rezervacije
                    await _rezervacijaService.DeleteRezervacija(SelectedReservation.rezervacijaId);
                    MessageBox.Show("Rezervacija je uspešno obrisana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Poziv za reload podataka nakon brisanja
                    _reloadDataAction();
                    NavigationService.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Došlo je do greške pri brisanju rezervacije: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Ako korisnik otkaže brisanje, jednostavno se vraća na prethodnu stranu
                NavigationService.GoBack();
            }
        }
        private void LoadGuestCountComboBox()
        {
            if (SelectedReservation.apartman != null)
            {
                int maxKapacitet = SelectedReservation.apartman.kapacitetDeca + SelectedReservation.apartman.kapacitetOdrasli;
                GuestCountComboBox.ItemsSource = Enumerable.Range(1, maxKapacitet); // Brojevi od 1 do maxKapacitet
                GuestCountComboBox.SelectedItem = SelectedReservation.brojGostiju; // Postavi trenutni broj gostiju
            }
        }
    }
}
