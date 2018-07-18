using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneClick : MonoBehaviour {
    public SkeletonAnimation anim;

    public void OnClick() {

        //Debug.Log("Click");

        anim.loop = false;
        anim.AnimationName = "click";

        anim.state.Complete += ResetAnim;
    }

    void ResetAnim(TrackEntry trackEntry)
    {
        anim.loop = true;
        anim.AnimationName = "animation";

        anim.state.Complete -= ResetAnim;
    }
}
