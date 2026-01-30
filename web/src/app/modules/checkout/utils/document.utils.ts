const CPF_FORMAT = "$1.$2.$3-$4";
const CNPJ_FORMAT = "$1.$2.$3/$4-$5";

/**
 * Mask document strings (CPF/CNPJ)
 * e.g. 99999999999 - 999.999.999-99
 * e.g. 99999999999999 - 99.999.999/9999-99
 */
export const maskDocument = (document: string) => {
  if (document.length === 14) {
    return document.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, CNPJ_FORMAT);
  }

  if (document.length === 11) {
    return document.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, CPF_FORMAT);
  }

  throw new Error("Invalid document format");
};
