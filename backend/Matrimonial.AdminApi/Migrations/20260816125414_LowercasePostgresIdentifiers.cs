using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matrimonial.AdminApi.Migrations
{
    /// <inheritdoc />
    public partial class LowercasePostgresIdentifiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_TenantSubscriptions_SubscriptionId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Tenants_TenantId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_AdminUsers_CreatedBy",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantSubscriptions_SubscriptionPlans_PlanId",
                table: "TenantSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantSubscriptions_Tenants_TenantId",
                table: "TenantSubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantSubscriptions",
                table: "TenantSubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemSettings",
                table: "SystemSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionPlans",
                table: "SubscriptionPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailTemplates",
                table: "EmailTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdminUsers",
                table: "AdminUsers");

            migrationBuilder.RenameTable(
                name: "TenantSubscriptions",
                newName: "tenantsubscriptions");

            migrationBuilder.RenameTable(
                name: "Tenants",
                newName: "tenants");

            migrationBuilder.RenameTable(
                name: "SystemSettings",
                newName: "systemsettings");

            migrationBuilder.RenameTable(
                name: "SubscriptionPlans",
                newName: "subscriptionplans");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "payments");

            migrationBuilder.RenameTable(
                name: "EmailTemplates",
                newName: "emailtemplates");

            migrationBuilder.RenameTable(
                name: "AdminUsers",
                newName: "adminusers");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "tenantsubscriptions",
                newName: "tenantid");

            migrationBuilder.RenameColumn(
                name: "SubscriptionStatus",
                table: "tenantsubscriptions",
                newName: "subscriptionstatus");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "tenantsubscriptions",
                newName: "startdate");

            migrationBuilder.RenameColumn(
                name: "PlanId",
                table: "tenantsubscriptions",
                newName: "planid");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "tenantsubscriptions",
                newName: "paymentstatus");

            migrationBuilder.RenameColumn(
                name: "NextBillingDate",
                table: "tenantsubscriptions",
                newName: "nextbillingdate");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "tenantsubscriptions",
                newName: "enddate");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "tenantsubscriptions",
                newName: "createdon");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "tenantsubscriptions",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "TenantSubscriptionsId",
                table: "tenantsubscriptions",
                newName: "tenantsubscriptionsid");

            migrationBuilder.RenameIndex(
                name: "IX_TenantSubscriptions_TenantId",
                table: "tenantsubscriptions",
                newName: "ix_tenantsubscriptions_tenantid");

            migrationBuilder.RenameIndex(
                name: "IX_TenantSubscriptions_PlanId",
                table: "tenantsubscriptions",
                newName: "ix_tenantsubscriptions_planid");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "tenants",
                newName: "zipcode");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "tenants",
                newName: "updatedon");

            migrationBuilder.RenameColumn(
                name: "TenantCode",
                table: "tenants",
                newName: "tenantcode");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "tenants",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "tenants",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "OwnerName",
                table: "tenants",
                newName: "ownername");

            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "tenants",
                newName: "logourl");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "tenants",
                newName: "isactive");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "tenants",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "DatabaseServer",
                table: "tenants",
                newName: "databaseserver");

            migrationBuilder.RenameColumn(
                name: "DatabaseName",
                table: "tenants",
                newName: "databasename");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "tenants",
                newName: "createdon");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "tenants",
                newName: "createdby");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "tenants",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "ConnectionString",
                table: "tenants",
                newName: "connectionstring");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "tenants",
                newName: "companyname");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "tenants",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "tenants",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "tenants",
                newName: "tenantid");

            migrationBuilder.RenameIndex(
                name: "IX_Tenants_TenantCode",
                table: "tenants",
                newName: "ix_tenants_tenantcode");

            migrationBuilder.RenameIndex(
                name: "IX_Tenants_CreatedBy",
                table: "tenants",
                newName: "ix_tenants_createdby");

            migrationBuilder.RenameColumn(
                name: "SettingValue",
                table: "systemsettings",
                newName: "settingvalue");

            migrationBuilder.RenameColumn(
                name: "SettingKey",
                table: "systemsettings",
                newName: "settingkey");

            migrationBuilder.RenameColumn(
                name: "SettingId",
                table: "systemsettings",
                newName: "settingid");

            migrationBuilder.RenameIndex(
                name: "IX_SystemSettings_SettingKey",
                table: "systemsettings",
                newName: "ix_systemsettings_settingkey");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "subscriptionplans",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "PlanName",
                table: "subscriptionplans",
                newName: "planname");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "subscriptionplans",
                newName: "isactive");

            migrationBuilder.RenameColumn(
                name: "DurationDays",
                table: "subscriptionplans",
                newName: "durationdays");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "subscriptionplans",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "subscriptionplans",
                newName: "createdon");

            migrationBuilder.RenameColumn(
                name: "PlanId",
                table: "subscriptionplans",
                newName: "planid");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "payments",
                newName: "transactionid");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "payments",
                newName: "tenantid");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "payments",
                newName: "subscriptionid");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "payments",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "payments",
                newName: "paymentmethod");

            migrationBuilder.RenameColumn(
                name: "PaymentGateway",
                table: "payments",
                newName: "paymentgateway");

            migrationBuilder.RenameColumn(
                name: "PaidOn",
                table: "payments",
                newName: "paidon");

            migrationBuilder.RenameColumn(
                name: "InvoiceNumber",
                table: "payments",
                newName: "invoicenumber");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "payments",
                newName: "currency");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "payments",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "payments",
                newName: "paymentid");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_TenantId",
                table: "payments",
                newName: "ix_payments_tenantid");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_SubscriptionId",
                table: "payments",
                newName: "ix_payments_subscriptionid");

            migrationBuilder.RenameColumn(
                name: "TemplateName",
                table: "emailtemplates",
                newName: "templatename");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "emailtemplates",
                newName: "subject");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "emailtemplates",
                newName: "isactive");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "emailtemplates",
                newName: "body");

            migrationBuilder.RenameColumn(
                name: "TemplateId",
                table: "emailtemplates",
                newName: "templateid");

            migrationBuilder.RenameIndex(
                name: "IX_EmailTemplates_TemplateName",
                table: "emailtemplates",
                newName: "ix_emailtemplates_templatename");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "adminusers",
                newName: "updatedon");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "adminusers",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "adminusers",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "adminusers",
                newName: "lastname");

            migrationBuilder.RenameColumn(
                name: "LastLogin",
                table: "adminusers",
                newName: "lastlogin");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "adminusers",
                newName: "isactive");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "adminusers",
                newName: "firstname");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "adminusers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "adminusers",
                newName: "createdon");

            migrationBuilder.RenameColumn(
                name: "AdminUserName",
                table: "adminusers",
                newName: "adminusername");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "adminusers",
                newName: "adminid");

            migrationBuilder.RenameIndex(
                name: "IX_AdminUsers_Email",
                table: "adminusers",
                newName: "ix_adminusers_email");

            migrationBuilder.RenameIndex(
                name: "IX_AdminUsers_AdminUserName",
                table: "adminusers",
                newName: "ix_adminusers_adminusername");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tenantsubscriptions",
                table: "tenantsubscriptions",
                column: "tenantsubscriptionsid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tenants",
                table: "tenants",
                column: "tenantid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_systemsettings",
                table: "systemsettings",
                column: "settingid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_subscriptionplans",
                table: "subscriptionplans",
                column: "planid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_payments",
                table: "payments",
                column: "paymentid");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "pk_tenants",
                table: "tenants");

            migrationBuilder.DropPrimaryKey(
                name: "pk_systemsettings",
                table: "systemsettings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subscriptionplans",
                table: "subscriptionplans");

            migrationBuilder.DropPrimaryKey(
                name: "pk_payments",
                table: "payments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_emailtemplates",
                table: "emailtemplates");

            migrationBuilder.DropPrimaryKey(
                name: "pk_adminusers",
                table: "adminusers");

            migrationBuilder.RenameTable(
                name: "tenantsubscriptions",
                newName: "TenantSubscriptions");

            migrationBuilder.RenameTable(
                name: "tenants",
                newName: "Tenants");

            migrationBuilder.RenameTable(
                name: "systemsettings",
                newName: "SystemSettings");

            migrationBuilder.RenameTable(
                name: "subscriptionplans",
                newName: "SubscriptionPlans");

            migrationBuilder.RenameTable(
                name: "payments",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "emailtemplates",
                newName: "EmailTemplates");

            migrationBuilder.RenameTable(
                name: "adminusers",
                newName: "AdminUsers");

            migrationBuilder.RenameColumn(
                name: "tenantid",
                table: "TenantSubscriptions",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "subscriptionstatus",
                table: "TenantSubscriptions",
                newName: "SubscriptionStatus");

            migrationBuilder.RenameColumn(
                name: "startdate",
                table: "TenantSubscriptions",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "planid",
                table: "TenantSubscriptions",
                newName: "PlanId");

            migrationBuilder.RenameColumn(
                name: "paymentstatus",
                table: "TenantSubscriptions",
                newName: "PaymentStatus");

            migrationBuilder.RenameColumn(
                name: "nextbillingdate",
                table: "TenantSubscriptions",
                newName: "NextBillingDate");

            migrationBuilder.RenameColumn(
                name: "enddate",
                table: "TenantSubscriptions",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "createdon",
                table: "TenantSubscriptions",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "TenantSubscriptions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "tenantsubscriptionsid",
                table: "TenantSubscriptions",
                newName: "TenantSubscriptionsId");

            migrationBuilder.RenameIndex(
                name: "ix_tenantsubscriptions_tenantid",
                table: "TenantSubscriptions",
                newName: "IX_TenantSubscriptions_TenantId");

            migrationBuilder.RenameIndex(
                name: "ix_tenantsubscriptions_planid",
                table: "TenantSubscriptions",
                newName: "IX_TenantSubscriptions_PlanId");

            migrationBuilder.RenameColumn(
                name: "zipcode",
                table: "Tenants",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "updatedon",
                table: "Tenants",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "tenantcode",
                table: "Tenants",
                newName: "TenantCode");

            migrationBuilder.RenameColumn(
                name: "state",
                table: "Tenants",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Tenants",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "ownername",
                table: "Tenants",
                newName: "OwnerName");

            migrationBuilder.RenameColumn(
                name: "logourl",
                table: "Tenants",
                newName: "LogoUrl");

            migrationBuilder.RenameColumn(
                name: "isactive",
                table: "Tenants",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Tenants",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "databaseserver",
                table: "Tenants",
                newName: "DatabaseServer");

            migrationBuilder.RenameColumn(
                name: "databasename",
                table: "Tenants",
                newName: "DatabaseName");

            migrationBuilder.RenameColumn(
                name: "createdon",
                table: "Tenants",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "createdby",
                table: "Tenants",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "Tenants",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "connectionstring",
                table: "Tenants",
                newName: "ConnectionString");

            migrationBuilder.RenameColumn(
                name: "companyname",
                table: "Tenants",
                newName: "CompanyName");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "Tenants",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "Tenants",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "tenantid",
                table: "Tenants",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "ix_tenants_tenantcode",
                table: "Tenants",
                newName: "IX_Tenants_TenantCode");

            migrationBuilder.RenameIndex(
                name: "ix_tenants_createdby",
                table: "Tenants",
                newName: "IX_Tenants_CreatedBy");

            migrationBuilder.RenameColumn(
                name: "settingvalue",
                table: "SystemSettings",
                newName: "SettingValue");

            migrationBuilder.RenameColumn(
                name: "settingkey",
                table: "SystemSettings",
                newName: "SettingKey");

            migrationBuilder.RenameColumn(
                name: "settingid",
                table: "SystemSettings",
                newName: "SettingId");

            migrationBuilder.RenameIndex(
                name: "ix_systemsettings_settingkey",
                table: "SystemSettings",
                newName: "IX_SystemSettings_SettingKey");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "SubscriptionPlans",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "planname",
                table: "SubscriptionPlans",
                newName: "PlanName");

            migrationBuilder.RenameColumn(
                name: "isactive",
                table: "SubscriptionPlans",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "durationdays",
                table: "SubscriptionPlans",
                newName: "DurationDays");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "SubscriptionPlans",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "createdon",
                table: "SubscriptionPlans",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "planid",
                table: "SubscriptionPlans",
                newName: "PlanId");

            migrationBuilder.RenameColumn(
                name: "transactionid",
                table: "Payments",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "tenantid",
                table: "Payments",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "subscriptionid",
                table: "Payments",
                newName: "SubscriptionId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Payments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "paymentmethod",
                table: "Payments",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "paymentgateway",
                table: "Payments",
                newName: "PaymentGateway");

            migrationBuilder.RenameColumn(
                name: "paidon",
                table: "Payments",
                newName: "PaidOn");

            migrationBuilder.RenameColumn(
                name: "invoicenumber",
                table: "Payments",
                newName: "InvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "currency",
                table: "Payments",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "Payments",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "paymentid",
                table: "Payments",
                newName: "PaymentId");

            migrationBuilder.RenameIndex(
                name: "ix_payments_tenantid",
                table: "Payments",
                newName: "IX_Payments_TenantId");

            migrationBuilder.RenameIndex(
                name: "ix_payments_subscriptionid",
                table: "Payments",
                newName: "IX_Payments_SubscriptionId");

            migrationBuilder.RenameColumn(
                name: "templatename",
                table: "EmailTemplates",
                newName: "TemplateName");

            migrationBuilder.RenameColumn(
                name: "subject",
                table: "EmailTemplates",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "isactive",
                table: "EmailTemplates",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "body",
                table: "EmailTemplates",
                newName: "Body");

            migrationBuilder.RenameColumn(
                name: "templateid",
                table: "EmailTemplates",
                newName: "TemplateId");

            migrationBuilder.RenameIndex(
                name: "ix_emailtemplates_templatename",
                table: "EmailTemplates",
                newName: "IX_EmailTemplates_TemplateName");

            migrationBuilder.RenameColumn(
                name: "updatedon",
                table: "AdminUsers",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "AdminUsers",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "AdminUsers",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "lastname",
                table: "AdminUsers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "lastlogin",
                table: "AdminUsers",
                newName: "LastLogin");

            migrationBuilder.RenameColumn(
                name: "isactive",
                table: "AdminUsers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "firstname",
                table: "AdminUsers",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "AdminUsers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "createdon",
                table: "AdminUsers",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "adminusername",
                table: "AdminUsers",
                newName: "AdminUserName");

            migrationBuilder.RenameColumn(
                name: "adminid",
                table: "AdminUsers",
                newName: "AdminId");

            migrationBuilder.RenameIndex(
                name: "ix_adminusers_email",
                table: "AdminUsers",
                newName: "IX_AdminUsers_Email");

            migrationBuilder.RenameIndex(
                name: "ix_adminusers_adminusername",
                table: "AdminUsers",
                newName: "IX_AdminUsers_AdminUserName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantSubscriptions",
                table: "TenantSubscriptions",
                column: "TenantSubscriptionsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants",
                column: "TenantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemSettings",
                table: "SystemSettings",
                column: "SettingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionPlans",
                table: "SubscriptionPlans",
                column: "PlanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailTemplates",
                table: "EmailTemplates",
                column: "TemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdminUsers",
                table: "AdminUsers",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_TenantSubscriptions_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId",
                principalTable: "TenantSubscriptions",
                principalColumn: "TenantSubscriptionsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Tenants_TenantId",
                table: "Payments",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_AdminUsers_CreatedBy",
                table: "Tenants",
                column: "CreatedBy",
                principalTable: "AdminUsers",
                principalColumn: "AdminId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSubscriptions_SubscriptionPlans_PlanId",
                table: "TenantSubscriptions",
                column: "PlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "PlanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSubscriptions_Tenants_TenantId",
                table: "TenantSubscriptions",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
