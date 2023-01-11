using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideController : Keys
{
    public GameObject Center;
    public Sprite Center_sp;

    private float distance = 0.035f;
    private Animator TAnimation;

    bool isHolded = false;
    Vector2 OriginPos;
    override protected bool TouchEvent(TouchPhase t, Vector2 p)
    {
        var RayHit = Physics2D.Raycast(p, Vector2.zero);
        if (RayHit.collider != gameObject.GetComponent<Collider2D>()) return false;
        if (Invailded) return false;

        if (t == TouchPhase.Began)
        {
            isHolded = true;
            OriginPos = p;
            return true;
        }
        else if (t == TouchPhase.Moved)
        {
            if (!isHolded) return false;
            if ((p - OriginPos).magnitude < distance) return true;
            if (Status == 0)
            {
                OnPrefect();
                TAnimation.SetTrigger("Perfect");
                Invailded = true;
                StartCoroutine(DelayDestroy(1f / TAnimation.speed));
            }
            else if (Status == 1)
            {
                OnGreat();
                TAnimation.SetTrigger("Great");
                Invailded = true;
                StartCoroutine(DelayDestroy(1f / TAnimation.speed));
            }
            else
            {
                OnBad();
                TAnimation.SetTrigger("Bad");
                Invailded = true;
                StartCoroutine(DelayDestroy(1f / TAnimation.speed));
            }
            return true;
        }
        else if (t == TouchPhase.Ended)
        {
            isHolded = false;
            return false;
        }
        return false;
    }
    static public SlideController Creat(GameScripting rootConfig,Vector3 Position, GameObject Origin, GameObject TransfronParent, float SecondPerBeat, float BeatOffset = 1f)
    {
        var r = Instantiate(Origin, TransfronParent.transform);
        r.transform.localPosition = Position;
        var Controller = r.GetComponent<SlideController>();

        Controller.BeatPerSecond = SecondPerBeat;
        Controller.Offset = BeatOffset;
        Controller.Z = Position.z;
        Controller.rootConfig = rootConfig;

        return Controller;
    }

    override public void SetWaveEffect()
    {
        base.SetWaveEffect();
        Center.GetComponent<SpriteRenderer>().sprite = Center_sp;
    }

    protected override void Start()
    {
        base.Start();
        TAnimation = GetComponent<Animator>();
    }


    public void EndEvent()
    {
        if (Invailded) return;
        TAnimation.speed = 1 / (BeatPerSecond * rootConfig.HeadPending); //如果已经进入判定区域，则应该把速度重置为节拍速度

        if (AutoMode)
        {
            OnPrefect();
            TAnimation.SetTrigger("Perfect");
            Invailded = true;
            StartCoroutine(DelayDestroy(1f / TAnimation.speed));
        }
    }
}
