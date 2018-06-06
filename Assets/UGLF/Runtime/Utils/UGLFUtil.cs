using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UGLFUtil
{
    #region 层级操作
    /// <summary>
    /// 显示某一层
    /// </summary>
    /// <param name="_layer">要显示的层</param>
    /// <param name="_camera">如果 Camera 为空，则操作 MainCamera </param>
    public static void EnableLayer(int _layer, Camera _camera = null)
    {
        Camera camera = _camera;
        if (camera == null)
        {
            camera = Camera.main;
        }

        camera.cullingMask |= ~(1 << _layer);
    }

    /// <summary>
    /// 显示某一层
    /// </summary>
    /// <param name="_layerName">层的名字</param>
    /// <param name="_camera">如果 Camera 为空，则操作 MainCamera </param>
    public static void EnableLayer(string _layerName, Camera _camera = null)
    {
        EnableLayer(LayerMask.NameToLayer(_layerName), _camera);
    }

    /// <summary>
    /// 隐藏某一层
    /// </summary>
    /// <param name="_layer">要隐藏的层</param>
    /// <param name="_camera"> 如果 Camera 为空则操作 MainCamera </param>
    public static void DisableLayer(int _layer, Camera _camera = null)
    {
        Camera camera = _camera;
        if (camera == null)
        {
            camera = Camera.main;
        }

        camera.cullingMask &= ~(1 << _layer);
    }

    /// <summary>
    /// 隐藏某一层
    /// </summary>
    /// <param name="_layerName">层的名字/param>
    /// <param name="_camera"> 如果 Camera 为空则操作 MainCamera </param>
    public static void DisableLayer(string _layerName, Camera _camera = null)
    {
        DisableLayer(LayerMask.NameToLayer(_layerName), _camera);
    }

    /// <summary>
    /// 设置一个GameObject的层
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_layer"></param>
    public static void SetGameObjectLayer(GameObject _obj, int _layer)
    {
        if (_obj == null)
        {
            return;
        }

        _obj.layer = _layer;
        Transform[] childs = _obj.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; ++i)
        {
            childs[i].gameObject.layer = _layer;
        }
    }

    /// <summary>
    /// 设置一个GameObject的层
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_layerName"></param>
    public static void SetGameObjectLayer(GameObject _obj, string _layerName)
    {
        int layer = LayerMask.NameToLayer(_layerName);
        SetGameObjectLayer(_obj, layer);
    }

    /// <summary>
    /// 一个层是否被渲染
    /// </summary>
    /// <param name="_camera"></param>
    /// <param name="_layer"></param>
    /// <returns></returns>
    public static bool IsLayerRendered(Camera _camera, int _layer)
    {
        Camera camera = _camera;
        if (camera == null)
        {
            camera = Camera.main;
        }

        return ((camera.cullingMask & (1 << _layer)) != 0);
    }
    #endregion


    # region UI相关
    public static bool IsPointerOverGameObject()
    {
        PointerEventData eventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;

        List<RaycastResult> list = new List<RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, list);
        return list.Count > 0;
    }

    #endregion


    #region 动画相关
    public static bool IsAnimPlaying(Animator _animator, string _animName)
    {
        AnimatorClipInfo[] clips = _animator.GetCurrentAnimatorClipInfo(0);
        if (clips != null && clips.Length > 0)
        {
            if (clips[0].clip.name.ToLower().Contains(_animName.ToLower()))
            {
                AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.normalizedTime < 1.0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    #endregion


}
