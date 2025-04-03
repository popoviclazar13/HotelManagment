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
    /// Interaction logic for AllOpremaPage.xaml
    /// </summary>
    public partial class AllOpremaPage : Page
    {
        private readonly IOpremaService _opremaService;

        public AllOpremaPage(IOpremaService opremaService)
        {
            InitializeComponent();
            _opremaService = opremaService;
            LoadOprema();
        }
        private async void LoadOprema()
        {
            try
            {
                var oprema = await _opremaService.GetAllOprema();
                OpremaDataGrid.ItemsSource = oprema;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom učitavanja opreme: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpremaDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OpremaDataGrid.SelectedItem is Oprema selectedOprema && selectedOprema.opremaId > 0)
            {
                // Ako je potrebno da napravite novi prozor za editovanje opreme
                // Ovdje možete pozvati novu stranicu ili prozor za editovanje opreme
                Action reloadDataAction = LoadOprema; // Reload funkcionalnost nakon promjena
                NavigationService.Navigate(new OpremaEditWindow(_opremaService, selectedOprema, reloadDataAction));
                OpremaDataGrid.SelectedItem = null; // Resetovanje selekcije
            }
        }
        private void AddOprema_Click(object sender, RoutedEventArgs e)
        {
            Action reloadDataAction = LoadOprema;
            NavigationService.Navigate(new OpremaAddWindow(_opremaService, reloadDataAction));
        }
    }
}
