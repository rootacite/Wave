using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Keys : MonoBehaviour
{
    // Static Attributes
    public static List<Keys> Instances = new List<Keys>(); //??????????????????????????????????Z?????????????????????
    
    // Special Attributes
    protected bool ForciblyPerfect = false;
    protected float BeatPerSecond; //?????????????
    protected float Offset = 1f; //???????????
    protected int Status = 2; //?????????????????XXXEvent??????????????
    private int _endStatus = 0; //?????OnInvalid????????????????????
    public bool isInWave;
    private bool _invalided = false;
    public bool Invalided
    {
        get => _invalided;
        protected set
        {
            if (value && (value != _invalided))
            {
                switch (_endStatus)
                {
                    case 0:
                        GameScripting.Instance.CurrentMark += 500;
                        GameScripting.Instance.ComboCount += 1;
                        break;
                    case 1:
                        GameScripting.Instance.CurrentMark += 200;
                        GameScripting.Instance.ComboCount += 1;
                        break;
                    case 2:
                        GameScripting.Instance.CurrentMark += 50;
                        GameScripting.Instance.ComboCount = 0;
                        break;
                    case 3:
                        GameScripting.Instance.CurrentMark += 0;
                        GameScripting.Instance.ComboCount = 0;
                        break;
                }
                
                if (_endStatus == 1)
                {
                    GameScripting.Instance.AP = false;
                }

                if (_endStatus == 2 || _endStatus == 3)
                {
                    GameScripting.Instance._AP = false;
                    GameScripting.Instance.AC = false;
                }

                OnInvalided?.Invoke(_endStatus);

                if (!(this is WaveController)) //??????Wave?????????????????????????????????????
                {
                    _bAnimation.speed = 1 / (BeatPerSecond * HeadPending);
                }

                if (this is DragController)
                {
                    if (((DragController)this).isNode) _bAnimation.speed = 3f / BeatPerSecond;
                }

                Destroy(_collider2D);

                //if(IsInWave || (this is SlideController))
                //{
                //    var ppr = Instantiate(PariEffect, transform.localPosition, transform.localRotation, transform.parent);
                //    ParticleSystem.PlaybackState ps;
                //   ppr.GetComponent<ParticleSystem>().playbackSpeed = BAnimation.speed;
                //}
            }

            _invalided = value;
        }
    }
    
    // Global Attributes
    public static bool AutoMode //?????????????????RootConfig??Start????????
    {
        get => PlayerPrefs.GetInt("AutoMode_Setting", 0) == 1;
    }
    protected float HeadPending
    {
        get => LevelBasicInformation.HeadPending;
    }
    
    // Unity Field

    [FormerlySerializedAs("PariEffect")] public GameObject pariEffect;
    private Animator _bAnimation; //???????????????????
    public float Z { get; protected set; } //???Z??????
    private Collider2D _collider2D;
    
    // Events
    public event Action Prefect; //0
    public event Action Great; //1
    public event Action Bad; //2
    public event Action Miss; //3
    public event OnInvalidedHandler OnInvalided; //????????????????????????????Miss????????
    public delegate void OnInvalidedHandler(int status);

    protected virtual void Start()
    {
        OnInvalided += (v) =>
        {
            var Fr = gameObject.transform.position;
            //Fr /= 1.25f;
            Fr.x /= 12.0f;
            Fr.y /= 5.40f;
            Fr.y /= 1.25f;
            Fr.x += 1f;
            Fr.y += 1f;

            Fr.x /= 2f;
            Fr.y /= 2f;

            Wave.Instance.SetPoint(Fr);
        };

        GameObject.Find("TouchManager").GetComponent<TouchManager>().OnTouch.Add(TouchEvent);

        if (Offset == 0)
        {
            _bAnimation.speed = float.MaxValue;
            ForciblyPerfect = true;
            return;
        }

        _bAnimation.speed = 1f / (Offset * BeatPerSecond); //??????????????????????????????

    }
    protected void OnPrefect()
    {
        Prefect?.Invoke();
        _endStatus = 0;
    }
    protected void OnGreat()
    {
        Great?.Invoke();
        _endStatus = 1;
    }
    protected void OnBad()
    {
        Bad?.Invoke();
        _endStatus = 2;
    }
    protected void OnMiss()
    {
        Miss?.Invoke();
        _endStatus = 3;
    }
    protected virtual IEnumerator DelayDestroy(double time)
    {
        yield return new WaitForSeconds((float)time);
        Destroy(this.gameObject);
    }
    protected abstract bool TouchEvent(TouchPhase t, Vector2 p); //?????????????????????????????????????
    protected void Awake()
    {
        Instances.Add(this);
        _collider2D = GetComponent<Collider2D>();
        _bAnimation = GetComponent<Animator>();
    }
    void OnDestroy()
    {
        Instances?.Remove(this);
        GameObject.Find("TouchManager")?.GetComponent<TouchManager>().OnTouch.Remove(TouchEvent);
    }

    protected virtual void Update()
    {

    }

    public virtual void PrefectEvent()
    {
        Status = 0;
    }
    public virtual void GreatEvent()
    {
        Status = 1;
    }
    public virtual void BadEvent()
    {
        Status = 2;
    }
    public virtual void MissEvent()
    {
        if (Invalided) return;

        _bAnimation.SetTrigger("Miss");
        OnMiss();

        Invalided = true;
        StartCoroutine(DelayDestroy(0.33f / _bAnimation.speed));
    }
    
    public virtual void SetWaveEffect()
    {
        isInWave = true;
    }
    
    
    public virtual double TotalLength => 1.0d;
}
