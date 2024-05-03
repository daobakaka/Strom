using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Entities;
using Unity.Mathematics;

readonly public partial struct ShiBingAspects : IAspect
{
    public readonly Entity ShiBing_Entity;//这个定义的Entire是用来删除选中的Entiy
    readonly RefRW<ShiBing> m_ShiBing;
    readonly RefRW<ShiBingChange> m_ShiBingChange;

    public float3 ShiBingInitDir { get { return m_ShiBingChange.ValueRW.Dir; } set { m_ShiBingChange.ValueRW.Dir = value; } }
    public ActState ACT { get { return m_ShiBingChange.ValueRW.Act; } set { m_ShiBingChange.ValueRW.Act = value; } }
    public Entity TarEntity { get { return m_ShiBing.ValueRW.TarEntity; } set { m_ShiBing.ValueRW.TarEntity = value; } }
    public Entity ShootEntity { get { return m_ShiBing.ValueRW.ShootEntity; } set { m_ShiBing.ValueRW.ShootEntity = value; } }
    public float Injuries { get { return m_ShiBing.ValueRW.Injuries; } set { m_ShiBing.ValueRW.Injuries = value; } }
    public Entity enemyJiDi { get { return m_ShiBingChange.ValueRW.enemyJiDi; } set { m_ShiBingChange.ValueRW.enemyJiDi = value; } }
    public bool fire_TakeTurnsBool { get { return m_ShiBing.ValueRW.Fire_TakeTurnsBool; } set { m_ShiBing.ValueRW.Fire_TakeTurnsBool = value; } }
    public int fire_TakeTurnsInt { get { return m_ShiBing.ValueRW.Fire_TakeTurnsInt; } set { m_ShiBing.ValueRW.Fire_TakeTurnsInt = value; } }
    public Entity FirePoint_R { get { return m_ShiBing.ValueRW.FirePoint_R; } }
    public Entity FirePoint_L { get { return m_ShiBing.ValueRW.FirePoint_L; } }
    public Entity FirePoint_R2 { get { return m_ShiBing.ValueRW.FirePoint_R2; } }
    public Entity FirePoint_L2 { get { return m_ShiBing.ValueRW.FirePoint_L2; } }
    public bool Is_Parasitic { get { return m_ShiBing.ValueRW.Is_Parasitic; } }
    public ShiBingName Name { get { return m_ShiBing.ValueRW.Name; } }
    public Entity JidiPoint { get { return m_ShiBing.ValueRW.JidiPoint; } set { m_ShiBing.ValueRW.JidiPoint = value; } }
    //public bool Is_Parasitic { get { return m_ShiBing.ValueRW.} }
    //public layer BelongsTo { get { return m_Layer.ValueRW.BelongsTo; } set { m_Layer.ValueRW.BelongsTo = value; } }
    //public layer CollidesWith { get { return m_Layer.ValueRW.CollidesWith; } set { m_Layer.ValueRW.CollidesWith = value; } }
    //public layer BulletCollidesWith { get { return m_Layer.ValueRW.BulletCollidesWith; } set { m_Layer.ValueRW.BulletCollidesWith = value; } }
    //public NativeList<float> ATList { get { return m_ShiBing.ValueRW.ATList; } }


    //播放动画=====================================================================
    //private readonly RefRW<GpuEcsAnimatorControlComponent> control;
    public void RunAnimation(ref GpuEcsAnimatorControlComponent control,int animationID,
        float speedFactor = 1f, float blendFactor = 0f,  float startNormalizedTime = 0f, float transitionSpeed = 0f)
    {
        control = new GpuEcsAnimatorControlComponent()
        {
            animatorInfo = new AnimatorInfo()
            {
                animationID = animationID,
                blendFactor = blendFactor,
                speedFactor = speedFactor
            },
            startNormalizedTime = startNormalizedTime,
            transitionSpeed = transitionSpeed
        };
    }


    public void PlayAnim(Entity entity)
    {
        
    }

    public int GetAnimIndex(ActState Act)//选择播放动画的ID
    {
        if(m_ShiBing.ValueRW.Name == ShiBingName.ChangGong)
        {
            switch(Act)
            {
                case ActState.Idle:  return (int)AnimationIdsChangGong.Idle;
                case ActState.Walk:  return (int)AnimationIdsChangGong.Walk;
                case ActState.Ready: return (int)AnimationIdsChangGong.Ready_1;
                case ActState.Fire:  return (int)AnimationIdsChangGong.Attack;
            }
        }
        else if (m_ShiBing.ValueRW.Name == ShiBingName.JianYa)
        {
            switch (Act)
            {
                case ActState.Idle: return (int)AnimationIdsJianYa.Idle;
                case ActState.Walk: return (int)AnimationIdsJianYa.Walk;
                case ActState.Ready: return (int)AnimationIdsJianYa.miaozhun;
                case ActState.Fire: return (int)AnimationIdsJianYa.Attack;
            }
        }
        else if (m_ShiBing.ValueRW.Name == ShiBingName.HuGuang)
        {
            if(Act == ActState.Fire)
            {
                if (m_ShiBing.ValueRW.AttackNumber >= 3 && m_ShiBing.ValueRW.Is_UpSkill)//三次普攻一次蓄力攻击
                {
                    m_ShiBing.ValueRW.AttackNumber = 0;
                    return (int)AnimationIdsHuGuang.Attack2;
                }
                else
                {
                    m_ShiBing.ValueRW.AttackNumber += 1;
                    return (int)AnimationIdsHuGuang.Attack;
                }

            }
            switch (Act)
            {
                case ActState.Idle: return (int)AnimationIdsHuGuang.Idle;
                case ActState.Walk: return (int)AnimationIdsHuGuang.Walk;
                case ActState.Ready: return (int)AnimationIdsHuGuang.Ready;
            }
        }
        else if (m_ShiBing.ValueRW.Name == ShiBingName.PaChong)
        {
            switch (Act)
            {
                case ActState.Idle: return (int)AnimationIdsPaChong.Idle;
                case ActState.Walk: return (int)AnimationIdsPaChong.Walk;
                case ActState.Ready: return (int)AnimationIdsPaChong.Ready;
                case ActState.Fire: return (int)AnimationIdsPaChong.Attack;
            }
        }
        else if (m_ShiBing.ValueRW.Name == ShiBingName.BingFeng)
        {
            switch (Act)
            {
                case ActState.Ready: return (int)AnimationIdsBingFeng.Idle;
                case ActState.Fire: return (int)AnimationIdsBingFeng.Fire;
            }
        }
        else if (m_ShiBing.ValueRW.Name == ShiBingName.ZhanZhengGongChang)
        {
            switch (Act)
            {
                case ActState.Walk: return (int)AnimationIdsZhanZhengGongChang.work_Show;
                case ActState.Move: return (int)AnimationIdsZhanZhengGongChang.work_Show;
                case ActState.Walk_R: return (int)AnimationIdsZhanZhengGongChang.Walk_R;
                case ActState.Walk_L: return (int)AnimationIdsZhanZhengGongChang.Walk_L;
            }
        }
        else if (m_ShiBing.ValueRW.Name == ShiBingName.HaiKe)
        {
            switch (Act)
            {
                case ActState.Idle: return (int)AnimationIdsHaiKe.Idle;
                case ActState.Walk: return (int)AnimationIdsHaiKe.Walk;
                case ActState.Move: return (int)AnimationIdsHaiKe.Walk;
                case ActState.Ready: return (int)AnimationIdsHaiKe.Idle;
                case ActState.Fire: return (int)AnimationIdsHaiKe.Idle;
            }
        }
        else if (m_ShiBing.ValueRW.Name == ShiBingName.YeMa)
        {
            return (int)AnimationIdsYeMa.idle;
        }
        else if (m_ShiBing.ValueRW.Name == ShiBingName.BaoLei)
        {
            if (Act == ActState.Idle)
                return (int)AnimationIdsBaolei.Idle;
            else if (Act == ActState.Walk)
                return (int)AnimationIdsBaolei.Walk;
            else if (Act == ActState.Ready)
                return (int)AnimationIdsBaolei.Ready;
            else if (Act == ActState.Fire)//轮流播放左右攻击行为
            {
                if(fire_TakeTurnsBool)
                {
                    fire_TakeTurnsBool = !fire_TakeTurnsBool;
                    return (int)AnimationIdsBaolei.Fire_R;
                }
                else if(fire_TakeTurnsBool == false)
                {
                    fire_TakeTurnsBool = !fire_TakeTurnsBool;
                    return (int)AnimationIdsBaolei.Fire_L;
                }
            }
            else
            {
                return 0;
            }
        }
        return 0;
    }



}
