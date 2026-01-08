// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.TouchTestingFramework.Appium;

/// <summary>
/// Provides utilities for filling and validating forms.
/// Handles form fields, validation, and batch operations.
/// </summary>
/// <example>
/// <code>
/// var formHelper = new FormHelper(driver);
/// 
/// // Fill form with data
/// await formHelper.FillAsync(new Dictionary&lt;string, object&gt;
/// {
///     { "username", "testuser" },
///     { "email", "test@example.com" },
///     { "age", 25 },
///     { "subscribe", true }
/// });
/// 
/// // Validate required fields
/// var validation = await formHelper.ValidateRequiredFieldsAsync();
/// </code>
/// </example>
public class FormHelper
{
    private readonly AvaloniaDriver _driver;

    /// <summary>
    /// Creates a new FormHelper.
    /// </summary>
    /// <param name="driver">The Avalonia driver.</param>
    public FormHelper(AvaloniaDriver driver)
    {
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    #region Form Filling

    /// <summary>
    /// Fills form fields with provided values.
    /// </summary>
    /// <param name="fieldValues">Dictionary of field names to values.</param>
    /// <param name="container">Optional container to search in.</param>
    public async Task FillAsync(Dictionary<string, object> fieldValues, AvaloniaElement? container = null)
    {
        if (fieldValues == null)
            throw new ArgumentNullException(nameof(fieldValues));

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                return;

            foreach (var kvp in fieldValues)
            {
                var control = FindFieldByName(root, kvp.Key);
                if (control != null)
                {
                    SetFieldValue(control, kvp.Value);
                }
            }
        });
    }

    /// <summary>
    /// Fills a single field by name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="container">Optional container to search in.</param>
    public async Task FillFieldAsync(string fieldName, object value, AvaloniaElement? container = null)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                throw new NoSuchElementException($"Field '{fieldName}' not found.");

            var control = FindFieldByName(root, fieldName);
            if (control == null)
                throw new NoSuchElementException($"Field '{fieldName}' not found.");

            SetFieldValue(control, value);
        });
    }

    /// <summary>
    /// Gets all form field values.
    /// </summary>
    /// <param name="container">Optional container to search in.</param>
    /// <returns>Dictionary of field names to values.</returns>
    public async Task<Dictionary<string, object?>> GetAllValuesAsync(AvaloniaElement? container = null)
    {
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                return new Dictionary<string, object?>();

            var values = new Dictionary<string, object?>();
            CollectFieldValues(root, values);
            return values;
        });
    }

    /// <summary>
    /// Gets a single field value by name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="container">Optional container to search in.</param>
    /// <returns>The field value.</returns>
    public async Task<object?> GetFieldValueAsync(string fieldName, AvaloniaElement? container = null)
    {
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                return null;

            var control = FindFieldByName(root, fieldName);
            return control != null ? GetFieldValue(control) : null;
        });
    }

    /// <summary>
    /// Clears all form fields.
    /// </summary>
    /// <param name="container">Optional container to search in.</param>
    public async Task ClearAllAsync(AvaloniaElement? container = null)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                return;

            ClearFieldsRecursive(root);
        });
    }

    /// <summary>
    /// Clears a single field by name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="container">Optional container to search in.</param>
    public async Task ClearFieldAsync(string fieldName, AvaloniaElement? container = null)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                return;

            var control = FindFieldByName(root, fieldName);
            if (control != null)
            {
                ClearField(control);
            }
        });
    }

    #endregion

    #region Form Validation

    /// <summary>
    /// Validates required fields are not empty.
    /// </summary>
    /// <param name="requiredFields">List of required field names.</param>
    /// <param name="container">Optional container to search in.</param>
    /// <returns>Validation result.</returns>
    public async Task<FormValidationResult> ValidateRequiredFieldsAsync(IEnumerable<string> requiredFields, AvaloniaElement? container = null)
    {
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            var result = new FormValidationResult { IsValid = true };

            if (root == null)
            {
                result.IsValid = false;
                result.Errors.Add("Form container not found.");
                return result;
            }

            foreach (var fieldName in requiredFields)
            {
                var control = FindFieldByName(root, fieldName);
                if (control == null)
                {
                    result.IsValid = false;
                    result.MissingFields.Add(fieldName);
                    result.Errors.Add($"Required field '{fieldName}' not found.");
                }
                else if (IsFieldEmpty(control))
                {
                    result.IsValid = false;
                    result.EmptyFields.Add(fieldName);
                    result.Errors.Add($"Required field '{fieldName}' is empty.");
                }
            }

            return result;
        });
    }

    /// <summary>
    /// Validates field value against a pattern.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="pattern">Regex pattern to match.</param>
    /// <param name="container">Optional container to search in.</param>
    /// <returns>True if valid.</returns>
    public async Task<bool> ValidateFieldPatternAsync(string fieldName, string pattern, AvaloniaElement? container = null)
    {
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                return false;

            var control = FindFieldByName(root, fieldName);
            if (control == null)
                return false;

            var value = GetFieldValue(control)?.ToString() ?? "";
            return System.Text.RegularExpressions.Regex.IsMatch(value, pattern);
        });
    }

    /// <summary>
    /// Gets all validation errors from data error info.
    /// </summary>
    /// <param name="container">Optional container to search in.</param>
    /// <returns>Dictionary of field names to error messages.</returns>
    public async Task<Dictionary<string, string>> GetValidationErrorsAsync(AvaloniaElement? container = null)
    {
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            var errors = new Dictionary<string, string>();

            if (root == null)
                return errors;

            CollectValidationErrors(root, errors);
            return errors;
        });
    }

    #endregion

    #region Form Information

    /// <summary>
    /// Gets information about all form fields.
    /// </summary>
    /// <param name="container">Optional container to search in.</param>
    /// <returns>List of field information.</returns>
    public async Task<List<FormFieldInfo>> GetFieldInfoAsync(AvaloniaElement? container = null)
    {
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            var fields = new List<FormFieldInfo>();

            if (root == null)
                return fields;

            CollectFieldInfo(root, fields);
            return fields;
        });
    }

    /// <summary>
    /// Checks if the form has any changes from initial values.
    /// </summary>
    /// <param name="initialValues">Initial field values.</param>
    /// <param name="container">Optional container to search in.</param>
    /// <returns>True if form has changes.</returns>
    public async Task<bool> HasChangesAsync(Dictionary<string, object?> initialValues, AvaloniaElement? container = null)
    {
        var currentValues = await GetAllValuesAsync(container);

        foreach (var kvp in initialValues)
        {
            if (currentValues.TryGetValue(kvp.Key, out var currentValue))
            {
                if (!Equals(kvp.Value, currentValue))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the changed fields compared to initial values.
    /// </summary>
    /// <param name="initialValues">Initial field values.</param>
    /// <param name="container">Optional container to search in.</param>
    /// <returns>Dictionary of changed field names to new values.</returns>
    public async Task<Dictionary<string, object?>> GetChangedFieldsAsync(Dictionary<string, object?> initialValues, AvaloniaElement? container = null)
    {
        var currentValues = await GetAllValuesAsync(container);
        var changes = new Dictionary<string, object?>();

        foreach (var kvp in currentValues)
        {
            if (initialValues.TryGetValue(kvp.Key, out var initialValue))
            {
                if (!Equals(initialValue, kvp.Value))
                    changes[kvp.Key] = kvp.Value;
            }
            else
            {
                // New field
                changes[kvp.Key] = kvp.Value;
            }
        }

        return changes;
    }

    #endregion

    #region Form Actions

    /// <summary>
    /// Submits the form by clicking a submit button.
    /// </summary>
    /// <param name="buttonText">Submit button text (defaults to common labels).</param>
    /// <param name="container">Optional container to search in.</param>
    public async Task SubmitAsync(string? buttonText = null, AvaloniaElement? container = null)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                throw new NoSuchElementException("Submit button not found.");

            var submitButton = FindSubmitButton(root, buttonText);
            if (submitButton == null)
                throw new NoSuchElementException("Submit button not found.");

            var bounds = submitButton.Bounds;
            var center = new Point(bounds.Width / 2, bounds.Height / 2);
            _driver.MouseSimulator.Click(submitButton, center);
        });
    }

    /// <summary>
    /// Resets the form by clicking a reset button.
    /// </summary>
    /// <param name="buttonText">Reset button text (defaults to common labels).</param>
    /// <param name="container">Optional container to search in.</param>
    public async Task ResetAsync(string? buttonText = null, AvaloniaElement? container = null)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                return;

            var resetButton = FindResetButton(root, buttonText);
            if (resetButton != null)
            {
                var bounds = resetButton.Bounds;
                var center = new Point(bounds.Width / 2, bounds.Height / 2);
                _driver.MouseSimulator.Click(resetButton, center);
            }
        });
    }

    /// <summary>
    /// Focuses the first field in the form.
    /// </summary>
    /// <param name="container">Optional container to search in.</param>
    public async Task FocusFirstFieldAsync(AvaloniaElement? container = null)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                return;

            var firstField = FindFirstFocusableField(root);
            if (firstField is Control control)
            {
                control.Focus();
            }
        });
    }

    /// <summary>
    /// Tabs through all form fields.
    /// </summary>
    /// <param name="container">Optional container to search in.</param>
    /// <returns>List of fields in tab order.</returns>
    public async Task<List<string>> GetTabOrderAsync(AvaloniaElement? container = null)
    {
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var root = container?.Control ?? _driver.Root;
            if (root == null)
                return new List<string>();

            var tabOrder = new List<string>();
            CollectTabOrder(root, tabOrder);
            return tabOrder;
        });
    }

    #endregion

    #region Private Methods

    private Control? FindFieldByName(Control root, string fieldName)
    {
        return FindControlRecursive(root, c =>
        {
            if (c.Name == fieldName)
                return true;

            // Check automation properties
            var automationId = c.GetValue(Avalonia.Automation.AutomationProperties.AutomationIdProperty);
            if (automationId == fieldName)
                return true;

            return false;
        });
    }

    private Control? FindControlRecursive(Control root, Func<Control, bool> predicate)
    {
        if (predicate(root))
            return root;

        foreach (var child in root.GetVisualChildren())
        {
            if (child is Control control)
            {
                var found = FindControlRecursive(control, predicate);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    private void SetFieldValue(Control control, object value)
    {
        switch (control)
        {
            case TextBox textBox:
                textBox.Text = value?.ToString() ?? "";
                break;
            case CheckBox checkBox:
                checkBox.IsChecked = Convert.ToBoolean(value);
                break;
            case RadioButton radioButton:
                radioButton.IsChecked = Convert.ToBoolean(value);
                break;
            case ComboBox comboBox:
                if (value is int index)
                    comboBox.SelectedIndex = index;
                else
                    SetComboBoxByValue(comboBox, value?.ToString());
                break;
            case NumericUpDown numericUpDown:
                numericUpDown.Value = Convert.ToDecimal(value);
                break;
            case Slider slider:
                slider.Value = Convert.ToDouble(value);
                break;
            case DatePicker datePicker:
                if (value is DateTime dt)
                    datePicker.SelectedDate = dt;
                else if (value is DateTimeOffset dto)
                    datePicker.SelectedDate = dto;
                break;
            case ToggleSwitch toggleSwitch:
                toggleSwitch.IsChecked = Convert.ToBoolean(value);
                break;
        }
    }

    private object? GetFieldValue(Control control)
    {
        return control switch
        {
            TextBox textBox => textBox.Text,
            CheckBox checkBox => checkBox.IsChecked,
            RadioButton radioButton => radioButton.IsChecked,
            ComboBox comboBox => comboBox.SelectedItem,
            NumericUpDown numericUpDown => numericUpDown.Value,
            Slider slider => slider.Value,
            DatePicker datePicker => datePicker.SelectedDate,
            ToggleSwitch toggleSwitch => toggleSwitch.IsChecked,
            _ => null
        };
    }

    private bool IsFieldEmpty(Control control)
    {
        var value = GetFieldValue(control);
        if (value == null)
            return true;
        if (value is string str)
            return string.IsNullOrWhiteSpace(str);
        return false;
    }

    private void ClearField(Control control)
    {
        switch (control)
        {
            case TextBox textBox:
                textBox.Clear();
                break;
            case CheckBox checkBox:
                checkBox.IsChecked = false;
                break;
            case RadioButton radioButton:
                radioButton.IsChecked = false;
                break;
            case ComboBox comboBox:
                comboBox.SelectedIndex = -1;
                break;
            case NumericUpDown numericUpDown:
                numericUpDown.Value = numericUpDown.Minimum;
                break;
            case Slider slider:
                slider.Value = slider.Minimum;
                break;
            case DatePicker datePicker:
                datePicker.SelectedDate = null;
                break;
        }
    }

    private void ClearFieldsRecursive(Control root)
    {
        ClearField(root);

        foreach (var child in root.GetVisualChildren())
        {
            if (child is Control control)
            {
                ClearFieldsRecursive(control);
            }
        }
    }

    private void CollectFieldValues(Control root, Dictionary<string, object?> values)
    {
        if (!string.IsNullOrEmpty(root.Name) && IsFormField(root))
        {
            values[root.Name] = GetFieldValue(root);
        }

        foreach (var child in root.GetVisualChildren())
        {
            if (child is Control control)
            {
                CollectFieldValues(control, values);
            }
        }
    }

    private void CollectFieldInfo(Control root, List<FormFieldInfo> fields)
    {
        if (IsFormField(root))
        {
            fields.Add(new FormFieldInfo
            {
                Name = root.Name,
                FieldType = root.GetType().Name,
                Value = GetFieldValue(root),
                IsEnabled = root.IsEnabled,
                IsVisible = root.IsVisible,
                IsEmpty = IsFieldEmpty(root)
            });
        }

        foreach (var child in root.GetVisualChildren())
        {
            if (child is Control control)
            {
                CollectFieldInfo(control, fields);
            }
        }
    }

    private void CollectValidationErrors(Control root, Dictionary<string, string> errors)
    {
        // Check for DataValidationErrors
        if (root is TextBox textBox && !string.IsNullOrEmpty(root.Name))
        {
            var dataValidationErrors = DataValidationErrors.GetErrors(textBox);
            if (dataValidationErrors != null)
            {
                var errorMessages = dataValidationErrors
                    .OfType<object>()
                    .Select(e => e.ToString())
                    .Where(e => !string.IsNullOrEmpty(e));
                    
                var combined = string.Join("; ", errorMessages);
                if (!string.IsNullOrEmpty(combined))
                {
                    errors[root.Name] = combined;
                }
            }
        }

        foreach (var child in root.GetVisualChildren())
        {
            if (child is Control control)
            {
                CollectValidationErrors(control, errors);
            }
        }
    }

    private void SetComboBoxByValue(ComboBox comboBox, string? value)
    {
        for (int i = 0; i < comboBox.ItemCount; i++)
        {
            var item = comboBox.Items[i];
            if (item?.ToString() == value)
            {
                comboBox.SelectedIndex = i;
                return;
            }
        }
    }

    private bool IsFormField(Control control)
    {
        return control is TextBox ||
               control is CheckBox ||
               control is RadioButton ||
               control is ComboBox ||
               control is NumericUpDown ||
               control is Slider ||
               control is DatePicker ||
               control is ToggleSwitch;
    }

    private Button? FindSubmitButton(Control root, string? buttonText)
    {
        var texts = buttonText != null 
            ? new[] { buttonText }
            : new[] { "Submit", "OK", "Save", "Confirm", "Send", "Apply" };

        return FindButtonByText(root, texts);
    }

    private Button? FindResetButton(Control root, string? buttonText)
    {
        var texts = buttonText != null 
            ? new[] { buttonText }
            : new[] { "Reset", "Clear", "Cancel" };

        return FindButtonByText(root, texts);
    }

    private Button? FindButtonByText(Control root, string[] texts)
    {
        return FindControlRecursive(root, c =>
        {
            if (c is Button button)
            {
                var content = button.Content?.ToString();
                return content != null && texts.Any(t => 
                    content.Equals(t, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }) as Button;
    }

    private Control? FindFirstFocusableField(Control root)
    {
        if (IsFormField(root) && root.IsEnabled && root.IsVisible && root.Focusable)
            return root;

        foreach (var child in root.GetVisualChildren().OrderBy(c => 
            (c as Control)?.GetValue(Avalonia.Input.KeyboardNavigation.TabIndexProperty) ?? 0))
        {
            if (child is Control control)
            {
                var found = FindFirstFocusableField(control);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    private void CollectTabOrder(Control root, List<string> tabOrder)
    {
        if (IsFormField(root) && root.Focusable && !string.IsNullOrEmpty(root.Name))
        {
            tabOrder.Add(root.Name);
        }

        var orderedChildren = root.GetVisualChildren()
            .OfType<Control>()
            .OrderBy(c => c.GetValue(Avalonia.Input.KeyboardNavigation.TabIndexProperty));

        foreach (var child in orderedChildren)
        {
            CollectTabOrder(child, tabOrder);
        }
    }

    #endregion
}

/// <summary>
/// Result of form validation.
/// </summary>
public class FormValidationResult
{
    /// <summary>Whether the form is valid.</summary>
    public bool IsValid { get; set; }

    /// <summary>List of error messages.</summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>Fields that were not found.</summary>
    public List<string> MissingFields { get; set; } = new();

    /// <summary>Required fields that are empty.</summary>
    public List<string> EmptyFields { get; set; } = new();
}

/// <summary>
/// Information about a form field.
/// </summary>
public class FormFieldInfo
{
    /// <summary>Field name.</summary>
    public string? Name { get; set; }

    /// <summary>Field type (TextBox, CheckBox, etc.).</summary>
    public string? FieldType { get; set; }

    /// <summary>Current value.</summary>
    public object? Value { get; set; }

    /// <summary>Whether the field is enabled.</summary>
    public bool IsEnabled { get; set; }

    /// <summary>Whether the field is visible.</summary>
    public bool IsVisible { get; set; }

    /// <summary>Whether the field is empty.</summary>
    public bool IsEmpty { get; set; }
}
