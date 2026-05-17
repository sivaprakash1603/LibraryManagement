using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LibraryManagementDALLibrary.Migrations
{
    /// <inheritdoc />
    public partial class inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bookcategories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bookcategories_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "membershiptypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    maximumborrowings = table.Column<int>(type: "integer", nullable: false),
                    maximumborrowdays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("membershiptypes_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    isbn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    authorname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    edition = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    categoryid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("books_pkey", x => x.isbn);
                    table.ForeignKey(
                        name: "fk_books_category",
                        column: x => x.categoryid,
                        principalTable: "bookcategories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    accountstatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'Active'::character varying"),
                    membershiptypeid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("members_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_members_membershiptype",
                        column: x => x.membershiptypeid,
                        principalTable: "membershiptypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bookcopies",
                columns: table => new
                {
                    barcodeno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    isbn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'Available'::character varying")
                },
                constraints: table =>
                {
                    table.PrimaryKey("bookcopies_pkey", x => x.barcodeno);
                    table.ForeignKey(
                        name: "fk_bookcopies_book",
                        column: x => x.isbn,
                        principalTable: "books",
                        principalColumn: "isbn",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "borrowings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    memberid = table.Column<int>(type: "integer", nullable: false),
                    barcodeno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    borrowdate = table.Column<DateOnly>(type: "date", nullable: false),
                    duedate = table.Column<DateOnly>(type: "date", nullable: false),
                    returndate = table.Column<DateOnly>(type: "date", nullable: true),
                    borrowstatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'Borrowed'::character varying")
                },
                constraints: table =>
                {
                    table.PrimaryKey("borrowings_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_borrowings_bookcopy",
                        column: x => x.barcodeno,
                        principalTable: "bookcopies",
                        principalColumn: "barcodeno",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_borrowings_member",
                        column: x => x.memberid,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fines",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    borrowingid = table.Column<int>(type: "integer", nullable: false),
                    finetype = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fineamount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ispaid = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("fines_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_fines_borrowing",
                        column: x => x.borrowingid,
                        principalTable: "borrowings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "finepayments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fineid = table.Column<int>(type: "integer", nullable: false),
                    amountpaid = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    paymentdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("finepayments_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_finepayments_fine",
                        column: x => x.fineid,
                        principalTable: "fines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "bookcategories_name_key",
                table: "bookcategories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_bookcopies_status",
                table: "bookcopies",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_bookcopies_isbn",
                table: "bookcopies",
                column: "isbn");

            migrationBuilder.CreateIndex(
                name: "idx_books_author",
                table: "books",
                column: "authorname");

            migrationBuilder.CreateIndex(
                name: "idx_books_title",
                table: "books",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "IX_books_categoryid",
                table: "books",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "idx_borrowings_member",
                table: "borrowings",
                column: "memberid");

            migrationBuilder.CreateIndex(
                name: "IX_borrowings_barcodeno",
                table: "borrowings",
                column: "barcodeno");

            migrationBuilder.CreateIndex(
                name: "IX_finepayments_fineid",
                table: "finepayments",
                column: "fineid");

            migrationBuilder.CreateIndex(
                name: "idx_fines_borrowing",
                table: "fines",
                column: "borrowingid");

            migrationBuilder.CreateIndex(
                name: "idx_members_email",
                table: "members",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "idx_members_phone",
                table: "members",
                column: "phone");

            migrationBuilder.CreateIndex(
                name: "IX_members_membershiptypeid",
                table: "members",
                column: "membershiptypeid");

            migrationBuilder.CreateIndex(
                name: "members_email_key",
                table: "members",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "members_phone_key",
                table: "members",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "membershiptypes_name_key",
                table: "membershiptypes",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "finepayments");

            migrationBuilder.DropTable(
                name: "fines");

            migrationBuilder.DropTable(
                name: "borrowings");

            migrationBuilder.DropTable(
                name: "bookcopies");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "books");

            migrationBuilder.DropTable(
                name: "membershiptypes");

            migrationBuilder.DropTable(
                name: "bookcategories");
        }
    }
}
