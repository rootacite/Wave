using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineArea : MonoBehaviour
{
    static public LineArea Create(GameObject Origin, GameObject Parent, Vector3 p1, Vector3 p2, float ExistTime)
    {
        GameObject obj = Instantiate(Origin, Parent.transform);
        var r = obj.GetComponent<LineArea>();
        var l = obj.GetComponent<LineRenderer>();

        r.Time = ExistTime;

        l.positionCount = 2;
        l.SetPositions(new Vector3[] { p1, p2 });

        return r;
    }

    static public LineArea Create(GameObject Origin, GameObject Parent, Vector3[] ps, float ExistTime)
    {
        GameObject obj = Instantiate(Origin, Parent.transform);
        var r = obj.GetComponent<LineArea>();
        var l = obj.GetComponent<LineRenderer>();

        r.Time = ExistTime;

        l.positionCount = ps.Length;
        l.SetPositions(ps);

        return r;
    }

    float Time;

    IEnumerator DelayExit()
    {
        yield return new  WaitForSeconds(Time);
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayExit());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
