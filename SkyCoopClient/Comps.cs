using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using SkyCoopServer;
using UnityEngine;

namespace SkyCoop;

public class Comps
{
    public static void RegisterComponents()
    {
        ClassInjector.RegisterTypeInIl2Cpp<UiButtonPressHook>();
        ClassInjector.RegisterTypeInIl2Cpp<UiButtonKeyboardPressSkip>();
        ClassInjector.RegisterTypeInIl2Cpp<UiButtonSettingHook>();
        ClassInjector.RegisterTypeInIl2Cpp<NetworkPlayer>();
        ClassInjector.RegisterTypeInIl2Cpp<OtherPlayerGear>();
    }

    public class UiButtonPressHook : MonoBehaviour
    {
        public int m_CustomId;
        public string m_PanelHandle = "";

        public UiButtonPressHook(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class UiButtonKeyboardPressSkip : MonoBehaviour
    {
        public Il2CppSystem.Collections.Generic.List<EventDelegate> m_Click;
        public Il2CppSystem.Collections.Generic.List<EventDelegate> m_DoubleClick;
        public Il2CppSystem.Collections.Generic.List<EventDelegate> m_DoubleDoubleClick;

        public UiButtonKeyboardPressSkip(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class UiButtonSettingHook : MonoBehaviour
    {
        public GameObject m_Background;

        public UiButtonSettingHook(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class OtherPlayerGear : MonoBehaviour
    {
        public string m_GearName = "";
        public NetworkPlayer.GearHandPose m_HandPose = 0;

        public OtherPlayerGear(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class NetworkPlayer : MonoBehaviour
    {
        public enum Actions
        {
            None = 0,
            Harvesting = 1,
            PistolAim = 2,
            RifleAim = 3,
            Igniting = 4
        }

        public enum GearHandPose
        {
            None = 0,
            Pistol = 1,
            Rifle = 2,
            Lantern = 3,
            GenericHold = 4,
            Matches = 5,
            Bow = 6
        }

        public int m_PlayerID;
        public Vector3 m_Position = Vector3.zero;
        public Quaternion m_Rotation = Quaternion.identity;
        public float m_SecondsBeforeHide = 5f;
        public Animator m_Animator;
        public Vector3 m_LastPosition = Vector3.zero;
        public float m_MinimalSpeed = 0.1f;
        public float m_Smoother = 0.1f;
        public GearHandPose m_GearHandPose = GearHandPose.None;
        public Actions m_Action = Actions.None;

        public List<OtherPlayerGear> m_VisualGears = new();

        public DataStr.PlayerVisualData m_VisualData = new();


        private readonly float s_DeltaMultiplayer = 20;
        private readonly float s_InActiveCooldown = 5f;
        private readonly float s_InterpolationSkipDistance = 15f;

        public NetworkPlayer(IntPtr ptr) : base(ptr)
        {
        }

        private void Update()
        {
            // Cause we no more broadcast all the players position constatly to all the clients.
            // Client side need somekind of failsafe.
            // if client won't get any updates about this player in s_InActiveCooldown,
            // This player going to be deactivated from scene.
            if (m_SecondsBeforeHide > 0)
            {
                m_SecondsBeforeHide -= Time.deltaTime;
                if (m_SecondsBeforeHide <= 0) m_SecondsBeforeHide = 0;
                //gameObject.SetActive(false);
            }

            UpdateAnimations();


            // That way, we can avoid stupid situations when previous position of the objects was too far away
            // would lead to character slide on high speed. This mostly noticable when player loads from Vector3.zero.
            if (Vector3.Distance(transform.position, m_Position) > s_InterpolationSkipDistance)
                transform.position = Vector3.Lerp(transform.position, m_Position, Time.deltaTime * s_DeltaMultiplayer);
            else
                transform.position = m_Position;

            transform.rotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * s_DeltaMultiplayer);
        }

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

        public void SetAcation(int Action)
        {
            m_Action = (Actions)Action;
            if (m_Animator) m_Animator.SetInteger("Action", Action);
        }

        public void DoFire()
        {
            if (m_Animator) m_Animator.SetTrigger("Shoot");
        }

        public void SetGear(string GearName, int GearVariant)
        {
            m_VisualData.m_GearInHands = GearName;
            m_VisualData.m_GearVariant = GearVariant;

            if (string.IsNullOrEmpty(GearName))
            {
                m_GearHandPose = GearHandPose.None;
                if (m_Animator) m_Animator.SetInteger("Gear", (int)m_GearHandPose);
            }

            foreach (var Gear in m_VisualGears)
            {
                if (Gear.m_GearName == GearName)
                {
                    m_GearHandPose = Gear.m_HandPose;
                    if (m_Animator) m_Animator.SetInteger("Gear", (int)m_GearHandPose);
                }

                Gear.gameObject.SetActive(Gear.m_GearName == GearName);
            }
        }

        public void SetCrouching(bool IsCrouching)
        {
            m_VisualData.m_Crouch = IsCrouching;
        }

        public static Transform GetBone(Animator Animator, HumanBodyBones Bone)
        {
            if (Animator.isHuman)
            {
                var T = Animator.GetBoneTransform(Bone);
                if (T == null) Logger.Log(ConsoleColor.Red, Animator.gameObject.name + " does not have " + Bone);

                return T;
            }

            Logger.Log(ConsoleColor.Red,
                "Can't get bone of " + Animator.gameObject.name + ", because this object is not Humanoid type!");
            return null;
        }

        public void LoadEquipment()
        {
            AddPlaceholderHoldingGear(this, "GEAR_Rifle", GearHandPose.Rifle);
            AddPlaceholderHoldingGear(this, "GEAR_Revolver", GearHandPose.Pistol);
            AddPlaceholderHoldingGear(this, "GEAR_Bow");
            AddPlaceholderHoldingGear(this, "GEAR_FlareGun", GearHandPose.Pistol);

            AddPlaceholderHoldingGear(this, "GEAR_Stone");
            AddPlaceholderHoldingGear(this, "GEAR_NoiseMaker");

            AddPlaceholderHoldingGear(this, "GEAR_SprayPaintCanGlyphA");

            AddPlaceholderHoldingGear(this, "GEAR_WoodMatches");
            AddPlaceholderHoldingGear(this, "GEAR_PackMatches");

            AddPlaceholderHoldingGear(this, "GEAR_KeroseneLampB", GearHandPose.Lantern);
            AddPlaceholderHoldingGear(this, "GEAR_BlueFlare");
            AddPlaceholderHoldingGear(this, "GEAR_FlareA");
            AddPlaceholderHoldingGear(this, "GEAR_Torch");

            AddPlaceholderHoldingGear(this, "GEAR_EmergencyStim");


            //ModMain.AddPlaceholderHoldingGear(this, "DarkWalker_Death", false);
            //ModMain.AddPlaceholderHoldingGear(this, "GEAR_Shovel", false);
            //ModMain.AddPlaceholderHoldingGear(this, "GEAR_ClothSheet", false);
            //ModMain.AddPlaceholderHoldingGear(this, "GEAR_FireAxe", false);
            //ModMain.AddPlaceholderHoldingGear(this, "CORPSE_Human_Frozen4", false);
        }

        public static void AddPlaceholderHoldingGear(NetworkPlayer Player, string GearName,
            GearHandPose HandPose = GearHandPose.None, bool Bogus = true)
        {
            var RightHand = GetBone(Player.m_Animator, HumanBodyBones.RightHand);
            if (RightHand)
            {
                GameObject Gear;
                if (Bogus)
                {
                    Gear = AssetManager.CreateBogusGear(GearName);
                    if (Gear)
                    {
                        Gear.transform.SetParent(RightHand);
                        Gear.transform.localPosition = Vector3.zero;
                    }

                    Gear.SetActive(false);
                }
                else
                {
                    var Reference = AssetManager.GetAssetFromGame<GameObject>(GearName);
                    Gear = Instantiate(Reference);
                    if (Gear)
                    {
                        Gear.transform.SetParent(RightHand);
                        Gear.transform.localPosition = Vector3.zero;
                    }

                    Gear.SetActive(true);
                }

                AddVisualGear(GearName, Gear, HandPose, Player);
            }
        }

        public static void AddVisualGear(string GearName, GameObject Obj, GearHandPose HandPose, NetworkPlayer Player)
        {
            var Gear = Obj.AddComponent<OtherPlayerGear>();
            Gear.m_GearName = GearName;
            Gear.m_HandPose = HandPose;
            Player.m_VisualGears.Add(Gear);
        }

        public void UpdateAnimations()
        {
            var Speed = (gameObject.transform.position - m_LastPosition) / Time.deltaTime;
            Speed.y = 0;
            var Direction = transform.InverseTransformDirection(Speed);
            m_LastPosition = gameObject.transform.position;

            if (m_Animator)
            {
                var AnimatorSpeed = Speed.magnitude;


                if (!m_VisualData.m_Crouch) AnimatorSpeed = AnimatorSpeed / 4;

                m_Animator.SetFloat("Speed", AnimatorSpeed);

                m_Animator.SetInteger("Gear", (int)m_GearHandPose);
                m_Animator.SetInteger("Action", (int)m_Action);

                var PreviousDirectionX = m_Animator.GetFloat("DirectionX");
                var PreviousDirectionY = m_Animator.GetFloat("DirectionY");
                m_Animator.SetBool("IsMoving", Direction.magnitude > m_MinimalSpeed);
                m_Animator.SetBool("Crouch", m_VisualData.m_Crouch);
                m_Animator.SetFloat("DirectionX",
                    Mathf.Lerp(PreviousDirectionX, Mathf.Clamp(Direction.x, -1, 1), m_Smoother));
                m_Animator.SetFloat("DirectionY",
                    Mathf.Lerp(PreviousDirectionY, Mathf.Clamp(Direction.z, -1, 1), m_Smoother));
            }
        }
    }
}