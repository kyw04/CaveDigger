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
    public Image buttonHoldImage;
    [HideInInspector] public GameObject selectedItem;
    [HideInInspector] public bool isFull;
    public float dropTime;
    public float dropDelay = 1.5f;

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    private Image itemImage;
    private TextMeshProUGUI itemName;
    private TextMeshProUGUI itemRank;
    private TextMeshProUGUI itemExplanation;
    private Item SelectedItemExplanation;
    private GameObject lastSelectedItem;

    private Queue<Transform> emptyItemSlot = new Queue<Transform>();

    private void Start()
    {
        inventoryUI.SetActive(false);

        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();

        itemImage = itemExplanationUI.transform.Find("Item Image").GetComponent<Image>();
        itemName = itemExplanationUI.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        itemRank = itemExplanationUI.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
        itemExplanation = itemExplanationUI.transform.Find("Explanation").GetComponent<TextMeshProUGUI>();

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
        if (emptyItemSlot.Count == 0)
            isFull = true;
        else
            isFull = false;

        if (Input.GetKeyDown(KeyCode.Tab) || (Input.GetKeyDown(KeyCode.Escape) && inventoryUI.activeSelf))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);

            if (inventoryUI.activeSelf)
                GameManager.instance.playerTimeScale = 0f;
            else
                GameManager.instance.playerTimeScale = 1f;
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

        selectedItem = null;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("GameController"))
            {
                selectedItem = result.gameObject;
                selectImage.position = selectedItem.GetComponent<RectTransform>().position;
                buttonHoldImage.transform.position = selectImage.position;
            }
        }

        if (selectedItem != null && SelectedItemExplanation != null)
        {
            itemImage.sprite = SelectedItemExplanation.sprite;
            itemName.text = SelectedItemExplanation.itemName;
            itemRank.text = SelectedItemExplanation.rankText;
            itemRank.color = SelectedItemExplanation.rankColor;
            itemExplanation.text = SelectedItemExplanation.explanation;

            selectImage.gameObject.SetActive(true);
            itemExplanationUI.SetActive(true);

            if (lastSelectedItem == null || lastSelectedItem != selectedItem)
            {
                lastSelectedItem = selectedItem;
                dropTime = 0f;
            }

            if (Input.GetKey(KeyCode.F))
            {
                if (dropTime >= dropDelay)
                {
                    dropTime = 0f;
                    SelectedItemExplanation.PutItem();
                    emptyItemSlot.Enqueue(selectedItem.transform);

                    selectedItem.SetActive(false);
                    selectedItem = null;
                    SelectedItemExplanation = null;
                }
                else
                {
                    dropTime += Time.deltaTime;
                }
            }
            else
            {
                dropTime = 0f;
            }
        }
        else
        {
            selectImage.gameObject.SetActive(false);
            itemExplanationUI.SetActive(false);
            dropTime = 0f;
        }

        buttonHoldImage.fillAmount = dropTime / dropDelay;
    }

    public void SelectItemExplanation(Item _item)
    {
        SelectedItemExplanation = _item;
    }

    public Transform GetItemSlot()
    {
        if (isFull == true)
            return null;

        return emptyItemSlot.Dequeue();
    }
}
