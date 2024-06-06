namespace FP.Core.Api.Services;

public class RandomStringBuilder
{
    public string Create()
    {
        var str = "";

        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";

        var charArray = (alphabet + alphabet.ToLower() + numbers).ToCharArray();

        var random = new Random();

        var lenght = random.Next(6, 11);

        for (var i = 0; i < lenght; i++)
            str += charArray[random.Next(charArray.Length)];

        return str;
    }
    public string Create(int lenght)
    {
        var str = "";

        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";

        var charArray = (alphabet + alphabet.ToLower() + numbers).ToCharArray();

        var random = new Random();

        for (var i = 0; i < lenght; i++)
            str += charArray[random.Next(charArray.Length)];

        return str;
    }
}