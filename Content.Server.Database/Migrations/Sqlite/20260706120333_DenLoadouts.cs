using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class DenLoadouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "job_loadout",
                columns: table => new
                {
                    job_loadout_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    profile_id = table.Column<int>(type: "INTEGER", nullable: false),
                    job_name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    loadout_profiles = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_loadout", x => x.job_loadout_id);
                    table.ForeignKey(
                        name: "FK_job_loadout_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "loadout_category",
                columns: table => new
                {
                    loadout_category_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    profile_id = table.Column<int>(type: "INTEGER", nullable: false),
                    category_unique_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    priority = table.Column<int>(type: "INTEGER", nullable: false),
                    category_name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    category_color = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loadout_category", x => x.loadout_category_id);
                    table.ForeignKey(
                        name: "FK_loadout_category_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "loadout_profile",
                columns: table => new
                {
                    loadout_profile_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    loadout_category_id = table.Column<int>(type: "INTEGER", nullable: false),
                    loadout_unique_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    loadout_name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    priority = table.Column<int>(type: "INTEGER", nullable: false),
                    loadout_items = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loadout_profile", x => x.loadout_profile_id);
                    table.ForeignKey(
                        name: "FK_loadout_profile_loadout_category_loadout_category_id",
                        column: x => x.loadout_category_id,
                        principalTable: "loadout_category",
                        principalColumn: "loadout_category_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_job_loadout_profile_id",
                table: "job_loadout",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_loadout_category_profile_id",
                table: "loadout_category",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_loadout_profile_loadout_category_id",
                table: "loadout_profile",
                column: "loadout_category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_loadout");

            migrationBuilder.DropTable(
                name: "loadout_profile");

            migrationBuilder.DropTable(
                name: "loadout_category");
        }
    }
}
