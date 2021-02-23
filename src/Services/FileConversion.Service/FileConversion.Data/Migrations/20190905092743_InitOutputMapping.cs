using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileConversion.Data.Migrations
{
    public partial class InitOutputMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MappingInformations");

            migrationBuilder.CreateTable(
                name: "OutputMapping",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false),
                    XmlConfiguration = table.Column<string>(maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutputMapping", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutputMapping");

            migrationBuilder.CreateTable(
                name: "MappingInformations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    StreamExportName = table.Column<string>(maxLength: 100, nullable: false),
                    StreamName = table.Column<string>(maxLength: 100, nullable: false),
                    TypeName = table.Column<string>(maxLength: 100, nullable: false),
                    XMLConfig = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingInformations", x => x.Id);
                });
        }
    }
}
