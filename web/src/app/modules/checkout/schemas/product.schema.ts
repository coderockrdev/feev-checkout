import * as z from "zod";

export const ProductSchema = z.object({
  id: z.guid(),
  name: z.string(),
  price: z.int().positive(),
});
