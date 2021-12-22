using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllLerpTestClass : MonoBehaviour
{
    [SerializeField] float timer;

    //Transform, yani animasyon tamamlanma süresi totalTime oluyor.
    [SerializeField] public float totalTime = 1;

    [SerializeField] private Transform startTransform = null, endTransform = null;
     private bool flag = false;
     private bool checkAnimation = false;

    //lerpValue = 0 olduğunda başlangıç pozisyonu, lerpValue = 1 olduğunda bitiş pozisyonu oluyor.
    [SerializeField] [Range(0f, 1f)] private float lerpValue = 0;

    private MeshRenderer _meshRenderer, _startObjectMeshRenderer, _endObjectMeshRenderer;

    private void OnValidate()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _startObjectMeshRenderer = startTransform.GetComponent<MeshRenderer>();
        _endObjectMeshRenderer = endTransform.GetComponent<MeshRenderer>();
    }

    public void Update()
    {

        

    if (flag == true){

        if (!checkAnimation){
        transform.position = Vector3.Lerp(startTransform.position, endTransform.position, lerpValue);
        transform.rotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, lerpValue);
        transform.localScale = Vector3.Lerp(startTransform.localScale, endTransform.localScale, lerpValue);
        }

        else{
        transform.position = Vector3.Lerp(endTransform.position, startTransform.position, lerpValue);
        transform.rotation = Quaternion.Lerp(endTransform.rotation, startTransform.rotation, lerpValue);
        transform.localScale = Vector3.Lerp(endTransform.localScale, startTransform.localScale, lerpValue);
        }

        //lerpValue max değeri olan 1'e ulaştıktan sonra, timer'ın değeri arkaplanda artmaya devam etmesin diye 100'e eşitliyoruz.
        //arkaplanda boşuna çalışmaması için yaptım.
        if(lerpValue <= 1)
        {
            timer += Time.deltaTime;
            lerpValue = timer/totalTime;
        }

        else if (lerpValue >= 1 && !checkAnimation){
            lerpValue = 0;
            timer = 0;
            checkAnimation = true;
            GetComponent<BeherBosalt>().BeheriBosalt();
        }
        else
        {
            timer = 100;
        }

    if(timer >=100)
    {
        flag = false;
    }
}
    }

    public void Lerper()
    {
        flag = true;


    }


    }