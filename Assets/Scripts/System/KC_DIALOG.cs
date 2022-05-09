using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KC_DIALOG : MonoBehaviour
{
    public GameObject ButtonOK;
    public GameObject ButtonCancel;
    public GameObject Panel;
    public GameObject Image;
    public GameObject Text;

    public string Content
    {
        get { return Text.GetComponent<TextMeshProUGUI>().text; }
        set { Text.GetComponent<TextMeshProUGUI>().text = value; }
    }
    public delegate int DialogResult();
    public event DialogResult OnOK;
    public event DialogResult OnCancel;

    IEnumerator DelayExit()
    {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        ButtonOK.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            int? r = OnOK?.Invoke();
            if (r == 0)
            {
                Panel.GetComponent<Animator>().SetTrigger("OnExit");
                StartCoroutine(DelayExit());
            }
        });

        ButtonCancel.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            int? r = OnCancel?.Invoke();
            if (r == 0)
            {
                Panel.GetComponent<Animator>().SetTrigger("OnExit");
                StartCoroutine(DelayExit());
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
