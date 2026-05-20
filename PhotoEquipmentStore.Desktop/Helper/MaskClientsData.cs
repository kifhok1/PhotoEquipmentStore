using System;
using System.Linq;

namespace PhotoEquipmentStore.Helper;

public class MaskClientsData
{
    public static string MaskPhoneNumber(string phone)
    {
        var digitIndexes = phone.Select((ch, idx) => new { ch, idx })
            .Where(x => char.IsDigit(x.ch))
            .Select(x => x.idx)
            .ToArray();
        
        int totalDigits = digitIndexes.Length;
        if (totalDigits <= 5)
            return phone;
        
        int firstDigitPos = digitIndexes[0];               
        int maskStartPos = 1;                              
        int maskEndPos = totalDigits - 5;                  
        
        char[] result = phone.ToCharArray();
        
        for (int i = maskStartPos; i <= maskEndPos; i++)
        {
            int pos = digitIndexes[i];
            result[pos] = 'X';
        }
        
        return new string(result);
    }
    
    public static string MaskFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return fullName;

        string[] parts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length == 0)
            return fullName;
        
        // Фамилия
        string surname = parts[0];
        
        // Имя (берём первые 4 символа, но не больше длины)
        string firstName = parts.Length > 1 ? parts[1] : "";
        
        // Отчество (первая буква и точка)
        string patronymic = parts.Length > 2 ? parts[2] : "";
        string maskedPatronymic = patronymic.Length > 0 
            ? patronymic[0] + "." 
            : "";
        
        // Собираем результат
        if (parts.Length == 2)
            return $"{surname} {firstName}";
        else if (parts.Length >= 3)
            return $"{surname} {firstName} {maskedPatronymic}";
        else
            return surname;
    }
}