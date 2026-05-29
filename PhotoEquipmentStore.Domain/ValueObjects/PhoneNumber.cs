using System.Text.RegularExpressions;
using PhotoEquipmentStore.Domain.Exceptions;

namespace PhotoEquipmentStore.Domain.ValueObjects;

public sealed class PhoneNumber
{
    private static readonly Regex PhonePattern =
        new(@"^\+7\(\d{3}\) \d{3}-\d{2}-\d{2}$", RegexOptions.Compiled);

    public string Value { get; }

    public PhoneNumber(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainException("Номер телефона не может быть пустым.");

        if (!PhonePattern.IsMatch(raw))
            throw new DomainException("Номер телефона должен быть в формате +7(XXX) XXX-XX-XX.");

        Value = raw;
    }

    public override string ToString() => Value;
}