using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;

public class SettingPanel : BasePanel
{
    static readonly string path = "UI/Prefabs/SettingPanel";
    public SettingPanel() : base(new UIType(path)) { }


    Slider MusicSlider;//音乐音量调节
    Slider EffectSlider;//音效音量调节
    public override void OnEnter()
    {
        //按下设置界面的保存按钮
        _UITool.GetOrAddComponentInChildren<Button>("Save_Button").onClick.AddListener(()=>
        {
            Pop();
        });
        //初始化音乐音效Slider
        InitMusicEffectSlider();
        //初始化礼物特效显示按钮
        InitGiftEffectsToggle();
        //初始化礼物图
        InitGiftImage();
    }


    public override void OnExit()
    {
        //关闭主界面设置按钮
        _UIManager.DestroyUI(_UIType);
        CloseMainPanelSettingToggle();
    }
    //初始化音乐音效Slider
    void InitMusicEffectSlider()
    {
        //音乐
        var audioMager = AudioManager.Instance;
        if (audioMager == null) return;
        MusicSlider = _UITool.GetOrAddComponentInChildren<Slider>("Scrollbar_Volume");
        MusicSlider.maxValue = 1;
        MusicSlider.minValue = 0;
        MusicSlider.value = audioMager.MusicSource.volume;
        MusicSlider.onValueChanged.AddListener(MusicHandleSliderValueChanged);
        ////音效
        //EffectSlider = _UITool.GetOrAddComponentInChildren<Slider>("Scrollbar_SoundEffects");
        //EffectSlider.maxValue = 1;
        //EffectSlider.minValue = 0;
        //EffectSlider.value = audioMager.EffectVolume;
        //EffectSlider.onValueChanged.AddListener(EffectHandleSliderValueChanged);
    }
    // 音乐滑块值变化时调用此方法
    void MusicHandleSliderValueChanged(float value)
    {
        // 设置音频源的音量为滑块的值
        var audioMager = AudioManager.Instance;
        audioMager.MusicSource.volume = value;
    }
    // 音效滑块值变化时调用此方法
    void EffectHandleSliderValueChanged(float value)
    {
        var audioMager = AudioManager.Instance;
        audioMager.EffectVolume = value;
        audioMager.UpdateAllEffectVolume(value);
    }
    //关闭主界面设置按钮
    void CloseMainPanelSettingToggle()
    {
        //获得mian面板
        var panelMager = PanelManager.instance;
        var uiMager = panelMager._UIManager;
        var mainPanel = uiMager.GetSingleUI("MainPanel");
        if (mainPanel == null) return;
        //获得setting Toggle
        var setting = _UITool.FindChildGameObject(mainPanel, "Setting");
        if (setting == null) return;
        Toggle settingToggle = setting.GetComponent<Toggle>();
        settingToggle.onValueChanged.RemoveAllListeners(); // 移除所有监听器
        settingToggle.isOn = false;
        // 需要时重新添加特定的监听器
        settingToggle.onValueChanged.AddListener(delegate { Push(new SettingPanel()); });
    }
    //同步礼物特效是否显示按钮
    void SyncGiftEffectsToggle()
    {
        var gameRoot = GameRoot.Instance;
        if(gameRoot.Is_Gift1EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle仙女棒开启").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle仙女棒关闭").isOn = true;
        if (gameRoot.Is_Gift2EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle甜甜圈开启").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle甜甜圈关闭").isOn = true;
        if (gameRoot.Is_Gift3EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle能量电池开启").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle能量电池关闭").isOn = true;
        if (gameRoot.Is_Gift4EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle恶魔炸弹开启").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle恶魔炸弹关闭").isOn = true;
        if (gameRoot.Is_Gift5EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle神秘空投开启").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle神秘空投关闭").isOn = true;
        if (gameRoot.Is_Gift6EffectsCanPlayed)
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle超能喷射开启").isOn = true;
        else
            _UITool.GetOrAddComponentInChildren<Toggle>("Toggle超能喷射关闭").isOn = true;

    }
    //初始化礼物特效显示按钮
    void InitGiftEffectsToggle()
    {
        //先同步按钮
        SyncGiftEffectsToggle();
        var gameRoot = GameRoot.Instance;
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle仙女棒关闭").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift1EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle仙女棒开启").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift1EffectsCanPlayed = true;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle甜甜圈关闭").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift2EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle甜甜圈开启").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift2EffectsCanPlayed = true;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle能量电池关闭").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift3EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle能量电池开启").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift3EffectsCanPlayed = true;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle恶魔炸弹关闭").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift4EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle恶魔炸弹开启").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift4EffectsCanPlayed = true;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle神秘空投关闭").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift5EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle神秘空投开启").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift5EffectsCanPlayed = true;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle超能喷射关闭").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift6EffectsCanPlayed = false;
        });
        _UITool.GetOrAddComponentInChildren<Toggle>("Toggle超能喷射开启").onValueChanged.AddListener(delegate
        {
            gameRoot.Is_Gift6EffectsCanPlayed = true;
        });
    }
    //初始化礼物图
    void InitGiftImage()
    {
        var gameRoot = GameRoot.Instance;
        //先同步礼物图按钮
        switch(gameRoot.giftImage)
        {
            case GiftImage.NUll: _UITool.GetOrAddComponentInChildren<Toggle>("不显示Toggle").isOn = true;break;
            case GiftImage.Heel: _UITool.GetOrAddComponentInChildren<Toggle>("显示横板Toggle").isOn = true;break;
            case GiftImage.VErtical: _UITool.GetOrAddComponentInChildren<Toggle>("显示竖板Toggle").isOn = true;break;
        }

        //按下显示礼物横板按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("显示横板Toggle").onValueChanged.AddListener(delegate
        {
            gameRoot.giftImage = GiftImage.Heel;
        });
        //按下显示礼物竖板按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("显示竖板Toggle").onValueChanged.AddListener(delegate
        {
            gameRoot.giftImage = GiftImage.VErtical;
        });
        //按下不显示礼物图按钮
        _UITool.GetOrAddComponentInChildren<Toggle>("不显示Toggle").onValueChanged.AddListener(delegate
        {
            gameRoot.giftImage = GiftImage.NUll;
        });
    }
}
