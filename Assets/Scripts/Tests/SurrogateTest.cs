using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTCommon;

using Battlehub.RTCommon;
using Battlehub.RTCommon.EditorTreeView;
using Battlehub.RTSL.Interface;
using UnityEngine.Battlehub.SL2;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;
using Battlehub.RTSL.Battlehub.SL2;
using Battlehub.Utils;
using Battlehub.RTSL;


public class SurrogateTest : ProjectGuids
{
    [SerializeField]
    private GameObject _prefab;
    // Start is called before the first frame update
    void Start()
    {
        ITypeMap typeMap = m_typeMap = new TypeMap<long>();
        IOC.Register(typeMap);

        object obj = _prefab;
        Type objType = obj.GetType();
        Type persistentType = m_typeMap.ToPersistentType(objType);
        List<UnityObject> notMapped = new List<UnityObject>();
        if (obj is GameObject)
        {
            GetUnmappedObjects((GameObject)obj, notMapped);
        }
        PersistentObject<Guid> persistentObject = (PersistentObject<Guid>)Activator.CreateInstance(persistentType);
        persistentObject.ReadFrom(obj);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
