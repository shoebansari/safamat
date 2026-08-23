using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matrimonial.AdminApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantPanelEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "member_plans",
                columns: table => new
                {
                    member_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    duration_days = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_member_plans", x => x.member_plan_id);
                    table.ForeignKey(
                        name: "fk_member_plans_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    profile_photo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    profile_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    photo_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_members", x => x.member_id);
                    table.ForeignKey(
                        name: "fk_members_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member_subscriptions",
                columns: table => new
                {
                    member_subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    member_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    assigned_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_member_subscriptions", x => x.member_subscription_id);
                    table.ForeignKey(
                        name: "fk_member_subscriptions_member_plans_member_plan_id",
                        column: x => x.member_plan_id,
                        principalTable: "member_plans",
                        principalColumn: "member_plan_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_member_subscriptions_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "member_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_member_subscriptions_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_member_plans_tenant_id",
                table: "member_plans",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_member_subscriptions_member_id",
                table: "member_subscriptions",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "ix_member_subscriptions_member_plan_id",
                table: "member_subscriptions",
                column: "member_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_member_subscriptions_tenant_id",
                table: "member_subscriptions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_members_tenant_id_user_code",
                table: "members",
                columns: new[] { "tenant_id", "user_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "member_subscriptions");

            migrationBuilder.DropTable(
                name: "member_plans");

            migrationBuilder.DropTable(
                name: "members");
        }
    }
}
