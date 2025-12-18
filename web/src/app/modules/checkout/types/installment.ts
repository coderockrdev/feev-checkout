import * as z from "zod";

import { InstallmentSchema } from "@modules/checkout/schemas/installment.schema";

export type Installment = z.infer<typeof InstallmentSchema>;
