using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnicalTest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCustometToTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTransfer_Customers_CustomerId",
                table: "AccountTransfer");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "AccountTransfer",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTransfer_Customers_CustomerId",
                table: "AccountTransfer",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTransfer_Customers_CustomerId",
                table: "AccountTransfer");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "AccountTransfer",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTransfer_Customers_CustomerId",
                table: "AccountTransfer",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
