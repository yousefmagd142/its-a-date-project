using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace its_a_date_project.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BgStart = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BgEnd = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CardBg = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ink = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    InkSoft = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Accent = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccentDeep = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccentSoft = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Gold = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Border = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ShadowRgba = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ThemeId = table.Column<int>(type: "int", nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    DefaultLanguage = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invites_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DateSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InviteId = table.Column<int>(type: "int", nullable: false),
                    ChosenDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmailSent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DateSubmissions_Invites_InviteId",
                        column: x => x.InviteId,
                        principalTable: "Invites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InviteTexts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InviteId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InviteTexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InviteTexts_Invites_InviteId",
                        column: x => x.InviteId,
                        principalTable: "Invites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DateSubmissions_InviteId",
                table: "DateSubmissions",
                column: "InviteId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_Slug",
                table: "Invites",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invites_ThemeId",
                table: "Invites",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_InviteTexts_InviteId_Language_Key",
                table: "InviteTexts",
                columns: new[] { "InviteId", "Language", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminSettings");

            migrationBuilder.DropTable(
                name: "DateSubmissions");

            migrationBuilder.DropTable(
                name: "InviteTexts");

            migrationBuilder.DropTable(
                name: "Invites");

            migrationBuilder.DropTable(
                name: "Themes");
        }
    }
}
