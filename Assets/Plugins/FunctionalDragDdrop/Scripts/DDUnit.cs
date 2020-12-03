using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DDUnit : MonoBehaviour, IBeginDragHandler , IDragHandler, IEndDragHandler, IPointerDownHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    // ---Unit Settings---
    public DDCanvas ddCanvas; // OBS.: The DDCanvas is set in the Start method to facilitate the use when instantiating it, but can be set manually via inspector

    public new string tag; // tag that can be used to define drop in/out rules in Vessels
                           // this rules can be applied adding the tag to the Vessel 'dropInDisabledTags' and/or 'dragOutDisabledTags' lists
                           // the GameObject tag is not used to simplify integration of this asset to any project

    public bool canSelect; // set if Unit can be selected
    public bool canDrag; // set if Unit can de dragged from its positions

    public bool enableSelectionOutline; // set if selection outline is shown when Unit is selected

    // defines the position of the Unit related to the pointer while dragged
    public enum DragAnchorItems { clickPosition, pivotPosition };
    public DragAnchorItems dragAnchor;

    // defines what happens to the Unit when it is dropped outside any Vessel
    public enum DropOnVoidItems { kill, backToLastPosition, stayInReleasePosition };
    public DropOnVoidItems dropOnVoid;
                           
    public Vector2 onDragSize; // set the values x, y 0 (zero) or Vector2.zero to let the value the same as before
    public Vector2 onDroppedSize; // set the values x, y 0 (zero) or Vector2.zero to let the value the same as before
    // OBS.: if the Unit is placed on a Vessel, the variable 'onDroppedSize' will be overridden by the LayoutGroup options
    
    public Vector3 onDragRotation; // set the values x, y and z 0 (zero) or Vector3.zero to let the value the same as before
    public Vector3 onDropRotation; // set the values x, y and z 0 (zero) or Vector3.zero to let the value the same as before 

    // --- Unit Control variables and properties---
    private Vector3 mouseUnitDiffPosition;
    private Vector3 dragPosition;

    public bool IsDroppedIn
    {
        get
        {
            bool isDroppedIn = true;
            try
            {
                bool isDDVesselEnabledInParent = transform.parent.GetComponent<DDVessel>().isActiveAndEnabled;
            }
            catch
            {
                isDroppedIn = false;
            }

            return isDroppedIn;
        }
    }
    public bool IsDDVessel
    {
        get
        {
            bool isDDVessel = true;
            try
            {
                bool isDDVesselEnabled = GetComponent<DDVessel>().isActiveAndEnabled;
            }
            catch
            {
                isDDVessel = false;
            }

            return isDDVessel;
        }
    }
    public bool CanBeDraggedOut
    {
        get{
            bool canBeDraggedOut = true;
            if (IsDroppedIn == true)
            {
                try
                {
                    if (transform.parent.GetComponent<DDVessel>().dragOutDisabledTags.Count > 0)
                    {
                        List<string> dragOutDisabledTagsList = transform.parent.GetComponent<DDVessel>().dragOutDisabledTags;
                        if (dragOutDisabledTagsList != null)
                        {
                            if (dragOutDisabledTagsList.Contains(tag))
                            {
                                canBeDraggedOut = false;
                            }
                        }
                    }
                }
                catch { }
            }
            return canBeDraggedOut;
        }
    }
    public Transform StartParent { get; set; }
    public Vector3 StartUnitPosition { get; set; }
    public int StartIndexInParent { get; set; }
    public DDUnit UnitUnderPointer { get; set; }
    public int IndexOfUnitUnderPointer { get; set; } // store the future drop position when pointer gets over another Unit and inside a vessel
    public bool IsBeingDragged { get; set; }
    public DDUnitImage DdUnitImage { get; set; }
    public bool WasDroppedOnVoid { get; set; }
    public GameObject GhostUnit { get; set; }
    
    public DDVessel ParentDDVessel
    {
        get
        {
            DDVessel parentDDVessel;
            try
            {
                parentDDVessel = transform.parent.GetComponent<DDVessel>();
            }
            catch
            {
                parentDDVessel = null;
            }

            return parentDDVessel;
        }
    }

    private void OnValidate()
    {
        try
        {
            GetComponent<Outline>().enabled = false;
        }
        catch
        {
            this.gameObject.AddComponent<Outline>();
            GetComponent<Outline>().enabled = false;
        }
    }

    void Start()
    {
        ddCanvas = GameObject.FindGameObjectWithTag("DDCanvas").GetComponent<DDCanvas>();

        ////Example initial settings
        //canSelect = true;
        //canDrag = true;
        //isBeingDragged = false;
        //dropOnVoid = DropOnVoidItems.backToLastPosition;
        //dragAnchor = DragAnchorItems.clickPosition;
        //enableSelectionOutline = true;
        //showGhost = true;
        //dropPosition = DropPositionItems.inheritFromVessel;
        //dropPositionIndex = 0;
        //onDragSize = Vector2.zero;
        //onDroppedSize = Vector2.zero;
        //onDragRotation = Vector3.zero;

        StartParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (canSelect)
        {
            if (!ddCanvas.SelectedUnits.Contains(this))
            {
                SelectUnit();
            }

            foreach (DDUnit ddUnit in ddCanvas.SelectedUnits)
            {
                ddUnit.WasDroppedOnVoid = true;

                if (ddUnit.CanBeDraggedOut)
                {
                    ddUnit.IsBeingDragged = true;

                    ddUnit.mouseUnitDiffPosition = Input.mousePosition - ddUnit.StartUnitPosition;

                    if (ddUnit.ParentDDVessel != null && ddUnit.ParentDDVessel.dropOverUnit == DDVessel.DropOverUnitItems.dropInOnGhostPosition) //(ddUnit.showGhost == true)
                    {
                        ddUnit.ShowGhostAtPosition(ddUnit);
                    }

                    SetUnitToCanvasOnDrag(ddUnit);
                    
                    if (ddUnit.onDragSize != Vector2.zero)
                    {
                        ddUnit.GetComponent<RectTransform>().sizeDelta = onDragSize;
                    }

                    if (ddUnit.onDragRotation != Vector3.zero)
                    {
                        ddUnit.transform.rotation = Quaternion.Euler(ddUnit.onDragRotation);
                    }
                }
            }
        }
    }

    //move object to a canvas without raycast interaction (DD Canvas OnDrag)
    public void SetUnitToCanvasOnDrag(DDUnit ddUnit)
    {
        ddUnit.transform.SetParent(ddUnit.ddCanvas.canvasOnDrag);
        ddUnit.transform.SetAsFirstSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    private void OnDestroy()
    {
        if (DdUnitImage != null)
        {
            Destroy(DdUnitImage.gameObject);
        }
        if (GhostUnit != null)
        {
            Destroy(GhostUnit.gameObject);
        }
    }

    public void ReturnUnitToLastPosition()
    {
        transform.position = StartUnitPosition;
        transform.SetParent(StartParent);
        transform.SetSiblingIndex(StartIndexInParent);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        sortImagesOneTime = true;

        foreach (DDUnit ddUnit in ddCanvas.SelectedUnits)
        {
            if (ddUnit.IsBeingDragged)
            {
                if (ddUnit.WasDroppedOnVoid == true)
                {
                    if (ddUnit.dropOnVoid == DropOnVoidItems.kill)
                    {
                        Destroy(ddUnit.gameObject);
                    }
                    else if (ddUnit.dropOnVoid == DropOnVoidItems.backToLastPosition)
                    {
                        ddUnit.ReturnUnitToLastPosition();
                    }
                    else if (ddUnit.dropOnVoid == DropOnVoidItems.stayInReleasePosition)
                    {
                        ddUnit.transform.SetParent(ddUnit.ddCanvas.transform);
                        //ParentDDVessel = null;
                    }
                }
                else
                {
                    //ParentDDVessel = transform.parent.GetComponent<DDVessel>();
                }

                if (ddUnit.GhostUnit != null)
                {
                    ddUnit.GhostUnit.SetActive(false);
                }

                ddUnit.transform.rotation = Quaternion.Euler(0, 0, 0);
                ddUnit.IsBeingDragged = false;

                if (ddUnit.onDroppedSize != Vector2.zero)
                {
                    ddUnit.GetComponent<RectTransform>().sizeDelta = onDroppedSize;
                }

                if (ddUnit.onDropRotation != Vector3.zero)
                {
                    ddUnit.transform.rotation = Quaternion.Euler(ddUnit.onDropRotation);
                }

                ddUnit.UnitUnderPointer = null;
            }
        }

        foreach (DDUnit ddUnit in ddCanvas.SelectedUnits)
        {
            ddUnit.StartParent = ddUnit.transform.parent;
            ddUnit.StartIndexInParent = ddUnit.transform.GetSiblingIndex();
            ddUnit.StartUnitPosition = ddUnit.transform.position;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerPress)
        {
            if (IsDDVessel == false)
            {
                if (ParentDDVessel != null && ParentDDVessel.dropOverUnit == DDVessel.DropOverUnitItems.dropInOnGhostPosition)
                {
                    foreach (DDUnit ddUnit in ddCanvas.SelectedUnits)
                    {
                        if (ddUnit.IsBeingDragged)
                        {
                            ddUnit.ShowGhostAtPosition(eventData.pointerEnter.GetComponent<DDUnit>());
                            ddUnit.UnitUnderPointer = eventData.pointerEnter.GetComponent<DDUnit>();
                        }
                    }
                }
                else
                {
                    foreach (DDUnit ddUnit in ddCanvas.SelectedUnits)
                    {
                        ddUnit.IndexOfUnitUnderPointer = transform.GetSiblingIndex();
                        
                        ddUnit.UnitUnderPointer = eventData.pointerEnter.GetComponent<DDUnit>();
                    }
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (DDUnit ddUnit in ddCanvas.SelectedUnits)
        {
            if (ParentDDVessel == null || ParentDDVessel.dropOverUnit != DDVessel.DropOverUnitItems.dropInOnGhostPosition)
            {
                ddUnit.UnitUnderPointer = null;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Input.GetKey(ddCanvas.multipleSelectionKey) || ddCanvas.enableMultipleSelection == false)
        {
            ddCanvas.UnselectAllUnits();
        }

        if (canSelect)
        {
            if (!ddCanvas.SelectedUnits.Contains(this))
            {
                SelectUnit();
            }
            else
            {
                UnselectUnit();
            }
        }
    }

    public void SelectUnit()
    {
        if (canSelect)
        {
            if (enableSelectionOutline)
            {
                if(DdUnitImage != null)
                {
                    DdUnitImage.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    GetComponent<Outline>().enabled = true;
                }
            }

            ddCanvas.SelectedUnits.Add(this);
            StartParent = transform.parent;
            StartIndexInParent = transform.GetSiblingIndex();
            StartUnitPosition = transform.position;
        }
    }

    public void UnselectUnit()
    {
        GetComponent<Outline>().enabled = false;
        if (DdUnitImage != null)
        {
            DdUnitImage.GetComponent<Outline>().enabled = false;
        }
        ddCanvas.SelectedUnits.Remove(this);
    }
    
    void Update()
    {
        
    }
    
    void FixedUpdate()
    {
        Move(Input.mousePosition);
    }

    void Move(Vector3 mousePosition)
    {
        if (IsBeingDragged)
        {
            if (canDrag)
            {
                
                if (dragAnchor == DragAnchorItems.clickPosition)
                {
                    dragPosition = mousePosition - mouseUnitDiffPosition;
                }
                else if (dragAnchor == DragAnchorItems.pivotPosition)
                {
                    dragPosition = mousePosition;
                }

                GroupUnits(mousePosition);

                transform.position = dragPosition;
            }
        }
    }

    bool sortImagesOneTime = true;

    // group the Units when dragged
    void GroupUnits(Vector3 mousePosition)
    {
        if (ddCanvas.SelectedUnits.Count > 1)
        {
            if (ddCanvas.multipleDragGrouping == DDCanvas.MultipleDragGroupingItems.stair)
            {
                dragPosition = mousePosition + (new Vector3(-ddCanvas.groupingOffset.x, ddCanvas.groupingOffset.y, 0) * transform.GetSiblingIndex());
            }
            else if (ddCanvas.multipleDragGrouping == DDCanvas.MultipleDragGroupingItems.cardDeck)
            {
                dragPosition = mousePosition + (new Vector3(-ddCanvas.groupingOffset.x, 0, 0) * transform.GetSiblingIndex());
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,
                    ddCanvas.groupingOffset.y * (transform.GetSiblingIndex() - (ddCanvas.SelectedUnits.Count / 2)));
            }
            else if (ddCanvas.multipleDragGrouping == DDCanvas.MultipleDragGroupingItems.none)
            {
                dragPosition = mousePosition;
            }

            if (sortImagesOneTime)
            {
                ddCanvas.SortUnitImages(ddCanvas.transform);
                sortImagesOneTime = false;
            }
        }
    }

    void ShowGhostAtPosition(DDUnit ddUnitPositio)
    {
        if (this.isActiveAndEnabled)
        {
            if (GhostUnit == null)
            {
                GhostUnit = Instantiate(ddCanvas.ghostUnit);
                GhostUnit.name = "ghost " + name;
            }

            GhostUnit.SetActive(true);
            GhostUnit.transform.SetParent(ddUnitPositio.transform.parent);
            GhostUnit.transform.position = ddUnitPositio.transform.position;
            GhostUnit.transform.SetSiblingIndex(ddUnitPositio.transform.GetSiblingIndex());
        }
    }
}
