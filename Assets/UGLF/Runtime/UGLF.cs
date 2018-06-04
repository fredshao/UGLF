using System;
using System.Collections.Generic;
using UnityEngine;

public class UGLF : MonoBehaviour {

    private static UGLF _inst;
    public static UGLF Inst {
        get {
            if(_inst == null)
            {
                GameObject obj = new GameObject("_UGLF");
                _inst = obj.AddComponent<UGLF>();
                DontDestroyOnLoad(obj);
                _inst.Init();
            }
            return _inst;
        }
    }

    /// <summary>
    /// 3D GameObject交互器
    /// </summary>
    private UGLF3DInterator _3dInterator = null;
    public UGLF3DInterator interator3D {
        get { return _3dInterator; }
    }



    private void Init()
    {
        _3dInterator = gameObject.AddComponent<UGLF3DInterator>();
    }



    public static void AddUpdateCallback(Action _callback) {

    }

    public static void AddFixedUpdateCallback(Action _callback) {

    }

    public static void AddLateUpdateCallback(Action _callback) {

    }
}
