using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matrimonial.AdminApi.Migrations
{
    /// <inheritdoc />
    public partial class SnakeCasePostgresIdentifiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_payments_tenants_tenantid",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_payments_tenantsubscriptions_subscriptionid",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_tenants_adminusers_createdby",
                table: "tenants");

            migrationBuilder.DropForeignKey(
                name: "fk_tenantsubscriptions_subscriptionplans_planid",
                table: "tenantsubscriptions");

            migrationBuilder.DropForeignKey(
                name: "fk_tenantsubscriptions_tenants_tenantid",
                table: "tenantsubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tenantsubscriptions",
                table: "tenantsubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_systemsettings",
                table: "systemsettings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subscriptionplans",
                table: "subscriptionplans");

            migrationBuilder.DropPrimaryKey(
                name: "pk_emailtemplates",
                table: "emailtemplates");

            migrationBuilder.DropPrimaryKey(
                name: "pk_adminusers",
                table: "adminusers");

            migrationBuilder.RenameTable(
                name: "tenantsubscriptions",
                newName: "tenant_subscriptions");

            migrationBuilder.RenameTable(
                name: "systemsettings",
                newName: "system_settings");

            migrationBuilder.RenameTable(
                name: "subscriptionplans",
                newName: "subscription_plans");

            migrationBuilder.RenameTable(
                name: "emailtemplates",
                newName: "email_templates");

            migrationBuilder.RenameTable(
                name: "adminusers",
                newName: "admin_users");

            migrationBuilder.RenameColumn(
                name: "zipcode",
                table: "tenants",
                newName: "zip_code");

            migrationBuilder.RenameColumn(
                name: "updatedon",
                table: "tenants",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "tenantcode",
                table: "tenants",
                newName: "tenant_code");

            migrationBuilder.RenameColumn(
                name: "ownername",
                table: "tenants",
                newName: "owner_name");

            migrationBuilder.RenameColumn(
                name: "logourl",
                table: "tenants",
                newName: "logo_url");

            migrationBuilder.RenameColumn(
                name: "isactive",
                table: "tenants",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "databaseserver",
                table: "tenants",
                newName: "database_server");

            migrationBuilder.RenameColumn(
                name: "databasename",
                table: "tenants",
                newName: "database_name");

            migrationBuilder.RenameColumn(
                name: "createdon",
                table: "tenants",
                newName: "created_on");

            migrationBuilder.RenameColumn(
                name: "createdby",
                table: "tenants",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "connectionstring",
                table: "tenants",
                newName: "connection_string");

            migrationBuilder.RenameColumn(
                name: "companyname",
                table: "tenants",
                newName: "company_name");

            migrationBuilder.RenameColumn(
                name: "tenantid",
                table: "tenants",
                newName: "tenant_id");

            migrationBuilder.RenameIndex(
                name: "ix_tenants_tenantcode",
                table: "tenants",
                newName: "ix_tenants_tenant_code");

            migrationBuilder.RenameIndex(
                name: "ix_tenants_createdby",
                table: "tenants",
                newName: "ix_tenants_created_by");

            migrationBuilder.RenameColumn(
                name: "transactionid",
                table: "payments",
                newName: "transaction_id");

            migrationBuilder.RenameColumn(
                name: "tenantid",
                table: "payments",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "subscriptionid",
                table: "payments",
                newName: "subscription_id");

            migrationBuilder.RenameColumn(
                name: "paymentmethod",
                table: "payments",
                newName: "payment_method");

            migrationBuilder.RenameColumn(
                name: "paymentgateway",
                table: "payments",
                newName: "payment_gateway");

            migrationBuilder.RenameColumn(
                name: "paidon",
                table: "payments",
                newName: "paid_on");

            migrationBuilder.RenameColumn(
                name: "invoicenumber",
                table: "payments",
                newName: "invoice_number");

            migrationBuilder.RenameColumn(
                name: "paymentid",
                table: "payments",
                newName: "payment_id");

            migrationBuilder.RenameIndex(
                name: "ix_payments_tenantid",
                table: "payments",
                newName: "ix_payments_tenant_id");

            migrationBuilder.RenameIndex(
                name: "ix_payments_subscriptionid",
                table: "payments",
                newName: "ix_payments_subscription_id");

            migrationBuilder.RenameColumn(
                name: "tenantid",
                table: "tenant_subscriptions",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "subscriptionstatus",
                table: "tenant_subscriptions",
                newName: "subscription_status");

            migrationBuilder.RenameColumn(
                name: "startdate",
                table: "tenant_subscriptions",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "planid",
                table: "tenant_subscriptions",
                newName: "plan_id");

            migrationBuilder.RenameColumn(
                name: "paymentstatus",
                table: "tenant_subscriptions",
                newName: "payment_status");

            migrationBuilder.RenameColumn(
                name: "nextbillingdate",
                table: "tenant_subscriptions",
                newName: "next_billing_date");

            migrationBuilder.RenameColumn(
                name: "enddate",
                table: "tenant_subscriptions",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "createdon",
                table: "tenant_subscriptions",
                newName: "created_on");

            migrationBuilder.RenameColumn(
                name: "tenantsubscriptionsid",
                table: "tenant_subscriptions",
                newName: "tenant_subscriptions_id");

            migrationBuilder.RenameIndex(
                name: "ix_tenantsubscriptions_tenantid",
                table: "tenant_subscriptions",
                newName: "ix_tenant_subscriptions_tenant_id");

            migrationBuilder.RenameIndex(
                name: "ix_tenantsubscriptions_planid",
                table: "tenant_subscriptions",
                newName: "ix_tenant_subscriptions_plan_id");

            migrationBuilder.RenameColumn(
                name: "settingvalue",
                table: "system_settings",
                newName: "setting_value");

            migrationBuilder.RenameColumn(
                name: "settingkey",
                table: "system_settings",
                newName: "setting_key");

            migrationBuilder.RenameColumn(
                name: "settingid",
                table: "system_settings",
                newName: "setting_id");

            migrationBuilder.RenameIndex(
                name: "ix_systemsettings_settingkey",
                table: "system_settings",
                newName: "ix_system_settings_setting_key");

            migrationBuilder.RenameColumn(
                name: "planname",
                table: "subscription_plans",
                newName: "plan_name");

            migrationBuilder.RenameColumn(
                name: "isactive",
                table: "subscription_plans",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "durationdays",
                table: "subscription_plans",
                newName: "duration_days");

            migrationBuilder.RenameColumn(
                name: "createdon",
                table: "subscription_plans",
                newName: "created_on");

            migrationBuilder.RenameColumn(
                name: "planid",
                table: "subscription_plans",
                newName: "plan_id");

            migrationBuilder.RenameColumn(
                name: "templatename",
                table: "email_templates",
                newName: "template_name");

            migrationBuilder.RenameColumn(
                name: "isactive",
                table: "email_templates",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "templateid",
                table: "email_templates",
                newName: "template_id");

            migrationBuilder.RenameIndex(
                name: "ix_emailtemplates_templatename",
                table: "email_templates",
                newName: "ix_email_templates_template_name");

            migrationBuilder.RenameColumn(
                name: "updatedon",
                table: "admin_users",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "lastname",
                table: "admin_users",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "lastlogin",
                table: "admin_users",
                newName: "last_login");

            migrationBuilder.RenameColumn(
                name: "isactive",
                table: "admin_users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "firstname",
                table: "admin_users",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "createdon",
                table: "admin_users",
                newName: "created_on");

            migrationBuilder.RenameColumn(
                name: "adminusername",
                table: "admin_users",
                newName: "admin_user_name");

            migrationBuilder.RenameColumn(
                name: "adminid",
                table: "admin_users",
                newName: "admin_id");

            migrationBuilder.RenameIndex(
                name: "ix_adminusers_email",
                table: "admin_users",
                newName: "ix_admin_users_email");

            migrationBuilder.RenameIndex(
                name: "ix_adminusers_adminusername",
                table: "admin_users",
                newName: "ix_admin_users_admin_user_name");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tenant_subscriptions",
                table: "tenant_subscriptions",
                column: "tenant_subscriptions_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_system_settings",
                table: "system_settings",
                column: "setting_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_subscription_plans",
                table: "subscription_plans",
                column: "plan_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_email_templates",
                table: "email_templates",
                column: "template_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_admin_users",
                table: "admin_users",
                column: "admin_id");

            migrationBuilder.AddForeignKey(
                name: "fk_payments_tenant_subscriptions_subscription_id",
                table: "payments",
                column: "subscription_id",
                principalTable: "tenant_subscriptions",
                principalColumn: "tenant_subscriptions_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_payments_tenants_tenant_id",
                table: "payments",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "tenant_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tenant_subscriptions_subscription_plans_plan_id",
                table: "tenant_subscriptions",
                column: "plan_id",
                principalTable: "subscription_plans",
                principalColumn: "plan_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tenant_subscriptions_tenants_tenant_id",
                table: "tenant_subscriptions",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "tenant_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tenants_admin_users_created_by",
                table: "tenants",
                column: "created_by",
                principalTable: "admin_users",
                principalColumn: "admin_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_payments_tenant_subscriptions_subscription_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_payments_tenants_tenant_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_tenant_subscriptions_subscription_plans_plan_id",
                table: "tenant_subscriptions");

            migrationBuilder.DropForeignKey(
                name: "fk_tenant_subscriptions_tenants_tenant_id",
                table: "tenant_subscriptions");

            migrationBuilder.DropForeignKey(
                name: "fk_tenants_admin_users_created_by",
                table: "tenants");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tenant_subscriptions",
                table: "tenant_subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_system_settings",
                table: "system_settings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subscription_plans",
                table: "subscription_plans");

            migrationBuilder.DropPrimaryKey(
                name: "pk_email_templates",
                table: "email_templates");

            migrationBuilder.DropPrimaryKey(
                name: "pk_admin_users",
                table: "admin_users");

            migrationBuilder.RenameTable(
                name: "tenant_subscriptions",
                newName: "tenantsubscriptions");

            migrationBuilder.RenameTable(
                name: "system_settings",
                newName: "systemsettings");

            migrationBuilder.RenameTable(
                name: "subscription_plans",
                newName: "subscriptionplans");

            migrationBuilder.RenameTable(
                name: "email_templates",
                newName: "emailtemplates");

            migrationBuilder.RenameTable(
                name: "admin_users",
                newName: "adminusers");

            migrationBuilder.RenameColumn(
                name: "zip_code",
                table: "tenants",
                newName: "zipcode");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "tenants",
                newName: "updatedon");

            migrationBuilder.RenameColumn(
                name: "tenant_code",
                table: "tenants",
                newName: "tenantcode");

            migrationBuilder.RenameColumn(
                name: "owner_name",
                table: "tenants",
                newName: "ownername");

            migrationBuilder.RenameColumn(
                name: "logo_url",
                table: "tenants",
                newName: "logourl");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "tenants",
                newName: "isactive");

            migrationBuilder.RenameColumn(
                name: "database_server",
                table: "tenants",
                newName: "databaseserver");

            migrationBuilder.RenameColumn(
                name: "database_name",
                table: "tenants",
                newName: "databasename");

            migrationBuilder.RenameColumn(
                name: "created_on",
                table: "tenants",
                newName: "createdon");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "tenants",
                newName: "createdby");

            migrationBuilder.RenameColumn(
                name: "connection_string",
                table: "tenants",
                newName: "connectionstring");

            migrationBuilder.RenameColumn(
                name: "company_name",
                table: "tenants",
                newName: "companyname");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "tenants",
                newName: "tenantid");

            migrationBuilder.RenameIndex(
                name: "ix_tenants_tenant_code",
                table: "tenants",
                newName: "ix_tenants_tenantcode");

            migrationBuilder.RenameIndex(
                name: "ix_tenants_created_by",
                table: "tenants",
                newName: "ix_tenants_createdby");

            migrationBuilder.RenameColumn(
                name: "transaction_id",
                table: "payments",
                newName: "transactionid");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "payments",
                newName: "tenantid");

            migrationBuilder.RenameColumn(
                name: "subscription_id",
                table: "payments",
                newName: "subscriptionid");

            migrationBuilder.RenameColumn(
                name: "payment_method",
                table: "payments",
                newName: "paymentmethod");

            migrationBuilder.RenameColumn(
                name: "payment_gateway",
                table: "payments",
                newName: "paymentgateway");

            migrationBuilder.RenameColumn(
                name: "paid_on",
                table: "payments",
                newName: "paidon");

            migrationBuilder.RenameColumn(
                name: "invoice_number",
                table: "payments",
                newName: "invoicenumber");

            migrationBuilder.RenameColumn(
                name: "payment_id",
                table: "payments",
                newName: "paymentid");

            migrationBuilder.RenameIndex(
                name: "ix_payments_tenant_id",
                table: "payments",
                newName: "ix_payments_tenantid");

            migrationBuilder.RenameIndex(
                name: "ix_payments_subscription_id",
                table: "payments",
                newName: "ix_payments_subscriptionid");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "tenantsubscriptions",
                newName: "tenantid");

            migrationBuilder.RenameColumn(
                name: "subscription_status",
                table: "tenantsubscriptions",
                newName: "subscriptionstatus");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "tenantsubscriptions",
                newName: "startdate");

            migrationBuilder.RenameColumn(
                name: "plan_id",
                table: "tenantsubscriptions",
                newName: "planid");

            migrationBuilder.RenameColumn(
                name: "payment_status",
                table: "tenantsubscriptions",
                newName: "paymentstatus");

            migrationBuilder.RenameColumn(
                name: "next_billing_date",
                table: "tenantsubscriptions",
                newName: "nextbillingdate");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "tenantsubscriptions",
                newName: "enddate");

            migrationBuilder.RenameColumn(
                name: "created_on",
                table: "tenantsubscriptions",
                newName: "createdon");

            migrationBuilder.RenameColumn(
                name: "tenant_subscriptions_id",
                table: "tenantsubscriptions",
                newName: "tenantsubscriptionsid");

            migrationBuilder.RenameIndex(
                name: "ix_tenant_subscriptions_tenant_id",
                table: "tenantsubscriptions",
                newName: "ix_tenantsubscriptions_tenantid");

            migrationBuilder.RenameIndex(
                name: "ix_tenant_subscriptions_plan_id",
                table: "tenantsubscriptions",
                newName: "ix_tenantsubscriptions_planid");

            migrationBuilder.RenameColumn(
                name: "setting_value",
                table: "systemsettings",
                newName: "settingvalue");

            migrationBuilder.RenameColumn(
                name: "setting_key",
                table: "systemsettings",
                newName: "settingkey");

            migrationBuilder.RenameColumn(
                name: "setting_id",
                table: "systemsettings",
                newName: "settingid");

            migrationBuilder.RenameIndex(
                name: "ix_system_settings_setting_key",
                table: "systemsettings",
                newName: "ix_systemsettings_settingkey");

            migrationBuilder.RenameColumn(
                name: "plan_name",
                table: "subscriptionplans",
                newName: "planname");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "subscriptionplans",
                newName: "isactive");

            migrationBuilder.RenameColumn(
                name: "duration_days",
                table: "subscriptionplans",
                newName: "durationdays");

            migrationBuilder.RenameColumn(
                name: "created_on",
                table: "subscriptionplans",
                newName: "createdon");

            migrationBuilder.RenameColumn(
                name: "plan_id",
                table: "subscriptionplans",
                newName: "planid");

            migrationBuilder.RenameColumn(
                name: "template_name",
                table: "emailtemplates",
                newName: "templatename");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "emailtemplates",
                newName: "isactive");

            migrationBuilder.RenameColumn(
                name: "template_id",
                table: "emailtemplates",
                newName: "templateid");

            migrationBuilder.RenameIndex(
                name: "ix_email_templates_template_name",
                table: "emailtemplates",
                newName: "ix_emailtemplates_templatename");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "adminusers",
                newName: "updatedon");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "adminusers",
                newName: "lastname");

            migrationBuilder.RenameColumn(
                name: "last_login",
                table: "adminusers",
                newName: "lastlogin");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "adminusers",
                newName: "isactive");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "adminusers",
                newName: "firstname");

            migrationBuilder.RenameColumn(
                name: "created_on",
                table: "adminusers",
                newName: "createdon");

            migrationBuilder.RenameColumn(
                name: "admin_user_name",
                table: "adminusers",
                newName: "adminusername");

            migrationBuilder.RenameColumn(
                name: "admin_id",
                table: "adminusers",
                newName: "adminid");

            migrationBuilder.RenameIndex(
                name: "ix_admin_users_email",
                table: "adminusers",
                newName: "ix_adminusers_email");

            migrationBuilder.RenameIndex(
                name: "ix_admin_users_admin_user_name",
                table: "adminusers",
                newName: "ix_adminusers_adminusername");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tenantsubscriptions",
                table: "tenantsubscriptions",
                column: "tenantsubscriptionsid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_systemsettings",
                table: "systemsettings",
                column: "settingid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_subscriptionplans",
                table: "subscriptionplans",
                column: "planid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_emailtemplates",
                table: "emailtemplates",
                column: "templateid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_adminusers",
                table: "adminusers",
                column: "adminid");

            migrationBuilder.AddForeignKey(
                name: "fk_payments_tenants_tenantid",
                table: "payments",
                column: "tenantid",
                principalTable: "tenants",
                principalColumn: "tenantid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payments_tenantsubscriptions_subscriptionid",
                table: "payments",
                column: "subscriptionid",
                principalTable: "tenantsubscriptions",
                principalColumn: "tenantsubscriptionsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tenants_adminusers_createdby",
                table: "tenants",
                column: "createdby",
                principalTable: "adminusers",
                principalColumn: "adminid",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_tenantsubscriptions_subscriptionplans_planid",
                table: "tenantsubscriptions",
                column: "planid",
                principalTable: "subscriptionplans",
                principalColumn: "planid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tenantsubscriptions_tenants_tenantid",
                table: "tenantsubscriptions",
                column: "tenantid",
                principalTable: "tenants",
                principalColumn: "tenantid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
