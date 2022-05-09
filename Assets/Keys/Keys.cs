using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Keys : MonoBehaviour
{
    public static bool AutoMode = false; //��ʶ�Ƿ����Զ�ģʽ����RootConfig��Start�����á�
    public GameObject PariEffect;

    public Animator BAnimation;//�����ȡ�ĸü�������������Ϊprivate�Ƿ�ֹ��ͻ

    public RootConfig rootConfig;

    protected float BeatPerSecond; //ÿ����ռ��ʱ�䣨�룩
    protected float Offset = 1f; //��ǰҡ�����ģ�
    protected int Status = 2; //ָʾ��ǰ���ж�״̬����XXXEvent���ɶ������ȴ���

    private int End_Status = 0; //���ݸ�OnInvaild�¼��Ĳ�����ָʾ�ж��Ľ��
    public static List<Keys> Instances = new List<Keys>(); //�洢��ǰ���ڵ����м�����Ҫ���ڼ����¼���Z�ᣨ��֤�����ɵ���Զ�ں��棩
    public float Z { get; protected set; } //�洢Z��λ��
    protected virtual void Start()
    {
        GameObject.Find("TouchManager").GetComponent<TouchManager>().OnTouch.Add(TouchEvent);

        if(Offset==0)
        {
            BAnimation.speed = float.MaxValue;
            return;
        }
        BAnimation.speed = 1f / (Offset * BeatPerSecond); //�κμ�λ����ʱ����Ӧ�ð��ٶ���ǰҡͬ��
        
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
    protected abstract bool TouchEvent(TouchPhase t, Vector2 p); //��ͬ�����͵Ĵ����¼�����Ҫ�������е���ʵ�֡�

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

                if (!(this is WaveController)) //��Ч��Wave���Ի�����һ�������ж��ߣ����Բ��������ٶ�
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

    public event OnInvaildedHandler OnInvailded; //�����ڼ�λ����Ч��ʱ����������Miss�����У�
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

    //���ĸ��¼������ڰ��������ﵽĳ���ж�ʱ��ʱ������
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

    //���������Wave�������Ӱ�����������ķ���Ӧ�ñ����á�
    //Ŀ����Ϊ����������е���Ч������������ز��滻Texture
    public virtual void SetWaveEffect()
    {
        IsInWave = true;
    }
    public bool IsInWave;
}
