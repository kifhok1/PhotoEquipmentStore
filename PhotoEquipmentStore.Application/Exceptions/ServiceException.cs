using System;

namespace PhotoEquipmentStore.Application.Exceptions;

public class ServiceException : Exception
{
    public ServiceException(string message, Exception inner)
        : base(message, inner) { }
}