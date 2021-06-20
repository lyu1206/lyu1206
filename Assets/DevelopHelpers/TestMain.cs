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
        var npcmodel = new EosModel(); npcmodel.Name = $"Slime{0}";
        workspace.AddChild(npcmodel);

        var modelscript = new EosScript { scriptname = "EngageLogic",Name = "EngageLogic" };
        npcmodel.AddChild(modelscript);

        modelscript = new EosScript { scriptname = "HostileNPC" , Name = "HostileNPC" };
        npcmodel.AddChild(modelscript);



        var pawn = new EosPawnActor();
        pawn.Layer = LayerMask.NameToLayer("Hostile");
        pawn.Name = EosHumanoid.humanoidroot;
        pawn.Body = _slimebody as BodyOre;
        npcmodel.AddChild(pawn);

        var pawncollider = new EosCollider {Name = "Body" }; pawncollider.ColliderType = ColliderType.Capsule;
        pawn.AddChild(pawncollider);
        pawncollider.Collider.Center = new Vector3(0, 5, 0);
        ((eosCapsuleCollider)pawncollider.Collider).Radius = 8;

        var humanoid = new EosHumanoid(); humanoid.Name = "Humanoid"; humanoid.Level = 1;

        var humanoidfsm = new EosFsm {Name = "humanoidFSM" }; humanoidfsm.FSM = _pcfsm;
        humanoid.AddChild(humanoidfsm);

        npcmodel.AddChild(humanoid);

        return npcmodel;
    }
    IEnumerator Start()
    {

         EosObjectBase solution = new Solution {Name = "Solution" };

        void Save()
        {
            var msgpackData = MessagePack.MessagePackSerializer.Serialize(solution);
            var path = $"{Application.streamingAssetsPath}/Solutions/map001.solution";
            System.IO.File.WriteAllBytes(path, msgpackData);

//            var decodetest = MessagePackSerializer.Deserialize<EosObjectBase>(msgpackData);

            UnityEditor.AssetDatabase.Refresh();
        }


        var aiservice = new AIService {Name = "AIService" };
        solution.AddChild(aiservice);

        var workspace = new Workspace { Name = "Workspace"};
        solution.AddChild(workspace);
        
        var terrainservice = new TerrainService{Name = "TerrainService" , _pvmOre = pvmore};
        solution.AddChild(terrainservice);

        
        var playerservice = new Players {Name = "Players" };
        solution.AddChild(playerservice);
        /*
        var player = playerservice.FindChild<Player>();
        */
        var starterplayer = new StarterPlayer { Name = "StarterPlayer" };
        solution.AddChild(starterplayer);

        var playermodel = new EosModel();playermodel.Name = $"Player - model";
        starterplayer.AddChild(playermodel);

        var modelscript = new EosScript{ scriptname = "EngageLogic",Name = "EngageLogic" };
        playermodel.AddChild(modelscript);


        var pawn = new EosPawnActor();
        pawn.Name = EosHumanoid.humanoidroot;
        pawn.Body = _bodyore;

        var textmesh = new EosTextMesh("headname");
        textmesh.Text = "Hello";
        pawn.AddChild(textmesh);

        playermodel.AddChild(pawn);
        var pawncollider = new EosCollider();pawncollider.ColliderType = ColliderType.Capsule;
        pawn.AddChild(pawncollider);
        ((eosCapsuleCollider)pawncollider.Collider).Radius = 8;

        var humanoid = new EosHumanoid();humanoid.Name = "Humanoid";humanoid.Level = 1;
        var humanoidfsm = new EosFsm { Name = "FSM"};humanoidfsm.FSM = _pcfsm;
        humanoid.AddChild(humanoidfsm);
        var battlescript = new EosScript { scriptname = "NonTargetBattle",Name = "NonTargetBattle" };
        humanoid.AddChild(battlescript);

        playermodel.AddChild(humanoid);


        var starterpack = new StarterPack {Name = "StarterPack" };
        solution.AddChild(starterpack);

        var tool = new EosTool();tool.Name = "Weapon";
        var weaponmesh = new EosMeshObject();weaponmesh.Name = "Sword";
        weaponmesh.Mesh = _weapon;
        tool.AddChild(weaponmesh);
        playermodel.AddChild(tool);
        starterpack.AddChild(tool);


        var camera = new EosCamera();camera.Name = "Cam";
        workspace.AddChild(camera);
        camera.LocalPosition = new Vector3(0,60,100);
        camera.LocalRotation = new Vector3(26,180,0);

        var light = new EosLight();light.Name = "Light";
        light.Light = _lightore;
        workspace.AddChild(light);

        var guiservice = new GUIService {Name = "GUIService" };
        solution.AddChild(guiservice);
        var uiobj = new EosUIObject(); uiobj.Name = "PAD";
        uiobj._uisource = _padui;
        guiservice.AddChild(uiobj);
        var script = new EosScript { scriptname = "PadControl",Name = "PadControl" };
        uiobj.AddChild(script);



        var slimecc = CreateNPC(workspace);


        Save();


        EosPlayer.EosPlayer.Instance.SetSolution(solution as Solution);


        solution.IterChilds((child) => child.OnCreate(), true);
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
