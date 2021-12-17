using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AllLerpTestClass : MonoBehaviour
{
    [SerializeField] float timer;

    //Transform, yani animasyon tamamlanma süresi totalTime oluyor.
    [SerializeField] public float totalTime = 2;

    [SerializeField] private Transform startTransform = null, endTransform = null;
    
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
        transform.position = Vector3.Lerp(startTransform.position, endTransform.position, lerpValue);
        transform.rotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, lerpValue);
        transform.localScale = Vector3.Lerp(startTransform.localScale, endTransform.localScale, lerpValue);
        
        _meshRenderer.material.color = Color.Lerp(_startObjectMeshRenderer.material.color, _endObjectMeshRenderer.material.color, lerpValue);


        //lerpValue max değeri olan 1'e ulaştıktan sonra, timer'ın değeri arkaplanda artmaya devam etmesin diye 100'e eşitliyoruz.
        //arkaplanda boşuna çalışmaması için yaptım.
        if(lerpValue <= 1)
        {
            timer += Time.deltaTime;
            lerpValue = timer/totalTime;
        }
        else
        {
            timer = 100;
        }
       
    
    }
}