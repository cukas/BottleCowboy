using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        StartCoroutine(KillMe());
	}

    IEnumerator KillMe (){
        yield return new WaitForSeconds(0.85f);
        Destroy(gameObject);
    }
}
