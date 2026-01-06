import * as z from "zod";

import { AddressSchema } from "@modules/checkout/schemas/address.schema";

export type Address = z.infer<typeof AddressSchema>;
