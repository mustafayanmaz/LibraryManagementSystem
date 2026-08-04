using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBookReservationRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookReservations_UserId",
                table: "BookReservations");

            migrationBuilder.CreateIndex(
                name: "UX_BookReservations_User_Book_Active",
                table: "BookReservations",
                columns: new[] { "UserId", "BookId" },
                unique: true,
                filter: "\"Status\" IN ('Pending', 'Approved')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_BookReservations_User_Book_Active",
                table: "BookReservations");

            migrationBuilder.CreateIndex(
                name: "IX_BookReservations_UserId",
                table: "BookReservations",
                column: "UserId");
        }
    }
}
