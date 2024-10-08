using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F24_Assignment3_mwebster.Migrations
{
    /// <inheritdoc />
    public partial class fixedReleaseDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ReleaseDate",
                table: "Movie",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ReleaseDate",
                table: "Movie",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
