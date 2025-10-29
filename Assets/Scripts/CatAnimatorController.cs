using UnityEngine;

public class CatAnimatorController : MonoBehaviour
{
    public Animator animator;

    public void PlayIdle()
    {
        animator.SetTrigger("TRG_Idle");
    }


    public void PlayWalk()
    {
        animator.SetTrigger("TRG_Walk");
    }
}
