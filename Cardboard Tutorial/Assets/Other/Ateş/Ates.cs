using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ates : MonoBehaviour


{
    public Material ates;
    public void changeColor(Material element){
        switch (element.name){
            case "NaCl":
                ates.color = Color.yellow;
                break;
            case "CuSO4":
                ates.color = Color.green;
                break;
            case "KCL":
                ates.color = Color.blue;
                break;
            case "CaCl2":
                ates.color = Color.red;
                break;
            default:
                break;
        }
    }
}
