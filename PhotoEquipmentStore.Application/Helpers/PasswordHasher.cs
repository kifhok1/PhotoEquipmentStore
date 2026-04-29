using System.Security.Cryptography;
using System.Text;

namespace PhotoEquipmentStore.Application.Helpers;

public class PasswordHasher
{
    public static string Hash(string password) =>
        Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(password)));

    public static bool Verify(string raw, string stored) =>
        string.Equals(Hash(raw), stored, StringComparison.OrdinalIgnoreCase);

}