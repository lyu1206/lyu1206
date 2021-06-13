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
public class RequireMoldAttribute : Attribute
{
    public string MoldName;
    public RequireMoldAttribute(string moldname)
    {
        MoldName = moldname;
    }
}
public abstract class CreationAttribute : Attribute
{
    public abstract bool CanCreate();
}
public class NoCreated : CreationAttribute
{
    public override bool CanCreate()
    {
        return false;
    }
}