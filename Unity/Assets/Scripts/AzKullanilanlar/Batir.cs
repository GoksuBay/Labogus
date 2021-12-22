using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Batir : MonoBehaviour
{
   [SerializeField] float timer;

    //Transform, yani animasyon tamamlanma süresi totalTime oluyor.
    [SerializeField] public float totalTime = 2;

    [SerializeField] private Transform startTransform = null, endTransform = null;
     private bool flag = false;
     private bool checkAnimation = false;

    //lerpValue = 0 olduğunda başlangıç pozisyonu, lerpValue = 1 olduğunda bitiş pozisyonu oluyor.
    [SerializeField] [Range(0f, 1f)] private float lerpValue = 0;

    private MeshRenderer _startObjectMeshRenderer, _endObjectMeshRenderer;

    public MeshRenderer _meshRenderer;
    public Material material;

    public void Update()
    {



    if (flag == true){

        if (!checkAnimation){
        _meshRenderer.transform.position = Vector3.Lerp(startTransform.position, endTransform.position, lerpValue);
        _meshRenderer.transform.rotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, lerpValue);
        _meshRenderer.transform.localScale = Vector3.Lerp(startTransform.localScale, endTransform.localScale, lerpValue);
        }

        else{
        _meshRenderer.transform.position = Vector3.Lerp(endTransform.position, startTransform.position, lerpValue);
        _meshRenderer.transform.rotation = Quaternion.Lerp(endTransform.rotation, startTransform.rotation, lerpValue);
        _meshRenderer.transform.localScale = Vector3.Lerp(endTransform.localScale, startTransform.localScale, lerpValue);
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
            _meshRenderer.material = material;
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