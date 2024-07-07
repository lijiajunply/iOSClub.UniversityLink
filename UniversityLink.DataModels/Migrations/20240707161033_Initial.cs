using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityLink.DataModels.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(32)", nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", nullable: false),
                    Description = table.Column<string>(type: "varchar(32)", nullable: true),
                    Icon = table.Column<string>(type: "varchar(16)", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(32)", nullable: false),
                    UserName = table.Column<string>(type: "varchar(32)", nullable: false),
                    Gender = table.Column<string>(type: "varchar(2)", nullable: false),
                    ClassName = table.Column<string>(type: "varchar(20)", nullable: false),
                    Identity = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(32)", nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", nullable: false),
                    Icon = table.Column<string>(type: "varchar(16)", nullable: false),
                    Url = table.Column<string>(type: "varchar(64)", nullable: false),
                    Description = table.Column<string>(type: "varchar(32)", nullable: true),
                    CategoryModelKey = table.Column<string>(type: "varchar(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Links_Categories_CategoryModelKey",
                        column: x => x.CategoryModelKey,
                        principalTable: "Categories",
                        principalColumn: "Key");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Index",
                table: "Categories",
                column: "Index");

            migrationBuilder.CreateIndex(
                name: "IX_Links_CategoryModelKey",
                table: "Links",
                column: "CategoryModelKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
