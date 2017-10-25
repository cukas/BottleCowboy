using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

public class ObjectCollectionManager : Singleton<ObjectCollectionManager> {

    [Tooltip("A collection of small prefabs to generate on all places in the world.")]
    public List<GameObject> SmallPrefabs;

    [Tooltip("The desired size of small sized objects in the world.")]
    public Vector3 SmallSize = new Vector3(1.0f, 1.0f, 1.0f);

    [Tooltip("A collection of big objects prefabs to generate near walls in the world.")]
    public List<GameObject> BigObjectPrefabs;

    [Tooltip("The desired size of big objects in the world.")]
    public Vector3 BigObjectSize = new Vector3(1.0f, 1.0f, 1.0f);

    [Tooltip("A collection of medium objects prefabs to generate in the world.")]
    public List<GameObject> MediumObjectPrefabs;

    [Tooltip("The desired size of tall objects in the world.")]
    public Vector3 MediumObjectsSize = new Vector3(1.0f, 1.0f, 1.0f);

    [Tooltip("A collection of wall prefabs to generate on walls in the world.")]
    public List<GameObject> WallPrefabs;

    [Tooltip("The desired size of wall objects in the world.")]
    public Vector3 WallObjectSize = new Vector3(1.0f, 1.0f, 1.0f);

    [Tooltip("Will be calculated at runtime if is not preset.")]
    public float ScaleFactor;

    public List<GameObject> ActiveHolograms = new List<GameObject>();

    public void CreateSmallPrefabs(int number, Vector3 positionCenter, Quaternion rotation) {
        CreateModels(SmallPrefabs[number], positionCenter, rotation, SmallSize);
    }

    public void CreateBigObjects(int number, Vector3 positionCenter, Quaternion rotation) {
        CreateModels(BigObjectPrefabs[number], positionCenter, rotation, BigObjectSize);
    }

    public void CreateMediumObjects(int number, Vector3 positionCenter, Quaternion rotation) {
        CreateModels(MediumObjectPrefabs[number], positionCenter, rotation, MediumObjectsSize);
    }
    public void CreateWallObjects(int number, Vector3 positionCenter, Quaternion rotation) {
        CreateModels(WallPrefabs[number], positionCenter, rotation, WallObjectSize);
    }


    private void CreateModels(GameObject modelsToCreate, Vector3 positionCenter, Quaternion rotation, Vector3 desiredSize) {
        // Stay center in the square but move down to the ground
        var position = positionCenter - new Vector3(0, desiredSize.y * .5f, 0);

        GameObject newObject = Instantiate(modelsToCreate, position, rotation);

        if (newObject != null) {
            // Set the parent of the new object the GameObject it was placed on
            newObject.transform.parent = gameObject.transform;

            ActiveHolograms.Add(newObject);
        }
    }

}