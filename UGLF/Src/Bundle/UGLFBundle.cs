using UnityEngine;
using System.Collections;

public class UGLFBundle : UGLFObject {

    private string remoteUrl = string.Empty;

    public UGLFBundle() : base(false) {

    }

    public void StartUpdate(string _remoteUrl) {
        remoteUrl = _remoteUrl;
        base.Start();
    }

    public override void Run() {
        base.Run();

    }

}
