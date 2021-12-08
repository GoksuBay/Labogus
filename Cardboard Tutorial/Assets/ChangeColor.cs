using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    
    public void Red() 
            {
                GetComponent<Renderer>().material.color = Color.red;
            }

    public void Blue() 
            {
                GetComponent<Renderer>().material.color = Color.blue;
            }
    
    public void Black() 
            {
                GetComponent<Renderer>().material.color = Color.black;
            }

}
