using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransform : MonoBehaviour
{
    public GameObject gameObj;
    [SerializeField] public float xDist, yDist, zDist;
    [SerializeField] public float xAngle, yAngle, zAngle;


    public void ObjTransform() 
    {
        gameObj.transform.Translate(xDist, yDist, zDist);
        gameObj.transform.Rotate(xAngle, yAngle, zAngle, Space.Self);
    }
}
