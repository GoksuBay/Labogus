using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeherBosalt : MonoBehaviour
{
    public Material[] material;
    public GameObject icerik;
    public GameObject degienIcerik;
    void Start()
    {
        //GetComponent<Renderer>().material = material[0];
        icerik.GetComponent<Renderer>().material = material[0];
    }

    public void BeheriBosalt()
    {
        
        //GetComponent<Renderer>().material = material[1];
        icerik.GetComponent<Renderer>().material = material[1];
        delay();
        degienIcerik.GetComponent<Renderer>().material = material[1];
        
    }

    private IEnumerator delay(){
        yield return new WaitForSeconds(10*Time.deltaTime);
    }
}
