using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

namespace SkyCoop
{
    public class SanityManager
    {
        public static bool m_CanSeeSanity = false;
        public static GameObject m_CurrentRefMan = null;
        public static bool m_SeenRefMan = false;
        public static bool m_Insanity = false;

        public static float m_CurrentSanity = 300;
        public static float m_PreviousSanity = 300;
        public static float m_MaxSanity = 300;
        public static bool m_GoingTriggerNightmare = false;

        public static float m_DaySanityRegeneration = 0.2f;
        public static float m_NightSanityDecrease = 0.23f;
        public static float m_NightOutDoorsSanityDecrease = 0.4f;
        public static float m_ScarySceneSanityDecrease = 0.23f;
        public static float m_FullDarknessSanityDecrease = 0.15f;
        public static float m_SleepingSanityRegereation = 5f;
        public static float m_DayRegenerationCap = 180;
        public static string m_TransitionDataBackup = "";


        public static string m_PreNightmareScene = "";
        public static string m_PreNightmareSceneWithGUID = "";
        public static Vector3 m_PreNightmarePosition = Vector3.zero;
        
        public static bool m_Debug = false;
        public static List<string> m_ScaryScenes = new List<string>()
        {
            "DamTransitionZone",
            "Dam",
            "BearCave",
        };

        public static void Update()
        {
            CheckSeeingRefMan();
        }

        public static void ProcessedSleep(float Hours)
        {
            float RestBonus = m_SleepingSanityRegereation * Hours;
            AddSanity(RestBonus);
            //MelonLogger.Msg("[SanityManager] Restored "+ RestBonus+" after sleeping");
        }
        public static void StartSleeping()
        {
            if(IsNight() && m_CurrentSanity < 10)
            {
                //m_GoingTriggerNightmare = true;
            }
        }

        public static void AddSanity(float Value)
        {
            m_CurrentSanity += Value;
            if(m_CurrentSanity < 0)
            {
                m_CurrentSanity = 0;
            }

            float MaxSanity = m_MaxSanity;

            if (m_Insanity)
            {
                MaxSanity = m_MaxSanity / 2;
            }

            if (m_CurrentSanity > MaxSanity)
            {
                m_CurrentSanity = MaxSanity;
            }
        }

        public static void DayBenefit()
        {
            if (!IsScaryScene() && !IsNight())
            {
                if(m_CurrentSanity < m_DayRegenerationCap)
                {
                    AddSanity(m_DaySanityRegeneration);
                }
            }
        }
        public static void NightAffection()
        {
            if (IsNight())
            {
                if (InDoors())
                {
                    AddSanity(-m_NightSanityDecrease);
                } else
                {
                    AddSanity(-m_NightOutDoorsSanityDecrease);
                }
            }
        }

        public static void ScaryPlaceAffection()
        {
            if (IsScaryScene())
            {
                AddSanity(-m_ScarySceneSanityDecrease);
            }
        }

        public static void FullDarknessAffection()
        {
            if (IsFullDarkness())
            {
                AddSanity(-m_FullDarknessSanityDecrease);
            }
        }

        public static float GetRate()
        {
            return m_CurrentSanity - m_PreviousSanity;
        }

        public static void EveryInGameMinute()
        {
            m_PreviousSanity = m_CurrentSanity;
            DayBenefit();
            NightAffection();
            ScaryPlaceAffection();
            FullDarknessAffection();
            if (m_Debug)
            {
                MelonLogger.Msg("[SanityManager] Sanity "+ m_CurrentSanity);
            }

            if (m_CurrentSanity < 30)
            {
                MaySpawnRefMan();
            }
            if(m_CurrentSanity < 50)
            {
                MayHearingHallucination();
            }

            if(m_GoingTriggerNightmare)
            {
                if(m_CurrentSanity > 10)
                {
                    m_GoingTriggerNightmare = false;
                } else if(GameManager.GetRestComponent() && GameManager.GetRestComponent().m_NumSecondsSleeping > 50)
                {
                    m_GoingTriggerNightmare = false;
                    TriggerNightmare();
                }
            }
            if(MyMod.level_name == "BearCave")
            {
                if(m_CurrentSanity > 50)
                {
                    FinishNightmare();
                }
            }
        }

        public static bool IsSleeping()
        {
            return GameManager.GetRestComponent() && GameManager.GetRestComponent().IsSleeping();
        }

        public static bool IsNight()
        {
            return GameManager.m_TimeOfDay != null && GameManager.m_TimeOfDay.IsNight();
        }

        public static bool InDoors()
        {
            return GameManager.GetWeatherComponent() && GameManager.GetWeatherComponent().IsIndoorScene();
        }
        public static bool IsFullDarkness()
        {
            return GameManager.GetWeatherComponent() && GameManager.GetWeatherComponent().IsTooDarkForAction(ActionsToBlock.Reading);
        }

        public static bool IsScaryScene()
        {
            foreach (string scene in m_ScaryScenes)
            {
                if(scene == MyMod.level_name)
                {
                    return true;
                }
            }
            return false;
        }

        public static void CheckSeeingRefMan()
        {
            if (m_CurrentRefMan != null)
            {
                if (m_CurrentRefMan.GetComponent<MeshRenderer>().isVisible && !m_SeenRefMan)
                {
                    m_SeenRefMan = true;
                    PlayerDamageEvent.SpawnAfflictionEvent("GAMEPLAY_AfflictionFearAfraid", "GAMEPLAY_Affliction", "ico_injury_eventEntity2", InterfaceManager.m_FirstAidRedColor);
                    GameObject emitterFromGameObject = GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject());
                    AkSoundEngine.StopPlayingID(AkSoundEngine.PostEvent("Play_FearAffliction", emitterFromGameObject, 4, null, null), 40000);
                    m_CurrentSanity = 0;
                }
            } else
            {
                m_SeenRefMan = false;
            }
        }

        public static void MaySpawnRefMan(float chanceOverride = 0.005f)
        {
            System.Random RNG = new System.Random();
            if (GameManager.m_TimeOfDay != null && GameManager.m_TimeOfDay.IsNight() && RNG.NextDouble() < chanceOverride)
            {
                SpawnRefMan();
            }
        }

        public static void MayHearingHallucination()
        {
            System.Random RNG = new System.Random();
            List<string> Sounds = new List<string>();
            float Chance = 0.07f;
            if (MyMod.level_name == "Dam" || MyMod.level_name == "DamTransitionZone")
            {
                Sounds.Add("PLAY_WOLFATTACK");
                Sounds.Add("PLAY_TIMBERWOLFATTACK");
                Sounds.Add("PLAY_TIMBERWOLFATTACKBITE");
                Sounds.Add("PLAY_SNDMECHMETALVAULT2OPEN");
            } else
            {
                Chance = 0.05f;
                Sounds.Add("PLAY_SURVIVORYOUNGMALEATTACKEDWOLF");
                Sounds.Add("PLAY_SURVIVORFEMALEATTACKEDWOLF");
                Sounds.Add("PLAY_DYSENTERY");
                Sounds.Add("PLAY_INFECTION");
                Sounds.Add("PLAY_RABBITKILL");
                Sounds.Add("PLAY_REVOLVERFIRE");
                Sounds.Add("PLAY_RIFLEFIRE");
                Sounds.Add("PLAY_ENTITYATTACKSUCCESS");
                Sounds.Add("PLAY_ENTITYDEMO");
                Sounds.Add("PLAY_FPH_GEAR_BOLTCUTTERS_INTERACTION_CUT_SFX");

                if(GameManager.GetPlayerManagerComponent() && GameManager.GetPlayerManagerComponent().m_VoicePersona == VoicePersona.Female)
                {
                    Sounds.Add("PLAY_SNDVOSMASTE33370");
                    Sounds.Add("PLAY_SNDVOSMASTE33390");
                    Sounds.Add("PLAY_SNDVOSMASTE33410");
                    Sounds.Add("PLAY_SNDVOSMASTSURVIVORSIGHTINGCLOSE01");
                } else
                {
                    Sounds.Add("PLAY_SNDVOSMMACE43550");
                    Sounds.Add("PLAY_SNDVOSMMAC200");
                    Sounds.Add("PLAY_SNDVOSMMAC140");
                    Sounds.Add("PLAY_SNDVOSMMACE43550");
                    Sounds.Add("PLAY_SNDVOSMMACE410000");
                }

                if (InDoors())
                {
                    Sounds.Add("PLAY_STIMPACK");
                    Sounds.Add("PLAY_KNOCKDOWNHIT");
                    Sounds.Add("PLAY_SNDMECHSAFETUMBLERFALL");
                    Sounds.Add("PLAY_BODYFALLLARGE");
                    Sounds.Add("PLAY_ENTITYOUTSIDECABIN");
                    Sounds.Add("PLAY_ENTITYSHORTATTACK");
                    Sounds.Add("PLAY_ENTITYDEMO");
                    Sounds.Add("PLAY_OPENINVENTORY");
                    Sounds.Add("PLAY_SNDMECHDOORWOODOPEN1");
                    Sounds.Add("PLAY_SNDMECHDOORWOODOPEN2");
                } else
                {
                    Sounds.Add("PLAY_WOLFATTACK");
                    Sounds.Add("PLAY_TIMBERWOLFATTACK");
                    Sounds.Add("PLAY_TIMBERWOLFATTACKBITE");
                    Sounds.Add("PLAY_ENTITYDEATHSFX");
                    Sounds.Add("PLAY_RIFLEFIRESPOTLIGHT");
                    Sounds.Add("PLAY_SNDMECHDOORMETALCHAINOPEN1");

                }
            }

            if(RNG.NextDouble() < Chance)
            {
                GameAudioManager.PlaySound(Sounds[RNG.Next(Sounds.Count)], InterfaceManager.GetSoundEmitter());
            }
        }

        public static void SpawnRefMan()
        {
            GameObject reference = Resources.Load<GameObject>("ref_man_1_85_Prefab");

            if (reference)
            {
                System.Random RNG = new System.Random();
                Vector3 v3 = GameManager.GetPlayerTransform().transform.position;
                v3 = v3 - GameManager.GetPlayerTransform().transform.forward * 2;
                m_CurrentRefMan = UnityEngine.Object.Instantiate<GameObject>(reference, v3, GameManager.GetPlayerTransform().transform.rotation);

                UnityEngine.Object.Destroy(m_CurrentRefMan, 5);
            }
        }

        public static void TriggerNightmare()
        {
            //string OutDoorToLoad = "BearCave";
            //GameObject DummyLoader = new GameObject();
            //LoadScene Loader = DummyLoader.AddComponent<LoadScene>();

            //Loader.m_SceneToLoad = OutDoorToLoad;
            //Loader.Activate();
            
            m_TransitionDataBackup = Utils.SerializeObject(GameManager.m_SceneTransitionData);

            string sceneToLoad = "BearCave";
            GameManager.m_SceneTransitionData.m_SpawnPointName = "DefaultSpawnPoint";
            GameManager.m_SceneTransitionData.m_SpawnPointAudio = "";
            GameManager.m_SceneTransitionData.m_ForceSceneOnNextNavMapLoad = "";
            GameManager.m_SceneTransitionData.m_ForceNextSceneLoadTriggerScene = "";
            GameManager.m_SceneTransitionData.m_SceneLocationLocIDToShow = "";
            m_PreNightmareScene = MyMod.level_name;
            m_PreNightmareSceneWithGUID = MyMod.level_guid;
            m_PreNightmarePosition = GameManager.GetPlayerTransform().position;
            GameManager.LoadScene(sceneToLoad, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent);
        }


        public static void FinishNightmare()
        {
            GameManager.m_SceneTransitionData.m_SpawnPointName = "";
            GameManager.m_SceneTransitionData.m_SpawnPointAudio = "";
            GameManager.m_SceneTransitionData.m_ForceSceneOnNextNavMapLoad = "";
            GameManager.m_SceneTransitionData.m_SceneLocationLocIDToShow = "";

            GameManager.m_SceneTransitionData.m_ForceNextSceneLoadTriggerScene = m_PreNightmareSceneWithGUID;

            GameManager.GetPlayerManagerComponent().m_DoTeleportAfterSceneLoad = true;
            GameManager.GetPlayerManagerComponent().m_TeleportPending = true;
            GameManager.GetPlayerManagerComponent().m_TeleportPendingPosition = m_PreNightmarePosition;

            GameManager.LoadScene(m_PreNightmareScene, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent);
        }
    }
}
