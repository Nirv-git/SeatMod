using System.Reflection;
using MelonLoader;
using UnityEngine;
using UnhollowerRuntimeLib;
using System.IO;
using ActionMenuApi.Api;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace SeatMod
{
    public class CustomActionMenu
    {
        public static AssetBundle assetBundleIcons;
        public static Texture2D arrowUp, arrowDown, arrowLeft, arrowRight, arrowForwards, arrowBackwards, dualDown, dualUp, rotateLeft, rotateRight, gears, mag, arrowsMenu, chairLeave, chair, checkMark, reset,
            animSitIdle, animSitCrossed, animLay, animBasicSit, cordXYZ, cordY, cordNone, x, noFalling, saveRotation, savePosition, save, load;
        public static Texture2D s1, s2, s3, s4, s5, s6, curr, reset_text;
        public static Image reset_Fade;

        private static void loadAssets()
        {
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SeatMod.seaticons"))
            {
                using (var tempStream = new MemoryStream((int)assetStream.Length))
                {
                    assetStream.CopyTo(tempStream);
                    assetBundleIcons = AssetBundle.LoadFromMemory_Internal(tempStream.ToArray(), 0);
                    assetBundleIcons.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                }
            }

            if (assetBundleIcons != null)
            {
                try { arrowDown = LoadTexture("arrow-Down.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { arrowUp = LoadTexture("arrow-Up.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { arrowLeft = LoadTexture("arrow-Left.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { arrowRight = LoadTexture("arrow-Right.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { dualDown = LoadTexture("dual-Down.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { dualUp = LoadTexture("dual-Up.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { rotateLeft = LoadTexture("rotate-Left.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { rotateRight = LoadTexture("rotate-Right.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { mag = LoadTexture("mag.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { gears = LoadTexture("gears.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { arrowForwards = LoadTexture("arrow-forward.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { arrowBackwards = LoadTexture("arrow-backwards.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { gears = LoadTexture("gears.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { arrowsMenu = LoadTexture("arrow-Menu.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { chairLeave = LoadTexture("chairLeave"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { chair = LoadTexture("chair.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { checkMark = LoadTexture("checkMark.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { reset = LoadTexture("reset.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { animLay = LoadTexture("anim-Lay.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { animSitCrossed = LoadTexture("amin-SitCrossed.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { animSitIdle = LoadTexture("anim-SitIdle.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { animBasicSit = LoadTexture("BasicSit.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { cordXYZ = LoadTexture("cord-XYZ.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { cordY = LoadTexture("cord-Y.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { cordNone = LoadTexture("cord-None.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { x = LoadTexture("x.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { noFalling = LoadTexture("noFalling.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { savePosition = LoadTexture("savePosition.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { saveRotation = LoadTexture("saveRotation.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { save = LoadTexture("save.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }
                try { load = LoadTexture("load.png"); } catch { Main.Logger.Error("Failed to load image from asset bundle"); }

            }
            else Main.Logger.Error("Bundle was null");
        }

        private static Texture2D LoadTexture(string Texture)
        { // https://github.com/KortyBoi/VRChat-TeleporterVR/blob/59bdfb200497db665621b519a9ff9c3d1c3f2bc8/Utils/ResourceManager.cs#L32
            Texture2D Texture2 = assetBundleIcons.LoadAsset_Internal(Texture, Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
            Texture2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            //Texture2.hideFlags = HideFlags.HideAndDontSave;
            return Texture2;
        }

        private static void LoadEmbeddedImages() 
        {
            using (var assetStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SeatMod.IconsEmbedded.reset_fade.png"))
            {
                using (var tempStream = new MemoryStream((int)assetStream.Length))
                {
                    assetStream.CopyTo(tempStream);
                    reset_Fade = new Bitmap(tempStream);
                }
            }

        }

        private static void InitTextures()
        {
            s1 = new Texture2D(2,2);
            s1.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            s2 = new Texture2D(2,2);
            s2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            s3 = new Texture2D(2,2);
            s3.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            s4 = new Texture2D(2,2);
            s4.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            s5 = new Texture2D(2,2);
            s5.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            s6 = new Texture2D(2,2);
            s6.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            curr = new Texture2D(2,2);
            curr.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            reset_text = new Texture2D(2,2);
            reset_text.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        }


        public static void InitUi()
        {
            loadAssets();
            InitTextures();
            LoadEmbeddedImages();

            if (Main.amapi_ModsFolder.Value)
                AMUtils.AddToModsFolder("<color=#ff00ff>Seat Mod</color>", () => AMsubMenu(), chair);
            else
                VRCActionMenuPage.AddSubMenu(ActionMenuPage.Main, "<color=#ff00ff>Seat Mod</color>", () => AMsubMenu(), chair);
        }


        private static void AMsubMenu()
        {
            CustomSubMenu.AddSubMenu("Unsit", () =>
            {
                CustomSubMenu.AddButton("Confirm?", () =>
                {
                    Main.Unsit();
                }, checkMark); //, !Main.SitActive);
            }, chairLeave);//, !Main.SitActive);;

            CustomSubMenu.AddSubMenu("Position Adjust", () =>
            {
                void ResetText()
                {
                    string current = $"Current\nY:{Utils.NumberFormat(Main.head_Offset.Value)}\nZ:{Utils.NumberFormat(Main.head_Offset_Back.Value)}\nX:{Utils.NumberFormat(Main.head_Offset_Left.Value)}";
                    ImageConversion.LoadImage(reset_text, ImageGen.ImageToPNG(ImageGen.DrawText(current, null, reset_Fade)));
                }
                ResetText();

                CustomSubMenu.AddButton("Up", () =>
                {
                    Main.head_Offset.Value += Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                    ResetText();
                }, arrowUp);
                CustomSubMenu.AddButton("Down", () =>
                {
                    Main.head_Offset.Value -= Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                    ResetText();
                }, arrowDown);
                CustomSubMenu.AddButton("Forward", () =>
                {
                    Main.head_Offset_Back.Value -= Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                    ResetText();
                }, arrowForwards);
                CustomSubMenu.AddButton("Back", () =>
                {
                    Main.head_Offset_Back.Value += Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                    ResetText();
                }, arrowBackwards);
                CustomSubMenu.AddButton("Right", () =>
                {
                    Main.head_Offset_Left.Value -= Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                    ResetText();
                }, arrowRight);
                CustomSubMenu.AddButton("Left", () =>
                {
                    Main.head_Offset_Left.Value += Main.highPrecision ? Main.highPrecisionPositionValue.Value : Main.positionValue.Value;
                    ResetText();
                }, arrowLeft);
                CustomSubMenu.AddToggle("High precision", Main.highPrecision, (action) =>
                {
                    Main.highPrecision = !Main.highPrecision;
                }, mag);
                CustomSubMenu.AddSubMenu("Reset", () =>
                {
                    CustomSubMenu.AddButton("Confirm?", () =>
                    {
                        Main.head_Offset_Back.Value = 0f;
                        Main.head_Offset.Value = 0f;
                        Main.head_Offset_Left.Value = 0f;
                        ResetText();
                    }, checkMark);
                }, reset_text);
            }, arrowsMenu);

            CustomSubMenu.AddSubMenu("Rotation Adjust", () =>
            {
                void ResetText()
                {
                    string current = $"Current\nY:{Utils.NumberFormat(Main.rotate_Around.Value)}\nZ:{Utils.NumberFormat(Main.rotate_Forward.Value)}\nX:{Utils.NumberFormat(Main.rotate_Side.Value)}";
                    ImageConversion.LoadImage(reset_text, ImageGen.ImageToPNG(ImageGen.DrawText(current, null, reset_Fade)));
                }
                ResetText();

                CustomSubMenu.AddButton("Rotate Left", () =>
                {
                    Main.rotate_Around.Value -= Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                    ResetText();
                }, rotateLeft);
                CustomSubMenu.AddButton("Rotate Right", () =>
                {
                    Main.rotate_Around.Value += Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                    ResetText();
                }, rotateRight);
                CustomSubMenu.AddButton("Tilt Forward", () =>
                {
                    Main.rotate_Forward.Value -= Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                    ResetText();
                }, arrowForwards);
                CustomSubMenu.AddButton("Tilt Backward", () =>
                {
                    Main.rotate_Forward.Value += Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                    ResetText();
                }, arrowBackwards);
                CustomSubMenu.AddButton("Tilt Right", () =>
                {
                    Main.rotate_Side.Value -= Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                    ResetText();
                }, arrowRight);
                CustomSubMenu.AddButton("Tilt Left", () =>
                {
                    Main.rotate_Side.Value += Main.highPrecision ? Main.highPrecisionRotationValue.Value : Main.rotationValue.Value;
                    ResetText();
                }, arrowLeft);
                CustomSubMenu.AddToggle("High precision", Main.highPrecision, (action) =>
                {
                    Main.highPrecision = !Main.highPrecision;
                }, mag);
                CustomSubMenu.AddSubMenu("Reset", () =>
                {
                    CustomSubMenu.AddButton("Confirm?", () =>
                    {
                        Main.rotate_Around.Value = 0;
                        Main.rotate_Side.Value = 0;
                        Main.rotate_Forward.Value = 0;
                        ResetText();
                    }, checkMark);
                }, reset_text);

            }, rotateRight);

            CustomSubMenu.AddSubMenu("Settings", () =>
            {//Saved Pos, Saved Rot, Rotation Chair, Chair Animations, No Falling, Parent Rotation
                StoredMenus(true); //Positionn
                StoredMenus(false); //Rotation

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
                Texture2D RotationChairIcon()
                {
                    switch (Main.rotate_Chair_en.Value)
                    {
                        case "Adjust": return cordNone;
                        case "Yaw-Rot": return cordY;
                        case "RollPitchYaw": return cordXYZ;
                        default: Main.Logger.Msg("Something Broke - RotationIcon Switch"); return rotateLeft;
                    }
                }
                CustomSubMenu.AddButton($"\nChair Rotation:\n{(RotationChairText())}", () =>
                {
                    Utils.RotateChairRotation();
                    AMUtils.RefreshActionMenu();
                }, RotationChairIcon());

                string AnimText()
                {
                    switch (Main.SittingAnim.Value)
                    {
                        //case "None": return "None";
                        case "SitIdle": return "SitIdle";
                        case "SitCrossed": return "SitCrossed";
                        case "Laydown": return "Laydown";
                        case "BasicSit": return "BasicSit";
                        default: return "Something Broke - AnimText Switch";
                    }
                }
                Texture2D AnimIcon()
                {
                    switch (Main.SittingAnim.Value)
                    {
                        //case "None": return x; 
                        case "SitIdle": return animSitIdle;
                        case "SitCrossed": return animSitCrossed;
                        case "Laydown": return animLay;
                        case "BasicSit": return animBasicSit;
                        default: Main.Logger.Msg("Something Broke - AnimIcon Switch"); return rotateLeft;
                    }
                }
                CustomSubMenu.AddButton($"\n\nChair Anim:\n{(AnimText())}", () =>
                {
                    Utils.RotateAnim();
                    Main main = new Main(); main.UpdateChairAnim();
                    AMUtils.RefreshActionMenu();
                }, AnimIcon());


                CustomSubMenu.AddToggle("\nNo Falling Animations", Main.noFallingAnim.Value, (action) =>
                {
                    Main.noFallingAnim.Value = !Main.noFallingAnim.Value;
                }, noFalling);

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
                Texture2D RotationParentIcon()
                {
                    switch (Main.rotate_Parent.Value)
                    {
                        case "None": return x;
                        case "Adjust": return cordNone;
                        case "RotateY": return cordY;
                        case "RotateAll": return cordXYZ;
                        default: Main.Logger.Msg("Something Broke - RotationParentIcon Switch"); return rotateLeft;
                    }
                }
                CustomSubMenu.AddButton($"\nParent Rotation:\n{(RotationParentText())}", () =>
                {
                    Utils.RotateParentRotate();
                    AMUtils.RefreshActionMenu();
                }, RotationParentIcon());

            }, gears);
        }


        static ref Texture2D StoredIcon(int key)
        {
            switch (key)
            {
                case 1: return ref s1;
                case 2: return ref s2;
                case 3: return ref s3;
                case 4: return ref s4;
                case 5: return ref s5;
                case 6: return ref s6;
                default: Main.Logger.Msg("Something Broke - StoredIcon Switch"); return ref x;
            }
        }

        private static void GenTextures(Dictionary<int, System.Tuple<float, float, float>> savedInfo, string current, bool type)
        {
            foreach (KeyValuePair<int, System.Tuple<float, float, float>> slot in savedInfo)
            {
                string label = $"Slot: {slot.Key}\nY:{Utils.NumberFormat(slot.Value.Item1)}\nZ:{Utils.NumberFormat(slot.Value.Item2)}\nX:{Utils.NumberFormat(slot.Value.Item3)}";
                ImageConversion.LoadImage(StoredIcon(slot.Key), ImageGen.ImageToPNG(ImageGen.DrawText(label)));
            }
            ImageConversion.LoadImage(curr, ImageGen.ImageToPNG(ImageGen.DrawText(current)));
        }

        private static void StoredMenus(bool type)
        {//type T|F - Pos|Rot

            MelonPreferences_Entry<float> melonPref1 = type ? Main.head_Offset : Main.rotate_Around;
            MelonPreferences_Entry<float> melonPref2 = type ? Main.head_Offset_Back : Main.rotate_Forward;
            MelonPreferences_Entry<float> melonPref3 = type ? Main.head_Offset_Left : Main.rotate_Side;

            CustomSubMenu.AddSubMenu(type ? "Saved Positions" : "Saved Rotations", () =>
            {
                var savedInfo = SaveSlots.GetSaved(type);
                string current4Image = $"Current:\nY:{Utils.NumberFormat(melonPref1.Value)}\nZ:{Utils.NumberFormat(melonPref2.Value)}\nX:{Utils.NumberFormat(melonPref3.Value)}";
                string current = $"Current - Y:{Utils.NumberFormat(melonPref1.Value)} Z:{Utils.NumberFormat(melonPref2.Value)} X:{Utils.NumberFormat(melonPref3.Value)}";
                GenTextures(savedInfo, current4Image, type);

                var slotNames = SaveSlots.GetSavedSlotNames(type);
                foreach (KeyValuePair<int, System.Tuple<float, float, float>> slot in savedInfo)
                {
                    string labelgen()
                    {
                        return $"Slot:{slot.Key} Y:{Utils.NumberFormat(slot.Value.Item1)} Z:{Utils.NumberFormat(slot.Value.Item2)} X:{Utils.NumberFormat(slot.Value.Item3)}";
                    }
                    
                    CustomSubMenu.AddSubMenu("<size=10>\n</size>" +  $"{slotNames[slot.Key]}", () => //"<size=10>\n" + labelgen() + "</size>", () =>
                    {
                        CustomSubMenu.AddButton($"\n<size=40>Save</size>", () =>
                        {
                            SaveSlots.Store(slot.Key, new System.Tuple<float, float, float>(melonPref1.Value, melonPref2.Value, melonPref3.Value), type);
                            GenTextures(SaveSlots.GetSaved(type), current4Image, type);
                            AMUtils.RefreshActionMenu();
                        }, save);

                        CustomSubMenu.AddButton("<size=10>\n</size>" + $"{slotNames[slot.Key]}", () => //"<size=10>\n" + labelgen() + "</size>", () =>  
                        {
                        }, StoredIcon(slot.Key));

                        CustomSubMenu.AddButton($"\n<size=40>Load</size>", () =>
                        {
                            melonPref1.Value = slot.Value.Item1;
                            melonPref2.Value = slot.Value.Item2;
                            melonPref3.Value = slot.Value.Item3;
                        }, load);
                    }, StoredIcon(slot.Key));
                }
                CustomSubMenu.AddButton("<size=10>\n" + current + "</size>", () =>
                {
                }, curr);
            }, type ? savePosition : saveRotation );
        }

    }
}
