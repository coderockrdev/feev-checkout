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
                    { new Guid("0103cc08-7c06-4484-82d1-a6f5053d604a"), "MASTER", 20, "25" },
                    { new Guid("01dd051b-9748-447b-b18c-7c09d5b97ea4"), "MASTER", 0, "546479" },
                    { new Guid("027b4a74-656d-46c6-ab8a-9cdfcd7e7840"), "MASTER", 0, "537993" },
                    { new Guid("0408c5f1-35ee-4a11-8c5f-86bbc51c845e"), "ELO", 5, "6516" },
                    { new Guid("048d4fa1-5fff-43b0-becf-c5cf3985f288"), "ELO", 0, "509294" },
                    { new Guid("059ac16b-97f2-40fb-9fab-f5941b086a6c"), "MASTER", 0, "640245" },
                    { new Guid("05c93df6-fa45-4ddd-af60-5cff4acb9261"), "ELO", 0, "509156" },
                    { new Guid("061ae4df-537e-4172-95c0-0e4ddf47c2ab"), "ELO", 10, "509091" },
                    { new Guid("07220f86-40c3-4cf8-b808-8f2ff71ed1fb"), "ELO", 10, "451416" },
                    { new Guid("087325c9-4153-4ba9-921c-8e7b6a9b7b11"), "DINERS", 30, "300" },
                    { new Guid("088760b9-e4d1-4d2f-b3b1-fcb46ff3db1c"), "HIPERCARD", 80, "606282" },
                    { new Guid("08f0118f-01a3-40ea-8a7a-87b7f0dde3cb"), "MASTER", 0, "570209" },
                    { new Guid("098048d4-5b95-4d37-9bac-e4a0f1d157a3"), "ELO", 10, "636368" },
                    { new Guid("0dede810-1393-4184-9d29-809226dc821e"), "MASTER", 0, "509257" },
                    { new Guid("0e1d995e-72e5-429f-b5bd-2771c5e258aa"), "MASTER", 20, "2228" },
                    { new Guid("0e736326-f548-4441-a03c-080e1592de58"), "MASTER", 0, "720358" },
                    { new Guid("0ebcb56a-2f6a-4a1a-8968-567e8cbdfa9b"), "MASTER", 0, "639350" },
                    { new Guid("12104118-8715-4c7a-8a09-f618be1681fb"), "MASTER", 20, "51" },
                    { new Guid("1267b22c-c2e0-497d-a7d5-9273d825e2ee"), "JCB", 90, "2131" },
                    { new Guid("1316f48d-45a0-422c-8052-194cc4058f17"), "ELO", 10, "650485" },
                    { new Guid("13330e1d-0cbd-489e-b6f9-e1ed7e1a341a"), "MASTER", 0, "560033" },
                    { new Guid("1500bd31-b2fe-4d46-bec1-e7404593cc77"), "ELO", 10, "650487" },
                    { new Guid("15bf01c9-1a14-4087-89d8-9a9d56abb717"), "DINERS", 30, "36" },
                    { new Guid("160a7892-5bfc-469f-a7ab-7b936c581b87"), "ELO", 10, "509045" },
                    { new Guid("1823e450-8874-4cc1-9de4-08523b8a4f3f"), "ELO", 10, "509064" },
                    { new Guid("1952ec49-7875-4b40-8add-56096f303527"), "ELO", 10, "50906" },
                    { new Guid("1c4ad172-a014-45ef-903d-408e79b602cc"), "MASTER", 0, "830050" },
                    { new Guid("1c9979a2-e1f7-4e15-a1e4-0476746eed26"), "ELO", 1, "6507" },
                    { new Guid("1f9eec56-2c92-4ec3-b8b2-be1f0a6692d1"), "DISCOVER", 70, "65" },
                    { new Guid("205b2eda-3c00-41c3-8f88-4d05c385c72b"), "ELO", 10, "650507" },
                    { new Guid("2068770b-7b3e-423a-9e9f-f5806af532a4"), "MASTER", 20, "52" },
                    { new Guid("2079b6de-05eb-41dd-9ba2-0b6af151f4f7"), "ELO", 10, "509066" },
                    { new Guid("2100eb4c-5756-442e-a9ae-49e8e5bd2bf2"), "DISCOVER", 70, "62219" },
                    { new Guid("22312464-a6e6-4ac5-a22d-11f3239c236d"), "DISCOVER", 70, "622922" },
                    { new Guid("2310ce2a-0cef-4e59-abad-9151ab7f8f86"), "ELO", 10, "65090" },
                    { new Guid("24942dd0-ed48-457a-95d6-a095c697555b"), "ELO", 10, "504175" },
                    { new Guid("2a64a61c-74f9-4559-8d3c-98e8ef70b968"), "ELO", 0, "509284" },
                    { new Guid("2e241a05-fc6b-4168-956e-3475acb11b4a"), "ELO", 10, "650494" },
                    { new Guid("2eae6dd4-3a84-4a06-b7c2-40f556a43c6f"), "CABAL", 60, "6042" },
                    { new Guid("2f31117e-0f1f-4c3a-ae85-6292677e20c4"), "JCB", 90, "1800" },
                    { new Guid("303fecf6-07f6-49fd-b130-7754381ba3a5"), "DISCOVER", 70, "622925" },
                    { new Guid("31466289-a64f-4ef4-8048-6a7b863384ad"), "MASTER", 0, "589916" },
                    { new Guid("32d81310-395c-44b3-8b28-0983cfb5fc66"), "DISCOVER", 70, "649" },
                    { new Guid("32dbc4a8-7102-46d1-9e16-150e7d470369"), "DISCOVER", 70, "6223" },
                    { new Guid("33e22d21-5b1e-42bb-a904-0863a0f5a0b0"), "MASTER", 20, "228" },
                    { new Guid("342af0d3-a1c0-4d5b-a112-b0455e9b2ab3"), "DISCOVER", 70, "62215" },
                    { new Guid("3496fd21-a699-4136-9138-58cbf5c58022"), "DISCOVER", 70, "622920" },
                    { new Guid("34ad1e86-756a-49f1-8cf6-d144eb637ae4"), "VISA", 0, "798431" },
                    { new Guid("3865bdf2-3641-4d89-a810-ee32ed6265a5"), "MASTER", 0, "316811" },
                    { new Guid("3868ef5b-44f4-48aa-a991-27b67178233f"), "ELO", 10, "650921" },
                    { new Guid("39bfb275-75cf-4f7f-a636-998cd110d5e2"), "ELO", 0, "6538" },
                    { new Guid("3a680ea8-9065-4cef-ad90-f945f7de4ad8"), "MASTER", 0, "5453" },
                    { new Guid("3c897b37-f1f7-41b6-8808-00e4fd6db6c1"), "MASTER", 0, "566269" },
                    { new Guid("403cdeab-2262-43ba-8291-4b6eb8069521"), "ELO", 0, "509423" },
                    { new Guid("40452bdf-a81f-4366-85fa-ebb00b013d2c"), "ELO", 10, "509040" },
                    { new Guid("41f8227b-2018-4bc8-9be7-33e879231c6a"), "MASTER", 1, "750209" },
                    { new Guid("481ea7cf-0931-4f88-b61a-109cf23a17af"), "VISA", 0, "985966" },
                    { new Guid("4a0b9d14-2669-4a39-8417-8ccedbfdff0b"), "DISCOVER", 1, "306262" },
                    { new Guid("4e44f826-0905-4c14-9570-667f45314af9"), "MASTER", 0, "502121" },
                    { new Guid("4e7bb92d-c26f-4b84-99fc-e2d68abc7446"), "MASTER", 0, "574080" },
                    { new Guid("509a4f19-82b9-4d75-88ef-d84ebd16781a"), "ELO", 10, "509052" },
                    { new Guid("51abab36-4a14-46a5-a2fe-2ccb6ce57a72"), "MASTER", 0, "3006" },
                    { new Guid("51fc59fa-38ed-475e-bfa3-7ef22efd197f"), "ELO", 10, "509059" },
                    { new Guid("52d7222a-5e9b-4a01-be92-2cc239449f09"), "DISCOVER", 70, "622923" },
                    { new Guid("5384e93c-9d3d-4eee-9718-b4c824deb41a"), "ELO", 5, "6505" },
                    { new Guid("54283d8d-6149-4d71-8aab-978dcf7cfcd6"), "VISA", 0, "970800" },
                    { new Guid("5477e2e8-99a5-421c-bc51-befcef0a3e0d"), "MASTER", 20, "2223" },
                    { new Guid("54eb1635-6a00-4f52-bef2-684a1b26ecda"), "MASTER", 20, "224" },
                    { new Guid("57054b54-2b5b-410b-9dff-e7a8f89a0a6d"), "ELO", 10, "509050" },
                    { new Guid("575fd7a7-cbaf-4b5e-b65b-20c8c6a0b00c"), "MASTER", 0, "615292" },
                    { new Guid("5ba8cd2b-160e-4ddb-9d9e-561d2f9ebc9b"), "MASTER", 0, "55909" },
                    { new Guid("5d38ad8f-caec-4857-b133-a706e4626c2a"), "ELO", 10, "509074" },
                    { new Guid("5d960482-3969-4c87-be29-17d5c43a2e55"), "MASTER", 20, "226" },
                    { new Guid("5e4a1ae8-f398-409a-93e1-435a0f0cac27"), "VISA", 5, "4984" },
                    { new Guid("5f31dd25-27db-4bf3-bf05-89fb535c4b87"), "ELO", 0, "502577" },
                    { new Guid("5f6b3239-1fcd-4bba-b9e5-08d431c0405e"), "VISA", 0, "40064" },
                    { new Guid("614b0a37-52dc-49ac-85d1-9110b54cd8de"), "HIPERCARD", 60, "38" },
                    { new Guid("61db91b8-953a-4734-bad9-3d1bcf98b22b"), "VISA", 0, "33994" },
                    { new Guid("61f52435-4e56-4461-90dc-4fdfe91dd708"), "MASTER", 0, "960379" },
                    { new Guid("632cd6db-9c30-4a66-b787-3b9dbcd36ed8"), "ELO", 10, "650912" },
                    { new Guid("64d678b6-65e6-4018-966d-2e9530a36aeb"), "AMEX", 40, "36" },
                    { new Guid("659cb6ca-ba9c-41e2-89ee-e6ce1edbb831"), "ELO", 10, "509049" },
                    { new Guid("65f70d51-92ff-4618-9012-ce798659818b"), "MASTER", 0, "126820" },
                    { new Guid("66c74f33-b738-4c2e-bd24-dc6a63b7e3aa"), "MASTER", 20, "53" },
                    { new Guid("67e560e6-51ad-4de2-b9bd-9f76f4012d72"), "AMEX", 40, "35" },
                    { new Guid("6898476d-4a09-4f0c-9911-ed18b86e97f7"), "MASTER", 20, "2221" },
                    { new Guid("68b87a6f-dd97-4a42-8d32-21d5d58ac7d8"), "MASTER", 0, "505381" },
                    { new Guid("6b97a641-4e01-458f-88a1-c4756cb236ba"), "DISCOVER", 70, "622129" },
                    { new Guid("6de3804d-4ce7-4435-aeb5-4258771a3d9d"), "NARANJA E NEVADA", 100, "589562" },
                    { new Guid("6ef6af52-2f75-432c-a125-f93f2b4806b7"), "VISA", 0, "960421" },
                    { new Guid("6f93f2a0-bad8-4855-9c8a-9f3559e3a9bf"), "MASTER", 0, "220209" },
                    { new Guid("703cc93b-eb8e-4ce3-b4ed-752fca750c98"), "MASTER", 0, "596497" },
                    { new Guid("7225a73b-1a07-4172-aa28-a0f1a43fdd1e"), "DISCOVER", 70, "622127" },
                    { new Guid("75e450fa-fce8-4520-b33b-7e882c9d685f"), "MASTER", 0, "585988" },
                    { new Guid("76e5495b-1c3a-4ce2-9a45-145fde4e509a"), "MASTER", 0, "507641" },
                    { new Guid("78ff5b1b-57eb-4ce2-88f3-fbf236e18279"), "VISA", 0, "854644" },
                    { new Guid("79f2ef93-1804-459b-8147-4c3fa8db191f"), "ELO", 10, "36297" },
                    { new Guid("7a0523bd-735c-417f-bad1-27ad67d1ac43"), "MASTER", 20, "24" },
                    { new Guid("7a921757-5c3d-4a5c-92f4-ffb9d1a73d15"), "MASTER", 0, "517349" },
                    { new Guid("7ac13fff-0453-44a8-8af0-da3b4aea8e14"), "ELO", 0, "650042" },
                    { new Guid("7cdfb881-bbde-4835-a641-dedf9a53ce88"), "ELO", 0, "699988" },
                    { new Guid("7e9a101e-dfe7-4298-a4a2-387aa3b80c64"), "MASTER", 20, "225" },
                    { new Guid("815bb75c-5a19-4f9c-8e46-18e3eb2124cb"), "HIPERCARD", 0, "506282" },
                    { new Guid("815cf975-492f-43ca-bec3-0c43dc060b91"), "DISCOVER", 70, "62217" },
                    { new Guid("82367ba3-735b-4ae4-a6dd-3ff197204db4"), "ELO", 0, "680405" },
                    { new Guid("82e81022-111f-49a2-aac3-43691f62cb53"), "ELO", 10, "4576" },
                    { new Guid("83106136-02d3-4a68-8ee9-d1777419301e"), "DINERS", 30, "302" },
                    { new Guid("84f7f5a9-20af-4470-b792-75acd7ee3233"), "DINERS", 30, "305" },
                    { new Guid("85072cd6-f40d-4140-a932-2cc43db6364f"), "DISCOVER", 70, "622128" },
                    { new Guid("8799baa1-c5ac-4311-a287-2c90203d16b7"), "DINERS", 0, "605919" },
                    { new Guid("88a01aa4-67cc-4038-a592-ae58e02e2ab3"), "DISCOVER", 70, "6222" },
                    { new Guid("88f3d8ac-c199-4241-9f09-8e7cbb056ebf"), "ELO", 0, "509730" },
                    { new Guid("892cf9af-6071-4c19-9f4d-769ab3a43bf0"), "VISA", 0, "48546" },
                    { new Guid("8ef050ac-1ed8-43ca-9633-3a02549f2bfb"), "VISA", 110, "4" },
                    { new Guid("8f04b17b-4c9d-47ce-836a-67dc1db40b1a"), "JCB", 90, "35" },
                    { new Guid("8fdaecd6-eee1-4177-91ca-94996b9c08a2"), "MASTER", 20, "2222" },
                    { new Guid("903a59f0-5174-43d3-bb15-bc945a1cb947"), "ELO", 4, "506741" },
                    { new Guid("91475940-8275-45a4-b541-84c3a573f4ac"), "ELO", 0, "509093" },
                    { new Guid("92d42519-20c2-4511-88f8-e885035a5544"), "HIPERCARD", 80, "3841" },
                    { new Guid("945975b4-c869-4ee5-8a54-ed664a0a7092"), "ELO", 5, "6504" },
                    { new Guid("95b4e502-a5e0-4d19-96c3-fdea0c4246d2"), "DISCOVER", 70, "622924" },
                    { new Guid("95e8bd16-68ac-43d1-9bc8-7f1d4469fdfb"), "MASTER", 0, "595922" },
                    { new Guid("95edd5a0-1e1b-420c-a1ee-b4a1e4360a61"), "AMEX", 40, "37" },
                    { new Guid("98cfde2c-73d7-410e-abe1-11ec07995bf6"), "ELO", 0, "509410" },
                    { new Guid("99d5fa65-a08e-4722-a972-a23e0b1f1bce"), "ELO", 10, "509046" },
                    { new Guid("9b4c2e5e-47a8-44e5-8af7-d9c737900528"), "VISA", 0, "121772" },
                    { new Guid("9e48b628-c17d-415c-825c-7ab63abc1158"), "MASTER", 0, "439133" },
                    { new Guid("a04042b2-6d1e-4939-b771-8fbe157e6277"), "ELO", 0, "650920" },
                    { new Guid("a07277cc-e441-413b-8554-31d99cc02c61"), "ELO", 5, "6550" },
                    { new Guid("a1256b81-5da8-44f0-b6e4-4a07a0d1a8e5"), "MASTER", 1, "608783" },
                    { new Guid("a299b9b1-a0f4-46da-8c52-f815b50d835b"), "VISA", 0, "444125" },
                    { new Guid("a3c2c1f4-8dde-402a-b7ff-68872be45c50"), "MASTER", 0, "212743" },
                    { new Guid("a4706497-f989-4df9-8c26-4aec0f3da14a"), "DISCOVER", 70, "6011" },
                    { new Guid("a483349d-23b1-4efb-85ca-3bb912a3becb"), "MASTER", 0, "576096" },
                    { new Guid("a7c26d98-1469-4753-8eb9-25022d98ca98"), "MASTER", 0, "576292" },
                    { new Guid("a7e7a14a-e5cc-47cb-b247-ba355664c21a"), "DISCOVER", 70, "62213" },
                    { new Guid("a8bdc9f1-83b6-4680-85e5-faf1d5f3972a"), "MASTER", 0, "7807" },
                    { new Guid("a90a56f9-c6b0-4f71-a20a-27a6a572da42"), "MASTER", 20, "2227" },
                    { new Guid("ab9fbafd-dfcf-4f93-91bf-3ce3b5af8e3a"), "VISA", 0, "560780" },
                    { new Guid("aceb18fe-99a6-4a35-91b8-d0dd83dae4cb"), "AMEX", 40, "34" },
                    { new Guid("ae99dfec-3235-476e-90ac-184f0303d2e3"), "MASTER", 0, "560780" },
                    { new Guid("af5291eb-c9d1-43c9-9dcf-e091accd92fd"), "DISCOVER", 70, "644" },
                    { new Guid("b062ccbd-f8ae-42d4-a036-93ffd390d483"), "MASTER", 0, "320132" },
                    { new Guid("b0ad50ec-ad85-4522-90d5-a8bc5a665107"), "ELO", 10, "509069" },
                    { new Guid("b0f9f7cc-df93-46ff-b0be-8688a941c940"), "MASTER", 0, "44027" },
                    { new Guid("b14b8f2d-beed-483e-9d26-8fd88665fe4a"), "MASTER", 20, "2226" },
                    { new Guid("b2e5fda3-90f5-4d95-bfe6-c266377a3ea1"), "DISCOVER", 70, "62216" },
                    { new Guid("b31e9b1a-ce6f-4420-8793-033e10c7ac3a"), "MASTER", 0, "505316" },
                    { new Guid("b3d9ab0b-75a7-484c-97dd-610c312428a7"), "DINERS", 30, "304" },
                    { new Guid("b6e31761-9896-4ed2-86ce-af5f18646d38"), "MASTER", 0, "608710" },
                    { new Guid("b7953602-f2c7-45aa-8ab2-6de218052755"), "VISA", 0, "27728" },
                    { new Guid("b90bed1e-eb1a-415d-9830-ed8762322a1a"), "ELO", 10, "409835" },
                    { new Guid("b999fbc9-b7ff-43a8-9070-b34dc0535fec"), "VISA", 0, "107915" },
                    { new Guid("bc676852-e91d-460d-a807-1b4bcf307f12"), "MASTER", 20, "2225" },
                    { new Guid("bd245a24-6d66-4e49-b472-47de7223197d"), "VISA", 0, "984428" },
                    { new Guid("be546703-a100-4bef-838d-69049919cf6f"), "MASTER", 20, "271" },
                    { new Guid("bea36cbc-dc2a-4275-8cf3-2d121ace428b"), "ELO", 10, "4011" },
                    { new Guid("bef7804f-070b-4681-b581-1f49ba2480aa"), "DINERS", 30, "303" },
                    { new Guid("bf283f74-b125-4638-8abb-dc79ed838347"), "ELO", 0, "6509" },
                    { new Guid("bfe6cec8-639d-4208-901d-a4b325c51e61"), "ELO", 5, "509023" },
                    { new Guid("c03ac10f-7e5d-483d-bd32-6d048971ffd9"), "ELO", 10, "438935" },
                    { new Guid("c1263f10-5c1b-4a2b-b05e-689cb4d96b3d"), "MASTER", 0, "970800" },
                    { new Guid("c1f1facb-eb39-4e46-aa9b-24cc89958e02"), "VISA", 0, "502121" },
                    { new Guid("c231fabb-2877-4539-8d46-c116e1ebe5a0"), "MASTER", 20, "2720" },
                    { new Guid("c24c41b4-5fa8-489e-a490-3626916aed04"), "ELO", 0, "506874" },
                    { new Guid("c595eb0b-ac0a-45f3-bd9e-5b81111adc2a"), "MASTER", 0, "637095" },
                    { new Guid("c63cb1fc-1113-48ed-9254-6d59994b8cfb"), "DISCOVER", 70, "62218" },
                    { new Guid("c763250f-ed31-4e6b-ba19-c965fdb9cea7"), "MASTER", 0, "569970" },
                    { new Guid("c856a63e-8489-4ddc-8ce4-aa86835cd54c"), "ELO", 10, "5067" },
                    { new Guid("c87a9cb4-99d3-4e8d-a24d-9dd3c6eaa7c9"), "MASTER", 20, "2229" },
                    { new Guid("c8b94981-985a-4429-b2d4-ffb052a5d874"), "MASTER", 20, "55" },
                    { new Guid("c8cb5834-8ff1-4b4d-938b-fa3a730dc9a3"), "DISCOVER", 70, "62214" },
                    { new Guid("c9231aba-0ace-4dd1-b866-f1321282e05f"), "DISCOVER", 70, "62290" },
                    { new Guid("cabdec57-49b3-45a9-bacd-8a6f03dfc301"), "MASTER", 20, "229" },
                    { new Guid("cb148a01-163a-4418-9f84-4079d1e73c20"), "MASTER", 20, "2224" },
                    { new Guid("cb90d1f2-768b-445c-8a63-f20591bd5e2e"), "ELO", 0, "640485" },
                    { new Guid("cd25a2d3-cb26-437c-a641-d78ae058e398"), "ELO", 10, "650491" },
                    { new Guid("ce6d9e37-985d-4679-8884-9c8c806c325e"), "ELO", 0, "509010" },
                    { new Guid("d045e1a1-ec32-4a3f-9102-9b0918f2e1ae"), "DISCOVER", 70, "6225" },
                    { new Guid("d047e37d-63bb-438d-ae24-044c21817612"), "ELO", 10, "509000" },
                    { new Guid("d156efbf-3c80-4654-b512-9e8544f7d173"), "ELO", 0, "67410" },
                    { new Guid("d25a8d96-b8b6-4e83-837d-ffdee9d0314e"), "ELO", 0, "627780" },
                    { new Guid("d3c145fc-4ef5-4071-9453-f55860569fbe"), "MASTER", 20, "227" },
                    { new Guid("d420ba9d-d31c-4951-9ca0-4b5ae1c8e3c9"), "ELO", 10, "509042" },
                    { new Guid("d42be9f9-c32c-4d4d-b08d-ac41c49b9716"), "DISCOVER", 70, "647" },
                    { new Guid("d442fb00-ca4d-4fd5-8445-4d7d17b828e1"), "MASTER", 20, "54" },
                    { new Guid("d54591b8-2886-47b8-ae6f-6033b3839d48"), "VISA", 0, "186307" },
                    { new Guid("d60c7d36-5b82-44e8-a20c-7090b14760a4"), "MASTER", 0, "502309" },
                    { new Guid("d6251d2e-565a-4dbd-b3db-e6dab45abd64"), "ELO", 0, "509004" },
                    { new Guid("d716abff-488f-4ea0-9508-fbebbf05fddb"), "MASTER", 1, "515894" },
                    { new Guid("d793b0aa-79ed-43b2-bb4d-fd59fd829e62"), "MASTER", 0, "330239" },
                    { new Guid("d7941097-958e-4415-a812-4e06ef8b6856"), "ELO", 10, "50904" },
                    { new Guid("d8adce02-94a1-46b6-b0d2-405ffb9aa06e"), "DISCOVER", 70, "6224" },
                    { new Guid("d9199f23-668c-4019-ab96-6319b17f9425"), "DISCOVER", 70, "645" },
                    { new Guid("da38e5da-6062-44ab-ba6b-e63676f8285a"), "VISA", 0, "627892" },
                    { new Guid("da4daec6-963a-4654-ae13-bc4b99654d1c"), "VISA", 0, "566269" },
                    { new Guid("dad6fddf-43e4-4e41-8a7f-7599818f84c6"), "MASTER", 0, "509059" },
                    { new Guid("dfac0de8-9c0d-4aa5-b1c7-df2b3fa4b3f5"), "ELO", 10, "509040" },
                    { new Guid("e1766abc-6e9b-4fcf-aa65-b391825ebbef"), "MASTER", 20, "26" },
                    { new Guid("e1f465c7-8077-4820-90cf-854cd5f98fc4"), "DISCOVER", 70, "648" },
                    { new Guid("e2545b32-b1ee-4975-9dcd-8442e1152ba3"), "DISCOVER", 70, "646" },
                    { new Guid("e2ab1c80-8fa3-4316-bd27-6c754db0a263"), "ELO", 5, "6280" },
                    { new Guid("e2c4c2cf-b9eb-43bf-8209-72f3a914c884"), "MASTER", 20, "23" },
                    { new Guid("e65e5c8e-0576-4e1c-8629-7535c996bebb"), "VISA", 0, "636355" },
                    { new Guid("e7b7774e-5cf7-4b74-880b-fbeda90773c3"), "MASTER", 0, "53508" },
                    { new Guid("e993c560-70ac-4b38-9e02-bc556776c6a9"), "ELO", 1, "509274" },
                    { new Guid("e9ac6cfc-bced-4e62-9eef-863b1e710193"), "ELO", 10, "650905" },
                    { new Guid("eaf2526b-34e1-4aad-b6c1-ef6e944db47f"), "ELO", 10, "509043" },
                    { new Guid("ec8cade7-e96f-4d63-8119-14bb9f57b847"), "ELO", 0, "509410" },
                    { new Guid("ec902657-8095-4a27-8b30-329f6790c944"), "VISA", 0, "210790" },
                    { new Guid("edd4843a-0241-4b16-b9f3-1d538a05419b"), "ELO", 0, "508757" },
                    { new Guid("f1ba3ab3-eae9-4086-90c8-0b5f3ef70fa4"), "DISCOVER", 70, "62291" },
                    { new Guid("f22d4fbc-1817-4708-a625-3c3f382b07ba"), "MASTER", 20, "223" },
                    { new Guid("f2f87403-1f00-4255-b413-3589f06814ac"), "DINERS", 30, "301" },
                    { new Guid("f3a08d27-a025-4033-881e-76f63c0069d3"), "MASTER", 0, "502299" },
                    { new Guid("f4469007-7a5d-4c3a-b677-99781db4ce01"), "ELO", 10, "50904" },
                    { new Guid("f5177978-b80c-481d-ae26-186ee5b009a7"), "DISCOVER", 70, "6228" },
                    { new Guid("f53a92e3-6a45-44dc-a7e6-f30c6df6b4f9"), "ELO", 0, "111111" },
                    { new Guid("f54543bd-b62a-4605-8198-3775850f350b"), "MASTER", 0, "5379" },
                    { new Guid("f5dfbc4b-c1bc-4ba4-874f-6809a03342fd"), "DISCOVER", 70, "6227" },
                    { new Guid("f8b906d8-90a5-452f-89b7-6b545ae9d862"), "ELO", 10, "509067" },
                    { new Guid("f98ed4ec-c970-4977-a143-a0b0ac8bb944"), "ELO", 1, "650914" },
                    { new Guid("f98fdbbb-5951-4581-bead-52afffbfa96b"), "DISCOVER", 70, "622921" },
                    { new Guid("fa547b11-ce6f-48cd-80da-0e60fd688803"), "ELO", 0, "509005" },
                    { new Guid("fa983b50-41b5-4ab7-adf6-f816be20395d"), "DISCOVER", 70, "6226" },
                    { new Guid("fb746902-bd84-49e6-8d7a-3b0fdaf16c76"), "MASTER", 0, "960397" },
                    { new Guid("fc034721-d459-4dd2-82b2-da51c8743c7c"), "DISCOVER", 70, "622126" },
                    { new Guid("fd364ef7-5e1d-4006-a790-76192a84ca55"), "VISA", 0, "988442" },
                    { new Guid("ff84d469-644d-4cda-a689-c52b23544007"), "ELO", 10, "509051" }
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
