using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UGLFHitObjectDelegate(GameObject _hitedObj);

/// <summary>
/// 用于检测世界中哪个物体被点击了，如果点击了UI，则不触发3D物体点击回调事件
/// </summary>
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
        if (onHitGameObject == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (UGLFUtil.IsPointerOverGameObject())
            {
                return;
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                for (int i = 0; i < Input.touchCount; ++i)
                {
                    if (UGLFUtil.IsPointerOverGameObject())
                    {
                        return;
                    }
                }

            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider != null)
                {
                    onHitGameObject(hit.collider.gameObject);
                }
            }
        }
    }

}
