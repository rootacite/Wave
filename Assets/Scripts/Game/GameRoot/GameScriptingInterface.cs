using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class GameScripting : MonoBehaviour
{
    public void DisablePause(bool tf)
    {
        pauseButton.GetComponent<Button>().interactable = !tf;
    }
}
