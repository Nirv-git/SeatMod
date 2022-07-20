using System;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;
using VRC.Core;
using SIDictionary = System.Collections.Generic.Dictionary<string, int>;

namespace SeatMod
{
    class RiskFunct
    {//I borrowed from https://github.com/Adnezz/VoiceFalloffOverride/blob/f1e6d300b0997e139e0bb616f32f8a9f7752f350/Utilities.cs#L42
        //Borrowed parts from https://github.com/loukylor/VRC-Mods/blob/main/VRChatUtilityKit/Utilities/VRCUtils.cs
        //And also https://github.com/Psychloor/PlayerRotater/blob/master/PlayerRotater/Utilities.cs

        private static bool alreadyCheckingWorld;
        private static SIDictionary checkedWorlds = new SIDictionary();
        //0: Unblocked
        //1: Club World
        //2: Game World
        //3: Emm Website Blacklisted, Mod Disabled
        //4: Emm GameObject Blacklisted, Mod Disabled
        //10: Not checked yet.
        //11: Allowed: Private Instance

        public static string WorldType()
        {
            switch (Main.WorldType)
            {
                case 0: return "World Allowed";
                case 1: return "Club World";
                case 2: return "Game World";
                case 3: return "EmmVRC DB Blacklisted";
                case 4: return "GameObject Blacklisted";
                case 10: return "Not checked yet - Error?";
                case 11: return "Private Instance: Mod Allowed";
                default: Main.Logger.Error($"Something Broke - Main.WorldType Switch - {Main.WorldType}"); return "Error";
            }
        }

        internal static System.Collections.IEnumerator CheckWorld()
        {
            if (alreadyCheckingWorld)
            {
                Main.Logger.Error("Attempted to check for world multiple times");
                yield break;
            }

            // Wait for RoomManager to exist before continuing.
            ApiWorld currentWorld = null;
            while (currentWorld == null)
            {
                currentWorld = RoomManager.field_Internal_Static_ApiWorld_0;
                yield return new WaitForSecondsRealtime(1);
            }
            var worldId = currentWorld.id;
            //Main.Logger.Msg($"Checking World with Id {worldId}");

            // Check if instance is private
            if (RoomManager.field_Internal_Static_ApiWorldInstance_0?.type == InstanceAccessType.InvitePlus ||
                RoomManager.field_Internal_Static_ApiWorldInstance_0?.type == InstanceAccessType.InviteOnly ||
                RoomManager.field_Internal_Static_ApiWorldInstance_0?.type == InstanceAccessType.FriendsOnly)
            {
                //Main.Logger.Msg($"Instance is private: '{RoomManager.field_Internal_Static_ApiWorldInstance_0?.type}'");
                Main.WorldType = 11;
                //Do not cache result
                yield break;
            }

            // Check cache for world, so we keep the number of API calls lower.
            if (checkedWorlds.TryGetValue(worldId, out int outres))
            {
                Main.WorldType = outres;
                //Main.Logger.Msg($"Using cached check {Main.WorldType} for world '{worldId}'");
                yield break;
            }

            // Check for Game Objects first, as it's the lowest cost check.
            if (GameObject.Find("eVRCRiskFuncEnable") != null || GameObject.Find("UniversalRiskyFuncEnable") != null || GameObject.Find("ModCompatRiskyFuncEnable") != null)
            {
                Main.WorldType = 0;
                checkedWorlds.Add(worldId, 0);
                yield break;
            }
            else if (GameObject.Find("eVRCRiskFuncDisable") != null || GameObject.Find("UniversalRiskyFuncDisable") != null || GameObject.Find("ModCompatRiskyFuncDisable") != null)
            {
                Main.WorldType = 4;
                checkedWorlds.Add(worldId, 4);
                yield break;
            }

            alreadyCheckingWorld = true;
            // Check if black/whitelisted from EmmVRC - thanks Emilia and the rest of EmmVRC Staff
            var uwr = UnityWebRequest.Get($"https://prod-dl.emmvrc.com/risky_func/{worldId}");
            uwr.SendWebRequest();
            while (!uwr.isDone)
                yield return new WaitForEndOfFrame();

            var result = uwr.downloadHandler.text?.Trim().ToLower();
            uwr.Dispose();
            if (!string.IsNullOrWhiteSpace(result))
            {
                switch (result)
                {
                    case "allowed":
                        Main.WorldType = 0;
                        checkedWorlds.Add(worldId, 0);
                        alreadyCheckingWorld = false;
                        //Main.Logger.Msg($"EmmVRC allows world '{worldId}'");
                        yield break;

                    case "denied":
                        Main.WorldType = 3;
                        checkedWorlds.Add(worldId, 3);
                        alreadyCheckingWorld = false;
                        //Main.Logger.Msg($"EmmVRC denies world '{worldId}'");
                        yield break;
                }
            }

            // No result from server or they're currently down
            // Check tags then. should also be in cache as it just got downloaded
            API.Fetch<ApiWorld>(
                worldId,
                new Action<ApiContainer>(
                    container =>
                    {
                        ApiWorld apiWorld;
                        if ((apiWorld = container.Model.TryCast<ApiWorld>()) != null)
                        {
                            short tagResult = 0;
                            foreach (var worldTag in apiWorld.tags)
                            {
                                if (worldTag.IndexOf("game", StringComparison.OrdinalIgnoreCase) != -1 && worldTag.IndexOf("games", StringComparison.OrdinalIgnoreCase) == -1)
                                {
                                    tagResult = 2;
                                    //Main.Logger.Msg($"Found game tag in world world '{worldId}'");
                                    break;
                                }
                                else if (worldTag.IndexOf("club", StringComparison.OrdinalIgnoreCase) != -1)
                                    tagResult = 1;
                            }
                            Main.WorldType = tagResult;
                            checkedWorlds.Add(worldId, tagResult);
                            alreadyCheckingWorld = false;
                            //Main.Logger.Msg($"Tag search result: '{tagResult}' for '{worldId}'");
                        }
                        else
                        {
                            Main.Logger.Error("Failed to cast ApiModel to ApiWorld");
                        }
                    }),
                disableCache: false);
            
        }

    }
}
