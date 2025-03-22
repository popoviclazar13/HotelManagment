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
        public Rezervacija SelectedReservation { get; set; }
        private readonly Action _reloadDataAction;
        public RezervacijaEditWindow(IRezervacijaService rezervacijaService, Rezervacija selectedReservation, Action reloadDataAction, IKorisnikService korisnikService, IAgencijaService agencijaService)
        {
            InitializeComponent();
            _rezervacijaService = rezervacijaService;
            _korisnikService = korisnikService;
            _agencijaService = agencijaService;
            SelectedReservation = selectedReservation;
            _reloadDataAction = reloadDataAction;
            LoadComboBoxData();
            LoadGuestCountComboBox();

            // Postavljanje početnih vrednosti
            StartDatePicker.SelectedDate = selectedReservation.pocetniDatum;
            EndDatePicker.SelectedDate = selectedReservation.krajnjiDatum;
            GuestCountComboBox.SelectedItem = selectedReservation.brojGostiju;
            FinalPriceTextBox.Text = selectedReservation.cenaKonacna.ToString("F2");
            PaymentMethodComboBox.SelectedItem = PaymentMethodComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == selectedReservation.nacinPlacanja);
            CommissionAmountTextBox.Text = selectedReservation.iznosProvizije.ToString("F2");
            PaidCheckBox.IsChecked = selectedReservation.placeno;

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

                // Sada kada su podaci učitani, postavi selektovane vrednosti
                UserComboBox.SelectedValue = SelectedReservation.korisnik?.korisnikId;
                AgencyComboBox.SelectedValue = SelectedReservation.agencija?.agencijaId;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju podataka: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedReservation.pocetniDatum = StartDatePicker.SelectedDate ?? SelectedReservation.pocetniDatum;
            SelectedReservation.krajnjiDatum = EndDatePicker.SelectedDate ?? SelectedReservation.krajnjiDatum;
            SelectedReservation.brojGostiju = (int)(GuestCountComboBox.SelectedItem ?? SelectedReservation.brojGostiju);
            SelectedReservation.cenaKonacna = double.TryParse(FinalPriceTextBox.Text, out double cenaKonacna) ? cenaKonacna : SelectedReservation.cenaKonacna;
            SelectedReservation.nacinPlacanja = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            SelectedReservation.iznosProvizije = double.TryParse(CommissionAmountTextBox.Text, out double iznosProvizije) ? iznosProvizije : SelectedReservation.iznosProvizije;
            SelectedReservation.placeno = PaidCheckBox.IsChecked ?? SelectedReservation.placeno;

            // Postavljanje selektovanog korisnika i agencije
            SelectedReservation.korisnik = UserComboBox.SelectedItem as Korisnik;
            SelectedReservation.agencija = AgencyComboBox.SelectedItem as Agencija;

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
