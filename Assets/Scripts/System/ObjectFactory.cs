using Eos.Objects;
using System;
using System.Reflection;



public enum ObjectType : uint
{
    Normal = 0,
    RunTime = 1,
}

public static class ObjectFactory
{

    public static T CreateInstance<T>(ObjectType type = ObjectType.RunTime) where T : EosObjectBase
    {
        var instance = Activator.CreateInstance<T>();
        instance.Ref.ObjectManager.RegistObject(instance);
        instance.ObjectID |= ((uint)type) << 24;
        return instance;
    }
    public static EosObjectBase CreateInstance(Type type)
    {
        return Activator.CreateInstance(type) as EosObjectBase;
    }
    public static EosObjectBase CopyObject(EosObjectBase src)
    {
        var clone = src.Clone();
        clone.Activate(src.Active);
        return clone;
    }
    public static ObjectType GetRegistType(EosObjectBase obj)
    {
        return (ObjectType)(obj.ObjectID >> 24);
    }

}
