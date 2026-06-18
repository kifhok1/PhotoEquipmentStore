using System;

namespace PhotoEquipmentStore.Infrastructure.Exceptions;

/// <summary>
/// Исключение, возникающее при ошибках работы с базой данных.
/// </summary>
public class DatabaseException : Exception
{
    /// <summary>
    /// Создаёт исключение с сообщением об ошибке.
    /// </summary>
    public DatabaseException(string message)
        : base(message) { }

    /// <summary>
    /// Создаёт исключение с сообщением и внутренним исключением-источником.
    /// </summary>
    public DatabaseException(string message, Exception inner)
        : base(message, inner) { }
}
