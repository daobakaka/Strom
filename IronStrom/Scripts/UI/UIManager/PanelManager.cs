using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//面板管理器
public class PanelManager
{
    private static readonly Lazy<PanelManager> lazy = new Lazy<PanelManager>(() => new PanelManager());
    public static PanelManager instance { get { return lazy.Value; } }

    //存储UI面板
    private Stack<BasePanel> PanelStack;

    //UI管理器
    public UIManager _UIManager;
    private BasePanel panel;

    public PanelManager()
    {
        PanelStack = new Stack<BasePanel>();
        _UIManager = new UIManager();
    }

    //UI的入栈操作,此操作会显示一个UI
    public void Push(BasePanel nextPanel)
    {
        if (PanelStack.Count > 0)
        {
            panel = PanelStack.Peek();//获取栈顶的面板，
            panel.OnPause();//然后暂停他
        }
        PanelStack.Push(nextPanel);//加入新面板
        GameObject panelGo = _UIManager.GetSingleUI(nextPanel._UIType);//实例化他
        nextPanel.Initialize(new UITool(panelGo));
        nextPanel.Initialize(instance);
        nextPanel.Initialize(_UIManager);
        nextPanel.OnEnter();
    }

    //弹出栈顶面板
    public void Pop()
    {
        if (PanelStack.Count > 0)
            PanelStack.Pop().OnExit();
        if (PanelStack.Count > 0)
            PanelStack.Peek().OnResume();
    }
    //执行所有面板的OnExit()
    public void PopAll()
    {
        while (PanelStack.Count > 0)
            PanelStack.Pop().OnExit();
    }

}
