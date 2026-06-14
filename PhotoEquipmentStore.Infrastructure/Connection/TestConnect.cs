using System;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Infrastructure.Connection;

/// <summary>
/// Проверка возможности подключения к базе данных MySQL по заданным настройкам.
/// </summary>
public class TestConnect
{
    private bool connected;
    private string? errorMassage;

    /// <summary>
    /// Результат последней попытки подключения: true — успешно, false — ошибка.
    /// </summary>
    public bool Connected
    {
        get { return connected; }
    }

    /// <summary>
    /// Текст ошибки при неудачном подключении; null при успехе.
    /// </summary>
    public string? ErrorMassage
    {
        get { return errorMassage; }
    }

    /// <summary>
    /// Выполняет пробное открытие и закрытие соединения с базой данных.
    /// </summary>
    public void Connect(ConnectionToDBSettings connectionSettings)
    {
        try
        {
            MySqlConnection conn = new MySqlConnection(connectionSettings.ConnectionString);
            conn.Open();
            conn.Close();
            connected = true;
        }
        catch (Exception ex)
        {
            connected = false;
            errorMassage = ex.Message;
        }
    }
}
