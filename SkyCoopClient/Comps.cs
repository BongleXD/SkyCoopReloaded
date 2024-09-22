
using UnityEngine;
using Il2CppCollections = Il2CppSystem.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
namespace SkyCoop
{
    public class Comps
    {
        public static void RegisterComponents()
        {
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonPressHook>();
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonKeyboardPressSkip>();
        }

        public class UiButtonPressHook : MonoBehaviour
        {
            public UiButtonPressHook(IntPtr ptr) : base(ptr) { }
            public int m_CustomId = 0;
            public string m_PanelHandle = "";
        }
        public class UiButtonKeyboardPressSkip : MonoBehaviour
        {
            public UiButtonKeyboardPressSkip(IntPtr ptr) : base(ptr) { }
            public Il2CppSystem.Collections.Generic.List<Il2Cpp.EventDelegate> m_Click;
            public Il2CppSystem.Collections.Generic.List<Il2Cpp.EventDelegate> m_DoubleClick;
            public Il2CppSystem.Collections.Generic.List<Il2Cpp.EventDelegate> m_DoubleDoubleClick;
        }
    }
}
