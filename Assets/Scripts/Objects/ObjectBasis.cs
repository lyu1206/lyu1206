using UnityEngine;
using System;
using System.Collections.Generic;

namespace Eos.Objects
{
    using Service;
    using MessagePack;

    public class EosModel : EosObjectBase , ITransform
    {
        private Transform _transform;
        public Transform Transform =>_transform;
        public EosTransformActor PrimaryActor;
        public EosModel()
        {
#if UNITY_EDITOR
            var trans = new GameObject(Name);
            _transform = trans.transform;
#endif
        }
        protected override void OnActivate(bool active)
        {
            PrimaryActor = FindChild<EosTransformActor>();
        }
#if UNITY_EDITOR
        public override string Name 
        { 
            get => base.Name; 
            set
            {
                base.Name = value;
                _transform.name = value;
            }
        }
        public override void OnAncestryChanged()
        {
            base.OnAncestryChanged();
            if (!(_parent is ITransform transactor))
                return;
            _transform.parent = transactor.Transform;
            _transform.localPosition = Vector3.zero;
            _transform.localRotation = Quaternion.identity;
        }

#endif
    }
    public class ReferPlayer
    {
        public EosPlayer.EosPlayer Ref => EosPlayer.EosPlayer.Instance;
    }

    [Serializable]
    [MessagePackObject]
    [Union(0,typeof(EosCamera))]
    [Union(1,typeof(EosTransformActor))]
    [Union(2,typeof(EosHumanoid))]
    [Union(3,typeof(GUIService))]
    [Union(4, typeof(Solution))]
    [Union(5, typeof(Workspace))]
    [Union(6, typeof(EosFsm))]
    [Union(7, typeof(EosLight))]
    [Union(8, typeof(EosMeshObject))]
    [Union(9, typeof(EosPawnActor))]
    [Union(10, typeof(EosScript))]
    [Union(11, typeof(EosTool))]
    public class EosObjectBase : ReferPlayer
    {
        public uint ObjectID;
        private string _name;
        private bool _active = true;
        [Key(1)]public virtual string Name{get=>_name;set=>_name = value;}
        [Key(2)]public bool Active 
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
        protected EosObjectBase _parent;
        public EosObjectBase Parent
        {
            get=>_parent;
            set
            {
                value.AddChild(this);
            }
        }
        protected List<EosObjectBase> _childrens = new List<EosObjectBase>();
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
        public void Activate(bool active,bool recursivechild = true)
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
        public void StartPlay()
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
        public EosObjectBase Clone()
        {
            var type = ObjectFactory.GetRegistType(this);
            if (type == ObjectType.RunTime)
                return null;
            var copy = ObjectFactory.CreateInstance(this.GetType());
            OnCopyTo(copy);
            foreach (var child in _childrens)
            {
                var childclone = child.Clone();
                if (childclone == null)
                    continue;
                copy.AddChild(childclone);
            }
            return copy;
        }
    }
}
