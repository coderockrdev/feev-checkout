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
                    Domain = table.Column<string>(type: "text", nullable: false),
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
                    CustomerAddressNumber = table.Column<string>(type: "text", nullable: false),
                    CustomerAddressComplement = table.Column<string>(type: "text", nullable: true),
                    CustomerAddressDistrict = table.Column<string>(type: "text", nullable: false),
                    CustomerAddressCity = table.Column<string>(type: "text", nullable: false),
                    CustomerAddressState = table.Column<string>(type: "text", nullable: false),
                    CustomerAddressZipCode = table.Column<string>(type: "text", nullable: false),
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
                    { new Guid("02ae6e45-adc6-484a-968c-efcc817cf14b"), "MASTER", 1, "515894" },
                    { new Guid("03021c6a-ab74-4194-9008-ccc40b3454b2"), "HIPERCARD", 80, "606282" },
                    { new Guid("033d4215-1800-410c-b309-5020a550c571"), "ELO", 10, "509064" },
                    { new Guid("04029e09-c56a-4849-946b-cffb23e8dbb4"), "DINERS", 30, "300" },
                    { new Guid("068c0d0c-ff50-47bd-86ee-54ab4160b9fd"), "JCB", 90, "1800" },
                    { new Guid("07e5a965-a474-4686-974b-47c90b528c06"), "MASTER", 20, "52" },
                    { new Guid("08b48b51-5431-4a5c-9b4b-c18590e2505e"), "MASTER", 0, "007807" },
                    { new Guid("09c81bd9-0a53-49f3-af35-7c521df59943"), "ELO", 10, "509051" },
                    { new Guid("0a40107f-db62-41eb-a507-dd13a6c9dbfc"), "ELO", 0, "509423" },
                    { new Guid("0d537f7f-7bbb-4125-ba56-464212680857"), "ELO", 10, "509046" },
                    { new Guid("0d91f868-a9af-4b67-a4dd-4309f217da9a"), "MASTER", 0, "537993" },
                    { new Guid("0f95b35d-b088-43f1-afdc-f1330ef094ff"), "VISA", 0, "444125" },
                    { new Guid("11b3dedf-f205-4f08-a247-1888ed5ddf7b"), "VISA", 0, "186307" },
                    { new Guid("1226024e-da5b-4ecb-b22b-2e5ccf10e759"), "ELO", 0, "680405" },
                    { new Guid("1866854f-75e4-48f7-afa8-8f5ebbad4c15"), "MASTER", 20, "2720" },
                    { new Guid("1895b3bf-ecc6-42c4-bb1f-5402b6aa9d62"), "DISCOVER", 1, "306262" },
                    { new Guid("1bd1b2df-f667-4853-b4ad-2c7ff84a039e"), "ELO", 0, "627780" },
                    { new Guid("1c84372b-2eee-4414-a07b-e912581da602"), "DISCOVER", 70, "644" },
                    { new Guid("1ea5f343-1f19-4c43-a9f5-a2a3adbbb6dc"), "DISCOVER", 70, "6223" },
                    { new Guid("1f3a9016-d24b-4c38-8bb0-1ef5db7a6656"), "ELO", 0, "509093" },
                    { new Guid("2139bc42-1b72-4dfd-bf7e-157c7a167649"), "DISCOVER", 70, "6226" },
                    { new Guid("215d0f4e-6f46-4c1a-8018-87d143b4baea"), "DISCOVER", 70, "622128" },
                    { new Guid("21bc57ee-6b18-45a3-ba13-db6e985dbd9a"), "MASTER", 0, "505381" },
                    { new Guid("22920e15-13f2-4985-adfd-513eb097aaa3"), "ELO", 0, "650042" },
                    { new Guid("2454f6c5-e0f5-46e5-a704-79ce888dda83"), "VISA", 0, "210790" },
                    { new Guid("2455bd06-47c5-4b20-a22b-b77010f9386e"), "AMEX", 40, "37" },
                    { new Guid("266c0ebf-210d-4c33-8b45-fe2f7edd6fd3"), "DISCOVER", 70, "62219" },
                    { new Guid("28ab96b0-c938-4dfc-a2db-a1ec475bcf1a"), "VISA", 0, "970800" },
                    { new Guid("2981e6f9-3475-44af-ad56-2af0d52bce8a"), "DISCOVER", 70, "648" },
                    { new Guid("2a91a797-706f-4aa0-b956-5105e9ec7e94"), "MASTER", 20, "54" },
                    { new Guid("2c78ebae-3891-4cd6-986b-e80fe208845c"), "ELO", 10, "636368" },
                    { new Guid("2e32e13e-f65b-4c43-b439-e497b7ffaa57"), "DISCOVER", 70, "62216" },
                    { new Guid("2ebd5eaf-c805-4446-b317-4f5f755136c8"), "DISCOVER", 70, "62217" },
                    { new Guid("2f268b01-22cc-4333-8b14-ca3676d32a18"), "VISA", 0, "627892" },
                    { new Guid("32e917d9-4b27-4e4e-8ede-e4bd461d8f03"), "ELO", 10, "509045" },
                    { new Guid("3303cee1-40fb-449f-8605-90e092b28d24"), "VISA", 0, "560780" },
                    { new Guid("34d2dd63-540d-402a-bb0b-62cf9dabd64e"), "ELO", 10, "509040" },
                    { new Guid("36894d44-4f2d-4140-8374-bd81bec5b943"), "VISA", 0, "107915" },
                    { new Guid("36de40c9-b74e-4d55-ab5a-82b4e2b6ff47"), "DISCOVER", 70, "645" },
                    { new Guid("377446b9-fd5b-484c-b0b0-71f5b5eec73a"), "DISCOVER", 70, "6011" },
                    { new Guid("3984dea5-ba1e-461c-af58-dd5ff7069845"), "MASTER", 20, "2224" },
                    { new Guid("39a3fb06-b3c5-4148-8251-88105679e9ed"), "ELO", 10, "509074" },
                    { new Guid("3b2e392f-8aba-47cb-b38e-6a9783f9c66c"), "ELO", 1, "6507" },
                    { new Guid("3c95709d-5f6e-4d3f-95b2-b6d4cb98b074"), "ELO", 0, "6509" },
                    { new Guid("3d2d694d-3d71-4bb3-a358-ddfeb99988e2"), "ELO", 0, "509004" },
                    { new Guid("3f02e54d-fd91-4968-b7f0-56e71698a746"), "MASTER", 0, "608710" },
                    { new Guid("40dade13-2d20-4f0e-8496-1a84dfd86181"), "ELO", 0, "640485" },
                    { new Guid("42e1eec8-bbcc-494e-bbed-00f938e97d1f"), "ELO", 10, "5067" },
                    { new Guid("43ed3784-00b3-4632-9290-09e59ce9c7e3"), "VISA", 0, "40064" },
                    { new Guid("443bd561-1f52-4d1b-8dc2-789fda1f7659"), "MASTER", 0, "003006" },
                    { new Guid("45a82d8f-f94e-4022-b8ea-8e333d10fa28"), "MASTER", 20, "53" },
                    { new Guid("48591305-20b7-4e23-b4f8-661c797d8a5a"), "MASTER", 20, "2228" },
                    { new Guid("4993c42d-cef1-40d6-a5d3-5ba3b41a3f9f"), "DINERS", 30, "304" },
                    { new Guid("4cb807b0-2380-4d25-8b56-db1eff7b6d86"), "MASTER", 0, "585988" },
                    { new Guid("4eaf9524-7252-4b25-8a24-071a8d979413"), "DISCOVER", 70, "62214" },
                    { new Guid("4f55c6ab-649e-44b8-82a3-48063bb66211"), "MASTER", 0, "5379" },
                    { new Guid("4ffe5519-7875-4e23-83e2-105477206428"), "ELO", 10, "509040" },
                    { new Guid("5127c77a-e485-474e-865f-d55d16104090"), "AMEX", 40, "35" },
                    { new Guid("51832641-df38-4420-85d8-4a4d8319342c"), "MASTER", 20, "51" },
                    { new Guid("51928f8d-621a-4b00-b6fe-dcd956dbe528"), "ELO", 10, "509049" },
                    { new Guid("519f03fa-651b-4558-8fed-5030bea1d6ab"), "MASTER", 0, "595922" },
                    { new Guid("52b1ba66-7e36-4820-9cd2-b009685ab521"), "VISA", 0, "027728" },
                    { new Guid("54b77a7f-4d86-44d3-b172-ce460b7ce84c"), "VISA", 0, "854644" },
                    { new Guid("56ba5f1a-ced6-4179-8e1d-e0310dde74b3"), "MASTER", 20, "24" },
                    { new Guid("56c99f1f-5063-402b-9b37-db9e2cdda98e"), "ELO", 0, "509730" },
                    { new Guid("571ca1dc-63ed-4d4d-bbfe-c00560fe4c08"), "ELO", 1, "509274" },
                    { new Guid("59057700-ef58-4bd6-a2e0-c0be0985db07"), "DISCOVER", 70, "647" },
                    { new Guid("5b204185-88c2-4e16-a7b8-ce96e52d5779"), "DISCOVER", 70, "62218" },
                    { new Guid("5bde3090-6f6e-43e1-b316-3851f8b0e591"), "ELO", 0, "006538" },
                    { new Guid("5da1f97b-fb75-4c61-b535-a3cc73bc0633"), "MASTER", 0, "570209" },
                    { new Guid("5eee4e2e-fa5a-4eaf-a660-66ec80483dc7"), "MASTER", 0, "212743" },
                    { new Guid("5f23e025-81db-445a-810b-c2d89c3b0d21"), "DISCOVER", 70, "622129" },
                    { new Guid("5feefe95-f62c-41e4-bb78-82c75857ce2c"), "MASTER", 0, "615292" },
                    { new Guid("6136fec1-3021-40d2-8f70-37e7c9c5384b"), "MASTER", 20, "25" },
                    { new Guid("61b0ece5-f43f-4bdb-ab94-73ced9107144"), "ELO", 0, "509294" },
                    { new Guid("61c93e03-1fa7-44c1-a44b-159d6834e77d"), "MASTER", 1, "608783" },
                    { new Guid("6415ce04-2849-4df0-9734-27dcbc562c49"), "VISA", 0, "566269" },
                    { new Guid("6479e443-5840-4b6d-ac0a-f3ad0ce80e4d"), "DISCOVER", 70, "6225" },
                    { new Guid("67b97e41-3a32-47c7-88f5-b08b28ca6442"), "VISA", 0, "121772" },
                    { new Guid("67c21260-6e79-45bd-9f4d-ff6b64bbf5d8"), "ELO", 4, "506741" },
                    { new Guid("67dfbf5d-1850-42ae-a0f4-661d32f15e52"), "ELO", 10, "4576" },
                    { new Guid("68d08bf5-9edc-4cfc-be6c-8b3116175f3d"), "MASTER", 0, "316811" },
                    { new Guid("69a4bca6-db70-4086-86f4-1daeb0cc3bab"), "VISA", 0, "988442" },
                    { new Guid("69cfdbd2-7a03-44f3-9b22-9ab9c302cc37"), "MASTER", 20, "227" },
                    { new Guid("69fa4e15-c7e6-4ce9-9862-407e8ef876d1"), "MASTER", 20, "2221" },
                    { new Guid("6b1b71d5-23e5-4e60-b740-25b3fc7569e4"), "MASTER", 20, "26" },
                    { new Guid("6b272f7e-2d41-48a0-8b76-c51ec97f1f89"), "MASTER", 0, "502299" },
                    { new Guid("6c060d1d-8c2f-45bd-83e5-b8765cea39ee"), "MASTER", 20, "2222" },
                    { new Guid("6c5a79b4-b41e-4b92-acf0-92a6b55abd36"), "DISCOVER", 70, "622925" },
                    { new Guid("6fc94aa9-eb2b-43eb-b2dd-87dd5361f16d"), "ELO", 5, "6516" },
                    { new Guid("713fd508-c89b-4894-b9f5-809792cced74"), "ELO", 0, "502577" },
                    { new Guid("7342fa6e-e737-45b3-8844-d85298e7975e"), "ELO", 5, "509023" },
                    { new Guid("73aa02f6-7f3a-41fc-9aa1-f878a47ecb53"), "MASTER", 0, "320132" },
                    { new Guid("741f140c-5beb-428c-b502-643d754fe23a"), "MASTER", 0, "5453" },
                    { new Guid("747ece4d-43a5-4793-b659-b3f11c7c7ada"), "ELO", 10, "504175" },
                    { new Guid("75dfd39f-8703-44e1-a04f-7e4b985f0159"), "ELO", 10, "509069" },
                    { new Guid("76354b51-5520-4415-bab9-03073028ddbe"), "VISA", 0, "960421" },
                    { new Guid("7662decc-e708-4b09-9de7-df03d12de745"), "ELO", 10, "409835" },
                    { new Guid("78607f37-648f-4f44-9996-33ad12a714fa"), "ELO", 10, "509000" },
                    { new Guid("7947ba52-2f17-41ad-937b-db6d9f728c6a"), "ELO", 10, "650494" },
                    { new Guid("7989c0be-2c68-455d-96ca-a2b10f3bc6f9"), "HIPERCARD", 60, "38" },
                    { new Guid("7a24a105-c838-42c3-a76c-74f52731b233"), "MASTER", 0, "720358" },
                    { new Guid("7b80f7b3-ca2b-4826-945b-2651efbd18ba"), "ELO", 10, "4011" },
                    { new Guid("7d069fcf-1e15-4757-bb62-fc8ace46c0c4"), "MASTER", 20, "226" },
                    { new Guid("7d440ed1-d01c-481e-b517-05b8cc76c579"), "DISCOVER", 70, "622924" },
                    { new Guid("7d7bda98-6e41-4463-9725-59b587e741da"), "MASTER", 20, "2223" },
                    { new Guid("7f8612d9-1ec0-452e-a3d7-02461952ec29"), "MASTER", 0, "55909" },
                    { new Guid("82be5869-6fee-4782-8961-024a1e2473e7"), "DISCOVER", 70, "6227" },
                    { new Guid("8310cfff-2ce9-488f-9472-0935b2b2c23c"), "MASTER", 0, "505316" },
                    { new Guid("83e0cf28-f802-4254-aa1b-1ced172edab6"), "ELO", 10, "509050" },
                    { new Guid("84aaa2a3-d5bf-462a-8554-6e13588f291a"), "MASTER", 20, "228" },
                    { new Guid("84b1df64-5352-4fdf-9817-492ce3da2684"), "VISA", 0, "033994" },
                    { new Guid("85eac545-decc-4313-a84b-055ec3eb30b7"), "ELO", 0, "508757" },
                    { new Guid("86507397-c2fe-4e92-a44e-5b2ee5e1a41b"), "MASTER", 0, "596497" },
                    { new Guid("8703460a-828d-4928-9a69-4cb97acad932"), "HIPERCARD", 0, "506282" },
                    { new Guid("87ab02f5-b316-48ac-95c5-5b771ba96f88"), "MASTER", 0, "560033" },
                    { new Guid("88395a41-5e25-4663-bd77-a4cbaa2989b0"), "MASTER", 20, "223" },
                    { new Guid("888f3cca-091b-4f4b-b375-6967c9e14ff8"), "ELO", 10, "650485" },
                    { new Guid("894cc484-c9af-4cec-8f05-681be73a7994"), "DINERS", 30, "36" },
                    { new Guid("8a456406-2c12-4a59-8ea5-0e2454f71796"), "DISCOVER", 70, "622920" },
                    { new Guid("8b591cfa-258a-4a7d-b619-ee5e9ba36340"), "ELO", 10, "650905" },
                    { new Guid("8d9cd703-0c4f-42b0-8914-2168189ee0fd"), "ELO", 10, "509067" },
                    { new Guid("919a9134-b3b9-4bf1-9fa0-48e507deb47b"), "ELO", 5, "6505" },
                    { new Guid("93699de3-edd6-4024-a5a7-bf8f936ca867"), "DISCOVER", 70, "6228" },
                    { new Guid("939e53b9-86ba-469f-ad97-17c32835e9d9"), "ELO", 10, "65090" },
                    { new Guid("940e0f85-d19b-43cb-982c-1f1e15deb620"), "MASTER", 20, "271" },
                    { new Guid("94c0a1c9-d3e7-47d9-91bf-365479d4f8f9"), "MASTER", 0, "830050" },
                    { new Guid("986c650d-77b2-4ebd-ba3a-05e19e28ba87"), "MASTER", 20, "2225" },
                    { new Guid("9920629d-d69b-47c4-8261-a13885ec06f6"), "MASTER", 20, "225" },
                    { new Guid("9c846466-addc-44a5-a9e9-c0d9ce6c6c55"), "ELO", 10, "36297" },
                    { new Guid("9c85bcf1-862a-4f10-bd8a-67655d47b148"), "VISA", 0, "636355" },
                    { new Guid("9f3280c6-280e-40d2-a92b-bbe3d2b7e1cd"), "DINERS", 30, "302" },
                    { new Guid("a0cef9b0-5c2c-4966-a242-39ccaa9f1dd2"), "ELO", 10, "451416" },
                    { new Guid("a1b1e2b6-ac4c-44ef-9f9b-482efef3f410"), "ELO", 10, "509043" },
                    { new Guid("a455223d-54e5-4459-8c0f-1d8a62481346"), "DINERS", 30, "305" },
                    { new Guid("a578ec2e-2492-4c6e-a783-8b5f928cac5a"), "ELO", 10, "509066" },
                    { new Guid("a5efa71d-7ce2-462b-ba32-e6b0bb6115c1"), "ELO", 10, "50904" },
                    { new Guid("a684588c-fd6b-4ce1-82a3-f1f9276976da"), "MASTER", 0, "960379" },
                    { new Guid("a737a5ec-d9ab-4c82-a211-aac229572c7e"), "ELO", 10, "650487" },
                    { new Guid("a7840efe-7668-40a9-bf65-3e4f43ebc80b"), "ELO", 10, "650921" },
                    { new Guid("a7bf25c7-75c7-4e4f-9cfe-f18838d5ff13"), "MASTER", 0, "330239" },
                    { new Guid("a8b85150-056a-453a-8678-c8d10779d5a1"), "ELO", 10, "50906" },
                    { new Guid("ab9ca0f5-30a2-4db5-8526-6bf2cd678cee"), "MASTER", 0, "044027" },
                    { new Guid("ac7bf604-5bd2-4dd0-8099-c15a88a7cbd0"), "HIPERCARD", 80, "3841" },
                    { new Guid("ad230cbe-9f7e-46e6-9b30-bcd0ce41e10b"), "MASTER", 0, "637095" },
                    { new Guid("ae29ebf9-69c0-4b66-abdf-2e48690e4a75"), "MASTER", 0, "509059" },
                    { new Guid("b11ac102-5dfd-4b8b-a4e8-d0e9204436fb"), "DISCOVER", 70, "62215" },
                    { new Guid("b1e470be-3bb1-4cd0-ade8-4f9fa46c2a3a"), "AMEX", 40, "36" },
                    { new Guid("b2f5de9e-c1c1-4e0d-9379-c1f24ef8e317"), "ELO", 10, "650491" },
                    { new Guid("b58cae42-3b1a-4b33-adc0-0605ca3c53be"), "MASTER", 0, "126820" },
                    { new Guid("b661f15e-c32d-4b24-980d-8b2f9f90dbe4"), "DISCOVER", 70, "622923" },
                    { new Guid("b6f7207a-21b9-4fb0-81e9-3e55d77ddfcf"), "DISCOVER", 70, "646" },
                    { new Guid("b8694624-7a37-4a1f-a021-39572403cdef"), "MASTER", 0, "576096" },
                    { new Guid("b86a08ba-7ef8-42af-8039-47e862936d91"), "NARANJA E NEVADA", 100, "589562" },
                    { new Guid("baa161f5-9fac-408f-a32e-4b4492924167"), "DISCOVER", 70, "6224" },
                    { new Guid("baebd91b-9623-4260-ad5b-af4662a10b96"), "DISCOVER", 70, "622921" },
                    { new Guid("bb547111-3fa8-496a-8852-45a2e19eaf5e"), "DISCOVER", 70, "622126" },
                    { new Guid("bbc8335f-21ac-4f3b-9895-195dc8039172"), "MASTER", 0, "509257" },
                    { new Guid("bce2bf7f-e123-483a-9ec2-9f1265c81f69"), "DINERS", 0, "605919" },
                    { new Guid("bd829626-5c9a-4f14-aa24-46be950051ea"), "ELO", 0, "699988" },
                    { new Guid("bf4969cb-fb5f-4000-8e12-e318d567a0d9"), "ELO", 5, "6550" },
                    { new Guid("bf681118-f178-4c8e-86c4-3b1deeb45111"), "MASTER", 0, "574080" },
                    { new Guid("c1196f95-2f9b-4e2d-8c68-6702e3393a89"), "MASTER", 20, "23" },
                    { new Guid("c17945d1-dfac-4528-92d3-58ba7b8673a9"), "MASTER", 0, "546479" },
                    { new Guid("c1acc363-7ccd-4f43-909c-2796f86f6811"), "ELO", 0, "506874" },
                    { new Guid("c31a0eb8-6ff4-4aef-a0c8-2e04fe66fa2c"), "MASTER", 0, "589916" },
                    { new Guid("c84779fa-e8ee-4508-8e80-775b0727716d"), "MASTER", 20, "55" },
                    { new Guid("c87079de-67b1-4461-8119-87a2f858b61d"), "MASTER", 0, "640245" },
                    { new Guid("ca5a23a0-ee54-4efc-ac1b-c13ce30f3af5"), "MASTER", 0, "439133" },
                    { new Guid("cb530f63-d672-49f9-b90e-198c1730c4a4"), "ELO", 0, "67410" },
                    { new Guid("cbd8e34e-7a1a-49eb-bb70-daea623fcc85"), "MASTER", 0, "220209" },
                    { new Guid("cd33e4b6-ffd7-48f5-bf7a-33d9a1416655"), "DINERS", 30, "301" },
                    { new Guid("cd449ce4-ec99-461e-878b-b7b284c6784a"), "ELO", 10, "650912" },
                    { new Guid("cd5a8a4d-3fe5-46c8-9bf4-ff632ed4a3e7"), "DISCOVER", 70, "649" },
                    { new Guid("cdce4927-80a9-4b6d-9438-56def8a2b1e6"), "ELO", 0, "111111" },
                    { new Guid("cf4c6b6d-810f-42a2-83f9-501716ac859c"), "VISA", 0, "984428" },
                    { new Guid("d0a80bae-3f0b-4519-bb13-3619262398a2"), "ELO", 5, "6280" },
                    { new Guid("d2076b08-7e08-4cc6-a533-37e6eccfd9fc"), "ELO", 1, "650914" },
                    { new Guid("d373b40b-66fa-451b-b5a8-965086723752"), "ELO", 0, "509156" },
                    { new Guid("d38e16f5-ca2e-42df-9c39-3ac77bf79f8d"), "VISA", 5, "4984" },
                    { new Guid("d4863a73-eafa-4e91-9e31-a890cc11e225"), "VISA", 0, "798431" },
                    { new Guid("d7198f2e-3210-480d-9bfb-3470bafc0f25"), "MASTER", 1, "750209" },
                    { new Guid("dd59877d-b1d8-4aa4-b2f2-dee2992c11fa"), "ELO", 0, "650920" },
                    { new Guid("ddbf27fc-21d8-444c-b93b-47a6f713bcf6"), "DISCOVER", 70, "62213" },
                    { new Guid("debdbe26-833d-47f6-9de6-d10fbe2af90a"), "ELO", 10, "438935" },
                    { new Guid("e07c6274-1a20-4b3b-9c3b-23ed9a4de815"), "ELO", 10, "509052" },
                    { new Guid("e1c13d5e-0221-4b5d-9faa-378bed549cf3"), "MASTER", 20, "2229" },
                    { new Guid("e1c5586d-fced-4bd0-b9fd-3546b1e223a4"), "MASTER", 0, "507641" },
                    { new Guid("e1e68a6e-3d21-4466-a014-a12cd6da4514"), "ELO", 5, "6504" },
                    { new Guid("e1f7e6f5-13e6-4b66-ad5c-5b66ed179356"), "DISCOVER", 70, "6222" },
                    { new Guid("e23f8298-2a50-46f8-a78d-6a362aff5a49"), "MASTER", 20, "2227" },
                    { new Guid("e274cbf7-4c1a-4553-b470-c571ecd1f416"), "ELO", 0, "509410" },
                    { new Guid("e28503e3-ff7e-47b7-aba8-6d4654ccf5fb"), "MASTER", 20, "2226" },
                    { new Guid("e358c814-280d-45eb-872c-920cd492b095"), "MASTER", 0, "569970" },
                    { new Guid("e6932ba8-8947-4555-a1d3-e93b37bc49a3"), "MASTER", 0, "960397" },
                    { new Guid("e7d74a25-fed9-4d49-96d9-ec4b6d4a3891"), "ELO", 10, "650507" },
                    { new Guid("e83043b6-560f-4746-a376-2536571017a7"), "MASTER", 0, "988996" },
                    { new Guid("e9aec32b-6185-4b1d-92f7-872a6584e30b"), "MASTER", 0, "502121" },
                    { new Guid("eafb8d4a-2fdc-454e-a659-0ae822e64022"), "DISCOVER", 70, "65" },
                    { new Guid("ecbe1d3f-9f8c-48f5-a658-3d9d5d7f91d2"), "ELO", 10, "506726" },
                    { new Guid("ed68f39d-f946-4af2-9dcd-a4b7056cae78"), "ELO", 0, "509005" },
                    { new Guid("ede7ea0e-8df3-465d-85e9-fb0671743b01"), "ELO", 10, "509042" },
                    { new Guid("ee4a29a6-ec2a-40ff-839a-924bf0aa6147"), "MASTER", 20, "229" },
                    { new Guid("f0c45cd5-388c-44b5-8287-1de96c580545"), "MASTER", 0, "502309" },
                    { new Guid("f128d9c6-a1e8-49c5-b4e1-7ed6ec5dfa49"), "ELO", 0, "509284" },
                    { new Guid("f19990fa-38ac-4b17-80f8-6cd7ad9f714b"), "MASTER", 0, "53508" },
                    { new Guid("f2dd52bd-da7c-4d38-a3fe-6595835fbe9d"), "AMEX", 40, "34" },
                    { new Guid("f49ba514-def7-4a62-a362-1d4aaad4b23a"), "MASTER", 0, "639350" },
                    { new Guid("f4db535d-ce27-4ebe-8d01-9771b81b0c11"), "CABAL", 60, "6042" },
                    { new Guid("f5c201c3-4cd1-4844-b1bf-ac72986cd43a"), "DISCOVER", 70, "622922" },
                    { new Guid("f6366254-277e-4e2a-8f2d-00ff4b964b83"), "VISA", 0, "48546" },
                    { new Guid("f6e69e5d-3940-4afb-ad48-b2932e075031"), "ELO", 0, "509010" },
                    { new Guid("f70886d7-dcba-451a-8bea-57a215a8865e"), "MASTER", 0, "517349" },
                    { new Guid("f77d1687-99cd-448e-ae7b-38536c2b87a7"), "MASTER", 0, "576292" },
                    { new Guid("f8100aea-d8c9-4335-85fc-e9870fb5cd2e"), "ELO", 10, "509059" },
                    { new Guid("f8418814-2f4b-466b-9a11-876be7daf054"), "VISA", 110, "4" },
                    { new Guid("f898b6b1-7cdd-4f8c-8cfc-35005326c523"), "VISA", 0, "985966" },
                    { new Guid("fa03dcfd-1f17-44b9-aa39-433ea948bfa3"), "DISCOVER", 70, "62290" },
                    { new Guid("fa55f0bb-898d-4ed4-aed7-4c345098d12f"), "JCB", 90, "2131" },
                    { new Guid("fc54a4f1-6a54-4017-8a49-84349154e923"), "ELO", 10, "509091" },
                    { new Guid("fe106cea-d47a-4744-aaeb-eeaac4c1297d"), "DISCOVER", 70, "622127" },
                    { new Guid("fe1a8677-1d09-459a-803d-e08ada083a65"), "DISCOVER", 70, "62291" },
                    { new Guid("fec41d70-aa8e-40f5-9b76-e2df4b02ccb9"), "MASTER", 20, "224" },
                    { new Guid("ffdc5e98-c34f-46c7-89c6-ef762b6e53eb"), "DINERS", 30, "303" }
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
