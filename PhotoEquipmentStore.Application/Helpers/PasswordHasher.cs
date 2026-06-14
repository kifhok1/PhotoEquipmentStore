using System;
using System.Security.Cryptography;
using System.Text;

namespace PhotoEquipmentStore.Application.Helpers;

/// <summary>
/// Утилита для хеширования и проверки паролей по алгоритму SHA-256.
/// </summary>
public class PasswordHasher
{

    /// <summary>
    /// Вычисляет SHA-256 хеш строки в шестнадцатеричном представлении.
    /// </summary>
    /// <param name="input">Исходная строка (пароль).</param>
    /// <returns>Хеш в нижнем регистре или пустая строка, если вход пуст.</returns>
    public static string ComputeSHA256Hash(string input)
    {

        if (string.IsNullOrEmpty(input))
            return string.Empty;

        using (SHA256 sha256 = SHA256.Create())
        {

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {

                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Сравнивает пароль в открытом виде с сохранённым хешем.
    /// </summary>
    /// <param name="raw">Пароль в открытом виде.</param>
    /// <param name="stored">Сохранённый хеш.</param>
    /// <returns>True, если пароль совпадает с хешем.</returns>
    public static bool Verify(string raw, string stored) =>
        string.Equals(ComputeSHA256Hash(raw), stored, StringComparison.OrdinalIgnoreCase);

}
