using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matrimonial.AdminApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantUserNamePassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "user_name",
                table: "tenants");
        }
    }
}
