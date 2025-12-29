import * as z from "zod";

import { ProductSchema } from "@modules/checkout/schemas/product.schema";

export type Product = z.infer<typeof ProductSchema>;
