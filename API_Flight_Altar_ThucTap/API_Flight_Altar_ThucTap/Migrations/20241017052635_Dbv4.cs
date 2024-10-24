using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Flight_Altar_ThucTap.Migrations
{
    public partial class Dbv4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_group_Types_permissions_IdPermission",
                table: "group_Types");

            migrationBuilder.DropForeignKey(
                name: "FK_group_Users_users_UserID",
                table: "group_Users");

            migrationBuilder.DropForeignKey(
                name: "FK_groups_users_UserId",
                table: "groups");

            migrationBuilder.DropForeignKey(
                name: "FK_typeDocs_users_UserId",
                table: "typeDocs");

            migrationBuilder.CreateTable(
                name: "flights",
                columns: table => new
                {
                    IdFlight = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PointOfLoading = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PointOfUnloading = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStart = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeEnd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flights", x => x.IdFlight);
                    table.ForeignKey(
                        name: "FK_flights_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    IdDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<double>(type: "float", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FlightId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.IdDocument);
                    table.ForeignKey(
                        name: "FK_documents_flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "flights",
                        principalColumn: "IdFlight",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_documents_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_documents_FlightId",
                table: "documents",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_documents_UserId",
                table: "documents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_flights_UserId",
                table: "flights",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_group_Types_permissions_IdPermission",
                table: "group_Types",
                column: "IdPermission",
                principalTable: "permissions",
                principalColumn: "idPermission",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_group_Users_users_UserID",
                table: "group_Users",
                column: "UserID",
                principalTable: "users",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_groups_users_UserId",
                table: "groups",
                column: "UserId",
                principalTable: "users",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_typeDocs_users_UserId",
                table: "typeDocs",
                column: "UserId",
                principalTable: "users",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_group_Types_permissions_IdPermission",
                table: "group_Types");

            migrationBuilder.DropForeignKey(
                name: "FK_group_Users_users_UserID",
                table: "group_Users");

            migrationBuilder.DropForeignKey(
                name: "FK_groups_users_UserId",
                table: "groups");

            migrationBuilder.DropForeignKey(
                name: "FK_typeDocs_users_UserId",
                table: "typeDocs");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "flights");

            migrationBuilder.AddForeignKey(
                name: "FK_group_Types_permissions_IdPermission",
                table: "group_Types",
                column: "IdPermission",
                principalTable: "permissions",
                principalColumn: "idPermission",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_group_Users_users_UserID",
                table: "group_Users",
                column: "UserID",
                principalTable: "users",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_groups_users_UserId",
                table: "groups",
                column: "UserId",
                principalTable: "users",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_typeDocs_users_UserId",
                table: "typeDocs",
                column: "UserId",
                principalTable: "users",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
