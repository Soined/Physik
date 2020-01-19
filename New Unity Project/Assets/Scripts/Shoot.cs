using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Animator gunAnimator;
    public Transform shootingPoint;
    public KeyCode freezeShootKey;
    public KeyCode flipShootKey;
    public GameObject lightningParent;
    public GameObject lightningEnd;

    private float shootCooldown = 0f;
    private float _shootCooldown = 0f;
    private bool isOnCD = false;

    private float lightningDuration = 0.15f;
    private float _lightningDuration = 0f;
    private bool lightningIsActive = false;

    private void Start()
    {
        shootCooldown = .4f;
        lightningParent.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKey(freezeShootKey))
        {
            HandleShot(true);
        }
        if(Input.GetKey(flipShootKey))
        {
            HandleShot(false);
        }

        if(isOnCD)
        {
            _shootCooldown += Time.deltaTime;
            if(_shootCooldown >= shootCooldown)
            {
                isOnCD = false;
                _shootCooldown = 0f;
            }
        }

        if(lightningIsActive)
        {
            _lightningDuration += Time.deltaTime;

            if(_lightningDuration >= lightningDuration)
            {
                lightningParent.SetActive(false);
                _lightningDuration = 0f;
                lightningIsActive = false;
            }
        }
    }

    private void HandleShot(bool isFreeze)
    {
        if (isOnCD || Time.timeScale == 0) return;

        gunAnimator.SetTrigger("shootTrigger");
        SoundManager.Instance.PlayNewSound(AudioType.gunShot);

        //We shoot a Ray through the middle of our screen and see what we find
        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0)), out RaycastHit hit))
        {
            //Debug.Log($"found: {hit.collider.name}");

            if(hit.collider.GetComponentInParent<GravityPhysics>() != null)
            {
                ActivateLightning(hit);
                GravityPhysics other = hit.collider.GetComponentInParent<GravityPhysics>();
                if(isFreeze)
                {
                    HandleFreezeOnOther(other);
                } else
                {
                    HandleFlipOnOther(other);   
                }

            }
        }
        isOnCD = true;
    }

    private void HandleFreezeOnOther(GravityPhysics other)
    {
        if (other.IsEnabled())
        {
            other.DisableGravity();
            Debug.Log($"got it disabled");
        }
        else
        {
            other.EnableGravity();
            Debug.Log($"got it enabled");

        }
    }

    private void HandleFlipOnOther(GravityPhysics other)
    {
        other.FlipGravity();
    }

    private void ActivateLightning(RaycastHit hit)
    {
        lightningParent.SetActive(true);
        lightningEnd.transform.position = hit.point;
        lightningIsActive = true;
    }
}
