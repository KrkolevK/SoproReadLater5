using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class ModifyCategoryAndBookmarkEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                type:"nvarchar(450)",
                table: "Categories",
                nullable: true);

          
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                type: "nvarchar(450)",
                table: "Bookmark",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
