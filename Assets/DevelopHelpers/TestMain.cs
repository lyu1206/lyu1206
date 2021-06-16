using Eos.Objects;
using Eos.Ore;
using Eos.Service;
using Eos.Service.AI;
using System.Collections;
using UnityEngine;
using MessagePack;

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

        var modelscript = new EosScript { scriptname = "EngageLogic" };
        npcmodel.AddChild(modelscript);

        modelscript = new EosScript { scriptname = "HostileNPC" };
        npcmodel.AddChild(modelscript);



        var pawn = new EosPawnActor();
        pawn.Layer = LayerMask.NameToLayer("Hostile");
        pawn.Name = EosHumanoid.humanoidroot;
        pawn.Body = _slimebody as BodyOre;
        npcmodel.AddChild(pawn);

        var pawncollider = new EosCollider(); pawncollider.ColliderType = ColliderType.Capsule;
        pawn.AddChild(pawncollider);
        pawncollider.Collider.Center = new Vector3(0, 5, 0);
        ((eosCapsuleCollider)pawncollider.Collider).Radius = 8;

        var humanoid = new EosHumanoid(); humanoid.Name = "Humanoid"; humanoid.Level = 1;

        var humanoidfsm = new EosFsm(); humanoidfsm.FSMore = _pcfsm;
        humanoid.AddChild(humanoidfsm);

        npcmodel.AddChild(humanoid);

        return npcmodel;
    }
    IEnumerator Start()
    {

        var solution = new Solution();

        void Save()
        {
            var msgpackData = MessagePack.MessagePackSerializer.Serialize(solution);
            var path = $"{Application.streamingAssetsPath}/Solutions/map001.solution";
            System.IO.File.WriteAllBytes(path, msgpackData);
            UnityEditor.AssetDatabase.Refresh();
        }


        var aiservice = new AIService();
        solution.AddChild(aiservice);

        var workspace = new Workspace();
        solution.AddChild(workspace);
        
        var terrainservice = new TerrainService{_pvmOre = pvmore};
        solution.AddChild(terrainservice);


        var playerservice = new Players();
        solution.AddChild(playerservice);
        playerservice.OnConnectPlayer(1);
        var player = playerservice.FindChild<Player>();

        var playermodel = new EosModel();playermodel.Name = $"{player.Name} - model";
        player.AddChild(playermodel);

        var modelscript = new EosScript{ scriptname = "EngageLogic" };
        playermodel.AddChild(modelscript);


        var pawn = new EosPawnActor();
        pawn.Name = EosHumanoid.humanoidroot;
        pawn.Body = _bodyore;

        var textmesh = new EosTextMesh("headname");
        pawn.AddChild(textmesh);

        playermodel.AddChild(pawn);
        var pawncollider = new EosCollider();pawncollider.ColliderType = ColliderType.Capsule;
        pawn.AddChild(pawncollider);
        ((eosCapsuleCollider)pawncollider.Collider).Radius = 8;

        var humanoid = new EosHumanoid();humanoid.Name = "Humanoid";humanoid.Level = 1;
        var humanoidfsm = new EosFsm();humanoidfsm.FSMore = _pcfsm;
        humanoid.AddChild(humanoidfsm);
        var battlescript = new EosScript { scriptname = "NonTargetBattle" };
        humanoid.AddChild(battlescript);

        playermodel.AddChild(humanoid);



        var camera = new EosCamera();camera.Name = "Cam";
        workspace.AddChild(camera);
        camera.LocalPosition = new Vector3(0,60,100);
        camera.LocalRotation = new Vector3(26,180,0);

        var light = new EosLight();light.Name = "Light";
        light.Light = _lightore;
        workspace.AddChild(light);

        var guiservice = new GUIService();
        solution.AddChild(guiservice);
        var uiobj = new EosUIObject(); uiobj.Name = "PAD";
        uiobj._uisource = _padui;
        guiservice.AddChild(uiobj);
        var script = new EosScript { scriptname = "PadControl" };
        uiobj.AddChild(script);



        var slimecc = CreateNPC(workspace);


        EosPlayer.EosPlayer.Instance.SetSolution(solution);
        solution.StartGame();

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
        var tool = new EosTool();tool.Name = "Weapon";
        var weaponmesh = new EosMeshObject();weaponmesh.Name = "Sword";
        weaponmesh.MeshOre = _weapon;
        weaponmesh.Activate(true);
        tool.AddChild(weaponmesh);
        playermodel.AddChild(tool);



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
