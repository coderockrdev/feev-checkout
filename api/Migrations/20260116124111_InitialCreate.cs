using System;
using System.Text.Json;
using FeevCheckout.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeevCheckout.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:payment_attempt_status", "pending,created,completed,failed")
                .Annotation("Npgsql:Enum:payment_method", "feev_pix,feev_boleto,braspag_cartao");

            migrationBuilder.CreateTable(
                name: "CardBrandPatterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    Prefix = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardBrandPatterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Establishments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    CNPJ = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    BankNumber = table.Column<string>(type: "text", nullable: true),
                    BankAgency = table.Column<string>(type: "text", nullable: true),
                    BankAccount = table.Column<string>(type: "text", nullable: true),
                    CheckingAccountNumber = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    ClientSecret = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establishments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Credentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstablishmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<PaymentMethod>(type: "payment_method", nullable: false),
                    Data = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    BraspagProvider = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Credentials_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstablishmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<PaymentMethod>(type: "payment_method", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<PaymentAttemptStatus>(type: "payment_attempt_status", nullable: false),
                    ExtraData = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    Response = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstablishmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CustomerName = table.Column<string>(type: "text", nullable: false),
                    CustomerDocument = table.Column<string>(type: "text", nullable: false),
                    CustomerEmail = table.Column<string>(type: "text", nullable: false),
                    CustomerAddressStreet = table.Column<string>(type: "text", nullable: false),
                    Customer_Address_Number = table.Column<string>(type: "text", nullable: false),
                    Customer_Address_Complement = table.Column<string>(type: "text", nullable: true),
                    Customer_Address_District = table.Column<string>(type: "text", nullable: false),
                    CustomerAddressCity = table.Column<string>(type: "text", nullable: false),
                    CustomerAddressUF = table.Column<string>(type: "text", nullable: false),
                    CustomerAddressPostalCode = table.Column<string>(type: "text", nullable: false),
                    TotalAmount = table.Column<int>(type: "integer", nullable: false),
                    PaymentRules = table.Column<string>(type: "jsonb", nullable: false),
                    SuccessfulPaymentAttemptId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExpireAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CallbackUrl = table.Column<string>(type: "text", nullable: false),
                    CanceledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_PaymentAttempts_SuccessfulPaymentAttemptId",
                        column: x => x.SuccessfulPaymentAttemptId,
                        principalTable: "PaymentAttempts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_EstablishmentId",
                table: "Credentials",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_EstablishmentId",
                table: "PaymentAttempts",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_TransactionId",
                table: "PaymentAttempts",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TransactionId",
                table: "Products",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EstablishmentId",
                table: "Transactions",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SuccessfulPaymentAttemptId",
                table: "Transactions",
                column: "SuccessfulPaymentAttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentAttempts_Transactions_TransactionId",
                table: "PaymentAttempts",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAttempts_Establishments_EstablishmentId",
                table: "PaymentAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Establishments_EstablishmentId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAttempts_Transactions_TransactionId",
                table: "PaymentAttempts");

            migrationBuilder.DropTable(
                name: "CardBrandPatterns");

            migrationBuilder.DropTable(
                name: "Credentials");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Establishments");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "PaymentAttempts");
        }
    }
}
