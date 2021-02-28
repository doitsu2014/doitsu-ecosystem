using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FileConversion.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MapperSourceTexts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceText = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapperSourceTexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutputMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsXml = table.Column<bool>(type: "boolean", nullable: false),
                    XmlConfiguration = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    NumberOfHeader = table.Column<int>(type: "integer", nullable: true),
                    NumberOfFooter = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutputMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InputMappings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InputType = table.Column<int>(type: "integer", nullable: false),
                    XmlConfiguration = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    StreamType = table.Column<int>(type: "integer", nullable: false),
                    Mapper = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    MapperSourceTextId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputMappings", x => new { x.Key, x.InputType });
                    table.ForeignKey(
                        name: "FK_InputMappings_MapperSourceTexts_MapperSourceTextId",
                        column: x => x.MapperSourceTextId,
                        principalTable: "MapperSourceTexts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InputMappings_MapperSourceTextId",
                table: "InputMappings",
                column: "MapperSourceTextId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InputMappings");

            migrationBuilder.DropTable(
                name: "OutputMappings");

            migrationBuilder.DropTable(
                name: "MapperSourceTexts");
        }
    }
}
