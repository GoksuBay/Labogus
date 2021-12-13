using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoluSise : MonoBehaviour
{
    public GameObject doluSise;
    

    [SerializeField] public float xDist, yDist, zDist;
    [SerializeField] public float xAngle, yAngle, zAngle;

    public void KaldirDok() 
    {
        doluSise.transform.Translate(xDist, yDist, zDist);
        doluSise.transform.Rotate(xAngle, yAngle, zAngle, Space.Self);
        
        
    }

    
}
