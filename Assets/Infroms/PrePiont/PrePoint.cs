using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrePoint : Infroms
{
    public AnimationClip LoopingClip;
    public KeyType KeyType;

    public Sprite[] Textures;

    public SpriteRenderer Spr;

    public float Angle;

    static public PrePoint Create(Vector3 Position, GameObject Origin, GameObject TransfronParent, float Time, float AnimationScale, KeyType type = KeyType.Tap)
    {
        var r = Instantiate(Origin, TransfronParent.transform);
        r.transform.localPosition = Position;
        var Controller = r.GetComponent<PrePoint>();
        Controller.Time = Time;
        Controller.AnimationScale = AnimationScale;
        Controller.KeyType = type;

        return Controller;
    }

    IEnumerator DelayExit(float Time)
    {
        yield return new WaitForSeconds(Time);
        GetComponent<Animator>().SetTrigger("Ends");
        yield return new WaitForSeconds(0.33f / AnimationScale);
        Destroy(this.gameObject);
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        if (KeyType != KeyType.Drag)
        {
            var anmiley = gameObject.AddComponent<Animation>(); //????animation?????¯Ì???animator?õd??????????ØS??£R??????????¡W?????????

            anmiley.playAutomatically = false;
            anmiley.AddClip(LoopingClip, "1");

            foreach (AnimationState i in anmiley)
            {
                i.speed = AnimationScale;
            }
            anmiley.Play("1");
        }

        switch (KeyType)
        {
            case KeyType.Tap:
                break;
            case KeyType.Hold:
                Spr.sprite = Textures[1];
                break;
            case KeyType.Slide:
                Spr.sprite = Textures[2];
                break;
            case KeyType.Drag:
                Spr.sprite = Textures[3];
                transform.Rotate(new Vector3(0, 0, Angle), Space.Self);
                break;
            case KeyType.Wave:
                break;
            case KeyType.HWave:
                Spr.sprite = Textures[1];
                break;
            default:
                break;
        }

        GetComponent<Animator>().speed = AnimationScale;
        StartCoroutine(DelayExit(Time));
    }
    // Update is called once per frame
    protected override void Update()
    {
        
    }
    public float Time { get; private set; }
    public float AnimationScale { get; private set; }
}
