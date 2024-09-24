
using UnityEngine;
using Il2CppCollections = Il2CppSystem.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Il2CppRewired;

namespace SkyCoop
{
    public class Comps
    {
        public static void RegisterComponents()
        {
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonPressHook>();
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonKeyboardPressSkip>();
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonSettingHook>();
            ClassInjector.RegisterTypeInIl2Cpp<NetworkPlayer>();
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

        public class UiButtonSettingHook : MonoBehaviour
        {
            public UiButtonSettingHook(IntPtr ptr) : base(ptr) { }
            public GameObject m_Background = null;
        }

        public class NetworkPlayer : MonoBehaviour
        {
            public NetworkPlayer(IntPtr ptr) : base(ptr) { }
            public int m_PlayerID = 0;
            public Vector3 m_Position = Vector3.zero;
            public Quaternion m_Rotation = Quaternion.identity;
            public float m_SecondsBeforeHide = 5f;
            float s_DeltaMultiplayer = 20;
            float s_InterpolationSkipDistance = 15f;
            float s_InActiveCooldown = 5f;

            public void SetTransform(Vector3 position, Quaternion rotation)
            {
                m_Position = position;
                m_Rotation = rotation;
                KeepVisible();
            }

            public void SetPosition(Vector3 position)
            {
                m_Position = position;
                KeepVisible();
            }

            public void SetRotation(Quaternion rotation)
            {
                m_Rotation = rotation;
                KeepVisible();
            }

            public void KeepVisible()
            {
                m_SecondsBeforeHide = s_InActiveCooldown;
                gameObject.SetActive(true);
            }

            void Update()
            {
                // Cause we no more broadcast all the players position constatly to all the clients.
                // Client side need somekind of failsafe.
                // if client won't get any updates about this player in s_InActiveCooldown,
                // This player going to be deactivated from scene.
                if (m_SecondsBeforeHide > 0)
                {
                    m_SecondsBeforeHide -= Time.deltaTime;
                    if(m_SecondsBeforeHide <= 0)
                    {
                        m_SecondsBeforeHide = 0;
                        gameObject.SetActive(false);
                    }
                }

                
                
                
                // That way, we can avoid stupid situations when previous position of the objects was too far away
                // would lead to character slide on high speed. This mostly noticable when player loads from Vector3.zero.
                if (Vector3.Distance(transform.position, m_Position) > s_InterpolationSkipDistance)
                {
                    transform.position = Vector3.Lerp(transform.position, m_Position, Time.deltaTime * s_DeltaMultiplayer);
                } else
                {
                    transform.position = m_Position;
                }

                transform.rotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * s_DeltaMultiplayer);
            }
        }
    }
}
