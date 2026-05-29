using System.Text.RegularExpressions;
using PhotoEquipmentStore.Domain.Exceptions;

namespace PhotoEquipmentStore.Domain.ValueObjects;

public sealed class FullName
{
    private static readonly Regex WordPattern =
        new(@"^[А-ЯЁ][а-яё]+$", RegexOptions.Compiled);

    public string  LastName   { get; }
    public string  FirstName  { get; }
    public string? Patronymic { get; }

    public FullName(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainException("ФИО не может быть пустым.");

        var parts = raw.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2 || parts.Length > 3)
            throw new DomainException("ФИО должно содержать фамилию и имя, отчество — необязательно.");

        if (!WordPattern.IsMatch(parts[0]))
            throw new DomainException($"Фамилия «{parts[0]}» должна начинаться с заглавной буквы и содержать только кириллицу.");

        if (!WordPattern.IsMatch(parts[1]))
            throw new DomainException($"Имя «{parts[1]}» должно начинаться с заглавной буквы и содержать только кириллицу.");

        if (parts.Length == 3 && !WordPattern.IsMatch(parts[2]))
            throw new DomainException($"Отчество «{parts[2]}» должно начинаться с заглавной буквы и содержать только кириллицу.");

        LastName   = parts[0];
        FirstName  = parts[1];
        Patronymic = parts.Length == 3 ? parts[2] : null;
    }

    public override string ToString() =>
        Patronymic is null
            ? $"{LastName} {FirstName}"
            : $"{LastName} {FirstName} {Patronymic}";
}