using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 游戏中生成的技能数据
/// </summary>
[Serializable]
public struct SkillGeneratedData
{
    /// <summary>
    /// 技能剩余冷却时间
    /// </summary>
    public float cdRemain;

    /// <summary>
    /// 作用目标对象数组
    /// </summary>
    [HideInInspector]
    public SkillSelectResult[] skillSelectResults;

    /// <summary>
    /// 技能所属的角色
    /// </summary>
    [HideInInspector]
    public GameObject owner;
}

/// <summary>
/// 与场景角色设置相关的技能配置
/// </summary>
[Serializable]
public struct SkillGenerateSettings
{
    /// <summary>
    /// 所属技能ID
    /// </summary>
    public int ownedSkillId;

    /// <summary>
    /// 技能所挂载的场景物体（技能来源，如果该值为空，则技能来源为SkillGeneratorComponent所属GameObject）
    /// </summary>
    public GameObject skillSource; // weapon, foot, etc.

    /// <summary>
    /// 播放技能动画的对象
    /// </summary>
    public GameObject[] animationObjects; // character, etc.

    /// <summary>
    /// 播放技能释放音效的音源对象
    /// </summary>
    public GameObject releaseSkillAudioSourceObject; // character, etc.
}

/// <summary>
/// 技能命中目标时的回调事件的参数
/// </summary>
[Serializable]
public struct SkillSelectResult
{
    /// <summary>
    /// 命中位置
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// 命中目标
    /// </summary>
    public GameObject target;
}

/// <summary>
/// 运行时技能数据
/// </summary>
[Serializable]
public class SkillRuntimeData
{
    public SkillRuntimeData(SkillStaticSettings skillSettings)
    {
        staticSettings = skillSettings;
        InitGeneratedData();
    }

    public void InitGeneratedData()
    {
        generatedData.cdRemain = 0;
    }

    public SkillBasicConfig basicConfig
    {
        get { return staticSettings.basicConfig; }
    }

    public SkillPostLoadSettings postLoadSettings
    {
        get { return staticSettings.postLoadSettings; }
    }

    public SkillStaticSettings staticSettings = new SkillStaticSettings();

    public SkillGeneratedData generatedData = new SkillGeneratedData();

    public SkillGenerateSettings generateSettings = new SkillGenerateSettings();

    public UnityEvent<SkillSelectResult> OnSkillHitsTarget = new UnityEvent<SkillSelectResult>();
    public UnityEvent OnSkillEnds = new UnityEvent();
    public UnityEvent OnSkillAnimationEnds = new UnityEvent();
}

