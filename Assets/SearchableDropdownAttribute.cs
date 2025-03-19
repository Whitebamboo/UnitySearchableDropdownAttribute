using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class SearchableDropdownAttribute : PropertyAttribute
{
    public Type OptionsProviderType { get; }

    public SearchableDropdownAttribute(Type optionsProviderType)
    {
        OptionsProviderType = optionsProviderType;
    }
}