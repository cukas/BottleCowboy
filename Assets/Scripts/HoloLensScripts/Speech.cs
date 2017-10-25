using HoloToolkit.Unity;
using UnityEngine;

public class Speech : MonoBehaviour {
    public SpatialUnderstandingCustomMesh SpatialUnderstandingMesh;
    private MeshRenderer textRender;
    private void Start() {
        textRender = gameObject.GetComponent<MeshRenderer>();
    }


    public  void ShowScore() {
         if (SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Done) return;
            textRender.enabled = true;
      
    }

    public void HideScore() {
        if (SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Done) return;
        textRender.enabled = false;

    }


}