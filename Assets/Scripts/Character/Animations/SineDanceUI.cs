using System.Diagnostics;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SineDanceUI : MonoBehaviour
{
    private RectTransform rect;
    private Vector3 startPosition;
    private Vector3 startRotation;
    private Vector3 startScale;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        
        startPosition = rect.anchoredPosition;
        startScale = rect.localScale;
        startRotation = rect.rotation.eulerAngles;

        autoOffsetAmount = Random.Range(0, 100);
    }

    //OFFSET
    public bool displayOffset;
    public bool offset;
    public bool autoOffset = true;
    private float autoOffsetAmount;
    public float offsetAmount;

    //ROTATE
    public bool displayRotate;
    public bool rotate;

    public bool rotateZ;
    public float sinRotSpeedZ = 5;
    public float sinRotAmpZ = 5;

    //SCALE
    public bool displayScale;

    public bool scale;
    public bool scaleLock = true;

    public bool scaleX = true;
    public float sinSizeAmpX = 1;
    public float sinSizeSpeedX = 1;

    public bool scaleY;
    public float sinSizeAmpY = 1;
    public float sinSizeSpeedY = 1;

    //MOVE
    public bool displayMove;
    public bool move;

    public bool moveX;
    public float sinXSpeed = 1;
    public float sinXAmp = 1;

    public bool moveY;
    public float sinYSpeed = 1;
    public float sinYAmp = 1;


    void Update()
    {
        if (offset)
        {
            if (autoOffset)
            {
                offsetAmount = autoOffsetAmount;
            }
        }

        if (rotate)
        {
            Vector3 rotationVector = new Vector3(rect.rotation.x, rect.rotation.y, startRotation.z + sinRotAmpZ * Mathf.Sin((Time.time + offsetAmount) * sinRotSpeedZ));
            Quaternion rotation = Quaternion.Euler(rotationVector);
            rect.localRotation = rotation;
        }

        if (scale)
        {
            float newScaleX = rect.localScale.x;
            float newScaleY = rect.localScale.y;
            float newScaleZ = rect.localScale.z;

            if (scaleX)
            {
                newScaleX = (startScale.x + (sinSizeAmpX * Mathf.Sin((Time.time + offsetAmount) * sinSizeSpeedX)) / 10);
            }
            if (scaleY)
            {
                newScaleY = (startScale.y + (sinSizeAmpY * Mathf.Sin((Time.time + offsetAmount) * sinSizeSpeedY)) / 10);
            }


            if (scaleLock)
            {
                newScaleY = newScaleX;
            }

            Vector3 scaleVector = new Vector3(newScaleX, newScaleY, newScaleZ);
            rect.localScale = scaleVector;
        }

        if (move)
        {
            float newX = rect.anchoredPosition.x;
            float newY = rect.anchoredPosition.y;
            
            if (moveX)
            {
                newX = startPosition.x + (sinXAmp * Mathf.Sin((Time.time + offsetAmount) * sinXSpeed));
            }

            if (moveY)
            {
                newY = startPosition.y + (sinYAmp * Mathf.Sin((Time.time + offsetAmount) * sinYSpeed));
            }

            rect.anchoredPosition = new Vector3(newX, newY, transform.localPosition.z);

        }
    }
}

[CustomEditor(typeof(SineDanceUI))]
public class SinDanceUIEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var sinDance = target as SineDanceUI;

        EditorGUI.indentLevel++;

        //OFFSET
        EditorGUILayout.Space(10);
        sinDance.displayOffset = EditorGUILayout.Foldout(sinDance.displayOffset, "Offset");

        if (sinDance.displayOffset)
        {
            sinDance.offset = GUILayout.Toggle(sinDance.offset, "Enable Offset");
            EditorGUILayout.Space(3);

            if (sinDance.offset)
            {
                sinDance.autoOffset = GUILayout.Toggle(sinDance.autoOffset, "Auto Offset");


                if (!sinDance.autoOffset)
                {
                    sinDance.offsetAmount = EditorGUILayout.FloatField("Offset Amount: ", sinDance.offsetAmount);
                }
            }
        }

        EditorGUI.indentLevel--;
        EditorGUI.indentLevel++;

        //ROTATE
        EditorGUILayout.Space(10);
        sinDance.displayRotate = EditorGUILayout.Foldout(sinDance.displayRotate, "Rotation");

        if (sinDance.displayRotate)
        {
            sinDance.rotate = GUILayout.Toggle(sinDance.rotate, "Enable Rotation");
            EditorGUILayout.Space(3);

            if (sinDance.rotate)
            {
                sinDance.sinRotAmpZ = EditorGUILayout.FloatField("Z Rotation Amount: ", sinDance.sinRotAmpZ);
                sinDance.sinRotSpeedZ = EditorGUILayout.FloatField("Z Rotation Speed: ", sinDance.sinRotSpeedZ);
            }
        }


        EditorGUI.indentLevel--;
        EditorGUI.indentLevel++;

        //SCALE
        EditorGUILayout.Space(10);
        sinDance.displayScale = EditorGUILayout.Foldout(sinDance.displayScale, "Scale");

        if (sinDance.displayScale)
        {
            sinDance.scale = GUILayout.Toggle(sinDance.scale, "Enable Scale");
            EditorGUILayout.Space(3);

            if (sinDance.scale)
            {
                sinDance.scaleLock = GUILayout.Toggle(sinDance.scaleLock, "Lock X Y Scale");

                sinDance.scaleX = GUILayout.Toggle(sinDance.scaleX, "Scale X");

                if (sinDance.scaleX)
                {
                    sinDance.sinSizeAmpX = EditorGUILayout.FloatField("Scale X Amount: ", sinDance.sinSizeAmpX);
                    sinDance.sinSizeSpeedX = EditorGUILayout.FloatField("Scale X Speed: ", sinDance.sinSizeSpeedX);
                }

                if (!sinDance.scaleLock)
                {
                    sinDance.scaleY = GUILayout.Toggle(sinDance.scaleY, "Scale Y");

                    if (sinDance.scaleY)
                    {
                        sinDance.sinSizeAmpY = EditorGUILayout.FloatField("Scale Y Amount: ", sinDance.sinSizeAmpY);
                        sinDance.sinSizeSpeedY = EditorGUILayout.FloatField("Scale Y Speed: ", sinDance.sinSizeSpeedY);
                    }
                }
            }
        }

        EditorGUI.indentLevel--;
        EditorGUI.indentLevel++;


        //POSITION
        EditorGUILayout.Space(10);
        sinDance.displayMove = EditorGUILayout.Foldout(sinDance.displayMove, "Position");

        if (sinDance.displayMove)
        {
            sinDance.move = GUILayout.Toggle(sinDance.move, "Enable Position Movement");
            EditorGUILayout.Space(3);

            if (sinDance.move)
            {
                //MOVE X
                sinDance.moveX = GUILayout.Toggle(sinDance.moveX, "Move X");

                if (sinDance.moveX)
                {
                    sinDance.sinXAmp = EditorGUILayout.FloatField("Movement Amount: ", sinDance.sinXAmp);
                    sinDance.sinXSpeed = EditorGUILayout.FloatField("Movement Speed: ", sinDance.sinXSpeed);
                }

                //MOVE Y
                sinDance.moveY = GUILayout.Toggle(sinDance.moveY, "Move Y");

                if (sinDance.moveY)
                {
                    sinDance.sinYAmp = EditorGUILayout.FloatField("Movement Amount: ", sinDance.sinYAmp);
                    sinDance.sinYSpeed = EditorGUILayout.FloatField("Movement Speed: ", sinDance.sinYSpeed);
                }
            }
        }

        EditorGUI.indentLevel--;
    }
}
