﻿using Il2CppInterop.Runtime;
using Il2CppSystem.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace SkyCoop;

internal class AssetManager
{
    public static string s_MainBundlePath = "Mods\\skycoop";
    public static AssetBundle s_MainBundle;

    public static void PreloadMainBundle()
    {
        if (s_MainBundle == null)
        {
            s_MainBundle = AssetBundle.LoadFromFile(s_MainBundlePath);
            if (s_MainBundle == null)
                Logger.Log(ConsoleColor.Red, "Have problems with loading main asset bundle!");
            else
                Logger.Log(ConsoleColor.Blue, "Main Asset Bundle is loaded.");
        }
    }

    public static T GetAssetFromGame<T>(string AssetName) where T : Object
    {
        var Asset = Addressables.LoadAssetAsync<T>(AssetName).WaitForCompletion();
        if (Asset == null)
        {
            Logger.Log(ConsoleColor.Red, "Can't load " + AssetName + " from game assets!");
            Logger.Log(ConsoleColor.DarkMagenta, "Fine...lets try old way");
            Asset = GetAssetFromResources_OLD<T>(AssetName);
            if (Asset == null) Logger.Log(ConsoleColor.DarkMagenta, "Na, bogus.");
        }

        return Asset;
    }

    public static GameObject CreateBogusGear(string GearName)
    {
        var Prefab = GetAssetFromGame<GameObject>(GearName);
        if (Prefab)
        {
            var GearObject = Object.Instantiate(Prefab);
            if (GearObject)
            {
                foreach (var Com in GearObject.GetComponents<Component>())
                {
                    var ComName = Com.GetIl2CppType().Name;
                    if (ComName != Il2CppType.Of<BoxCollider>().Name
                        && ComName != Il2CppType.Of<SphereCollider>().Name
                        && ComName != Il2CppType.Of<CapsuleCollider>().Name
                        && ComName != Il2CppType.Of<MeshCollider>().Name
                        && ComName != Il2CppType.Of<PhysicMaterial>().Name
                        && ComName != Il2CppType.Of<MeshFilter>().Name
                        && ComName != Il2CppType.Of<LODGroup>().Name
                        && ComName != Il2CppType.Of<Transform>().Name
                        && ComName != Il2CppType.Of<Rigidbody>().Name
                        && ComName != Il2CppType.Of<MeshRenderer>().Name
                        && ComName != Il2CppType.Of<SkinnedMeshRenderer>().Name)
                        Object.Destroy(Com);
                }

                return GearObject;
            }

            Logger.Log(ConsoleColor.Red, "Can't instantiate " + Prefab.name);
        }

        return null;
    }

    public static T GetAssetFromResources_OLD<T>(string AssetName) where T : Object
    {
        var Asset = Resources.Load(AssetName);
        if (Asset) return Resources.Load(AssetName).Cast<T>();
        return null;
    }

    // This using casting, because we can use it to load textures, audio clips and etc, not just prefabs.
    public static T GetAssetFromBundle<T>(string AssetName) where T : Object
    {
        if (s_MainBundle == null)
        {
            Logger.Log(ConsoleColor.Red, "Can't load " + AssetName + " because bundle is missing!");
            return null;
        }

        return s_MainBundle.LoadAsset<T>(AssetName);
    }

    public static void DumpAddressablesContent()
    {
        foreach (var item in Addressables.ResourceLocators.ToList())
        foreach (var key in item.Keys.ToList())
            Logger.Log(ConsoleColor.Magenta, "[Addressables][LocatorId=" + item.LocatorId + "] " + key.ToString());
    }

    public static void DumpPrefabsList()
    {
        foreach (var item in Resources.LoadAll("")) Logger.Log(ConsoleColor.Magenta, "[Resources] " + item.name);
    }
}