using System;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Infrastructure.Connection;

public class TestConnect
{
    private bool connected;
    private string? errorMassage;

    public bool Connected
    {
        get { return connected; }
    }

    public string? ErrorMassage
    {
        get { return errorMassage; }
    }

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
