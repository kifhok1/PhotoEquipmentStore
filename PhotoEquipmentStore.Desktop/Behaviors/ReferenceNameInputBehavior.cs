using System;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.ViewModels.Pages.Admin;

namespace PhotoEquipmentStore.Behaviors;

public class ReferenceNameInputBehavior : Behavior<TextBox>
{
    private Regex?  _allowed;
    private string  _errorText = string.Empty;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.DataContextChanged += OnDataContextChanged;
        ApplyRules(AssociatedObject.DataContext);
        AssociatedObject.AddHandler(
            InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.DataContextChanged -= OnDataContextChanged;
        AssociatedObject.RemoveHandler(InputElement.TextInputEvent, OnTextInput);
        base.OnDetaching();
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
        => ApplyRules(AssociatedObject?.DataContext);

    private void ApplyRules(object? dataContext)
    {
        if (dataContext is not ReferenceAddViewModel vm) return;
        (_allowed, _errorText) = ReferenceAddViewModel.GetRuleForType(vm.ReferenceType);
    }

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (_allowed is null || string.IsNullOrEmpty(e.Text)) return;

        if (!_allowed.IsMatch(e.Text))
        {
            e.Handled = true;
            InputValidation.SetInputError(AssociatedObject!, _errorText);
        }
        else
        {
            InputValidation.SetInputError(AssociatedObject!, string.Empty);
        }
    }
}
