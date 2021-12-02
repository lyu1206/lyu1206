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


public class SurrogateTest : Project
{
    [SerializeField]
    private GameObject _prefab;
    private IAssetDB<long> m_assetDB;

    // Start is called before the first frame update
    protected override void Awake()
    {
        ITypeMap typeMap = new TypeMap<long>();
        IOC.Register(typeMap);
        var assetDb = new AssetDB();
        IOC.Register<IAssetDB>(assetDb);
        IOC.Register<IAssetDB<long>>(assetDb);
        base.Awake();
    }
    private void _getUnmappedObjects(GameObject go, List<UnityObject> notMapped)
    {
        if (go.GetComponent<RTSLIgnore>())
        {
            return;
        }

        if (!m_assetDB.IsMapped(go))
        {
            notMapped.Add(go);
        }

        Transform[] transforms = go.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < transforms.Length; ++i)
        {
            Transform tf = transforms[i];
            if (tf.gameObject != go && !m_assetDB.IsMapped(tf.gameObject))
            {
                notMapped.Add(tf.gameObject);
            }

            Component[] components = tf.GetComponents<Component>();
            for (int j = 0; j < components.Length; ++j)
            {
                Component comp = components[j];
                if (!m_assetDB.IsMapped(comp))
                {
                    notMapped.Add(comp);
                }
            }
        }
    }

    void Start()
    {
        m_assetDB = IOC.Resolve<IAssetDB<long>>();
        object obj = _prefab;
        Type objType = obj.GetType();
        Type persistentType = m_typeMap.ToPersistentType(objType);
        List<UnityObject> notMapped = new List<UnityObject>();
        if (persistentType.GetGenericTypeDefinition() == typeof(PersistentGameObject<>))
        {
            persistentType = typeof(PersistentRuntimePrefab<long>);
        }

        if (obj is GameObject)
        {
            GetUnmappedObjects((GameObject)obj, notMapped);
            //            _getUnmappedObjects((GameObject)obj, notMapped);
        }
        var persistentObject = (PersistentObject<long>)Activator.CreateInstance(persistentType);
        persistentObject.ReadFrom(obj);

        IResourcePreviewUtility resourcePreview = this.gameObject.AddComponent<ResourcePreviewUtility>(); IOC.Resolve<IResourcePreviewUtility>();
        //IResourcePreviewUtility resourcePreview = IOC.Resolve<IResourcePreviewUtility>();
        var previewdata = resourcePreview.CreatePreviewData((GameObject)obj);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
