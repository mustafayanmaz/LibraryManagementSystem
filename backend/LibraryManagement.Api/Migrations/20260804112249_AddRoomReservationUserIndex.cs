using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomReservationUserIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomReservations_UserId",
                table: "RoomReservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RoomReservations_Time",
                table: "RoomReservations");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_User_Date_Time",
                table: "RoomReservations",
                columns: new[] { "UserId", "ReservationDate", "StartTime", "EndTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomReservations_User_Date_Time",
                table: "RoomReservations");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_UserId",
                table: "RoomReservations",
                column: "UserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RoomReservations_Time",
                table: "RoomReservations",
                sql: "\"EndTime\" > \"StartTime\"");
        }
    }
}
