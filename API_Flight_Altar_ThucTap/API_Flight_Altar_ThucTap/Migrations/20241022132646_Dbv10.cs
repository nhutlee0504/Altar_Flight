using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Flight_Altar_ThucTap.Migrations
{
    public partial class Dbv10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "documents");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "documents",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_documents_typeDocs_FlightId",
                table: "documents",
                column: "FlightId",
                principalTable: "typeDocs",
                principalColumn: "IdTypeDoc",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_documents_typeDocs_FlightId",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "documents");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "documents",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
