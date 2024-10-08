using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F24_Assignment3_mwebster.Migrations
{
    /// <inheritdoc />
    public partial class addedImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "MovieImage",
                table: "Movie",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ActorImage",
                table: "Actor",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieImage",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "ActorImage",
                table: "Actor");
        }
    }
}
