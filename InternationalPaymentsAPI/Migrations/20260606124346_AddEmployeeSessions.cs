using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternationalPaymentsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblEmployeeSession",
                columns: table => new
                {
                    employee_Session_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_Id = table.Column<int>(type: "int", nullable: false),
                    login_Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    logout_Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_Active = table.Column<bool>(type: "bit", nullable: false),
                    session_Token_Hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    expires_On = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblEmployeeSession", x => x.employee_Session_Id);
                    table.ForeignKey(
                        name: "FK_tblEmployeeSession_tblEmployee_employee_Id",
                        column: x => x.employee_Id,
                        principalTable: "tblEmployee",
                        principalColumn: "employee_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblEmployeeSession_employee_Id",
                table: "tblEmployeeSession",
                column: "employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tblEmployeeSession_session_Token_Hash",
                table: "tblEmployeeSession",
                column: "session_Token_Hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblEmployeeSession");
        }
    }
}
