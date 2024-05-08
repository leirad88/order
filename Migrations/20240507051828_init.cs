using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace acme.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Street = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Pack = table.Column<string>(type: "TEXT", nullable: false),
                    Uom = table.Column<string>(type: "TEXT", nullable: false),
                    Comments = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false),
                    Total = table.Column<double>(type: "REAL", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    DepartmentName = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: true),
                    DateActive = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateDeactive = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Priority = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportedOrder",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    OrderNumber = table.Column<string>(type: "TEXT", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    ShippingMethod = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedDeliveryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ShippingAddressId = table.Column<string>(type: "TEXT", nullable: true),
                    BillingAddressId = table.Column<string>(type: "TEXT", nullable: true),
                    Subtotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    ShippingHandling = table.Column<decimal>(type: "TEXT", nullable: false),
                    Tax = table.Column<decimal>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportedOrder_Address_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImportedOrder_Address_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderLine",
                columns: table => new
                {
                    OrderLineId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    PricePerUnit = table.Column<double>(type: "REAL", nullable: false),
                    Total = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLine", x => x.OrderLineId);
                    table.ForeignKey(
                        name: "FK_OrderLine_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderLine_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportedOrderItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerNumber = table.Column<string>(type: "TEXT", nullable: false),
                    DistributionNumber = table.Column<string>(type: "TEXT", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    UnitType = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Subtotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    ImportedOrderId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedOrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportedOrderItem_ImportedOrder_ImportedOrderId",
                        column: x => x.ImportedOrderId,
                        principalTable: "ImportedOrder",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportedOrder_BillingAddressId",
                table: "ImportedOrder",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportedOrder_ShippingAddressId",
                table: "ImportedOrder",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportedOrderItem_ImportedOrderId",
                table: "ImportedOrderItem",
                column: "ImportedOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLine_OrderId",
                table: "OrderLine",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLine_ProductId",
                table: "OrderLine",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportedOrderItem");

            migrationBuilder.DropTable(
                name: "OrderLine");

            migrationBuilder.DropTable(
                name: "ImportedOrder");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
