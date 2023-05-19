using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Numerics;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public GameObject inventoryUI;
    public Slot[] itemSlots;
    public RectTransform selectImage;
    public GameObject itemExplanationUI;
    public Image buttonHoldImage;
    [HideInInspector] public Slot selectedItem;
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

    private Queue<Slot> emptyItemSlot = new Queue<Slot>();
    public List<Slot> fullItemSlot = new List<Slot>();
    public List<Item> items = new List<Item>();
    private int selectedItemIndex;

    private void Start()
    {
        inventoryUI.SetActive(false);

        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();

        itemImage = itemExplanationUI.transform.Find("Item Image").GetComponent<Image>();
        itemName = itemExplanationUI.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        itemRank = itemExplanationUI.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
        itemExplanation = itemExplanationUI.transform.Find("Explanation").GetComponent<TextMeshProUGUI>();

        foreach (Slot slot in itemSlots)
        {
            if (slot.itemIndex == -1)
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
                GameManager.instance.playerTime.scale = 0f;
            else
                GameManager.instance.playerTime.scale = 1f;
        }

        if (!inventoryUI.activeSelf)
        {
            selectedItem = null;
            selectedItemIndex = -1;
            return;
        }

        ResetIventory();

        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("GameController"))
            {
                selectedItem = result.gameObject.GetComponent<Slot>();
            }
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        int newIndex;
        if (horizontal != 0 && Input.GetButtonDown("Horizontal"))
        {
            newIndex = selectedItemIndex + (int)horizontal;
            if (newIndex < 0)
            {
                selectedItemIndex = fullItemSlot.Count - 1;
            }
            else
            {
                selectedItemIndex = newIndex % fullItemSlot.Count;
            }
        }
        if (vertical != 0 && Input.GetButtonDown("Vertical"))
        {
            newIndex = selectedItemIndex + (int)vertical * -3;
            if (newIndex < 0)
            {
                selectedItemIndex = fullItemSlot.Count + newIndex;
            }
            else
            {
                selectedItemIndex = newIndex % fullItemSlot.Count;
            }
        }

        if (selectedItemIndex != -1)
        {
            selectedItem = fullItemSlot[selectedItemIndex];
        }

        if (selectedItem != null && SelectedItemExplanation != null)
        {
            selectImage.position = selectedItem.GetComponent<RectTransform>().position;
            buttonHoldImage.transform.position = selectImage.position;
            itemImage.sprite = SelectedItemExplanation.sprite;
            itemName.text = SelectedItemExplanation.itemName;
            itemRank.text = SelectedItemExplanation.rankText;
            itemRank.color = SelectedItemExplanation.rankColor;
            itemExplanation.text = SelectedItemExplanation.explanation;

            selectImage.gameObject.SetActive(true);
            itemExplanationUI.SetActive(true);

            if (lastSelectedItem == null || lastSelectedItem != selectedItem)
            {
                lastSelectedItem = selectedItem.gameObject;
                dropTime = 0f;
            }

            if (Input.GetKey(KeyCode.F))
            {
                if (dropTime >= dropDelay)
                {
                    dropTime = 0f;
                    SelectedItemExplanation = DropItem(selectedItem, true);
                    selectedItem = null;
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
            SelectedItemExplanation = SelectItem(selectedItem);
            selectImage.gameObject.SetActive(false);
            itemExplanationUI.SetActive(false);
            dropTime = 0f;
        }

        buttonHoldImage.fillAmount = dropTime / dropDelay;
    }

    private void ResetIventory()
    {
        while (fullItemSlot.Count > 0)
        {
            items.Remove(DropItem(fullItemSlot[0], false));
        }

        while (items.Count > 0)
        {
            //Item itme = items.Dequeue();
            //itme.GetItem();
        }
    }

    public Item SelectItem(Slot _slot)
    {
        Item selectedItem = null;
        foreach (Item item in items)
        {
            if (item.itemSlotIndex == _slot.itemIndex)
            {
                selectedItem = item;
                break;
            }
        }
        return selectedItem;
    }

    private Item DropItem(Slot _item, bool itemRespawn)
    {
        selectedItem = _item;
        Slot _itemSlot = _item.GetComponent<Slot>();

        Item selectItem = SelectItem(_itemSlot);
        emptyItemSlot.Enqueue(_itemSlot);
        fullItemSlot.Remove(_itemSlot);

        _item.gameObject.SetActive(false);
        if (itemRespawn)
        {
            selectItem.PutItem();
            return null;
        }

        return selectItem;
    }

    public Transform GetItemSlot()
    {
        if (isFull == true)
            return null;

        Slot itemSlot = emptyItemSlot.Dequeue();
        fullItemSlot.Add(itemSlot);
        itemSlot.itemIndex = fullItemSlot.Count - 1;
        return itemSlot.gameObject.transform;
    }
}
