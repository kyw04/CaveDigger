using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public GameObject inventoryUI;
    public Slot[] itemSlots;
    public RectTransform selectImage;
    public GameObject itemExplanationUI;
    public Image buttonHoldImage;
    [HideInInspector] public bool isFull;

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    private Image itemImage;
    private TextMeshProUGUI itemName;
    private TextMeshProUGUI itemRank;
    private TextMeshProUGUI itemExplanation;
    private Slot lastSelectedItemSlot;
    private Vector2 lastMousePosition;
    private float dropTime;
    private float dropDelay;
    private int selectedItemSlotIndex;

    private SortedList<int, Slot> emptyItemSlot = new SortedList<int, Slot>();
    [HideInInspector] public List<Slot> fullItemSlot = new List<Slot>();
    [HideInInspector] public List<Item> items = new List<Item>();

    private void Start()
    {
        inventoryUI.SetActive(false);

        lastMousePosition = Vector2.zero;

        dropDelay = GameManager.instance.itemUseDelay;

        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();

        itemImage = itemExplanationUI.transform.Find("Item Image").GetComponent<Image>();
        itemName = itemExplanationUI.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        itemRank = itemExplanationUI.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
        itemExplanation = itemExplanationUI.transform.Find("Explanation").GetComponent<TextMeshProUGUI>();

        foreach (Slot slot in itemSlots)
        {
            bool isFullSlot = false; 
            foreach (Slot fullSlot in fullItemSlot)
            {
                if (slot == fullSlot)
                {
                    isFullSlot = true;
                }
            }

            if (!isFullSlot)
                emptyItemSlot.Add(slot.index, slot);
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
            {
                //ResetIventory();
                GameManager.instance.playerTime.scale = 0f;
            }
            else
            {
                GameManager.instance.playerTime.scale = 1f;
            }
        }

        if (!inventoryUI.activeSelf)
        {
            selectedItemSlotIndex = -1;
            return;
        }

        Slot selectedItemSlot = ItemSelect();
        if (selectedItemSlot != null)
        {
            selectImage.position = selectedItemSlot.GetComponent<RectTransform>().position;
            buttonHoldImage.transform.position = selectImage.position;
            itemImage.sprite = selectedItemSlot.item.sprite;
            itemName.text = selectedItemSlot.item.itemName;
            itemRank.text = selectedItemSlot.item.rankText;
            itemRank.color = selectedItemSlot.item.rankColor;
            itemExplanation.text = selectedItemSlot.item.explanation;

            selectImage.gameObject.SetActive(true);
            itemExplanationUI.SetActive(true);

            if (lastSelectedItemSlot == null || lastSelectedItemSlot != selectedItemSlot)
            {
                lastSelectedItemSlot = selectedItemSlot;
                //Debug.Log("lastSelectedItemSlot != selectedItemSlot");
                dropTime = 0f;
            }

            if (Input.GetKey(KeyCode.F))
            { 
                //Debug.Log($"Drop Tiem: " + dropTime.ToString("F1"));
                if (dropTime >= dropDelay)
                {
                    dropTime = 0f;
                    selectedItemSlot.item = DropItem(selectedItemSlot, true);
                    ResetIventory();
                }
                else
                {
                    dropTime += Time.deltaTime;
                }
            }
            else
            {
                //Debug.Log("No Key Pressed");
                dropTime = 0f;
            }
        }
        else
        {
            selectImage.gameObject.SetActive(false);
            itemExplanationUI.SetActive(false);

            //Debug.Log("!(selectedItemSlot != null && selectedItemSlot.item != null)");
            dropTime = 0f;
        }

        buttonHoldImage.fillAmount = dropTime / dropDelay;
    }

    private Slot ItemSelect()
    {
        Vector2 currentMousePosition = Input.mousePosition;
        if (currentMousePosition != lastMousePosition)
        {
            lastMousePosition = currentMousePosition;
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("GameController"))
                {
                    selectedItemSlotIndex = result.gameObject.GetComponent<Slot>().index;
                }
            }
        }

        if (fullItemSlot.Count > 0)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            int newIndex;
            if (selectedItemSlotIndex != -1)
            {
                if (Input.GetButtonDown("Horizontal"))
                {
                    newIndex = selectedItemSlotIndex + (int)horizontal;
                    if (newIndex < 0)
                    {
                        selectedItemSlotIndex = fullItemSlot.Count - 1;
                    }
                    else
                    {
                        selectedItemSlotIndex = newIndex % fullItemSlot.Count;
                    }
                }
                if (Input.GetButtonDown("Vertical"))
                {

                    newIndex = selectedItemSlotIndex + (int)vertical * -3;
                    if (newIndex < 0)
                    {
                        selectedItemSlotIndex = fullItemSlot.Count + newIndex;
                    }
                    else
                    {
                        selectedItemSlotIndex = newIndex % fullItemSlot.Count;
                    }
                }
            }
            else if (horizontal != 0 || vertical != 0)
            {
                selectedItemSlotIndex = 0;
            }
        }

        if (selectedItemSlotIndex != -1)
        {
            return fullItemSlot[selectedItemSlotIndex];
        }

        return null;
    }

    private void ResetIventory()
    {
        Queue<Item> resetItems = new Queue<Item>();
        while (fullItemSlot.Count > 0)
        {
            resetItems.Enqueue(DropItem(fullItemSlot[0], false));
        }

        //Debug.Log($"resetItems count: {resetItems.Count}");
        while (resetItems.Count > 0)
        {
            Item item = resetItems.Dequeue();
            //Debug.Log($"resetItems name: {item.name}");
            item.GetItem();
        }
    }

    private Item DropItem(Slot itemSlot, bool itemRespawn)
    {
        selectedItemSlotIndex = -1;

        emptyItemSlot.Add(itemSlot.index, itemSlot);
        isFull = false;
        fullItemSlot.Remove(itemSlot);
        itemSlot.gameObject.SetActive(false);

        Item selectItem = itemSlot.item;
        itemSlot.item = null;
        items.Remove(selectItem);
        if (itemRespawn)
        {
            selectItem.PutItem();
            return null;
        }

        //Debug.Log($"drop itme name: {selectItem.name}");
        return selectItem;
    }

    public Slot GetItemSlot()
    {
        if (isFull == true)
            return null;

        Slot itemSlot = emptyItemSlot.Values[0];
        emptyItemSlot.RemoveAt(0);
        fullItemSlot.Add(itemSlot);

        //Debug.Log("get item slot");
        return itemSlot;
    }
}
