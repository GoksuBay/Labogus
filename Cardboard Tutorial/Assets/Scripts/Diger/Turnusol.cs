using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turnusol : MonoBehaviour
{
    public Material turnusol;

    public void changeColor(Maddeler material){
        if(material.pH > 7) {turnusol.color = Color.blue;}
        else {turnusol.color = Color.red;}
    }
}
