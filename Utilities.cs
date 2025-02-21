using System.Linq;
using System.Reflection;
using UnityEngine;
using Ship.Interface.Settings;
using UI.BuildMenu;

namespace Radio;

public static class Utilities
{
    /// <summary>
    /// Adds a custom part to the game's menus
    /// Method should be called after loading the main scene
    /// </summary>
    public static void AddPart(PartSettings part)
    {
        BuildMenu buildMenu = Object.FindFirstObjectByType<BuildMenu>();
        BuildPanel partsPanel = buildMenu.GetComponentsInChildren<BuildPanel>().FirstOrDefault(p => p.gameObject.name == "PartsPanel");

        FieldInfo rowsField = partsPanel.GetType().GetField("_rows", BindingFlags.NonPublic | BindingFlags.Instance);
        BuildRow[] originalRows = (BuildRow[])rowsField.GetValue(partsPanel);

        BuildRow row = originalRows.FirstOrDefault();
        row.parts.Add(part);
    }

    // <summary>
    /// Checks if a Part with the ID already exists
    /// </summary>
    public static bool IsPartIDTaken(ushort id)
    {
        return Resources.FindObjectsOfTypeAll<PartSettings>().Any(p => p.id == id);
    }

    public static void MigrateMaterials(GameObject gameObject)
    {
        var sdLit = Shader.Find("Shader Graphs/PlanetObjectDefaultLighting");

        var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        var materials = renderers.SelectMany(r => r.materials);

        foreach (var material in materials)
        {
            bool isLit = material.shader.name == "PlanetObjectDefaultLighting";

            if (isLit) material.shader = sdLit;
        }
    }
}
