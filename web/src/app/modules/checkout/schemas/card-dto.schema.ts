import * as z from "zod";

export const CardDtoSchema = z
  .object({
    cardHolder: z.string().min(6).max(255),
    cardNumber: z.string().min(10).max(19),
    validThru: z.string().regex(/^\d{2}\/\d{2}$/),
    cvv: z.string().regex(/^\d{3}$/),
  })
  .transform((payload) => {
    const [month, shortYear] = payload.validThru.split("/");
    const thousandYear = Math.floor(new Date().getFullYear() / 100) * 100; // e.g. 2000 | 2100

    return {
      holder: payload.cardHolder,
      number: payload.cardNumber.replace(/\D/g, ""),
      dueAt: `${month}/${thousandYear + parseInt(shortYear)}`,
      securityCode: payload.cvv,
    };
  });
