using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DDVessel : MonoBehaviour, IDropHandler
{
    // ---Vessel Settings---
    public DDCanvas ddCanvas; // OBS.: The DDCanvas is set in the Start method to facilitate the use when instantiating it, but can be set manually via inspector

    public enum DropInTagsRuleItems { dropInOnlyEnabledTags, doNotDropInDisabledTags };
    public DropInTagsRuleItems dropInTagsRule;
    public List<string> dropInEnabledTags;
    public List<string> dropInDisabledTags;

    //public enum SnapOutTagsRuleItems { enabledTags, disabledTags }; //not implemented yet
    //public SnapOutTagsRuleItems snapOutTagsRule; //not implemented yet
    //later create snapOutEnabledTags;
    public List<string> dragOutDisabledTags;
    
    public int maxNumberOfUnits; // number of maximum Units that can be placed in this Vessel (0 for no limit)

    // defines the position that the Unit will be after being dropped on this Vessel
    public enum DefaultDropPositionItems { dropInAtFirstPosition, dropInAtLastPosition, dropInAtIndex };
    public DefaultDropPositionItems defaultDropPosition;
    public int defaultDropPositionIndex;

    // OBS.: recomende to use 'switch position' only when multiple selection is disabled
    public enum DropOverUnitItems { none, dropInBefore, dropInAfter, dropInOnGhostPosition, switchUnitsPositions };
    public DropOverUnitItems dropOverUnit;

    void Start()
    {
        ddCanvas = GameObject.FindGameObjectWithTag("DDCanvas").GetComponent<DDCanvas>();

        ////Example initial settings
        //dropDefaultPosition = DropDefaultPositionItems.firstSibling; 
        //dropDefaultPositionIndex = 0;
    }

    public bool CanTagDropIn(DDUnit ddUnit)
    {
        bool canDropIn = true;
        if (dropInTagsRule == DropInTagsRuleItems.dropInOnlyEnabledTags)
        {
            if (!dropInEnabledTags.Contains(ddUnit.tag))
            {
                canDropIn = false;
            }
        }
        else if (dropInTagsRule == DropInTagsRuleItems.doNotDropInDisabledTags)
        {
            if (dropInDisabledTags.Contains(ddUnit.tag))
            {
                canDropIn = false;
            }
        }
        
        return canDropIn;
    }

    public bool HasVesselEnoughSpots(DDUnit ddUnit)
    {
        bool canDropIn = true;
        int childs = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy == true && child.GetComponent<DDUnit>() != null)
            {
                childs++;
            }
        }
        if (maxNumberOfUnits > 0 && childs >= maxNumberOfUnits)
        {
            canDropIn = false;
        }

        return canDropIn;
    }

    public void OnDrop(PointerEventData eventData)
    {
        foreach (DDUnit ddUnit in ddCanvas.SelectedUnits)
        {
            if (ddUnit.IsBeingDragged)
            {
                ddUnit.WasDroppedOnVoid = false;

                if (dropOverUnit == DropOverUnitItems.none || ddUnit.UnitUnderPointer == null)
                {
                    if (CanTagDropIn(ddUnit) && HasVesselEnoughSpots(ddUnit))
                    {
                        ddUnit.transform.SetParent(transform);
                        if (defaultDropPosition == DefaultDropPositionItems.dropInAtFirstPosition)
                        {
                            ddUnit.transform.SetAsFirstSibling();
                        }
                        else if (defaultDropPosition == DefaultDropPositionItems.dropInAtLastPosition)
                        {
                            ddUnit.transform.SetAsLastSibling();
                        }
                        else if (defaultDropPosition == DefaultDropPositionItems.dropInAtIndex)
                        {
                            ddUnit.transform.SetSiblingIndex(defaultDropPositionIndex);
                        }
                    }
                    else if (ddUnit.IsBeingDragged)
                    {
                        ddUnit.ReturnUnitToLastPosition();
                    }
                }
                else
                {
                    if (CanTagDropIn(ddUnit))
                    {
                        if (dropOverUnit == DropOverUnitItems.switchUnitsPositions)
                        {
                            SwitchUnitsPosition(ddUnit, ddUnit.UnitUnderPointer);
                        }
                        else if (HasVesselEnoughSpots(ddUnit))
                        {
                            ddUnit.transform.SetParent(transform);
                            if (dropOverUnit == DropOverUnitItems.dropInBefore)
                            {
                                ddUnit.transform.SetSiblingIndex(ddUnit.IndexOfUnitUnderPointer);
                            }
                            else if (dropOverUnit == DropOverUnitItems.dropInAfter)
                            {
                                ddUnit.transform.SetSiblingIndex(ddUnit.IndexOfUnitUnderPointer + 1);
                            }
                            else if (dropOverUnit == DropOverUnitItems.dropInOnGhostPosition)
                            {
                                ddUnit.transform.SetSiblingIndex(ddUnit.GhostUnit.transform.GetSiblingIndex() + 1);
                            }
                        }
                        else
                        {
                            ddUnit.ReturnUnitToLastPosition();
                        }
                    }
                    else if (ddUnit.IsBeingDragged)
                    {
                        ddUnit.ReturnUnitToLastPosition();
                    }
                }

            }
        }

        //if needed, layout rebuild can be added
        //LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    // method that swithches the position of the Unit dragged and the Unit under it
    public void SwitchUnitsPosition(DDUnit ddUnitIn, DDUnit ddUnitOut)
    {
        Vector3 tempPos = ddUnitOut.transform.position;
        Transform tempParent = ddUnitOut.transform.parent;
        int temIndex = ddUnitOut.transform.GetSiblingIndex();

        if (CanSwitchOut(ddUnitIn, ddUnitOut))
        {
            ddUnitOut.transform.SetParent(ddUnitIn.StartParent);
            ddUnitOut.transform.SetSiblingIndex(ddUnitIn.StartIndexInParent);
            ddUnitOut.transform.position = ddUnitIn.StartUnitPosition;

            ddUnitIn.transform.SetParent(tempParent);
            ddUnitIn.transform.SetSiblingIndex(temIndex);
            ddUnitIn.transform.position = tempPos;
        }
        else
        {
            ddUnitIn.ReturnUnitToLastPosition();
        }
    }

    public bool CanSwitchOut(DDUnit ddUnitIn, DDUnit ddUnitOut)
    {
        bool canSwitch = true;
        try
        {
            DDVessel tempParentVessel = ddUnitIn.StartParent.GetComponent<DDVessel>();
            canSwitch = tempParentVessel.CanTagDropIn(ddUnitOut);
        }
        catch { }

        return canSwitch;
    }
}
