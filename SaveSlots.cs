using MelonLoader;
using System;
using System.Linq;
using System.Collections.Generic;


namespace SeatMod
{
    class SaveSlots
    {
        //Data will look like 1,5.666,5344.55,343.56;
        public static Dictionary<int, System.Tuple<float, float, float>> GetSaved(bool type)
        {//type T|F - Pos|Rot
            MelonPreferences_Entry<string> melonPref = type ? Main.savedPos : Main.savedRot;
            try
            {
                //MelonLoader.MelonLogger.Msg("Value: " + melonPref.Value);
                return new Dictionary<int, System.Tuple<float, float, float>>(melonPref.Value.Split(';').Select(s => s.Split(',')).ToDictionary(p => int.Parse(p[0]), p => new System.Tuple<float, float, float>(float.Parse(p[1]), float.Parse(p[2]), float.Parse(p[3]))));
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"Error loading {(type ? "SavedPos" : "SavedRot")} - Resetting to Defaults:\n" + ex.ToString()); melonPref.Value = "1,0.0,0.0,0.0;2,0.0,0.0,0.0;3,0.0,0.0,0.0;4,0.0,0.0,0.0;5,0.0,0.0,0.0;6,0.0,0.0,0.0"; }
            return new Dictionary<int, System.Tuple<float, float, float>>() { { 1, new System.Tuple<float, float, float>(999.999f, 999.999f, 999.999f)} };
            
        }

        public static void Store(int location, System.Tuple<float, float, float> updated, bool type)
        {//type T|F - Pos|Rot
            MelonPreferences_Entry<string> melonPref = type ? Main.savedPos : Main.savedRot;
            try
            {
                var Dict = GetSaved(type);
                Dict[location] = updated;
                melonPref.Value = string.Join(";", Dict.Select(s => String.Format("{0},{1},{2},{3}", s.Key, s.Value.Item1.ToString("F5").TrimEnd('0'), s.Value.Item2.ToString("F5").TrimEnd('0'), s.Value.Item3.ToString("F5").TrimEnd('0'))));
                Main.cat.SaveToFile();
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"Error storing new saved {(type ? "position" : "rotation")}\n" + ex.ToString()); }
        }

        //Slot names
        //"1,Slot 1;2,Slot 2;3,Slot 3;4,Slot 4;5,Slot 5;6,Slot 6"
        public static Dictionary<int, string> GetSavedSlotNames(bool type)
        {
            MelonPreferences_Entry<string> melonPref = type ? Main.savedPosSlotNames : Main.savedRotSlotNames;
            try
            {
                //MelonLoader.MelonLogger.Msg("Value: " + melonPref.Value);
                return new Dictionary<int, string>(melonPref.Value.Split(';').Select(s => s.Split(',')).ToDictionary(p => int.Parse(p[0]), p => p[1]));
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"Error loading slot names - {(type ? "position" : "rotation")}- Resetting to Defaults:\n" + ex.ToString()); melonPref.Value = "1,N/A;2,N/A;3,N/A;4,N/A;5,N/A;6,N/A"; }
            return new Dictionary<int, string>() { { 1, "Error" } };

        }

        public static void StoreSlotNames(int location, string updated, bool type)
        {
            MelonPreferences_Entry<string> melonPref = type ? Main.savedPosSlotNames : Main.savedRotSlotNames;
            try
            {
                var Dict = GetSavedSlotNames(type);
                Dict[location] = updated;
                melonPref.Value = string.Join(";", Dict.Select(s => String.Format("{0},{1}", s.Key, s.Value)));
                Main.cat.SaveToFile();
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"Error storing new saved slot names - {(type ? "position" : "rotation")}\n" + ex.ToString()); }
        }

    }
}
