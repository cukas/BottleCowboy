using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class ShieldHit : MonoBehaviour, IInputClickHandler, IInputHandler {
    Animator anim;

 

    void Start () {
        anim = GetComponentInParent<Animator>();
	}

    public void OnInputClicked(InputClickedEventData eventData) {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.GunFire);
        if (gameObject.name == "Shield_right") {
            anim.SetBool("isHittingRight", true);
 
        } else {
            anim.SetBool("isHittingLeft", true);
        }
        
       
    }

    public void OnInputUp(InputEventData eventData) {
        if (gameObject.name == "Shield_right") {
            anim.SetBool("isHittingRight", false);

        } else {
            anim.SetBool("isHittingLeft", false);
        }

    }

    public void OnInputDown(InputEventData eventData) {

    }
}