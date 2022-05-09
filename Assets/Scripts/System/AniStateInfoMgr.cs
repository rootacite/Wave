using UnityEngine;

public class AniStateInfoMgr : MonoBehaviour
{
    Animator anim;
    AnimatorStateInfo animatorInfo;
    string stateName;
    float aniStateSpeed;
    int layer;
    float lastSpeed;
    bool isStart;

    public void SetAniSpeed(Animator anim, string stateName, int layer, float aniStateSpeed)
    {
        this.anim = anim;
        this.stateName = stateName;
        this.layer = layer;
        this.aniStateSpeed = aniStateSpeed;
        lastSpeed = anim.speed;
        isStart = true;
    }

    void Update()
    {
        if (isStart)
        {
            animatorInfo = anim.GetCurrentAnimatorStateInfo(layer);
            if (animatorInfo.IsName(stateName))//ע������ָ�Ĳ��Ƕ��������ֶ��Ƕ���״̬������
            {
                anim.speed = aniStateSpeed;
            }
            else
            {
                anim.speed = lastSpeed;
            }
        }
    }

    private void OnDestroy()
    {
        anim.speed = lastSpeed;
    }
}