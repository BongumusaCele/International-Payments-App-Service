using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternationalPaymentsAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                name: "tblEmployee",
                columns: table => new
                {
                    employee_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password_Hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    full_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblEmployee", x => x.employee_Id);
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
                    is_Active = table.Column<bool>(type: "bit", nullable: false),
                    session_Token_Hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    expires_On = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "tblMfaChallenge",
                columns: table => new
                {
                    mfa_Challenge_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_Id = table.Column<int>(type: "int", nullable: false),
                    otp_Code_Hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    created_On = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expires_On = table.Column<DateTime>(type: "datetime2", nullable: false),
                    consumed_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    attempt_Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMfaChallenge", x => x.mfa_Challenge_Id);
                    table.ForeignKey(
                        name: "FK_tblMfaChallenge_tblCustomer_customer_Id",
                        column: x => x.customer_Id,
                        principalTable: "tblCustomer",
                        principalColumn: "customer_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblPayment",
                columns: table => new
                {
                    payment_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_Id = table.Column<int>(type: "int", nullable: false),
                    beneficiary_Id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    swift_Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    created_On = table.Column<DateTime>(type: "datetime2", nullable: false),
                    verified_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    verified_By_Employee_Id = table.Column<int>(type: "int", nullable: true),
                    submitted_To_Swift_On = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPayment", x => x.payment_Id);
                    table.ForeignKey(
                        name: "FK_tblPayment_tblBeneficiary_beneficiary_Id",
                        column: x => x.beneficiary_Id,
                        principalTable: "tblBeneficiary",
                        principalColumn: "beneficiary_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblPayment_tblCustomer_customer_Id",
                        column: x => x.customer_Id,
                        principalTable: "tblCustomer",
                        principalColumn: "customer_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblPayment_tblEmployee_verified_By_Employee_Id",
                        column: x => x.verified_By_Employee_Id,
                        principalTable: "tblEmployee",
                        principalColumn: "employee_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblBeneficiary_customer_Id",
                table: "tblBeneficiary",
                column: "customer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomerSession_customer_Id",
                table: "tblCustomerSession",
                column: "customer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomerSession_session_Token_Hash",
                table: "tblCustomerSession",
                column: "session_Token_Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblEmployee_username",
                table: "tblEmployee",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblMfaChallenge_customer_Id",
                table: "tblMfaChallenge",
                column: "customer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tblPayment_beneficiary_Id",
                table: "tblPayment",
                column: "beneficiary_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tblPayment_customer_Id",
                table: "tblPayment",
                column: "customer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tblPayment_verified_By_Employee_Id",
                table: "tblPayment",
                column: "verified_By_Employee_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblCustomerSession");

            migrationBuilder.DropTable(
                name: "tblMfaChallenge");

            migrationBuilder.DropTable(
                name: "tblPayment");

            migrationBuilder.DropTable(
                name: "tblBeneficiary");

            migrationBuilder.DropTable(
                name: "tblEmployee");

            migrationBuilder.DropTable(
                name: "tblCustomer");
        }
    }
}
