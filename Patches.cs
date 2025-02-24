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
        public static void IDManager_Furnitures()
        {
            try
            {
                Traverse idManagerTraverse = Traverse.Create(Singleton<IDManager>.Instance);

                List<DisplaySO> m_Displays = idManagerTraverse.Field("m_Displays").GetValue<List<DisplaySO>>();
                List<FurnitureSO> m_Furnitures = idManagerTraverse.Field("m_Furnitures").GetValue<List<FurnitureSO>>();
                bool isPatched = m_Furnitures.FindIndex(furniture => furniture.ID == STANDING_FREEZER_A_ID) != -1;

                if (!isPatched)
                {
                    DisplaySO fridgeA = m_Displays.Find(displaySO => displaySO.ID == FRIDGE_A_ID);
                    DisplaySO fridgeB = m_Displays.Find(displaySO => displaySO.ID == FRIDGE_B_ID);
                    BuildFreezer(name: "FreezerA", id: STANDING_FREEZER_A_ID, baseDisplay: fridgeA, cost: Plugin.freezerACost.Value, customVisuals: Plugin.customVisualsA.Value, signTexturePath: Plugin.signTextureA.Value);
                    BuildFreezer(name: "FreezerB", id: STANDING_FREEZER_B_ID, baseDisplay: fridgeB, cost: Plugin.freezerBCost.Value, customVisuals: Plugin.customVisualsB.Value, signTexturePath: Plugin.signTextureB.Value);
                    Plugin.Log.LogInfo("Custom objects added");
                }
            }
            catch (System.Exception e)
            {
                Plugin.Log.LogError(e);
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(DisplaySlot), "SpawnProduct")]
        [HarmonyPatch(typeof(DisplaySlot), "AddProduct")]
        public static void AddProduct_Prefix(DisplaySlot __instance, int productID, out GridLayout3D.GridLayout __state)
        {
            __state = null;

            if (__instance.Display.ID == STANDING_FREEZER_A_ID || __instance.Display.ID == STANDING_FREEZER_B_ID)
            {
                ProductSO productSO = Singleton<IDManager>.Instance.ProductSO(productID);
                if (productSO == null) return;

                // save original GridLayoutInStorage
                __state = new GridLayout3D.GridLayout(productSO.GridLayoutInStorage);

                // override scale multiplier for this call
                productSO.GridLayoutInStorage.spacing = Plugin.productScaleMultiplier.Value * productSO.GridLayoutInStorage.spacing;
                productSO.GridLayoutInStorage.scaleMultiplier = Plugin.productScaleMultiplier.Value * productSO.GridLayoutInStorage.scaleMultiplier;

            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DisplaySlot), "SpawnProduct")]
        [HarmonyPatch(typeof(DisplaySlot), "AddProduct")]
        public static void AddProduct_Postfix(DisplaySlot __instance, int productID, GridLayout3D.GridLayout __state)
        {
            if (__instance.Display.ID == STANDING_FREEZER_A_ID || __instance.Display.ID == STANDING_FREEZER_B_ID)
            {
                ProductSO productSO = Singleton<IDManager>.Instance.ProductSO(productID);
                if (productSO == null) return;

                // restore original GridLayoutInStorage
                if (__state != null)
                    productSO.GridLayoutInStorage = __state;
            }
        }



        public static void BuildFreezer(string name, int id, DisplaySO baseDisplay, float cost, bool customVisuals, string signTexturePath)
        {
            // Get access to private fields
            Traverse idManagerTraverse = Traverse.Create(Singleton<IDManager>.Instance);
            List<DisplaySO> m_Displays = idManagerTraverse.Field("m_Displays").GetValue<List<DisplaySO>>();
            List<FurnitureSO> m_Furnitures = idManagerTraverse.Field("m_Furnitures").GetValue<List<FurnitureSO>>();

            // Fetch base game freezer
            DisplaySO freezer = m_Displays.Find(displaySO => displaySO.ID == FREEZER_ID);

            // Create custom Freezer
            DisplaySO customFreezer = ScriptableObject.CreateInstance<DisplaySO>();
            customFreezer.name = name;
            customFreezer.LocalizedName = freezer.LocalizedName; // TODO - how to localize?
            customFreezer.ID = id;
            customFreezer.BoxSize = baseDisplay.BoxSize;
            customFreezer.DisplayType = DisplayType.FREEZER;
            customFreezer.FurnitureIcon = baseDisplay.FurnitureIcon; // TODO - load custom icon, orig is Fridge_Double 
            customFreezer.AtlasIndex = baseDisplay.AtlasIndex; // Atlas icons are used by ProductAtlasManager#SetFurnitureIcon
            customFreezer.AtlasPosition = baseDisplay.AtlasPosition;
            customFreezer.Cost = Plugin.freezerBCost.Value;
            customFreezer.IsMainFurniture = true;

            // Create custom prefab
            GameObject prefab = Object.Instantiate(baseDisplay.FurniturePrefab);
            prefab.SetActive(false);
            prefab.name = name;

            // Add custom visuals
            if (customVisuals)
            {
                Transform visuals = prefab.transform.Find("Visuals");
                Transform mainMesh = visuals.GetChild(0).GetChild(1);
                Material mainMeshMaterial = mainMesh.GetComponent<Renderer>().material;
                // override default texture to be transparent for sign area
                mainMeshMaterial.mainTexture = Plugin.LoadTextureFromResource("TrimSheet_Fridges&Freezers");

                // create a new object to show our custom sign
                Texture2D signBg = (signTexturePath.IsNullOrEmpty()) ? Plugin.LoadTextureFromResource("SignBg") : Plugin.LoadTextureFromFile(signTexturePath);
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.localPosition = new Vector3(0, 1.88f, 0.298f);
                plane.transform.localRotation = Quaternion.Euler(90, 0, 0);
                plane.transform.localScale = new Vector3(0.12f, 0.1f, 0.024f);
                plane.transform.SetParent(visuals);
                plane.SetActive(true);
                plane.GetComponent<Renderer>().material = Material.Instantiate(mainMeshMaterial);
                plane.GetComponent<Renderer>().material.mainTexture = signBg;
                plane.GetComponent<Renderer>().material.SetTexture("_MainTex", signBg);
                plane.GetComponent<Renderer>().material.SetTexture("_EmissionMap", Plugin.LoadTextureFromResource("SignEmissive"));
                plane.GetComponent<Renderer>().material.SetTexture("_BumpMap", null);
                plane.GetComponent<Renderer>().material.SetTexture("_MetallicGlossMap", Texture2D.blackTexture);
            }

            // Set custom prefab display data
            global::Display freezerBDisplay = prefab.GetComponent<global::Display>();
            freezerBDisplay.Data.FurnitureID = id;
            Traverse displayTraverse = Traverse.Create(freezerBDisplay);
            displayTraverse.Field("m_Type").SetValue(DisplayType.FREEZER);
            displayTraverse.Field("m_ID").SetValue(id);

            customFreezer.FurniturePrefab = prefab;

            // Append custom object to lists
            m_Displays.Add(customFreezer);
            m_Furnitures.Add(customFreezer);
        }

    }
}
