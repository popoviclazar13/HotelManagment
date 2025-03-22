using HotelManagment.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Agencija> Agencije { get; set; }
        public DbSet<Apartman> Apartmani { get; set; }
        public DbSet<ApartmanOprema> ApartmaniOprema { get; set; }
        public DbSet<ApartmanPopust> ApartmaniPopust { get; set; }
        public DbSet<CenaApartmana> CeneApartmana { get; set; }
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Oprema> Opreme { get; set; }
        public DbSet<Popust> Pupusti { get; set; }
        public DbSet<Rezervacija> Rezervacije { get; set; }
        public DbSet<RezervacijaUsluga> RezervacijeUsluga { get; set; }
        public DbSet<TipApartmana> TipoviApartmana { get; set; }
        public DbSet<Usluga> Usluge { get; set;}
        public DbSet<Zgrada> Zgrade { get; set; }
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer("Server=localhost;Database=VilaBojana;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }
}
