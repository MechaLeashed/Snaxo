using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class KitchenSetup : MonoBehaviour
{
    [MenuItem("Kitchen Tools/1. Create Plating Station", false, 10)]
    public static void CreatePlatingStation()
    {
        // Create the base object (a simple cube)
        GameObject station = GameObject.CreatePrimitive(PrimitiveType.Cube);
        station.name = "Plating Station";
        
        // Ensure it has a box collider for the player to interact with
        if (station.GetComponent<BoxCollider>() == null)
            station.AddComponent<BoxCollider>();

        // Add the PlatingStation script
        PlatingStation platingScript = station.AddComponent<PlatingStation>();

        // Create the "Plate" area on top of the counter
        GameObject plate = new GameObject("PlateTransform");
        plate.transform.SetParent(station.transform);
        // Position it slightly above the cube
        plate.transform.localPosition = new Vector3(0, 0.6f, 0); 

        // Automatically assign the plate to the script
        platingScript.plateTransform = plate.transform;

        // Select the newly created object in the editor
        Selection.activeGameObject = station;
        
        Debug.Log("Created Plating Station! You can now move it around your scene. Don't forget to add your Recipes to its 'Available Recipes' list.");
    }

    [MenuItem("Kitchen Tools/2. Create Customer NPC", false, 11)]
    public static void CreateNPC()
    {
        // 1. Create NPC Base (a simple cylinder)
        GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        npc.name = "Customer NPC";
        
        if (npc.GetComponent<BoxCollider>() == null)
            npc.AddComponent<BoxCollider>();

        NPCOrder npcScript = npc.AddComponent<NPCOrder>();

        // 2. Create the UI Canvas for the pop-up bubble
        GameObject canvasObj = new GameObject("Order Canvas");
        canvasObj.transform.SetParent(npc.transform);
        // Position the bubble above the NPC's head
        canvasObj.transform.localPosition = new Vector3(0, 1.5f, 0); 
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2, 2);
        canvasRect.localScale = Vector3.one; // Ensure scale is 1
        
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 3. Create the Background Bubble Image
        GameObject bgObj = new GameObject("Bubble Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = Color.white; // White bubble
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(1.5f, 1.5f);
        bgRect.anchoredPosition = Vector2.zero;

        // 4. Create the Food Icon Image inside the bubble
        GameObject iconObj = new GameObject("Food Icon");
        iconObj.transform.SetParent(bgObj.transform, false);
        Image foodImage = iconObj.AddComponent<Image>();
        foodImage.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Gray placeholder until assigned
        
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(1, 1);
        iconRect.anchoredPosition = Vector2.zero;

        // 5. Assign the UI references to the script automatically!
        npcScript.uiPanel = canvasObj;
        npcScript.orderBubbleImage = foodImage;

        // Select the newly created object in the editor
        Selection.activeGameObject = npc;
        
        Debug.Log("Created Customer NPC! Don't forget to assign a 'Wanted Meal' in the inspector.");
    }
}
