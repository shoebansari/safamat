using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matrimonial.AdminApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPanelEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    user_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    password = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "blocked_users",
                columns: table => new
                {
                    blocked_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blocked_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_blocked_users", x => x.blocked_id);
                    table.ForeignKey(
                        name: "fk_blocked_users_users_blocked_user_id",
                        column: x => x.blocked_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_blocked_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "favorites",
                columns: table => new
                {
                    favorite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    favorite_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_favorites", x => x.favorite_id);
                    table.ForeignKey(
                        name: "fk_favorites_users_favorite_user_id",
                        column: x => x.favorite_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_favorites_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "interest_requests",
                columns: table => new
                {
                    interest_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    sent_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    responded_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_interest_requests", x => x.interest_id);
                    table.ForeignKey(
                        name: "fk_interest_requests_users_receiver_user_id",
                        column: x => x.receiver_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_interest_requests_users_sender_user_id",
                        column: x => x.sender_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id1 = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id2 = table.Column<Guid>(type: "uuid", nullable: false),
                    match_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    matched_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_matches", x => x.match_id);
                    table.ForeignKey(
                        name: "fk_matches_users_user_id1",
                        column: x => x.user_id1,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_matches_users_user_id2",
                        column: x => x.user_id2,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    sent_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.message_id);
                    table.ForeignKey(
                        name: "fk_messages_users_receiver_user_id",
                        column: x => x.receiver_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_messages_users_sender_user_id",
                        column: x => x.sender_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "fk_notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "profile_views",
                columns: table => new
                {
                    view_id = table.Column<Guid>(type: "uuid", nullable: false),
                    viewer_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    viewed_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    viewed_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profile_views", x => x.view_id);
                    table.ForeignKey(
                        name: "fk_profile_views_users_viewed_user_id",
                        column: x => x.viewed_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_profile_views_users_viewer_user_id",
                        column: x => x.viewer_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    report_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reporter_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reported_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    details = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reports", x => x.report_id);
                    table.ForeignKey(
                        name: "fk_reports_users_reported_user_id",
                        column: x => x.reported_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reports_users_reporter_user_id",
                        column: x => x.reporter_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_educations",
                columns: table => new
                {
                    education_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    qualification = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    college = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    university = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    passing_year = table.Column<int>(type: "integer", nullable: true),
                    education_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_educations", x => x.education_id);
                    table.ForeignKey(
                        name: "fk_user_educations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_family_details",
                columns: table => new
                {
                    family_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    family_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    family_status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    father_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    father_occupation = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    mother_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    mother_occupation = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    brothers = table.Column<int>(type: "integer", nullable: true),
                    sisters = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_family_details", x => x.family_id);
                    table.ForeignKey(
                        name: "fk_user_family_details_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_lifestyles",
                columns: table => new
                {
                    lifestyle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    diet = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    smoking = table.Column<bool>(type: "boolean", nullable: false),
                    drinking = table.Column<bool>(type: "boolean", nullable: false),
                    hobbies = table.Column<string>(type: "text", nullable: true),
                    languages_known = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_lifestyles", x => x.lifestyle_id);
                    table.ForeignKey(
                        name: "fk_user_lifestyles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_locations",
                columns: table => new
                {
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    pincode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_locations", x => x.location_id);
                    table.ForeignKey(
                        name: "fk_user_locations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_occupations",
                columns: table => new
                {
                    occupation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occupation = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    company_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    designation = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    annual_income = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    work_location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_occupations", x => x.occupation_id);
                    table.ForeignKey(
                        name: "fk_user_occupations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_photos",
                columns: table => new
                {
                    photo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    photo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false),
                    uploaded_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_photos", x => x.photo_id);
                    table.ForeignKey(
                        name: "fk_user_photos_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                columns: table => new
                {
                    preference_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    min_age = table.Column<int>(type: "integer", nullable: true),
                    max_age = table.Column<int>(type: "integer", nullable: true),
                    min_height = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    max_height = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    religion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    caste = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    education = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    occupation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_preferences", x => x.preference_id);
                    table.ForeignKey(
                        name: "fk_user_preferences_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    height = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    weight = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    marital_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    religion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    caste = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    sub_caste = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    mother_tongue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    blood_group = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    about_me = table.Column<string>(type: "text", nullable: true),
                    is_profile_completed = table.Column<bool>(type: "boolean", nullable: false),
                    profile_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_profiles", x => x.profile_id);
                    table.ForeignKey(
                        name: "fk_user_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    refresh_token = table.Column<string>(type: "text", nullable: true),
                    device_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    browser = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ip_address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    login_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    logout_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_sessions", x => x.session_id);
                    table.ForeignKey(
                        name: "fk_user_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_subscriptions",
                columns: table => new
                {
                    user_subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    member_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    assigned_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_subscriptions", x => x.user_subscription_id);
                    table.ForeignKey(
                        name: "fk_user_subscriptions_member_plans_member_plan_id",
                        column: x => x.member_plan_id,
                        principalTable: "member_plans",
                        principalColumn: "member_plan_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_subscriptions_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_blocked_users_blocked_user_id",
                table: "blocked_users",
                column: "blocked_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_blocked_users_user_id_blocked_user_id",
                table: "blocked_users",
                columns: new[] { "user_id", "blocked_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_favorites_favorite_user_id",
                table: "favorites",
                column: "favorite_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_favorites_user_id_favorite_user_id",
                table: "favorites",
                columns: new[] { "user_id", "favorite_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interest_requests_receiver_user_id",
                table: "interest_requests",
                column: "receiver_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_interest_requests_sender_user_id_receiver_user_id",
                table: "interest_requests",
                columns: new[] { "sender_user_id", "receiver_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_matches_user_id1_user_id2",
                table: "matches",
                columns: new[] { "user_id1", "user_id2" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_matches_user_id2",
                table: "matches",
                column: "user_id2");

            migrationBuilder.CreateIndex(
                name: "ix_messages_receiver_user_id",
                table: "messages",
                column: "receiver_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_sender_user_id",
                table: "messages",
                column: "sender_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_profile_views_viewed_user_id",
                table: "profile_views",
                column: "viewed_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_profile_views_viewer_user_id",
                table: "profile_views",
                column: "viewer_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_reported_user_id",
                table: "reports",
                column: "reported_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_reporter_user_id",
                table: "reports",
                column: "reporter_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_educations_user_id",
                table: "user_educations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_family_details_user_id",
                table: "user_family_details",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_lifestyles_user_id",
                table: "user_lifestyles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_locations_user_id",
                table: "user_locations",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_occupations_user_id",
                table: "user_occupations",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_photos_user_id",
                table: "user_photos",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_preferences_user_id",
                table: "user_preferences",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_profiles_user_id",
                table: "user_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_user_id",
                table: "user_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_subscriptions_member_plan_id",
                table: "user_subscriptions",
                column: "member_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_subscriptions_tenant_id",
                table: "user_subscriptions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_subscriptions_user_id",
                table: "user_subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_tenant_id_user_code",
                table: "users",
                columns: new[] { "tenant_id", "user_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_tenant_id_user_name",
                table: "users",
                columns: new[] { "tenant_id", "user_name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blocked_users");

            migrationBuilder.DropTable(
                name: "favorites");

            migrationBuilder.DropTable(
                name: "interest_requests");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "profile_views");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "user_educations");

            migrationBuilder.DropTable(
                name: "user_family_details");

            migrationBuilder.DropTable(
                name: "user_lifestyles");

            migrationBuilder.DropTable(
                name: "user_locations");

            migrationBuilder.DropTable(
                name: "user_occupations");

            migrationBuilder.DropTable(
                name: "user_photos");

            migrationBuilder.DropTable(
                name: "user_preferences");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "user_sessions");

            migrationBuilder.DropTable(
                name: "user_subscriptions");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
