using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UGLFHitObjectDelegate(GameObject _hitedObj);

public class UGLF3DInterator : MonoBehaviour {

    



    private event UGLFHitObjectDelegate onHitGameObject;

    public void AddListener(UGLFHitObjectDelegate _callback)
    {
        onHitGameObject += _callback;
    }

    public void RemoveListener(UGLFHitObjectDelegate _callback)
    {
        onHitGameObject -= _callback;
    }

    private void Update()
    {
        
    }



}
