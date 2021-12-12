using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public GameObject player;

    public void ChangeSpin() 
    {
        player.transform.Rotate(0f, 350 * Time.deltaTime, 0f);
    }
}
