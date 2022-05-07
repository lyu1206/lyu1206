namespace Battlehub.RTSL
{
    public class RuntimePrefab : UnityEngine.Object
    { }
    public class RuntimeScene : RuntimePrefab
    { }
}
namespace UnityEngine
{
    public class NodeChannel : Object
    {
        public string path;
        public string propertyname;
        public UnityEngine.Keyframe[] keys;
    }
    public class NodeAnimationClip : Object
    {
        public NodeChannel[] nodeChannels;
    }
}