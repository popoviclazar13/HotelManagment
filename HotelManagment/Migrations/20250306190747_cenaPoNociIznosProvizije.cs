using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagment.Migrations
{
    /// <inheritdoc />
    public partial class cenaPoNociIznosProvizije : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "iznosProvizije",
                table: "Rezervacije",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "cenaPoNoci",
                table: "CeneApartmana",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "iznosProvizije",
                table: "Rezervacije");

            migrationBuilder.DropColumn(
                name: "cenaPoNoci",
                table: "CeneApartmana");
        }
    }
}
