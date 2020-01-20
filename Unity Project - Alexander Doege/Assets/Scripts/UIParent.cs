using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParent : MonoBehaviour
{
    public GameObject EndScreenPanel;
    public FPController player;

    #region Singleton

    public static UIParent Main;

    private void Awake()
    {
        if(Main == null)
        {
            Main = this;
        } else
        {
            Destroy(this);
        }
    }

    #endregion


    private void Start()
    {
        EndScreenPanel.gameObject.SetActive(false);

        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPController>();
        }
    }

    public void ActivateEndScreen()
    {
        Time.timeScale = 0;
        player.DisableControlls();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        EndScreenPanel.gameObject.SetActive(true);
    }

    #region Buttons

    public void QuitButton()
    {
        Application.Quit();
    }

    public void RestartButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        player.EnableControlls();
        EndScreenPanel.gameObject.SetActive(false);
        player.Die();
    }

    #endregion
}
