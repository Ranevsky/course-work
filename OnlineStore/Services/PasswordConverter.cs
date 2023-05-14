using System.Security.Cryptography;
using System.Text;

namespace OnlineStore.Services;

public static class PasswordConverter
{
    public static string Hash(string password)
    {
        var hasher = MD5.Create();
        var passByte = Encoding.UTF8.GetBytes(password);
        var hash = hasher.ComputeHash(passByte);
        var hashString = Encoding.UTF8.GetString(hash);

        return hashString;
    }

    public static bool Compare(string password, string hash)
    {
        var hashPass = Hash(password);
        var passByte = Encoding.UTF8.GetBytes(hashPass);

        var hashByte = Encoding.UTF8.GetBytes(hash);

        var equal = true;
        for (var i = 0; i < hashByte.Length; i++)
        {
            if (hashByte[i] != passByte[i])
            {
                equal = false;
            }
        }

        return equal;
    }
}