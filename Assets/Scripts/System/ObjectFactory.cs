using System;
using UnityEngine;
using Eos.Objects;


public enum ObjectType : uint
{
    Normal = 0,
    RunTime = 1,
    Editor = 2
}

public static class ObjectFactory
{
    private static Transform _unityInstaceRoot;
    public static Transform UnityInstanceRoot{ set => _unityInstaceRoot = value; }
    public static T CreateEosObject<T>(params object[]args) where T : EosObjectBase
    {
        var instance = Activator.CreateInstance(typeof(T),args) as T;
        instance.OnCreate();
        instance.Ref.ObjectManager.RegistObject(instance);
        instance.ObjectID |= ((uint)ObjectType.RunTime) << 24;
        return instance;
    }
    public static T CreateInstance<T>() where T : class
    {
        var instance = Activator.CreateInstance<T>();
        return instance;
    }
    public static T CreateInstance<T>(ObjectType type = ObjectType.RunTime) where T : EosObjectBase
    {
        var instance = Activator.CreateInstance<T>();
        instance.OnCreate();
        instance.Ref.ObjectManager.RegistObject(instance);
        instance.ObjectID |= ((uint)type) << 24;
        return instance;
    }
    public static EosObjectBase CreateInstance(Type type)
    {
        return Activator.CreateInstance(type) as EosObjectBase;
    }
    public static EosObjectBase CreateInstance(string typename)
    {
        return Activator.CreateInstance(Type.GetType(typename)) as EosObjectBase;
    }
    public static EosObjectBase CopyObject(EosObjectBase parent,EosObjectBase src)
    {
        var clone = src.Clone(parent);
        clone.Activate(src.Active);
        return clone;
    }
    public static ObjectType GetRegistType(EosObjectBase obj)
    {
        return (ObjectType)(obj.ObjectID >> 24);
    }
    public static Transform CreateUnityInstance(string name = null)
    {
        var obj = new GameObject(name);
        if (_unityInstaceRoot!=null)
            obj.transform.SetParent(_unityInstaceRoot, false);
        return obj.transform;
    }
    public static Transform CreateUnityInstance(string name,params Type[] components)
    {
        var obj = new GameObject(name,components);
        obj.transform.SetParent(_unityInstaceRoot, false);
        return obj.transform;
    }
}
