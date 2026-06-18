using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using PhotoEquipmentStore.ViewModels;

namespace PhotoEquipmentStore;

[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
/// <summary>
/// Локатор представлений: сопоставляет ViewModel с соответствующим View по соглашению об именовании.
/// </summary>
public class ViewLocator : IDataTemplate
{
    /// <summary>
    /// Создаёт экземпляр View для переданной ViewModel через рефлексию.
    /// </summary>
    /// <param name="data">Объект ViewModel.</param>
    /// <returns>Элемент управления или текстовое сообщение об ошибке.</returns>
    public Control? Build(object? data)
    {
        if (data is null)
            return new TextBlock { Text = "null" };

        var originalName = data.GetType().FullName!;
        var name = originalName.Replace("ViewModel", "View", StringComparison.Ordinal);

        var type = Type.GetType(name);

        if (type != null)
        {
            try
            {
                var control = (Control)Activator.CreateInstance(type)!;
                control.DataContext = data;
                return control;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                }
                return new TextBlock { Text = $"Error: {ex.Message}" };
            }
        }
        return new TextBlock { Text = "Not Found: " + name };
    }

    /// <summary>
    /// Определяет, подходит ли данный шаблон для объекта (наследник <see cref="ViewModelBase"/>).
    /// </summary>
    /// <param name="data">Проверяемый объект контекста данных.</param>
    /// <returns><c>true</c>, если объект является ViewModel.</returns>
    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
