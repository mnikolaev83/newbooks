using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookDataProvider.Migrations
{
    public partial class book3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorFullName",
                table: "Books",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAddedToStore",
                table: "Books",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Books",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PagesAmnt",
                table: "Books",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "Books",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RunAmnt",
                table: "Books",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Target",
                table: "Books",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Books",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorFullName",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DateAddedToStore",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PagesAmnt",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "RunAmnt",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Target",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Books");
        }
    }
}
