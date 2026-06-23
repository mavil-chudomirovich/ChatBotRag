using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RagChatbot.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLecturerAndSubjectAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectAssignments");

            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastActiveDate", "LastQueryDate" },
                values: new object[] { new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 9, 8, 27, 13, 752, DateTimeKind.Utc).AddTicks(9548) });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LastActiveDate", "LastQueryDate" },
                values: new object[] { new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 9, 8, 27, 13, 753, DateTimeKind.Utc).AddTicks(429) });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "LastActiveDate", "LastQueryDate" },
                values: new object[] { new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 9, 8, 27, 13, 753, DateTimeKind.Utc).AddTicks(496) });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "LastActiveDate", "LastQueryDate" },
                values: new object[] { new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 9, 8, 27, 13, 753, DateTimeKind.Utc).AddTicks(539) });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 8, 27, 13, 752, DateTimeKind.Utc).AddTicks(9113));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectAssignments",
                columns: table => new
                {
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    LecturerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectAssignments", x => new { x.SubjectId, x.LecturerId });
                    table.ForeignKey(
                        name: "FK_SubjectAssignments_AppUsers_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectAssignments_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastActiveDate", "LastQueryDate" },
                values: new object[] { new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 5, 5, 22, 6, 538, DateTimeKind.Utc).AddTicks(1288) });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LastActiveDate", "LastQueryDate" },
                values: new object[] { new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 5, 5, 22, 6, 538, DateTimeKind.Utc).AddTicks(2108) });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "LastActiveDate", "LastQueryDate" },
                values: new object[] { new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 5, 5, 22, 6, 538, DateTimeKind.Utc).AddTicks(2139) });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "LastActiveDate", "LastQueryDate" },
                values: new object[] { new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 5, 5, 22, 6, 538, DateTimeKind.Utc).AddTicks(2154) });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "DailyQueryCount", "DepartmentId", "Email", "FirstName", "IsActive", "LastActiveDate", "LastName", "LastQueryDate", "PasswordHash", "Role", "Subscription", "TodayChatCount" },
                values: new object[] { 2, 0, 1, "lecturer@gmail.com", "Nguyễn", true, new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Giảng Viên 1", new DateTime(2026, 6, 5, 5, 22, 6, 538, DateTimeKind.Utc).AddTicks(2059), "Yz9PJlOwHiN+8KJrW6mbQYyJTl9BLR121umofM8/fNg=", "Lecturer", 0, 0 });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 5, 5, 22, 6, 538, DateTimeKind.Utc).AddTicks(983));

            migrationBuilder.CreateIndex(
                name: "IX_SubjectAssignments_LecturerId",
                table: "SubjectAssignments",
                column: "LecturerId");
        }
    }
}
