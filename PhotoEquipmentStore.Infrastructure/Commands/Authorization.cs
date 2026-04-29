using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Infrastructure.Сonnection;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Helpers;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class Authorization : IUserRepository
{
    private static readonly string connString = ConnectionSettingsParser.Load().ToString();

    private static MySqlConnection GetConnection()
    {
        return new MySqlConnection(connString);
    }

    private static UserAuth? GetUser(string login)
    {
        using var conn = GetConnection();
        conn.Open();

        const string query = @"SELECT w.ID_Worker, 
                                      w.Fio, 
                                      w.PhoneNumber, 
                                      w.Login, 
                                      w.Password, 
                                      w.Image, 
                                      r.ID_Rule, 
                                      r.Rule 
                               FROM workers AS w
                               INNER JOIN rule AS r ON r.ID_Rule = w.rule
                               WHERE w.Login = @login;";

        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@login", login);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        return new UserAuth(
            id:            reader.GetInt32("ID_Worker"),
            name:          reader.GetString("Fio"),
            login:         reader.GetString("Login"),
            heshPassword:  reader.GetString("Password"),
            userImage:     BlobReader.ToBytes(reader, "Image"),
            roleId:        reader.GetInt32("ID_Rule"),
            roleName:      reader.GetString("Rule"),
            timeOfLogout:  3
        );
    }

    public UserAuth? GetByLogin(string login) => Authorization.GetUser(login);
}