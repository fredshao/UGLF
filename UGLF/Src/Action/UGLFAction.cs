using System;
using System.Collections.Generic;

public class UGLFAction : IDisposable
{
    private bool _actionStarted = false;

    public UGLFAction()
    {
        UGLF.AddUpdateCallback(_Internal_Tick);
    }


    public void EnterAction()
    {
        _actionStarted = true;
        OnEnterAction();
    }

    public virtual void OnEnterAction()
    {

    }

    private void _Internal_Tick()
    {
        if (_actionStarted)
        {
            UpdateAction();
        }
    }


    public virtual void UpdateAction()
    {

    }

    public void ExitAction()
    {
        _actionStarted = false;
        OnExitAction();
    }

    public virtual void OnExitAction()
    {

    }

    ~UGLFAction()
    {

    }

    private void Dispose(bool _disposing)
    {
        
    }

    public virtual void Dispose()
    {

    }

}
