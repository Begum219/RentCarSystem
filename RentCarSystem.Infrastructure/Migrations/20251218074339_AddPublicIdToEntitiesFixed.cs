using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentCarSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdToEntitiesFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "PaymentRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()"); 

            migrationBuilder.CreateIndex(
                name: "IX_Users_PublicId",
                table: "Users",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PublicId",
                table: "Reservations",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PublicId",
                table: "Payments",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
            name: "IX_PaymentRequests_PublicId",
            table: "PaymentRequests",
            column: "PublicId",
            unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_PublicId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_PublicId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PublicId",
                table: "Payments");

            migrationBuilder.DropIndex(
            name: "IX_PaymentRequests_PublicId",
            table: "PaymentRequests");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "PaymentRequests");
        }
    }
}
