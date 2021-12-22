using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class VRButton : MonoBehaviour
{
    public Image imgGaze;
    public UnityEvent GVRClick;
    public float totalTime = 1;
    bool gvrStatus;
    public float gvrTimer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gvrStatus)
        {
            gvrTimer += Time.deltaTime;
            imgGaze.fillAmount = gvrTimer/totalTime;
        }

        if(gvrTimer > totalTime)
        {
            GVRClick.Invoke();
        }
    }

    public void GVROn()
    {
        gvrStatus = true;
    }

    public void GVROff()
    {
        gvrStatus = false;
        gvrTimer = 0;
        imgGaze.fillAmount = 0;
    }
}
