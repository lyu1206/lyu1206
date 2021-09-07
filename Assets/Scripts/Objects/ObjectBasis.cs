using UnityEngine;
using System;
using System.Collections.Generic;

namespace Eos.Objects
{
    using UI;
    using Service;
    using Service.AI;
    using MessagePack;

    [System.Serializable]
    public class EosModel : EosObjectBase , ITransform
    {
        private EosTransform _transform;
        [IgnoreMember]public EosTransform Transform =>_transform;
        [IgnoreMember] public EosTransformActor PrimaryActor;
        public EosModel()
        {
#if UNITY_EDITOR
            _transform = ObjectFactory.CreateInstance<EosTransform>();
            _transform.Create(Name);
#endif
        }
        protected override void OnActivate(bool active)
        {
            PrimaryActor = FindChild<EosTransformActor>();
        }
#if UNITY_EDITOR
        [MessagePack.IgnoreMember]
        public override string Name 
        { 
            get => base.Name; 
            set
            {
                base.Name = value;
                if (_transform == null)
                    return;
                _transform.Name = value;
            }
        }
        public override void OnAncestryChanged()
        {
            base.OnAncestryChanged();
            if (!(_parent is ITransform transactor))
                return;
            _transform.SetParent(transactor.Transform);
        }

#endif
    }
    public class ReferPlayer
    {
        [IgnoreMember]public EosPlayer.EosPlayer Ref => EosPlayer.EosPlayer.Instance;
    }
    [Serializable]
    [MessagePackObject]
    [Union(0, typeof(EosCamera))]
    [Union(1, typeof(EosTransformActor))]
    [Union(2, typeof(EosHumanoid))]
    [Union(3, typeof(GUIService))]
    [Union(4, typeof(AIService))]
    [Union(5, typeof(Solution))]
    [Union(6, typeof(Workspace))]
    [Union(7, typeof(EosFsm))]
    [Union(8, typeof(EosLight))]
    [Union(9, typeof(EosMeshObject))]
    [Union(10, typeof(EosPawnActor))]
    [Union(11, typeof(EosScript))]
    [Union(12, typeof(EosTool))]
    [Union(13, typeof(EosModel))]
    [Union(14, typeof(TerrainService))]
    [Union(15, typeof(EosTerrain))]
    [Union(16, typeof(StarterPlayer))]
    [Union(17, typeof(StarterPack))]
    [Union(18, typeof(EosUIObject))]
    [Union(19, typeof(EosTextMesh))]
    [Union(20, typeof(EosCollider))]
    public abstract partial class EosObjectBase : ReferPlayer, IMessagePackSerializationCallbackReceiver
    {
        public EosObjectBase()
        {
        }
        [IgnoreMember]public uint ObjectID;
        private string _name;
        private bool _active = true;
        [Key(1)] public List<EosObjectBase> _childrens  = new List<EosObjectBase>();
        [Inspector("Basic","Name")]
        [Key(2)]public virtual string Name{get=>_name;set=>_name = value;}
        [Inspector("Basic", "Active")]
        [Key(3)]public bool Active 
        { 
            get => _active;
            set
            {
                if (_active == value)
                    return;
                Activate(value);
            }
        }
        [IgnoreMember]
        public bool ActiveInHierachy
        {
            get
            {
                if (!Active) return false;
                if (Parent == null) return Active;
                return Active && Parent.ActiveInHierachy;
            }
        }
        [IgnoreMember] protected EosObjectBase _parent;

        [IgnoreMember]
        public EosObjectBase Parent
        {
            get=>_parent;
            set
            {
                value.AddChild(this);
            }
        }
        public void AddChild(EosObjectBase obj)
        {
            if (obj.ObjectID==0)
                Ref.ObjectManager.RegistObject(obj);
            if (obj._parent !=null)
                obj._parent.RemoveChild(obj);
            obj._parent = this;
            obj.AncestryChanged();
            _childrens.Add(obj);
            OnChildAdded(obj);
        }
        public virtual void OnCopyTo(EosObjectBase target)
        {
            if (target.ObjectID == 0)
                Ref.ObjectManager.RegistObject(target);
            target.Name = Name;
        }
        public virtual void OnCreate()
        {
            if (ObjectID == 0)
                Ref.ObjectManager.RegistObject(this);
        }
        public virtual void Activate(bool active,bool recursivechild = true)
        {
            OnActivate(active);
            if (recursivechild)
            {
                foreach (var child in _childrens)
                    child.Activate(active, recursivechild);
            }
        }
        public void Destroy(bool silence = true)
        {
            if (silence && _parent!=null)
                _parent.RemoveChild(this);
            Ref.ObjectManager.UnRegistObject(this);
            OnDestroy();
            foreach (var child in _childrens)
                child.Destroy(false);
        }
        public virtual void OnDestroy()
        {
        }
        public virtual void StartPlay()
        {
            OnStartPlay();
            foreach(var child in _childrens)
                child.StartPlay();
        }
        protected virtual void OnActivate(bool active){}
        protected virtual void OnStartPlay(){}
        public virtual void Update(float delta){}
        public void RemoveChild(EosObjectBase obj)
        {
            _childrens.Remove(obj);
            OnChildRemoved(obj);
        }
        public virtual void OnChildAdded(EosObjectBase child)
        {

        }
        public virtual void OnChildRemoved(EosObjectBase child)
        {

        }
        public void AncestryChanged()
        {
            OnAncestryChanged();
            foreach(var child in _childrens)
                child.AncestryChanged();
        }
        public void IterChilds(Action<EosObjectBase> action,bool isrecursive = false)
        {
            _childrens.ForEach(it =>
            {
                action(it);
                if (isrecursive && it!=null)
                    it.IterChilds(action,isrecursive);
            });
        }
        public T FindDeepChild<T>() where T : EosObjectBase
        {
            var find = FindChild<T>();
            if (find != null)
                return find;
            foreach (var child in _childrens)
            {
                find = child.FindDeepChild<T>();
                if (find != null)
                    return find;
            }
            return null;
        }
        public T FindChild<T>() where T : EosObjectBase
        {
            foreach(var child in _childrens)
            {
                if (child is T)
                    return (T)child;
            }
            return null;
        }
        public List<T> FindChilds<T>() where T : EosObjectBase
        {
            var retlist = new List<T>();
            foreach (var child in _childrens)
            {
                if (child is T)
                    retlist.Add(child as T);
            }
            return retlist;
        }
        public T FindChild<T>(string name) where T :EosObjectBase
        {
            foreach(var child in _childrens)
            {
                if (child is T && child.Name == name)
                    return (T)child;
            }
            return null;
        }
        public T FindInParent<T>() where T :EosObjectBase
        {
            var parent = _parent;
            while(parent!=null)
            {
                if (parent is T)
                    return parent as T;
                parent = parent._parent;
            }
            return null;
        }
        public virtual void OnAncestryChanged()
        {
        }
        public EosObjectBase Clone(EosObjectBase parent)
        {
            var type = ObjectFactory.GetRegistType(this);
            //if (type == ObjectType.RunTime)
            //    return null;
            var copy = ObjectFactory.CreateInstance(this.GetType());
            copy.OnCreate();
            OnCopyTo(copy);
            parent?.AddChild(copy);
            foreach (var child in _childrens)
            {
                var childclone = child.Clone(copy);
                if (childclone == null)
                    continue;
            }
            return copy;
        }

        public virtual void OnBeforeSerialize()
        {
//            throw new NotImplementedException();
        }

        public virtual void OnAfterDeserialize()
        {
            OnCreate();
            if (_childrens == null)
                return;
            var childrens = new List<EosObjectBase>(_childrens);
            _childrens.Clear();
            foreach (var it in childrens)
            {
                if (it == null)
                    continue;
                AddChild(it);
            }
            //            throw new NotImplementedException();
        }
    }
}
