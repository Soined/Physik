using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Animator gunAnimator;
    public Transform shootingPoint;
    public KeyCode freezeShootKey;
    public KeyCode flipShootKey;

    private float shootCooldown = 0f;
    private float _shootCooldown = 0f;
    private bool isOnCD = false;

    private void Start()
    {
        shootCooldown = .4f;
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
    }

    private void HandleShot(bool isFreeze)
    {
        if (isOnCD) return;

        gunAnimator.SetTrigger("shootTrigger");
        SoundManager.Instance.PlayNewSound(AudioType.gunShot);

        //We shoot a Ray through the middle of our screen and see what we find
        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0)), out RaycastHit hit))
        {
            //Debug.Log($"found: {hit.collider.name}");

            if(hit.collider.GetComponentInParent<GravityPhysics>() != null)
            {
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
}
