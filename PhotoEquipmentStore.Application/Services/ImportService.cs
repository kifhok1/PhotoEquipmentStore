using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Helpers;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

public class ImportService : IImportService
{
    private readonly ImportCommands _cmd = new();

    private static readonly Dictionary<string, string[]> ExpectedHeaders = new()
    {
        ["roles"]          = ["name"],
        ["manufacturers"]  = ["name"],
        ["suppliers"]      = ["name"],
        ["categories"]     = ["name"],
        ["order_statuses"] = ["name"],
        ["clients"]        = ["full_name", "phone"],
        ["users"]          = ["full_name", "login", "password_hash", "phone", "role_id"],
        ["products"]       = ["name", "category_id", "price", "discount_percent",
                               "description", "stock_quantity", "supplier_id", "manufacturer_id"],
        ["orders"]         = ["article", "status_id", "client_id",
                               "discount_percent", "employee_id", "created_at"],
        ["order_items"]    = ["order_article", "product_id", "quantity", "price", "discount_percent"],
    };

    private static readonly Regex RussianName =
        new(@"^[А-ЯЁа-яё]+(?: [А-ЯЁа-яё]+){1,2}$", RegexOptions.Compiled);

    private static readonly Regex PhoneFormat =
        new(@"^\+7\(\d{3}\) \d{3}-\d{2}-\d{2}$", RegexOptions.Compiled);

    private static readonly Regex LoginChars =
        new(@"^[a-zA-Z0-9!@#$%^&*()\-_=+\[\]{};':"",./<>?\\|`~]+$", RegexOptions.Compiled);

    private static readonly int[] AllowedOrderDiscounts = [5, 10, 15];

    public async Task<ImportResultDto> ImportAsync(string tableName, string csvFilePath)
    {
        if (!ExpectedHeaders.ContainsKey(tableName))
            return ImportResultDto.Failure($"Неизвестная таблица: {tableName}.");

        if (!File.Exists(csvFilePath))
            return ImportResultDto.Failure("Файл не найден.");

        List<string[]> rows;
        try
        {
            rows = ReadCsv(csvFilePath, out var header);

            var expected = ExpectedHeaders[tableName];
            if (!header.SequenceEqual(expected))
                return ImportResultDto.Failure(
                    $"Неверная структура файла.\n" +
                    $"Ожидается: {string.Join(", ", expected)}\n" +
                    $"Получено:  {string.Join(", ", header)}");
        }
        catch (Exception ex)
        {
            return ImportResultDto.Failure($"Ошибка чтения файла: {ex.Message}");
        }

        return tableName switch
        {
            "roles"          => await ImportSimpleAsync("roles",          rows),
            "manufacturers"  => await ImportSimpleAsync("manufacturers",  rows),
            "suppliers"      => await ImportSimpleAsync("suppliers",      rows),
            "categories"     => await ImportSimpleAsync("categories",     rows),
            "order_statuses" => await ImportSimpleAsync("order_statuses", rows),
            "clients"        => await ImportClientsAsync(rows),
            "users"          => await ImportUsersAsync(rows),
            "products"       => await ImportProductsAsync(rows),
            "orders"         => await ImportOrdersAsync(rows),
            "order_items"    => await ImportOrderItemsAsync(rows),
            _                => ImportResultDto.Failure("Импорт для этой таблицы не поддерживается.")
        };
    }

    private async Task<ImportResultDto> ImportSimpleAsync(string table, List<string[]> rows)
    {
        int imported = 0, skipped = 0;
        var reasons  = new List<string>();

        for (var i = 0; i < rows.Count; i++)
        {
            var row    = rows[i];
            var lineNo = i + 2;

            if (row.Length < 1 || string.IsNullOrWhiteSpace(row[0]))
            {
                reasons.Add($"Строка {lineNo}: пустое поле name.");
                skipped++; continue;
            }

            var name = row[0].Trim();

            if (name.Length > 100)
            {
                reasons.Add($"Строка {lineNo}: name превышает 100 символов.");
                skipped++; continue;
            }

            try
            {
                if (!await _cmd.IsUniqueAsync(table, "name", name))
                {
                    reasons.Add($"Строка {lineNo}: «{name}» уже существует.");
                    skipped++; continue;
                }

                await _cmd.InsertSimpleAsync(table, name);
                imported++;
            }
            catch (DatabaseException ex)
            {
                reasons.Add($"Строка {lineNo}: ошибка БД — {ex.Message}");
                skipped++;
            }
        }

        return ImportResultDto.Success(imported, skipped, reasons);
    }

    private async Task<ImportResultDto> ImportClientsAsync(List<string[]> rows)
    {
        int imported = 0, skipped = 0;
        var reasons  = new List<string>();

        for (var i = 0; i < rows.Count; i++)
        {
            var row    = rows[i];
            var lineNo = i + 2;

            if (row.Length < 2)
            {
                reasons.Add($"Строка {lineNo}: недостаточно полей.");
                skipped++; continue;
            }

            var fullName = row[0].Trim();
            var phone    = row[1].Trim();

            if (!RussianName.IsMatch(fullName))
            {
                reasons.Add($"Строка {lineNo}: full_name должно содержать 2–3 слова на русском.");
                skipped++; continue;
            }

            if (fullName.Length > 150)
            {
                reasons.Add($"Строка {lineNo}: full_name превышает 150 символов.");
                skipped++; continue;
            }

            if (!PhoneFormat.IsMatch(phone))
            {
                reasons.Add($"Строка {lineNo}: phone имеет неверный формат (+7(XXX) XXX-XX-XX).");
                skipped++; continue;
            }

            try
            {
                if (!await _cmd.IsPhoneUniqueAsync(phone))
                {
                    reasons.Add($"Строка {lineNo}: телефон {phone} уже используется.");
                    skipped++; continue;
                }

                await _cmd.InsertClientAsync(fullName, phone);
                imported++;
            }
            catch (DatabaseException ex)
            {
                reasons.Add($"Строка {lineNo}: ошибка БД — {ex.Message}");
                skipped++;
            }
        }

        return ImportResultDto.Success(imported, skipped, reasons);
    }

    private async Task<ImportResultDto> ImportUsersAsync(List<string[]> rows)
    {
        int imported = 0, skipped = 0;
        var reasons  = new List<string>();

        for (var i = 0; i < rows.Count; i++)
        {
            var row    = rows[i];
            var lineNo = i + 2;

            if (row.Length < 5)
            {
                reasons.Add($"Строка {lineNo}: недостаточно полей.");
                skipped++; continue;
            }

            var fullName     = row[0].Trim();
            var login        = row[1].Trim();
            var passwordRaw  = row[2].Trim();
            var phone        = row[3].Trim();
            var roleIdStr    = row[4].Trim();

            if (!RussianName.IsMatch(fullName))
            {
                reasons.Add($"Строка {lineNo}: full_name должно содержать 2–3 слова на русском.");
                skipped++; continue;
            }

            if (fullName.Length > 150)
            {
                reasons.Add($"Строка {lineNo}: full_name превышает 150 символов.");
                skipped++; continue;
            }

            if (!LoginChars.IsMatch(login) || login.Length > 50)
            {
                reasons.Add($"Строка {lineNo}: login содержит недопустимые символы или превышает 50 символов.");
                skipped++; continue;
            }

            if (!LoginChars.IsMatch(passwordRaw))
            {
                reasons.Add($"Строка {lineNo}: password содержит недопустимые символы.");
                skipped++; continue;
            }

            if (!PhoneFormat.IsMatch(phone))
            {
                reasons.Add($"Строка {lineNo}: phone имеет неверный формат (+7(XXX) XXX-XX-XX).");
                skipped++; continue;
            }

            if (!int.TryParse(roleIdStr, out var roleId))
            {
                reasons.Add($"Строка {lineNo}: role_id должен быть числом.");
                skipped++; continue;
            }

            try
            {
                if (!await _cmd.IsUniqueAsync("users", "login", login))
                {
                    reasons.Add($"Строка {lineNo}: login «{login}» уже занят.");
                    skipped++; continue;
                }

                if (!await _cmd.IsPhoneUniqueAsync(phone))
                {
                    reasons.Add($"Строка {lineNo}: телефон {phone} уже используется.");
                    skipped++; continue;
                }

                if (!await _cmd.ExistsAsync("roles", "id", roleId))
                {
                    reasons.Add($"Строка {lineNo}: role_id={roleId} не существует.");
                    skipped++; continue;
                }

                var hash = PasswordHasher.ComputeSHA256Hash(passwordRaw);
                await _cmd.InsertUserAsync(fullName, login, hash, phone, roleId);
                imported++;
            }
            catch (DatabaseException ex)
            {
                reasons.Add($"Строка {lineNo}: ошибка БД — {ex.Message}");
                skipped++;
            }
        }

        return ImportResultDto.Success(imported, skipped, reasons);
    }

    private async Task<ImportResultDto> ImportProductsAsync(List<string[]> rows)
    {
        int imported = 0, skipped = 0;
        var reasons  = new List<string>();

        for (var i = 0; i < rows.Count; i++)
        {
            var row    = rows[i];
            var lineNo = i + 2;

            if (row.Length < 8)
            {
                reasons.Add($"Строка {lineNo}: недостаточно полей.");
                skipped++; continue;
            }

            var name         = row[0].Trim();
            var categoryIdStr  = row[1].Trim();
            var priceStr       = row[2].Trim();
            var discountStr    = row[3].Trim();
            var description    = row[4].Trim();
            var stockStr       = row[5].Trim();
            var supplierIdStr  = row[6].Trim();
            var manufacturerIdStr = row[7].Trim();

            if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
            {
                reasons.Add($"Строка {lineNo}: name пустое или превышает 200 символов.");
                skipped++; continue;
            }

            if (!int.TryParse(categoryIdStr, out var categoryId))
            {
                reasons.Add($"Строка {lineNo}: category_id должен быть числом.");
                skipped++; continue;
            }

            if (!decimal.TryParse(priceStr, NumberStyles.Number,
                    CultureInfo.InvariantCulture, out var price) || price < 0)
            {
                reasons.Add($"Строка {lineNo}: price имеет неверный формат.");
                skipped++; continue;
            }

            if (!int.TryParse(discountStr, out var discount) || discount < 0 || discount > 99)
            {
                reasons.Add($"Строка {lineNo}: discount_percent должен быть от 0 до 99.");
                skipped++; continue;
            }

            if (!int.TryParse(stockStr, out var stock) || stock < 0 || stock > 99)
            {
                reasons.Add($"Строка {lineNo}: stock_quantity должен быть от 0 до 99.");
                skipped++; continue;
            }

            if (!int.TryParse(supplierIdStr, out var supplierId))
            {
                reasons.Add($"Строка {lineNo}: supplier_id должен быть числом.");
                skipped++; continue;
            }

            if (!int.TryParse(manufacturerIdStr, out var manufacturerId))
            {
                reasons.Add($"Строка {lineNo}: manufacturer_id должен быть числом.");
                skipped++; continue;
            }

            try
            {
                if (!await _cmd.IsUniqueAsync("products", "name", name))
                {
                    reasons.Add($"Строка {lineNo}: товар «{name}» уже существует.");
                    skipped++; continue;
                }

                if (!await _cmd.ExistsAsync("categories", "id", categoryId))
                {
                    reasons.Add($"Строка {lineNo}: category_id={categoryId} не существует.");
                    skipped++; continue;
                }

                if (!await _cmd.ExistsAsync("suppliers", "id", supplierId))
                {
                    reasons.Add($"Строка {lineNo}: supplier_id={supplierId} не существует.");
                    skipped++; continue;
                }

                if (!await _cmd.ExistsAsync("manufacturers", "id", manufacturerId))
                {
                    reasons.Add($"Строка {lineNo}: manufacturer_id={manufacturerId} не существует.");
                    skipped++; continue;
                }

                await _cmd.InsertProductAsync(name, categoryId, price, discount,
                    description, stock, supplierId, manufacturerId);
                imported++;
            }
            catch (DatabaseException ex)
            {
                reasons.Add($"Строка {lineNo}: ошибка БД — {ex.Message}");
                skipped++;
            }
        }

        return ImportResultDto.Success(imported, skipped, reasons);
    }

    private async Task<ImportResultDto> ImportOrdersAsync(List<string[]> rows)
    {
        int imported = 0, skipped = 0;
        var reasons  = new List<string>();

        for (var i = 0; i < rows.Count; i++)
        {
            var row    = rows[i];
            var lineNo = i + 2;

            if (row.Length < 6)
            {
                reasons.Add($"Строка {lineNo}: недостаточно полей.");
                skipped++; continue;
            }

            var article      = row[0].Trim();
            var statusIdStr  = row[1].Trim();
            var clientIdStr  = row[2].Trim();
            var discountStr  = row[3].Trim();
            var employeeIdStr = row[4].Trim();
            var createdAtStr = row[5].Trim();

            if (article.Length != 8 || !article.All(char.IsDigit))
            {
                reasons.Add($"Строка {lineNo}: article должен состоять ровно из 8 цифр.");
                skipped++; continue;
            }

            if (!int.TryParse(statusIdStr, out var statusId))
            {
                reasons.Add($"Строка {lineNo}: status_id должен быть числом.");
                skipped++; continue;
            }

            if (!int.TryParse(clientIdStr, out var clientId))
            {
                reasons.Add($"Строка {lineNo}: client_id должен быть числом.");
                skipped++; continue;
            }

            if (!int.TryParse(discountStr, out var discount)
                || !AllowedOrderDiscounts.Contains(discount))
            {
                reasons.Add($"Строка {lineNo}: discount_percent допустимые значения: 5, 10, 15.");
                skipped++; continue;
            }

            if (!int.TryParse(employeeIdStr, out var employeeId))
            {
                reasons.Add($"Строка {lineNo}: employee_id должен быть числом.");
                skipped++; continue;
            }

            if (!DateTime.TryParseExact(createdAtStr, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var createdAt)
                || createdAt.Date > DateTime.Today)
            {
                reasons.Add($"Строка {lineNo}: created_at неверный формат или дата в будущем (yyyy-MM-dd).");
                skipped++; continue;
            }

            try
            {
                if (!await _cmd.IsUniqueAsync("orders", "article", article))
                {
                    reasons.Add($"Строка {lineNo}: article {article} уже существует.");
                    skipped++; continue;
                }

                if (!await _cmd.ExistsAsync("order_statuses", "id", statusId))
                {
                    reasons.Add($"Строка {lineNo}: status_id={statusId} не существует.");
                    skipped++; continue;
                }

                if (!await _cmd.ExistsAsync("clients", "id", clientId))
                {
                    reasons.Add($"Строка {lineNo}: client_id={clientId} не существует.");
                    skipped++; continue;
                }

                if (!await _cmd.ExistsAsync("users", "id", employeeId))
                {
                    reasons.Add($"Строка {lineNo}: employee_id={employeeId} не существует.");
                    skipped++; continue;
                }

                await _cmd.InsertOrderAsync(article, statusId, clientId,
                    discount, employeeId, createdAt);
                imported++;
            }
            catch (DatabaseException ex)
            {
                reasons.Add($"Строка {lineNo}: ошибка БД — {ex.Message}");
                skipped++;
            }
        }

        return ImportResultDto.Success(imported, skipped, reasons);
    }

    private async Task<ImportResultDto> ImportOrderItemsAsync(List<string[]> rows)
    {
        int imported = 0, skipped = 0;
        var reasons  = new List<string>();

        for (var i = 0; i < rows.Count; i++)
        {
            var row    = rows[i];
            var lineNo = i + 2;

            if (row.Length < 5)
            {
                reasons.Add($"Строка {lineNo}: недостаточно полей.");
                skipped++; continue;
            }

            var orderArticle  = row[0].Trim();
            var productIdStr  = row[1].Trim();
            var quantityStr   = row[2].Trim();
            var priceStr      = row[3].Trim();
            var discountStr   = row[4].Trim();

            if (!int.TryParse(productIdStr, out var productId))
            {
                reasons.Add($"Строка {lineNo}: product_id должен быть числом.");
                skipped++; continue;
            }

            if (!int.TryParse(quantityStr, out var quantity) || quantity < 0 || quantity > 99)
            {
                reasons.Add($"Строка {lineNo}: quantity должен быть от 0 до 99.");
                skipped++; continue;
            }

            if (!decimal.TryParse(priceStr, NumberStyles.Number,
                    CultureInfo.InvariantCulture, out var price) || price < 0)
            {
                reasons.Add($"Строка {lineNo}: price имеет неверный формат.");
                skipped++; continue;
            }

            if (!int.TryParse(discountStr, out var discount) || discount < 0 || discount > 99)
            {
                reasons.Add($"Строка {lineNo}: discount_percent должен быть от 0 до 99.");
                skipped++; continue;
            }

            try
            {
                if (!await _cmd.ExistsAsync("orders", "article", orderArticle))
                {
                    reasons.Add($"Строка {lineNo}: order_article={orderArticle} не существует в orders.");
                    skipped++; continue;
                }

                if (!await _cmd.ExistsAsync("products", "id", productId))
                {
                    reasons.Add($"Строка {lineNo}: product_id={productId} не существует.");
                    skipped++; continue;
                }

                await _cmd.InsertOrderItemAsync(orderArticle, productId,
                    quantity, price, discount);
                imported++;
            }
            catch (DatabaseException ex)
            {
                reasons.Add($"Строка {lineNo}: ошибка БД — {ex.Message}");
                skipped++;
            }
        }

        return ImportResultDto.Success(imported, skipped, reasons);
    }

    private static List<string[]> ReadCsv(string path, out string[] header)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord  = true,
            TrimOptions      = TrimOptions.Trim,
            MissingFieldFound = null,
        };

        using var reader = new StreamReader(path, Encoding.UTF8);
        using var csv    = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();
        header = csv.HeaderRecord ?? [];

        var rows = new List<string[]>();
        while (csv.Read())
        {
            var row = new string[header.Length];
            for (var j = 0; j < header.Length; j++)
                row[j] = csv.GetField(j) ?? string.Empty;
            rows.Add(row);
        }

        return rows;
    }
}
