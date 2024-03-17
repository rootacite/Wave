using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public partial class Creator : MonoBehaviour
{
    // Unity
    public Metronome Metronome;
    
    private readonly List<List<FlatKey>> _keyGroups = new();
    public void Flush(XElement dataRoot)
    {
        _keyGroups.Clear();
        foreach (XElement Group in dataRoot.Nodes())
        {
            List<FlatKey> local = new List<FlatKey>();

            foreach (XElement Key in Group.Nodes())
            {
                local.Add(new FlatKey(Key));
            }

            local.Sort(new Comparison<FlatKey>(
                (FlatKey x, FlatKey y) =>
                {
                    if (x.Time < y.Time) return -1;

                    else if (x.Time == y.Time) return 0;

                    else return 1;
                }));
            _keyGroups.Add(local);
        }

        if (Metronome.CurrentBeat == 1) return;

        while (true)
        {
            if (_keyGroups.Count == 0) break;
            if (_keyGroups[0].Count == 0) { _keyGroups.RemoveAt(0); continue; }
            if (_keyGroups[0][0].Time < Metronome.CurrentBeat + 1)
                _keyGroups[0].RemoveAt(0);
            else
            {
                break;
            }
        }

    }
    void Start()
    {
         if (!LevelBasicInformation.UseWorldCoordinate)
             Metronome.OnBeat += (t) =>
            {
                while (true)
                {
                    if (_keyGroups.Count == 0) return;
                    if (_keyGroups[0].Count == 0) { new_section_flag = true; _keyGroups.RemoveAt(0); continue; }
                    break;
                }


                bool Reser = false;
                Vector2? Last_Pos_Save = null;

                while (t + LevelBasicInformation.HeadPending >= _keyGroups[0][0].Time)
                {
                    bool ns = false;
                    if (new_section_flag) { ns = true; new_section_flag = false; }

                    //????????????????????ж???????????
                    float cy = (float)Metronome.GetJudgeY(_keyGroups[0][0].Time);
                    if (_keyGroups[0][0].ForceY != null)
                    {
                        cy = (int)_keyGroups[0][0].ForceY.Value;
                    }

                    var wp = Camera.main.ScreenToWorldPoint(new Vector3(LevelBasicInformation.Accuracy + LevelBasicInformation.Accuracy * (float)_keyGroups[0][0].Pos, cy + 540, 0));

                    if (_keyGroups[0][0].ForceY == null)
                    {
                        wp.y = cy;
                    }

                    Keys ct = null;
                    if (_keyGroups[0][0].Type == KeyType.Tap)
                    {
                        ct = CreateTap(wp, LevelBasicInformation.HeadPending);

                        wp.z = -9f;
                        var dc = CreateVecLine(wp, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
                        //dc.Key = ct.BAnimation;
                    }
                    else

                    if (_keyGroups[0][0].Type == KeyType.Hold)
                    {
                        ct = CreateHold(wp, (float)_keyGroups[0][0].Length, LevelBasicInformation.HeadPending);

                        wp.z = -9f;
                        var dc = CreateVecLine(wp, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
                        //dc.Key = ct.BAnimation;
                    }
                    else

                    if (_keyGroups[0][0].Type == KeyType.Slide)
                    {
                        ct = CreateSlide(wp, LevelBasicInformation.HeadPending);

                        wp.z = -9f;
                        var dc = CreateVecLine(wp, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
                        //dc.Key = ct.BAnimation;
                    }
                    else

                    if (_keyGroups[0][0].Type == KeyType.Wave)
                    {
                        ct = CreateWave(wp, _keyGroups[0][0].Children, (float)_keyGroups[0][0].Length, _keyGroups[0][0].TimeOfLastChild, LevelBasicInformation.HeadPending, (float)_keyGroups[0][0].WaveScale);
                        wp.z = -9f;

                        var dc = CreateVecLine(wp, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
                        //dc.Key = ct.BAnimation;

                    }
                    else
                    if (_keyGroups[0][0].Type == KeyType.HWave)
                    {
                        ct = CreateHWave(wp, _keyGroups[0][0].Children, (float)_keyGroups[0][0].Length, _keyGroups[0][0].TimeOfLastChild, LevelBasicInformation.HeadPending, (float)_keyGroups[0][0].WaveScale);
                        wp.z = -9f;

                        var dc = CreateVecLine(wp, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
                        //dc.Key = ct.BAnimation;
                    }
                    else
                    if (_keyGroups[0][0].Type == KeyType.Drag)
                    {
                        StartCoroutine(CreateDrag(_keyGroups[0][0].DragData.From, _keyGroups[0][0].DragData.To, _keyGroups[0][0].DragData.Count, _keyGroups[0][0].Length, LevelBasicInformation.HeadPending));
                    }

                    if (ct != null && _keyGroups[0][0].NextToward != null)
                    {
                        var ttObj = Instantiate(TowardTip, wp, gameObject.transform.rotation, gameObject.transform);
                        ttObj.transform.Rotate(new Vector3(0, 0, (float)_keyGroups[0][0].NextToward), Space.Self);
                        ct.OnInvalided += (s) =>
                        {
                            Destroy(ttObj);
                        };
                    }
                    if (Reser)
                    {
                        var wp1 = Camera.main.ScreenToWorldPoint(new Vector3(LevelBasicInformation.Accuracy + LevelBasicInformation.Accuracy * Last_Pos_Save.Value.x, 0, 0));
                        wp1.y = Last_Pos_Save.Value.y;
                        var wp2 = Camera.main.ScreenToWorldPoint(new Vector3(LevelBasicInformation.Accuracy + LevelBasicInformation.Accuracy * (float)(_keyGroups[0][0].Type == KeyType.Drag ? _keyGroups[0][0].DragData.From : _keyGroups[0][0].Pos), 0, 0));
                        wp2.y = wp.y;

                        wp1.z = 10;
                        wp2.z = 10;

                        LineArea.Create(LineAreaObj, gameObject, wp1, wp2, LevelBasicInformation.HeadPending * Metronome.BeatSpeed);

                        if(Math.Abs(wp1.y) >3.2|| Math.Abs(wp2.y) > 3.2)
                        {
                            Debug.Log("Enter");
                        }
                    }

                    Last_Pos_Save = new Vector2((float)(_keyGroups[0][0].Type == KeyType.Drag ? _keyGroups[0][0].DragData.From : _keyGroups[0][0].Pos), wp.y);
                    Reser = true;

                    _keyGroups[0].RemoveAt(0);
                    if (_keyGroups[0].Count == 0)
                    {
                        break;
                    }

                }
            };
        else
            Metronome.OnBeat += (t) =>    //World Coord is RECOMMANDED to Use
            {
                while (true)
                {
                    if (_keyGroups.Count == 0) return;
                    if (_keyGroups[0].Count == 0) { new_section_flag = true; _keyGroups.RemoveAt(0); continue; }
                    break;
                }

                bool Reser = false;
                Vector2? Last_Pos_Save = null;

                while (t + LevelBasicInformation.HeadPending >= _keyGroups[0][0].Time)
                {
                    if (new_section_flag) { new_section_flag = false; }

                    //????????????????????ж???????????
                    Vector3 CreateAtHere = new Vector2();

                    if (_keyGroups[0][0].ForceY != null)
                    {
                        CreateAtHere.y = (float)_keyGroups[0][0].ForceY.Value;
                    }
                    else
                        CreateAtHere.y = (float)Metronome.GetJudgeY(_keyGroups[0][0].Time);

                    CreateAtHere.x = (float)_keyGroups[0][0].Pos;
                    CreateAtHere.z = -9f;


                    Keys ct = null;
                    if (_keyGroups[0][0].Type == KeyType.Tap)
                    {
                        ct = CreateTap(CreateAtHere, LevelBasicInformation.HeadPending);

                        var dc = CreateVecLine(CreateAtHere, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
                        //dc.Key = ct.BAnimation;

                    }
                    else if (_keyGroups[0][0].Type == KeyType.Hold)
                    {
                        ct = CreateHold(CreateAtHere, (float)_keyGroups[0][0].Length, LevelBasicInformation.HeadPending);

                        var dc = CreateVecLine(CreateAtHere, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
                        //dc.Key = ct.BAnimation;
                    }
                    else if (_keyGroups[0][0].Type == KeyType.Slide)
                    {
                        ct = CreateSlide(CreateAtHere, LevelBasicInformation.HeadPending);

                        var dc = CreateVecLine(CreateAtHere, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
                        //dc.Key = ct.BAnimation;
                    }
                    else if (_keyGroups[0][0].Type == KeyType.Wave)
                    {
                        ct = CreateWave(CreateAtHere, _keyGroups[0][0].Children, (float)_keyGroups[0][0].Length, _keyGroups[0][0].TimeOfLastChild, _keyGroups[0][0].Rotate, LevelBasicInformation.HeadPending, (float)_keyGroups[0][0].WaveScale);
                        
                        var dc = CreateVecLine(CreateAtHere, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
                        //dc.Key = ct.BAnimation;
                    }
                    else if (_keyGroups[0][0].Type == KeyType.HWave)
                    {
                        ct = CreateHWave(CreateAtHere, _keyGroups[0][0].Children, (float)_keyGroups[0][0].Length, _keyGroups[0][0].TimeOfLastChild, LevelBasicInformation.HeadPending, (float)_keyGroups[0][0].WaveScale);

                        var dc = CreateVecLine(CreateAtHere, gameObject.transform, 1f / (LevelBasicInformation.HeadPending * Metronome.BeatSpeed));
                        //dc.Key = ct.BAnimation;
                    }
                    else if (_keyGroups[0][0].Type == KeyType.Drag)
                    {

                        StartCoroutine(CreateDrag(_keyGroups[0][0].DragData.From, _keyGroups[0][0].DragData.To, _keyGroups[0][0].DragData.Count, _keyGroups[0][0].Length, LevelBasicInformation.HeadPending));

                    }
                    if (ct != null && _keyGroups[0][0].NextToward != null)
                    {
                        var ttObj = Instantiate(TowardTip, CreateAtHere, gameObject.transform.rotation, gameObject.transform);
                        ttObj.transform.Rotate(new Vector3(0, 0, (float)_keyGroups[0][0].NextToward), Space.Self);
                        ct.OnInvalided += (s) =>
                        {
                            Destroy(ttObj);
                        };
                    }
                    if (Reser)
                    {
                        var wp1 = new Vector3(Last_Pos_Save.Value.x, Last_Pos_Save.Value.y, 10);
                        var wp2 = new Vector3((float)(_keyGroups[0][0].Type == KeyType.Drag ? _keyGroups[0][0].DragData.From : _keyGroups[0][0].Pos), CreateAtHere.y, 10);


                        LineArea.Create(LineAreaObj, gameObject, wp1, wp2, LevelBasicInformation.HeadPending * Metronome.BeatSpeed);
                    }

                    Last_Pos_Save = new Vector2((float)(_keyGroups[0][0].Type == KeyType.Drag ? _keyGroups[0][0].DragData.From : _keyGroups[0][0].Pos), CreateAtHere.y);
                    Reser = true;

                    _keyGroups[0].RemoveAt(0);
                    if (_keyGroups[0].Count == 0)
                    {
                        break;
                    }

                }
            };
    }
    
    #region Keys
    public GameObject Tap;
    public GameObject Hold;
    public GameObject Slide;
    public GameObject Wave;
    public GameObject Drag;
    public GameObject HWave;
    public GameObject LineAreaObj;
    public GameObject Drop;
    public GameObject PrePoint_Obj;
    public GameObject TowardTip;
    #endregion
    
    bool new_section_flag = true;
}
