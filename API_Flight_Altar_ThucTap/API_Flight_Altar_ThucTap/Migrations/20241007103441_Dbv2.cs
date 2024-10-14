using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Flight_Altar_ThucTap.Migrations
{
    public partial class Dbv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "group_Types",
                columns: table => new
                {
                    IdGT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdGroup = table.Column<int>(type: "int", nullable: false),
                    IdType = table.Column<int>(type: "int", nullable: false),
                    IdPermission = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_Types", x => x.IdGT);
                    table.ForeignKey(
                        name: "FK_group_Types_groups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "groups",
                        principalColumn: "IdGroup",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_group_Types_permissions_IdPermission",
                        column: x => x.IdPermission,
                        principalTable: "permissions",
                        principalColumn: "idPermission",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_group_Types_typeDocs_IdType",
                        column: x => x.IdType,
                        principalTable: "typeDocs",
                        principalColumn: "IdTypeDoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_group_Types_IdGroup",
                table: "group_Types",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_group_Types_IdPermission",
                table: "group_Types",
                column: "IdPermission");

            migrationBuilder.CreateIndex(
                name: "IX_group_Types_IdType",
                table: "group_Types",
                column: "IdType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_Types");
        }
    }
}
