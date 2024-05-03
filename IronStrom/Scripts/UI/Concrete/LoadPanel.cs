using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPanel : BasePanel
{
    static readonly string path = "UI/Prefabs/LoadPanel";
    public LoadPanel() : base(new UIType(path)) { }

    public override void OnEnter()
    {
        var audioMager = AudioManager.Instance;
        if (audioMager == null) return;

        audioMager.MusicSource.Stop();

    }

    public override void OnExit()
    {
        _UIManager.DestroyUI(_UIType);
    }

}
