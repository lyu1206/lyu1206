#if !RTSL_MAINTENANCE
using Battlehub.RTSL;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace UnityEngine.Battlehub.SL2
{
    [CustomImplementation]
    public partial class PersistentRuntimeAnimatorController<TID>
    {
        [ProtoMember(277)]
        public TID[] animationclips;

        public override void ReadFrom(object obj)
        {
            base.ReadFrom(obj);
            var controller = obj as RuntimeAnimatorController;
            if (controller == null)
                return;
            var aniclips = controller.animationClips;
            var cliplist = new List<TID>();
            foreach (var clip in aniclips)
            {
                var clipid = ToID(clip);
                cliplist.Add(clipid);
            }
            animationclips = cliplist.ToArray();
        }

        public override object WriteTo(object obj)
        {
            var uo = obj as AnimatorOverrideController;
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach(var ani in animationclips)
            {
                var clip = FromID<AnimationClip>(ani);
                uo[clip.name] = clip;
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
            var controller = obj as RuntimeAnimatorController;
            if (controller == null)
                return;
            var clips = controller.animationClips;
            foreach (var clip in clips)
                AddDep(clip, context);
        }
        public override bool CanInstantiate(Type type)
        {
            return true;
        }
        public override object Instantiate(Type type)
        {
            var controller = Resources.Load("RuntimeAnimatorController") as RuntimeAnimatorController;
            var runtimecontroller = RuntimeAnimatorController.Instantiate<RuntimeAnimatorController>(controller);
            return new AnimatorOverrideController(runtimecontroller);
        }
    }
}
#endif

