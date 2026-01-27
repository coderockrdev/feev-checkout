using System;
using System.Text.Json;
using FeevCheckout.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                    CallbackUrl = table.Column<string>(type: "text", nullable: false),
                    SuccessfulPaymentAttemptId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExpireAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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

            migrationBuilder.InsertData(
                table: "CardBrandPatterns",
                columns: new[] { "Id", "Brand", "Order", "Prefix" },
                values: new object[,]
                {
                    { new Guid("003ca013-ebc2-4cae-b9bb-8a36f2a62b5d"), "DISCOVER", 70, "6223" },
                    { new Guid("02368803-b488-46d3-8a05-8bc17394df4e"), "ELO", 10, "509045" },
                    { new Guid("0372d39d-ff94-464e-821a-f0265cfed04b"), "MASTER", 0, "5379" },
                    { new Guid("0550c691-6bbb-4c0f-9eac-7372e1ad6d23"), "ELO", 0, "509410" },
                    { new Guid("0597e697-a01c-478a-bc42-365c4135cfe8"), "ELO", 10, "50904" },
                    { new Guid("05b1ce54-7f86-48f5-872e-c6ae7ba735d7"), "AMEX", 40, "34" },
                    { new Guid("074d352d-e808-4ec6-b4ce-91fcd5bc478d"), "MASTER", 20, "2224" },
                    { new Guid("0827d174-1cb2-4be4-a1c7-ab21ee846115"), "ELO", 10, "409835" },
                    { new Guid("0da4373c-2933-4980-96a2-4c9f08169aa1"), "ELO", 5, "6516" },
                    { new Guid("0e86b520-54f9-4d44-8b0f-2309416551f3"), "ELO", 10, "438935" },
                    { new Guid("0f532812-5a1c-4c0f-aafc-1d9623580336"), "MASTER", 0, "615292" },
                    { new Guid("0f80fd08-8aa0-4b8d-b8f6-86805829e0c3"), "MASTER", 0, "576292" },
                    { new Guid("112075dc-afda-42dc-82ff-34952749cd2d"), "MASTER", 0, "3006" },
                    { new Guid("1217e59d-8ba1-4f0a-844c-d93bcea0acc4"), "MASTER", 0, "596497" },
                    { new Guid("12512908-6dbd-4598-b932-355c0eb3a025"), "MASTER", 20, "2221" },
                    { new Guid("13f031e0-e546-46fe-8474-865a009a832c"), "DINERS", 30, "300" },
                    { new Guid("140002b3-939a-4e14-a393-e0f2f0b95d98"), "DISCOVER", 70, "62214" },
                    { new Guid("140283ed-f263-4085-9ba7-de36ddeb73f4"), "MASTER", 20, "2226" },
                    { new Guid("149f3dad-3f31-4a3d-beeb-027bbd93998f"), "MASTER", 0, "505381" },
                    { new Guid("14d5c5ec-4aa8-46ff-a797-f27b98c93014"), "DINERS", 30, "301" },
                    { new Guid("14d8a98e-9eee-46d9-b1ae-c73fdfc56469"), "ELO", 0, "650920" },
                    { new Guid("1845dd8f-0bda-4c48-a464-dbe98ec54d18"), "MASTER", 20, "52" },
                    { new Guid("193bd82e-3cb3-45d9-9256-f2a51e425699"), "ELO", 10, "509059" },
                    { new Guid("1998ec6e-986d-494e-a0ef-8c1f43d1e992"), "MASTER", 0, "316811" },
                    { new Guid("1a4bdc97-6389-4560-a145-64e133e78642"), "ELO", 0, "509294" },
                    { new Guid("1c5c003c-7bb0-489a-8fbb-11f10a63559b"), "VISA", 0, "960421" },
                    { new Guid("1c5de06c-faef-4e5e-adca-1f599fd9ba95"), "MASTER", 20, "223" },
                    { new Guid("1eb5c78f-a23b-4f3c-b495-90ebb49579c5"), "ELO", 10, "36297" },
                    { new Guid("1eefb3ff-b14b-4729-8f2d-7a69f7a40d80"), "ELO", 5, "6280" },
                    { new Guid("1f3b8ff6-b8dc-4731-ad0c-7bac237ccdd3"), "ELO", 0, "6538" },
                    { new Guid("1f5ed430-5e2a-453e-8b3c-1cfd120a9217"), "ELO", 1, "509274" },
                    { new Guid("204dbfc7-a5bf-453c-90a1-82ebb5be8f36"), "ELO", 0, "509010" },
                    { new Guid("20554144-3fdb-47a1-b373-87218211e503"), "AMEX", 40, "37" },
                    { new Guid("207794f8-ed91-4007-814c-07e088613ff4"), "DISCOVER", 70, "6228" },
                    { new Guid("20bb2330-e2fe-4938-8563-1d695ba18fec"), "ELO", 10, "509000" },
                    { new Guid("20e7fcff-de40-4abd-88ce-bef52b9b1c8a"), "MASTER", 0, "509059" },
                    { new Guid("2131f038-e60f-4eb1-a1fe-533c6cbc3bfb"), "ELO", 10, "636368" },
                    { new Guid("23bb3055-647d-4bc3-b838-98a2d46e2119"), "AMEX", 40, "35" },
                    { new Guid("23c30aeb-719f-41c5-94c2-0d0ea7c91118"), "ELO", 0, "509284" },
                    { new Guid("245fb9ef-0be9-4fb9-8808-593045dc424a"), "MASTER", 0, "212743" },
                    { new Guid("258f3368-0550-4c4c-bbe2-7bc5999a7eca"), "VISA", 0, "984428" },
                    { new Guid("2784b6e5-734c-4490-9287-2b1ab6a42ad4"), "DISCOVER", 70, "622925" },
                    { new Guid("2868f315-bb0e-404d-b43e-fd19679f9682"), "MASTER", 1, "515894" },
                    { new Guid("2a8d4bdb-a301-4d50-91f4-9a03d8a28cd7"), "DISCOVER", 70, "622129" },
                    { new Guid("2ab53a7f-4b66-43a6-bebe-4c0d20097ab3"), "MASTER", 0, "44027" },
                    { new Guid("2cf4364f-e126-4f21-a891-f66445809dba"), "DISCOVER", 70, "648" },
                    { new Guid("2e2944ec-61a9-4a59-ba3c-fcb34ccb2627"), "ELO", 10, "650507" },
                    { new Guid("2e9c2a21-33af-4fe9-821f-52358f5027b6"), "MASTER", 0, "830050" },
                    { new Guid("2ee19789-b5a0-46f8-a9c1-bb3264bcd5a1"), "MASTER", 20, "226" },
                    { new Guid("2f301774-4bb8-4a19-8205-311247e6484d"), "MASTER", 0, "570209" },
                    { new Guid("3190514e-4e2f-4f2c-97b6-fb064bab71e7"), "ELO", 10, "4576" },
                    { new Guid("357452f7-5187-4037-a2f3-68726506fbf6"), "DISCOVER", 70, "649" },
                    { new Guid("3905d34f-4f18-45de-a214-85744d446cf3"), "ELO", 10, "509074" },
                    { new Guid("392bb78d-e5eb-4853-90ec-c12ae199c3db"), "ELO", 1, "650914" },
                    { new Guid("39c25b53-3bb8-4fae-bba5-78e4344b7985"), "MASTER", 0, "502121" },
                    { new Guid("39c58b4c-4194-4e43-9807-bb2045b1c552"), "ELO", 5, "6504" },
                    { new Guid("39d68950-193c-41fa-a6cb-cd98021d51ca"), "VISA", 0, "798431" },
                    { new Guid("3a82b5c4-2153-4e87-bd08-4fa85bfdca9f"), "MASTER", 0, "589916" },
                    { new Guid("3b2b4695-4f51-4a66-975c-4be5f9c742fe"), "MASTER", 20, "26" },
                    { new Guid("3b612e2f-0b55-49c7-bb4e-1745dcb1d52a"), "ELO", 10, "650485" },
                    { new Guid("40c2ce8b-6856-4809-b27b-4fd9817b34da"), "DISCOVER", 70, "646" },
                    { new Guid("423f9d1c-1874-49b4-8d4e-0f941f7c7505"), "DINERS", 30, "302" },
                    { new Guid("426aa0fe-1a19-4ce7-9049-28ab78121ee3"), "DISCOVER", 70, "62215" },
                    { new Guid("42b59229-509f-4a24-aed1-983583429b6d"), "ELO", 10, "509064" },
                    { new Guid("42baf655-d570-435d-b789-4cd57d7c4981"), "ELO", 10, "451416" },
                    { new Guid("42c7b9ce-7917-48b3-a18b-091f095d9cc3"), "VISA", 0, "854644" },
                    { new Guid("42e3cc17-8d0f-44e8-a966-2a3ffc049a23"), "DISCOVER", 70, "6225" },
                    { new Guid("431e24fb-a724-49b7-a0bf-f183015b6de4"), "MASTER", 0, "637095" },
                    { new Guid("44073e7c-0920-4616-8c37-5707d2837c24"), "MASTER", 20, "53" },
                    { new Guid("45394277-6bfd-41ec-9e30-9027e083bfde"), "AMEX", 40, "36" },
                    { new Guid("45e6bb9c-04c5-4409-9352-b392e9148ee4"), "VISA", 0, "560780" },
                    { new Guid("47ff6a78-5958-4830-88a7-ad74f0dda9df"), "ELO", 0, "508757" },
                    { new Guid("494e8a2f-e9d9-4bfd-9374-aefa53548401"), "ELO", 0, "509156" },
                    { new Guid("4befd7bb-618f-4b65-956e-5b6833411015"), "DISCOVER", 70, "62217" },
                    { new Guid("4c2b3fd1-7631-4076-9ede-330372fbafb7"), "ELO", 10, "509040" },
                    { new Guid("4c6cb400-e78a-48b1-b056-9d21486d9478"), "MASTER", 0, "585988" },
                    { new Guid("4f5b783e-e628-4832-8f6d-a2494631c0ea"), "ELO", 10, "650912" },
                    { new Guid("4f917698-6f82-4dae-bec0-2b01668baeda"), "ELO", 0, "680405" },
                    { new Guid("5076f15a-f83c-4f4a-adcb-e3b7e5c0dc3d"), "VISA", 5, "4984" },
                    { new Guid("50baf0cd-e00e-4cdc-bd3d-ba09f98497a4"), "MASTER", 0, "595922" },
                    { new Guid("50e71794-c759-4eb1-921c-2fae18d088cb"), "MASTER", 0, "576096" },
                    { new Guid("5353880a-8720-4369-a0be-19955332d311"), "MASTER", 20, "2227" },
                    { new Guid("58920d2c-44e7-48de-8d60-bcd591d70145"), "MASTER", 0, "7807" },
                    { new Guid("58f3277e-2b4e-41d6-a2c6-70ba30680c8e"), "DISCOVER", 70, "6227" },
                    { new Guid("5a1ff8c2-2ace-40f8-9a7d-026008c4db5d"), "DISCOVER", 70, "62218" },
                    { new Guid("5c3d0094-37c0-476a-91a5-8d53ace96adb"), "ELO", 10, "509049" },
                    { new Guid("5cd6bec0-c70f-44c1-92b4-70e8eb6a862c"), "ELO", 10, "650494" },
                    { new Guid("5d2299c5-a37c-43dc-a932-63fcdb3d2298"), "MASTER", 0, "330239" },
                    { new Guid("5f2cdf81-7c20-4915-9fc6-940ae81122bf"), "MASTER", 0, "126820" },
                    { new Guid("5ff0e16e-fbd2-492f-9857-d4d1ea1068af"), "ELO", 10, "65090" },
                    { new Guid("5ffc17d1-e0aa-4753-be75-4cf098220a85"), "ELO", 10, "50906" },
                    { new Guid("615b54ee-0fa5-4a9d-83df-371386bd03bf"), "MASTER", 20, "224" },
                    { new Guid("6167a90a-fd65-4279-937a-cbe6a1766f43"), "ELO", 0, "67410" },
                    { new Guid("620c8e49-e384-4fa4-8c4b-77616844a0cb"), "ELO", 10, "509091" },
                    { new Guid("635c31f3-bbc8-4fc0-a1a9-16b568b327c3"), "MASTER", 0, "720358" },
                    { new Guid("6497a227-ec37-44d3-a77a-bcb2cc80f332"), "MASTER", 0, "546479" },
                    { new Guid("68448fcb-c813-403b-a302-4ced63a34a82"), "VISA", 0, "33994" },
                    { new Guid("6b7c2cb6-4ed9-4720-9161-9cbe0be9b07f"), "DISCOVER", 70, "622127" },
                    { new Guid("6bddf73a-c0ff-4dfe-93a0-6e743b2f96a0"), "MASTER", 0, "517349" },
                    { new Guid("6c29bf88-0f06-4ce0-8a58-189979b96f1c"), "MASTER", 0, "505316" },
                    { new Guid("6cd62b8d-21bf-4435-99b9-f26c611eab84"), "MASTER", 0, "502309" },
                    { new Guid("6d0b34de-32b6-411f-97c9-5897f208c451"), "MASTER", 0, "537993" },
                    { new Guid("6dbafbd3-7214-4766-a31c-35a276123b86"), "VISA", 0, "970800" },
                    { new Guid("6e104ced-1dcb-4b88-9daf-89a103d5b994"), "DISCOVER", 70, "62290" },
                    { new Guid("7083e8bd-dc7f-4db2-80fb-a690617a1aac"), "ELO", 1, "6507" },
                    { new Guid("7162b706-ed8b-4481-8feb-c997dd184238"), "MASTER", 20, "54" },
                    { new Guid("7218e237-a417-4d28-8d39-f0425b00d001"), "ELO", 0, "506874" },
                    { new Guid("74b58e28-2558-49db-bfa2-5e120a91f5cd"), "ELO", 0, "509093" },
                    { new Guid("7603940e-9c51-45cd-9a29-4428e9e9db09"), "ELO", 0, "699988" },
                    { new Guid("768dc1ca-1b4d-4f80-ac3b-a000482fef37"), "VISA", 0, "566269" },
                    { new Guid("780d7b75-9d66-4cf0-ae4a-03a79fcb01aa"), "ELO", 10, "509050" },
                    { new Guid("7a1edf38-4cfb-482c-b10e-31100dc3967f"), "ELO", 0, "650042" },
                    { new Guid("7a5ce568-cb4c-4975-ae5d-8eb49489b3f7"), "ELO", 5, "6505" },
                    { new Guid("7b2bbd5a-fb11-46b4-b74f-5b5291eab350"), "VISA", 0, "27728" },
                    { new Guid("7becaa1e-4c35-453d-b142-2df2a6802002"), "HIPERCARD", 0, "506282" },
                    { new Guid("7e4af5f4-a029-447e-9fcb-90befde26747"), "DISCOVER", 70, "622920" },
                    { new Guid("7ee5c159-16d2-4cd8-bed6-b8422e56b655"), "MASTER", 20, "2222" },
                    { new Guid("80729eec-ea75-4613-a720-4f0e773dbad2"), "ELO", 10, "4011" },
                    { new Guid("80d44c56-7c25-42da-9fe2-9dfd81b18eb2"), "VISA", 0, "502121" },
                    { new Guid("81ee61f1-ecb5-4f1c-8388-3bfc7a0d560a"), "MASTER", 0, "960379" },
                    { new Guid("8226d599-c8b7-4bad-9c7a-8cb428f47a4f"), "MASTER", 0, "569970" },
                    { new Guid("824c64db-22d4-40bd-8358-54024dc07f14"), "MASTER", 20, "227" },
                    { new Guid("82e0bb07-94a8-4671-9798-41c62fcde07c"), "MASTER", 20, "225" },
                    { new Guid("83dc1d02-efc9-476f-91fe-41554f21ca74"), "ELO", 10, "509051" },
                    { new Guid("84f534bd-ab60-4117-8983-4f68cf4d2d28"), "MASTER", 0, "53508" },
                    { new Guid("85904af7-a34f-4f11-a38d-0a7c5286f1b1"), "ELO", 10, "650491" },
                    { new Guid("86f23be4-eb81-49dc-b4f7-90338ad348bf"), "DISCOVER", 70, "6224" },
                    { new Guid("889d3111-6180-48db-ba5b-835e7f960db6"), "ELO", 10, "5067" },
                    { new Guid("896a25f1-7d39-446e-8a9f-548160a2adb1"), "DISCOVER", 70, "622128" },
                    { new Guid("898e9f08-8dbb-4b5e-a9f7-9333123f9733"), "ELO", 0, "509730" },
                    { new Guid("8a819351-27e3-4991-9fa7-b5e9381fdd13"), "MASTER", 20, "2228" },
                    { new Guid("8bbb1da9-76fa-446d-83c6-238912cc3b98"), "ELO", 10, "504175" },
                    { new Guid("8c19b8b9-896e-4f8c-872a-72d381f2c9c0"), "ELO", 0, "627780" },
                    { new Guid("8c84b351-aaf5-445d-9c21-c2264485b611"), "ELO", 10, "650921" },
                    { new Guid("8cfab46d-5200-4d2a-9d10-5a3c1c493eff"), "DISCOVER", 70, "62219" },
                    { new Guid("8e8a1f88-d6f7-46a2-9ea6-6f6681df13b1"), "VISA", 0, "40064" },
                    { new Guid("90c0f4f3-7387-4de7-9d92-922c73bb6bcd"), "MASTER", 20, "2229" },
                    { new Guid("91055709-76f9-4ac8-b291-3b91af7a76a5"), "VISA", 0, "636355" },
                    { new Guid("91946c7e-123c-49d1-874b-e390b3396a5c"), "HIPERCARD", 80, "3841" },
                    { new Guid("93e7517c-4a78-4c41-b37c-44f2e119eaa9"), "ELO", 10, "650487" },
                    { new Guid("946120b1-2232-45de-9ad1-22f9cccc0c96"), "VISA", 110, "4" },
                    { new Guid("951f5097-ceeb-4a11-9179-b12141e0fa50"), "MASTER", 20, "24" },
                    { new Guid("965b7f59-f2ba-4eba-bf22-a1a07714cd88"), "ELO", 10, "509042" },
                    { new Guid("9af7c82d-2d36-47b2-8933-a8620e427b0c"), "DISCOVER", 70, "622126" },
                    { new Guid("9e4bd89d-eba3-4a56-bab8-1ff80bc1e300"), "MASTER", 20, "2223" },
                    { new Guid("a04e48e4-3fa4-4964-85e6-bc797307c615"), "DINERS", 30, "305" },
                    { new Guid("a1992f42-cc76-45a5-ac52-953b67156c9f"), "MASTER", 20, "51" },
                    { new Guid("a2f1cd02-22e0-4840-bdd8-221413dd56d4"), "MASTER", 20, "23" },
                    { new Guid("a4ff758d-2979-4cda-91c4-bb1b3b0c97b3"), "MASTER", 0, "5453" },
                    { new Guid("a53b134e-d42d-4142-9921-2034417563b9"), "DISCOVER", 70, "622923" },
                    { new Guid("a9dd2458-4140-4a0c-98d7-e98980316a7d"), "ELO", 0, "509004" },
                    { new Guid("aabad584-d0c9-42ee-881b-7e663819986b"), "DISCOVER", 70, "6011" },
                    { new Guid("aaf21de7-a469-4328-a35d-9658d917cf89"), "MASTER", 1, "608783" },
                    { new Guid("ad552afb-bd64-4c5d-9b3f-6f81eefc1ba7"), "VISA", 0, "121772" },
                    { new Guid("ad7ecacd-10f4-425b-87bf-982b44d5e06f"), "MASTER", 0, "55909" },
                    { new Guid("ae290d4c-bd22-4c3c-9f59-98f402c24cd8"), "ELO", 10, "650905" },
                    { new Guid("ae849b51-ea12-4d43-86a0-8c6d9119ed7e"), "ELO", 10, "509043" },
                    { new Guid("b12920c0-8282-4691-9948-db61546c5ca2"), "VISA", 0, "627892" },
                    { new Guid("b2932bde-922a-45fa-b73a-b97ce915a15d"), "MASTER", 0, "220209" },
                    { new Guid("b3772fad-3bd7-4891-a118-9ebf270a1d6b"), "ELO", 10, "509067" },
                    { new Guid("b382fdd2-e26f-4aec-826f-089d45147836"), "MASTER", 1, "750209" },
                    { new Guid("b6c38a2b-d006-4fad-8090-c21a6ece57d9"), "MASTER", 0, "560033" },
                    { new Guid("b87c558c-c996-4050-81a0-a8570910048c"), "DINERS", 30, "303" },
                    { new Guid("b91f93d7-92b5-4241-86e1-e4344782c8bc"), "ELO", 10, "509046" },
                    { new Guid("b96529a8-0fc1-4cb0-88f6-cba42b19a2c1"), "MASTER", 0, "502299" },
                    { new Guid("b9bd0699-7af0-48d8-8f14-915be884ae13"), "DINERS", 30, "36" },
                    { new Guid("b9edfac7-ab54-4b4f-88ff-f753e956a03b"), "DISCOVER", 70, "622924" },
                    { new Guid("ba1de02c-3a8a-4294-9164-2ac70fe406ea"), "ELO", 5, "509023" },
                    { new Guid("ba5f0b56-4b59-4d63-a5f5-43e133e71156"), "MASTER", 20, "2720" },
                    { new Guid("bb4178f3-5243-4d2e-9695-096f45ac2259"), "ELO", 4, "506741" },
                    { new Guid("bc8cfa6f-cfaf-48f1-8b06-4aa9a44e6eab"), "NARANJA E NEVADA", 100, "589562" },
                    { new Guid("bd97ec5c-9e84-4722-84e6-19a2da4756d7"), "MASTER", 0, "560780" },
                    { new Guid("be18e58a-648a-47c0-a47d-3c252ea6b56d"), "ELO", 0, "502577" },
                    { new Guid("be993bbb-927d-4fe6-9e47-6969b920286b"), "HIPERCARD", 80, "606282" },
                    { new Guid("bfd09fec-9f46-47fa-95a4-c00f0d63d3d4"), "DISCOVER", 1, "306262" },
                    { new Guid("c0ba026e-7d79-4943-ae65-edf14f5a72f7"), "ELO", 10, "50904" },
                    { new Guid("c11cca32-ed79-44d8-b42e-11cf9873e42a"), "DINERS", 0, "605919" },
                    { new Guid("c37a1ba3-aeb5-4cce-86f2-dc00ae664678"), "DINERS", 30, "304" },
                    { new Guid("c3a0f9c7-7173-4963-8d16-05247462fa89"), "ELO", 5, "6550" },
                    { new Guid("c4df1f22-e790-4948-a819-abeaa7693ebc"), "MASTER", 0, "608710" },
                    { new Guid("c54e18b5-73fa-4aae-962b-50225e2957f6"), "MASTER", 20, "2225" },
                    { new Guid("c790b10b-0b6f-466b-a586-889ea5f4381c"), "ELO", 0, "6509" },
                    { new Guid("c8b91ff0-d28d-43cb-961c-87d1f79970a0"), "ELO", 0, "509423" },
                    { new Guid("c95dab81-a5bf-471a-97af-0fd7f670fbab"), "DISCOVER", 70, "647" },
                    { new Guid("c97bafe1-1412-42dc-961b-564ba4a92ce4"), "ELO", 10, "509040" },
                    { new Guid("ca16b818-3920-418b-aa9c-116efef91682"), "DISCOVER", 70, "645" },
                    { new Guid("cf2c69ac-c01e-445a-8455-ac6dbd84bf4f"), "ELO", 0, "111111" },
                    { new Guid("cfeea6d1-8f95-4751-ba5c-8d8ff9a09609"), "ELO", 0, "509410" },
                    { new Guid("d0dbd78b-3c16-498c-af71-b532c220a4e3"), "ELO", 0, "640485" },
                    { new Guid("d205370b-7c51-479c-8380-04072fb0f30f"), "MASTER", 0, "566269" },
                    { new Guid("d2b5be9e-ac4c-4218-8ba5-73a4ccbd14ec"), "JCB", 90, "2131" },
                    { new Guid("d3814d80-1b06-438d-b14e-97f3123a82bd"), "VISA", 0, "444125" },
                    { new Guid("d3a67c1c-ad20-47e4-9dfe-5f0908c364af"), "DISCOVER", 70, "65" },
                    { new Guid("d3c8c7e1-14e2-4a6b-aad8-4532e16e5dab"), "MASTER", 0, "439133" },
                    { new Guid("d3eec0c4-ad64-43c7-b752-ecb7460c8ad8"), "VISA", 0, "48546" },
                    { new Guid("d5086915-b137-4acf-81d7-264d65d6f1cb"), "DISCOVER", 70, "62213" },
                    { new Guid("d69871e3-1697-4c7f-8424-5476351f781f"), "MASTER", 20, "55" },
                    { new Guid("d891674c-4039-4724-aaa2-682c10238bf3"), "MASTER", 0, "970800" },
                    { new Guid("da0d3bed-181a-4201-98cb-326ec5668e4e"), "MASTER", 20, "228" },
                    { new Guid("da549a33-686a-4aa8-89b1-f08d3095a72c"), "MASTER", 0, "574080" },
                    { new Guid("da73bb3a-939e-4db3-8727-78e0465ffc4a"), "HIPERCARD", 60, "38" },
                    { new Guid("db597607-4b52-4fc8-8be7-9eabcb0fdb4a"), "MASTER", 0, "639350" },
                    { new Guid("dbabbfe6-3782-409a-a08b-5d187fd67642"), "MASTER", 0, "320132" },
                    { new Guid("ddcaba72-bc88-4327-ae2c-7d796a82ddee"), "CABAL", 60, "6042" },
                    { new Guid("dffddc29-0ffb-4b4d-8904-7826e9764855"), "VISA", 0, "107915" },
                    { new Guid("e07e64cb-3102-4f96-bbec-46e53c5bb156"), "VISA", 0, "985966" },
                    { new Guid("e0ae42d4-ec5d-4f2c-a082-574aa625b8e0"), "MASTER", 0, "507641" },
                    { new Guid("e0e0d7e8-9047-4ccf-b293-704fd67a2288"), "JCB", 90, "35" },
                    { new Guid("e1e5b16c-3742-4e1b-8828-80a98a7eef77"), "MASTER", 0, "509257" },
                    { new Guid("e34700a3-d5ac-4d77-9973-8bc42762c4bf"), "VISA", 0, "210790" },
                    { new Guid("e3d29cc4-6253-4e29-8281-da32f81e1d8c"), "ELO", 0, "509005" },
                    { new Guid("e672dfd9-ff77-416b-a21c-9fb688802a70"), "MASTER", 20, "25" },
                    { new Guid("eadb7f44-8da5-41cd-b349-bb05c9c65a74"), "VISA", 0, "988442" },
                    { new Guid("ed736c5e-ff2d-4d13-85ce-4675a966c49e"), "ELO", 10, "509069" },
                    { new Guid("ee4aa599-02f3-4465-bc88-207538f6d27e"), "DISCOVER", 70, "622921" },
                    { new Guid("eefc0690-c626-4986-99dd-1f6dbfc4bac1"), "ELO", 10, "509052" },
                    { new Guid("ef69207b-8e29-4732-b169-705138eeb9ec"), "DISCOVER", 70, "6226" },
                    { new Guid("f0f21d1b-0dd8-470b-9989-49d4ad351640"), "DISCOVER", 70, "6222" },
                    { new Guid("f378f748-162b-4f50-83e7-19d7ffef8742"), "DISCOVER", 70, "62216" },
                    { new Guid("f623d679-b511-49f1-9341-8d94955b920b"), "MASTER", 20, "229" },
                    { new Guid("f6570cca-3017-4daf-9624-e7f3add45aec"), "DISCOVER", 70, "62291" },
                    { new Guid("f7db8949-c05d-4b17-a2e7-7ed85b24b67c"), "MASTER", 0, "960397" },
                    { new Guid("f80c6c6d-e821-4dd7-a0f1-cc998f47bb3a"), "MASTER", 0, "640245" },
                    { new Guid("f868c53f-55dd-416b-91ee-ff0640f3e2f6"), "JCB", 90, "1800" },
                    { new Guid("f8fa036f-3ee6-4325-96b8-a7f639be3134"), "DISCOVER", 70, "622922" },
                    { new Guid("f97b52fe-4334-4c50-a65f-e82cf5254c32"), "VISA", 0, "186307" },
                    { new Guid("fa8dc73a-af9f-4a73-bc1e-42e387e901e1"), "DISCOVER", 70, "644" },
                    { new Guid("fcbf8323-69fd-4eca-97a6-eae5c3938e64"), "MASTER", 20, "271" },
                    { new Guid("fe26c2f6-f87f-40a4-bc46-f0ed73a90e9f"), "ELO", 10, "509066" }
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
