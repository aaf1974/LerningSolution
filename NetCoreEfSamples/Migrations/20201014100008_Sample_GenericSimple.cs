using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCoreEfSamples.Migrations
{
    public partial class Sample_GenericSimple : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaseObjectGeoObjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseObjectGeoObjectTitle = table.Column<string>(nullable: true),
                    VTBBaseObjectGeoObjectTitle = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseObjectGeoObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseObjectRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseObjectRoleTitle = table.Column<string>(nullable: true),
                    VTBBaseObjectRoleTitle = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseObjectRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseObjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseObjectTitle = table.Column<string>(nullable: true),
                    BaseObjectRoleId = table.Column<int>(nullable: false),
                    BaseObjectGeoObjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseObjects_BaseObjectGeoObjects_BaseObjectGeoObjectId",
                        column: x => x.BaseObjectGeoObjectId,
                        principalTable: "BaseObjectGeoObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseObjects_BaseObjectRoles_BaseObjectRoleId",
                        column: x => x.BaseObjectRoleId,
                        principalTable: "BaseObjectRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseObjects_BaseObjectGeoObjectId",
                table: "BaseObjects",
                column: "BaseObjectGeoObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseObjects_BaseObjectRoleId",
                table: "BaseObjects",
                column: "BaseObjectRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseObjects");

            migrationBuilder.DropTable(
                name: "BaseObjectGeoObjects");

            migrationBuilder.DropTable(
                name: "BaseObjectRoles");
        }
    }
}
