using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FileConversion.Data.Migrations
{
    public partial class MappingSourceText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MapperSourceTextId",
                table: "InputMapping",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MapperSourceText",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SourceText = table.Column<string>(maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapperSourceText", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InputMapping_MapperSourceTextId",
                table: "InputMapping",
                column: "MapperSourceTextId");

            migrationBuilder.AddForeignKey(
                name: "FK_InputMapping_MapperSourceText_MapperSourceTextId",
                table: "InputMapping",
                column: "MapperSourceTextId",
                principalTable: "MapperSourceText",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InputMapping_MapperSourceText_MapperSourceTextId",
                table: "InputMapping");

            migrationBuilder.DropTable(
                name: "MapperSourceText");

            migrationBuilder.DropIndex(
                name: "IX_InputMapping_MapperSourceTextId",
                table: "InputMapping");

            migrationBuilder.DropColumn(
                name: "MapperSourceTextId",
                table: "InputMapping");
        }
    }
}
