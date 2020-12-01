using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DDCanvas : MonoBehaviour, IPointerDownHandler
{
    // ---Canvas Settings---
    public Transform canvasOnDrag; // OBS.: set canvasOnDrag manually (inspector or code)
    public Transform canvasImages; // OBS.: set canvasImages manually (inspector or code)
    public GameObject ghostUnit; // OBS.: set ghostUnit manually (inspector or code)

    public bool enableMultipleSelection;
    public KeyCode multipleSelectionKey;
    public float alphaOnDrag;

    public enum MultipleDragGroupingItems { none, stair, cardDeck };
    public MultipleDragGroupingItems multipleDragGrouping;
    public Vector2 groupingOffset;

    // ---Canvas Control variables and properties---
    private List<DDUnit> selectedUnits;
    public List<DDUnit> SelectedUnits
    {
        get
        {
            if (selectedUnits == null)
            {
                selectedUnits = new List<DDUnit>();
            }
            return selectedUnits;
        }
        set
        {
            selectedUnits = value;
        }
    }

    private void Awake()
    {
        tag = "DDCanvas";
    }

    public void OnValidate()
    {
        canvasOnDrag.GetComponent<CanvasGroup>().alpha = alphaOnDrag; //update the alpha when changed in inspector
    }

    void Start()
    {
        //alphaOnDrag = 0.5f;
        //canvasOnDrag.GetComponent<CanvasGroup>().alpha = alphaOnDrag;
        //multipleDragGrouping = MultipleDragGroupingItems.cardDeck;
        //groupingOffset = new Vector2(10, 10);

        ////Example initial settings
        //enableMultipleSelection = true;
        multipleSelectionKey = KeyCode.LeftControl;
    }

    // recursive method to organize the Unit Images based on the hierarchy of the DDCanvas
    // OBS.: this method should be used only if the project uses overlapped Unit Images and, if possible, be called only on hierarchy changes
    // to avoid efficiency loss
    public void SortUnitImages(Transform parent)
    {
        foreach (Transform child in parent)
        {
            try
            {
                DDUnitImage childImage = child.GetComponent<DDUnit>().DdUnitImage;
                childImage.transform.SetAsLastSibling();
            }
            catch { }

            SortUnitImages(child);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        try
        {
            eventData.pointerPress.GetComponent<DDUnit>();
            if (!Input.GetKey(multipleSelectionKey) || enableMultipleSelection == false)
            {
                UnselectAllUnits();
            }
        }
        catch
        {
            UnselectAllUnits();
        }
    }

    public void UnselectAllUnits()
    {
        foreach (DDUnit ddUnit in SelectedUnits)
        {
            if (ddUnit.enableSelectionOutline)
            {
                ddUnit.GetComponent<Outline>().enabled = false;
                try
                {
                    ddUnit.DdUnitImage.GetComponent<Outline>().enabled = false;
                }
                catch { }
            }
        }
        SelectedUnits.Clear();
    }
}
