using Microsoft.EntityFrameworkCore.Migrations;

namespace UrGuide.Data.Migrations
{
    public partial class new_author_tour_reviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Balance_regions_RegionId",
                table: "Balance");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_authors_AuthorId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_tours_TourId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_authors_Balance_BalanceId",
                schema: "ug",
                table: "authors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Balance",
                table: "Balance");

            migrationBuilder.RenameTable(
                name: "Review",
                newName: "reviews",
                newSchema: "ug");

            migrationBuilder.RenameTable(
                name: "Balance",
                newName: "author_balance",
                newSchema: "ug");

            migrationBuilder.RenameIndex(
                name: "IX_Review_TourId",
                schema: "ug",
                table: "reviews",
                newName: "IX_reviews_TourId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_AuthorId",
                schema: "ug",
                table: "reviews",
                newName: "IX_reviews_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Balance_RegionId",
                schema: "ug",
                table: "author_balance",
                newName: "IX_author_balance_RegionId");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "ug",
                table: "reviews",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                schema: "ug",
                table: "reviews",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReviewId",
                schema: "ug",
                table: "reviews",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "RegionId",
                schema: "ug",
                table: "author_balance",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Coins",
                schema: "ug",
                table: "author_balance",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "Bonus",
                schema: "ug",
                table: "author_balance",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "BalanceId",
                schema: "ug",
                table: "author_balance",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_reviews",
                schema: "ug",
                table: "reviews",
                column: "ReviewId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_author_balance",
                schema: "ug",
                table: "author_balance",
                column: "BalanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_author_balance_regions_RegionId",
                schema: "ug",
                table: "author_balance",
                column: "RegionId",
                principalSchema: "ug",
                principalTable: "regions",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_authors_author_balance_BalanceId",
                schema: "ug",
                table: "authors",
                column: "BalanceId",
                principalSchema: "ug",
                principalTable: "author_balance",
                principalColumn: "BalanceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_reviews_authors_AuthorId",
                schema: "ug",
                table: "reviews",
                column: "AuthorId",
                principalSchema: "ug",
                principalTable: "authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reviews_tours_TourId",
                schema: "ug",
                table: "reviews",
                column: "TourId",
                principalSchema: "ug",
                principalTable: "tours",
                principalColumn: "TourId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_author_balance_regions_RegionId",
                schema: "ug",
                table: "author_balance");

            migrationBuilder.DropForeignKey(
                name: "FK_authors_author_balance_BalanceId",
                schema: "ug",
                table: "authors");

            migrationBuilder.DropForeignKey(
                name: "FK_reviews_authors_AuthorId",
                schema: "ug",
                table: "reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_reviews_tours_TourId",
                schema: "ug",
                table: "reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reviews",
                schema: "ug",
                table: "reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_author_balance",
                schema: "ug",
                table: "author_balance");

            migrationBuilder.RenameTable(
                name: "reviews",
                schema: "ug",
                newName: "Review");

            migrationBuilder.RenameTable(
                name: "author_balance",
                schema: "ug",
                newName: "Balance");

            migrationBuilder.RenameIndex(
                name: "IX_reviews_TourId",
                table: "Review",
                newName: "IX_Review_TourId");

            migrationBuilder.RenameIndex(
                name: "IX_reviews_AuthorId",
                table: "Review",
                newName: "IX_Review_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_author_balance_RegionId",
                table: "Balance",
                newName: "IX_Balance_RegionId");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Review",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Review",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ReviewId",
                table: "Review",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.AlterColumn<string>(
                name: "RegionId",
                table: "Balance",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<decimal>(
                name: "Coins",
                table: "Balance",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "Bonus",
                table: "Balance",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "BalanceId",
                table: "Balance",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "ReviewId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Balance",
                table: "Balance",
                column: "BalanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Balance_regions_RegionId",
                table: "Balance",
                column: "RegionId",
                principalSchema: "ug",
                principalTable: "regions",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_authors_AuthorId",
                table: "Review",
                column: "AuthorId",
                principalSchema: "ug",
                principalTable: "authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_tours_TourId",
                table: "Review",
                column: "TourId",
                principalSchema: "ug",
                principalTable: "tours",
                principalColumn: "TourId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_authors_Balance_BalanceId",
                schema: "ug",
                table: "authors",
                column: "BalanceId",
                principalTable: "Balance",
                principalColumn: "BalanceId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
