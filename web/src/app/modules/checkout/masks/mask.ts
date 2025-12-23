import { MaskitoOptions } from "@maskito/core";
import { maskitoDateOptionsGenerator } from "@maskito/kit";

export const Mask = {
  card: {
    mask: [
      ...new Array(4).fill(/\d/),
      " ",
      ...new Array(4).fill(/\d/),
      " ",
      ...new Array(4).fill(/\d/),
      " ",
      ...new Array(4).fill(/\d/),
      " ",
      ...new Array(3).fill(/\d/),
    ],
  },
  validThru: maskitoDateOptionsGenerator({
    mode: "mm/yy",
    separator: "/",
  }),
  cvv: {
    mask: [...new Array(3).fill(/\d/)],
  },
} satisfies Record<string, MaskitoOptions>;
