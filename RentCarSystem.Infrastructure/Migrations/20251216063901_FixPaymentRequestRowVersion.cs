using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentCarSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPaymentRequestRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // STEP 1: Eski RowVersion kolonunu sil
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PaymentRequests");

            // STEP 2: Yeni RowVersion kolonunu ekle (rowversion/timestamp)
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PaymentRequests",
                type: "rowversion",
                rowVersion: true,
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: Yeni kolonu sil
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PaymentRequests");

            // Eski kolonu geri ekle
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PaymentRequests",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}