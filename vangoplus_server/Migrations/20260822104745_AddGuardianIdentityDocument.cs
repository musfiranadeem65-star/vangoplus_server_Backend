using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vangoplus_server.Migrations
{
    /// <inheritdoc />
    public partial class AddGuardianIdentityDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityDocumentPath",
                table: "Guardians",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "IdentityDocumentPath",
               table: "Guardians");
        }
    }
}
