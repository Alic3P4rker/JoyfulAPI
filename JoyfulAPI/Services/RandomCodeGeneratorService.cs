namespace Joyful.API.Services;

public static class RandomCodeGeneratorService
{
    private static readonly Random rnd = new Random();
    public static string GenerateRandomAlphanumericCode(int size)
    {
        char[] code = new char[size];
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890";

        for (int i = 0; i < size; i++)
        {
            int x = rnd.Next(chars.Length);
            code[i] = chars[x];
        }

        return new string (code);
    }
}