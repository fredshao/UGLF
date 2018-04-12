﻿using System.Collections;
    /// 设置一个GameObject的层
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_layer"></param>
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
    }
    /// 设置一个GameObject的层
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_layerName"></param>
    {
        int layer = LayerMask.NameToLayer(_layerName);
        SetGameObjectLayer(_obj, layer);
    }