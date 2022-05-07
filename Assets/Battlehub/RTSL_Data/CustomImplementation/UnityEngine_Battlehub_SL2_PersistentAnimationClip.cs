#if !RTSL_MAINTENANCE
using UnityEditor;
using Battlehub.RTSL;
using ProtoBuf;

namespace Battlehub.RTAnimation.Battlehub.SL2
{
    public class AA
    {

    }
}

namespace UnityEngine.Battlehub.SL2
{
    [CustomImplementation]
    public partial class PersistentAnimationClip<TID>
    {
        [ProtoMember(261)]
        public PersistentNodeChannel<TID>[] animationChannels;
        public override void ReadFrom(object obj)
        {
            base.ReadFrom(obj);
            var clip = obj as AnimationClip;
            var curves = UnityEditor.AnimationUtility.GetCurveBindings(clip);
            var channelindex = 0;
            animationChannels = new PersistentNodeChannel<TID>[curves.Length];
            foreach (var curv in curves)
            {
                var channel = new PersistentNodeChannel<TID> { path = curv.path, propertyname = curv.propertyName };
                var cv = UnityEditor.AnimationUtility.GetEditorCurve(clip, curv);
//                channel.type = 1;
                channel.keys = new PersistentKeyframe<TID>[cv.length];
                for (int i = 0; i < cv.length; i++)
                {
                    var key = new PersistentKeyframe<TID>();
                    key.ReadFrom(cv.keys[i]);
                    channel.keys[i] = key;
                }
                animationChannels[channelindex++] = channel;
            }
        }

        public override object WriteTo(object obj)
        {
            //var clip = obj as AnimationClip;
            //base.WriteTo(obj);
            //foreach (var curve in animationChannels)
            //{
            //    var anicurve = new AnimationCurve();
            //    var nodechannel = new NodeChannel();
            //    curve.ReadFrom(nodechannel);
            //    clip.SetCurve(curve.path, typeof(Transform), curve.propertyname, new AnimationCurve { keys = nodechannel.keys });
            //}
            return obj;
        }

        public override void GetDeps(GetDepsContext<TID> context)
        {
            base.GetDeps(context);
        }

        public override void GetDepsFrom(object obj, GetDepsFromContext context)
        {
            base.GetDepsFrom(obj, context);
        }
    }
}
#endif

