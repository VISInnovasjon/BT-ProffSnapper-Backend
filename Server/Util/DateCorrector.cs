namespace Util.DateCorrector;


public class DateCorrector
{
    /// <summary>
    /// For the weird edgecases in ProffApiReturn where date is represented as a string with no dividers</br>
    /// example: "02012987"
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static DateTime CorrectDate(string input)
    {
        try
        {
            input = input.Insert(4, ".").Insert(2, ".");
            return DateTime.ParseExact(input, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            throw new ArgumentOutOfRangeException($"Couldn't parse {input} into a date, {ex.Message}");
        }
    }
    public static DateTime ConvertDate(string input)
    {
        if (string.IsNullOrEmpty(input)) throw new ArgumentNullException($"{input} Is null value, can't parse it into a date.");
        try
        {
            return DateTime.ParseExact(input, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            throw new ArgumentOutOfRangeException($"Couldn't parse {input} into a date, {ex.Message}");
        }
    }
}