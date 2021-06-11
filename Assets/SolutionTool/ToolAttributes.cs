using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InspectorAttribute : Attribute
{
    public string Category;
    public string Name;
    public InspectorAttribute(string name)
    {
        Name = name;
    }
    public InspectorAttribute(string category,string name)
    {
        Category = category;
        Name = name;
    }
}