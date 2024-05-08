using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace acme.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Address");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Address",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Address",
                newName: "FullAddress");

            migrationBuilder.AddColumn<string>(
                name: "PO",
                table: "ImportedOrder",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PO",
                table: "ImportedOrder");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Address",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "FullAddress",
                table: "Address",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Address",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Address",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Address",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Address",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
