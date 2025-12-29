import * as z from "zod";

export const ValidThruSchema = z
  .string()
  .nonempty({ message: "A data de validade é obrigatória" })
  .trim()
  .regex(/^\d{2}\/\d{2}$/, {
    message: "Formato inválido (MM/AA)",
  })
  .refine(
    (value) => {
      const [month, year] = value.split("/").map(Number);

      // month must be 01–12
      if (month < 1 || month > 12) return false;

      const now = new Date();
      const currentYear = now.getFullYear() % 100;
      const currentMonth = now.getMonth() + 1;

      // Check if it's expired
      if (year < currentYear || (year === currentYear && year < currentMonth)) return false;

      return true;
    },
    {
      message: "Cartão expirado",
    },
  );
