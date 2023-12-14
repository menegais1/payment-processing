using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentProcessing.Migrations
{
    /// <inheritdoc />
    public partial class add_organization_fk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Transactions",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OrganizationId",
                table: "Transactions",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Organizations_OrganizationId",
                table: "Transactions",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Organizations_OrganizationId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_OrganizationId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Transactions");
        }
    }
}
