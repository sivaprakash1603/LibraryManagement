using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryManagementDALLibrary.Migrations
{
    /// <inheritdoc />
    public partial class SeedMembershipTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "membershiptypes",
                columns: new[] { "id", "maximumborrowdays", "maximumborrowings", "name" },
                values: new object[,]
                {
                    { 1, 7, 2, "Basic" },
                    { 2, 10, 3, "Student" },
                    { 3, 15, 5, "Premium" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "membershiptypes",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "membershiptypes",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "membershiptypes",
                keyColumn: "id",
                keyValue: 3);
        }
    }
}
