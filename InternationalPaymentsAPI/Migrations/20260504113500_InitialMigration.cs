using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternationalPaymentsAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblCustomer",
                columns: table => new
                {
                    customer_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    last_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    id_Number = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    email_Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    account_Number = table.Column<int>(type: "int", nullable: false),
                    preferred_Currency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_On = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomer", x => x.customer_Id);
                });

            migrationBuilder.CreateTable(
                name: "tblBeneficiary",
                columns: table => new
                {
                    beneficiary_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_Id = table.Column<int>(type: "int", nullable: false),
                    beneficiary_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    bank_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    account_Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    swift_Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    country = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBeneficiary", x => x.beneficiary_Id);
                    table.ForeignKey(
                        name: "FK_tblBeneficiary_tblCustomer_customer_Id",
                        column: x => x.customer_Id,
                        principalTable: "tblCustomer",
                        principalColumn: "customer_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblCustomerSession",
                columns: table => new
                {
                    session_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_Id = table.Column<int>(type: "int", nullable: false),
                    login_Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    logout_Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomerSession", x => x.session_Id);
                    table.ForeignKey(
                        name: "FK_tblCustomerSession_tblCustomer_customer_Id",
                        column: x => x.customer_Id,
                        principalTable: "tblCustomer",
                        principalColumn: "customer_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblBeneficiary_customer_Id",
                table: "tblBeneficiary",
                column: "customer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomerSession_customer_Id",
                table: "tblCustomerSession",
                column: "customer_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblBeneficiary");

            migrationBuilder.DropTable(
                name: "tblCustomerSession");

            migrationBuilder.DropTable(
                name: "tblCustomer");
        }
    }
}
