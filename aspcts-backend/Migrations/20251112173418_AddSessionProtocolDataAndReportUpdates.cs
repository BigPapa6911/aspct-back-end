using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspcts_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionProtocolDataAndReportUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Children_ChildId",
                table: "Reports");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Reports",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "SessionProtocolData",
                columns: table => new
                {
                    SessionProtocolDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TotalTrials = table.Column<int>(type: "int", nullable: false),
                    AttentionCorrect = table.Column<int>(type: "int", nullable: false),
                    AttentionTotal = table.Column<int>(type: "int", nullable: false),
                    ImitationCorrect = table.Column<int>(type: "int", nullable: false),
                    ImitationTotal = table.Column<int>(type: "int", nullable: false),
                    ContactCorrect = table.Column<int>(type: "int", nullable: false),
                    ContactTotal = table.Column<int>(type: "int", nullable: false),
                    DeskActivitiesCorrect = table.Column<int>(type: "int", nullable: false),
                    DeskActivitiesTotal = table.Column<int>(type: "int", nullable: false),
                    IndependenceCorrect = table.Column<int>(type: "int", nullable: false),
                    IndependenceTotal = table.Column<int>(type: "int", nullable: false),
                    TimeRegistered = table.Column<int>(type: "int", nullable: false),
                    TimeTotal = table.Column<int>(type: "int", nullable: false),
                    ProtocolNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionProtocolData", x => x.SessionProtocolDataId);
                    table.ForeignKey(
                        name: "FK_SessionProtocolData_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "ReportId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SessionProtocolData_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionDate",
                table: "Sessions",
                column: "SessionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_Period",
                table: "Reports",
                columns: new[] { "StartPeriod", "EndPeriod" });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportDate",
                table: "Reports",
                column: "ReportDate");

            migrationBuilder.CreateIndex(
                name: "IX_SessionProtocolData_ReportId",
                table: "SessionProtocolData",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionProtocolData_SessionId",
                table: "SessionProtocolData",
                column: "SessionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Children_ChildId",
                table: "Reports",
                column: "ChildId",
                principalTable: "Children",
                principalColumn: "ChildId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Children_ChildId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "SessionProtocolData");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_SessionDate",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Reports_Period",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportDate",
                table: "Reports");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Reports",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Children_ChildId",
                table: "Reports",
                column: "ChildId",
                principalTable: "Children",
                principalColumn: "ChildId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
