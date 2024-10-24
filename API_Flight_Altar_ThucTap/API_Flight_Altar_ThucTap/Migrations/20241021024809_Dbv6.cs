using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Flight_Altar_ThucTap.Migrations
{
    public partial class Dbv6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "flights",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "documents",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "flights");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "documents");
        }
    }
}
