using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class BottleDestroyer : MonoBehaviour, IInputClickHandler {

    public GameObject particleEffect;

    private void Start() {
        GameManager.BottleInGame++;
    }

    public void OnInputClicked(InputClickedEventData eventData) {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.GunFire);
        StartCoroutine(DestroyBottle());
    }



    IEnumerator DestroyBottle() {
        yield return new WaitForSeconds(0.1f);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.BottleKill);
        GameObject effect = Instantiate(particleEffect) as GameObject;
        effect.transform.position = gameObject.transform.position;
        GameManager.Score++;
        GameManager.BottleInGame--;
        Destroy(gameObject);
    }

}

