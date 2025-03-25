# Unity Searchable Dropdown Attribute
This is a Unity Editor property attribute for a searchable dropdown list

```csharp
// Example
using UnityEngine;

public class ExampleComponent : MonoBehaviour
{
    [SearchableDropdown(typeof(ExampleOptionsProvider))]
    public string selectedOption;
}

public static class ExampleOptionsProvider
{
    public static string[] GetOptions()
    {
        return new[] { "Apple", "Banana", "Cherry", "Date", "Elderberry", "Fig", "Grape", "Honeydew", "Kiwi", "Lemon", "Mango" };
    }
}
