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
                    { new Guid("019d83f5-1626-4deb-b902-adb3178bdd0f"), "DISCOVER", 70, "622129" },
                    { new Guid("047c0f44-b4d7-4b15-b2ac-916b61e71fd7"), "MASTER", 20, "271" },
                    { new Guid("05e514b9-0ecd-4608-a3da-5226ef0a1f3a"), "ELO", 10, "509074" },
                    { new Guid("08726280-f1b6-40f0-b720-2c337671e48c"), "ELO", 1, "6507" },
                    { new Guid("0a191ab4-3265-409e-9dd3-9aa77bcdad33"), "ELO", 10, "509040" },
                    { new Guid("0d0241fd-e17c-4fbf-8c01-e2feb1cd2fb6"), "VISA", 0, "33994" },
                    { new Guid("0e4188da-a747-440b-9a62-0e4a3bac889a"), "JCB", 90, "1800" },
                    { new Guid("0efffe4e-19d3-4ba9-8d02-a99782a2117a"), "ELO", 10, "509045" },
                    { new Guid("0f122ac3-c9c5-4f9e-8f0c-7a7e498cd0f6"), "ELO", 10, "650494" },
                    { new Guid("0f577894-9c74-489c-9868-aa1ab8053b65"), "MASTER", 0, "637095" },
                    { new Guid("0fc1c37c-bb1e-460d-b850-f10809fb596b"), "VISA", 0, "121772" },
                    { new Guid("11c02a97-ea68-4d40-8dfc-f98fcde11593"), "MASTER", 0, "5379" },
                    { new Guid("11eade65-4c39-4970-a763-dea6f383c77a"), "DISCOVER", 70, "644" },
                    { new Guid("12882009-01a3-4aae-8a57-4b29a7649cf5"), "ELO", 0, "509423" },
                    { new Guid("14fd766b-5ed6-48a8-a7a6-285dca27abdc"), "MASTER", 20, "229" },
                    { new Guid("16c439f4-9b15-4f59-8f25-d512b9dd6a88"), "MASTER", 0, "589916" },
                    { new Guid("170330e9-2313-4c53-b961-eabb06009b25"), "MASTER", 0, "320132" },
                    { new Guid("185eb821-d117-4685-ad22-0d80091df950"), "MASTER", 20, "2223" },
                    { new Guid("19459571-d258-4b67-8963-5d605399dcc2"), "DINERS", 30, "305" },
                    { new Guid("1996a12f-1f5e-4d52-8d3a-10c4a89f8630"), "ELO", 0, "509005" },
                    { new Guid("1d7b2c52-de41-4d0b-b3dc-0501c15e7029"), "MASTER", 20, "23" },
                    { new Guid("1f2da3f2-4515-44da-8a7d-a10526c6f18d"), "VISA", 0, "186307" },
                    { new Guid("1f4b766b-4283-4276-ace4-305b36f9f477"), "AMEX", 40, "34" },
                    { new Guid("1fbe6a7e-85c7-41b3-a6e8-6f0181005eb2"), "DISCOVER", 70, "6224" },
                    { new Guid("25435b42-5fc1-41e1-906e-4a20ec2a1e59"), "MASTER", 0, "576292" },
                    { new Guid("27913d95-9d7c-4934-a27c-da77f468fa00"), "ELO", 0, "509730" },
                    { new Guid("27bfc9a1-9ef3-4530-85fb-efc2583b7d37"), "ELO", 10, "650921" },
                    { new Guid("27e20732-3f4c-4ef5-b1d4-cb3116d75611"), "VISA", 0, "970800" },
                    { new Guid("28d805d5-0ba3-4bc1-9399-c55819b43552"), "ELO", 0, "509010" },
                    { new Guid("29e6af86-784f-4d62-9a1f-701edbef8766"), "MASTER", 0, "3006" },
                    { new Guid("2ab60906-3bec-404c-8989-cb524787e766"), "JCB", 90, "35" },
                    { new Guid("2bc7ea58-9b8a-4596-b0b7-d9d82a090525"), "ELO", 10, "509051" },
                    { new Guid("2cc7b2ce-7529-40b5-8d1b-2ab208c0205e"), "ELO", 10, "4576" },
                    { new Guid("2d61317d-82c5-4f58-a0e6-8499a4c67950"), "VISA", 0, "984428" },
                    { new Guid("2d790db4-4f72-45cb-ace0-c089fcd3f186"), "MASTER", 0, "220209" },
                    { new Guid("2dd401d8-4ee5-44e2-872c-2a9ca5f4bd7d"), "AMEX", 40, "36" },
                    { new Guid("2f88a8c7-25a8-49a7-a994-ad1b6020dacf"), "ELO", 10, "50904" },
                    { new Guid("31758c60-2da2-465e-9bdf-1b4d6c611ce4"), "DISCOVER", 70, "622126" },
                    { new Guid("328a0b13-d4af-48e8-a874-e9899c44c20a"), "VISA", 5, "4984" },
                    { new Guid("32a55f83-0266-40d3-b9c5-825ecc616ab1"), "ELO", 10, "509059" },
                    { new Guid("32b2666c-3a65-469d-8ab6-3c5fadbdb23c"), "ELO", 10, "50904" },
                    { new Guid("33e6c3e1-4fd8-4f2b-8d1b-81d860a0d281"), "MASTER", 0, "640245" },
                    { new Guid("361043a9-cfb4-4051-9952-7578139a741e"), "MASTER", 20, "223" },
                    { new Guid("372c1e1d-6da6-4e6f-ae74-9bd5e98d9eb6"), "MASTER", 0, "595922" },
                    { new Guid("37a29ac2-329a-43a2-ba5f-d8f1dd86df7e"), "MASTER", 20, "2224" },
                    { new Guid("38098e34-530e-4fda-8863-b09a4a4ac4dd"), "ELO", 10, "509052" },
                    { new Guid("39e90751-8331-4290-a400-7fa9f8bf5438"), "ELO", 10, "650485" },
                    { new Guid("3a49a94a-79de-45b1-97fd-7fa826f17b64"), "ELO", 0, "509294" },
                    { new Guid("3bc2f9e9-9a4c-4ae3-bfc9-095f23615327"), "VISA", 0, "854644" },
                    { new Guid("3bc777cf-5c1a-46ac-b1b4-fd53d45b8c91"), "MASTER", 0, "53508" },
                    { new Guid("3c1cbe69-a722-49ff-96d4-b98591aa069e"), "MASTER", 20, "26" },
                    { new Guid("3c351858-f756-4b58-bc62-ae33e2bff185"), "DISCOVER", 70, "62218" },
                    { new Guid("3d04e024-b004-4e9c-ba56-21ca22e65fc5"), "MASTER", 0, "639350" },
                    { new Guid("3d0cbd21-fa13-4f5f-b74d-3dcb66a05943"), "DISCOVER", 70, "6228" },
                    { new Guid("3d6ce16e-4aed-498c-b231-85acb1750733"), "ELO", 0, "699988" },
                    { new Guid("3e362513-32dc-45f8-8627-557960c8203f"), "DISCOVER", 70, "645" },
                    { new Guid("40970807-f897-4ead-b9e7-ff75f806d4a4"), "DINERS", 30, "36" },
                    { new Guid("418648de-f22c-4e02-a92b-6c74c445a16b"), "MASTER", 0, "509059" },
                    { new Guid("41ed3b63-f623-4b24-913f-af8fb70b8352"), "DINERS", 30, "301" },
                    { new Guid("4380b3c8-2ebd-4190-9cbe-5f5917b6dfa9"), "DISCOVER", 70, "6223" },
                    { new Guid("43c8f398-4f62-4ab1-bc28-d4c87d9ce2f1"), "MASTER", 1, "515894" },
                    { new Guid("44a95072-45b3-4739-8c6c-0d688806c971"), "DISCOVER", 70, "622127" },
                    { new Guid("458d0eb6-a67a-45c9-8a06-225095ba8cd0"), "ELO", 10, "509091" },
                    { new Guid("49cb8a2d-288a-4b05-bc22-6dbe65b7230c"), "MASTER", 0, "560780" },
                    { new Guid("4e3298c3-b678-4f2c-804f-2ab1cba41c57"), "MASTER", 20, "2221" },
                    { new Guid("4e7d4dd3-6a4d-42b3-ad40-31466a28c195"), "ELO", 10, "506726" },
                    { new Guid("501d8c70-019e-4e30-aa47-36a70f06a4ac"), "AMEX", 40, "35" },
                    { new Guid("50f6ed36-2f03-422a-afdc-9b068db4f193"), "CABAL", 60, "6042" },
                    { new Guid("51a027d5-c707-45e0-b84c-ee4fe8346ccf"), "HIPERCARD", 80, "3841" },
                    { new Guid("521c76cd-dd9c-4705-b1fa-850da80fac2f"), "ELO", 0, "509410" },
                    { new Guid("539b0701-d51f-4424-9e66-b276bb662586"), "ELO", 0, "640485" },
                    { new Guid("53a22cfe-d70d-4fcf-968c-4f341b97bb56"), "DISCOVER", 70, "6222" },
                    { new Guid("540c9c7b-f865-4e2b-bd01-e40aa4f475ac"), "MASTER", 20, "54" },
                    { new Guid("557e910b-e244-47ca-8859-038d9b79fcd1"), "MASTER", 20, "2228" },
                    { new Guid("5649ee67-c70d-4e7a-8993-4cfc53f5beb7"), "MASTER", 20, "2720" },
                    { new Guid("57544e75-332c-4a8a-88ae-2e4373591bbe"), "VISA", 0, "48546" },
                    { new Guid("5866c9af-2a7e-4eff-9994-035314df0635"), "MASTER", 20, "25" },
                    { new Guid("58985d65-33a6-4366-8dd4-253871eb19df"), "JCB", 90, "2131" },
                    { new Guid("5a872234-d9de-4064-82fb-352a54e85882"), "ELO", 10, "65090" },
                    { new Guid("5b1428e7-d4ae-4afb-9178-6b5dd54f2c89"), "ELO", 10, "50906" },
                    { new Guid("5babcabd-b249-4acb-9569-79fdc4a56614"), "VISA", 0, "985966" },
                    { new Guid("5d03a975-b5e2-438d-a938-d8356915e565"), "DISCOVER", 70, "622925" },
                    { new Guid("5d7e4867-f73b-476f-bc1a-c06d57c36e88"), "MASTER", 0, "505381" },
                    { new Guid("5dc92acc-204b-48fb-8863-d0d8b0fef710"), "MASTER", 0, "7807" },
                    { new Guid("5e00b7c5-2767-493d-858c-5b66fde72d67"), "ELO", 0, "627780" },
                    { new Guid("5e43e8b2-b3e0-4deb-a55e-75008b5503b7"), "MASTER", 0, "960397" },
                    { new Guid("5f06ede1-292a-4c9b-8719-9890880bdcc9"), "ELO", 1, "650914" },
                    { new Guid("5f8337ad-4351-4c08-98bb-b965ad228c9c"), "MASTER", 0, "569970" },
                    { new Guid("6213c361-2936-4171-9f55-b3b3c8a607c9"), "ELO", 10, "36297" },
                    { new Guid("62524d2e-2653-4242-8d48-36817a9991fb"), "MASTER", 0, "502309" },
                    { new Guid("62bb1673-9d47-4801-ade1-b1629e231093"), "VISA", 0, "40064" },
                    { new Guid("65a5579a-1bec-4a59-8985-8b0d82a2885b"), "MASTER", 0, "546479" },
                    { new Guid("6670c7c3-06d6-49b3-801b-6efc0d98f19f"), "HIPERCARD", 0, "506282" },
                    { new Guid("66b0704d-2017-4f93-80a9-7842ac76a570"), "ELO", 10, "504175" },
                    { new Guid("677097eb-3dca-49aa-a861-349737aea62a"), "ELO", 10, "509066" },
                    { new Guid("68350d71-df3e-435d-a29d-55066293a9d8"), "ELO", 10, "650905" },
                    { new Guid("688d9acc-9ba0-4783-bacc-9ddb4173c2b9"), "DISCOVER", 70, "622920" },
                    { new Guid("6bd521c8-8f0d-4b86-b3d9-b57923be374f"), "VISA", 0, "798431" },
                    { new Guid("6d0c70e5-9a34-4e42-a10c-12d2c1c85db8"), "DISCOVER", 70, "62290" },
                    { new Guid("6d929da7-ec2d-4214-b36f-93bac4e8906b"), "MASTER", 0, "439133" },
                    { new Guid("6dfb02ee-192f-48d2-99c8-6a7dbf3332d2"), "ELO", 10, "409835" },
                    { new Guid("6ff60a32-4520-4174-808f-b89804dce07e"), "DISCOVER", 70, "622923" },
                    { new Guid("708600f5-65ee-42fe-9fd8-cbe07b3b0489"), "DISCOVER", 70, "649" },
                    { new Guid("72f7ce26-1b8a-4b54-8366-7acac5fa9798"), "DISCOVER", 70, "65" },
                    { new Guid("736a228d-d1be-4bb1-9f98-34e81837737a"), "ELO", 0, "6509" },
                    { new Guid("746c6af8-763b-4b14-85c4-af6de723fcd0"), "ELO", 10, "650507" },
                    { new Guid("74ca13cb-8f1f-4a5e-a7ce-828a05c389d0"), "ELO", 1, "509274" },
                    { new Guid("75ef5156-9be7-4b8e-a2fc-d6487ddd04ab"), "VISA", 0, "566269" },
                    { new Guid("762eadbb-2f8a-42d3-8831-ac83d1a6b16a"), "DISCOVER", 70, "6227" },
                    { new Guid("79ce0aec-fdc1-49d8-8894-a4cd11e077c8"), "DINERS", 30, "302" },
                    { new Guid("7b833d12-ecdf-4d42-9845-bd61db0974d7"), "DISCOVER", 70, "62213" },
                    { new Guid("7d5b1844-e69e-4aff-b9ee-b28ac6fc2414"), "ELO", 0, "502577" },
                    { new Guid("7dcdaf17-5a44-4d0d-b68d-aa46e25d58d9"), "MASTER", 1, "750209" },
                    { new Guid("7e90319e-b81b-4f36-ad6c-1f9959997b28"), "VISA", 0, "636355" },
                    { new Guid("7f181468-040a-466f-89d7-0f12d2d15c17"), "DISCOVER", 70, "646" },
                    { new Guid("8200fe5a-0df3-4aac-a833-1469f3aa4333"), "DISCOVER", 1, "306262" },
                    { new Guid("831bc4f9-d0c2-4071-9a37-fe623504002b"), "DISCOVER", 70, "648" },
                    { new Guid("8335fbd9-9f8b-47ae-8b02-0e02b4e13da0"), "ELO", 10, "451416" },
                    { new Guid("847d863a-69aa-4f13-8ee4-0f55714cd315"), "ELO", 0, "509284" },
                    { new Guid("86f4e81b-4022-49f0-8b4f-959232eba8ec"), "ELO", 0, "509093" },
                    { new Guid("86fdf28e-b9b8-48bc-bf73-dc7070d5b430"), "MASTER", 20, "2222" },
                    { new Guid("87511687-047d-4426-9a88-5aa269f1c395"), "DISCOVER", 70, "62216" },
                    { new Guid("88dad83b-83c3-4069-88c2-b7ee6ddc2a27"), "ELO", 0, "111111" },
                    { new Guid("8958053f-d01a-4f82-bc3d-e02b7eaed729"), "ELO", 0, "650042" },
                    { new Guid("8b4c594c-8c0c-4d7a-bffd-06519797b1ec"), "DISCOVER", 70, "647" },
                    { new Guid("8bc204af-8164-4733-a54b-93cddc840373"), "MASTER", 20, "24" },
                    { new Guid("8e6dfe42-bb55-41f5-a2e1-cb4adabd5bde"), "MASTER", 20, "2229" },
                    { new Guid("8f4aab9d-c445-4f39-9290-8a471efa7b06"), "ELO", 10, "650487" },
                    { new Guid("92aebe51-c8a7-419c-8348-76d4562a0085"), "MASTER", 0, "5453" },
                    { new Guid("93391f3e-498e-4754-80a0-7281a5ca09f3"), "HIPERCARD", 80, "606282" },
                    { new Guid("9364e7a1-9c51-4b95-a927-e4f1d3b50c69"), "NARANJA E NEVADA", 100, "589562" },
                    { new Guid("94270a91-e4c1-4bc9-9d7f-5bb94c21c444"), "VISA", 110, "4" },
                    { new Guid("95b0b90f-0fb7-4d30-831f-5836f275adf2"), "ELO", 4, "506741" },
                    { new Guid("95ea7338-4c7d-40ef-9734-916fe482b4ca"), "MASTER", 0, "608710" },
                    { new Guid("9662223b-bd9f-481e-a293-06350d404c55"), "ELO", 10, "509040" },
                    { new Guid("980ad963-4f15-433e-9e82-af5edd80aeeb"), "MASTER", 0, "502299" },
                    { new Guid("9822027e-cf84-4ae4-a704-e0b530517404"), "MASTER", 0, "830050" },
                    { new Guid("98ab92ae-7016-46cf-a066-b086adde815a"), "ELO", 0, "6538" },
                    { new Guid("99d379f4-3a00-4aff-a1da-68327d36093b"), "DISCOVER", 70, "622128" },
                    { new Guid("99edce04-46b4-4046-a8a7-59048557d4c1"), "ELO", 5, "6550" },
                    { new Guid("9a94673a-54f3-4f09-93d1-16ef865614c5"), "VISA", 0, "988442" },
                    { new Guid("9aaab5d3-64b0-4379-98e9-5be970739959"), "ELO", 10, "509046" },
                    { new Guid("9b25bef6-c2b5-44d6-b2a2-c9b3e754be02"), "MASTER", 20, "2227" },
                    { new Guid("9b99adf1-7ad3-4216-88eb-5247ad25ea1b"), "MASTER", 0, "517349" },
                    { new Guid("9c3c15eb-be3b-4e12-b6f5-ad576b56520d"), "DINERS", 30, "303" },
                    { new Guid("a07f8613-ef45-427d-9ca1-4231b16b614c"), "ELO", 10, "438935" },
                    { new Guid("a235020a-9827-4c93-9c5f-e1335658bc8d"), "ELO", 0, "509156" },
                    { new Guid("a24d717b-481d-4e84-b7e3-ba0da06cc8a8"), "VISA", 0, "444125" },
                    { new Guid("a517ed0a-a6c8-4b4f-884e-abb9e64a1589"), "MASTER", 20, "55" },
                    { new Guid("a5e0c5b1-1ead-40eb-a93b-61200aa43115"), "DINERS", 0, "605919" },
                    { new Guid("a79bb1ac-62d8-4ba7-8cba-ed97104357db"), "MASTER", 0, "960379" },
                    { new Guid("ab87f213-fbf9-4a6e-a0e8-a9159f08738f"), "DISCOVER", 70, "62214" },
                    { new Guid("abe5ce0a-4520-4484-9d45-3eba2d8fab84"), "MASTER", 20, "227" },
                    { new Guid("ac37c8ec-de0e-4aef-b459-74a9bbca0d0e"), "MASTER", 0, "502121" },
                    { new Guid("ad509314-5aa6-42fd-983a-c2bc67f66d25"), "DISCOVER", 70, "6011" },
                    { new Guid("ad96d000-08b2-4924-ac34-49f4d581db4c"), "DISCOVER", 70, "62291" },
                    { new Guid("adf7ca3b-aee0-4b05-8441-0a91274b627c"), "ELO", 10, "636368" },
                    { new Guid("af0f800d-0301-4458-90dc-61142c273710"), "MASTER", 20, "228" },
                    { new Guid("b0275d62-1505-4b36-ae3b-d6fe317ae0b9"), "ELO", 10, "509069" },
                    { new Guid("b1794db5-ffe7-460d-9590-6679ec55785d"), "MASTER", 20, "225" },
                    { new Guid("b227c17f-ac09-4295-9292-ca23976d84c7"), "ELO", 10, "650491" },
                    { new Guid("b28bcfab-5dec-4c5b-be1f-aabdf3d397f6"), "ELO", 10, "509042" },
                    { new Guid("b2b8f277-605b-49b5-a12c-9b1bc386df26"), "VISA", 0, "107915" },
                    { new Guid("b55478b9-1828-435d-9098-91e6d1c1b89f"), "MASTER", 0, "566269" },
                    { new Guid("b7a7f2ed-dc45-4927-a134-abf95b91c9f9"), "DISCOVER", 70, "622922" },
                    { new Guid("b83a298c-82d3-4956-814b-dae7384a7488"), "ELO", 0, "650920" },
                    { new Guid("b90d7e08-ce95-4626-bd15-e1c840b9189c"), "MASTER", 0, "585988" },
                    { new Guid("ba07bae5-de24-4814-8d37-28ecf47bd316"), "ELO", 10, "509000" },
                    { new Guid("ba15f959-82dd-48b8-b452-741c8a74e3ec"), "MASTER", 0, "615292" },
                    { new Guid("baef3ee4-5786-4be1-af86-5e296fd7adbe"), "AMEX", 40, "37" },
                    { new Guid("bb9cff2d-5de1-47e3-be24-a5f118182119"), "ELO", 5, "6504" },
                    { new Guid("bce3dd28-390b-44f6-a9d7-2963690538d3"), "ELO", 10, "509043" },
                    { new Guid("bd85ac24-7040-4ce0-b011-9b9302c169d7"), "MASTER", 0, "507641" },
                    { new Guid("be79c275-0c2e-4817-a35d-ebafc23a15ac"), "ELO", 5, "6505" },
                    { new Guid("be926869-a7ee-45b9-85d3-9d2a615ce558"), "ELO", 0, "508757" },
                    { new Guid("bec9013a-a868-4259-a00c-632dd261eaae"), "MASTER", 0, "212743" },
                    { new Guid("bf120782-1aee-4b8c-aa6a-f6223a546e35"), "MASTER", 20, "2226" },
                    { new Guid("c06633be-2f73-4fc2-9765-3812e9cc991e"), "ELO", 10, "650912" },
                    { new Guid("c0a360dc-7399-455b-ad53-9103c42702b9"), "DISCOVER", 70, "62219" },
                    { new Guid("c27513d6-d57e-43a2-8647-d079fb8fec13"), "MASTER", 0, "574080" },
                    { new Guid("c2e4dfc9-5879-4b30-92cf-017b2d5cefa3"), "ELO", 10, "509064" },
                    { new Guid("c302dd70-0829-4d81-a1f4-f4dd00c14577"), "VISA", 0, "560780" },
                    { new Guid("c364772c-e29d-418f-9088-21a04d2a8131"), "MASTER", 0, "505316" },
                    { new Guid("c5986fd7-e850-40c9-9047-79f57106d49a"), "HIPERCARD", 60, "38" },
                    { new Guid("c8f0cab3-95de-4989-bf7a-58aa0d95df66"), "VISA", 0, "502121" },
                    { new Guid("cacdf39b-765f-4795-be3a-fcb7214d9cee"), "ELO", 10, "509067" },
                    { new Guid("cb9b52e6-b079-4155-9508-8a7ce918fb0e"), "MASTER", 20, "51" },
                    { new Guid("cd9b4256-bb0b-4c62-8b96-1adb296b0993"), "ELO", 0, "506874" },
                    { new Guid("ce53323d-7965-40ff-9bb9-45dcb5afdde7"), "VISA", 0, "27728" },
                    { new Guid("cf860308-7466-4c8d-be93-f6e6a79429c3"), "MASTER", 0, "330239" },
                    { new Guid("d025fc43-c20e-4982-a46a-d6ab64240ebd"), "ELO", 0, "509410" },
                    { new Guid("d073f1a4-e16d-4e9b-ae76-f7fbd8f4ca5f"), "MASTER", 0, "570209" },
                    { new Guid("d2170b50-fe3d-40db-b080-d662011e4607"), "VISA", 0, "210790" },
                    { new Guid("d252ac93-ec82-4507-86db-895e98cd3d41"), "VISA", 0, "960421" },
                    { new Guid("d2841054-979f-4fd9-9a01-887d4ce0d9eb"), "ELO", 0, "67410" },
                    { new Guid("d2e38307-5b62-46a6-9be1-1e89d81f501f"), "MASTER", 0, "509257" },
                    { new Guid("d4dbde02-b942-4f83-b437-42b7189696ab"), "MASTER", 20, "52" },
                    { new Guid("d51d94c5-dd4a-4ca8-ae0a-61193865aa7a"), "DINERS", 30, "304" },
                    { new Guid("d64fe8ba-89ad-45d8-bae5-06f0864bd9b1"), "ELO", 5, "509023" },
                    { new Guid("d9b2ed55-e962-4114-9b75-2c72f76a6fa5"), "MASTER", 0, "576096" },
                    { new Guid("da10feeb-3dfc-4441-b530-6a96bbab723f"), "MASTER", 0, "970800" },
                    { new Guid("da4ee1ae-4a46-4a7e-b101-dba4cc336dc3"), "VISA", 0, "627892" },
                    { new Guid("dd49c942-b732-45fd-8a6d-2472747c11ca"), "DISCOVER", 70, "622921" },
                    { new Guid("df421d77-42ce-4536-aab0-01b5393806da"), "ELO", 5, "6516" },
                    { new Guid("df792cb3-1851-4a6d-8d40-53380bc4c093"), "ELO", 10, "509049" },
                    { new Guid("dfb83d25-918d-4269-8ba2-aa5c8f782ac3"), "ELO", 10, "509050" },
                    { new Guid("e113ce0a-5f78-4dd4-a2a7-0cdcdcae045e"), "DISCOVER", 70, "62217" },
                    { new Guid("e3e3eee2-ce55-4bbd-b840-7840b72433f6"), "ELO", 5, "6280" },
                    { new Guid("e4956d8a-2a04-4fcd-9d22-57408b61f7cf"), "MASTER", 1, "608783" },
                    { new Guid("e4d939bb-efa9-423c-9436-23c0b045a09c"), "MASTER", 0, "126820" },
                    { new Guid("e605cac4-fce8-4d4e-910e-67ffc13d719a"), "DISCOVER", 70, "6225" },
                    { new Guid("e71142f4-c940-4cde-8c37-475778526b0a"), "MASTER", 0, "720358" },
                    { new Guid("e78ba8b1-97a8-48c0-9903-d745fe64fd10"), "MASTER", 20, "2225" },
                    { new Guid("e9473e88-1553-4235-bcf8-9c390a7840b9"), "MASTER", 20, "226" },
                    { new Guid("e97c6e32-8477-453b-a893-143781fbd35b"), "MASTER", 20, "53" },
                    { new Guid("ea094b8e-5cb4-4488-b71d-7f1f199bf893"), "ELO", 0, "509004" },
                    { new Guid("ea837aed-c58b-4ddb-a85b-74b750b8614a"), "MASTER", 0, "537993" },
                    { new Guid("eb9144ff-fde6-4400-917d-c3bae245ed37"), "ELO", 10, "4011" },
                    { new Guid("eccc2b67-0a24-4d3b-bf24-7353255d752c"), "MASTER", 20, "224" },
                    { new Guid("ed5a2d66-2676-4018-a3a5-3c12064145a0"), "ELO", 0, "680405" },
                    { new Guid("ef493529-5de0-4b11-bd6c-04e0a827d244"), "DISCOVER", 70, "62215" },
                    { new Guid("f00923b3-dd48-43bb-999e-a2fc1e3b36e1"), "MASTER", 0, "44027" },
                    { new Guid("f024b2de-3ea8-4f42-8916-18b964a7c60c"), "MASTER", 0, "560033" },
                    { new Guid("f4a35792-64d5-4a86-8b68-736d58edc789"), "MASTER", 0, "596497" },
                    { new Guid("f943faed-c3fc-4b8e-90fa-2b1a584479fc"), "ELO", 10, "5067" },
                    { new Guid("fa35ebc4-ae95-4755-bf8c-5487fe3742ee"), "MASTER", 0, "55909" },
                    { new Guid("fa74a800-6f01-477f-979a-ee4d9429a513"), "DISCOVER", 70, "622924" },
                    { new Guid("fb1dcd78-7d83-4493-805a-b5c9f66cec28"), "DINERS", 30, "300" },
                    { new Guid("fb4d74ed-d7d8-4f65-aea2-7fec10fa9d17"), "MASTER", 0, "316811" },
                    { new Guid("fd627daa-421a-4ed6-b947-6894887cb8c0"), "DISCOVER", 70, "6226" }
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
