using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaijaPut.Core.Migrations
{
    public partial class updateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Active",
                table: "AspNetUsers",
                newName: "SuspendUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SuspendUser",
                table: "AspNetUsers",
                newName: "Active");
        }
    }
}
