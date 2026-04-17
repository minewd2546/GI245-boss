using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInShop : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Toggle toggleIcon;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text itemText;
    [SerializeField] private TMP_Text priceText;

    private Item item;
    private bool isShopItem;
    private UIManager uiManager;
    private Button selectButton;

    public Item Item => item;
    public bool IsSelected => toggleIcon != null && toggleIcon.isOn;

    public void Init(Item itemData, bool fromShop, UIManager owner)
    {
        item = itemData;
        isShopItem = fromShop;
        uiManager = owner;

        if (selectButton == null)
            selectButton = GetComponent<Button>();

        if (selectButton == null)
            selectButton = gameObject.AddComponent<Button>();

        if (toggleIcon == null)
            toggleIcon = GetComponentInChildren<Toggle>(true);

        if (iconImage == null)
        {
            Transform iconTransform = transform.Find("Toggle/IconImage");
            if (iconTransform == null)
                iconTransform = transform.Find("IconImage");

            if (iconTransform != null)
                iconImage = iconTransform.GetComponent<Image>();
        }

        if (iconImage == null && toggleIcon != null)
        {
            Image[] images = toggleIcon.GetComponentsInChildren<Image>(true);
            foreach (Image imageCandidate in images)
            {
                if (imageCandidate.gameObject.name == "IconImage")
                {
                    iconImage = imageCandidate;
                    break;
                }
            }
        }

        if (itemText == null || priceText == null)
        {
            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
            if (texts.Length > 0 && itemText == null)
                itemText = texts[0];
            if (texts.Length > 1 && priceText == null)
                priceText = texts[1];
        }

        if (itemText != null)
            itemText.text = item.ItemName;

        if (priceText != null)
            priceText.text = item.NormalPrice.ToString();

        Image image = iconImage;
        if (image == null && toggleIcon != null)
            image = toggleIcon.targetGraphic as Image;
        if (image != null)
            image.sprite = item.Icon;

        if (toggleIcon != null)
        {
            toggleIcon.SetIsOnWithoutNotify(false);
            toggleIcon.onValueChanged.RemoveAllListeners();
            toggleIcon.onValueChanged.AddListener(HandleToggle);
        }

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(SelectThisCard);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectThisCard();
    }

    private void SelectThisCard()
    {
        HandleToggle(true);

        if (toggleIcon != null)
            toggleIcon.isOn = true;
    }

    private void HandleToggle(bool isOn)
    {
        if (!isOn || uiManager == null)
            return;

        if (isShopItem)
            uiManager.SelectShopItem(item);
        else
            uiManager.SelectPartyItem(item);
    }
}
