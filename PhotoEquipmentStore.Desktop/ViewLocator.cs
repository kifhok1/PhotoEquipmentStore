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


    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}