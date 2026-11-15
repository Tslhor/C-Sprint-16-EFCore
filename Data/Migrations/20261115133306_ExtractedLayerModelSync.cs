using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtractedLayerModelSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_SuperMarkets_SupermarketId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_Price_Positive",
                table: "Products",
                sql: "Price > 0");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId_ProductId",
                table: "OrderDetails",
                columns: new[] { "OrderId", "ProductId" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderDetails_Quantity_Positive",
                table: "OrderDetails",
                sql: "Quantity > 0");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_SuperMarkets_SupermarketId",
                table: "Orders",
                column: "SupermarketId",
                principalTable: "SuperMarkets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_SuperMarkets_SupermarketId",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_Price_Positive",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderId_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderDetails_Quantity_Positive",
                table: "OrderDetails");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_SuperMarkets_SupermarketId",
                table: "Orders",
                column: "SupermarketId",
                principalTable: "SuperMarkets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}


