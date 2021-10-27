using Eos.Objects;
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
public class EosObjectAttribute: Attribute
{

}
public abstract class CreationAttribute : Attribute
{
    public virtual bool CanCreate(Eos.Objects.EosObjectBase obj) { return false; }
}
public class NoCreated : CreationAttribute
{
    public override bool CanCreate(EosObjectBase obj)
    {
        return false;
    }
}
public class NoChild : CreationAttribute
{
    public override bool CanCreate(EosObjectBase obj)
    {
        return false;
    }
}
public class ChildOf : CreationAttribute
{
    protected Type[] _types;
    public ChildOf()
    {
    }
    public ChildOf(Type type)
    {
        _types = new Type[] { type };
    }
    public ChildOf(Type type, Type type2)
    {
        _types = new Type[] { type, type2 };
    }
    public ChildOf(Type type,Type type2, Type type3)
    {
        _types = new Type[] { type, type2, type2 };
    }
    public override bool CanCreate(EosObjectBase obj)
    {
        foreach (var t in _types)
        {
            if (obj.GetType()==t)
                return true;
            if (obj.Parent != null && obj.Parent.GetType() == t)
                return true;
        }
        return false;
    }
}
public class DescendantOf : ChildOf
{
    public DescendantOf(Type type) : base(type)
    {
    }
    public DescendantOf(Type type, Type type2):base(type,type2)
    {
    }
    public DescendantOf(Type type, Type type2, Type type3):base(type,type2,type3)
    {
    }
    public override bool CanCreate(EosObjectBase obj)
    {
        foreach(var t in _types)
        {
            if (obj.IsDescendantOf(t))
                return true;
        }
        return false;
    }
}