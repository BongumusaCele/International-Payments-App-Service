using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternationalPaymentsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentRejectionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "rejected_By_Employee_Id",
                table: "tblPayment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "rejected_On",
                table: "tblPayment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rejection_Reason",
                table: "tblPayment",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rejected_By_Employee_Id",
                table: "tblPayment");

            migrationBuilder.DropColumn(
                name: "rejected_On",
                table: "tblPayment");

            migrationBuilder.DropColumn(
                name: "rejection_Reason",
                table: "tblPayment");
        }
    }
}
