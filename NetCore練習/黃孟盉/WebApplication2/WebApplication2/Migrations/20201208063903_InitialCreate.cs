using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication2.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Birthday = table.Column<DateTime>(type: "dateTime", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Records_Date",
                table: "Records",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Records");
        }
    }
}
