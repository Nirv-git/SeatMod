using System;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UIExpansionKit.API;
using UnityEngine;
using UnityEngine.UI;

namespace SeatMod
{
    class UIX
    {
        private static Transform chairAnim, buttRotateChair, buttRotateParent, buttReset, buttKeyboard;
        private static GameObject lastLocation;
        private static string tempString = "";

        public static IEnumerator SetupUI()
        {
            yield return null;
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.UserQuickMenu).AddSimpleButton("Sit On...", () => SitOnMenu());
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Sit Menu", () => SitOnOptions(false));
        }

        private static void SitOnMenu() 
        { 
            var sitOnMenu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
            sitOnMenu.AddSimpleButton("Parent to Head", (() =>
            {
                var sitOnBoneConfirm = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
                sitOnBoneConfirm.AddLabel($"Are you sure you want to parent to their Head?");
                sitOnBoneConfirm.AddSimpleButton($"Yes", () =>
                {
                    sitOnMenu.Hide();
                    GameObject arm = GameObject.Find(Utils.GetSelectedUser().gameObject.name + "/ForwardDirection/Avatar");
                    GameObject Head = arm?.transform?.root?.GetComponentInChildren<VRCPlayer>()?.field_Internal_Animator_0?.GetBoneTransform(HumanBodyBones.Head)?.gameObject;
                    MelonCoroutines.Start(Main.HeadSit(Head));
                    MelonLoader.MelonLogger.Msg("Parented to Head - Press '9' to break out");
                    sitOnBoneConfirm.Hide();
                });
                sitOnBoneConfirm.AddSimpleButton($"No - Back", () =>
                {
                    sitOnBoneConfirm.Hide();
                    sitOnMenu.Show();
                });
                sitOnBoneConfirm.AddLabel($"This will simply move your avatar to follow this user's head. Use the 'Sit On...' button in your QM to adjust position");
                sitOnBoneConfirm.AddLabel($"Use 'Sit/Parent to Bone>Parent to -' for more advanced and rotation options");
                sitOnBoneConfirm.AddToggleButton("Suppress Falling Animations", (action) =>
                {
                    Main.noFallingAnim.Value = !Main.noFallingAnim.Value;
                }, () => Main.noFallingAnim.Value);
                sitOnBoneConfirm.Show();
            }));
            sitOnMenu.AddSimpleButton("Sit/Parent to Bone", (() =>
            {
                GameObject arm = GameObject.Find(Utils.GetSelectedUser().gameObject.name + "/ForwardDirection/Avatar");
                if (arm != null)
                {
                    sitOnMenu.Hide();
                    //lastLocaiton stuff
                    GameObject HeadBone = null;
                    if (Main.defaultHeadBone.Value)
                        HeadBone = arm?.transform?.root?.GetComponentInChildren<VRCPlayer>()?.field_Internal_Animator_0?.GetBoneTransform(HumanBodyBones.Head)?.gameObject;

                    if (Main.lastLocation.Value && (!lastLocation?.Equals(null) ?? false) && lastLocation.transform.IsChildOf(arm.transform.root))
                        SitOnBoneMenu(lastLocation);
                    else if(!HeadBone?.Equals(null) ?? false)
                        SitOnBoneMenu(HeadBone);
                    else
                        SitOnBoneMenu(arm);
                }
                else
                    MelonLogger.Msg(ConsoleColor.Yellow, "Can not find avatar '/ForwardDirection/Avatar'");
            }));
            sitOnMenu.AddSimpleButton("Options", () =>
            {
                SitOnOptions(true);
                sitOnMenu.Hide();
            });
            sitOnMenu.AddSimpleButton("Close", () => sitOnMenu.Hide());

            var AddKeyWarning = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1Column4Butt);
            AddKeyWarning.AddLabel($"This user does not have any consent phrase in their Bio or Status.\nHave them add one of the below, or your private phrase '{Main.privateKey}'");
            AddKeyWarning.AddLabel($"siton\nSit with me\nSeats Together");
            AddKeyWarning.AddSimpleButton("Close", () => AddKeyWarning.Hide());

            var blacklist = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1Column4Butt);
            blacklist.AddLabel($"This world does not allow use of this mod");
            blacklist.AddLabel($"Reason: {RiskFunct.WorldType()}");
            blacklist.AddSimpleButton("Close", () => blacklist.Hide());

            var noSit = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1Column4Butt);
            noSit.AddLabel($"This user currently does not allow use of this mod");
            noSit.AddLabel($"(Is DND or has a blacklisted status)");
            noSit.AddSimpleButton("Close", () => noSit.Hide());

            if ((Main.WorldType == 1 || Main.WorldType == 2 || Main.WorldType == 3 || Main.WorldType == 4 || Main.WorldType == 10)) 
                blacklist.Show();
            else if (Utils.CanSit(Utils.GetSelectedUser()) == 1) //Ensure someone has the right to sit
                sitOnMenu.Show();
            else if (Utils.CanSit(Utils.GetSelectedUser()) == 3) 
                noSit.Show();
            else AddKeyWarning.Show();  
        }

        private static void SitOnBoneMenu(GameObject selectedObject)
        {
            //lastLocation = selectedObject.transform.GetPath();
            lastLocation = selectedObject;

            var sitOnBoneMenu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnNarrow);
            sitOnBoneMenu.AddLabel($"Sit on/parent to a selected bone"); 
            sitOnBoneMenu.AddSimpleButton($"Sit with chair - {selectedObject.transform.name}", () => {
                var sitOnBoneConfirm = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
                sitOnBoneConfirm.AddLabel($"Are you sure you want to sit on {selectedObject.transform.name}");
                sitOnBoneConfirm.AddSimpleButton($"Yes", () => {
                    void SitOnBone()
                    {
                        Main.boneToSit = selectedObject;
                        Main.useChair = true;
                        MelonCoroutines.Start(Main.SitOnBone());
                        MelonLoader.MelonLogger.Msg("Sat on Bone - Press '9' to break out");
                        sitOnBoneConfirm.Hide();
                    }

                    if (Utils.LocalPlayerFBT() && Utils.IKTweaksEnabled() && !(Utils.IKTweaksAnimMode() == "All"))
                    {
                        var IKTanimBeforeChair = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
                        IKTanimBeforeChair.AddLabel($"IKTweaks is installed, enabled and you are in FBT  but IKTweaks Ignore Animations isn't set to 'All'. \nThis means you will be in the chair animation");
                        IKTanimBeforeChair.AddLabel($"instead of IKTweaks overriding it.\n  \nDo you want to change that option?");
                        IKTanimBeforeChair.AddSimpleButton($"Set IKTweaks:\n'Animations mode in FBT' to 'Ignore all'\nThen sit on bone", () =>
                        {
                            Utils.SetIKTweaksDisableAnim();
                            SitOnBone();
                            IKTanimBeforeChair.Hide();
                        });
                        IKTanimBeforeChair.AddSimpleButton($"Sit without making changes", () =>
                        {
                            SitOnBone();
                            IKTanimBeforeChair.Hide();
                        });
                        IKTanimBeforeChair.AddSimpleButton($"Back", () =>
                        {
                            IKTanimBeforeChair.Hide();
                            SitOnBoneMenu(selectedObject);
                        });
                        IKTanimBeforeChair.Show();
                    }
                    else
                        SitOnBone();
                });
                sitOnBoneConfirm.AddSimpleButton($"No - Back", () => {
                    sitOnBoneConfirm.Hide();
                    SitOnBoneMenu(selectedObject);
                });
                sitOnBoneConfirm.AddLabel($"This will seat you in a chair that is parented to the specific bone. If you are using FBT w/ IKTweaks Animations disabled, your pose should be your normal FBT one as IKT w/ that setting overrides chair animations, otherwise you will use the animation below.");
                string RotationText()
                {
                    switch (Main.rotate_Chair_en.Value)
                    {
                        case "Adjust": return "Adjustments Only";
                        case "Yaw-Rot": return "Yaw Rotation";
                        case "RollPitchYaw": return "Roll/Pitch/Yaw Rotation";
                        default: return "Something Broke - RotText Switch";
                    }
                }
                sitOnBoneConfirm.AddSimpleButton($"Rotation - {(RotationText())}", () =>
                {
                    Utils.RotateChairRotation();
                    buttRotateChair.GetComponentInChildren<Text>().text = $"Rotation - {(RotationText())}";
                }, (button) => buttRotateChair = button.transform);
                sitOnBoneConfirm.AddSimpleButton($"Chair animations: {Main.SittingAnim.Value}", () => {
                    Utils.RotateAnim();
                    chairAnim.GetComponentInChildren<Text>().text = $"Chair animations: {Main.SittingAnim.Value}";
                }, (button) => chairAnim = button.transform);
                sitOnBoneConfirm.Show();
            });

            sitOnBoneMenu.AddSimpleButton($"Parent to - {selectedObject.transform.name}", () => {
                var sitOnBoneConfirm = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
                sitOnBoneConfirm.AddLabel($"Are you sure you want to parent to {selectedObject.transform.name}");
                sitOnBoneConfirm.AddSimpleButton($"Yes", () => {
                    sitOnBoneConfirm.Hide();
                    void ParentToBone()
                    {
                        Main.boneToSit = selectedObject;
                        Main.useChair = false;
                        MelonCoroutines.Start(Main.SitOnBone());
                        MelonLoader.MelonLogger.Msg("Parented to Bone - Press '9' to break out");
                    }
                    if (Main.rotate_Parent.Value != "None" && ( !Utils.LocalPlayerFBT() || !(Utils.IKTweaksAnimMode() == "All") || !Utils.IKTweaksEnabled() ) && 
                    (Main.rotate_Around.Value != 0 || Main.rotate_Side.Value != 0 || Main.rotate_Forward.Value != 0))
                    {
                        var resetBeforeParent = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
                        resetBeforeParent.AddLabel($"You have selected a parent rotation option other than 'None' and there are currently adjustments offsets configured.");
                        resetBeforeParent.AddLabel($"This may cause odd head rotations since you are not using FBT and IKTweaks Disable All Animations.");
                        resetBeforeParent.AddSimpleButton($"Set rotation to 'None' and parent to bone", () => {
                            Main.rotate_Parent.Value = "None";
                            ParentToBone();
                            resetBeforeParent.Hide();
                        });
                        resetBeforeParent.AddSimpleButton($"Reset offsets and parent to bone", () => {
                        Main.rotate_Around.Value = 0;
                        Main.rotate_Side.Value = 0;
                        Main.rotate_Forward.Value = 0;
                        ParentToBone();
                        resetBeforeParent.Hide();
                        });
                        resetBeforeParent.AddSimpleButton($"Keep current settings and parent to bone", () => {
                            ParentToBone();
                            resetBeforeParent.Hide();
                        });
                        resetBeforeParent.AddSimpleButton($"Back", () => {
                            resetBeforeParent.Hide();
                            SitOnBoneMenu(selectedObject);
                        });
                        resetBeforeParent.Show();
                    }
                    else
                        ParentToBone();
                });
                sitOnBoneConfirm.AddSimpleButton($"No - Back", () => {
                    sitOnBoneConfirm.Hide();
                    SitOnBoneMenu(selectedObject);
                });
                sitOnBoneConfirm.AddLabel($"This will make your motions follow this specific bone.");
                string RotationParentText()
                {
                    switch (Main.rotate_Parent.Value)
                    {
                        case "None": return "None";
                        case "Adjust": return "Adjustments Only";
                        case "RotateY": return "Yaw Rotation";
                        case "RotateAll": return "Roll/Pitch/Yaw Rotation";
                        default: return "Something Broke - RotationParentText Switch";
                    }
                }
                sitOnBoneConfirm.AddSimpleButton($"Rotation: {(RotationParentText())}", () =>
                {
                    Utils.RotateParentRotate();
                    buttRotateParent.GetComponentInChildren<Text>().text = $"Rotation - {(RotationParentText())}";
                }, (button) => buttRotateParent = button.transform);
                sitOnBoneConfirm.AddToggleButton("Suppress Falling Animations", (action) =>
                {
                    Main.noFallingAnim.Value = !Main.noFallingAnim.Value;
                }, () => Main.noFallingAnim.Value);

                sitOnBoneConfirm.Show();
            });

            sitOnBoneMenu.AddSimpleButton($"Humanoid Shortcuts", () =>
            {
                var shortcutsMenu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
                shortcutsMenu.AddSimpleButton("<--Back", () => { shortcutsMenu.Hide(); SitOnBoneMenu(selectedObject); });

                if (selectedObject?.transform?.root?.GetComponentInChildren<VRCPlayer>()?.field_Internal_Animator_0?.isHuman ?? false)
                {
                    HumanBodyBones[] list = 
                    { HumanBodyBones.Hips, HumanBodyBones.Chest, HumanBodyBones.Head,
                      HumanBodyBones.LeftShoulder, HumanBodyBones.LeftHand,
                      HumanBodyBones.RightShoulder, HumanBodyBones.RightHand,
                      HumanBodyBones.LeftFoot, HumanBodyBones.RightFoot
                    };
                    //foreach (HumanBodyBones bodybone in Enum.GetValues(typeof(HumanBodyBones)))
                    foreach (HumanBodyBones bodybone in list)
                    {
                        try
                        {
                            var bone = selectedObject?.transform?.root?.GetComponentInChildren<VRCPlayer>().field_Internal_Animator_0.GetBoneTransform(bodybone);
                            if (!bone?.name?.Equals(null) ?? false) {
                                shortcutsMenu.AddSimpleButton($"{bodybone}|{bone.name}", () =>
                                {
                                    shortcutsMenu.Hide();
                                    shortcutsMenu = null;
                                    SitOnBoneMenu(bone.gameObject);
                                });
                            }
                        }
                        catch (Exception ex) { MelonLogger.Msg(ConsoleColor.Red, "" + ex.ToString()); }
                    }
                }
                else shortcutsMenu.AddLabel($"Avatar not Humanoid");
                shortcutsMenu.Show();

            });

            sitOnBoneMenu.AddSimpleButton("Close/Back", () => { sitOnBoneMenu.Hide(); SitOnMenu(); });

            sitOnBoneMenu.AddLabel($"--Children GameObjects--");
            //MelonLoader.MelonLogger.Msg(selectedObject.name);
            if (selectedObject?.transform?.parent?.gameObject != null && selectedObject?.transform?.parent?.name != "ForwardDirection") sitOnBoneMenu.AddSimpleButton($" <--Back", () => SitOnBoneMenu(selectedObject.transform.parent.gameObject));
                else sitOnBoneMenu.AddSimpleButton($" <--Back", () => { });
            if (selectedObject != null)
            {
                for (int i = 0; i < selectedObject.transform.childCount; i++)
                {
                    GameObject child = selectedObject.transform.GetChild(i).gameObject;
                    if (child?.name is null) continue;
                    if (child.active == false) continue;
                    sitOnBoneMenu.AddSimpleButton($"{child.name}", () => {
                        sitOnBoneMenu.Hide();
                        sitOnBoneMenu = null;
                        SitOnBoneMenu(child);
                    });
                }
            }
            sitOnBoneMenu.Show();
        }

        private static void SitOnOptions(bool UserQM)
        {
            var sitOnOptionsMenu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
            sitOnOptionsMenu.AddSimpleButton("Unsit", new Action(() =>
            {
                Main.Unsit();
            }));
            sitOnOptionsMenu.AddSimpleButton("Position Adjust", (() =>
            {
                sitOnOptionsMenu.Hide();
                SitOnAdjust(UserQM);
            }));
            sitOnOptionsMenu.AddSimpleButton("Rotation Adjust", (() =>
            {
                sitOnOptionsMenu.Hide();
                ShowRotationMenu(UserQM);
            }));
            sitOnOptionsMenu.AddSimpleButton("Settings", (() =>
            {
                sitOnOptionsMenu.Hide();
                SitOnSettings(UserQM);
            }));
            sitOnOptionsMenu.AddSimpleButton("Back/Close", (() =>
            {
                sitOnOptionsMenu.Hide();
                if (UserQM)
                    SitOnMenu();
                else
                    sitOnOptionsMenu.Hide();
            }));
            sitOnOptionsMenu.Show();
        }

        private static void SitOnSettings(bool UserQM)
        {
            var sitOnSettingsMenu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWideSlim);

            sitOnSettingsMenu.AddSimpleButton("Stored Positions", (() =>
            {
                StoredMenu(UserQM, true);
            }));
            sitOnSettingsMenu.AddSimpleButton("Stored Rotations", (() =>
            {
                StoredMenu(UserQM, false);
            }));

            string RotationChairText()
            {
                switch (Main.rotate_Chair_en.Value)
                {
                    case "Adjust": return "Adjustments Only";
                    case "Yaw-Rot": return "Yaw Rotation";
                    case "RollPitchYaw": return "Roll/Pitch/Yaw Rotation";
                    default: return "Something Broke - RotText Switch";
                }
            }
            sitOnSettingsMenu.AddSimpleButton($"Chair Rotation: {(RotationChairText())}", () =>
            {
                Utils.RotateChairRotation();
                buttRotateChair.GetComponentInChildren<Text>().text = $"Chair Rotation - {(RotationChairText())}";
            }, (button) => buttRotateChair = button.transform);


            sitOnSettingsMenu.AddSimpleButton($"Chair animations: {Main.SittingAnim.Value}", () => {
                Utils.RotateAnim();
                chairAnim.GetComponentInChildren<Text>().text = $"Chair animations: {Main.SittingAnim.Value}";
                Main main = new Main(); main.UpdateChairAnim();
            }, (button) => chairAnim = button.transform);

            sitOnSettingsMenu.AddToggleButton("No Falling Animations", (action) =>
            {
                Main.noFallingAnim.Value = !Main.noFallingAnim.Value;
            }, () => Main.noFallingAnim.Value);

            string RotationParentText()
            {
                switch (Main.rotate_Parent.Value)
                {
                    case "None": return "None";
                    case "Adjust": return "Adjustments Only";
                    case "RotateY": return "Yaw Rotation";
                    case "RotateAll": return "Roll/Pitch/Yaw Rotation";
                    default: return "Something Broke - RotationParentText Switch";
                }
            }

            sitOnSettingsMenu.AddSimpleButton($"Parent Rotation: {(RotationParentText())}", () =>
            {
                Utils.RotateParentRotate();
                buttRotateParent.GetComponentInChildren<Text>().text = $"Rotation - {(RotationParentText())}";
            }, (button) => buttRotateParent = button.transform);

            sitOnSettingsMenu.AddSimpleButton($"Personal Pin - {Main.privateKey}", () =>
            {
                var confirm = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
                confirm.AddSimpleButton($"Back", () => {
                    confirm.Hide();
                    SitOnSettings(UserQM);
                });
                confirm.AddLabel("- Debug - ");
                confirm.AddLabel($"Current world type:\n{RiskFunct.WorldType()}");
                confirm.AddLabel($"IKTweaks Enabled? " + Utils.IKTweaksEnabled() + $"\nIKTweaks Ignoring Animations: " + Utils.IKTweaksAnimMode());
                confirm.AddLabel($"\nCurrent user FBT? " + Utils.LocalPlayerFBT());

                confirm.Show();
            });

            sitOnSettingsMenu.AddSimpleButton("Back", (() =>
            {
                SitOnOptions(UserQM);
            }));

            sitOnSettingsMenu.Show();
        }

        private static void StoredMenu(bool UserQM, bool type)
        {//type T|F - Pos|Rot
            MelonPreferences_Entry<float> melonPref1 = type ? Main.head_Offset : Main.rotate_Around;
            MelonPreferences_Entry<float> melonPref2 = type ? Main.head_Offset_Back : Main.rotate_Forward;
            MelonPreferences_Entry<float> melonPref3 = type ? Main.head_Offset_Left : Main.rotate_Side;

            var storedMenu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu3ColumnsSlim);

            var slotNames = SaveSlots.GetSavedSlotNames(type);

            foreach (KeyValuePair<int, System.Tuple<float, float, float>> slot in SaveSlots.GetSaved(type))
            {
                string label = type ? $"Slot: {slot.Key}\n{slotNames[slot.Key]}\nUp:{Utils.NumberFormat(slot.Value.Item1)}\nForward:{Utils.NumberFormat(slot.Value.Item2)}\nSide:{Utils.NumberFormat(slot.Value.Item3)}"
                        :
                        $"Slot: {slot.Key}\n{slotNames[slot.Key]}\nY:{Utils.NumberFormat(slot.Value.Item1)}\nZ:{Utils.NumberFormat(slot.Value.Item2)}\nX:{Utils.NumberFormat(slot.Value.Item3)}";
                storedMenu.AddLabel(label);
                storedMenu.AddSimpleButton($"Load", () =>
                {
                    melonPref1.Value = slot.Value.Item1;
                    melonPref2.Value = slot.Value.Item2;
                    melonPref3.Value = slot.Value.Item3;
                    storedMenu.Hide();
                    StoredMenu(UserQM, type);
                });
                storedMenu.AddSimpleButton($"Save", () =>
                {
                    SaveSlots.Store(slot.Key, new System.Tuple<float, float, float>(melonPref1.Value, melonPref2.Value, melonPref3.Value), type);
                    storedMenu.Hide();
                    StoredMenu(UserQM, type);
                });
            }
            storedMenu.AddSimpleButton("Back", (() =>
            {
                SitOnSettings(UserQM);
            }));
            string current = type ? $"Current:\nUp:{Utils.NumberFormat(melonPref1.Value)}\nForward:{Utils.NumberFormat(melonPref2.Value)}\nSide:{Utils.NumberFormat(melonPref3.Value)}"
                        :
                        $"Current:\nY:{Utils.NumberFormat(melonPref1.Value)}\nZ:{Utils.NumberFormat(melonPref2.Value)}\nX:{Utils.NumberFormat(melonPref3.Value)}";
            storedMenu.AddLabel(current);
            storedMenu.AddSimpleButton($"Edit Slot Names", () =>
            {
                EditSlotNames(UserQM, type);
            });

            storedMenu.Show();
        }

        private static void EditSlotNames(bool UserQM, bool type)
        {//type T|F - Pos|Rot
            var menu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu3ColumnsSlim);

            var slotNames = SaveSlots.GetSavedSlotNames(type);
            foreach (KeyValuePair<int, string> slot in slotNames)
            {
                menu.AddLabel($"Slot: {slot.Key}\n{slot.Value}");
                menu.AddSimpleButton($"Load", () =>
                {
                    tempString = slot.Value;
                    EditSlotNames(UserQM, type);
                });
                menu.AddSimpleButton($"Save", () =>
                {
                    SaveSlots.StoreSlotNames(slot.Key, tempString, type);
                    EditSlotNames(UserQM, type);
                });

            }
            menu.AddSimpleButton("Back", (() =>
            {
                StoredMenu(UserQM, type);
            }));
            menu.AddLabel($"Current String:\n{tempString}");
            menu.AddSimpleButton($"Edit String", () =>
            {
                SetName(UserQM, type, false);
            });
            menu.Show();
        }

        public static void SetName(bool UserQM, bool type, bool cas)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz-_ ".ToCharArray();
            var menu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu3Column10Row);

            menu.AddSimpleButton("<-Back", () => { EditSlotNames(UserQM, type); });
            menu.AddLabel(tempString, (button) => buttKeyboard = button.transform); ;
            menu.AddSimpleButton("BackSpace", () => {
                if (tempString.Length > 0) tempString = tempString.Remove(tempString.Length - 1, 1);
                buttKeyboard.GetComponentInChildren<Text>().text = tempString;
            });

            foreach (char c in chars)
            {
                var s = cas ? c.ToString().ToUpper() : c.ToString();
                menu.AddSimpleButton(s, () => {
                    tempString += s;
                    buttKeyboard.GetComponentInChildren<Text>().text = tempString;
                });
            }

            menu.AddSimpleButton("Toggle Case", () => { SetName(UserQM, type, !cas); });
            menu.Show();
        }

        private static void SitOnAdjust(bool UserQM)
        {
            string ResetText() { return $"Reset\nY:{Utils.NumberFormat(Main.head_Offset.Value)}\nZ:{Utils.NumberFormat(Main.head_Offset_Back.Value)}\nX:{Utils.NumberFormat(Main.head_Offset_Left.Value)}"; }

            var sitOnAdjustMenu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescription.QuickMenu3Columns);
            sitOnAdjustMenu.AddSimpleButton($"Close/Back", () => {
                sitOnAdjustMenu.Hide();
                SitOnOptions(UserQM);
            });
            sitOnAdjustMenu.AddSimpleButton($"Forward", () => {
                Main.head_Offset_Back.Value -= Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            sitOnAdjustMenu.AddSimpleButton($"Up", () => {
                Main.head_Offset.Value += Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            sitOnAdjustMenu.AddSimpleButton($"Left", () => {
                Main.head_Offset_Left.Value += Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            sitOnAdjustMenu.AddSimpleButton(ResetText(), () => {
                var resetConfirm = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
                resetConfirm.AddLabel($"Are you sure you want to reset all values?");
                resetConfirm.AddSimpleButton($"Yes", () => {
                    Main.head_Offset_Back.Value = 0f;
                    Main.head_Offset.Value = 0f;
                    Main.head_Offset_Left.Value = 0f;
                    resetConfirm.Hide();
                    SitOnAdjust(UserQM);
                });
                resetConfirm.AddSimpleButton($"No", () => {
                    resetConfirm.Hide();
                    SitOnAdjust(UserQM);
                });
                resetConfirm.Show();         
            }, (button) => buttReset = button.transform);
            sitOnAdjustMenu.AddSimpleButton($"Right", () => {
                Main.head_Offset_Left.Value -= Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            sitOnAdjustMenu.AddToggleButton("High precision", b => Main.highPrecision = b, () => Main.highPrecision);
            sitOnAdjustMenu.AddSimpleButton($"Back", () => {
                Main.head_Offset_Back.Value += Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            sitOnAdjustMenu.AddSimpleButton($"Down", () => {
                Main.head_Offset.Value -= Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            sitOnAdjustMenu.Show();
        }

        private static void ShowRotationMenu(bool UserQM)
        {
            string ResetText() { return $"Reset\nY:{Utils.NumberFormat(Main.rotate_Around.Value)}\nZ:{Utils.NumberFormat(Main.rotate_Forward.Value)}\nX:{Utils.NumberFormat(Main.rotate_Side.Value)}"; }
            var rotationMenu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescription.QuickMenu3Columns);

            rotationMenu.AddSimpleButton("Close/Back", () => { rotationMenu.Hide(); SitOnOptions(UserQM); });
            
            rotationMenu.AddSimpleButton("Tilt Forward", () => {
                Main.rotate_Forward.Value -= Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            rotationMenu.AddSimpleButton("Rotate\nRight", () => {
                Main.rotate_Around.Value += Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            rotationMenu.AddSimpleButton("Tilt Left", () => {
                Main.rotate_Side.Value += Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            rotationMenu.AddSimpleButton(ResetText(), () => {
                var resetConfirm = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu1ColumnWide);
                resetConfirm.AddLabel($"Are you sure you want to reset all values?");
                resetConfirm.AddSimpleButton($"Yes", () => {
                    Main.rotate_Around.Value = 0;
                    Main.rotate_Side.Value = 0;
                    Main.rotate_Forward.Value = 0;
                    resetConfirm.Hide();
                    SitOnAdjust(UserQM);
                });
                resetConfirm.AddSimpleButton($"No", () => {
                    resetConfirm.Hide();
                    SitOnAdjust(UserQM);
                });
                resetConfirm.Show();
            }, (button) => buttReset = button.transform); 
            rotationMenu.AddSimpleButton("Tilt Right", () => {
                Main.rotate_Side.Value -= Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            rotationMenu.AddToggleButton("High precision", b => Main.highPrecision = b, () => Main.highPrecision);
            rotationMenu.AddSimpleButton("Tilt Backward", () => {
                Main.rotate_Forward.Value += Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            rotationMenu.AddSimpleButton("Rotate\nLeft", () => {
                Main.rotate_Around.Value -= Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                buttReset.GetComponentInChildren<Text>().text = ResetText();
            });
            rotationMenu.Show();
        }
    }
}

namespace UIExpansionKit.API
{
    public struct LayoutDescriptionCustom
    {
        // QuickMenu3Columns = new LayoutDescription { NumColumns = 3, RowHeight = 380 / 3, NumRows = 3 };
        // QuickMenu4Columns = new LayoutDescription { NumColumns = 4, RowHeight = 95, NumRows = 4 };
        // WideSlimList = new LayoutDescription { NumColumns = 1, RowHeight = 50, NumRows = 8 };
        public static LayoutDescription QuickMenu3ColumnsSlim = new LayoutDescription { NumColumns = 6, RowHeight = 380 / 4, NumRows = 4 };
        public static LayoutDescription QuickMenu1ColumnWide = new LayoutDescription { NumColumns = 1, RowHeight = 380 / 6, NumRows = 6 };
        public static LayoutDescription QuickMenu1ColumnWideSlim = new LayoutDescription { NumColumns = 1, RowHeight = 380 / 8, NumRows = 8 };
        public static LayoutDescription QuickMenu1Column4Butt = new LayoutDescription { NumColumns = 1, RowHeight = 380 / 4, NumRows = 4 };
        public static LayoutDescription QuickMenu1ColumnNarrow = new LayoutDescription { NumColumns = 1, RowHeight = 380 / 10, NumRows = 10 };
        public static LayoutDescription QuickMenu3Column10Row = new LayoutDescription { NumColumns = 3, RowHeight = 380 / 10, NumRows = 10 };
    }
}