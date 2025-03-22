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
    /// Interaction logic for PopustEditWindow.xaml
    /// </summary>
    public partial class PopustEditWindow : Page
    {
        private readonly IPopustService _popustService;
        private dynamic _selectedDiscount;
        private readonly Action _reloadDataAction;

        public PopustEditWindow(IPopustService popustService, Popust selectedDiscount, Action reloadDataAction)
        {
            InitializeComponent();
            _selectedDiscount = selectedDiscount;
            _popustService = popustService;
            _reloadDataAction = reloadDataAction;

            // Popunjavanje polja sa postojećim podacima
            PocetniDatumPicker.SelectedDate = selectedDiscount.pocetniDatum;
            KrajnjiDatumPicker.SelectedDate = selectedDiscount.krajnjiDatum;
            IznosPopustaBox.Text = selectedDiscount.vrednost.ToString();
            NazivPopustaBox.Text = selectedDiscount.nazivPopusta;
            
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Ažuriranje podataka
            _selectedDiscount.pocetniDatum = PocetniDatumPicker.SelectedDate ?? _selectedDiscount.pocetniDatum;
            _selectedDiscount.krajnjiDatum = KrajnjiDatumPicker.SelectedDate ?? _selectedDiscount.krajnjiDatum;
            _selectedDiscount.vrednost = double.TryParse(IznosPopustaBox.Text, out double iznosPopusta) ? iznosPopusta : _selectedDiscount.vrednost;
            _selectedDiscount.nazivPopusta = NazivPopustaBox.Text;

            try
            {
                await _popustService.UpdatePopust(_selectedDiscount);
                MessageBox.Show("Popust uspešno ažuriran!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                _reloadDataAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom ažuriranja popusta: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Vraćanje na prethodnu stranicu
            NavigationService.GoBack();
        }
    }
}
