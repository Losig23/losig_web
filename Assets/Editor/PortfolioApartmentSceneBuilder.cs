#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PortfolioApartmentSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/PortfolioApartment.unity";
    private const string MaterialFolder = "Assets/Materials/Apartment";
    private const string InteractableLayerName = "Interactable";

    private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

    [MenuItem("Tools/Portfolio/Build Apartment MVP Scene")]
    public static void BuildApartmentScene()
    {
        EnsureFolders();
        EnsureLayer(InteractableLayerName);
        CreateMaterials();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject root = new GameObject("Portfolio Apartment MVP");
        GameObject structure = ChildRoot(root, "Apartment Structure");
        GameObject furniture = ChildRoot(root, "Furniture And Props");
        GameObject interactables = ChildRoot(root, "Interactables");

        BuildApartmentShell(structure.transform);
        BuildKitchen(furniture.transform);
        BuildLivingRoom(furniture.transform, interactables.transform);
        BuildHallway(furniture.transform);
        BuildBedroom(furniture.transform);
        BuildBathroom(furniture.transform);
        BuildLighting();

        InteractionPromptUI promptUI = BuildPromptUI();
        BuildPlayer(promptUI);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Built portfolio apartment MVP scene at {ScenePath}");
    }

    private static void EnsureFolders()
    {
        EnsureFolder("Assets", "Editor");
        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets/Materials", "Apartment");
        EnsureFolder("Assets", "Scenes");
        EnsureFolder("Assets", "StreamingAssets");
        EnsureFolder("Assets", "Prefabs");
        EnsureFolder("Assets", "Models");
        EnsureFolder("Assets", "UI");
        EnsureFolder("Assets", "Scripts");
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = $"{parent}/{child}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, child);
        }
    }

    private static GameObject ChildRoot(GameObject parent, string name)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent.transform);
        return child;
    }

    private static void CreateMaterials()
    {
        Materials.Clear();
        AddMaterial("WallCream", new Color(0.78f, 0.75f, 0.65f), 0.2f);
        AddMaterial("CeilingPopcorn", new Color(0.70f, 0.68f, 0.61f), 0.1f);
        AddMaterial("TrimWhite", new Color(0.88f, 0.86f, 0.78f), 0.25f);
        AddMaterial("CarpetTaupe", new Color(0.42f, 0.36f, 0.29f), 0.05f);
        AddMaterial("CarpetLightFleck", new Color(0.62f, 0.56f, 0.47f), 0.05f);
        AddMaterial("WoodFloor", new Color(0.33f, 0.27f, 0.23f), 0.35f);
        AddMaterial("DarkDoorWood", new Color(0.13f, 0.08f, 0.04f), 0.25f);
        AddMaterial("CabinetWood", new Color(0.62f, 0.36f, 0.13f), 0.35f);
        AddMaterial("Countertop", new Color(0.75f, 0.72f, 0.66f), 0.45f);
        AddMaterial("BlackMetal", new Color(0.02f, 0.02f, 0.02f), 0.5f);
        AddMaterial("DarkFabric", new Color(0.04f, 0.05f, 0.08f), 0.2f);
        AddMaterial("RedChairFabric", new Color(0.55f, 0.09f, 0.10f), 0.25f);
        AddMaterial("WhiteAppliance", new Color(0.86f, 0.85f, 0.78f), 0.25f);
        AddMaterial("MonitorScreen", new Color(0.01f, 0.012f, 0.018f), 0.7f);
        AddMaterial("GlassBlue", new Color(0.54f, 0.78f, 0.95f, 0.35f), 0.2f);
        AddMaterial("Brass", new Color(0.95f, 0.65f, 0.21f), 0.6f);
        AddMaterial("PlantGreen", new Color(0.07f, 0.32f, 0.12f), 0.2f);
        AddMaterial("Cardboard", new Color(0.50f, 0.34f, 0.18f), 0.15f);
        AddMaterial("BathroomFloor", new Color(0.14f, 0.12f, 0.10f), 0.3f);
        AddMaterial("BeddingWhite", new Color(0.86f, 0.84f, 0.78f), 0.2f);
        AddMaterial("BlanketPattern", new Color(0.14f, 0.18f, 0.20f), 0.2f);
    }

    private static void AddMaterial(string name, Color color, float smoothness)
    {
        string path = $"{MaterialFolder}/{name}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (material == null)
        {
            material = new Material(FindLitShader());
            AssetDatabase.CreateAsset(material, path);
        }

        material.shader = FindLitShader();
        SetMaterialColor(material, color);

        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", smoothness);
        }

        if (color.a < 1f)
        {
            ConfigureTransparent(material);
        }

        EditorUtility.SetDirty(material);
        Materials[name] = material;
    }

    private static Shader FindLitShader()
    {
        return Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
    }

    private static void SetMaterialColor(Material material, Color color)
    {
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        else if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
    }

    private static void ConfigureTransparent(Material material)
    {
        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1f);
        }

        material.SetOverrideTag("RenderType", "Transparent");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }

    private static void BuildApartmentShell(Transform parent)
    {
        const float wallHeight = 2.8f;
        const float wallThickness = 0.12f;

        CreateCube("Living Room Carpet", parent, new Vector3(0f, -0.035f, 2.65f), new Vector3(11f, 0.07f, 7.7f), Materials["CarpetTaupe"]);
        CreateCube("Hallway Carpet Adjacent To Kitchen", parent, new Vector3(0f, -0.03f, -5.05f), new Vector3(2.4f, 0.06f, 6.9f), Materials["CarpetTaupe"]);
        CreateCube("Big Bedroom Carpet At Hall End", parent, new Vector3(-3.35f, -0.035f, -5.85f), new Vector3(4.3f, 0.07f, 5.3f), Materials["CarpetTaupe"]);
        CreateCube("Kitchen Wood Floor Right Of Entry", parent, new Vector3(3.35f, -0.02f, -4.85f), new Vector3(4.3f, 0.04f, 7.3f), Materials["WoodFloor"]);
        CreateCube("Bathroom Dark Vinyl Floor", parent, new Vector3(-3.35f, -0.015f, -2.2f), new Vector3(4.3f, 0.05f, 2.0f), Materials["BathroomFloor"]);

        AddCarpetFlecks(parent);

        WallZ("West Exterior Wall", parent, -5.5f, -8.5f, 6.5f, wallHeight, wallThickness);
        WallZ("East Exterior Wall", parent, 5.5f, -8.5f, 6.5f, wallHeight, wallThickness);
        WallX("Entry Front Wall Left Of Door", parent, -5.5f, -0.75f, -8.5f, wallHeight, wallThickness);
        WallX("Entry Front Wall Right Of Door And Kitchen Side", parent, 0.75f, 5.5f, -8.5f, wallHeight, wallThickness);

        WallX("Balcony Wall Far Left", parent, -5.5f, -4.5f, 6.5f, wallHeight, wallThickness);
        WallX("Balcony Wall Between Window And Door", parent, -2.1f, -1.75f, 6.5f, wallHeight, wallThickness);
        WallX("Balcony Wall Far Right", parent, 1.25f, 5.5f, 6.5f, wallHeight, wallThickness);

        WallZ("Hall Left Wall Segment A Bedroom Side", parent, -1.2f, -8.5f, -6.6f, wallHeight, wallThickness);
        WallZ("Hall Left Wall Segment B Bedroom Bath Side", parent, -1.2f, -5.35f, -2.85f, wallHeight, wallThickness);
        WallZ("Hall Left Wall Segment C Bath Living Side", parent, -1.2f, -1.75f, -1.2f, wallHeight, wallThickness);
        WallZ("Hall Right Wall Segment A Kitchen Side", parent, 1.2f, -8.5f, -6.6f, wallHeight, wallThickness);
        WallZ("Hall Right Wall Segment B Kitchen Side", parent, 1.2f, -5.35f, -2.85f, wallHeight, wallThickness);
        WallZ("Hall Right Wall Segment C Living Side", parent, 1.2f, -1.75f, -1.2f, wallHeight, wallThickness);
        WallX("Big Bedroom Bathroom Divider", parent, -5.5f, -1.2f, -3.2f, wallHeight, wallThickness);
        WallX("Bathroom Living Divider", parent, -5.5f, -1.2f, -1.2f, wallHeight, wallThickness);

        CreateCube("Entry Support Column Beside Kitchen", parent, new Vector3(1.2f, wallHeight * 0.5f, -6.8f), new Vector3(0.42f, wallHeight, 0.42f), Materials["WallCream"]);
        CreateCube("Kitchen Soffit Column", parent, new Vector3(1.2f, wallHeight * 0.5f, -1.2f), new Vector3(0.42f, wallHeight, 0.42f), Materials["WallCream"]);

        CreateCube("Main Ceiling With Popcorn Texture Color", parent, new Vector3(0f, 2.82f, -1f), new Vector3(11.2f, 0.08f, 15.1f), Materials["CeilingPopcorn"], false);
        AddCeilingSpeckles(parent);
        AddBaseboards(parent);

        CreateDoor("Dark Front Door Left Of Kitchen", parent, new Vector3(0f, 1.05f, -8.56f), new Vector3(1.3f, 2.1f, 0.08f), Quaternion.identity);
        CreateDoor("Big Bedroom Door At Hall End", parent, new Vector3(-1.14f, 1.05f, -6.0f), new Vector3(0.08f, 2.1f, 1.05f), Quaternion.identity);
        CreateDoor("Bathroom Door", parent, new Vector3(-1.14f, 1.05f, -2.3f), new Vector3(0.08f, 2.1f, 1.0f), Quaternion.Euler(0f, -25f, 0f));

        AddDoorFrame(parent, new Vector3(0f, 1.05f, -8.58f), 1.42f, 2.22f, true);
        AddDoorFrame(parent, new Vector3(-1.18f, 1.05f, -6f), 1.12f, 2.18f, false);
        AddDoorFrame(parent, new Vector3(-1.18f, 1.05f, -2.3f), 1.08f, 2.18f, false);

        AddBalconyGlass(parent);
        AddBedroomWindow(parent);
    }

    private static void BuildKitchen(Transform parent)
    {
        CreateCube("White Refrigerator Right Of Entry", parent, new Vector3(5.05f, 0.95f, -6.35f), new Vector3(0.75f, 1.9f, 1.05f), Materials["WhiteAppliance"]);
        CreateCube("Refrigerator Door Handles", parent, new Vector3(4.64f, 1.0f, -6.35f), new Vector3(0.04f, 1.35f, 0.06f), Materials["TrimWhite"]);

        CreateCube("Kitchen Lower Cabinets On Right Wall", parent, new Vector3(4.9f, 0.45f, -3.6f), new Vector3(0.9f, 0.9f, 3.4f), Materials["CabinetWood"]);
        CreateCube("Kitchen Countertop", parent, new Vector3(4.82f, 0.94f, -3.6f), new Vector3(1.05f, 0.08f, 3.55f), Materials["Countertop"]);
        CreateCube("Kitchen Sink Basin", parent, new Vector3(4.25f, 0.99f, -3.15f), new Vector3(0.42f, 0.05f, 0.62f), Materials["BlackMetal"]);
        CreateCylinder("Kitchen Faucet", parent, new Vector3(4.25f, 1.18f, -3.48f), 0.04f, 0.34f, Materials["Brass"]);

        CreateCube("Upper Cabinet Left", parent, new Vector3(4.95f, 1.95f, -4.35f), new Vector3(0.78f, 0.75f, 1.25f), Materials["CabinetWood"]);
        CreateCube("Upper Cabinet Right", parent, new Vector3(4.95f, 1.95f, -2.75f), new Vector3(0.78f, 0.75f, 1.25f), Materials["CabinetWood"]);
        CreateCube("Fluorescent Kitchen Light", parent, new Vector3(3.4f, 2.73f, -4.35f), new Vector3(1.65f, 0.05f, 0.28f), Materials["WhiteAppliance"], false);

        CreateCylinder("Round Dining Table Top", parent, new Vector3(2.8f, 0.76f, -2.2f), 0.85f, 0.09f, Materials["CabinetWood"]);
        CreateCylinder("Round Dining Table Pedestal", parent, new Vector3(2.8f, 0.38f, -2.2f), 0.09f, 0.76f, Materials["CabinetWood"]);
        CreateCube("Red Dining Chair Back", parent, new Vector3(2.2f, 0.9f, -1.35f), new Vector3(0.75f, 0.95f, 0.12f), Materials["RedChairFabric"]);
        CreateCube("Red Dining Chair Seat", parent, new Vector3(2.2f, 0.5f, -1.75f), new Vector3(0.72f, 0.14f, 0.72f), Materials["RedChairFabric"]);
        AddChairLegs(parent, new Vector3(2.2f, 0.25f, -1.75f), 0.62f, 0.62f, 0.5f, Materials["CabinetWood"]);

        CreateCube("Toaster Oven Placeholder", parent, new Vector3(2.85f, 0.92f, -2.2f), new Vector3(0.62f, 0.36f, 0.42f), Materials["WhiteAppliance"]);
        CreateCube("Pizza Box Placeholder", parent, new Vector3(2.55f, 0.98f, -1.8f), new Vector3(0.85f, 0.04f, 0.42f), Materials["Cardboard"]);
        CreateCylinder("Black Trash Can", parent, new Vector3(4.95f, 0.43f, -7.55f), 0.32f, 0.86f, Materials["BlackMetal"]);
        AddBroom(parent, new Vector3(5.18f, 0.9f, -7.75f));
    }

    private static void BuildLivingRoom(Transform parent, Transform interactables)
    {
        CreateCube("Desk Surface", parent, new Vector3(-4.72f, 0.76f, 2.15f), new Vector3(1.4f, 0.11f, 0.74f), Materials["CabinetWood"]);
        CreateCube("Desk Left Leg", parent, new Vector3(-5.25f, 0.38f, 1.85f), new Vector3(0.08f, 0.76f, 0.08f), Materials["BlackMetal"]);
        CreateCube("Desk Right Leg", parent, new Vector3(-4.2f, 0.38f, 1.85f), new Vector3(0.08f, 0.76f, 0.08f), Materials["BlackMetal"]);
        CreateCube("Desk Back Left Leg", parent, new Vector3(-5.25f, 0.38f, 2.45f), new Vector3(0.08f, 0.76f, 0.08f), Materials["BlackMetal"]);
        CreateCube("Desk Back Right Leg", parent, new Vector3(-4.2f, 0.38f, 2.45f), new Vector3(0.08f, 0.76f, 0.08f), Materials["BlackMetal"]);

        GameObject computer = new GameObject("Resume Computer");
        computer.transform.SetParent(interactables);
        computer.transform.position = Vector3.zero;
        computer.AddComponent<ResumeComputerInteractable>();
        SetLayerRecursively(computer, LayerMask.NameToLayer(InteractableLayerName));

        Transform computerRoot = computer.transform;
        CreateCube("Monitor Screen - Press E Resume Target", computerRoot, new Vector3(-5.02f, 1.28f, 2.15f), new Vector3(0.06f, 0.48f, 0.82f), Materials["MonitorScreen"]);
        CreateCube("Monitor Frame", computerRoot, new Vector3(-5.06f, 1.28f, 2.15f), new Vector3(0.04f, 0.58f, 0.92f), Materials["BlackMetal"], false);
        CreateCube("Monitor Stand", computerRoot, new Vector3(-4.96f, 0.98f, 2.15f), new Vector3(0.08f, 0.32f, 0.08f), Materials["BlackMetal"]);
        CreateCube("Monitor Base", computerRoot, new Vector3(-4.86f, 0.84f, 2.15f), new Vector3(0.28f, 0.05f, 0.44f), Materials["BlackMetal"]);
        SetLayerRecursively(computer, LayerMask.NameToLayer(InteractableLayerName));

        PlacePrefabFitted(
            "Assets/Laptop/laptop.prefab",
            "Laptop Asset On Desk",
            parent,
            new Vector3(-4.25f, 0.86f, 1.9f),
            new Vector3(0.55f, 0.12f, 0.38f),
            Quaternion.Euler(0f, 170f, 0f),
            Materials["BlackMetal"]);

        CreateCube("Visitor Chair Seat", parent, new Vector3(-3.25f, 0.46f, 2.0f), new Vector3(0.65f, 0.12f, 0.65f), Materials["DarkFabric"]);
        CreateCube("Visitor Chair Back", parent, new Vector3(-3.25f, 0.95f, 2.3f), new Vector3(0.66f, 0.9f, 0.09f), Materials["DarkFabric"]);
        AddChairLegs(parent, new Vector3(-3.25f, 0.23f, 2.0f), 0.55f, 0.55f, 0.46f, Materials["BlackMetal"]);

        CreateCube("Black Futon Couch Base", parent, new Vector3(4.65f, 0.43f, 2.8f), new Vector3(1.25f, 0.35f, 2.5f), Materials["DarkFabric"]);
        CreateCube("Black Futon Couch Back", parent, new Vector3(5.12f, 0.94f, 2.8f), new Vector3(0.25f, 1.0f, 2.5f), Materials["DarkFabric"]);
        CreateCube("Laptop On Futon Placeholder", parent, new Vector3(4.25f, 0.73f, 3.2f), new Vector3(0.52f, 0.04f, 0.36f), Materials["BlackMetal"]);

        CreateCylinder("Standing Lamp Pole", parent, new Vector3(4.9f, 1.35f, 4.95f), 0.035f, 2.35f, Materials["BlackMetal"]);
        CreateSphere("Standing Lamp Shade Top", parent, new Vector3(4.7f, 2.35f, 4.8f), new Vector3(0.35f, 0.35f, 0.35f), Materials["TrimWhite"]);
        CreateSphere("Standing Lamp Shade Mid", parent, new Vector3(5.03f, 1.95f, 4.9f), new Vector3(0.30f, 0.30f, 0.30f), Materials["TrimWhite"]);

        CreateCube("Wall AC Unit", parent, new Vector3(-4.75f, 2.02f, 6.23f), new Vector3(1.2f, 0.55f, 0.18f), Materials["WhiteAppliance"]);
        AddVentLines(parent, new Vector3(-4.75f, 2.02f, 6.12f), 1.0f, 0.34f, 6, true);

        PlacePrefabFitted(
            "Assets/AssetsStore/Furniture/Assets/Prefabs/rack.prefab",
            "Shoe Rack Asset",
            parent,
            new Vector3(4.85f, 0.65f, -7.0f),
            new Vector3(0.75f, 1.1f, 1.45f),
            Quaternion.identity,
            Materials["BlackMetal"]);
        AddShoeRackFallbackDetails(parent);
    }

    private static void BuildHallway(Transform parent)
    {
        AddPlant(parent, new Vector3(-0.95f, 0f, -6.8f));
        CreateCube("Wall Thermostat", parent, new Vector3(-1.27f, 1.35f, -6.15f), new Vector3(0.05f, 0.22f, 0.25f), Materials["TrimWhite"]);
        CreateSphere("Hall Ceiling Light One", parent, new Vector3(0f, 2.63f, -5.8f), new Vector3(0.42f, 0.18f, 0.42f), Materials["TrimWhite"]);
        CreateSphere("Hall Ceiling Light Two", parent, new Vector3(0f, 2.63f, -3.3f), new Vector3(0.34f, 0.14f, 0.34f), Materials["TrimWhite"]);
    }

    private static void BuildBedroom(Transform parent)
    {
        PlacePrefabFitted(
            "Assets/AssetsStore/Furniture/Assets/Prefabs/bed.prefab",
            "Bedroom Bed Asset",
            parent,
            new Vector3(-3.65f, 0.42f, -7.05f),
            new Vector3(2.5f, 0.85f, 2.1f),
            Quaternion.Euler(0f, 90f, 0f),
            Materials["BeddingWhite"]);

        CreateCube("Bed Placeholder Mattress", parent, new Vector3(-3.65f, 0.58f, -7.05f), new Vector3(2.4f, 0.35f, 2.0f), Materials["BeddingWhite"]);
        CreateCube("Dark Bed Base", parent, new Vector3(-3.65f, 0.25f, -7.05f), new Vector3(2.55f, 0.5f, 2.15f), Materials["DarkFabric"]);
        CreateCube("Metal Headboard", parent, new Vector3(-3.65f, 1.0f, -8.05f), new Vector3(2.55f, 0.75f, 0.08f), Materials["Brass"]);
        CreateCube("Blanket Pile", parent, new Vector3(-3.5f, 0.86f, -6.75f), new Vector3(1.2f, 0.16f, 0.6f), Materials["BlanketPattern"]);
        CreateCube("Pillow Left", parent, new Vector3(-4.2f, 0.86f, -7.82f), new Vector3(0.72f, 0.16f, 0.36f), Materials["BeddingWhite"]);
        CreateCube("Pillow Right", parent, new Vector3(-3.0f, 0.86f, -7.82f), new Vector3(0.72f, 0.16f, 0.36f), Materials["BeddingWhite"]);

        CreateCube("Bedroom Moving Box A", parent, new Vector3(-2.0f, 0.28f, -4.35f), new Vector3(0.75f, 0.55f, 0.65f), Materials["Cardboard"]);
        CreateCube("Bedroom Moving Box B", parent, new Vector3(-4.95f, 0.25f, -4.1f), new Vector3(0.65f, 0.5f, 0.55f), Materials["Cardboard"]);
        CreateCube("Bedroom Side Table", parent, new Vector3(-4.95f, 0.38f, -7.75f), new Vector3(0.55f, 0.75f, 0.5f), Materials["CabinetWood"]);
    }

    private static void BuildBathroom(Transform parent)
    {
        CreateCube("Bathroom Vanity", parent, new Vector3(-4.95f, 0.45f, -2.3f), new Vector3(0.75f, 0.9f, 1.1f), Materials["WhiteAppliance"]);
        CreateCube("Bathroom Countertop", parent, new Vector3(-4.95f, 0.95f, -2.3f), new Vector3(0.82f, 0.08f, 1.16f), Materials["Countertop"]);
        CreateCube("Bathroom Mirror", parent, new Vector3(-5.42f, 1.65f, -2.3f), new Vector3(0.04f, 0.9f, 0.72f), Materials["GlassBlue"]);

        CreateCylinder("Toilet Bowl", parent, new Vector3(-3.2f, 0.42f, -1.55f), 0.34f, 0.36f, Materials["WhiteAppliance"]);
        CreateCube("Toilet Tank", parent, new Vector3(-3.2f, 0.75f, -1.18f), new Vector3(0.75f, 0.5f, 0.2f), Materials["WhiteAppliance"]);
        CreateCylinder("Small Bathroom Trash Can", parent, new Vector3(-2.3f, 0.26f, -1.55f), 0.22f, 0.52f, Materials["BlackMetal"]);

        CreateCube("Towel Bar", parent, new Vector3(-2.8f, 1.35f, -3.13f), new Vector3(1.1f, 0.04f, 0.04f), Materials["Brass"]);
        CreateCube("Hanging Towel", parent, new Vector3(-2.8f, 1.05f, -3.1f), new Vector3(0.85f, 0.55f, 0.05f), Materials["CarpetLightFleck"]);
        AddBroom(parent, new Vector3(-4.45f, 0.85f, -1.45f));
    }

    private static void BuildLighting()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.68f, 0.72f, 0.78f);
        RenderSettings.ambientEquatorColor = new Color(0.45f, 0.42f, 0.35f);
        RenderSettings.ambientGroundColor = new Color(0.30f, 0.27f, 0.22f);

        GameObject sun = new GameObject("Directional Sunlight From Balcony");
        Light sunLight = sun.AddComponent<Light>();
        sunLight.type = LightType.Directional;
        sunLight.intensity = 1.15f;
        sun.transform.rotation = Quaternion.Euler(45f, -25f, 0f);

        AddPointLight("Warm Kitchen Light", new Vector3(3.5f, 2.35f, -3.6f), 3.5f, 1.1f);
        AddPointLight("Living Room Window Glow", new Vector3(-1f, 2.0f, 5.8f), 5.5f, 1.0f);
        AddPointLight("Hall Light", new Vector3(0f, 2.35f, -4.7f), 3.2f, 0.85f);
        AddPointLight("Bedroom Window Fill", new Vector3(-3.7f, 2.0f, -8.0f), 4.5f, 0.8f);
    }

    private static void AddPointLight(string name, Vector3 position, float range, float intensity)
    {
        GameObject lightObject = new GameObject(name);
        lightObject.transform.position = position;
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.92f, 0.78f);
        light.range = range;
        light.intensity = intensity;
    }

    private static InteractionPromptUI BuildPromptUI()
    {
        GameObject canvasObject = new GameObject("Interaction Prompt Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject promptRoot = new GameObject("Prompt Root");
        promptRoot.transform.SetParent(canvasObject.transform, false);
        RectTransform promptRect = promptRoot.AddComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0f);
        promptRect.anchorMax = new Vector2(0.5f, 0f);
        promptRect.pivot = new Vector2(0.5f, 0f);
        promptRect.anchoredPosition = new Vector2(0f, 80f);
        promptRect.sizeDelta = new Vector2(560f, 72f);
        Image promptBackground = promptRoot.AddComponent<Image>();
        promptBackground.color = new Color(0f, 0f, 0f, 0.62f);

        GameObject textObject = new GameObject("Prompt Text");
        textObject.transform.SetParent(promptRoot.transform, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(18f, 8f);
        textRect.offsetMax = new Vector2(-18f, -8f);
        Text text = textObject.AddComponent<Text>();
        text.text = "Press E to view resume";
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 28;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        InteractionPromptUI promptUI = canvasObject.AddComponent<InteractionPromptUI>();
        SerializedObject promptSerialized = new SerializedObject(promptUI);
        promptSerialized.FindProperty("promptRoot").objectReferenceValue = promptRoot;
        promptSerialized.FindProperty("promptText").objectReferenceValue = text;
        promptSerialized.ApplyModifiedProperties();

        promptRoot.SetActive(false);
        return promptUI;
    }

    private static void BuildPlayer(InteractionPromptUI promptUI)
    {
        GameObject starterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Starter Assets/Runtime/FirstPersonController/Prefabs/NestedParent_Unpack.prefab");

        GameObject playerRoot;

        if (starterPrefab != null)
        {
            playerRoot = PrefabUtility.InstantiatePrefab(starterPrefab) as GameObject;
            playerRoot.name = "First Person Player";
        }
        else
        {
            playerRoot = CreateFallbackPlayer();
        }

        Transform playerCapsule = FindDeepChild(playerRoot.transform, "PlayerCapsule") ?? playerRoot.transform;
        playerCapsule.position = new Vector3(0f, 0.05f, -7.35f);
        playerCapsule.rotation = Quaternion.Euler(0f, 0f, 0f);

        Camera playerCamera = Object.FindFirstObjectByType<Camera>();
        if (playerCamera == null)
        {
            GameObject cameraObject = new GameObject("MainCamera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(playerCapsule);
            cameraObject.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            cameraObject.transform.localRotation = Quaternion.identity;
            playerCamera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        playerCamera.tag = "MainCamera";

        PlayerInteractor interactor = playerCapsule.GetComponent<PlayerInteractor>();
        if (interactor == null)
        {
            interactor = playerCapsule.gameObject.AddComponent<PlayerInteractor>();
        }

        SerializedObject interactorSerialized = new SerializedObject(interactor);
        interactorSerialized.FindProperty("playerCamera").objectReferenceValue = playerCamera;
        interactorSerialized.FindProperty("promptUI").objectReferenceValue = promptUI;
        interactorSerialized.FindProperty("interactionRange").floatValue = 3.25f;
        interactorSerialized.FindProperty("interactableLayers").intValue = ~0;
        interactorSerialized.ApplyModifiedProperties();

    }

    private static GameObject CreateFallbackPlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Fallback First Person Player";
        player.transform.position = new Vector3(0f, 0.05f, -7.35f);
        player.AddComponent<CharacterController>();
        return player;
    }

    private static void AddBalconyGlass(Transform parent)
    {
        CreateCube("Balcony Window Glass", parent, new Vector3(-3.25f, 1.35f, 6.45f), new Vector3(1.9f, 1.85f, 0.04f), Materials["GlassBlue"]);
        CreateCube("Balcony Door Glass", parent, new Vector3(-0.25f, 1.25f, 6.45f), new Vector3(1.85f, 2.2f, 0.04f), Materials["GlassBlue"]);
        CreateCube("Balcony Door Dark Frame Left", parent, new Vector3(-1.2f, 1.25f, 6.42f), new Vector3(0.08f, 2.25f, 0.08f), Materials["DarkDoorWood"]);
        CreateCube("Balcony Door Dark Frame Right", parent, new Vector3(0.72f, 1.25f, 6.42f), new Vector3(0.08f, 2.25f, 0.08f), Materials["DarkDoorWood"]);
        CreateCube("Balcony Door Dark Frame Top", parent, new Vector3(-0.25f, 2.36f, 6.42f), new Vector3(1.95f, 0.08f, 0.08f), Materials["DarkDoorWood"]);
        CreateSphere("Balcony Door Brass Knob", parent, new Vector3(0.58f, 1.05f, 6.35f), new Vector3(0.12f, 0.12f, 0.12f), Materials["Brass"]);
        AddVerticalBlinds(parent, -4.1f, 0.65f, 6.30f, 1.45f, 2.05f, 18);
        AddOutsideView(parent, new Vector3(-0.7f, 1.1f, 6.8f), new Vector3(5.2f, 2.4f, 0.04f));
    }

    private static void AddBedroomWindow(Transform parent)
    {
        CreateCube("Bedroom Window Glass", parent, new Vector3(-3.6f, 1.45f, -8.44f), new Vector3(2.45f, 1.05f, 0.04f), Materials["GlassBlue"]);
        CreateCube("Bedroom Window Frame Top", parent, new Vector3(-3.6f, 2.0f, -8.4f), new Vector3(2.55f, 0.06f, 0.08f), Materials["TrimWhite"]);
        CreateCube("Bedroom Window Frame Bottom", parent, new Vector3(-3.6f, 0.88f, -8.4f), new Vector3(2.55f, 0.06f, 0.08f), Materials["TrimWhite"]);
        AddHorizontalBlinds(parent, -4.75f, -2.45f, -8.32f, 1.45f, 0.95f, 9);
        AddOutsideView(parent, new Vector3(-3.6f, 1.42f, -8.75f), new Vector3(2.6f, 1.35f, 0.04f));
    }

    private static void AddOutsideView(Transform parent, Vector3 position, Vector3 size)
    {
        GameObject sky = CreateCube("Simple Exterior View", parent, position, size, Materials["GlassBlue"], false);
        CreateCube("Exterior Grass Band", sky.transform, position + new Vector3(0f, -0.55f, -0.04f), new Vector3(size.x, 0.28f, 0.03f), Materials["PlantGreen"], false);
        CreateCube("Exterior Apartment Shape", sky.transform, position + new Vector3(0f, 0f, -0.06f), new Vector3(size.x * 0.42f, size.y * 0.42f, 0.03f), Materials["TrimWhite"], false);
    }

    private static void AddVerticalBlinds(Transform parent, float minX, float maxX, float z, float centerY, float height, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float t = count <= 1 ? 0f : i / (count - 1f);
            float x = Mathf.Lerp(minX, maxX, t);
            CreateCube($"Vertical Blind Slat {i + 1:00}", parent, new Vector3(x, centerY, z), new Vector3(0.035f, height, 0.035f), Materials["TrimWhite"], false);
        }
    }

    private static void AddHorizontalBlinds(Transform parent, float minX, float maxX, float z, float centerY, float height, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float t = count <= 1 ? 0f : i / (count - 1f);
            float y = centerY - height * 0.5f + height * t;
            CreateCube($"Bedroom Horizontal Blind {i + 1:00}", parent, new Vector3((minX + maxX) * 0.5f, y, z), new Vector3(maxX - minX, 0.025f, 0.035f), Materials["TrimWhite"], false);
        }
    }

    private static void AddBaseboards(Transform parent)
    {
        Material trim = Materials["TrimWhite"];
        CreateCube("West Baseboard", parent, new Vector3(-5.43f, 0.08f, -1f), new Vector3(0.07f, 0.16f, 15f), trim, false);
        CreateCube("East Baseboard", parent, new Vector3(5.43f, 0.08f, -1f), new Vector3(0.07f, 0.16f, 15f), trim, false);
        CreateCube("Front Baseboard Left", parent, new Vector3(-3.1f, 0.08f, -8.43f), new Vector3(4.7f, 0.16f, 0.07f), trim, false);
        CreateCube("Front Baseboard Right", parent, new Vector3(3.1f, 0.08f, -8.43f), new Vector3(4.7f, 0.16f, 0.07f), trim, false);
        CreateCube("Balcony Baseboard", parent, new Vector3(3.4f, 0.08f, 6.43f), new Vector3(4.1f, 0.16f, 0.07f), trim, false);
    }

    private static void AddCarpetFlecks(Transform parent)
    {
        for (int i = 0; i < 85; i++)
        {
            float x = Mathf.Lerp(-5.2f, 5.2f, Halton(i + 1, 2));
            float z = Mathf.Lerp(-8.1f, 6.0f, Halton(i + 3, 3));
            if (x < -1.25f && z < -1.2f)
            {
                continue;
            }

            if (x > 1.2f && z > -3.2f && z < -1.2f)
            {
                continue;
            }

            CreateCube($"Carpet Fleck {i:00}", parent, new Vector3(x, 0.012f, z), new Vector3(0.12f, 0.01f, 0.025f), Materials["CarpetLightFleck"], false);
        }
    }

    private static void AddCeilingSpeckles(Transform parent)
    {
        for (int i = 0; i < 120; i++)
        {
            float x = Mathf.Lerp(-5.2f, 5.2f, Halton(i + 5, 2));
            float z = Mathf.Lerp(-8.1f, 6.2f, Halton(i + 7, 3));
            CreateCube($"Popcorn Ceiling Speckle {i:000}", parent, new Vector3(x, 2.765f, z), new Vector3(0.045f, 0.012f, 0.045f), Materials["TrimWhite"], false);
        }
    }

    private static float Halton(int index, int basis)
    {
        float result = 0f;
        float fraction = 1f / basis;

        while (index > 0)
        {
            result += fraction * (index % basis);
            index = Mathf.FloorToInt(index / (float)basis);
            fraction /= basis;
        }

        return result;
    }

    private static void AddDoorFrame(Transform parent, Vector3 center, float width, float height, bool wallAlongX)
    {
        if (wallAlongX)
        {
            CreateCube("Door Frame Top", parent, center + new Vector3(0f, height * 0.5f, 0f), new Vector3(width, 0.08f, 0.12f), Materials["DarkDoorWood"], false);
            CreateCube("Door Frame Left", parent, center + new Vector3(-width * 0.5f, 0f, 0f), new Vector3(0.08f, height, 0.12f), Materials["DarkDoorWood"], false);
            CreateCube("Door Frame Right", parent, center + new Vector3(width * 0.5f, 0f, 0f), new Vector3(0.08f, height, 0.12f), Materials["DarkDoorWood"], false);
        }
        else
        {
            CreateCube("Door Frame Top", parent, center + new Vector3(0f, height * 0.5f, 0f), new Vector3(0.12f, 0.08f, width), Materials["DarkDoorWood"], false);
            CreateCube("Door Frame Left", parent, center + new Vector3(0f, 0f, -width * 0.5f), new Vector3(0.12f, height, 0.08f), Materials["DarkDoorWood"], false);
            CreateCube("Door Frame Right", parent, center + new Vector3(0f, 0f, width * 0.5f), new Vector3(0.12f, height, 0.08f), Materials["DarkDoorWood"], false);
        }
    }

    private static void AddChairLegs(Transform parent, Vector3 center, float width, float depth, float height, Material material)
    {
        float y = center.y;
        float x = width * 0.42f;
        float z = depth * 0.42f;
        CreateCube("Chair Leg FL", parent, new Vector3(center.x - x, y, center.z - z), new Vector3(0.06f, height, 0.06f), material);
        CreateCube("Chair Leg FR", parent, new Vector3(center.x + x, y, center.z - z), new Vector3(0.06f, height, 0.06f), material);
        CreateCube("Chair Leg BL", parent, new Vector3(center.x - x, y, center.z + z), new Vector3(0.06f, height, 0.06f), material);
        CreateCube("Chair Leg BR", parent, new Vector3(center.x + x, y, center.z + z), new Vector3(0.06f, height, 0.06f), material);
    }

    private static void AddBroom(Transform parent, Vector3 basePosition)
    {
        CreateCube("Broom Handle", parent, basePosition + new Vector3(0f, 0.6f, 0f), new Vector3(0.04f, 1.2f, 0.04f), Materials["BlackMetal"]);
        CreateCube("Broom Brush", parent, basePosition + new Vector3(0f, 0.08f, 0f), new Vector3(0.38f, 0.16f, 0.08f), Materials["CabinetWood"]);
    }

    private static void AddPlant(Transform parent, Vector3 basePosition)
    {
        CreateCylinder("Black Plant Pot", parent, basePosition + new Vector3(0f, 0.23f, 0f), 0.28f, 0.45f, Materials["BlackMetal"]);

        for (int i = 0; i < 7; i++)
        {
            float angle = i * 51f;
            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * new Vector3(0.16f, 0.6f, 0f);
            GameObject leaf = CreateCube($"Snake Plant Leaf {i + 1}", parent, basePosition + offset, new Vector3(0.06f, 0.9f, 0.18f), Materials["PlantGreen"]);
            leaf.transform.rotation = Quaternion.Euler(12f, angle, 0f);
        }
    }

    private static void AddVentLines(Transform parent, Vector3 center, float width, float height, int count, bool horizontal)
    {
        for (int i = 0; i < count; i++)
        {
            float t = count <= 1 ? 0f : i / (count - 1f);
            if (horizontal)
            {
                float y = center.y - height * 0.5f + height * t;
                CreateCube($"AC Vent Line {i + 1}", parent, new Vector3(center.x, y, center.z), new Vector3(width, 0.025f, 0.025f), Materials["BlackMetal"], false);
            }
            else
            {
                float x = center.x - width * 0.5f + width * t;
                CreateCube($"AC Vent Line {i + 1}", parent, new Vector3(x, center.y, center.z), new Vector3(0.025f, height, 0.025f), Materials["BlackMetal"], false);
            }
        }
    }

    private static void AddShoeRackFallbackDetails(Transform parent)
    {
        for (int i = 0; i < 4; i++)
        {
            CreateCube($"Shoe Pair Placeholder {i + 1}", parent, new Vector3(4.85f, 0.18f + i * 0.22f, -7.45f + i * 0.16f), new Vector3(0.42f, 0.1f, 0.16f), Materials["WhiteAppliance"]);
        }
    }

    private static GameObject CreateDoor(string name, Transform parent, Vector3 position, Vector3 size, Quaternion rotation)
    {
        GameObject door = CreateCube(name, parent, position, size, Materials["DarkDoorWood"]);
        door.transform.rotation = rotation;
        CreateSphere($"{name} Brass Knob", parent, position + new Vector3(size.x > size.z ? size.x * 0.34f : 0f, 0f, size.z > size.x ? size.z * 0.34f : -0.07f), new Vector3(0.11f, 0.11f, 0.11f), Materials["Brass"]);
        return door;
    }

    private static void WallX(string name, Transform parent, float minX, float maxX, float z, float height, float thickness)
    {
        float length = maxX - minX;
        CreateCube(name, parent, new Vector3(minX + length * 0.5f, height * 0.5f, z), new Vector3(length, height, thickness), Materials["WallCream"]);
    }

    private static void WallZ(string name, Transform parent, float x, float minZ, float maxZ, float height, float thickness)
    {
        float length = maxZ - minZ;
        CreateCube(name, parent, new Vector3(x, height * 0.5f, minZ + length * 0.5f), new Vector3(thickness, height, length), Materials["WallCream"]);
    }

    private static GameObject CreateCube(string name, Transform parent, Vector3 position, Vector3 size, Material material, bool collider = true)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent);
        cube.transform.position = position;
        cube.transform.localScale = size;
        ApplyMaterial(cube, material);

        if (!collider)
        {
            Object.DestroyImmediate(cube.GetComponent<Collider>());
        }

        return cube;
    }

    private static GameObject CreateCylinder(string name, Transform parent, Vector3 position, float radius, float height, Material material, bool collider = true)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = name;
        cylinder.transform.SetParent(parent);
        cylinder.transform.position = position;
        cylinder.transform.localScale = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
        ApplyMaterial(cylinder, material);

        if (!collider)
        {
            Object.DestroyImmediate(cylinder.GetComponent<Collider>());
        }

        return cylinder;
    }

    private static GameObject CreateSphere(string name, Transform parent, Vector3 position, Vector3 size, Material material, bool collider = true)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.SetParent(parent);
        sphere.transform.position = position;
        sphere.transform.localScale = size;
        ApplyMaterial(sphere, material);

        if (!collider)
        {
            Object.DestroyImmediate(sphere.GetComponent<Collider>());
        }

        return sphere;
    }

    private static void ApplyMaterial(GameObject gameObject, Material material)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            renderer.sharedMaterial = material;
        }
    }

    private static void ApplyMaterialToChildren(GameObject gameObject, Material material)
    {
        if (material == null)
        {
            return;
        }

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.sharedMaterial = material;
        }
    }

    private static GameObject PlacePrefabFitted(string assetPath, string name, Transform parent, Vector3 targetCenter, Vector3 targetSize, Quaternion rotation, Material fallbackMaterial)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        GameObject instance;

        if (prefab != null)
        {
            instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            instance.name = name;
            instance.transform.SetParent(parent);
            instance.transform.rotation = rotation;
            ApplyMaterialToChildren(instance, fallbackMaterial);
            FitObjectToBounds(instance, targetCenter, targetSize);
            return instance;
        }

        instance = CreateCube(name, parent, targetCenter, targetSize, fallbackMaterial);
        instance.transform.rotation = rotation;
        return instance;
    }

    private static void FitObjectToBounds(GameObject instance, Vector3 targetCenter, Vector3 targetSize)
    {
        instance.transform.position = Vector3.zero;
        Bounds bounds = CalculateRendererBounds(instance);

        if (bounds.size.x <= 0.001f || bounds.size.y <= 0.001f || bounds.size.z <= 0.001f)
        {
            instance.transform.position = targetCenter;
            return;
        }

        Vector3 scale = instance.transform.localScale;
        scale.x *= targetSize.x / bounds.size.x;
        scale.y *= targetSize.y / bounds.size.y;
        scale.z *= targetSize.z / bounds.size.z;
        instance.transform.localScale = scale;

        bounds = CalculateRendererBounds(instance);
        Vector3 offset = targetCenter - bounds.center;
        instance.transform.position += offset;
    }

    private static Bounds CalculateRendererBounds(GameObject instance)
    {
        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            return new Bounds(instance.transform.position, Vector3.one);
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }

    private static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }

            Transform result = FindDeepChild(child, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static void EnsureLayer(string layerName)
    {
        if (LayerMask.NameToLayer(layerName) != -1)
        {
            return;
        }

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                return;
            }
        }

        Debug.LogWarning($"Could not create layer '{layerName}'. All-layer raycasts will still work.");
    }

    private static void SetLayerRecursively(GameObject gameObject, int layer)
    {
        if (layer < 0)
        {
            return;
        }

        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
#endif
