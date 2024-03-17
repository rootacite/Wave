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
        if (Invalided) return false;

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
                Invalided = true;
                StartCoroutine(DelayDestroy(1f / TAnimation.speed));
            }
            else if (Status == 1)
            {
                OnGreat();
                TAnimation.SetTrigger("Great");
                Invalided = true;
                StartCoroutine(DelayDestroy(1f / TAnimation.speed));
            }
            else
            {
                OnBad();
                TAnimation.SetTrigger("Bad");
                Invalided = true;
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
    static public SlideController Creat(Vector3 Position, GameObject Origin, GameObject TransfronParent, float SecondPerBeat, float BeatOffset = 1f)
    {
        var r = Instantiate(Origin, TransfronParent.transform);
        r.transform.localPosition = Position;
        r.transform.position = new Vector3(r.transform.position.x, r.transform.position.y, Position.z);
        var Controller = r.GetComponent<SlideController>();

        Controller.BeatPerSecond = SecondPerBeat;
        Controller.Offset = BeatOffset;
        Controller.Z = r.transform.position.z;

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
        if (Invalided) return;
        TAnimation.speed = 1 / (BeatPerSecond * HeadPending); //����Ѿ������ж�������Ӧ�ð��ٶ�����Ϊ�����ٶ�

        if (AutoMode)
        {
            OnPrefect();
            TAnimation.SetTrigger("Perfect");
            Invalided = true;
            StartCoroutine(DelayDestroy(1f / TAnimation.speed));
        }
    }
}
