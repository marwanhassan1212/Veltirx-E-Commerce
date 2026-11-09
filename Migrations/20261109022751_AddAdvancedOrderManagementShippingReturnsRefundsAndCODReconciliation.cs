using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veltrix.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedOrderManagementShippingReturnsRefundsAndCODReconciliation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerNotes",
                table: "Orders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancelledQuantity",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FulfilledQuantity",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VariantName",
                table: "OrderItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VariantOptionsSnapshot",
                table: "OrderItems",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CodReconciliations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReconciliationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpectedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CollectedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DifferenceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReconciledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReconciledByUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodReconciliations", x => x.Id);
                    table.CheckConstraint("CK_CodReconciliations_Amounts", "[ExpectedAmount] >= 0 AND [CollectedAmount] >= 0");
                    table.CheckConstraint("CK_CodReconciliations_Difference", "[DifferenceAmount] = [CollectedAmount] - [ExpectedAmount]");
                    table.ForeignKey(
                        name: "FK_CodReconciliations_Users_ReconciledByUserId",
                        column: x => x.ReconciledByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderFulfillments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FulfillmentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFulfillments", x => x.Id);
                    table.CheckConstraint("CK_OrderFulfillments_CompletedAt", "[CompletedAt] IS NULL OR [CompletedAt] >= [CreatedAt]");
                    table.ForeignKey(
                        name: "FK_OrderFulfillments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderReturns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CustomerNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderReturns", x => x.Id);
                    table.CheckConstraint("CK_OrderReturns_ApprovedAt", "[ApprovedAt] IS NULL OR [ApprovedAt] >= [RequestedAt]");
                    table.CheckConstraint("CK_OrderReturns_CompletedAt", "[CompletedAt] IS NULL OR [CompletedAt] >= [RequestedAt]");
                    table.CheckConstraint("CK_OrderReturns_ReceivedAt", "[ReceivedAt] IS NULL OR [ReceivedAt] >= [RequestedAt]");
                    table.ForeignKey(
                        name: "FK_OrderReturns_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackingNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CarrierName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShippedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedDeliveryAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.CheckConstraint("CK_Shipments_DeliveredAt", "[DeliveredAt] IS NULL OR [ShippedAt] IS NOT NULL");
                    table.CheckConstraint("CK_Shipments_ShippedAt", "[ShippedAt] IS NULL OR [ShippedAt] >= [CreatedAt]");
                    table.ForeignKey(
                        name: "FK_Shipments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CodReconciliationItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpectedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CollectedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DifferenceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReconciliationId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodReconciliationItems", x => x.Id);
                    table.CheckConstraint("CK_CodReconciliationItems_Amounts", "[ExpectedAmount] >= 0 AND [CollectedAmount] >= 0");
                    table.CheckConstraint("CK_CodReconciliationItems_Difference", "[DifferenceAmount] = [CollectedAmount] - [ExpectedAmount]");
                    table.ForeignKey(
                        name: "FK_CodReconciliationItems_CodReconciliations_ReconciliationId",
                        column: x => x.ReconciliationId,
                        principalTable: "CodReconciliations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodReconciliationItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FulfillmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    FulfillmentId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FulfillmentItems", x => x.Id);
                    table.CheckConstraint("CK_FulfillmentItems_Quantity", "[Quantity] > 0");
                    table.ForeignKey(
                        name: "FK_FulfillmentItems_OrderFulfillments_FulfillmentId",
                        column: x => x.FulfillmentId,
                        principalTable: "OrderFulfillments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FulfillmentItems_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderReturnItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ConditionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReturnId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderReturnItems", x => x.Id);
                    table.CheckConstraint("CK_OrderReturnItems_Quantity", "[Quantity] > 0");
                    table.ForeignKey(
                        name: "FK_OrderReturnItems_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderReturnItems_OrderReturns_ReturnId",
                        column: x => x.ReturnId,
                        principalTable: "OrderReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefundNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReturnId = table.Column<int>(type: "int", nullable: false),
                    ProcessedByUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                    table.CheckConstraint("CK_Refunds_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_Refunds_ProcessedAt", "[ProcessedAt] IS NULL OR [ProcessedAt] >= [CreatedAt]");
                    table.ForeignKey(
                        name: "FK_Refunds_OrderReturns_ReturnId",
                        column: x => x.ReturnId,
                        principalTable: "OrderReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Users_ProcessedByUserId",
                        column: x => x.ProcessedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentTrackingEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShipmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentTrackingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentTrackingEvents_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItems_CancelledQuantity",
                table: "OrderItems",
                sql: "[CancelledQuantity] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItems_FulfilledQuantity",
                table: "OrderItems",
                sql: "[FulfilledQuantity] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItems_ProcessedQuantity",
                table: "OrderItems",
                sql: "[CancelledQuantity] + [FulfilledQuantity] <= [Quantity]");

            migrationBuilder.CreateIndex(
                name: "IX_CodReconciliationItems_OrderId",
                table: "CodReconciliationItems",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodReconciliationItems_ReconciliationId",
                table: "CodReconciliationItems",
                column: "ReconciliationId");

            migrationBuilder.CreateIndex(
                name: "IX_CodReconciliations_CreatedAt",
                table: "CodReconciliations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CodReconciliations_ReconciledByUserId",
                table: "CodReconciliations",
                column: "ReconciledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CodReconciliations_ReconciliationNumber",
                table: "CodReconciliations",
                column: "ReconciliationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodReconciliations_Status",
                table: "CodReconciliations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentItems_FulfillmentId_OrderItemId",
                table: "FulfillmentItems",
                columns: new[] { "FulfillmentId", "OrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentItems_OrderItemId",
                table: "FulfillmentItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFulfillments_FulfillmentNumber",
                table: "OrderFulfillments",
                column: "FulfillmentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderFulfillments_OrderId",
                table: "OrderFulfillments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFulfillments_Status",
                table: "OrderFulfillments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrderReturnItems_OrderItemId",
                table: "OrderReturnItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderReturnItems_ReturnId_OrderItemId",
                table: "OrderReturnItems",
                columns: new[] { "ReturnId", "OrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderReturns_OrderId",
                table: "OrderReturns",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderReturns_ReturnNumber",
                table: "OrderReturns",
                column: "ReturnNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderReturns_Status",
                table: "OrderReturns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_ProcessedByUserId",
                table: "Refunds",
                column: "ProcessedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_RefundNumber",
                table: "Refunds",
                column: "RefundNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_ReturnId",
                table: "Refunds",
                column: "ReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_Status",
                table: "Refunds",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OrderId",
                table: "Shipments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Status",
                table: "Shipments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_TrackingNumber",
                table: "Shipments",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTrackingEvents_ShipmentId_OccurredAt",
                table: "ShipmentTrackingEvents",
                columns: new[] { "ShipmentId", "OccurredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodReconciliationItems");

            migrationBuilder.DropTable(
                name: "FulfillmentItems");

            migrationBuilder.DropTable(
                name: "OrderReturnItems");

            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropTable(
                name: "ShipmentTrackingEvents");

            migrationBuilder.DropTable(
                name: "CodReconciliations");

            migrationBuilder.DropTable(
                name: "OrderFulfillments");

            migrationBuilder.DropTable(
                name: "OrderReturns");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItems_CancelledQuantity",
                table: "OrderItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItems_FulfilledQuantity",
                table: "OrderItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItems_ProcessedQuantity",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerNotes",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CancelledQuantity",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "FulfilledQuantity",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "VariantName",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "VariantOptionsSnapshot",
                table: "OrderItems");
        }
    }
}
