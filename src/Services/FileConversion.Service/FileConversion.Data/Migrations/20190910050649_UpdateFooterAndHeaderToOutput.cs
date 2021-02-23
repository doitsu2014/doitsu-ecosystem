using Microsoft.EntityFrameworkCore.Migrations;

namespace FileConversion.Data.Migrations
{
    public partial class UpdateFooterAndHeaderToOutput : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfFooter",
                table: "OutputMapping",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfHeader",
                table: "OutputMapping",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfFooter",
                table: "OutputMapping");

            migrationBuilder.DropColumn(
                name: "NumberOfHeader",
                table: "OutputMapping");
        }
    }
}
