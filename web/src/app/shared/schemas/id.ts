import * as z from "zod";

export const GUIDSchema = z.guid().brand<"GUID">();
