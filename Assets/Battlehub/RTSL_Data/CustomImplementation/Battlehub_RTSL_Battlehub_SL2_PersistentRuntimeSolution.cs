#if !RTSL_MAINTENANCE
using Battlehub.RTSL;
using ProtoBuf;

namespace Battlehub.RTSL.Battlehub.SL2
{
    [CustomImplementation]
    public partial class PersistentRuntimeSolution<TID>
    {
        [ProtoMember(1)]
        public byte[] _solutiondata;
        public Eos.Service.Solution Solution;
        public override void ReadFrom(object obj)
        {
            var solution = obj as RuntimeSolution;
            _solutiondata = MessagePack.MessagePackSerializer.Serialize(solution.Solution);
            base.ReadFrom(obj);
        }

        public override object WriteTo(object obj)
        {
            return base.WriteTo(obj);
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

