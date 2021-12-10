using MelonLoader;
using UIExpansionKit.API;
using UnityEngine;
using UnhollowerRuntimeLib;
using System;
using System.Linq;
using System.Collections;
using VRC.SDKBase;
using VRC.Animation;
using System.IO;

[assembly: MelonInfo(typeof(SeatMod.Main), "SeatMod", "1.0.3", "Nirvash")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.DarkBlue)]
[assembly: MelonOptionalDependencies("ActionMenuApi")]

//If anyone acutally reads this code, I am sorry, it is based on several months of me poking at this on and off and I forget why things are setup the way they are
//Okay, most has been cleaned up and is now readable

namespace SeatMod
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;

        public static bool SitActive;
        public static bool useChair;
        public static GameObject boneToSit;
        public static int SitType;
        public static GameObject _baseObj;
        public static bool highPrecision = false;
        public static readonly Vector3 gravity = Physics.gravity;
        private static Transform cameraTransform = null;
        private static Quaternion originalRotation;
        public static VRCPlayerApi _vpalocal;
        public static int WorldType = 10;
        public static VRCMotionState playerMotion;
        public static string privateKey;

        private const string catagory = "SeatMod";
        public static MelonPreferences_Category cat;
        public static MelonPreferences_Entry<float> teleRate;
        public static MelonPreferences_Entry<float> head_Offset;
        public static MelonPreferences_Entry<float> head_Offset_Back;
        public static MelonPreferences_Entry<float> head_Offset_Left;
        public static MelonPreferences_Entry<string> rotate_Chair_en;
        public static MelonPreferences_Entry<string> rotate_Parent;
        public static MelonPreferences_Entry<float> rotate_Forward;
        public static MelonPreferences_Entry<float> rotate_Side;
        public static MelonPreferences_Entry<float> rotate_Around;

        public static MelonPreferences_Entry<string> savedPos;
        public static MelonPreferences_Entry<string> savedRot;
        public static MelonPreferences_Entry<string> savedPosSlotNames;
        public static MelonPreferences_Entry<string> savedRotSlotNames;

        public static MelonPreferences_Entry<bool> noFallingAnim;
        public static MelonPreferences_Entry<string> SittingAnim;
        public static MelonPreferences_Entry<bool> defaultHeadBone;
        public static MelonPreferences_Entry<bool> lastLocation;

        public static MelonPreferences_Entry<float> rotationValue;
        public static MelonPreferences_Entry<float> highPrecisionRotationValue;
        public static MelonPreferences_Entry<float> positionValue;
        public static MelonPreferences_Entry<float> highPrecisionPositionValue;

        public static MelonPreferences_Entry<bool> amapi_en;
        public static MelonPreferences_Entry<bool> amapi_ModsFolder;

        public static MelonPreferences_Entry<bool> spacer1;
        public static MelonPreferences_Entry<bool> testing;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("SeatMod");
            cat = MelonPreferences.CreateCategory(catagory, "SeatMod Settings");

            teleRate = MelonPreferences.CreateEntry(catagory, nameof(teleRate), 1f / 1000f, "TeleportRate");
            rotate_Chair_en = MelonPreferences.CreateEntry(catagory, nameof(rotate_Chair_en), "Yaw-Rot", "Chair Rotation");
            ExpansionKitApi.RegisterSettingAsStringEnum(catagory, nameof(rotate_Chair_en), new[] { ("Adjust", "Adjustments Only"), ("Yaw-Rot", "Yaw Rotation"), ("RollPitchYaw", "Roll/Pitch/Yaw Rotation") });
            SittingAnim = MelonPreferences.CreateEntry(catagory, nameof(SittingAnim), "SitCrossed", "Chair sitting animation");
            ExpansionKitApi.RegisterSettingAsStringEnum(catagory, nameof(SittingAnim), new[] { ("BasicSit", "BasicSit"), ("SitIdle", "SitIdle"), ("SitCrossed", "SitCrossed"), ("Laydown", "Laydown") });

            amapi_en = MelonPreferences.CreateEntry(catagory, nameof(amapi_en), true, "Use Action Menu API (Restert Required)");
            rotate_Parent = MelonPreferences.CreateEntry(catagory, nameof(rotate_Parent), "RotateY", "Parent Rotation");
            ExpansionKitApi.RegisterSettingAsStringEnum(catagory, nameof(rotate_Parent), new[] { ("None", "None"), ("Adjust", "Adjustments Only"), ("RotateY", "Yaw Rotation") }); //, ("RotateAll", "RotateAll") });
            noFallingAnim = MelonPreferences.CreateEntry(catagory, nameof(noFallingAnim), true, "No Failling Animations");

            amapi_ModsFolder = MelonPreferences.CreateEntry(catagory, nameof(amapi_ModsFolder), false, "Place Action Menu in 'Mods' Sub Menu (Restert Required)");
            positionValue = MelonPreferences.CreateEntry(catagory, nameof(positionValue), .2f, "Position Adj value");
            highPrecisionPositionValue = MelonPreferences.CreateEntry(catagory, nameof(highPrecisionPositionValue), .05f, "Position Adj value (High precision)");
            //spacer1 = MelonPreferences.CreateEntry(catagory, nameof(spacer1), false, "-Does nothing-");

            defaultHeadBone = MelonPreferences.CreateEntry(catagory, nameof(defaultHeadBone), true, "Default to Head Bone in 'Sit/Parent to Bone' menu");
            rotationValue = MelonPreferences.CreateEntry(catagory, nameof(rotationValue), 22.5f, "Rotation Adj value");
            highPrecisionRotationValue = MelonPreferences.CreateEntry(catagory, nameof(highPrecisionRotationValue), 1f, "Rotation Adj value (High precision)");

            lastLocation = MelonPreferences.CreateEntry(catagory, nameof(lastLocation), true, "Try to return to the last location in 'Sit/Parent to Bone' menu");


            head_Offset = MelonPreferences.CreateEntry(catagory, nameof(head_Offset), 0f, "HeadOffset-Up", "", true);
            head_Offset_Back = MelonPreferences.CreateEntry(catagory, nameof(head_Offset_Back), 0f, "HeadOffset-Back", "", true);
            head_Offset_Left = MelonPreferences.CreateEntry(catagory, nameof(head_Offset_Left), 0f, "HeadOffset-Left", "", true);
            rotate_Forward = MelonPreferences.CreateEntry(catagory, nameof(rotate_Forward), 0f, "RotateForward", "", true);
            rotate_Side = MelonPreferences.CreateEntry(catagory, nameof(rotate_Side), 0f, "RotateSide", "", true);
            rotate_Around = MelonPreferences.CreateEntry(catagory, nameof(rotate_Around), 0f, "RotateAround", "", true);

            savedPos = MelonPreferences.CreateEntry(catagory, nameof(savedPos), "1,0.0,0.0,0.0;2,0.0,0.0,0.0;3,0.0,0.0,0.0;4,0.0,0.0,0.0;5,0.0,0.0,0.0;6,0.0,0.0,0.0", "savedPos", "", true);
            savedRot = MelonPreferences.CreateEntry(catagory, nameof(savedRot), "1,0.0,0.0,0.0;2,0.0,0.0,0.0;3,0.0,0.0,0.0;4,0.0,0.0,0.0;5,0.0,0.0,0.0;6,0.0,0.0,0.0", "savedRot", "", true);
            savedPosSlotNames = MelonPreferences.CreateEntry(catagory, nameof(savedPosSlotNames), "1,N/A;2,N/A;3,N/A;4,N/A;5,N/A;6,N/A", "savedSlotNames", "", true);
            savedRotSlotNames = MelonPreferences.CreateEntry(catagory, nameof(savedRotSlotNames), "1,N/A;2,N/A;3,N/A;4,N/A;5,N/A;6,N/A", "savedSlotNames", "", true);

            ExpansionKitApi.RegisterWaitConditionBeforeDecorating(UIX.SetupUI());
            ExpansionKitApi.OnUiManagerInit += UiManagerInit;

            loadAssets();

            if (MelonHandler.Mods.Any(m => m.Info.Name == "ActionMenuApi") && amapi_en.Value)
            {
                CustomActionMenu.InitUi();
            }
            else Logger.Msg("ActionMenuApi is missing, or setting is toggled off in Mod Settings - Not adding controls to ActionMenu");
            MelonCoroutines.Start(OnLoad());
        }

        public static IEnumerator OnLoad()
        {
            while (Utils.CurrentUser?._player?.field_Private_APIUser_0?.id == null)
                yield return new WaitForSeconds(5f);
            privateKey = Utils.GetUserCode();
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha9)) //The '9' key on the top of the alphanumeric keyboard.
                Unsit();
        }

        public void UiManagerInit()
        { //https://github.com/d-magit/VRC-Mods/blob/b69aaa85348b324f355711108d29ee3ecd3a2c88/TrackingRotator/Main.cs#L92
            //Using this as the method for rotating the camera view, only works for adjustments and Yaw
            var camera = UnityEngine.Object.FindObjectOfType<VRCVrCamera>();
            var Transform = camera.GetIl2CppType().GetFields(Il2CppSystem.Reflection.BindingFlags.Public | Il2CppSystem.Reflection.BindingFlags.Instance).Where(f => f.FieldType == Il2CppType.Of<Transform>()).ToArray()[0];
            cameraTransform = Transform.GetValue(camera).Cast<Transform>();
            originalRotation = cameraTransform.localRotation;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (buildIndex)
            {
                case -1:
                    WorldType = 10;
                    MelonCoroutines.Start(RiskFunct.CheckWorld());
                    if (_vpalocal != null)
                        _vpalocal = null;
                    break;
                default:
                    break;
            }
        }

        public static void Unsit()
        {
            if (SitActive)
            {
                switch (SitType)
                {
                    case 1:
                        MelonCoroutines.Stop(HeadSit(null));
                        Physics.gravity = Main.gravity;
                        SitActive = false;
                        Logger.Msg("Unsit - Parent Head");
                        break;
                    case 2:
                        MelonCoroutines.Stop(SitOnBone());
                        if (_baseObj != null) _baseObj.GetOrAddComponent<VRCSDK2.VRC_Station>().UseStation(Utils.LocalPlayerApi);
                        ToggleChair(false);
                        SitActive = false;
                        Logger.Msg("Unsit - Bone Chair");
                        break;
                    case 3:
                        MelonCoroutines.Stop(SitOnBone());
                        Physics.gravity = Main.gravity;
                        cameraTransform.localRotation = originalRotation;
                        SitActive = false;
                       Logger.Msg("Unsit - Bone Solo");
                        break;
                    default:
                        Logger.Error("Something broke - Unsit, SitType - Switch");
                        break;
                }
            }
            else Logger.Msg("SitActive not true, doing nothing");
        }

        public static IEnumerator SitOnBone()
        {
            SitActive = true;
            if (useChair)
            {
                SitType = 2;
                ToggleChair(true);
                _baseObj.GetOrAddComponent<VRCSDK2.VRC_Station>().UseStation(Utils.LocalPlayerApi);
                Quaternion chairDefault = _baseObj.transform.rotation;
                while (SitActive)
                {
                    try
                    {
                        _baseObj.transform.position = boneToSit.transform.position - boneToSit.transform.forward * head_Offset_Back.Value / 10f - boneToSit.transform.right * head_Offset_Left.Value / 10f + (rotate_Chair_en.Value == "RollPitchYaw" ? boneToSit.transform.up : Vector3.up) * head_Offset.Value / 10f;

                        switch (rotate_Chair_en.Value)
                        {
                            case "Adjust": //Adjustments only
                                _baseObj.transform.rotation = chairDefault * Quaternion.AngleAxis(rotate_Forward.Value, Vector3.left) * Quaternion.AngleAxis(rotate_Side.Value, Vector3.forward) * Quaternion.AngleAxis(rotate_Around.Value, Vector3.up);
                                break;
                            case "Yaw-Rot": //Yaw only roation with Adjustments
                                _baseObj.transform.rotation = Quaternion.Euler(chairDefault.eulerAngles.x, boneToSit.transform.rotation.eulerAngles.y, chairDefault.eulerAngles.z) * Quaternion.AngleAxis(rotate_Forward.Value, Vector3.left) * Quaternion.AngleAxis(rotate_Side.Value, Vector3.forward) * Quaternion.AngleAxis(rotate_Around.Value, Vector3.up);
                                break;
                            case "RollPitchYaw": //All rotation with Adjustments
                                _baseObj.transform.rotation = boneToSit.transform.rotation * Quaternion.AngleAxis(rotate_Forward.Value, Vector3.left) * Quaternion.AngleAxis(rotate_Side.Value, Vector3.forward) * Quaternion.AngleAxis(rotate_Around.Value, Vector3.up);
                                break;
                            default: Logger.Error("Something Broke - rotate_Chair_en Switch"); SitActive = false; break;
                        }
                    }
                    catch (System.Exception ex) { Logger.Error("Error in loop, calling unsit method:\n" + ex.ToString()); Unsit(); }
                    if (teleRate.Value > .0011) yield return new WaitForSeconds(teleRate.Value);
                    else yield return null;
                }
            }
            else //Not using chair
            {
                SitType = 3;
                playerMotion = Utils.CurrentUser.GetComponent<VRCMotionState>();
                while (SitActive)
                {
                    try
                    {
                        Physics.gravity = (Vector3.zero);
                        Utils.CurrentUser.transform.position = boneToSit.transform.position - boneToSit.transform.forward * head_Offset_Back.Value / 10f - boneToSit.transform.right * head_Offset_Left.Value / 10f + ((rotate_Parent.Value == "RotateAll") ? boneToSit.transform.up : Vector3.up) * head_Offset.Value / 10f;

                        switch (rotate_Parent.Value)
                        {
                            case "None": //None
                                cameraTransform.localRotation = originalRotation;
                                break;
                            case "Adjust": //Adjustments only
                                cameraTransform.localRotation = originalRotation * Quaternion.AngleAxis(rotate_Forward.Value, Vector3.left) * Quaternion.AngleAxis(rotate_Side.Value, Vector3.forward) * Quaternion.AngleAxis(rotate_Around.Value, Vector3.up); ;
                                break;
                            case "RotateY": //Yaw only roation with Adjustments
                                cameraTransform.rotation = Quaternion.Euler(originalRotation.eulerAngles.x, boneToSit.transform.rotation.eulerAngles.y, originalRotation.eulerAngles.z) * Quaternion.AngleAxis(rotate_Forward.Value, Vector3.left) * Quaternion.AngleAxis(rotate_Side.Value, Vector3.forward) * Quaternion.AngleAxis(rotate_Around.Value, Vector3.up);
                                break;
                            //case "RotateAll": //All rotation with Adjustments
                            //    cameraTransform.rotation = boneToSit.transform.rotation * Quaternion.AngleAxis(rotate_Forward.Value, Vector3.left) * Quaternion.AngleAxis(rotate_Side.Value, Vector3.forward) * Quaternion.AngleAxis(rotate_Around.Value, Vector3.up);
                            //    break;
                            default: Logger.Error("Something Broke - rotate_Parent.Value Switch"); SitActive = false; break;
                        }
                        if (noFallingAnim.Value)
                            playerMotion?.Reset();
                    }
                    catch (System.Exception ex) { Logger.Error("Error in loop, calling unsit method:\n" + ex.ToString()); Unsit(); }
                    if (teleRate.Value > .0011) yield return new WaitForSeconds(teleRate.Value);
                    else yield return null;
                }
            }
        }

        public static IEnumerator HeadSit(GameObject head)
        {//Ah, your simple, just parent to the head
            SitType = 1;
            playerMotion = Utils.CurrentUser.GetComponent<VRCMotionState>();
            GameObject sitTrans = null;
            if(head != null) 
                sitTrans = head;
            else
            {
                VRC.Player selctedAvatar = Utils.GetSelectedUser();
                sitTrans = GameObject.Find(selctedAvatar.gameObject.name + "/AnimationController/HeadAndHandIK/HeadEffector");
                Logger.Msg(ConsoleColor.Yellow, "Using IK HeadEffector");
            }

            SitActive = true;
            while (SitActive)
            {
                try
                {
                    Physics.gravity = (Vector3.zero);
                    Utils.CurrentUser.transform.position = sitTrans.transform.position - sitTrans.transform.forward * head_Offset_Back.Value / 10f - sitTrans.transform.right * head_Offset_Left.Value / 10f + Vector3.up * head_Offset.Value / 10f;
                    if (noFallingAnim.Value)
                        playerMotion?.Reset(); //Thanks to RequiDev/ReModCE
                }
                catch (System.Exception ex) { Logger.Error("Error in loop, calling unsit method:\n" + ex.ToString()); Unsit(); }
                if(teleRate.Value > .0011) yield return new WaitForSeconds(teleRate.Value);
                else yield return null;
            }
        }

        private static void ToggleChair(Boolean enableChair)
        {
            if (!enableChair && _baseObj != null)
            {
                UnityEngine.Object.Destroy(_baseObj);
                _baseObj = null;
            }
            if (enableChair)
            {
                GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                baseObj.transform.localScale = new Vector3(.005f, .005f, .005f);
                baseObj.name = "ChairBase";
                UnityEngine.Object.Destroy(baseObj.GetComponent<Collider>());
                baseObj.GetOrAddComponent<BoxCollider>().size = new Vector3(.005f, .005f, .005f);
                baseObj.GetOrAddComponent<BoxCollider>().isTrigger = true;

                baseObj.AddComponent<VRCSDK2.VRC_Station>();
                baseObj.GetComponent<VRCSDK2.VRC_Station>().PlayerMobility = VRCStation.Mobility.Immobilize;
                baseObj.GetOrAddComponent<VRCSDK2.VRC_Station>().seated = true;
                baseObj.GetOrAddComponent<VRCSDK2.VRC_Station>().animatorController = GetChairAnim();
                baseObj.GetOrAddComponent<VRCSDK2.VRC_Station>().stationEnterPlayerLocation = baseObj.transform;
                baseObj.GetOrAddComponent<VRCSDK2.VRC_Station>().stationExitPlayerLocation = baseObj.transform;
                baseObj.GetOrAddComponent<MeshRenderer>().enabled = false;
                _baseObj = baseObj;
            }
        }

        public static AnimatorOverrideController GetChairAnim()
        {
            switch (SittingAnim.Value)
            {
                //case "None": return null;
                case "SitIdle": return SitIdle;
                case "SitCrossed": return SitCrossed; 
                case "Laydown": return Laydown;
                case "BasicSit": return BasicSit;
                default: Logger.Error("Something Broke - GetChairAnim Switch"); return null;
            }
        }

        public void UpdateChairAnim()
        {if (SitType == 2 && _baseObj != null)
            {
                Logger.Msg("Resetting Sit for Animation");
                Unsit();
                MelonCoroutines.Start(Main.SitOnBone());
            }
            //else MelonLoader.Logger.Msg("_baseObj is null");
        }

        public static AssetBundle assetBundle;
        public static AnimatorOverrideController SitIdle;
        public static AnimatorOverrideController SitCrossed;
        public static AnimatorOverrideController Laydown;
        public static AnimatorOverrideController BasicSit;

        private void loadAssets()
        {//https://github.com/ddakebono/BTKSASelfPortrait/blob/master/BTKSASelfPortrait.cs
            using (var assetStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SeatMod.sittingoverrides"))
            {
                using (var tempStream = new MemoryStream((int)assetStream.Length))
                {
                    assetStream.CopyTo(tempStream);
                    assetBundle = AssetBundle.LoadFromMemory_Internal(tempStream.ToArray(), 0);
                    assetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                }
            }

            if (assetBundle != null)
            {
                SitIdle = assetBundle.LoadAsset_Internal("SitIdle", Il2CppType.Of<AnimatorOverrideController>()).Cast<AnimatorOverrideController>();
                SitIdle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                SitCrossed = assetBundle.LoadAsset_Internal("SitCrossed", Il2CppType.Of<AnimatorOverrideController>()).Cast<AnimatorOverrideController>();
                SitCrossed.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                Laydown = assetBundle.LoadAsset_Internal("Laydown", Il2CppType.Of<AnimatorOverrideController>()).Cast<AnimatorOverrideController>();
                Laydown.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                BasicSit = assetBundle.LoadAsset_Internal("BasicSit", Il2CppType.Of<AnimatorOverrideController>()).Cast<AnimatorOverrideController>();
                BasicSit.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
            else Logger.Error("Bundle was null");
        }
    }
}



