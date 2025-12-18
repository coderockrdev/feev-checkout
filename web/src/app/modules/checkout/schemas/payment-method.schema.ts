import * as z from "zod";

export const PaymentMethodSchema = z.enum(["feev_pix", "feev_boleto", "braspag_cartao"]);
