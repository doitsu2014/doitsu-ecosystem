using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FileConversion.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MapperSourceText",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceText = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapperSourceText", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutputMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    XmlConfiguration = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    NumberOfHeader = table.Column<int>(type: "integer", nullable: false),
                    NumberOfFooter = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutputMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InputMapping",
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
                    table.PrimaryKey("PK_InputMapping", x => new { x.Key, x.InputType });
                    table.ForeignKey(
                        name: "FK_InputMapping_MapperSourceText_MapperSourceTextId",
                        column: x => x.MapperSourceTextId,
                        principalTable: "MapperSourceText",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InputMapping_MapperSourceTextId",
                table: "InputMapping",
                column: "MapperSourceTextId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InputMapping");

            migrationBuilder.DropTable(
                name: "OutputMapping");

            migrationBuilder.DropTable(
                name: "MapperSourceText");
        }
    }
}
