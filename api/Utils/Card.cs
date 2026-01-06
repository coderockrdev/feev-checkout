namespace FeevCheckout.Utils;

public class CardUtils
{
    public static bool IsTesting(string number)
    {
        return number == "0000000000000001";
    }
}
