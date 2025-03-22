using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagment.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agencije",
                columns: table => new
                {
                    agencijaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nazivAgencije = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencije", x => x.agencijaId);
                });

            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    korisnikId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    imePrezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    zemlja = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.korisnikId);
                });

            migrationBuilder.CreateTable(
                name: "Opreme",
                columns: table => new
                {
                    opremaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nazivOprema = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opreme", x => x.opremaId);
                });

            migrationBuilder.CreateTable(
                name: "Pupusti",
                columns: table => new
                {
                    popustId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nazivPopusta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vrednost = table.Column<double>(type: "float", nullable: false),
                    pocetniDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    krajnjiDatum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pupusti", x => x.popustId);
                });

            migrationBuilder.CreateTable(
                name: "TipoviApartmana",
                columns: table => new
                {
                    tipApartmanaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nazivTipaApartmana = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoviApartmana", x => x.tipApartmanaId);
                });

            migrationBuilder.CreateTable(
                name: "Usluge",
                columns: table => new
                {
                    uslugaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nazivUsluge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cenaUsluge = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usluge", x => x.uslugaId);
                });

            migrationBuilder.CreateTable(
                name: "Zgrade",
                columns: table => new
                {
                    zgradaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    naziv = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zgrade", x => x.zgradaId);
                });

            migrationBuilder.CreateTable(
                name: "Apartmani",
                columns: table => new
                {
                    apartmanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nazivApartmana = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    brojSprata = table.Column<int>(type: "int", nullable: false),
                    zauzet = table.Column<bool>(type: "bit", nullable: false),
                    kapacitetOdrasli = table.Column<int>(type: "int", nullable: false),
                    kapacitetDeca = table.Column<int>(type: "int", nullable: false),
                    zgradaId = table.Column<int>(type: "int", nullable: false),
                    tipApartmanaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartmani", x => x.apartmanId);
                    table.ForeignKey(
                        name: "FK_Apartmani_TipoviApartmana_tipApartmanaId",
                        column: x => x.tipApartmanaId,
                        principalTable: "TipoviApartmana",
                        principalColumn: "tipApartmanaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Apartmani_Zgrade_zgradaId",
                        column: x => x.zgradaId,
                        principalTable: "Zgrade",
                        principalColumn: "zgradaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApartmaniOprema",
                columns: table => new
                {
                    apartmanOpremaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kolicinaOpreme = table.Column<int>(type: "int", nullable: false),
                    apartmanId = table.Column<int>(type: "int", nullable: false),
                    opremaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmaniOprema", x => x.apartmanOpremaId);
                    table.ForeignKey(
                        name: "FK_ApartmaniOprema_Apartmani_apartmanId",
                        column: x => x.apartmanId,
                        principalTable: "Apartmani",
                        principalColumn: "apartmanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApartmaniOprema_Opreme_opremaId",
                        column: x => x.opremaId,
                        principalTable: "Opreme",
                        principalColumn: "opremaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApartmaniPopust",
                columns: table => new
                {
                    apartmanPopustId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    apartmanId = table.Column<int>(type: "int", nullable: false),
                    popustId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmaniPopust", x => x.apartmanPopustId);
                    table.ForeignKey(
                        name: "FK_ApartmaniPopust_Apartmani_apartmanId",
                        column: x => x.apartmanId,
                        principalTable: "Apartmani",
                        principalColumn: "apartmanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApartmaniPopust_Pupusti_popustId",
                        column: x => x.popustId,
                        principalTable: "Pupusti",
                        principalColumn: "popustId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CeneApartmana",
                columns: table => new
                {
                    cenaApartmanaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pocetniDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    krajnjiDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    cenaPoOsobi = table.Column<double>(type: "float", nullable: false),
                    apartmanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CeneApartmana", x => x.cenaApartmanaId);
                    table.ForeignKey(
                        name: "FK_CeneApartmana_Apartmani_apartmanId",
                        column: x => x.apartmanId,
                        principalTable: "Apartmani",
                        principalColumn: "apartmanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezervacije",
                columns: table => new
                {
                    rezervacijaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pocetniDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    krajnjiDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    brojGostiju = table.Column<int>(type: "int", nullable: false),
                    ukupnaCena = table.Column<double>(type: "float", nullable: false),
                    cenaKonacna = table.Column<double>(type: "float", nullable: false),
                    komentar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    placeno = table.Column<bool>(type: "bit", nullable: false),
                    apartmanId = table.Column<int>(type: "int", nullable: false),
                    korisnikId = table.Column<int>(type: "int", nullable: false),
                    agencijaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacije", x => x.rezervacijaId);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Agencije_agencijaId",
                        column: x => x.agencijaId,
                        principalTable: "Agencije",
                        principalColumn: "agencijaId");
                    table.ForeignKey(
                        name: "FK_Rezervacije_Apartmani_apartmanId",
                        column: x => x.apartmanId,
                        principalTable: "Apartmani",
                        principalColumn: "apartmanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Korisnici_korisnikId",
                        column: x => x.korisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "korisnikId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RezervacijeUsluga",
                columns: table => new
                {
                    rezervacijaUslugaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kolicina = table.Column<int>(type: "int", nullable: false),
                    datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    rezervacijaId = table.Column<int>(type: "int", nullable: false),
                    uslugaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RezervacijeUsluga", x => x.rezervacijaUslugaId);
                    table.ForeignKey(
                        name: "FK_RezervacijeUsluga_Rezervacije_rezervacijaId",
                        column: x => x.rezervacijaId,
                        principalTable: "Rezervacije",
                        principalColumn: "rezervacijaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RezervacijeUsluga_Usluge_uslugaId",
                        column: x => x.uslugaId,
                        principalTable: "Usluge",
                        principalColumn: "uslugaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartmani_tipApartmanaId",
                table: "Apartmani",
                column: "tipApartmanaId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartmani_zgradaId",
                table: "Apartmani",
                column: "zgradaId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmaniOprema_apartmanId",
                table: "ApartmaniOprema",
                column: "apartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmaniOprema_opremaId",
                table: "ApartmaniOprema",
                column: "opremaId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmaniPopust_apartmanId",
                table: "ApartmaniPopust",
                column: "apartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmaniPopust_popustId",
                table: "ApartmaniPopust",
                column: "popustId");

            migrationBuilder.CreateIndex(
                name: "IX_CeneApartmana_apartmanId",
                table: "CeneApartmana",
                column: "apartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_agencijaId",
                table: "Rezervacije",
                column: "agencijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_apartmanId",
                table: "Rezervacije",
                column: "apartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_korisnikId",
                table: "Rezervacije",
                column: "korisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_RezervacijeUsluga_rezervacijaId",
                table: "RezervacijeUsluga",
                column: "rezervacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_RezervacijeUsluga_uslugaId",
                table: "RezervacijeUsluga",
                column: "uslugaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApartmaniOprema");

            migrationBuilder.DropTable(
                name: "ApartmaniPopust");

            migrationBuilder.DropTable(
                name: "CeneApartmana");

            migrationBuilder.DropTable(
                name: "RezervacijeUsluga");

            migrationBuilder.DropTable(
                name: "Opreme");

            migrationBuilder.DropTable(
                name: "Pupusti");

            migrationBuilder.DropTable(
                name: "Rezervacije");

            migrationBuilder.DropTable(
                name: "Usluge");

            migrationBuilder.DropTable(
                name: "Agencije");

            migrationBuilder.DropTable(
                name: "Apartmani");

            migrationBuilder.DropTable(
                name: "Korisnici");

            migrationBuilder.DropTable(
                name: "TipoviApartmana");

            migrationBuilder.DropTable(
                name: "Zgrade");
        }
    }
}
