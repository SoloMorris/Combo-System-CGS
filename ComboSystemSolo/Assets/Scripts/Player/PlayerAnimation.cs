using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerAnimation : PlayerComponent
{
    private Animator animator;
    
    // Animator parameters
    private static readonly int Moving = Animator.StringToHash("Moving");
    private static readonly int Stop = Animator.StringToHash("Stop");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(AnimationClip anim)
    {
        animator.SetTrigger(anim.name);
    }

    public void OnMove()
    {
       animator.SetBool(Moving, true); 
    }

    public void StopAnimation()
    {
        animator.SetTrigger(Stop);
    }

    public void OnStopMove()
    {
        animator.SetBool(Moving, false);
    }
}
