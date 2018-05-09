using System.Collections;using System.Collections.Generic;using UnityEngine;public class UGLFLayerUtil {    /// <summary>    /// 显示某一层    /// </summary>    /// <param name="_layer">要显示的层</param>    /// <param name="_camera">如果 Camera 为空，则操作 MainCamera </param>    public static void EnableLayer(int _layer, Camera _camera = null)    {        Camera camera = _camera;        if(camera == null)        {            camera = Camera.main;        }        camera.cullingMask |= ~(1 << _layer);    }    /// <summary>    /// 显示某一层    /// </summary>    /// <param name="_layerName">层的名字</param>    /// <param name="_camera">如果 Camera 为空，则操作 MainCamera </param>    public static void EnableLayer(string _layerName, Camera _camera = null)    {        EnableLayer(LayerMask.NameToLayer(_layerName), _camera);    }    /// <summary>    /// 隐藏某一层    /// </summary>    /// <param name="_layer">要隐藏的层</param>    /// <param name="_camera"> 如果 Camera 为空则操作 MainCamera </param>    public static void DisableLayer(int _layer, Camera _camera = null)    {        Camera camera = _camera;        if (camera == null)        {            camera = Camera.main;        }        camera.cullingMask &= ~(1 << _layer);    }    /// <summary>    /// 隐藏某一层    /// </summary>    /// <param name="_layerName">层的名字/param>    /// <param name="_camera"> 如果 Camera 为空则操作 MainCamera </param>    public static void DisableLayer(string _layerName, Camera _camera = null)    {        DisableLayer(LayerMask.NameToLayer(_layerName), _camera);    }    /// <summary>
    /// 设置一个GameObject的层
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_layer"></param>    public static void SetGameObjectLayer(GameObject _obj, int _layer)
    {
        if(_obj == null)
        {
            return;
        }

        _obj.layer = _layer;
        Transform[] childs = _obj.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; ++i)
        {
            childs[i].gameObject.layer = _layer;
        }
    }    /// <summary>
    /// 设置一个GameObject的层
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_layerName"></param>    public static void SetGameObjectLayer(GameObject _obj, string _layerName)
    {
        int layer = LayerMask.NameToLayer(_layerName);
        SetGameObjectLayer(_obj, layer);
    }    public bool IsLayerRendered(Camera _camera, int _layer)
    {
        Camera camera = _camera;
        if(camera == null)
        {
            camera = Camera.main;
        }

        return ((camera.cullingMask & (1 << _layer)) != 0);
    }}