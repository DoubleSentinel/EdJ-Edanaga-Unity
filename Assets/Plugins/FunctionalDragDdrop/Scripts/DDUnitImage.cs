using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDUnitImage : MonoBehaviour
{
    // defines how the Unit follows the pointer while dragged
    public enum TranslationFunctionItems { none, lerp };
    public TranslationFunctionItems translationFunction;
    public float translationSpeed; // defines the speed that the Unit follows the pointer using lerp on 'mouseFollowFunction'

    // defines the function that rotates the Unit
    public enum RotationFunctionItems { none, lerp };
    public RotationFunctionItems rotationFunction;
    public float rotationSpeed;

    // defines the function that "animates" the Unit while being dragged
    // OBS.: enabled only if 'rotationFunction' = lerp
    public enum OnDragAnimationItems { none, rotateToPointer };
    public OnDragAnimationItems onDragAnimation;
    public float rotateToPointerMaxAngle;

    // defines the function that resizes the Unit
    public enum ResizeFunctionItems { none, lerp };
    public ResizeFunctionItems resizeFunction;
    public float resizeSpeed;

    // --- Unit Image Control variables---
    private float x;
    private float y;
    private DDUnit ddUnit;
    private bool enableResize;

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

    private void Start()
    {
        enableResize = false;
        StartCoroutine(EnableResize(0.1f));
        ddUnit = transform.parent.GetComponent<DDUnit>();
        ddUnit.DdUnitImage = this;

        transform.SetParent(ddUnit.ddCanvas.canvasImages.transform);
    }

    // this method combined with the MatchOther are responsible for keeping the Units consistent on the start of the game/app
    // the UnitImage is resized correctly right before the lerp function is available so it does not glitches in the beginning
    IEnumerator EnableResize(float time)
    {
        yield return new WaitForSeconds(time);
        enableResize = true;
    }

    // places rect transform to have the same dimensions as 'other', even if they don't have same parent.
    // also modifies scale of rectTransf to match the scale of other
    // mehotd from IgorAherne's post on Unity Forum.
    public void MatchOther(RectTransform rt, RectTransform other)
    {
        Vector2 myPrevPivot = rt.pivot;
        myPrevPivot = other.pivot;
        rt.position = other.position;
        rt.localScale = other.localScale;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, other.rect.width);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, other.rect.height);
        rt.pivot = myPrevPivot;
    }

    void Update()
    {
        if (ddUnit.IsBeingDragged)
        {
            transform.SetParent(ddUnit.ddCanvas.canvasOnDrag.transform);
        }
        else
        {
            transform.SetParent(ddUnit.ddCanvas.canvasImages.transform); 
        }

        if (translationFunction == TranslationFunctionItems.none)
        {
            transform.position = ddUnit.transform.position;
        }
        else if (translationFunction == TranslationFunctionItems.lerp)
        {
            transform.position = Vector3.Lerp(transform.position, ddUnit.transform.position, Time.deltaTime * translationSpeed);
        }

        if(enableResize == false)
        {
            MatchOther(GetComponent<RectTransform>(), ddUnit.GetComponent<RectTransform>());
        }
        else
        {
            if (resizeFunction == ResizeFunctionItems.none)
            {
                GetComponent<RectTransform>().sizeDelta = ddUnit.GetComponent<RectTransform>().sizeDelta; 
            }
            else if (resizeFunction == ResizeFunctionItems.lerp)
            {
                GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(GetComponent<RectTransform>().sizeDelta, ddUnit.GetComponent<RectTransform>().sizeDelta,
                Time.deltaTime * resizeSpeed);
            }
        }

        if (ddUnit.IsDroppedIn == false)
        {
            if (rotationFunction == RotationFunctionItems.none)
            {
                transform.rotation = ddUnit.transform.rotation;
            }
            else if (rotationFunction == RotationFunctionItems.lerp)
            {
                if (onDragAnimation == OnDragAnimationItems.rotateToPointer)
                {
                    //algorithm to make the Unit face the mouse, making a smooth animation when dragged
                    Vector3 a = ddUnit.transform.position - transform.position;
                    float mult = 2;
                    x = a.y * mult;
                    y = a.x * mult;
                    if (x >= rotateToPointerMaxAngle)
                        x = rotateToPointerMaxAngle;
                    if (x <= -rotateToPointerMaxAngle)
                        x = -rotateToPointerMaxAngle;
                    if (y >= rotateToPointerMaxAngle)
                        y = rotateToPointerMaxAngle;
                    if (y <= -rotateToPointerMaxAngle)
                        y = -rotateToPointerMaxAngle;
                }

                Quaternion newRotation = Quaternion.Euler(ddUnit.transform.rotation.eulerAngles.x + x, ddUnit.transform.rotation.eulerAngles.y - y, ddUnit.transform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            }

        }
        else if (ddUnit.IsDroppedIn == true)
        {
            if (rotationFunction == RotationFunctionItems.none)
            {
                transform.rotation = ddUnit.transform.rotation;
            }
            else if (rotationFunction == RotationFunctionItems.lerp)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, ddUnit.transform.rotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
