using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RagChatbot.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteAndHodTerm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "HodTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    StartAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HodTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HodTerms_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HodTerms_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsDeleted", "LastQueryDate" },
                values: new object[] { false, new DateTime(2026, 6, 9, 8, 56, 1, 435, DateTimeKind.Utc).AddTicks(5419) });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsDeleted", "LastQueryDate" },
                values: new object[] { false, new DateTime(2026, 6, 9, 8, 56, 1, 435, DateTimeKind.Utc).AddTicks(5769) });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "IsDeleted", "LastQueryDate" },
                values: new object[] { false, new DateTime(2026, 6, 9, 8, 56, 1, 435, DateTimeKind.Utc).AddTicks(5784) });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "IsDeleted", "LastQueryDate" },
                values: new object[] { false, new DateTime(2026, 6, 9, 8, 56, 1, 435, DateTimeKind.Utc).AddTicks(5794) });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 8, 56, 1, 435, DateTimeKind.Utc).AddTicks(5316));

            migrationBuilder.CreateIndex(
                name: "IX_HodTerms_AppUserId",
                table: "HodTerms",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HodTerms_DepartmentId",
                table: "HodTerms",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HodTerms");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppUsers");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastQueryDate",
                value: new DateTime(2026, 6, 9, 8, 27, 13, 752, DateTimeKind.Utc).AddTicks(9548));

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "LastQueryDate",
                value: new DateTime(2026, 6, 9, 8, 27, 13, 753, DateTimeKind.Utc).AddTicks(429));

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastQueryDate",
                value: new DateTime(2026, 6, 9, 8, 27, 13, 753, DateTimeKind.Utc).AddTicks(496));

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 100,
                column: "LastQueryDate",
                value: new DateTime(2026, 6, 9, 8, 27, 13, 753, DateTimeKind.Utc).AddTicks(539));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 8, 27, 13, 752, DateTimeKind.Utc).AddTicks(9113));
        }
    }
}
