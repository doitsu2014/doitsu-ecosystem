using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileConversion.Data.Migrations
{
    public partial class InitFileConversion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InputMapping",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 50, nullable: false),
                    InputType = table.Column<int>(nullable: false),
                    XmlConfiguration = table.Column<string>(maxLength: 4000, nullable: true),
                    StreamType = table.Column<int>(nullable: false),
                    Mapper = table.Column<string>(maxLength: 256, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputMapping", x => new { x.Key, x.InputType });
                });

            migrationBuilder.CreateTable(
                name: "MappingInformations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TypeName = table.Column<string>(maxLength: 100, nullable: false),
                    XMLConfig = table.Column<string>(nullable: false),
                    StreamName = table.Column<string>(maxLength: 100, nullable: false),
                    StreamExportName = table.Column<string>(maxLength: 100, nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingInformations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InputMapping");

            migrationBuilder.DropTable(
                name: "MappingInformations");
        }
    }
}
