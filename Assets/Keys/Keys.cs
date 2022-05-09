using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Keys : MonoBehaviour
{
    public static bool AutoMode = false; //标识是否处于自动模式，在RootConfig的Start中设置。
    public GameObject PariEffect;

    public Animator BAnimation;//基类获取的该键动画对象，设置为private是防止冲突

    public RootConfig rootConfig;

    protected float BeatPerSecond; //每拍所占的时间（秒）
    protected float Offset = 1f; //键前摇（节拍）
    protected int Status = 2; //指示当前的判定状态，在XXXEvent中由动画进度触发

    private int End_Status = 0; //传递给OnInvaild事件的参数，指示判定的结果
    public static List<Keys> Instances = new List<Keys>(); //存储当前存在的所有键，主要用于计算新键的Z轴（保证新生成的永远在后面）
    public float Z { get; protected set; } //存储Z轴位置
    protected virtual void Start()
    {
        GameObject.Find("TouchManager").GetComponent<TouchManager>().OnTouch.Add(TouchEvent);

        if(Offset==0)
        {
            BAnimation.speed = float.MaxValue;
            return;
        }
        BAnimation.speed = 1f / (Offset * BeatPerSecond); //任何键位生成时，都应该把速度与前摇同步
        
    }
    public event Action Prefect; //0
    public event Action Great; //1
    public event Action Bad; //2
    public event Action Miss; //3

    virtual protected void OnPrefect()
    {
        Prefect?.Invoke();
        End_Status = 0;
    }
    virtual protected void OnGreat()
    {
        Great?.Invoke();
        End_Status = 1;
    }
    virtual protected void OnBad()
    {
        Bad?.Invoke();
        End_Status = 2;
    }
    virtual protected void OnMiss()
    {
        Miss?.Invoke();
        End_Status = 3;
    }

    protected virtual IEnumerator DelayDestroy(float Time)
    {
        yield return new WaitForSeconds(Time);
        Destroy(this.gameObject);
    }
    protected abstract bool TouchEvent(TouchPhase t, Vector2 p); //不同键类型的触摸事件，需要在子类中单独实现。

    private bool _Invailded = false;
    public bool Invailded {
        get 
        { 
            return _Invailded;
        }
        protected set 
        {
            if (value && (value != _Invailded))
            {
                if(End_Status == 1)
                {
                    rootConfig.AP = false;
                }
                if (End_Status == 2 || End_Status == 3)
                {
                    rootConfig._AP = false;
                    rootConfig.AC = false;
                }
                OnInvailded?.Invoke(End_Status);

                if (!(this is WaveController)) //无效的Wave键仍会生成一个环形判定线，所以不能重置速度
                {
                    BAnimation.speed = 1 / (BeatPerSecond * rootConfig.HeadPending);
                }
                if(this is DragController)
                {
                    if (((DragController)this).isNode) BAnimation.speed = 3f / BeatPerSecond;
                }
                Destroy(c2d);

                //if(IsInWave || (this is SlideController))
                //{
                //    var ppr = Instantiate(PariEffect, transform.localPosition, transform.localRotation, transform.parent);
                //    ParticleSystem.PlaybackState ps;
                 //   ppr.GetComponent<ParticleSystem>().playbackSpeed = BAnimation.speed;
                //}
            }

            _Invailded = value;
        }
    }

    public event OnInvaildedHandler OnInvailded; //将会在键位被无效化时触发（包括Miss或命中）
    public delegate void OnInvaildedHandler(int Status);
    private Collider2D c2d;
    virtual protected void Awake()
    {
        Instances.Add(this);
        c2d = GetComponent<Collider2D>();
        BAnimation = GetComponent<Animator>();
    }
    virtual protected void OnDestroy()
    {
        Instances?.Remove(this);
        GameObject.Find("TouchManager")?.GetComponent<TouchManager>().OnTouch.Remove(TouchEvent);
    }
    virtual protected void Update()
    {

    }

    //这四个事件都会在按键动画达到某个判定时机时被触发
    virtual public void PrefectEvent()
    {
        Status = 0;
    }
    virtual public void GreatEvent()
    {
        Status = 1;
    }
    virtual public void BadEvent()
    {
        Status = 2;
    }
    virtual public void MissEvent()
    {
        if (Invailded) return;

        BAnimation.SetTrigger("Miss");
        OnMiss();

        Invailded = true;
        StartCoroutine(DelayDestroy(0.33f / BAnimation.speed));
    }

    //如果按键是Wave按键的子按键，则下面的方法应该被调用。
    //目的是为按键添加特有的特效，或用另外的素材替换Texture
    public virtual void SetWaveEffect()
    {
        IsInWave = true;
    }
    public bool IsInWave;
}
