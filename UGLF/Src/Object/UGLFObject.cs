using System;
using System.Collections.Generic;

public class UGLFObject {
    private bool started = false;

    public UGLFObject(bool _autoStart = true) {
        started = _autoStart;
        UGLF.AddUpdateCallback(_Tick);
    }

    public void Start() {
        started = true;
    }

    public void Stop() {
        started = false;
    }

    public virtual void Run() {

    }

    private void _Tick() {
        if (started == false) {
            return;
        }
        Run();
    }
}
