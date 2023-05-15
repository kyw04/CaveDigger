using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public GameObject inventoryUI;
    public Transform[] itemSlots;
    public RectTransform selectImage;
    public GameObject itemExplanationUI;

    private GameObject selectedItem;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    private Queue<Transform> emptyItemSlot = new Queue<Transform>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        inventoryUI.SetActive(false);

        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();

        foreach (Transform slot in itemSlots)
        {
            if (slot.childCount == 0)
            {
                emptyItemSlot.Enqueue(slot);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || (Input.GetKeyDown(KeyCode.Escape) && inventoryUI.activeSelf))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);

            if (inventoryUI.activeSelf)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }

        if (!inventoryUI.activeSelf)
        {
            selectedItem = null;
            return;
        }

        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("GameController"))
            {
                //Debug.Log(result.gameObject.name);
                selectedItem = result.gameObject;
                selectImage.position = selectedItem.GetComponent<RectTransform>().position;
            }
        }

        if (selectedItem != null)
        {
            Item itemExplanation = selectedItem.GetComponentInChildren<Item>();

            Image itemImage = itemExplanationUI.transform.Find("Item Image").GetComponent<Image>();
            TextMeshProUGUI name = itemExplanationUI.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI rank = itemExplanationUI.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI explanation = itemExplanationUI.transform.Find("Explanation").GetComponent<TextMeshProUGUI>();

            itemImage.sprite = itemExplanation.sprite;
            name.text = itemExplanation.itemName;
            rank.text = itemExplanation.rankText;
            rank.color = itemExplanation.rankColor;
            explanation.text = itemExplanation.explanation;

            selectImage.gameObject.SetActive(true);
            itemExplanationUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                selectedItem.transform.DetachChildren();

                itemExplanation.PutItem();
                emptyItemSlot.Enqueue(selectedItem.transform);

                selectedItem.SetActive(false);
                selectedItem = null;
            }
        }
        else
        {
            selectImage.gameObject.SetActive(false);
            itemExplanationUI.SetActive(false);
        }
    }

    public Transform GetItemSlot()
    {
        if (emptyItemSlot.Count == 0)
            return null;

        Transform t = emptyItemSlot.Dequeue();
        Debug.Log(t.name);
        return t;
        //return emptyItemSlot.Dequeue();
    }
}
