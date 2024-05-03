using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//UI面板父类
public class BasePanel
{
    //UI信息
    public UIType _UIType { get; private set; }
    //UI管理工具
    public UITool _UITool { get; private set; }
    //面板管理器
    public PanelManager _PanelManager { get; private set; }
    //UI管理器
    public UIManager _UIManager { get; private set; }

    public BasePanel(UIType uiType)
    {
        _UIType = uiType;
    }

    //初始化UITool
    public void Initialize(UITool tool)
    {
        _UITool = tool;
    }
    //初始化面板管理器
    public void Initialize(PanelManager panelManager)
    {
        _PanelManager = panelManager;
    }
    //初始化UI管理器
    public void Initialize(UIManager uiManager)
    {
        _UIManager = uiManager;
    }

    //UI进入时执行的操作，只会执行一次
    public virtual void OnEnter() { }

    //UI暂停时执行的操作
    public virtual void OnPause()
    {
        //这个面板不可射线检测
        _UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = false;
    }

    //UI继续时执行的操作
    public virtual void OnResume()
    {
        //这个面板恢复射线检测
        _UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = true;
    }

    //UI退出时执行的操作
    public virtual void OnExit()
    {
        _UIManager.DestroyUI(_UIType);
    }

    //显示一个面板
    public void Push(BasePanel panel) => _PanelManager?.Push(panel);
    //关闭一个面板
    public void Pop() => _PanelManager?.Pop();
}
