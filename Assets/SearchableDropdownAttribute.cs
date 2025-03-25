using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class SearchableDropdownAttribute : PropertyAttribute
{
    public string MemberName { get; private set; }

    public object[] Parameters { get; private set; }

    public SearchableDropdownAttribute(string memberName)
    {
        MemberName = memberName;
    }

    public SearchableDropdownAttribute(string memberName, params object[] parameters)
    {
        MemberName = memberName;
        Parameters = parameters;
    }
}
