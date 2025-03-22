using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagment.Migrations
{
    /// <inheritdoc />
    public partial class nacinPlacanja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nacinPlacanja",
                table: "Rezervacije",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nacinPlacanja",
                table: "Rezervacije");
        }
    }
}
