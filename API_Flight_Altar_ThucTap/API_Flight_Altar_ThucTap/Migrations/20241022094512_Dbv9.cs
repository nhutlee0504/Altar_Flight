using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Flight_Altar_ThucTap.Migrations
{
    public partial class Dbv9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "flights");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "flights",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
