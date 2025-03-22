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
    /// Interaction logic for AllUslugePage.xaml
    /// </summary>
    public partial class AllUslugePage : Page
    {
        private readonly IUslugaService _uslugaService;

        // Constructor that accepts a service for fetching Usluga data
        public AllUslugePage(IUslugaService uslugaService)
        {
            InitializeComponent();
            _uslugaService = uslugaService;
            LoadUsluge();
        }
        // Metod za učitavanje usluga
        private async void LoadUsluge()
        {
            try
            {
                var usluge = await _uslugaService.GetAllUsluga(); // Pretpostavljamo da metoda vrati listu Usluga
                UslugeDataGrid.ItemsSource = usluge;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom učitavanja usluga: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Obrada selekcije reda u DataGrid-u
        private void UslugeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UslugeDataGrid.SelectedItem is Usluga selectedUsluga && selectedUsluga.uslugaId > 0)
            {
                // Ako je potrebno da napraviš prozor za editovanje ili detalje o usluzi
                // Ovde možeš pozvati novu stranicu ili prozor sa detaljima
                Action reloadDataAction = LoadUsluge; // Funkcija za ponovno učitavanje podataka
                NavigationService.Navigate(new UslugaEditWindow(_uslugaService, selectedUsluga, reloadDataAction));
                UslugeDataGrid.SelectedItem = null; // Resetovanje selekcije
            }
        }
    }
}
