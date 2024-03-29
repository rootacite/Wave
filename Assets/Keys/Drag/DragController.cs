using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : Keys
{
    public float NodeToward;

    public CircleCollider2D Collider;

    public SpriteRenderer Border;
    public SpriteRenderer Center;

    public Sprite Border_sp;
    public Sprite Center_sp;

    public Sprite Border_nd;
    public Sprite Center_nd;

    public GameObject Exp_Area;

    public GameObject Effect;
    // Start is called before the first frame update
    static public DragController Creat(Vector3 Position, GameObject Origin, GameObject TransfronParent, float SecondPerBeat, float BeatOffset = 1f)
    {
        var r = Instantiate(Origin, TransfronParent.transform);
        r.transform.localPosition = Position;
        r.transform.position = new Vector3(r.transform.position.x, r.transform.position.y, Position.z);
        var Controller = r.GetComponent<DragController>();

        Controller.BeatPerSecond = SecondPerBeat;
        Controller.Offset = BeatOffset;
        Controller.Z = r.transform.position.z;
        //Controller.rootConfig = rootConfig;
        
        return Controller;
    }
    private Animator TAnimation;
    protected override void Start()
    {
        base.Start();

        TAnimation = GetComponent<Animator>();

        Prefect += () =>
        {
                Effect.SetActive(true);
        };
    }
    bool touched = false;
    protected override void Update()
    {
        base.Update();
    }
    override protected bool TouchEvent(TouchPhase t, Vector2 p)
    {
        if (t == TouchPhase.Moved || t == TouchPhase.Stationary)
        {
            var RayHit = Physics2D.Raycast(p, Vector2.zero);
            if (RayHit.collider == gameObject.GetComponent<Collider2D>())
            {
                if (Status != 0 && Status != 1) return false;

                touched = true;
                if(ended)
                {
                    if (Invalided) return false;
                    OnPrefect();
                    TAnimation.SetTrigger("Perfect");
                    Invalided = true;
                    StartCoroutine(DelayDestroy(1f / TAnimation.speed));
                }
                return false;
            }

            return false;
        }
        return false;
    }
    public void EndEvent()
    {
        ended = true;
        if (Invalided) return;
        TAnimation.speed = 1 / (BeatPerSecond * HeadPending); //����Ѿ������ж�������Ӧ�ð��ٶ�����Ϊ�����ٶȳ����ײ��ӳ�
        if (AutoMode || ForciblyPerfect)
        {
            OnPrefect();
            TAnimation.SetTrigger("Perfect");
            Invalided = true;
            StartCoroutine(DelayDestroy(1f / TAnimation.speed));
            return;
        }

        if (touched)
        {
            OnPrefect();
            TAnimation.SetTrigger("Perfect");
            Invalided = true;
            StartCoroutine(DelayDestroy(1f / TAnimation.speed));
        }
    }
    override public void SetWaveEffect()
    {
        base.SetWaveEffect();
        Exp_Area.SetActive(false);
        Border.sprite = Border_sp;
        Center.sprite = Center_sp;
    }

    bool ended = false;
    public bool isNode = false;

    public void SetNodeMode(float NodeToward, float AvaBeat)
    {
        transform.localScale /= 6;
        Collider.radius *= 6;

        Border.sprite = Border_nd;
        Border.gameObject.GetComponent<Renderer>().enabled = false;
        
        Center.sprite = Center_nd;

        isNode = true;

        this.NodeToward = NodeToward;
       // OnInvailded += (s) =>
       // {
       //    var ppr = Instantiate(PariEffect, transform.localPosition, transform.localRotation, transform.parent);
       //     ppr.transform.Rotate(new Vector3(this.NodeToward, 0, 0), Space.Self);
       //     ParticleSystem.PlaybackState ps;
       //     ppr.GetComponent<ParticleSystem>().playbackSpeed = 1 / (AvaBeat * rootConfig.SecondPerBeat * 8f);
       // };
    }
}
