import * as z from "zod";

import { CustomerSchema } from "@modules/checkout/schemas/customer.schema";

export type Customer = z.infer<typeof CustomerSchema>;
