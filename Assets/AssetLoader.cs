using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Radio.Assets;

/// <summary>
/// Loads Assets in an Assembly for later use
/// (Assumes every EmbeddedResource is an AssetBundle)
/// </summary>
public class AssetLoader
{
    private readonly Assembly _assembly;
    private readonly MelonLogger.Instance? _logger;
    private readonly Dictionary<string, Object?> _assets;


    /// <summary> Creates new AssetLoader that wont utilise logging </summary>
    /// <param name="assembly"> The Assembly the Assets' AssetBundles(as "EmbeddedResource"s) are located in </param>
    /// <param name="assetNames"> The Names of the Assets that should be loaded </param>
    /// <remarks> Usage before OnLateInitializeMelon may cause Native Error! </remarks>
    public AssetLoader(Assembly assembly, string[] assetNames)
    {
        _assembly = assembly;
        _assets = [];

        Initialize(assetNames);
    }

    /// <summary> Creates new AssetLoader and logs the loading results </summary>
    /// <param name="assembly"> The Assembly the Assets' AssetBundles(as "EmbeddedResource"s) are located in </param>
    /// <param name="logger"> The Logger that should be used </param>;
    /// <param name="assetNames"> The Names of the Assets that should be loaded </param>
    /// <remarks> Usage before OnLateInitializeMelon may cause Native Error! </remarks>
    public AssetLoader(Assembly assembly, MelonLogger.Instance logger, string[] assetNames)
    {
        _assembly = assembly;
        _logger = logger;
        _assets = [];

        Initialize(assetNames);
    }


    /// <summary> Gets a previously loaded Asset by Name </summary>
    /// <param name="name"> The Name of the Asset </param>
    public Object? GetAsset(string name)
    {
        return _assets[name];
    }

    /// <summary> Gets a previously loaded Asset by Name and casts it </summary>
    /// <param name="name"> The Name of the Asset </param>
    public T? GetAsset<T>(string name) where T : Object
    {
        return (T?)GetAsset(name);
    }


    private void Initialize(string[] assetNames)
    {
        foreach (var assetName in assetNames)
        {
            _assets.Add(assetName, null);
        }

        try
        {
            LoadAndSetAssets();
        }
        catch (Exception exception)
        {
            _logger?.Error("Failed loading assets : " + exception);
        }

        foreach (var assetKey in _assets.Keys)
        {
            if (GetAsset(assetKey) == null)
            {
                _logger?.Error("Didn't load : " + assetKey);
            }
        }
    }

    private void LoadAndSetAssets()
    {
        var resourceNames = _assembly.GetManifestResourceNames();
        _logger?.Msg("Loading resources...");

        foreach (var name in resourceNames)
        {
            _logger?.Msg($"Processing resource: {name}");

            Stream stream = _assembly.GetManifestResourceStream(name);
            if (stream == null)
            {
                _logger?.Error($"Resource stream is null for: {name}");
                continue;
            }

            try
            {
                AssetBundle assetBundle = AssetBundle.LoadFromStream(stream);
                if (assetBundle == null)
                {
                    _logger?.Error($"AssetBundle could not be loaded from: {name}");
                    continue;
                }

                Object[] assets = assetBundle.LoadAllAssets();
                SetMatchingAssets(assets);
            }
            catch (Exception e)
            {
                _logger?.Error($"Error loading AssetBundle from resource: {name}. Exception: {e}");
            }
        }
    }

    private void SetMatchingAssets(Object[] assets)
    {
        foreach (var asset in assets)
        {
            var name = asset.name;

            if (!_assets.Keys.Contains(name)) continue;
            if (_assets[name] != null) continue;

            _assets[name] = asset;
            _logger?.Msg($"Successfully loaded : {name} ({asset.GetType()})");
        }
    }
}
