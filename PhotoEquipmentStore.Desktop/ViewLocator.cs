using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using PhotoEquipmentStore.ViewModels;

namespace PhotoEquipmentStore;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return new TextBlock { Text = "null" };

        var originalName = data.GetType().FullName!;
        var name = originalName.Replace("ViewModel", "View", StringComparison.Ordinal);
    
        Console.WriteLine($"ViewLocator: Looking for View '{name}' for ViewModel '{originalName}'");
    
        var type = Type.GetType(name);

        if (type != null)
        {
            Console.WriteLine($"ViewLocator: Found View {name}, creating instance...");
            try
            {
                var control = (Control)Activator.CreateInstance(type)!;
                control.DataContext = data;
                Console.WriteLine($"ViewLocator: Successfully created View {name}");
                return control;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ViewLocator: Error creating View {name}: {ex.Message}");
                Console.WriteLine($"ViewLocator: StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"ViewLocator: InnerException: {ex.InnerException.Message}");
                    Console.WriteLine($"ViewLocator: InnerException StackTrace: {ex.InnerException.StackTrace}");
                }
                return new TextBlock { Text = $"Error: {ex.Message}" };
            }
        }

        Console.WriteLine($"ViewLocator: View not found: {name}");
        return new TextBlock { Text = "Not Found: " + name };
    }


    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}