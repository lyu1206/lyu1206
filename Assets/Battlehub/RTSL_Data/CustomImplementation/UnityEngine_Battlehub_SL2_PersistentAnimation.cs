#if !RTSL_MAINTENANCE
using System;
using System.Collections.Generic;
using Battlehub.RTSL;
using ProtoBuf;
namespace UnityEngine.Battlehub.SL2
{
    [CustomImplementation]
    public partial class PersistentAnimation<TID>
    {
        [ProtoMember(277)]
        public TID[] animationclips;

        public override void ReadFrom(object obj)
        {
            base.ReadFrom(obj);
            var animation = obj as Animation;
            if (animation == null)
                return;
            //var aniclips = animation.animationClips;
            var cliplist = new List<TID>();
            foreach (AnimationState clip in animation)
            {
                var clipid = ToID(clip.clip);
                cliplist.Add(clipid);
            }
            animationclips = cliplist.ToArray();
        }

        public override object WriteTo(object obj)
        {
            var uo = obj as Animation;
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var ani in animationclips)
            {
                var clip = FromID<AnimationClip>(ani);
                //if (string.IsNullOrEmpty(clip.name))
                //    continue;
                uo.AddClip(clip, clip.name);
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
            }
            //            uo.ApplyOverrides(anims);
            return base.WriteTo(obj);
        }

        public override void GetDeps(GetDepsContext<TID> context)
        {
            foreach (var clip in animationclips)
                context.Dependencies.Add(clip);
            base.GetDeps(context);
        }

        public override void GetDepsFrom(object obj, GetDepsFromContext context)
        {
            base.GetDepsFrom(obj, context);
            var animation = obj as Animation;
            if (animation == null)
                return;
            var clips = animation;
            foreach (AnimationState clip in clips)
                AddDep(clip.clip, context);
        }
        //public override bool CanInstantiate(Type type)
        //{
        //    return true;
        //}
        //public override object Instantiate(Type type)
        //{
        //    var controller = Resources.Load("RuntimeAnimatorController") as RuntimeAnimatorController;
        //    var runtimecontroller = RuntimeAnimatorController.Instantiate<RuntimeAnimatorController>(controller);
        //    return new AnimatorOverrideController(runtimecontroller);
        //}
    }
}
#endif

