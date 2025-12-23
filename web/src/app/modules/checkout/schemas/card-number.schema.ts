import * as z from "zod";
import { luhnCheck } from "@modules/checkout/utils/luhn-check.utils";

export const CardNumberSchema = z
  .string()
  .nonempty({ message: "O número do cartão obrigatório" })
  .transform((v) => v.replace(/\D/g, ""))
  .refine((v) => v.length >= 13, { message: "Número do cartão inválido" })
  .refine(luhnCheck, { message: "Número do cartão inválido" });
