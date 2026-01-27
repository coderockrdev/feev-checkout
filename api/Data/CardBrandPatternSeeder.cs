using FeevCheckout.Models;

namespace FeevCheckout.Data;

public static class CardBrandPatternSeeder
{
    public static IEnumerable<CardBrandPattern> GetSeeds()
    {
        return
        [
            // AMEX
            new CardBrandPattern
                { Id = Guid.Parse("f2dd52bd-da7c-4d38-a3fe-6595835fbe9d"), Brand = "AMEX", Prefix = "34", Order = 40 },
            new CardBrandPattern
                { Id = Guid.Parse("5127c77a-e485-474e-865f-d55d16104090"), Brand = "AMEX", Prefix = "35", Order = 40 },
            new CardBrandPattern
                { Id = Guid.Parse("b1e470be-3bb1-4cd0-ade8-4f9fa46c2a3a"), Brand = "AMEX", Prefix = "36", Order = 40 },
            new CardBrandPattern
                { Id = Guid.Parse("2455bd06-47c5-4b20-a22b-b77010f9386e"), Brand = "AMEX", Prefix = "37", Order = 40 },

            // CABAL
            new CardBrandPattern
            {
                Id = Guid.Parse("f4db535d-ce27-4ebe-8d01-9771b81b0c11"), Brand = "CABAL", Prefix = "6042", Order = 60
            },

            // DINERS
            new CardBrandPattern
            {
                Id = Guid.Parse("bce2bf7f-e123-483a-9ec2-9f1265c81f69"), Brand = "DINERS", Prefix = "605919", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("04029e09-c56a-4849-946b-cffb23e8dbb4"), Brand = "DINERS", Prefix = "300", Order = 30
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("cd33e4b6-ffd7-48f5-bf7a-33d9a1416655"), Brand = "DINERS", Prefix = "301", Order = 30
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("9f3280c6-280e-40d2-a92b-bbe3d2b7e1cd"), Brand = "DINERS", Prefix = "302", Order = 30
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ffdc5e98-c34f-46c7-89c6-ef762b6e53eb"), Brand = "DINERS", Prefix = "303", Order = 30
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("4993c42d-cef1-40d6-a5d3-5ba3b41a3f9f"), Brand = "DINERS", Prefix = "304", Order = 30
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("a455223d-54e5-4459-8c0f-1d8a62481346"), Brand = "DINERS", Prefix = "305", Order = 30
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("894cc484-c9af-4cec-8f05-681be73a7994"), Brand = "DINERS", Prefix = "36", Order = 30
            },

            // DISCOVER
            new CardBrandPattern
            {
                Id = Guid.Parse("1895b3bf-ecc6-42c4-bb1f-5402b6aa9d62"), Brand = "DISCOVER", Prefix = "306262",
                Order = 1
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("377446b9-fd5b-484c-b0b0-71f5b5eec73a"), Brand = "DISCOVER", Prefix = "6011", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("eafb8d4a-2fdc-454e-a659-0ae822e64022"), Brand = "DISCOVER", Prefix = "65", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("1c84372b-2eee-4414-a07b-e912581da602"), Brand = "DISCOVER", Prefix = "644", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("36de40c9-b74e-4d55-ab5a-82b4e2b6ff47"), Brand = "DISCOVER", Prefix = "645", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("b6f7207a-21b9-4fb0-81e9-3e55d77ddfcf"), Brand = "DISCOVER", Prefix = "646", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("59057700-ef58-4bd6-a2e0-c0be0985db07"), Brand = "DISCOVER", Prefix = "647", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("2981e6f9-3475-44af-ad56-2af0d52bce8a"), Brand = "DISCOVER", Prefix = "648", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("cd5a8a4d-3fe5-46c8-9bf4-ff632ed4a3e7"), Brand = "DISCOVER", Prefix = "649", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("bb547111-3fa8-496a-8852-45a2e19eaf5e"), Brand = "DISCOVER", Prefix = "622126",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("fe106cea-d47a-4744-aaeb-eeaac4c1297d"), Brand = "DISCOVER", Prefix = "622127",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("215d0f4e-6f46-4c1a-8018-87d143b4baea"), Brand = "DISCOVER", Prefix = "622128",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("5f23e025-81db-445a-810b-c2d89c3b0d21"), Brand = "DISCOVER", Prefix = "622129",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ddbf27fc-21d8-444c-b93b-47a6f713bcf6"), Brand = "DISCOVER", Prefix = "62213",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("4eaf9524-7252-4b25-8a24-071a8d979413"), Brand = "DISCOVER", Prefix = "62214",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("b11ac102-5dfd-4b8b-a4e8-d0e9204436fb"), Brand = "DISCOVER", Prefix = "62215",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("2e32e13e-f65b-4c43-b439-e497b7ffaa57"), Brand = "DISCOVER", Prefix = "62216",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("2ebd5eaf-c805-4446-b317-4f5f755136c8"), Brand = "DISCOVER", Prefix = "62217",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("5b204185-88c2-4e16-a7b8-ce96e52d5779"), Brand = "DISCOVER", Prefix = "62218",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("266c0ebf-210d-4c33-8b45-fe2f7edd6fd3"), Brand = "DISCOVER", Prefix = "62219",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e1f7e6f5-13e6-4b66-ad5c-5b66ed179356"), Brand = "DISCOVER", Prefix = "6222", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("1ea5f343-1f19-4c43-a9f5-a2a3adbbb6dc"), Brand = "DISCOVER", Prefix = "6223", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("baa161f5-9fac-408f-a32e-4b4492924167"), Brand = "DISCOVER", Prefix = "6224", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("6479e443-5840-4b6d-ac0a-f3ad0ce80e4d"), Brand = "DISCOVER", Prefix = "6225", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("2139bc42-1b72-4dfd-bf7e-157c7a167649"), Brand = "DISCOVER", Prefix = "6226", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("82be5869-6fee-4782-8961-024a1e2473e7"), Brand = "DISCOVER", Prefix = "6227", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("93699de3-edd6-4024-a5a7-bf8f936ca867"), Brand = "DISCOVER", Prefix = "6228", Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("fa03dcfd-1f17-44b9-aa39-433ea948bfa3"), Brand = "DISCOVER", Prefix = "62290",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("fe1a8677-1d09-459a-803d-e08ada083a65"), Brand = "DISCOVER", Prefix = "62291",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("8a456406-2c12-4a59-8ea5-0e2454f71796"), Brand = "DISCOVER", Prefix = "622920",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("baebd91b-9623-4260-ad5b-af4662a10b96"), Brand = "DISCOVER", Prefix = "622921",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f5c201c3-4cd1-4844-b1bf-ac72986cd43a"), Brand = "DISCOVER", Prefix = "622922",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("b661f15e-c32d-4b24-980d-8b2f9f90dbe4"), Brand = "DISCOVER", Prefix = "622923",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("7d440ed1-d01c-481e-b517-05b8cc76c579"), Brand = "DISCOVER", Prefix = "622924",
                Order = 70
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("6c5a79b4-b41e-4b92-acf0-92a6b55abd36"), Brand = "DISCOVER", Prefix = "622925",
                Order = 70
            },

            // ELO
            new CardBrandPattern
            {
                Id = Guid.Parse("1bd1b2df-f667-4853-b4ad-2c7ff84a039e"), Brand = "ELO", Prefix = "627780", Order = 0
            },
            new CardBrandPattern
                { Id = Guid.Parse("cb530f63-d672-49f9-b90e-198c1730c4a4"), Brand = "ELO", Prefix = "67410", Order = 0 },
            new CardBrandPattern
            {
                Id = Guid.Parse("cdce4927-80a9-4b6d-9438-56def8a2b1e6"), Brand = "ELO", Prefix = "111111", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("c1acc363-7ccd-4f43-909c-2796f86f6811"), Brand = "ELO", Prefix = "506874", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("40dade13-2d20-4f0e-8496-1a84dfd86181"), Brand = "ELO", Prefix = "640485", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("1f3a9016-d24b-4c38-8bb0-1ef5db7a6656"), Brand = "ELO", Prefix = "509093", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("22920e15-13f2-4985-adfd-513eb097aaa3"), Brand = "ELO", Prefix = "650042", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e274cbf7-4c1a-4553-b470-c571ecd1f416"), Brand = "ELO", Prefix = "509410", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ed68f39d-f946-4af2-9dcd-a4b7056cae78"), Brand = "ELO", Prefix = "509005", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("dd59877d-b1d8-4aa4-b2f2-dee2992c11fa"), Brand = "ELO", Prefix = "650920", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("bd829626-5c9a-4f14-aa24-46be950051ea"), Brand = "ELO", Prefix = "699988", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("85eac545-decc-4313-a84b-055ec3eb30b7"), Brand = "ELO", Prefix = "508757", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("1226024e-da5b-4ecb-b22b-2e5ccf10e759"), Brand = "ELO", Prefix = "680405", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("0a40107f-db62-41eb-a507-dd13a6c9dbfc"), Brand = "ELO", Prefix = "509423", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("5bde3090-6f6e-43e1-b316-3851f8b0e591"), Brand = "ELO", Prefix = "006538", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("d373b40b-66fa-451b-b5a8-965086723752"), Brand = "ELO", Prefix = "509156", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f128d9c6-a1e8-49c5-b4e1-7ed6ec5dfa49"), Brand = "ELO", Prefix = "509284", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("713fd508-c89b-4894-b9f5-809792cced74"), Brand = "ELO", Prefix = "502577", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("3d2d694d-3d71-4bb3-a358-ddfeb99988e2"), Brand = "ELO", Prefix = "509004", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("61b0ece5-f43f-4bdb-ab94-73ced9107144"), Brand = "ELO", Prefix = "509294", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("56c99f1f-5063-402b-9b37-db9e2cdda98e"), Brand = "ELO", Prefix = "509730", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f6e69e5d-3940-4afb-ad48-b2932e075031"), Brand = "ELO", Prefix = "509010", Order = 0
            },
            new CardBrandPattern
                { Id = Guid.Parse("3c95709d-5f6e-4d3f-95b2-b6d4cb98b074"), Brand = "ELO", Prefix = "6509", Order = 0 },
            new CardBrandPattern
                { Id = Guid.Parse("3b2e392f-8aba-47cb-b38e-6a9783f9c66c"), Brand = "ELO", Prefix = "6507", Order = 1 },
            new CardBrandPattern
            {
                Id = Guid.Parse("d2076b08-7e08-4cc6-a533-37e6eccfd9fc"), Brand = "ELO", Prefix = "650914", Order = 1
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("571ca1dc-63ed-4d4d-bbfe-c00560fe4c08"), Brand = "ELO", Prefix = "509274", Order = 1
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("67c21260-6e79-45bd-9f4d-ff6b64bbf5d8"), Brand = "ELO", Prefix = "506741", Order = 4
            },
            new CardBrandPattern
                { Id = Guid.Parse("919a9134-b3b9-4bf1-9fa0-48e507deb47b"), Brand = "ELO", Prefix = "6505", Order = 5 },
            new CardBrandPattern
                { Id = Guid.Parse("d0a80bae-3f0b-4519-bb13-3619262398a2"), Brand = "ELO", Prefix = "6280", Order = 5 },
            new CardBrandPattern
                { Id = Guid.Parse("e1e68a6e-3d21-4466-a014-a12cd6da4514"), Brand = "ELO", Prefix = "6504", Order = 5 },
            new CardBrandPattern
                { Id = Guid.Parse("bf4969cb-fb5f-4000-8e12-e318d567a0d9"), Brand = "ELO", Prefix = "6550", Order = 5 },
            new CardBrandPattern
            {
                Id = Guid.Parse("7342fa6e-e737-45b3-8844-d85298e7975e"), Brand = "ELO", Prefix = "509023", Order = 5
            },
            new CardBrandPattern
                { Id = Guid.Parse("6fc94aa9-eb2b-43eb-b2dd-87dd5361f16d"), Brand = "ELO", Prefix = "6516", Order = 5 },
            new CardBrandPattern
            {
                Id = Guid.Parse("e7d74a25-fed9-4d49-96d9-ec4b6d4a3891"), Brand = "ELO", Prefix = "650507", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("2c78ebae-3891-4cd6-986b-e80fe208845c"), Brand = "ELO", Prefix = "636368", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("debdbe26-833d-47f6-9de6-d10fbe2af90a"), Brand = "ELO", Prefix = "438935", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("747ece4d-43a5-4793-b659-b3f11c7c7ada"), Brand = "ELO", Prefix = "504175", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("a0cef9b0-5c2c-4966-a242-39ccaa9f1dd2"), Brand = "ELO", Prefix = "451416", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("a5efa71d-7ce2-462b-ba32-e6b0bb6115c1"), Brand = "ELO", Prefix = "50904", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("8d9cd703-0c4f-42b0-8914-2168189ee0fd"), Brand = "ELO", Prefix = "509067", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("51928f8d-621a-4b00-b6fe-dcd956dbe528"), Brand = "ELO", Prefix = "509049", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("75dfd39f-8703-44e1-a04f-7e4b985f0159"), Brand = "ELO", Prefix = "509069", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("83e0cf28-f802-4254-aa1b-1ced172edab6"), Brand = "ELO", Prefix = "509050", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("39a3fb06-b3c5-4148-8251-88105679e9ed"), Brand = "ELO", Prefix = "509074", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("a8b85150-056a-453a-8678-c8d10779d5a1"), Brand = "ELO", Prefix = "50906", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("4ffe5519-7875-4e23-83e2-105477206428"), Brand = "ELO", Prefix = "509040", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("32e917d9-4b27-4e4e-8ede-e4bd461d8f03"), Brand = "ELO", Prefix = "509045", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("09c81bd9-0a53-49f3-af35-7c521df59943"), Brand = "ELO", Prefix = "509051", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("0d537f7f-7bbb-4125-ba56-464212680857"), Brand = "ELO", Prefix = "509046", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("a578ec2e-2492-4c6e-a783-8b5f928cac5a"), Brand = "ELO", Prefix = "509066", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ede7ea0e-8df3-465d-85e9-fb0671743b01"), Brand = "ELO", Prefix = "509042", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e07c6274-1a20-4b3b-9c3b-23ed9a4de815"), Brand = "ELO", Prefix = "509052", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("a1b1e2b6-ac4c-44ef-9f9b-482efef3f410"), Brand = "ELO", Prefix = "509043", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("033d4215-1800-410c-b309-5020a550c571"), Brand = "ELO", Prefix = "509064", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("34d2dd63-540d-402a-bb0b-62cf9dabd64e"), Brand = "ELO", Prefix = "509040", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("9c846466-addc-44a5-a9e9-c0d9ce6c6c55"), Brand = "ELO", Prefix = "36297", Order = 10
            },
            new CardBrandPattern
                { Id = Guid.Parse("42e1eec8-bbcc-494e-bbed-00f938e97d1f"), Brand = "ELO", Prefix = "5067", Order = 10 },
            new CardBrandPattern
                { Id = Guid.Parse("67dfbf5d-1850-42ae-a0f4-661d32f15e52"), Brand = "ELO", Prefix = "4576", Order = 10 },
            new CardBrandPattern
                { Id = Guid.Parse("7b80f7b3-ca2b-4826-945b-2651efbd18ba"), Brand = "ELO", Prefix = "4011", Order = 10 },
            new CardBrandPattern
            {
                Id = Guid.Parse("78607f37-648f-4f44-9996-33ad12a714fa"), Brand = "ELO", Prefix = "509000", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("8b591cfa-258a-4a7d-b619-ee5e9ba36340"), Brand = "ELO", Prefix = "650905", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("7947ba52-2f17-41ad-937b-db6d9f728c6a"), Brand = "ELO", Prefix = "650494", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("888f3cca-091b-4f4b-b375-6967c9e14ff8"), Brand = "ELO", Prefix = "650485", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("fc54a4f1-6a54-4017-8a49-84349154e923"), Brand = "ELO", Prefix = "509091", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("b2f5de9e-c1c1-4e0d-9379-c1f24ef8e317"), Brand = "ELO", Prefix = "650491", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("939e53b9-86ba-469f-ad97-17c32835e9d9"), Brand = "ELO", Prefix = "65090", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("a737a5ec-d9ab-4c82-a211-aac229572c7e"), Brand = "ELO", Prefix = "650487", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ecbe1d3f-9f8c-48f5-a658-3d9d5d7f91d2"), Brand = "ELO", Prefix = "506726", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("7662decc-e708-4b09-9de7-df03d12de745"), Brand = "ELO", Prefix = "409835", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f8100aea-d8c9-4335-85fc-e9870fb5cd2e"), Brand = "ELO", Prefix = "509059", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("a7840efe-7668-40a9-bf65-3e4f43ebc80b"), Brand = "ELO", Prefix = "650921", Order = 10
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("cd449ce4-ec99-461e-878b-b7b284c6784a"), Brand = "ELO", Prefix = "650912", Order = 10
            },

            // HIPERCARD
            new CardBrandPattern
            {
                Id = Guid.Parse("8703460a-828d-4928-9a69-4cb97acad932"), Brand = "HIPERCARD", Prefix = "506282",
                Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("7989c0be-2c68-455d-96ca-a2b10f3bc6f9"), Brand = "HIPERCARD", Prefix = "38", Order = 60
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("03021c6a-ab74-4194-9008-ccc40b3454b2"), Brand = "HIPERCARD", Prefix = "606282",
                Order = 80
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ac7bf604-5bd2-4dd0-8099-c15a88a7cbd0"), Brand = "HIPERCARD", Prefix = "3841",
                Order = 80
            },

            // JCB
            new CardBrandPattern
                { Id = Guid.Parse("fa55f0bb-898d-4ed4-aed7-4c345098d12f"), Brand = "JCB", Prefix = "2131", Order = 90 },
            new CardBrandPattern
                { Id = Guid.Parse("068c0d0c-ff50-47bd-86ee-54ab4160b9fd"), Brand = "JCB", Prefix = "1800", Order = 90 },

            // MASTER
new CardBrandPattern
            {
                Id = Guid.Parse("c17945d1-dfac-4528-92d3-58ba7b8673a9"), Brand = "MASTER", Prefix = "546479", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ae29ebf9-69c0-4b66-abdf-2e48690e4a75"), Brand = "MASTER", Prefix = "509059", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("c31a0eb8-6ff4-4aef-a0c8-2e04fe66fa2c"), Brand = "MASTER", Prefix = "589916", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("4cb807b0-2380-4d25-8b56-db1eff7b6d86"), Brand = "MASTER", Prefix = "585988", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e9aec32b-6185-4b1d-92f7-872a6584e30b"), Brand = "MASTER", Prefix = "502121", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ca5a23a0-ee54-4efc-ac1b-c13ce30f3af5"), Brand = "MASTER", Prefix = "439133", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("7f8612d9-1ec0-452e-a3d7-02461952ec29"), Brand = "MASTER", Prefix = "55909", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f19990fa-38ac-4b17-80f8-6cd7ad9f714b"), Brand = "MASTER", Prefix = "53508", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("73aa02f6-7f3a-41fc-9aa1-f878a47ecb53"), Brand = "MASTER", Prefix = "320132", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("b58cae42-3b1a-4b33-adc0-0605ca3c53be"), Brand = "MASTER", Prefix = "126820", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f77d1687-99cd-448e-ae7b-38536c2b87a7"), Brand = "MASTER", Prefix = "576292", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("741f140c-5beb-428c-b502-643d754fe23a"), Brand = "MASTER", Prefix = "5453", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("4f55c6ab-649e-44b8-82a3-48063bb66211"), Brand = "MASTER", Prefix = "5379", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("0d91f868-a9af-4b67-a4dd-4309f217da9a"), Brand = "MASTER", Prefix = "537993", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f49ba514-def7-4a62-a362-1d4aaad4b23a"), Brand = "MASTER", Prefix = "639350", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("94c0a1c9-d3e7-47d9-91bf-365479d4f8f9"), Brand = "MASTER", Prefix = "830050", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f70886d7-dcba-451a-8bea-57a215a8865e"), Brand = "MASTER", Prefix = "517349", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("87ab02f5-b316-48ac-95c5-5b771ba96f88"), Brand = "MASTER", Prefix = "560033", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("21bc57ee-6b18-45a3-ba13-db6e985dbd9a"), Brand = "MASTER", Prefix = "505381", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("519f03fa-651b-4558-8fed-5030bea1d6ab"), Brand = "MASTER", Prefix = "595922", Order = 0
            },
new CardBrandPattern
            {
                Id = Guid.Parse("08b48b51-5431-4a5c-9b4b-c18590e2505e"), Brand = "MASTER", Prefix = "007807", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("bbc8335f-21ac-4f3b-9895-195dc8039172"), Brand = "MASTER", Prefix = "509257", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f0c45cd5-388c-44b5-8287-1de96c580545"), Brand = "MASTER", Prefix = "502309", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("5eee4e2e-fa5a-4eaf-a660-66ec80483dc7"), Brand = "MASTER", Prefix = "212743", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("8310cfff-2ce9-488f-9472-0935b2b2c23c"), Brand = "MASTER", Prefix = "505316", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("7a24a105-c838-42c3-a76c-74f52731b233"), Brand = "MASTER", Prefix = "720358", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("6b272f7e-2d41-48a0-8b76-c51ec97f1f89"), Brand = "MASTER", Prefix = "502299", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("a7bf25c7-75c7-4e4f-9cfe-f18838d5ff13"), Brand = "MASTER", Prefix = "330239", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("c87079de-67b1-4461-8119-87a2f858b61d"), Brand = "MASTER", Prefix = "640245", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("443bd561-1f52-4d1b-8dc2-789fda1f7659"), Brand = "MASTER", Prefix = "003006", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("5feefe95-f62c-41e4-bb78-82c75857ce2c"), Brand = "MASTER", Prefix = "615292", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("bf681118-f178-4c8e-86c4-3b1deeb45111"), Brand = "MASTER", Prefix = "574080", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("3f02e54d-fd91-4968-b7f0-56e71698a746"), Brand = "MASTER", Prefix = "608710", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e1c5586d-fced-4bd0-b9fd-3546b1e223a4"), Brand = "MASTER", Prefix = "507641", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("68d08bf5-9edc-4cfc-be6c-8b3116175f3d"), Brand = "MASTER", Prefix = "316811", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e358c814-280d-45eb-872c-920cd492b095"), Brand = "MASTER", Prefix = "569970", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e6932ba8-8947-4555-a1d3-e93b37bc49a3"), Brand = "MASTER", Prefix = "960397", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("cbd8e34e-7a1a-49eb-bb70-daea623fcc85"), Brand = "MASTER", Prefix = "220209", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ad230cbe-9f7e-46e6-9b30-bcd0ce41e10b"), Brand = "MASTER", Prefix = "637095", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("b8694624-7a37-4a1f-a021-39572403cdef"), Brand = "MASTER", Prefix = "576096", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("86507397-c2fe-4e92-a44e-5b2ee5e1a41b"), Brand = "MASTER", Prefix = "596497", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("5da1f97b-fb75-4c61-b535-a3cc73bc0633"), Brand = "MASTER", Prefix = "570209", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ab9ca0f5-30a2-4db5-8526-6bf2cd678cee"), Brand = "MASTER", Prefix = "044027", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("a684588c-fd6b-4ce1-82a3-f1f9276976da"), Brand = "MASTER", Prefix = "960379", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e83043b6-560f-4746-a376-2536571017a7"), Brand = "MASTER", Prefix = "988996", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("61c93e03-1fa7-44c1-a44b-159d6834e77d"), Brand = "MASTER", Prefix = "608783", Order = 1
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("d7198f2e-3210-480d-9bfb-3470bafc0f25"), Brand = "MASTER", Prefix = "750209", Order = 1
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("02ae6e45-adc6-484a-968c-efcc817cf14b"), Brand = "MASTER", Prefix = "515894", Order = 1
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("69fa4e15-c7e6-4ce9-9862-407e8ef876d1"), Brand = "MASTER", Prefix = "2221", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("6c060d1d-8c2f-45bd-83e5-b8765cea39ee"), Brand = "MASTER", Prefix = "2222", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("7d7bda98-6e41-4463-9725-59b587e741da"), Brand = "MASTER", Prefix = "2223", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("3984dea5-ba1e-461c-af58-dd5ff7069845"), Brand = "MASTER", Prefix = "2224", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("986c650d-77b2-4ebd-ba3a-05e19e28ba87"), Brand = "MASTER", Prefix = "2225", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e28503e3-ff7e-47b7-aba8-6d4654ccf5fb"), Brand = "MASTER", Prefix = "2226", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e23f8298-2a50-46f8-a78d-6a362aff5a49"), Brand = "MASTER", Prefix = "2227", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("48591305-20b7-4e23-b4f8-661c797d8a5a"), Brand = "MASTER", Prefix = "2228", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("e1c13d5e-0221-4b5d-9faa-378bed549cf3"), Brand = "MASTER", Prefix = "2229", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("88395a41-5e25-4663-bd77-a4cbaa2989b0"), Brand = "MASTER", Prefix = "223", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("fec41d70-aa8e-40f5-9b76-e2df4b02ccb9"), Brand = "MASTER", Prefix = "224", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("9920629d-d69b-47c4-8261-a13885ec06f6"), Brand = "MASTER", Prefix = "225", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("7d069fcf-1e15-4757-bb62-fc8ace46c0c4"), Brand = "MASTER", Prefix = "226", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("69cfdbd2-7a03-44f3-9b22-9ab9c302cc37"), Brand = "MASTER", Prefix = "227", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("84aaa2a3-d5bf-462a-8554-6e13588f291a"), Brand = "MASTER", Prefix = "228", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("ee4a29a6-ec2a-40ff-839a-924bf0aa6147"), Brand = "MASTER", Prefix = "229", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("c1196f95-2f9b-4e2d-8c68-6702e3393a89"), Brand = "MASTER", Prefix = "23", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("56ba5f1a-ced6-4179-8e1d-e0310dde74b3"), Brand = "MASTER", Prefix = "24", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("6136fec1-3021-40d2-8f70-37e7c9c5384b"), Brand = "MASTER", Prefix = "25", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("6b1b71d5-23e5-4e60-b740-25b3fc7569e4"), Brand = "MASTER", Prefix = "26", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("940e0f85-d19b-43cb-982c-1f1e15deb620"), Brand = "MASTER", Prefix = "271", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("1866854f-75e4-48f7-afa8-8f5ebbad4c15"), Brand = "MASTER", Prefix = "2720", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("51832641-df38-4420-85d8-4a4d8319342c"), Brand = "MASTER", Prefix = "51", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("07e5a965-a474-4686-974b-47c90b528c06"), Brand = "MASTER", Prefix = "52", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("45a82d8f-f94e-4022-b8ea-8e333d10fa28"), Brand = "MASTER", Prefix = "53", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("2a91a797-706f-4aa0-b956-5105e9ec7e94"), Brand = "MASTER", Prefix = "54", Order = 20
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("c84779fa-e8ee-4508-8e80-775b0727716d"), Brand = "MASTER", Prefix = "55", Order = 20
            },

            // NARANJA E NEVADA
            new CardBrandPattern
            {
                Id = Guid.Parse("b86a08ba-7ef8-42af-8039-47e862936d91"), Brand = "NARANJA E NEVADA", Prefix = "589562",
                Order = 100
            },

            // VISA
            new CardBrandPattern
            {
                Id = Guid.Parse("2f268b01-22cc-4333-8b14-ca3676d32a18"), Brand = "VISA", Prefix = "627892", Order = 0
            },

            new CardBrandPattern
            {
                Id = Guid.Parse("67b97e41-3a32-47c7-88f5-b08b28ca6442"), Brand = "VISA", Prefix = "121772", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("43ed3784-00b3-4632-9290-09e59ce9c7e3"), Brand = "VISA", Prefix = "40064", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("0f95b35d-b088-43f1-afdc-f1330ef094ff"), Brand = "VISA", Prefix = "444125", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("d4863a73-eafa-4e91-9e31-a890cc11e225"), Brand = "VISA", Prefix = "798431", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f6366254-277e-4e2a-8f2d-00ff4b964b83"), Brand = "VISA", Prefix = "48546", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("69a4bca6-db70-4086-86f4-1daeb0cc3bab"), Brand = "VISA", Prefix = "988442", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("6415ce04-2849-4df0-9734-27dcbc562c49"), Brand = "VISA", Prefix = "566269", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("28ab96b0-c938-4dfc-a2db-a1ec475bcf1a"), Brand = "VISA", Prefix = "970800", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("3303cee1-40fb-449f-8605-90e092b28d24"), Brand = "VISA", Prefix = "560780", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("cf4c6b6d-810f-42a2-83f9-501716ac859c"), Brand = "VISA", Prefix = "984428", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("9c85bcf1-862a-4f10-bd8a-67655d47b148"), Brand = "VISA", Prefix = "636355", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("52b1ba66-7e36-4820-9cd2-b009685ab521"), Brand = "VISA", Prefix = "027728", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("36894d44-4f2d-4140-8374-bd81bec5b943"), Brand = "VISA", Prefix = "107915", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("2454f6c5-e0f5-46e5-a704-79ce888dda83"), Brand = "VISA", Prefix = "210790", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("54b77a7f-4d86-44d3-b172-ce460b7ce84c"), Brand = "VISA", Prefix = "854644", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("84b1df64-5352-4fdf-9817-492ce3da2684"), Brand = "VISA", Prefix = "033994", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("f898b6b1-7cdd-4f8c-8cfc-35005326c523"), Brand = "VISA", Prefix = "985966", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("76354b51-5520-4415-bab9-03073028ddbe"), Brand = "VISA", Prefix = "960421", Order = 0
            },
            new CardBrandPattern
            {
                Id = Guid.Parse("11b3dedf-f205-4f08-a247-1888ed5ddf7b"), Brand = "VISA", Prefix = "186307", Order = 0
            },
            new CardBrandPattern
                { Id = Guid.Parse("d38e16f5-ca2e-42df-9c39-3ac77bf79f8d"), Brand = "VISA", Prefix = "4984", Order = 5 },
            new CardBrandPattern
                { Id = Guid.Parse("f8418814-2f4b-466b-9a11-876be7daf054"), Brand = "VISA", Prefix = "4", Order = 110 },
        ];
    }
}
