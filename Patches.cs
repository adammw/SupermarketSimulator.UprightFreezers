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
        [HarmonyPatch(typeof(IDManager), "Furnitures", MethodType.Getter)]
        public static void Prefix()
        {
            List<FurnitureSO> m_Furnitures = typeof(IDManager).GetField("m_Furnitures", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<IDManager>.Instance) as List<FurnitureSO>;
            bool isPatched = m_Furnitures.FindIndex(furniture => furniture.ID == STANDING_FREEZER_A_ID) != -1;

            if (!isPatched)
            {
                AddCustomObjects();
                Plugin.Log.LogInfo("Custom objects added");
            }
        }

        private static void AddCustomObjects()
        {
            IDManager idManager = Singleton<IDManager>.Instance;

            // Get access to private fields
            List<DisplaySO> m_Displays = typeof(IDManager).GetField("m_Displays", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(idManager) as List<DisplaySO>;
            List<FurnitureSO> m_Furnitures = typeof(IDManager).GetField("m_Furnitures", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(idManager) as List<FurnitureSO>;

            // Fetch base game objects
            DisplaySO fridgeA = m_Displays.Find(displaySO => displaySO.ID == FRIDGE_A_ID);
            DisplaySO fridgeB = m_Displays.Find(displaySO => displaySO.ID == FRIDGE_B_ID);
            DisplaySO freezer = m_Displays.Find(displaySO => displaySO.ID == FREEZER_ID);

            if (fridgeA == null || fridgeB == null || freezer == null)
            {
                
                Plugin.Log.LogError("Base game objects not found");
                return;
            }

            // Create custom Freezer A
            DisplaySO freezerA = ScriptableObject.CreateInstance<DisplaySO>();
            freezerA.name = "FreezerA";
            freezerA.LocalizedName = freezer.LocalizedName; // TODO - how to localize?
            freezerA.ID = STANDING_FREEZER_A_ID;
            freezerA.BoxSize = fridgeA.BoxSize;
            freezerA.DisplayType = DisplayType.FREEZER;
            freezerA.FurnitureIcon = fridgeA.FurnitureIcon; // TODO - load custom icon, orig is Fridge_Single 
            freezerA.AtlasIndex = fridgeA.AtlasIndex; // Atlas icons are used by ProductAtlasManager#SetFurnitureIcon
            freezerA.AtlasPosition = fridgeA.AtlasPosition;
            freezerA.Cost = Plugin.freezerACost.Value;
            freezerA.IsMainFurniture = true;

            GameObject freezerAGameObject = Object.Instantiate(fridgeA.FurniturePrefab);
            freezerAGameObject.SetActive(false);
            freezerAGameObject.name = "FreezerA";
            Renderer freezerARenderer = freezerAGameObject.transform.Find("Visuals/SM_Fridge_Single/SM_Fridge_Single_2").GetComponent<Renderer>();
            freezerARenderer.material.mainTexture = Plugin.LoadTexture("FreezerA");

            global::Display freezerADisplay = freezerAGameObject.GetComponent<global::Display>();
            freezerADisplay.Data.FurnitureID = STANDING_FREEZER_A_ID;
            
            //display.m_Type = DisplayType.FREEZER;
            typeof(global::Display).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(freezerADisplay, DisplayType.FREEZER);
            typeof(global::Display).GetField("m_ID", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(freezerADisplay, STANDING_FREEZER_A_ID);

            freezerA.FurniturePrefab = freezerAGameObject;
            m_Displays.Add(freezerA);
            m_Furnitures.Add(freezerA);

            // Create custom Freezer B
            DisplaySO freezerB = ScriptableObject.CreateInstance<DisplaySO>();
            freezerB.name = "FreezerB";
            freezerB.LocalizedName = freezer.LocalizedName; // TODO - how to localize?
            freezerB.ID = STANDING_FREEZER_B_ID;
            freezerB.BoxSize = fridgeB.BoxSize;
            freezerB.DisplayType = DisplayType.FREEZER;
            freezerB.FurnitureIcon = fridgeB.FurnitureIcon; // TODO - load custom icon, orig is Fridge_Double 
            freezerB.AtlasIndex = fridgeB.AtlasIndex; // Atlas icons are used by ProductAtlasManager#SetFurnitureIcon
            freezerB.AtlasPosition = fridgeB.AtlasPosition;
            freezerB.Cost = Plugin.freezerBCost.Value;
            freezerB.IsMainFurniture = true;

            GameObject freezerBGameObject = Object.Instantiate(fridgeB.FurniturePrefab);
            freezerBGameObject.SetActive(false);
            freezerBGameObject.name = "FreezerB";
            Renderer freezerBRenderer = freezerBGameObject.transform.Find("Visuals/SM_Fridge_Double/SM_Fridge_Double_2").GetComponent<Renderer>();
            freezerBRenderer.material.mainTexture = Plugin.LoadTexture("FreezerA");

            global::Display freezerBDisplay = freezerBGameObject.GetComponent<global::Display>();
            freezerBDisplay.Data.FurnitureID = STANDING_FREEZER_B_ID;

            //display.m_Type = DisplayType.FREEZER;
            typeof(global::Display).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(freezerBDisplay, DisplayType.FREEZER);
            typeof(global::Display).GetField("m_ID", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(freezerBDisplay, STANDING_FREEZER_B_ID);

            freezerB.FurniturePrefab = freezerBGameObject;

            m_Displays.Add(freezerB);
            m_Furnitures.Add(freezerB);
        }
    }
}
