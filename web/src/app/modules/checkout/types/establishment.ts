import * as z from "zod";

import { EstablishmentSchema } from "@modules/checkout/schemas/establishment.schema";

export type Establishment = z.infer<typeof EstablishmentSchema>;
