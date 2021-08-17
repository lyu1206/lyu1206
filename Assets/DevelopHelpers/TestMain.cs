using Eos.Objects;
using Eos.Ore;
using Eos.Service;
using Eos.Service.AI;
using System.Collections;
using UnityEngine;
//using MessagePack;
//using Eos.Objects;
using Eos.Objects.UI;

public class TestMain : MonoBehaviour
{
    [SerializeField]
    BodyOre _bodyore;
    [SerializeField]
    PVMOre pvmore;
    [SerializeField]
    OreBase _lightore;
    // Start is called before the first frame update
    [SerializeField]
    OreBase _pcfsm;
    [SerializeField]
    OreBase _slimebody;
    [SerializeField]
    MeshOre _weapon;
    [SerializeField]
    OreBase _padui;
    private EosModel CreateNPC(Workspace workspace)
    {
        var npcmodel = ObjectFactory.CreateEosObject<EosModel>() /*new EosModel()*/; npcmodel.Name = $"Slime{0}";
        workspace.AddChild(npcmodel);

        var modelscript = ObjectFactory.CreateEosObject<EosScript>(); modelscript.scriptname = "EngageLogic";modelscript.Name = "EngageLogic";
        npcmodel.AddChild(modelscript);

        modelscript = ObjectFactory.CreateEosObject<EosScript>(); modelscript.scriptname = "HostileNPC"; modelscript.Name = "HostileNPC";
        npcmodel.AddChild(modelscript);



        var pawn = ObjectFactory.CreateEosObject<EosPawnActor>();
        pawn.Layer = LayerMask.NameToLayer("Hostile");
        pawn.Name = EosHumanoid.humanoidroot;
        pawn.Body = _slimebody as BodyOre;
        pawn.BodyOre = new OreReference { Mold = "BodyMolds", OreID = Eos.Assets.Mold.GetMold("BodyMolds").GetOreID(1) };
        npcmodel.AddChild(pawn);

        var pawncollider = ObjectFactory.CreateEosObject<EosCollider>(); pawncollider.Name = "Body"; pawncollider.ColliderType = ColliderType.Capsule;
        pawn.AddChild(pawncollider);
        pawncollider.Collider.Center = new Vector3(0, 5, 0);
        ((eosCapsuleCollider)pawncollider.Collider).Radius = 8;

        var humanoid = ObjectFactory.CreateEosObject<EosHumanoid>(); humanoid.Name = "Humanoid"; humanoid.Level = 1;

        var humanoidfsm = ObjectFactory.CreateEosObject<EosFsm>(); humanoidfsm.Name = "humanoidFSM"; humanoidfsm.FSM = _pcfsm;
        humanoid.AddChild(humanoidfsm);

        npcmodel.AddChild(humanoid);

        return npcmodel;
    }
    IEnumerator Start()
    {

        EosObjectBase solution = ObjectFactory.CreateEosObject<Solution>();solution.Name = "Solution";

        void Save()
        {
            var msgpackData = MessagePack.MessagePackSerializer.Serialize(solution);
            var path = $"{Application.streamingAssetsPath}/Solutions/map001.solution";
            System.IO.File.WriteAllBytes(path, msgpackData);

//            var decodetest = MessagePackSerializer.Deserialize<EosObjectBase>(msgpackData);

            UnityEditor.AssetDatabase.Refresh();
        }


        var aiservice = ObjectFactory.CreateEosObject<AIService>();aiservice.Name = "AIService";
        solution.AddChild(aiservice);

        var workspace = ObjectFactory.CreateEosObject<Workspace>();workspace.Name = "Workspace";
        solution.AddChild(workspace);

        var terrainservice = ObjectFactory.CreateEosObject<TerrainService>();terrainservice.Name = "TerrainService";terrainservice._pvmOre = pvmore;
        solution.AddChild(terrainservice);
        var terrain = ObjectFactory.CreateEosObject<EosTerrain>();terrain.Name = "Terrain";terrain.TerrainOre = new OreReference { Mold = "TerrainMold", OreID = Eos.Assets.Mold.GetMold("TerrainMold").GetOreID(0) };
        terrainservice.AddChild(terrain);


        var playerservice = ObjectFactory.CreateEosObject<Players>();playerservice.Name = "Players";
        solution.AddChild(playerservice);
        /*
        var player = playerservice.FindChild<Player>();
        */
        var starterplayer = ObjectFactory.CreateEosObject<StarterPlayer>();starterplayer.Name = "StarterPlayer";
        solution.AddChild(starterplayer);

        var playermodel = ObjectFactory.CreateEosObject<EosModel>(); playermodel.Name = $"Player - model";
        starterplayer.AddChild(playermodel);

        var modelscript = ObjectFactory.CreateEosObject<EosScript>();modelscript.scriptname = "EngageLogic";modelscript.Name = "EngageLogic";
        playermodel.AddChild(modelscript);


        var pawn = ObjectFactory.CreateEosObject<EosPawnActor>();
        pawn.Name = EosHumanoid.humanoidroot;
        pawn.Body = _bodyore;

        var textmesh = ObjectFactory.CreateEosObject<EosTextMesh>("headname");
        textmesh.Text = "Hello";
        textmesh.LocalPosition = new Vector3(0,30,0);
        pawn.AddChild(textmesh);

        playermodel.AddChild(pawn);
        var pawncollider = ObjectFactory.CreateEosObject<EosCollider>(); pawncollider.ColliderType = ColliderType.Capsule;
        pawn.AddChild(pawncollider);
        ((eosCapsuleCollider)pawncollider.Collider).Radius = 8;

        var humanoid = ObjectFactory.CreateEosObject<EosHumanoid>();humanoid.Name = "Humanoid";humanoid.Level = 1;
        var humanoidfsm = ObjectFactory.CreateEosObject<EosFsm>();humanoidfsm.Name = "FSM";humanoidfsm.FSM = _pcfsm;
        humanoid.AddChild(humanoidfsm);
        var battlescript = ObjectFactory.CreateEosObject<EosScript>();battlescript.scriptname = "NonTargetBattle";battlescript.Name = "NonTargetBattle";
        humanoid.AddChild(battlescript);

        playermodel.AddChild(humanoid);


        var starterpack = ObjectFactory.CreateEosObject<StarterPack>();starterpack.Name = "StarterPack";
        solution.AddChild(starterpack);

        var tool = ObjectFactory.CreateEosObject <EosTool>();tool.Name = "Weapon";
        var weaponmesh = ObjectFactory.CreateEosObject < EosMeshObject>();weaponmesh.Name = "Sword";
        weaponmesh.Mesh = _weapon;
        tool.AddChild(weaponmesh);
        playermodel.AddChild(tool);
        starterpack.AddChild(tool);


        var camera = ObjectFactory.CreateEosObject<EosCamera>(); camera.Name = "Cam";
        workspace.AddChild(camera);
        camera.LocalPosition = new Vector3(-80,106,-80);
        camera.LocalRotation = new Vector3(60,-180,0);

        var light = ObjectFactory.CreateEosObject <EosLight>();light.Name = "Light";
        light.Light = _lightore;
        workspace.AddChild(light);

        var guiservice = ObjectFactory.CreateEosObject<GUIService>();guiservice.Name = "GUIService";
        solution.AddChild(guiservice);
        var uiobj = ObjectFactory.CreateEosObject<EosUIObject>(); uiobj.Name = "PAD";
        uiobj._uisource = _padui;
        guiservice.AddChild(uiobj);
        var script = ObjectFactory.CreateEosObject<EosScript>();script.scriptname = "PadControl";script.Name = "PadControl";
        uiobj.AddChild(script);



        var slimecc = CreateNPC(workspace);


        Save();


        EosPlayer.EosPlayer.Instance.SetSolution(solution as Solution);


//        solution.IterChilds((child) => child.OnCreate(), true);
        ((Solution)solution).StartGame();
       yield return new WaitForEndOfFrame();

        //        playermodel.FindChild<EosPawnActor>().PlayNode("idle");

        var eplayer = EosPlayer.EosPlayer.Instance;
        var ws = eplayer.Solution.Workspace;
        var temppos = eplayer.Solution.Terrain.FindNode("obj_SpawnOut");
        var slime = eplayer.Solution.Workspace.FindChild<EosModel>("Slime0");
        var slimehumanoidroot = slime.FindChild<EosPawnActor>();
        slimehumanoidroot.LocalPosition = temppos.position;

        //var slimeclone = ObjectFactory.CopyObject(slimecc) as EosModel;
        //slimeclone.FindChild<EosHumanoid>().SetPosition(slimeclone.PrimaryActor.LocalPosition - new Vector3(40, 0, 40));
        //slimeclone.Name = "Slime1";
        //workspace.AddChild(slimeclone);


        //        CreateNPC(workspace);

        // equip weapon test



        //eplayer.Scheduler.Schedule(() =>
        //{
        //    Debug.Log("forever update..");
        //});
        //eplayer.Scheduler.ScheduleOnce(() =>
        //{
        //    Debug.Log("EXEcute once");
        //});
        //eplayer.Scheduler.ScheduleDetail(() =>
        //{
        //    Debug.Log("ScheduleDetail 4 time");
        //}, 2f,4);
    }
}
