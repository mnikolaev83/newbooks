using Microsoft.EntityFrameworkCore.Migrations;

namespace BookDataProvider.Migrations
{
    public partial class Ignore2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "IgnoreList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "IgnoreList");
        }
    }
}
