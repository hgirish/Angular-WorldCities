using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldCities.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name_Lat_Lon_CountryId",
                table: "Cities",
                columns: new[] { "Name", "Lat", "Lon", "CountryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cities_Name_Lat_Lon_CountryId",
                table: "Cities");
        }
    }
}
