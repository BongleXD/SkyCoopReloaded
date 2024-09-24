using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoop
{
    internal class AssetManager
    {
        public static string s_MainBundlePath = "Mods\\multiplayerstuff.unity3d";
        public static AssetBundle s_MainBundle = null;

        public static void PreloadMainBundle()
        {
            if(s_MainBundle == null)
            {
                s_MainBundle = AssetBundle.LoadFromFile(s_MainBundlePath);
                if (s_MainBundle == null)
                {
                    Logger.Log(ConsoleColor.Red,"Have problems with loading main asset bundle!");
                } else
                {
                    Logger.Log(ConsoleColor.Blue, "Main Asset Bundle is loaded.");
                }
            }
        }

        // This using casting, because we can use it to load textures, audio clips and etc, not just prefabs.
        public static T GetAssetFromBundle<T>(string AssetName) where T : UnityEngine.Object
        {
            if (s_MainBundle == null)
            {
                Logger.Log(ConsoleColor.Red, "Can't load "+AssetName+" because bundle is missing!");
                return null;
            }
            return s_MainBundle.LoadAsset<T>(AssetName);
        }
    }
}
