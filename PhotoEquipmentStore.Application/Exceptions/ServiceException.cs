using System;

namespace PhotoEquipmentStore.Application.Exceptions;

/// <summary>
/// Исключение прикладного слоя, оборачивающее внутреннюю ошибку сервиса.
/// </summary>
public class ServiceException : Exception
{
    /// <summary>
    /// Создаёт исключение сервиса с сообщением и внутренней причиной.
    /// </summary>
    /// <param name="message">Текст ошибки.</param>
    /// <param name="inner">Исходное исключение.</param>
    public ServiceException(string message, Exception inner)
        : base(message, inner) { }
}
