using System;
using System.Linq;
using System.Collections.Generic;
using Eos.Objects;

public class AttributeCaches
{
    //�̰� ���� ������ �ƴϴ�.�ٸ� �̷��� ������ ������ ���� ������� �־� ����.
    private static Type[] _eosobjecttypes;
    public static void Initialize()
    {
        var type = typeof(EosObjectBase);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && type.IsAssignableFrom(x));
        _eosobjecttypes = types.ToArray();
    }

    public static Type[] GetTypeNames<T>()
    {
        var type = typeof(T);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && type.IsAssignableFrom(x));
        var names = new List<string>();
        foreach (var t in types)
            names.Add(t.Name);
        return types.ToArray();
    }
    public static Type[] GetAvailableTypeNames<T>(Type parenttype)
    {
        //var type = typeof(T);
        //var types = AppDomain.CurrentDomain.GetAssemblies()
        //    .SelectMany(x => x.GetTypes())
        //    .Where(x => x.IsClass && type.IsAssignableFrom(x));

        //var availabletypes = new List<Type>();
        //foreach (var it in types)
        //{
        //    var customtypes = it.GetCustomAttributes(false);
        //    var isavailable = true;
        //    foreach(var iit in customtypes)
        //    {
        //        if (iit is NoCreated nocreate && nocreate.CanCreate() == false)
        //        {
        //            isavailable = false;
        //            break;
        //        }
        //    }
        //    if (isavailable)
        //        availabletypes.Add(it);
        //}
        //return availabletypes.ToArray();
        return null;
    }
}
