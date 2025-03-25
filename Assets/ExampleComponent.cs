using UnityEngine;

public class ExampleComponent : MonoBehaviour
{
    [SearchableDropdown(nameof(GetOptions))]
    public string selectedOption;

    public string[] GetOptions()
    {
        return new[] { "Apple", "Banana", "Cherry", "Date", "Elderberry", "Fig", "Grape", "Honeydew", "Kiwi", "Lemon", "Mango" };
    }
}