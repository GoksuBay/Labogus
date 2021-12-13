using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public GameObject player;
    [SerializeField] public float xDist, yDist, zDist;
    [SerializeField] public float xAngle, yAngle, zAngle;


    public void ChangeSpin() 
    {
        player.transform.Translate(xDist, yDist, zDist);
        player.transform.Rotate(xAngle, yAngle, zAngle, Space.Self);
    }
}
