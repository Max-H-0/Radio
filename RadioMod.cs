using System;
using System.Linq;
using MelonLoader;
using UnityEngine;
using Ship.Interface.Settings;
using Radio.Assets;
using Ship.Interface.Model.Parts;
using Ship.Parts.Common;
using Ship.Interface.Model;
using Ship.Interface.Model.Parts.StateTypes;

using Object = UnityEngine.Object;

namespace Radio;

public class RadioMod : MelonMod
{
    public static MelonPreferences_Category PreferenceCategory { get; set; } = MelonPreferences.CreateCategory("RadioMod");
    public static MelonPreferences_Entry<ushort> PartID { get; set; } = PreferenceCategory.CreateEntry<ushort>("PartID", 0b_10000000_00000001);
 

    public static AssetLoader? AssetLoader { get; set; }
    public static EventHandler? OnCreatedAssetLoader;


    struct Keys
    {
        public const string RadioPrefab = "radio-prefab";
        public const string FieryEnd = "fiery-end";
    }


    public override void OnLateInitializeMelon()
    {
        AssetLoader = new(MelonAssembly.Assembly, LoggerInstance,
        [
            Keys.RadioPrefab,
            Keys.FieryEnd
        ]);
        OnCreatedAssetLoader?.Invoke(this, EventArgs.Empty);
    }


    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (buildIndex != 1) return;
        if (Utilities.IsPartIDTaken(PartID.Value))
        {
            LoggerInstance.Warning($"Part ID({PartID.Value}) is already taken!");
            return;
        }

        CreatePart();
    }

    public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
    {
        if (buildIndex != 1) return;

        foreach (var item in Object.FindObjectsOfType<PartSettings>().Where(p => p.id == PartID.Value).ToArray())
        {
            Object.Destroy(item);
        }
    }


    private void CreatePart()
    {
        PartSettings part = ScriptableObject.CreateInstance<PartSettings>();
        part.name = "Radio";
        part.fullLabel = "Radio";
        part.description = "Speaker go brr";
        part.internalStateType = PartInternalStateType.Lever;
        part.size = Vector3.one;
        part.thumbnailTex = Texture2D.blackTexture;
        part.snappingStyle = SnappingStyle.PreciseOnFloor;
        part.id = PartID.Value;
        part.buildingCost = [];
        part.part = CreateRadio();

        Utilities.AddPart(part);
    }


    private GameObject CreateRadio()
    {
        var prefab = AssetLoader?.GetAsset<GameObject>(Keys.RadioPrefab);
        if (prefab is not GameObject radioObject) throw new Exception("Couldn't get Radio Prefab!");
        radioObject = Object.Instantiate(radioObject);

        Utilities.MigrateMaterials(radioObject);
        radioObject.name = "Radio"; // !! DO NOT CHANGE NAME !!  Important for interaction outline 

        Vector3 scale = Vector3.one * 0.5f;
        Vector3 offset = Vector3.up * (scale.y / 2);


        GameObject part = new("Radio Part");

        var bounds = part.AddComponent<ShipPartBounds>();
        bounds.bounds = scale;
        bounds.center = offset;


        GameObject visuals = new("Visuals");
        visuals.AddComponent<RadioVisuals>();
        visuals.transform.parent = part.transform;


        GameObject interactions = new("Interactions");
        interactions.AddComponent<DefaultShipPartInteractions>();
        interactions.transform.parent = part.transform;
        interactions.layer = LayerMask.NameToLayer("Interactable");
        interactions.tag = "Interactable";


        GameObject radioInteractions = new("Radio"); // !! DO NOT CHANGE NAME !!  Important for interaction outline 
        radioInteractions.AddComponent<RadioInteraction>();
        radioInteractions.transform.parent = interactions.transform;
        radioInteractions.layer = interactions.layer;
        radioInteractions.tag = interactions.tag;

        var boxCollider = radioInteractions.AddComponent<BoxCollider>();
        boxCollider.center = offset;
        boxCollider.size = 2 * scale;


        radioObject.transform.parent = visuals.transform;
        radioObject.transform.localPosition = offset;
        radioObject.transform.localRotation = Quaternion.identity;
        

        part.SetActive(false);

        return part;
    }
}
