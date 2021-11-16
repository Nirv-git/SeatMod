using UnityEngine;
using VRC.SDKBase;
using VRC;
using System;
using MelonLoader;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using VRC.UI.Elements.Menus;


namespace SeatMod
{
    public static class Utils
    {
        public static Player GetSelectedUser()
        {
            //return QuickMenu.prop_QuickMenu_0.field_Private_Player_0;
            var iuser = GameObject.Find("/UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local").GetComponentInChildren<SelectedUserMenuQM>().field_Private_IUser_0;
            var userID = iuser.prop_String_0;
            foreach (Player player in PlayerManager.Method_Public_Static_ArrayOf_Player_0())
            {
                if (!player) continue;
                if (player.prop_APIUser_0.id.Equals(userID)) return player;
            }
            return null;
        }

        public static VRCPlayer CurrentUser
        {
            get => VRCPlayer.field_Internal_Static_VRCPlayer_0;
            set => Utils.CurrentUser = Utils.CurrentUser;
        }

        /// <returns>Returns 3 if blacklisted status/busy, 1 if allowed, else 0</returns>
        public static int CanSit(Player player)
        {
            string bio = player.prop_APIUser_0.bio.ToLower();
            string status = player.prop_APIUser_0.statusDescription.ToLower();
            string statusType = player.prop_APIUser_0.status;
            //MelonLoader.MelonLogger.Msg(bio);
            //MelonLoader.MelonLogger.Msg(status);
            if(statusType == "busy" || status.Contains("nosit"))
                return 3;

            if (bio.Contains(Main.privateKey.ToString()) || status.Contains(Main.privateKey.ToString()) ||
                bio.Contains("siton") || status.Contains("siton") ||
                bio.Contains("sit with me") || status.Contains("sit with me") ||
                bio.Contains("seats together") || status.Contains("seats together")
                )
                return 1;
            else return 0;
        }
        public static string RandomString(int length)
        {
            var chars = "123456789";
            var stringChars = new char[length];
            var random = new System.Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            string temp = new String(stringChars);
            MelonLoader.MelonLogger.Msg("RandomString" + temp);
            return temp;
        }

        /// <returns>Returns the value of IKT 'IgnoreAnimationsMode' - 'N/A' if not installed</returns>
        public static string IKTweaksAnimMode()
        {
            if (MelonHandler.Mods.Any(m => m.Info.Name == "IKTweaks"))
            { //None - "Play all animations"
              //Head - "Ignore head animations"
              //Hands - "Ignore hands animations"
              //HandAndHead - "Ignore head and hands"
              //All - "Ignore all (always slide around)
                return MelonPreferences.GetEntryValue<string>("IkTweaks", "IgnoreAnimationsMode");
            }
            //else MelonLogger.Msg("IKTweaks is missing");
            return "N/A";
        }

        public static bool IKTweaksEnabled()
        {
            if (MelonHandler.Mods.Any(m => m.Info.Name == "IKTweaks"))
            { 
                return MelonPreferences.GetEntryValue<bool>("IkTweaks", "FullBodyVrIk");
            }
            return false;
        }

        public static void SetIKTweaksDisableAnim()
        {
            if (MelonHandler.Mods.Any(m => m.Info.Name == "IKTweaks"))
            {
                MelonPreferences.SetEntryValue<string>("IkTweaks", "IgnoreAnimationsMode", "All");
                MelonPreferences.Save();
            }
        }

        public static bool LocalPlayerFBT()
        { //Thanks Don Cheadle
            var IKController = Utils.CurrentUser.field_Private_VRC_AnimationController_0.field_Private_IkController_0;
            var IKControllerEnum = IKController.prop_IkType_0; //prop_EnumNPublicSealedvaInNoLiThFoSi7vUnique_0   //prop_IkType_0
            var hasfbt = IKControllerEnum.HasFlag(IkController.IkType.SixPoint); //EnumNPublicSealedvaInNoLiThFoSi7vUnique
            return hasfbt;
        }
 
        public static string GetUserCode()
        {//Say hi to a convoluted process of getting a number mostly unique to a user, but isn't exactly their ID
            string value = Regex.Replace(GetMd5Hash(Utils.CurrentUser.prop_String_3), "[^0-9]", "");
            return value.Substring(value.Length - 5, 5);
        }



        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));// Convert the input string to a byte array and compute the hash.
            StringBuilder sBuilder = new StringBuilder();// Create a new Stringbuilder to collect the byte and create a string.
            for (int i = 0; i < data.Length; i++)// Loop through each byte of the hashed data and format each one as a hexadecimal string.
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();// Return the hexadecimal string.
        }

        public static Player GetPlayer(this VRCPlayer Instance) => Instance.prop_Player_0;

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                return gameObject.AddComponent<T>();
            }
            return component;
        }

        public static VRCPlayer GetVRCPlayer()
        {
            return VRCPlayer.field_Internal_Static_VRCPlayer_0;
        }

        internal static VRCPlayerApi LocalPlayerApi
        {
            get
            {
                if (Main._vpalocal == null)
                {
                    Main._vpalocal = VRCPlayerApi.GetPlayerByGameObject(VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject);
                    //MelonLoader.MelonLogger.Msg("vpalocal if: " + Main._vpalocal);//
                }
                //MelonLoader.MelonLogger.Msg("vpalocal return: " + Main._vpalocal);//
                return Main._vpalocal;
            }
        }
        public static string NumberFormat(float value)
        {
            return value.ToString("F3").TrimEnd('0');
        }

        public static string GetPath(this Transform current)
        { //http://answers.unity.com/answers/261847/view.html
            if (current.parent == null)
                return "/" + current.name;
            return current.parent.GetPath() + "/" + current.name;
        }

        public static void RotateAnim()
        {
            switch (Main.SittingAnim.Value)
            {
                case "BasicSit": Main.SittingAnim.Value = "SitIdle"; break;
                case "SitIdle": Main.SittingAnim.Value = "SitCrossed"; break;
                case "SitCrossed": Main.SittingAnim.Value = "Laydown"; break;
                case "Laydown": Main.SittingAnim.Value = "BasicSit"; break;
                default: Main.SittingAnim.Value = "SitCrossed"; MelonLoader.MelonLogger.Msg("Something Broke - Utils.RotateAnim - Switch"); break;
            }
        }

        public static void RotateChairRotation()
        {
            switch (Main.rotate_Chair_en.Value)
            {
                case "Adjust": Main.rotate_Chair_en.Value = "Yaw-Rot"; break;
                case "Yaw-Rot": Main.rotate_Chair_en.Value = "RollPitchYaw"; break;
                case "RollPitchYaw": Main.rotate_Chair_en.Value = "Adjust"; break;
                default: Main.rotate_Chair_en.Value = "Yaw-Rot"; MelonLoader.MelonLogger.Msg("Something Broke - Utils.RotateChairRotation - Switch"); break;
            }
        }

        public static void RotateParentRotate()
        {
            switch (Main.rotate_Parent.Value)
            {
                case "None": Main.rotate_Parent.Value = "Adjust"; break;
                case "Adjust": Main.rotate_Parent.Value = "RotateY"; break;
                case "RotateY": Main.rotate_Parent.Value = "None"; break;
                //case "RotateAll": Main.rotate_Parent.Value = "None"; break;
                default: Main.rotate_Parent.Value = "RotateY"; MelonLoader.MelonLogger.Msg("Something Broke - Utils.RotateParentRotate - Switch"); break;
            }
        }

    }
}
