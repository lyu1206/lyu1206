using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Eos.Objects
{
    public class ObjectManager
    {
        private Dictionary<uint,EosObjectBase> _objectlist = new Dictionary<uint, EosObjectBase>();
        private Dictionary<int,EosObjectBase> _unitylist = new Dictionary<int, EosObjectBase>();
        private List<EosObjectBase> _updateobjectlist = new List<EosObjectBase>();
        private List<EosObjectBase> _latedeleteupdateobjectlist = new List<EosObjectBase>();
        private List<EosObjectBase> _lateaddupdateobjectlist = new List<EosObjectBase>();
        public EventHandler<EosObjectBase> OnUnRegistObject;
        public EosObjectBase this[uint index]
        {
            get
            {
                if (_objectlist.ContainsKey(index))
                    return _objectlist[index];
                return null;
            }
        }
        static private uint objectkey = 1;
        public void RegistObject(EosObjectBase obj)
        {
            if (_objectlist.ContainsKey(obj.ObjectID))
                return;
            // if (obj is ITransform unityobj )
            //     _unitylist.Add(unityobj.Transform.gameObject.GetHashCode(),obj);
            if (obj.ObjectID != 0)
                _objectlist.Add(obj.ObjectID, obj);
            else
            {
                obj.ObjectID = objectkey;
                _objectlist.Add(objectkey, obj);
                objectkey++;
            }
        }
        public void UnRegistObject(EosObjectBase obj)
        {
            _objectlist.Remove(obj.ObjectID);
            UnRegistUpdateObject(obj);
            OnUnRegistObject?.Invoke(this, obj);
        }
        public void Reset()
        {
            _objectlist = new Dictionary<uint, EosObjectBase>();
            _unitylist = new Dictionary<int, EosObjectBase>();
            _updateobjectlist = new List<EosObjectBase>();
            _latedeleteupdateobjectlist = new List<EosObjectBase>();
            _lateaddupdateobjectlist = new List<EosObjectBase>();
        }
        public EosObjectBase GetFromUnityObject(GameObject unityobj)
        {
            var unityhashcode = unityobj.GetHashCode();
            if (_unitylist.ContainsKey(unityhashcode))
                return _unitylist[unityhashcode];
            return null;
        }
        public void RegistUpdateObject(EosObjectBase obj)
        {
            if (_lateaddupdateobjectlist.Contains(obj))
                return;
            _lateaddupdateobjectlist.Add(obj);
        }
        public void UnRegistUpdateObject(EosObjectBase obj)
        {
            if (_latedeleteupdateobjectlist.Contains(obj))
                return;
            _latedeleteupdateobjectlist.Add(obj);
        }
        public void Update(float delta)
        {
            if (_lateaddupdateobjectlist.Count>0)
                _lateaddupdateobjectlist.ForEach(obj => _updateobjectlist.Add(obj));
            if (_latedeleteupdateobjectlist.Count>0)
                _latedeleteupdateobjectlist.ForEach(obj => _updateobjectlist.Remove(obj));
            foreach (var it in _updateobjectlist)
                it.Update(delta);
            _lateaddupdateobjectlist.Clear();
            _latedeleteupdateobjectlist.Clear();
        }
    }
}
namespace EosPlayer
{
    using EosLuaPlayer;
    using Eos.Service;
    using Eos.Objects;
    using Eos.Script;

    public partial class EosPlayer : MonoBehaviour
    {
        private Solution _solution;
        private ObjectManager _objectmanager = new ObjectManager();
        private ScriptPlayer _scriptPlayer = new ScriptPlayer();
        private Scheduler _scheduler = new Scheduler();
        private EosLuaPlayer _luaplayer = new EosLuaPlayer();
        private bool _isplaying;
        private static EosPlayer _instance;
        public ObjectManager ObjectManager =>_objectmanager;
        public Solution Solution => _solution;
        public CoroutineManager Coroutine = null;
        public ScriptPlayer ScriptPlayer => _scriptPlayer;
        public Scheduler Scheduler => _scheduler;
        public EosLuaPlayer LuaPlayer => _luaplayer;
        public bool IsPlaying => _isplaying;

        public static EosPlayer Instance
        {
            get
            {
                if (_instance ==null)
                {
                    var unityadapter = ObjectFactory.CreateUnityInstance("EosPlayer").gameObject;
                    _instance = unityadapter.AddComponent<EosPlayer>();
                    _instance.Coroutine = new CoroutineManager(_instance);
                }
                return _instance;
            }
        }
        void Awake()
        {
            IngameScriptContainer.Initialize();
            enabled = false;
        }
        void Update()
        {
            ObjectManager.Update(Time.deltaTime);
            Scheduler.Update(Time.deltaTime);
            LuaPlayer.Update();
        }
        private void LateUpdate()
        {
            
        }
        public void Play()
        {

            _isplaying = true;
            _luaplayer.Init();
            _instance.Solution?.StartGame();
            enabled = true;

        }
        public void Stop()
        {
            _isplaying = false;
            enabled = false;
            try
            {
                _instance?.Solution?.Destroy();
            }
            catch(Exception ex)
            {

            }
            Scheduler.Clear();
        }
        public static void ShutDown()
        {
            _instance?.Solution?.Destroy();
            _instance = null;
            
        }
        void OnApplicationQuit()
        {
            ShutDown();
        }

#if UNITY_EDITOR
        public void SetSolution(Solution solution)
        {
            _solution = solution;
        }
#endif
    }
    public class UGUIEvents : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IDropHandler
    {
        private EventHandler<PointerEventData> _onbegindrag;
        private EventHandler<PointerEventData> _ondrag;
        private EventHandler<PointerEventData> _ondrop;
        private EventHandler<PointerEventData> _onenddrag;
        private EventHandler<PointerEventData> _onpointerdown;
        public void RegistOnBeginDrag(EventHandler<PointerEventData> callback)
        {
            _onbegindrag += callback;
        }
        public void UnRegistOnBeginDrag(EventHandler<PointerEventData> callback)
        {
            _onbegindrag -= callback;
        }
        public void RegistOnDrag(EventHandler<PointerEventData> callback)
        {
            _ondrag += callback;
        }
        public void UnRegistOnDrag(EventHandler<PointerEventData> callback)
        {
            _ondrag -= callback;
        }
        public void RegistOnDrop(EventHandler<PointerEventData> callback)
        {
            _ondrop += callback;
        }
        public void UnRegistOnDrop(EventHandler<PointerEventData> callback)
        {
            _ondrop -= callback;
        }
        public void RegistOnEndDrag(EventHandler<PointerEventData> callback)
        {
            _onenddrag += callback;
        }
        public void UnRegistOnEndDrag(EventHandler<PointerEventData> callback)
        {
            _onenddrag -= callback;
        }
        public void RegistOnPointerDown(EventHandler<PointerEventData> callback)
        {
            _onpointerdown += callback;
        }
        public void UnRegistOnPointerDown(EventHandler<PointerEventData> callback)
        {
            _onpointerdown -= callback;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            _onbegindrag?.Invoke(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _ondrag?.Invoke(this, eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            _ondrop?.Invoke(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _onenddrag?.Invoke(this, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onpointerdown?.Invoke(this, eventData);
        }
    }
}