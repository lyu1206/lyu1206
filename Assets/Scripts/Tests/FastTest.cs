using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTSL;
using Battlehub.RTCommon;
using Battlehub.RTSL.Battlehub.SL2;
using Battlehub.RTSL.Interface;
using UnityEngine.Battlehub.SL2;
using UnityObject = UnityEngine.Object;
using Battlehub.Utils;
using ProtoBuf.Meta;
using System.Linq;

namespace Eos.Test
{
    using EosPlayer;
    using Objects;
    using Service;
    public class FastTest : Project
    {
        [SerializeField]
        private GameObject _bone;
        [SerializeField]
        private GameObject[] _gears;

        private static FastTest _instance;
        public static FastTest Instance => _instance;

        private ITypeMap m_typeMap;
        private IAssetDB<long> m_assetDB;
        private Dictionary<long, AssetItem> _resourcesmeta;
        private Dictionary<long, AssetItem> _rawresourcesmeta;
        private Dictionary<long, AssetItem> _rawresources;

        public override void Awake_Internal()
        {
            _instance = this;
            Init();
            base.Awake_Internal();
        }
        private void Init()
        {
            IOC.Register<ITypeMap>(m_typeMap = new TypeMap<long>());
            IOC.Register<ISerializer>(new ProtobufSerializer());
            var assetDb = new AssetDB();
            IOC.Register<IAssetDB>(assetDb);
            IOC.Register<IAssetDB<long>>(assetDb);
            IUnityObjectFactory objFactory = new UnityObjectFactory();
            IOC.Register(objFactory);
            IMaterialUtil materialUtil = new StandardMaterialUtils();
            IOC.Register(materialUtil);
            IRuntimeShaderUtil shaderUtil = new RuntimeShaderUtil();
            IOC.Register(shaderUtil);

            m_assetDB = IOC.Resolve<IAssetDB<long>>();
        }
        private Tuple<AssetItem,PersistentObject<long>, Dictionary<long,PersistentObject<long>>> PTest(GameObject obj)
        {
            var notMapped = new List<UnityObject>();
            var assetItem = new AssetItem();
            assetItem.ItemID = m_assetDB.ToID(obj);
            assetItem.Name = ((UnityObject)obj).name;
            assetItem.Ext = GetExt(obj);
            assetItem.TypeGuid = m_typeMap.ToGuid(obj.GetType());


            IResourcePreviewUtility resourcePreview = this.gameObject.AddComponent<ResourcePreviewUtility>(); IOC.Resolve<IResourcePreviewUtility>();
            //IResourcePreviewUtility resourcePreview = IOC.Resolve<IResourcePreviewUtility>();
            var previewdata = resourcePreview.CreatePreviewData((GameObject)obj);
            assetItem.Preview = new Preview { PreviewData = previewdata };
            SetPersistentID(assetItem.Preview, ToPersistentID(assetItem));

            var objType = obj.GetType();
            var persistentType = m_typeMap.ToPersistentType(objType);

            GetUnmappedObjects(obj, notMapped);

            foreach (var it in notMapped)
            {
                m_assetDB.RegisterDynamicResource((long)ToPersistentID(it), it);
            }


            if (persistentType.GetGenericTypeDefinition() == typeof(PersistentGameObject<>))
            {
                persistentType = typeof(PersistentRuntimePrefab<long>);
            }
            var persistentObject = (PersistentObject<long>)Activator.CreateInstance(persistentType);

            persistentObject.ReadFrom(obj);
            var context = new GetDepsFromContext();
            persistentObject.GetDepsFrom(obj, context);

//            SaveDependencies(context, m_typeMap, (IAssetDB)m_assetDB, Application.streamingAssetsPath + "/ggg");


            var dependancies = new Dictionary<long, PersistentObject<long>>();
            var dpids = new List<long>();
            foreach (var dep in context.Dependencies)
            {
                var persisttype = m_typeMap.ToPersistentType(dep.GetType());
                if (persisttype == null)
                {
                    Debug.LogError($"_____no persistent type:{dep.GetType()}");
                    continue;
                }
                var componentData = (PersistentObject<long>)Activator.CreateInstance(persisttype);
                componentData.ReadFrom(dep);
                var depcontext = new GetDepsFromContext();
                componentData.GetDepsFrom(dep,depcontext);
                var pid = (long)ToPersistentID(dep as UnityObject);
                dpids.Add(pid);
                dependancies.Add(pid, componentData);
                foreach (var idp in depcontext.Dependencies)
                {
                    var ipid = (long)ToPersistentID(idp as UnityObject);
                    persisttype = m_typeMap.ToPersistentType(idp.GetType());
                    if (persisttype == null)
                        continue;
                    componentData = (PersistentObject<long>)Activator.CreateInstance(persisttype);
                    componentData.ReadFrom(idp);
                    dpids.Add(ipid);
                    dependancies.Add(ipid, componentData);
                }
            }
            assetItem.Dependencies = dpids.ToArray();
            return new Tuple<AssetItem, PersistentObject<long>, Dictionary<long, PersistentObject<long>>>(assetItem,persistentObject,dependancies);
        }
        private const string PreviewExt = ".rtview";
        private const string MetaExt = ".rtmeta";
        void WriteAssetItem(Tuple<AssetItem, PersistentObject<long>, Dictionary<long, PersistentObject<long>>> asset)
        {
            var serializer = IOC.Resolve<ISerializer>();
            var assetitem = asset.Item1;
            var pobj = asset.Item2;
            var librarypath = Application.streamingAssetsPath;
            var previewpath = librarypath + "/" + assetitem.NameExt + PreviewExt;
            using (FileStream fs = File.Create(previewpath))
            {
                serializer.Serialize(assetitem.Preview, fs);
            }
            using (FileStream fs = File.Create(librarypath + "/" + assetitem.NameExt))
            {
                using (var br = new BinaryWriter(fs))
                {
                    var data = serializer.Serialize(pobj);
                    br.Write(data.Length);
                    br.Write(data);
                    assetitem.CustomDataOffset = br.BaseStream.Position;
                }
            }
            var factory = IOC.Resolve<IUnityObjectFactory>();
            var dpassetpath = Application.streamingAssetsPath + "/RawResource/";
            foreach (var dpitem in asset.Item3)
            {
                if (!m_assetDB.IsMapped(dpitem.Key))
                {
                    var dpassetitem = new AssetItem();
                    dpassetitem.ItemID = dpitem.Key;
                    dpassetitem.Name = dpitem.Value.name;
                    var unitydpobjtype = m_typeMap.ToUnityType(dpitem.Value.GetType());
                    if (unitydpobjtype != null)
                    {
                        if (factory.CanCreateInstance(unitydpobjtype, dpitem.Value))
                        {
                            UnityObject assetInstance = factory.CreateInstance(unitydpobjtype, dpitem.Value);
                            dpassetitem.Ext = GetExt(unitydpobjtype);
                            if (assetInstance != null)
                            {
//                                m_assetDB.RegisterDynamicResource(dpitem.Key, assetInstance);
                            }
                        }
                    }
                    using (FileStream fs = File.Create(dpassetpath + dpassetitem.NameExt+MetaExt))
                    {
                        serializer.Serialize(dpassetitem, fs);
                    }
                    using(FileStream fs = File.Create(dpassetpath + dpassetitem.NameExt))
                    {
                        serializer.Serialize(dpitem.Value, fs);
                    }
                }
            }
            File.Delete(librarypath + "/" + assetitem.NameExt + MetaExt);
            using (FileStream fs = File.Create(librarypath + "/" + assetitem.NameExt + MetaExt))
            {
                serializer.Serialize(assetitem, fs);
            }
        }
        private Dictionary<long, PersistentObject<long>> _resourcepersistents = new Dictionary<long, PersistentObject<long>>();
        private void LoadAssetWithMetaData(long metakey)
        {
            var librarypath = Application.streamingAssetsPath;
            var serializer = IOC.Resolve<ISerializer>();
            var typeMap = IOC.Resolve<ITypeMap>();
            var factory = IOC.Resolve<IUnityObjectFactory>();
            if (_rawresourcesmeta.ContainsKey(metakey) && !m_assetDB.IsMapped(metakey))
            {
                var meta = _rawresourcesmeta[metakey];
                var assetobjectpath = librarypath + "/RawResource/" + meta.NameExt;
                var resource = Load<PersistentObject<long>>(serializer, assetobjectpath);
                var unitydpobjtype = typeMap.ToUnityType(resource.GetType());
                if (unitydpobjtype != null)
                {
                    if (factory.CanCreateInstance(unitydpobjtype, resource))
                    {
                        UnityObject assetInstance = factory.CreateInstance(unitydpobjtype, resource);
                        if (assetInstance != null)
                        {
                            m_assetDB.RegisterSceneObject(metakey, assetInstance);
                            _resourcepersistents.Add(metakey, resource);
                            //assetInstances[i] = assetInstance;
                            //idToUnityObj.Add(AssetIds[i], assetInstance);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Unable to create object of type " + unitydpobjtype.ToString());
                    }
                }

                var depscontext = new GetDepsContext<long>();
                resource.GetDeps(depscontext);
                // 여기서 m_asetDB에 올라간 UnityObject에다가 Write할수 있도록 리스트등에 너어서 실제로 만들어 내야된다 ㅇㅋ?
                if (depscontext.Dependencies.Count == 0)
                    return;
                foreach (var it in depscontext.Dependencies)
                    LoadAssetWithMetaData(it);
            }
        }
        private UnityObject ReadAssetItem(string assetobjectname)
        {
            var typeMap = IOC.Resolve<ITypeMap>();
            var factory = IOC.Resolve<IUnityObjectFactory>();
            var librarypath = Application.streamingAssetsPath;
            var assetobjectpath = librarypath + "/" + assetobjectname;
            var serializer = IOC.Resolve<ISerializer>();
            PersistentRuntimePrefab<long> obj = null;
            var idtoobj = new Dictionary<long, UnityObject>();
            List<GameObject> createdGameObjects = new List<GameObject>();
            using (FileStream fs = File.OpenRead(assetobjectpath))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    var datasize = br.ReadInt32();
                    var data = br.ReadBytes(datasize);
                    obj = serializer.Deserialize<PersistentRuntimePrefab<long>>(data);
                    obj.CreateGameObjectWithComponents(m_typeMap, obj.Descriptors[0], idtoobj, null, createdGameObjects);
                    m_assetDB.RegisterDynamicResources(idtoobj);
                    foreach (var it in obj.Dependencies)
                    {
                        if (idtoobj.ContainsKey(it))
                            m_assetDB.RegisterSceneObject(it, idtoobj[it]);
                        else
                        {
                            LoadAssetWithMetaData(it);
                        }
                    }
                }
            }
            obj.WriteTo(createdGameObjects[0]);
            return createdGameObjects[0];
        }
        void ReadyFiles()
        {
            var asset = PTest(_bone);
            WriteAssetItem(asset);

            foreach (var g in _gears)
            {
                asset = PTest(g);
                WriteAssetItem(asset);
            }
        }
        void ReadFiles()
        {
            var librarypath = Application.streamingAssetsPath;
            var assetpath = librarypath + "/" + "Bag01.rtprefab";
            var previewpath = librarypath + "/" + "Bag01.rtprefab"+PreviewExt;
            var serializer = IOC.Resolve<ISerializer>();
            using (FileStream fs = File.OpenRead(assetpath))
            {
                var idToObj = new Dictionary<long, UnityObject>();
                PersistentRuntimePrefab<long> asset = null;
                List<GameObject> createdGameObjects = new List<GameObject>();
                asset = serializer.Deserialize<PersistentRuntimePrefab<long>>(fs);
                asset.CreateGameObjectWithComponents(m_typeMap, asset.Descriptors[0], idToObj, null/*m_dynamicPrefabsRoot*/, createdGameObjects);
            }

        }
        public UnityObject GetResource(long uid)
        {
            if (!_resourcesmeta.ContainsKey(uid))
                return null;
            var rr = _resourcesmeta[uid];
            var uo = ReadAssetItem(rr.NameExt);

            foreach (var it in _resourcepersistents)
            {
                Debug.Log($"{it.Value.name} loaded.");
                var unityobject = m_assetDB.FromID<UnityObject>(it.Key);
                it.Value.WriteTo(unityobject);
            }
            return uo;
        }
        public UnityObject GetAsset(long uid)
        {
            if (!_rawresourcesmeta.ContainsKey(uid))
                return null;
            var rr = _rawresourcesmeta[uid];
            var uo = ReadAssetItem(rr.NameExt);

            foreach (var it in _resourcepersistents)
            {
                Debug.Log($"{it.Value.name} loaded.");
                var unityobject = m_assetDB.FromID<UnityObject>(it.Key);
                it.Value.WriteTo(unityobject);
            }

            return uo;
        }
        // Start is called before the first frame update
        void Start()
        {
            _rawresourcesmeta = GetRawResourceMeta("/RawResource/");
            _resourcesmeta = GetRawResourceMeta("");

            //            Init();

            //var asset = PTest(_bone);
            //WriteAssetItem(asset);


            ReadyFiles();
            //ReadFiles();


            var tt = ReadAssetItem("Bag01.rtprefab");
            tt = ReadAssetItem("glasses03.rtprefab");
            tt = ReadAssetItem("Hair03.rtprefab");

            foreach(var it in _resourcepersistents)
            {
                Debug.Log($"{it.Value.name} loaded.");
                var unityobject = m_assetDB.FromID<UnityObject>(it.Key);
                it.Value.WriteTo(unityobject);
            }

            var solution = ObjectFactory.CreateEosObject<Solution>(); solution.Name = "Solution";
            var workspace = ObjectFactory.CreateEosObject<Workspace>(); workspace.Name = "Workspace";
            solution.AddChild(workspace);

            var camera = ObjectFactory.CreateEosObject<EosCamera>();camera.Name = "Cam";
            camera.LocalPosition = new Vector3(0, 6, -8);
            camera.LocalRotation = new Vector3(25, 0, 0);
            workspace.AddChild(camera);

            var floor = ObjectFactory.CreateEosObject<EosShape>(PrimitiveType.Cube);floor.Name = "Floor";
            floor.LocalScale = new Vector3(50, 0.5f, 50);
            workspace.AddChild(floor);
            //var avatar = ObjectFactory.CreateEosObject<EosShape>(PrimitiveType.Capsule); floor.Name = "Avatar";
            //avatar.LocalPosition = new Vector3(0, 1, 0);
            //floor.AddChild(avatar);
            var objroot = ObjectFactory.CreateEosObject<EosTransformActor>();objroot.Name = "Root";
            objroot.LocalScale = Vector3.one * 0.1f;
            workspace.AddChild(objroot);

            var avatar = ObjectFactory.CreateEosObject<EosPawnActor>();avatar.Name = "Avatar";
            var bone = ObjectFactory.CreateEosObject<EosBone>();bone.Bone = _bone;bone.Name = "bone";bone.BoneGUID = 8589957722;
            avatar.AddChild(bone);
               
            objroot.AddChild(avatar);
            avatar.LocalPosition = new Vector3(0, 3, 0);
            avatar.LocalScale = Vector3.one;
            foreach (var gearsrc in _gears)
            {
                var gear = ObjectFactory.CreateEosObject<EosGear>();
                gear.Part = gearsrc;
                avatar.AddChild(gear);
            }
            camera.LocalPosition = new Vector3(0, 10, 10);
            camera.LookAt(avatar);


            EosPlayer.Instance.SetSolution(solution as Solution);
            EosPlayer.Instance.Play();


            var typemap = IOC.Resolve<ITypeMap>();
            var ptype = typemap.ToPersistentType(solution.GetType());



            Type objType = solution.GetType();
            Type persistentType = m_typeMap.ToPersistentType( typeof(RuntimeSolution));
            var serializer = IOC.Resolve<ISerializer>();
            var persistentObject = (PersistentRuntimeSolution<long>)Activator.CreateInstance(persistentType);
//            persistentObject.ReadFrom(solution);

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnDestroy()
        {
            m_assetDB.UnregisterDynamicResources();
            m_assetDB.UnregisterSceneObjects();
        }
        private Dictionary<long, AssetItem> GetRawResourceMeta(string folder)
        {
            var result = new Dictionary<long, AssetItem>();
            var serializer = IOC.Resolve<ISerializer>();
            var rawresourcepath = Application.streamingAssetsPath + folder;
            var storage = IOC.Resolve<IStorage<long>>();
            string[] files = Directory.GetFiles(rawresourcepath, "*" + MetaExt);
            for (int i = 0; i < files.Length; ++i)
            {
                string file = files[i];
                if (!File.Exists(file.Replace(MetaExt, string.Empty)))
                {
                    continue;
                }

                AssetItem assetItem = LoadItem<AssetItem>(serializer, file);
                Debug.Log($"{assetItem.ItemID} - {assetItem.NameExt}");
                result.Add(assetItem.ItemID, assetItem);
            }
            return result;
        }
        private static T LoadItem<T>(ISerializer serializer, string path) where T : ProjectItem, new()
        {
            T item = Load<T>(serializer, path);

            string fileNameWithoutMetaExt = Path.GetFileNameWithoutExtension(path);
            item.Name = Path.GetFileNameWithoutExtension(fileNameWithoutMetaExt);
            item.Ext = Path.GetExtension(fileNameWithoutMetaExt);

            return item;
        }

        private static T Load<T>(ISerializer serializer, string path) where T : new()
        {
            string metaFile = path;
            T item;
            if (File.Exists(metaFile))
            {
                try
                {
                    using (FileStream fs = File.OpenRead(metaFile))
                    {
                        item = serializer.Deserialize<T>(fs);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Unable to read meta file: {0} -> got exception: {1} ", metaFile, e.ToString());
                    item = new T();
                }
            }
            else
            {
                item = new T();
            }

            return item;
        }
        private static void SaveDependencies(GetDepsFromContext context, ITypeMap typeMap, IAssetDB assetDB, string path)
        {
            object[] dependencies = context.Dependencies.ToArray();
            foreach (UnityObject dep in dependencies)
            {
                Type persistentType = typeMap.ToPersistentType(dep.GetType());
                if (persistentType != null)
                {
                    context.Dependencies.Clear();

                    IPersistentSurrogate persistentObject = (IPersistentSurrogate)Activator.CreateInstance(persistentType);
                    persistentObject.GetDepsFrom(dep, context);

                    SaveDependencies(context, typeMap, assetDB, path);
                }
            }

            foreach (UnityObject dep in dependencies)
            {
                if (dep is Component || dep is GameObject || !assetDB.IsDynamicResourceID(assetDB.ToID(dep)))
                {
                    continue;
                }

                string name = dep.name;
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = dep.GetType().Name;
                }

                //string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}/{1}.asset", path, name));
                //AssetDatabase.CreateAsset(dep, uniqueAssetPath);
            }
        }

    }
}
