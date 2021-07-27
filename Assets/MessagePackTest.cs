using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

[MessagePackObject]
[Union(1, typeof(PARENTclass))]
[Union(2, typeof(TESTClass))]
//[Union(3, typeof(MMBase))]
public abstract class MBase
{
    private string _name;
    public MBase()
    {
    }
    [Key(1)]
    public List<MBase> TestList = new List<MBase>();
    [Key(222)]
    public virtual string Name { get=> _name; set=> _name=value; }
}
[MessagePackObject]
public class MMBase : MBase
{
    private string _dddd;
    [Key(11)]
    public int A;
    [Key(2222)] public override string Name { get => base.Name; set => _dddd = value; }
}
[MessagePackObject]
public class TESTClass : MMBase
{
    [Key(21)]
    public int B;

}
[MessagePackObject]
public class PARENTclass : MMBase
{
    [Key(21)]
    public int C;

}
[System.Serializable]
public class SSSSClass
{
    public int A;
}
public class MessagePackTest : MonoBehaviour
{
    [SerializeField]
    SSSSClass GGG;
    [SerializeField]
    Eos.Objects.EosObjectBase ter = new Eos.Objects.EosTerrain();
    // Start is called before the first frame update
    void Start()
    {
        MBase mmbase = new PARENTclass();
        var tt = new TESTClass();
        mmbase.TestList.Add(tt);
        var msgpackData = MessagePackSerializer.Serialize(mmbase);
        var decodetest = MessagePackSerializer.Deserialize<MBase>(msgpackData);

        var testobj = gameObject.AddComponent<Eos.Objects.Editor.EosTransformActorEditor>();
        testobj._transformactor = new Eos.Objects.EosTransformActor(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
