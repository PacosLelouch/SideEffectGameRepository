using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

// 由技能生成的物体挂载的带有技能信息的组件
public abstract class SkillEntityComponentBase : MonoBehaviour
{
    private bool _isSkillEnds = true;
    private bool _isSkillAnimationEnds = true;
    private SkillRuntimeData _skillData = null; 
    public SkillRuntimeData skillData //技能管理器提供
    {
        get 
        { 
            return _skillData; 
        }
        set 
        { 
            _skillData = value; 
            InitEntity(); 
        }
    }

    /// <summary>
    /// 范围选择算法
    /// </summary>
    private ISkillSelector _selector;
    /// <summary>
    /// 一系列技能效果
    /// </summary>
    private IImpactEffect[] _impactArray;
    /// <summary>
    /// 初始化技能实体
    /// </summary>
    private void InitEntity()
    {
        //范围选择
        _selector = SkillBehaviorConfigFactory.CreateSkillSelector(_skillData);
        //效果
        _impactArray = SkillBehaviorConfigFactory.CreateImpactEffects(_skillData);
    }
    /// <summary>
    /// 范围选择
    /// </summary>
    public void PutTargetsIntoSkillData()
    {
        if (_selector != null)
        {
            skillData.generatedData.skillSelectResults = _selector.SelectTarget(skillData, gameObject);
        }
    }
    /// <summary>
    /// 碰撞目标
    /// </summary>
    public void CollideTargets(Collision[] collisions)
    {
        if (_selector != null)
        {
            IContactSkillContext contackSkillContext = _selector as IContactSkillContext;
            contackSkillContext.collisions = collisions;
            skillData.generatedData.skillSelectResults = _selector.SelectTarget(skillData, gameObject);
        }
    }
    /// <summary>
    /// 接触目标（非碰撞）
    /// </summary>
    public void ContactTargets(Collider[] colliders)
    {
        if (_selector != null)
        {
            IContactSkillContext contackSkillContext = _selector as IContactSkillContext;
            contackSkillContext.colliders = colliders;
            skillData.generatedData.skillSelectResults = _selector.SelectTarget(skillData, gameObject);
        }
    }
    /// <summary>
    /// 效果生效
    /// </summary>
    public void ImpactTargets()
    {
        foreach(SkillSelectResult skillSelectResult in skillData.generatedData.skillSelectResults)
        {
            CreateHitVFX(skillSelectResult);
            PlayHitAFX(skillSelectResult);
            OnSkillHitsTarget(skillSelectResult);
            skillData.OnSkillHitsTarget?.Invoke(skillSelectResult);
        }
        for (int i = 0; i < _impactArray.Length; i++)
        {
            _impactArray[i]?.Execute(this);
        }
        // 非群攻技能
        if (skillData.generatedData.skillSelectResults.Length > 0 && 
            skillData.basicConfig.attackType != SkillAttackType.AOE &&
            skillData.basicConfig.disappearType == DisappearType.CheckOver)
        {
            Invoke(nameof(EndSkill), 0f);
        }
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    public void ReleaseSkill()
    {
        _isSkillEnds = false;
        float animationStartTime = skillData.basicConfig.animationStartTime;
        Invoke(nameof(PlayReleaseAnimation), animationStartTime);
        PlayReleaseAFX();
        OnSkillReleased();
        float warmUpTime = skillData.basicConfig.warmUpTime;
        Invoke(nameof(OnWarmUpEnds), warmUpTime);
        // 如果是按时间算，需要在持续时间后结束技能
        // 否则需要手动结束，或者具体技能有提前结束的时机
        if (skillData.basicConfig.disappearType == DisappearType.TimeOver)
        {
            float skillDuration = skillData.basicConfig.durationTime;
            Invoke(nameof(EndSkill), warmUpTime + skillDuration);
        }
        if (skillData.basicConfig.animationNames?.Length > 0)
        {
            float animationDurationTime = skillData.basicConfig.animationDurationTime;
            float animationEndTime = Mathf.Min(animationStartTime + animationDurationTime, skillData.basicConfig.skillCd);
            Invoke(nameof(EndSkillAnimation), animationEndTime);
        }
    }

    /// <summary>
    /// 结束技能
    /// </summary>
    public void EndSkill()
    {
        // 防止多次结束技能
        if (_isSkillEnds)
        {
            return;
        }
        _isSkillEnds = true;
        if (skillData != null)
        {
            skillData.OnSkillHitsTarget?.RemoveAllListeners();
            skillData.OnSkillEnds?.Invoke();
            skillData.OnSkillEnds?.RemoveAllListeners();
        }
        OnSkillEnds();
        //OnSkillEnds?.Invoke();
    }

    /// <summary>
    /// 结束技能动画
    /// </summary>
    public void EndSkillAnimation()
    {
        // 防止多次结束技能
        if (_isSkillAnimationEnds)
        {
            return;
        }
        _isSkillAnimationEnds = true;
        if (skillData != null)
        {
            skillData.OnSkillAnimationEnds?.Invoke();
            skillData.OnSkillAnimationEnds?.RemoveAllListeners();
        }
    }

    /// <summary>
    /// 技能释放时的回调，由子类实现，定义具体释放策略
    /// </summary>
    protected abstract void OnSkillReleased();

    /// <summary>
    /// 前摇结束时的回调，由子类实现，定义具体前摇策略
    /// </summary>
    protected abstract void OnWarmUpEnds();

    /// <summary>
    /// 技能命中敌人时的回调，由子类实现，定义具体处理策略
    /// </summary>
    protected abstract void OnSkillHitsTarget(SkillSelectResult skillSelectResult);

    /// <summary>
    /// 技能结束时的回调，由子类实现，定义具体回收策略（如果需要破坏自身，需要在这里面实现）
    /// </summary>
    //public UnityEvent OnSkillEnds = new UnityEvent();
    protected abstract void OnSkillEnds();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void PlayReleaseAnimation()
    {
        if (skillData != null)
        {
            string[] animationNames = skillData.basicConfig.animationNames;
            if (skillData.generateSettings.animationObjects?.Length > 0 && animationNames.Length > 0)
            {
                foreach (GameObject animationObject in skillData.generateSettings.animationObjects)
                {
                    if (animationObject != null)
                    {
                        Animator[] animators = animationObject.GetComponents<Animator>();
                        foreach (Animator animator in animators)
                        {
                            int layerId = 0;
                            AnimatorStateInfo animationStateInfo = animator.GetCurrentAnimatorStateInfo(layerId);
                            int indexBeforeTarget = -1;
                            for (int index = 0; index < animationNames.Length; ++index)
                            {
                                if (animationStateInfo.IsName(animationNames[index]))
                                {
                                    indexBeforeTarget = index;
                                    break;
                                }
                            }

                            int targetIndex = (indexBeforeTarget + 1) % animationNames.Length;
                            string animationName = animationNames[targetIndex];
                            // if (!animationStateInfo.IsName(animationName))
                            {
                                int stateHashName = Animator.StringToHash(animationName);
                                if (animator.HasState(layerId, stateHashName))
                                {
                                    animator.CrossFadeInFixedTime(animationName, 0.2f, layerId);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    protected void PlayReleaseAFX()
    {
        if (skillData != null)
        {
            AudioClip audioClip = skillData.postLoadSettings.releaseAFXClip;
            if (audioClip != null)
            {
                AudioSource audioSource = null;
                if (skillData.generateSettings.releaseSkillAudioSourceObject != null)
                {
                    audioSource = skillData.generateSettings.releaseSkillAudioSourceObject.GetComponent<AudioSource>();
                }
                if (audioSource != null)
                {
                    audioSource.volume = skillData.basicConfig.releaseAFXVolume;
                    audioSource.PlayOneShot(audioClip);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(audioClip, transform.position, skillData.basicConfig.releaseAFXVolume);
                }
            }
        }
    }
    protected void PlayHitAFX(SkillSelectResult skillSelectResult)
    {
        if (skillData != null)
        {
            AudioClip audioClip = skillData.postLoadSettings.hitAFXClip;
            if (audioClip != null)
            {
                // TODO: dopplerLevel要调，output要指定AudioMixer（用预制体？）
                AudioSource.PlayClipAtPoint(audioClip, skillSelectResult.position, skillData.basicConfig.hitAFXVolume);
            }
        }
    }
    protected void CreateHitVFX(SkillSelectResult skillSelectResult)
    {
        if (skillData != null)
        {
            GameObject hitVFXPrefab = skillData.postLoadSettings.hitVFXPrefab;
            if (hitVFXPrefab != null)
            {
                GameObject GO = Instantiate(hitVFXPrefab, skillSelectResult.position, Quaternion.identity);
                if (skillData.basicConfig.hitVFXDuration >= 0f)
                {
                    Destroy(GO, skillData.basicConfig.hitVFXDuration);
                }
            }
        }
    }
}
