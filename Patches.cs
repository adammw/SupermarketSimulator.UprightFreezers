using HarmonyLib;
using MyBox;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UprightFreezers
{
    [HarmonyPatch]
    public class Patches
    {
        public const int FRIDGE_A_ID = 2;
        public const int FRIDGE_B_ID = 3;
        public const int FREEZER_ID = 4;
        public const int STANDING_FREEZER_A_ID = 542201;
        public const int STANDING_FREEZER_B_ID = 542202;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(IDManager), "FurnitureSO")]
        public static void Postfix(int id)
        {
            bool isPatched = Singleton<IDManager>.Instance.Furnitures.FindIndex(furniture => furniture.ID == STANDING_FREEZER_A_ID) != -1;

            if (!isPatched)
            {
                AddCustomObjects();
                Plugin.Log.LogInfo("Custom objects added");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocalizationManager), "UpdateLocalization")]
        public static void UpdateLocalizations(ref Dictionary<int, string> ___m_LocalizedFurnitureNames)
        {
            ___m_LocalizedFurnitureNames[STANDING_FREEZER_A_ID] = "Freezer A";
            ___m_LocalizedFurnitureNames[STANDING_FREEZER_B_ID] = "Freezer B";
        }

        private static void AddCustomObjects()
        {
            // Get access to private fields
            //Singleton<IDManager>.Instance.m_Display
            List<DisplaySO> m_Displays = typeof(IDManager).GetField("m_Displays", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<IDManager>.Instance) as List<DisplaySO>;

            // Fetch base game objects
            DisplaySO fridgeA = m_Displays.Find(displaySO => displaySO.ID == FRIDGE_A_ID);
            DisplaySO fridgeB = m_Displays.Find(displaySO => displaySO.ID == FRIDGE_B_ID);
            DisplaySO freezer = m_Displays.Find(displaySO => displaySO.ID == FREEZER_ID);

            // Create custom Freezer A
            DisplaySO freezerA = ScriptableObject.CreateInstance<DisplaySO>();
            freezerA.name = "FreezerA";
            freezerA.LocalizedName = freezer.LocalizedName; // TODO - how to localize?
            freezerA.ID = STANDING_FREEZER_A_ID;
            freezerA.BoxSize = fridgeA.BoxSize;
            freezerA.DisplayType = DisplayType.FREEZER;
            freezerA.FurnitureIcon = fridgeA.FurnitureIcon;
            freezerA.Cost = 600f;

            GameObject freezerAGameObject = Object.Instantiate(fridgeA.FurniturePrefab);
            freezerAGameObject.SetActive(false);
            freezerAGameObject.name = "FreezerA";

            global::Display freezerADisplay = freezerAGameObject.GetComponent<global::Display>();
            freezerADisplay.Data.FurnitureID = STANDING_FREEZER_A_ID;
            
            //display.m_Type = DisplayType.FREEZER;
            typeof(global::Display).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(freezerADisplay, DisplayType.FREEZER);
            typeof(global::Display).GetField("m_ID", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(freezerADisplay, STANDING_FREEZER_A_ID);

            freezerA.FurniturePrefab = freezerAGameObject;
            m_Displays.Add(freezerA);
            Singleton<IDManager>.Instance.Furnitures.Add(freezerA);

            // Create custom Freezer B
            DisplaySO freezerB = ScriptableObject.CreateInstance<DisplaySO>();
            freezerB.name = "FreezerB";
            freezerB.LocalizedName = freezer.LocalizedName; // TODO - how to localize?
            freezerB.ID = STANDING_FREEZER_B_ID;
            freezerB.BoxSize = fridgeB.BoxSize;
            freezerB.DisplayType = DisplayType.FREEZER;
            freezerB.FurnitureIcon = fridgeB.FurnitureIcon;
            freezerB.Cost = 1200f;

            GameObject freezerBGameObject = Object.Instantiate(fridgeB.FurniturePrefab);
            freezerBGameObject.SetActive(false);
            freezerBGameObject.name = "FreezerB";

            global::Display freezerBDisplay = freezerBGameObject.GetComponent<global::Display>();
            freezerBDisplay.Data.FurnitureID = STANDING_FREEZER_B_ID;

            //display.m_Type = DisplayType.FREEZER;
            typeof(global::Display).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(freezerBDisplay, DisplayType.FREEZER);
            typeof(global::Display).GetField("m_ID", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(freezerBDisplay, STANDING_FREEZER_B_ID);

            freezerB.FurniturePrefab = freezerBGameObject;

            m_Displays.Add(freezerB);
            Singleton<IDManager>.Instance.Furnitures.Add(freezerB);

            // Trigger update localizations
            Dictionary<int, string> m_LocalizedFurnitureNames = typeof(LocalizationManager).GetField("m_LocalizedFurnitureNames", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<LocalizationManager>.Instance) as Dictionary<int, string>;
            UpdateLocalizations(ref m_LocalizedFurnitureNames);
        }
    }
}
