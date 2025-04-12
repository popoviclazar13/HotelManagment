using HotelManagment.Data;
using HotelManagment.Entitys;
using HotelManagment.InterfaceRepository;
using HotelManagment.Pages;
using HotelManagment.Repository;
using HotelManagment.Service;
using HotelManagment.ServiceRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace HotelManagment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //U WPF aplikacijama, klasa App.xaml.cs se koristi kao Program.cs, u njoj je potrebno da imamo postavke za bazu i migracije!!!
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    context.Database.OpenConnection();
                    MessageBox.Show("Uspesno povezan na bazu!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Neuspelo povezivanje: {ex.Message}", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=localhost;Database=VilaBojana;Trusted_Connection=True;TrustServerCertificate=True;"));

            // Generički repozitorijum za sve entitete
            //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Repozitorijumi
            services.AddScoped<IRepository<Zgrada>, ZgradaRepository>();
            services.AddScoped<IRepository<Apartman>, ApartmanRepository>();
            services.AddScoped<IRepository<Agencija>, AgencijaRepository>();
            services.AddScoped<IRepository<ApartmanOprema>, ApartmanOpremaRepository>();
            services.AddScoped<IRepository<ApartmanPopust>, ApartmanPopustRepository>();
            services.AddScoped<IRepository<CenaApartmana>, CenaApartmanaRepository>();
            services.AddScoped<IRepository<Korisnik>, KorisnikRepository>();
            services.AddScoped<IRepository<Oprema>, OpremaRepository>();
            services.AddScoped<IRepository<Popust>, PopustRepository>();
            services.AddScoped<IRepository<Rezervacija>, RezervacijaRepository>();
            services.AddScoped<IRepository<RezervacijaUsluga>, RezervacijaUslugaRepository>();
            services.AddScoped<IRepository<TipApartmana>, TipApartmanaRepository>();
            services.AddScoped<IRepository<Usluga>, UslugaRepository>();

            // Servisi
            services.AddScoped<IZgradaService, ZgradaService>();
            services.AddScoped<IApartmanService, ApartmanService>();
            services.AddScoped<IApartmanOpremaService, ApartmanOpremaService>();
            services.AddScoped<IAgencijaService, AgencijaService>();
            services.AddScoped<IApartmanPopustService, ApartmanPopustService>();
            services.AddScoped<ICenaApartmanaService, CenaApartmanaService>();
            services.AddScoped<IKorisnikService, KorisnikService>();
            services.AddScoped<IOpremaService, OpremaService>();
            services.AddScoped<IPopustService, PopustService>();
            services.AddScoped<IRezervacijaService, RezervacijaService>();
            services.AddScoped<IRezervacijaUslugaService, RezervacijaUslugaService>();
            services.AddScoped<ITipApartmanaService, TipApartmanaService>();
            services.AddScoped<IUslugaService, UslugaService>();

            // Stranice
            services.AddScoped<ApartmaniPage>();
            services.AddScoped<HomePage>();
            services.AddScoped<LoginPage>();
        }
    }

}
