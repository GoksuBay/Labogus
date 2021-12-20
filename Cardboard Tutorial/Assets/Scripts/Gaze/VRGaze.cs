using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRGaze : MonoBehaviour
{
    public Image imgGaze;

    public float totalTime = 2;
    bool gvrStatus;
    float gvrTimer;

    public int distanceOfRay = 10;
    private RaycastHit _hit;

    // Start is called before the first frame update
    void Start()
    {
        totalTime = 1.0f;   
    }

    // Update is called once per frame
    void Update()
    {
        if (gvrStatus)
        {
            gvrTimer += Time.deltaTime;
            imgGaze.fillAmount = gvrTimer/totalTime;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));

        if(Physics.Raycast(ray, out _hit,distanceOfRay))
        {
            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("Teleport"))
            {
                _hit.transform.gameObject.GetComponent<Teleport>().TeleportPlayer();
            }

            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("RotateCube") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<RotateCube>().ChangeSpin();
                gvrStatus = false;
            }

            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("Obje") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<ObjectTransform>().ObjTransform();
                gvrStatus = false;
            }

            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("Beher") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<BeherBosalt>().BeheriBosalt();
                gvrStatus = false;
            }

            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("Lerp") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<AllLerpTestClass>().Lerper();
                gvrStatus = false;
            }

            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("ChangeScene") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<ChangeScene>().SceneChange();
                gvrStatus = false;
            }

            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("lerporiginal") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<LerpOriginal>().Lerper();
                gvrStatus = false;
            }
            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("Beherbalon") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<LerpBalon>().Lerper();
                gvrStatus = false;
            }

            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("Turnusol") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<Turnusol>().Lerper();
                gvrStatus = false;
            }

            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("Element") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<Batir>().Lerper();
                gvrStatus = false;
            }

            if(imgGaze.fillAmount == 1 && _hit.transform.CompareTag("AtesLerp") && gvrStatus)
            {
                _hit.transform.gameObject.GetComponent<AtesliLerp>().Lerper();
                gvrStatus = false;
            }

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
