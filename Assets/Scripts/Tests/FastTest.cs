using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using ProtoBuf;
using UnityEngine.Networking;

namespace Battlehub.RTSL
{
    public class RemoteAssetDB : AssetDB
    {

    }
}
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

        [SerializeField]
        private RuntimeAnimatorController _controller;

        private static FastTest _instance;
        public static FastTest Instance => _instance;

        private ITypeMap m_typeMap;
        private IAssetDB<long> m_assetDB;
        private Dictionary<long, RemoteAssetItem> _resourcesmeta;
        private Dictionary<long, RemoteAssetItem> _rawresources;

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
        public UnityObject Convert(UnityObject obj)
        {
            var animator = obj as Animator;
            var convertgm = new GameObject();
            var to = convertgm.AddComponent<Animation>();
            var aninames = new List<string>();

            if (animator.runtimeAnimatorController != null)
            {
                animator.runtimeAnimatorController.animationClips.ForEach
                    (
                        it =>
                        {
                            it.legacy = true;
                            to.AddClip(it, it.name);
                            aninames.Add(it.name);
                        }
                    );
            }
            DestroyImmediate(obj);
            //Destroy(to);
            GameObject.DestroyImmediate(convertgm);
            return to;
        }
        private Tuple<RemoteAssetItem,PersistentObject<long>, Dictionary<long,PersistentObject<long>>> PTest(GameObject obj)
        {
            var notMapped = new List<UnityObject>();
            var assetItem = new RemoteAssetItem();
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

            for (int i=0;i<notMapped.Count;i++)
            {
                var it = notMapped[i];
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
                Type persisttype = null;
                if (dep.GetType() == typeof(UnityEditor.Animations.AnimatorController))
                {
                    persisttype = typeof(PersistentRuntimeAnimatorController<long>);
                }
                else
                    persisttype = m_typeMap.ToPersistentType(dep.GetType());
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
            return new Tuple<RemoteAssetItem, PersistentObject<long>, Dictionary<long, PersistentObject<long>>>(assetItem,persistentObject,dependancies);
        }
        private const string PreviewExt = ".rtview";
        private const string MetaExt = ".rtmeta";
        void WriteAssetItem(Tuple<RemoteAssetItem, PersistentObject<long>, Dictionary<long, PersistentObject<long>>> asset)
        {
            var serializer = IOC.Resolve<ISerializer>();
            var assetitem = asset.Item1;
            var pobj = asset.Item2;
            var metafilepath = Application.streamingAssetsPath + "/";
            var librarypath = Application.streamingAssetsPath;
            var previewpath = librarypath + "/" + assetitem.NameExt + PreviewExt;
            assetitem.path = librarypath + "/";
            using (FileStream fs = File.Create(previewpath))
            {
                serializer.Serialize(assetitem.Preview, fs);
            }
            using (FileStream fs = File.Create(assetitem.path + assetitem.NameExt))
            {
                serializer.Serialize(pobj,fs);
            }
            var factory = IOC.Resolve<IUnityObjectFactory>();
            foreach (var dpitem in asset.Item3)
            {
//                if (!m_assetDB.IsMapped(dpitem.Key))
                {
                    string dpassetpath;
                    var context = new GetDepsContext<long>();
                    dpitem.Value.GetDeps(context);
                    if (context.Dependencies.Count>0)
                        dpassetpath = Application.streamingAssetsPath + "/";
                    else
                        dpassetpath = Application.streamingAssetsPath + "/RawResource/";
                    var dpassetitem = new RemoteAssetItem();
                    dpassetitem.ItemID = dpitem.Key;
                    dpassetitem.Name = dpitem.Value.name;
                    dpassetitem.path = dpassetpath;
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
                    using (FileStream fs = File.Create(metafilepath + dpassetitem.NameExt+MetaExt))
                    {
                        serializer.Serialize(dpassetitem, fs);
                    }
                    using(FileStream fs = File.Create(dpassetpath + dpassetitem.NameExt))
                    {
                        serializer.Serialize(dpitem.Value, fs);
                    }
                }
            }
            File.Delete(metafilepath + assetitem.NameExt + MetaExt);
            using (FileStream fs = File.Create(metafilepath + assetitem.NameExt + MetaExt))
            {
                serializer.Serialize(assetitem, fs);
            }
        }
        private Stack<Tuple<long, PersistentObject<long>>> _resourcepersistents = new Stack<Tuple<long, PersistentObject<long>>>();
        private void LoadAssetWithMetaData(long metakey)
        {
            var librarypath = Application.streamingAssetsPath;
            var serializer = IOC.Resolve<ISerializer>();
            var typeMap = IOC.Resolve<ITypeMap>();
            var factory = IOC.Resolve<IUnityObjectFactory>();
            if (_resourcesmeta.ContainsKey(metakey) && !m_assetDB.IsMapped(metakey))
            {
                var meta = _resourcesmeta[metakey];
                var assetobjectpath = meta.path + meta.NameExt;
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
                            _resourcepersistents.Push(new Tuple<long, PersistentObject<long>>(metakey,resource));
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
                obj = serializer.Deserialize<PersistentRuntimePrefab<long>>(fs);
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
                //using (BinaryReader br = new BinaryReader(fs))
                //{
                //    var datasize = br.ReadInt32();
                //    var data = br.ReadBytes(datasize);
                //    obj = serializer.Deserialize<PersistentRuntimePrefab<long>>(data);
                //    obj.CreateGameObjectWithComponents(m_typeMap, obj.Descriptors[0], idtoobj, null, createdGameObjects);
                //    m_assetDB.RegisterDynamicResources(idtoobj);
                //    foreach (var it in obj.Dependencies)
                //    {
                //        if (idtoobj.ContainsKey(it))
                //            m_assetDB.RegisterSceneObject(it, idtoobj[it]);
                //        else
                //        {
                //            LoadAssetWithMetaData(it);
                //        }
                //    }
                //}
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
            foreach (var it in _resourcepersistents)
            {
                Debug.Log($"{it.Item2.name} loaded.");
                var unityobject = m_assetDB.FromID<UnityObject>(it.Item1);
                it.Item2.WriteTo(unityobject);
            }
            var rr = _resourcesmeta[uid];
            var uo = ReadAssetItem(rr.NameExt);
            return uo;
        }
        public void GetDependancies(long id,Stack<long> dep)
        {
            var meta = _resourcesmeta[id];
            dep.Push(id);
            if (meta.Dependencies == null)
                return;
            foreach (var it in meta.Dependencies)
                GetDependancies(it, dep);
        }
        public async Task AssetLoadTest(Stack<long> deps)
        {
            var serializer = IOC.Resolve<ISerializer>();
            foreach (var assetid in deps)
            {
                var rr = _resourcesmeta[assetid];
                var req = UnityWebRequest.Get(rr.Path + rr.NameExt);
                await req.SendWebRequest();
                if (req.isDone)
                {
                    var data = req.downloadHandler.data;
                    var type = m_typeMap.ToType(rr.TypeGuid);
                    var item = serializer.Deserialize<PersistentObject<long>>(data);

                    if (type == typeof(GameObject))
                    {
                        var idtoobj = new Dictionary<long, UnityObject>();
                        var prefab = item as PersistentRuntimePrefab<long>;
                        List<GameObject> createdGameObjects = new List<GameObject>();
                        prefab.CreateGameObjectWithComponents(m_typeMap, prefab.Descriptors[0], idtoobj, null, createdGameObjects);
                        foreach(var idobj in idtoobj)
                        {
                            m_assetDB.RegisterDynamicResource(idobj.Key, idobj.Value);
                        }
                        prefab.WriteTo(createdGameObjects[0]);
                        m_assetDB.RegisterSceneObject(assetid, createdGameObjects[0]);
                    }
                    else
                    {
                        var factory = IOC.Resolve<IUnityObjectFactory>();
                        var unitydpobjtype = m_typeMap.ToUnityType(item.GetType());
                        if (unitydpobjtype != null)
                        {
                            if (factory.CanCreateInstance(unitydpobjtype, item))
                            {
                                UnityObject assetInstance = factory.CreateInstance(unitydpobjtype, item);
                                if (assetInstance != null)
                                {
                                    m_assetDB.RegisterSceneObject(assetid, assetInstance);
                                    item.WriteTo(assetInstance);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void LoadAndSolutionTest()
        {
            var rts = OpenSolutionTest();

            var cam = rts.FindDeepChild<EosCamera>();
            var avt = rts.FindDeepChild<EosPawnActor>();
            cam.LocalPosition = new Vector3(0, 10, 10);
            cam.LookAt(avt);


            EosPlayer.Instance.SetSolution(rts as Solution);
            EosPlayer.Instance.Play();
        }
        // Start is called before the first frame update
        void Start()
        {
            _resourcesmeta = GetRawResourceMeta("");

            //LoadAndSolutionTest();
            //return;


            //foreach(var it in _resourcesmeta)
            //{
            //    LoadAssetWithMetaData(it.Key);
            //}

            //var deps = new Stack<long>();
            //GetDependancies(8589957390, deps);
            //AssetLoadTest(deps);

            //var cr = GetAsset(8589957384);
            //var controller = GetResource(8589957384);
            //            Init();

            //var asset = PTest(_bone);
            //WriteAssetItem(asset);


            //ReadyFiles();
            //ReadFiles();


            //var tt = ReadAssetItem("Bag01.rtprefab");
            //tt = ReadAssetItem("glasses03.rtprefab");
            //tt = ReadAssetItem("Hair03.rtprefab");
            //            tt = ReadAssetItem("CharacterBone01.rtprefab");

            //foreach (var it in _resourcepersistents)
            //{
            //    Debug.Log($"{it.Item2.name} loaded.");
            //    var unityobject = m_assetDB.FromID<UnityObject>(it.Item1);
            //    it.Item2.WriteTo(unityobject);
            //}

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

            var humanoid = ObjectFactory.CreateEosObject<EosHumanoid>();humanoid.Name = "Player";
            objroot.AddChild(humanoid);

            var avatar = ObjectFactory.CreateEosObject<EosPawnActor>();avatar.Name = EosHumanoid.humanoidroot;
            var bone = ObjectFactory.CreateEosObject<EosBone>();bone.Bone = _bone;bone.Name = "bone";bone.BoneGUID = 8589957390;
            avatar.AddChild(bone);
            var collider = ObjectFactory.CreateEosObject<EosCollider>();collider.Name = "Collider";
            collider.ColliderType = ColliderType.Capsule;
            avatar.AddChild(collider);

            objroot.AddChild(avatar);
            avatar.LocalPosition = new Vector3(0, 3, 0);
            avatar.LocalScale = Vector3.one;

            foreach (var gearsrc in _gears)
            {
                var gear = ObjectFactory.CreateEosObject<EosGear>();
                var gearmeta = _resourcesmeta.Where(t => t.Value.NameExt == gearsrc.name + ".rtprefab").Select(t => t.Value);
                var gearmetalist = gearmeta.ToList();
                gear.Part = gearsrc;
                gear.GearGUID = gearmetalist[0].ItemID;
                avatar.AddChild(gear);
            }

            camera.LocalPosition = new Vector3(0, 10, 10);
            camera.LookAt(avatar);


            var testscript = new EosScript { Name = "Script" };
            testscript.LuaScript =
                @"
                    local i = 0
                    local this = _this_object
                    local avatar = this.Parent;
                    print('Script Owner:',avatar.Name,' position:',avatar.LocalPosition)
                    local position = avatar.LocalPosition
                    position.x = 3;
                    avatar.LocalPosition = position
                    while i<60 do
                        i = i + 1
                        print('second count:',i,' obj - ',tostring(this),' objname:',this.Name)
                        coroutine.yield()
                    end
                ";
            avatar.AddChild(testscript);



            var typemap = IOC.Resolve<ITypeMap>();
            var ptype = typemap.ToPersistentType(solution.GetType());



            Type objType = solution.GetType();
            Type persistentType = m_typeMap.ToPersistentType( typeof(RuntimeSolution));
            var rtsolution = ScriptableObject.CreateInstance<RuntimeSolution>();
            rtsolution.Solution = solution;
            var serializer = IOC.Resolve<ISerializer>();
            var persistentObject = Activator.CreateInstance(persistentType) as PersistentObject<long>;
            persistentObject.ReadFrom(rtsolution);

            #region Save Workspace to Solution
            //var solutionpath = Application.streamingAssetsPath + "/";
            //using (FileStream fs = File.Create(solutionpath + "FastTest.solution"))
            //{
            //    serializer.Serialize(persistentObject, fs);
            //}
            #endregion

            EosPlayer.Instance.SetSolution(solution as Solution);
            EosPlayer.Instance.Play();

        }
        Solution OpenSolutionTest()
        {
            var solutionpath = Application.streamingAssetsPath + "/";
            var serializer = IOC.Resolve<ISerializer>();
            using (FileStream fs = File.Open(solutionpath + "FastTest.solution",FileMode.Open))
            {
                var solution = serializer.Deserialize<PersistentRuntimeSolution<long>>(fs);
                var rtsolution = ScriptableObject.CreateInstance<RuntimeSolution>();
                solution.WriteTo(null);
                return solution.Solution;
            }
        }

        // Update is called once per frame
        void Update()
        {
            var direction = Vector3.zero;
            var h = EosPlayer.Instance.Solution.Workspace.FindDeepChild<EosHumanoid>();
            if (Input.GetKey(KeyCode.W))
                direction += -Vector3.forward; ;
            if (Input.GetKey(KeyCode.S))
                direction += Vector3.forward;
            if (Input.GetKey(KeyCode.A))
                direction += Vector3.left; ;
            if (Input.GetKey(KeyCode.D))
                direction += Vector3.right;
            if (Input.GetKeyDown(KeyCode.Space))
                h.Jump = true;
            h.MoveDirection = direction;
        }
        private void OnDestroy()
        {
            m_assetDB.UnregisterDynamicResources();
            m_assetDB.UnregisterSceneObjects();
        }
        private Dictionary<long, RemoteAssetItem> GetRawResourceMeta(string folder)
        {
            var result = new Dictionary<long, RemoteAssetItem>();
            var serializer = IOC.Resolve<ISerializer>();
            var rawresourcepath = Application.streamingAssetsPath + folder;
            var storage = IOC.Resolve<IStorage<long>>();
            string[] files = Directory.GetFiles(rawresourcepath, "*" + MetaExt,SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                string file = files[i];
                //if (!File.Exists(file.Replace(MetaExt, string.Empty)))
                //{
                //    continue;
                //}

                var assetItem = LoadItem<RemoteAssetItem>(serializer, file);
//                Debug.Log($"{assetItem.ItemID} - {assetItem.NameExt}");
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
        ////////////////////////// ---------------------------- async Resource Managing --------------------------- ///////////////////////////
        ///
        public async Task<UnityObject> GetAsset(long uid)
        {
            if (!_resourcesmeta.ContainsKey(uid))
                return null;
            var rr = _resourcesmeta[uid];
            var req = UnityWebRequest.Get(rr.Path + rr.NameExt);
            await req.SendWebRequest();
            if (req.isDone)
            {
                var serializer = IOC.Resolve<ISerializer>();
                PersistentRuntimePrefab<long> obj = null;
                var idtoobj = new Dictionary<long, UnityObject>();
                List<GameObject> createdGameObjects = new List<GameObject>();
                using (BinaryReader br = new BinaryReader(new MemoryStream(req.downloadHandler.data)))
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
                            await GetAsset(it);
                        }
                    }
                }

            }
            var uo = ReadAssetItem(rr.NameExt);

            foreach (var it in _resourcepersistents)
            {
                Debug.Log($"{it.Item2.name} loaded.");
                var unityobject = m_assetDB.FromID<UnityObject>(it.Item1);
                it.Item2.WriteTo(unityobject);
            }

            return uo;
        }
    }
}
