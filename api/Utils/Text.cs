namespace FeevCheckout.Utils;

public class TextUtils
{
    public static string MaskDocumentNumber(string number)
    {
        var digits = new string(number.Where(char.IsDigit).ToArray());

        return digits.Length switch
        {
            11 => $"***.{digits[3..6]}.{digits[6..9]}-**", // CPF: ***.456.789-**
            14 => $"**.{digits[2..5]}.{digits[5..8]}/****-**", // CNPJ: **.345.678/****-**
            _ => new string('*', number.Length)
        };
    }
}
