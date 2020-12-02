using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNDropControler : MonoBehaviour
{

    public Transform DDMatrice0_2;
    public Transform DDMatrice3_5;
    
    void Start()
    {
        StartCoroutine(onCoroutine());
    }

    IEnumerator onCoroutine()
    {
        while (true)
        {
            foreach (Transform child in DDMatrice0_2)
                Debug.Log("Foreach loop1: " + child);
            foreach (Transform child in DDMatrice3_5)
                print("Foreach loop2: " + child);

            //Debug.Log("OnCoroutine: " + (int)Time.time);
            yield return new WaitForSeconds(1f);
        }
    }
}
