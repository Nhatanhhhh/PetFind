using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PetStore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PetReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Species = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Breed = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    LostOrFoundDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAISearchEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentModerations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetReportId = table.Column<int>(type: "int", nullable: false),
                    ModeratorId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentModerations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentModerations_PetReports_PetReportId",
                        column: x => x.PetReportId,
                        principalTable: "PetReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentModerations_Users_ModeratorId",
                        column: x => x.ModeratorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentModerations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    PetReportId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_PetReports_PetReportId",
                        column: x => x.PetReportId,
                        principalTable: "PetReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 21, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(1764), "Quản trị viên hệ thống", "Admin" },
                    { 2, new DateTime(2025, 6, 21, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(1766), "Người dùng thông thường", "User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "AvatarUrl", "CreatedAt", "Email", "FirstName", "IsActive", "LastLoginAt", "LastName", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[,]
                {
                    { 1, "123 Lê Lợi, Q1, TP.HCM", "/images/avatars/admin.jpg", new DateTime(2025, 6, 21, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2201), "admin@petstore.com", "Admin", true, null, "System", "6G94qKPK8LYNjnTllCqm2G3BUM08AzOK7yW30tfjrMc=", "0901234567", "admin@petstore.com" },
                    { 2, "789 Lý Tự Trọng, Q1, TP.HCM", "/images/avatars/user.jpg", new DateTime(2025, 6, 21, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2204), "user@petstore.com", "Nguyễn", true, null, "Văn A", "PnwZV2SIhigW8TtRLKzz5LqX3ZckPqC9airRZC2GunI=", "0901111222", "user@petstore.com" }
                });

            migrationBuilder.InsertData(
                table: "PetReports",
                columns: new[] { "Id", "Address", "Breed", "City", "Color", "ContactEmail", "ContactName", "ContactNote", "ContactPhone", "CreatedAt", "Description", "District", "ImageUrl", "IsAISearchEnabled", "Latitude", "Longitude", "LostOrFoundDate", "PetName", "Species", "Status", "Title", "Type", "UserId" },
                values: new object[,]
                {
                    { 1, null, "Golden Retriever", "TP.HCM", "Vàng", "user@petstore.com", "Nguyễn Văn A", null, "0901111222", new DateTime(2025, 6, 19, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2261), "Chó nhà tôi bị lạc ở công viên 23/9. Có đeo vòng cổ màu đỏ, thân thiện với người.", "Quận 1", "/images/pets/dog1.jpg", true, 10.776899999999999, 106.7009, new DateTime(2025, 6, 19, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2252), "Max", "Dog", "Searching", "Mất chó Golden màu vàng", "Lost", 2 },
                    { 2, null, "Mèo ta", "TP.HCM", "Đen trắng", "user@petstore.com", "Nguyễn Văn A", null, "0901111222", new DateTime(2025, 6, 20, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2266), "Nhặt được mèo khoảng 3 tháng tuổi ở khu vực chợ Bến Thành. Rất ngoan và sạch sẽ.", "Quận 1", "/images/pets/cat1.jpg", false, 10.7723, 106.69799999999999, new DateTime(2025, 6, 20, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2264), null, "Cat", "Pending", "Nhặt được mèo đen trắng", "Found", 2 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "ContentModerations",
                columns: new[] { "Id", "CreatedAt", "ModeratorId", "PetReportId", "RejectionReason", "ReviewedAt", "Status", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 20, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2287), 1, 1, null, new DateTime(2025, 6, 20, 19, 20, 46, 29, DateTimeKind.Utc).AddTicks(2288), "Approved", null, null },
                    { 2, new DateTime(2025, 6, 21, 1, 20, 46, 29, DateTimeKind.Utc).AddTicks(2292), 1, 2, null, null, "Pending", null, null }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "Content", "CreatedAt", "IsRead", "PetReportId", "ReceiverId", "SenderId" },
                values: new object[] { 1, "Xin chào, báo cáo của bạn về chú chó Max đã được duyệt.", new DateTime(2025, 6, 21, 2, 20, 46, 29, DateTimeKind.Utc).AddTicks(2309), true, 1, 2, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_ContentModerations_ModeratorId",
                table: "ContentModerations",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentModerations_PetReportId",
                table: "ContentModerations",
                column: "PetReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentModerations_UserId",
                table: "ContentModerations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_PetReportId",
                table: "Messages",
                column: "PetReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PetReports_UserId",
                table: "PetReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentModerations");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "PetReports");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
