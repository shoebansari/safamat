using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matrimonial.AdminApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationRelatedUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "related_user_id",
                table: "notifications",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "related_user_id",
                table: "notifications");
        }
    }
}
