using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Reticle Animation Callback state machine control
 */
public class ReticleStateBehaviour : StateMachineBehaviour
{
    /// <summary>
    /// Disables the image once the animation has been played
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="animatorStateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Image>().enabled = false;
    }
}
