using Microsoft.EntityFrameworkCore.Migrations;

namespace Kutori.Migrations
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PostId = table.Column<int>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Timestamp = table.Column<long>(nullable: false),
                    IP = table.Column<string>(nullable: true),
                    ImageData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
