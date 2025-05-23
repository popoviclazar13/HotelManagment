﻿using HotelManagment.Entitys;
using HotelManagment.Repository;
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
    /// Interaction logic for ApartmaniDetailsPage.xaml
    /// </summary>
    public partial class ApartmaniDetailsPage : Page
    {
        private readonly IApartmanService _apartmanService;
        private readonly IPopustService _popustService;
        private readonly IRezervacijaService _rezervacijaService;
        private readonly IKorisnikService _korisnikService;
        private readonly IAgencijaService _agencijaService;
        private readonly ICenaApartmanaService _cenaApartmanaService;
        private readonly IRezervacijaUslugaService _rezervacijaUslugaService;
        private readonly IUslugaService _uslugaService;
        private readonly int _apartmanId;

        public ApartmaniDetailsPage(int apartmanId, IApartmanService apartmanService, IPopustService opustService, IRezervacijaService rezervacijaService, IKorisnikService korisnikService, IAgencijaService agencijaService, ICenaApartmanaService cenaApartmanaService, IRezervacijaUslugaService rezervacijaUslugaService, IUslugaService uslugaService)
        {
            InitializeComponent();
            _apartmanService = apartmanService;
            _popustService = opustService;
            _rezervacijaService = rezervacijaService;
            _korisnikService = korisnikService;
            _agencijaService = agencijaService;
            _cenaApartmanaService = cenaApartmanaService;
            _apartmanId = apartmanId;
            _rezervacijaUslugaService = rezervacijaUslugaService;
            _uslugaService = uslugaService;
            LoadApartmanDetails();           
        }

        private async void LoadApartmanDetails()
        {
            var apartman = await _apartmanService.GetByIdApartman(_apartmanId);

            // Postavljanje naziva apartmana
            ApartmentName.Text = apartman.nazivApartmana;

            // Postavljanje kapaciteta
            ApartmentCapacity.Text = $"Kapacitet: {apartman.kapacitetOdrasli + apartman.kapacitetDeca} osoba ({apartman.kapacitetOdrasli} odraslih, {apartman.kapacitetDeca} dece)";

            // Postavljanje liste opreme
            EquipmentList.ItemsSource = apartman.listaApartmanOprema.Select(o => $"{o.oprema.nazivOprema} (Količina: {o.kolicinaOpreme})").ToList();

            // Postavljanje cene za različite datume
            PriceList.ItemsSource = apartman.listaCeneApartmana.ToList();


            var discountList = apartman.listaApartmanPopust
                .Where(p => p.popust != null
                    && p.popust.popustId > 0
                    && !string.IsNullOrEmpty(p.popust.nazivPopusta)
                    && p.popust.vrednost > 0) // Eliminacija nevalidnih popusta
                .Select(p => p.popust)
                .ToList();

            foreach (var discount in discountList)
            {
                Debug.WriteLine($"Popust: {discount.nazivPopusta}, Vrednost: {discount.vrednost}");
            }

            //DiscountList.ItemsSource = discountList;
            // Postavljanje rezervacija
            
            var validReservations = apartman.listaRezervacija
                .Where(r => r.pocetniDatum != DateTime.MinValue
                    && r.krajnjiDatum != DateTime.MinValue
                    && r.brojGostiju > 0
                    && r.cenaKonacna > 0
                    && !string.IsNullOrEmpty(r.nacinPlacanja)
                    && r.iznosProvizije >= 0
                    && r.korisnik != null
                    && !string.IsNullOrEmpty(r.korisnik.imePrezime)) // Provera samo korisnika, agencija može biti null
                .ToList();

            ReservationList.ItemsSource = validReservations;

        }

        private void PriceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PriceList.SelectedItem is CenaApartmana selectedPrice && selectedPrice.cenaApartmanaId > 0)
            {
                Action reloadDataAction = LoadApartmanDetails;
                NavigationService.Navigate(new CenaApartmanaEditWindow(_cenaApartmanaService, selectedPrice, reloadDataAction));
                PriceList.SelectedItem = null; // Resetovanje selekcije
            }
        }
        /*private void DiscountList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DiscountList.SelectedItem is Popust selectedDiscount && selectedDiscount.popustId > 0)
            {
                Action reloadDataAction = LoadApartmanDetails;
                NavigationService.Navigate(new PopustEditWindow(_popustService, selectedDiscount, reloadDataAction));
                DiscountList.SelectedItem = null;
            }
        }*/
        /*
         * <!-- Popusti -->
                    <StackPanel Grid.Column="2" VerticalAlignment="Top" HorizontalAlignment="Center" Width="500">
                        <TextBlock Text="Popusti" FontSize="24" FontWeight="Bold" HorizontalAlignment="Center" Margin="0,20,0,10"/>
                        <ScrollViewer Height="120" VerticalScrollBarVisibility="Auto">
                            <DataGrid x:Name="DiscountList"
                              AutoGenerateColumns="False"
                              SelectionMode="Single"
                              SelectionChanged="DiscountList_SelectionChanged"
                              CanUserAddRows="False"
                              Width="370">
                                <DataGrid.Columns>
                                    <DataGridTextColumn Header="Naziv" Binding="{Binding nazivPopusta}" Width="100">
                                        <DataGridTextColumn.ElementStyle>
                                            <Style TargetType="TextBlock">
                                                <Setter Property="TextAlignment" Value="Center"/>
                                                <Setter Property="HorizontalAlignment" Value="Center"/>
                                            </Style>
                                        </DataGridTextColumn.ElementStyle>
                                    </DataGridTextColumn>
                                    <DataGridTextColumn Header="Vrednost" Binding="{Binding vrednost}" Width="90">
                                        <DataGridTextColumn.ElementStyle>
                                            <Style TargetType="TextBlock">
                                                <Setter Property="TextAlignment" Value="Center"/>
                                                <Setter Property="HorizontalAlignment" Value="Center"/>
                                            </Style>
                                        </DataGridTextColumn.ElementStyle>
                                    </DataGridTextColumn>
                                    <DataGridTextColumn Header="Datum Od" Binding="{Binding pocetniDatum, StringFormat={}{0:dd.MM.yyyy}}" Width="90">
                                        <DataGridTextColumn.ElementStyle>
                                            <Style TargetType="TextBlock">
                                                <Setter Property="TextAlignment" Value="Center"/>
                                                <Setter Property="HorizontalAlignment" Value="Center"/>
                                            </Style>
                                        </DataGridTextColumn.ElementStyle>
                                    </DataGridTextColumn>
                                    <DataGridTextColumn Header="Datum Do" Binding="{Binding krajnjiDatum, StringFormat={}{0:dd.MM.yyyy}}" Width="90">
                                        <DataGridTextColumn.ElementStyle>
                                            <Style TargetType="TextBlock">
                                                <Setter Property="TextAlignment" Value="Center"/>
                                                <Setter Property="HorizontalAlignment" Value="Center"/>
                                            </Style>
                                        </DataGridTextColumn.ElementStyle>
                                    </DataGridTextColumn>
                                </DataGrid.Columns>
                            </DataGrid>
                        </ScrollViewer>
                    </StackPanel>
         */
        private void ReservationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReservationList.SelectedItem is Rezervacija selectedReservation && selectedReservation.rezervacijaId > 0)
            {
                Action reloadDataAction = LoadApartmanDetails; // ili neka druga metoda za ponovno učitavanje podataka, ako je potrebno
                NavigationService.Navigate(new RezervacijaEditWindow(_rezervacijaService, selectedReservation, reloadDataAction, _korisnikService, _agencijaService, _apartmanService, _rezervacijaUslugaService, _uslugaService));
                ReservationList.SelectedItem = null; // Resetovanje selekcije
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
