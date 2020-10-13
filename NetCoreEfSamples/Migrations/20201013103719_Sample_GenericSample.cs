using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCoreEfSamples.Migrations
{
    public partial class Sample_GenericSample : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "genericPropChildren",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropBaseValue = table.Column<string>(nullable: true),
                    PropChildValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genericPropChildren", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mainSamples",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    NavPropId = table.Column<int>(nullable: false),
                    AnyVal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mainSamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mainSamples_genericPropChildren_NavPropId",
                        column: x => x.NavPropId,
                        principalTable: "genericPropChildren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mainSamples_NavPropId",
                table: "mainSamples",
                column: "NavPropId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mainSamples");

            migrationBuilder.DropTable(
                name: "genericPropChildren");
        }
    }
}
