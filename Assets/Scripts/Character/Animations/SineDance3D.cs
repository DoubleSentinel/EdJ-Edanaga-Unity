using System.Diagnostics;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class SineDance3D : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 startRotation;
    private Vector3 startScale;

    void Start()
    {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
        startRotation = transform.localRotation.eulerAngles;

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
    public bool rotateX;
    public float sinRotSpeedX = 5;
    public float sinRotAmpX = 5;

    public bool rotateY;
    public float sinRotSpeedY = 5;
    public float sinRotAmpY = 5;

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

    public bool scaleZ;
    public float sinSizeAmpZ = 1;
    public float sinSizeSpeedZ = 1;

    //MOVE
    public bool displayMove;
    public bool move;

    public bool moveX;
    public float sinXSpeed = 1;
    public float sinXAmp = 1;

    public bool moveY;
    public float sinYSpeed = 1;
    public float sinYAmp = 1;

    public bool moveZ;
    public float sinZSpeed = 1;
    public float sinZAmp = 1;

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
            float newRotationX = startRotation.x;
            float newRotationY = startRotation.y;
            float newRotationZ = startRotation.z;
            
            if (rotateX)
            {
                newRotationX = startRotation.x + sinRotAmpX * Mathf.Sin((Time.time + offsetAmount) * sinRotSpeedX);
            }

            if (rotateY)
            {
                newRotationY = startRotation.y + sinRotAmpY * Mathf.Sin((Time.time + offsetAmount) * sinRotSpeedY);
            }

            if (rotateZ)
            {
                newRotationZ = startRotation.z + sinRotAmpZ * Mathf.Sin((Time.time + offsetAmount) * sinRotSpeedZ);
            }

            Vector3 rotationVector = new Vector3(newRotationX, newRotationY, newRotationZ);
            Quaternion rotation = Quaternion.Euler(rotationVector);
            transform.localRotation = rotation;
        }

        if (scale)
        {
            float newScaleX = transform.localScale.x;
            float newScaleY = transform.localScale.y;
            float newScaleZ = transform.localScale.z;
            
            if (scaleX)
            {
                newScaleX = (startScale.x + (sinSizeAmpX * Mathf.Sin((Time.time + offsetAmount) * sinSizeSpeedX)) / 10);
            }
            if (scaleY)
            {
                newScaleY = (startScale.y + (sinSizeAmpY * Mathf.Sin((Time.time + offsetAmount) * sinSizeSpeedY)) / 10);
            }

            if (scaleZ)
            {
                newScaleZ = (startScale.z + (sinSizeAmpZ * Mathf.Sin((Time.time + offsetAmount) * sinSizeSpeedZ)) / 10);
            }

            if (scaleLock)
            {
                newScaleY = newScaleX;
                newScaleZ = newScaleX;
            }

            Vector3 scaleVector = new Vector3(newScaleX, newScaleY, newScaleZ);
            transform.localScale = scaleVector;
        }

        if (move)
        {
            float newX = transform.localPosition.x;
            float newY = transform.localPosition.y;
            float newZ = transform.localPosition.z;
            
            if (moveX)
            {
                newX = startPosition.x + (sinXAmp * Mathf.Sin((Time.time + offsetAmount) * sinXSpeed));
            }

            if (moveY)
            {
                newY = startPosition.y + (sinYAmp * Mathf.Sin((Time.time + offsetAmount) * sinYSpeed));
            }

            if (moveZ)
            {
                newZ = startPosition.z + (sinZAmp * Mathf.Sin((Time.time + offsetAmount) * sinYSpeed));
            }

            transform.localPosition = new Vector3(newX, newY, newZ);
        }
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(SineDance3D))]
public class SinDance3DEditor : Editor
{
    override public void OnInspectorGUI()
    {
       var sinDance = target as SineDance3D;

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
                sinDance.rotateX = GUILayout.Toggle(sinDance.rotateX, "Rotate On X");

                if (sinDance.rotateX)
                {
                    sinDance.sinRotAmpX = EditorGUILayout.FloatField("X Rotation Amount: ", sinDance.sinRotAmpX);
                    sinDance.sinRotSpeedX = EditorGUILayout.FloatField("X Rotation Speed: ", sinDance.sinRotSpeedX);
                }

                sinDance.rotateY = GUILayout.Toggle(sinDance.rotateY, "Rotate On Y");

                if (sinDance.rotateY)
                {
                    sinDance.sinRotAmpY = EditorGUILayout.FloatField("Y Rotation Amount: ", sinDance.sinRotAmpY);
                    sinDance.sinRotSpeedY = EditorGUILayout.FloatField("Y Rotation Speed: ", sinDance.sinRotSpeedY);
                }

                sinDance.rotateZ = GUILayout.Toggle(sinDance.rotateZ, "Rotate On Z");

                if (sinDance.rotateZ)
                {
                    sinDance.sinRotAmpZ = EditorGUILayout.FloatField("Z Rotation Amount: ", sinDance.sinRotAmpZ);
                    sinDance.sinRotSpeedZ = EditorGUILayout.FloatField("Z Rotation Speed: ", sinDance.sinRotSpeedZ);
                }
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
                sinDance.scaleLock = GUILayout.Toggle(sinDance.scaleLock, "Lock X Y Z Scale");

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

                    sinDance.scaleZ = GUILayout.Toggle(sinDance.scaleZ, "Scale Z");

                    if (sinDance.scaleZ)
                    {
                        sinDance.sinSizeAmpZ = EditorGUILayout.FloatField("Scale Z Amount: ", sinDance.sinSizeAmpZ);
                        sinDance.sinSizeSpeedZ = EditorGUILayout.FloatField("Scale Z Speed: ", sinDance.sinSizeSpeedZ);
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

                sinDance.moveZ = GUILayout.Toggle(sinDance.moveZ, "Move Z");

                if (sinDance.moveZ)
                {
                    sinDance.sinZAmp = EditorGUILayout.FloatField("Movement Amount: ", sinDance.sinZAmp);
                    sinDance.sinZSpeed = EditorGUILayout.FloatField("Movement Speed: ", sinDance.sinZSpeed);
                }
            }
        }

       

        EditorGUI.indentLevel--;
    }
}
