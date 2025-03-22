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
    /// Interaction logic for AllPopustPage.xaml
    /// </summary>
    public partial class AllPopustPage : Page
    {
        private readonly IPopustService _popustService;

        public AllPopustPage(IPopustService popustService)
        {
            InitializeComponent();
            _popustService = popustService;
            LoadPopusti();
        }
        private async void LoadPopusti()
        {
            try
            {
                var popusti = await _popustService.GetAllPopust();

                // Check if the list is not null or empty
                if (popusti != null && popusti.Any())
                {
                    PopustiDataGrid.ItemsSource = popusti;
                }
                else
                {
                    // Handle empty or null data if necessary
                    PopustiDataGrid.ItemsSource = new List<Popust>(); // Empty list to avoid blank rows
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom učitavanja popusta: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopustiDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PopustiDataGrid.SelectedItem is Popust selectedPopust && selectedPopust.popustId > 0)
            {
                // Ako je potrebno da napravite novi prozor za editovanje popusta
                // Ovdje možete pozvati novu stranicu ili prozor za editovanje popusta
                Action reloadDataAction = LoadPopusti; // Reload funkcionalnost nakon promjena
                NavigationService.Navigate(new PopustEditWindow(_popustService, selectedPopust, reloadDataAction));
                PopustiDataGrid.SelectedItem = null; // Resetovanje selekcije
            }
        }
    }
}
