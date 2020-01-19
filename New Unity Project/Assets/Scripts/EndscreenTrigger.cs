using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndscreenTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && Time.timeScale > 0)
        {
            UIParent.Main.ActivateEndScreen();
        }
    }
}
