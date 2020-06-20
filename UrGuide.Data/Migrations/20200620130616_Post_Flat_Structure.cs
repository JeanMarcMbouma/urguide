using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class Post_Flat_Structure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AllocatedSeats",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "BidCount",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "BidEnabled",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Cost",
                schema: "ug",
                table: "Posts",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GeoLocation",
                schema: "ug",
                table: "Posts",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItineraryCount",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LastBid",
                schema: "ug",
                table: "Posts",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "ReservedSeats",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Reviews",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "ug",
                table: "Posts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                schema: "ug",
                table: "Posts",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllocatedSeats",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "BidCount",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "BidEnabled",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Cost",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Dislikes",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "GeoLocation",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ItineraryCount",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LastBid",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Likes",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Rating",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ReservedSeats",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Reviews",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "ug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Tags",
                schema: "ug",
                table: "Posts");
        }
    }
}
