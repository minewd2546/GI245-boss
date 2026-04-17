using UnityEngine;
using UnityEngine.EventSystems;

public class LeftClick : MonoBehaviour
{
    public static LeftClick instance;

    private Camera cam;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private RectTransform boxSelection;
    private Vector2 oldAnchoredPos;
    private Vector2 startPos;
    private bool wasDraggingSelection;

    void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Character", "Building", "Item");
        boxSelection = UIManager.instance.SelectionBox;
    }

    private void ClearRingSelection()
    {
        foreach (Character h in PartyManager.instance.SelectChars)
            h.ToggleRingSelection(false);
    }

    private void ClearEverything()
    {
        ClearRingSelection();
        PartyManager.instance.SelectChars.Clear();
        UIManager.instance.MapToggleAvatar();
        UIManager.instance.ShowMagicToggles();
        UIManager.instance.RefreshSelectedHeroPanel();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            wasDraggingSelection = false;

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            ClearEverything();
        }

        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Vector2.Distance(startPos, Input.mousePosition) > 10f)
            {
                wasDraggingSelection = true;
                UpdateSelectionBox(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (wasDraggingSelection)
                ReleaseSelectionBox();
            else if (!EventSystem.current.IsPointerOverGameObject())
                TrySelect(Input.mousePosition);
        }
    }

    private void TrySelect(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Player":
                case "Hero":
                    SelectCharacter(hit);
                    break;
            }
        }
    }

    private void UpdateSelectionBox(Vector2 mousePos)
    {
        if (!boxSelection.gameObject.activeInHierarchy)
            boxSelection.gameObject.SetActive(true);

        float width = mousePos.x - startPos.x;
        float height = mousePos.y - startPos.y;

        boxSelection.anchoredPosition = startPos + new Vector2(width / 2f, height / 2f);
        boxSelection.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        oldAnchoredPos = boxSelection.anchoredPosition;
    }

    private void ReleaseSelectionBox()
    {
        Vector2 corner1 = oldAnchoredPos - (boxSelection.sizeDelta / 2f);
        Vector2 corner2 = oldAnchoredPos + (boxSelection.sizeDelta / 2f);

        boxSelection.gameObject.SetActive(false);

        foreach (Character member in PartyManager.instance.Members)
        {
            Vector2 unitPos = cam.WorldToScreenPoint(member.transform.position);

            if ((unitPos.x > corner1.x && unitPos.x < corner2.x) &&
                (unitPos.y > corner1.y && unitPos.y < corner2.y))
            {
                if (!PartyManager.instance.SelectChars.Contains(member))
                    PartyManager.instance.SelectChars.Add(member);

                member.ToggleRingSelection(true);
            }
        }

        boxSelection.sizeDelta = Vector2.zero;
        UIManager.instance.MapToggleAvatar();
        UIManager.instance.ShowMagicToggles();
        UIManager.instance.RefreshSelectedHeroPanel();
    }

    private void SelectCharacter(RaycastHit hit)
    {
        Character hero = hit.collider.GetComponent<Character>();
        if (hero == null)
            return;

        ClearEverything();
        PartyManager.instance.SelectChars.Add(hero);
        hero.ToggleRingSelection(true);
        UIManager.instance.MapToggleAvatar();
        UIManager.instance.ShowMagicToggles();
        UIManager.instance.RefreshSelectedHeroPanel();
    }
}
