using System;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour {
    public bool DrawDebugBoxes = false;
    public bool DrawObject = true;


    public Material OccludedMaterial;

    public SpatialUnderstandingCustomMesh SpatialUnderstandingMesh;

    private readonly List<BoxDrawer.Box> _lineBoxList = new List<BoxDrawer.Box>();

    private readonly Queue<PlacementResult> _results = new Queue<PlacementResult>();

    private bool _timeToHideMesh;
    private BoxDrawer _boxDrawing;

    // Use this for initialization
    void Start() {
        if (DrawDebugBoxes) {
            _boxDrawing = new BoxDrawer(gameObject);
        }

    }

    void Update() {
        ProcessPlacementResults();

        if (_timeToHideMesh) {
            SpatialUnderstandingState.Instance.HideText = true;
            HideGridEnableOcclulsion();
            _timeToHideMesh = false;
        }

        if (DrawDebugBoxes) {
            _boxDrawing.UpdateBoxes(_lineBoxList);
        }

    }
    private void HideGridEnableOcclulsion() {
        SpatialUnderstandingMesh.MeshMaterial = OccludedMaterial;
    }

    public void CreateScene() {
        // Only if we're enabled
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding) {
            return;
        }
        //Calls the Solver after Spatial is finalized
        SpatialUnderstandingDllObjectPlacement.Solver_Init();

        SpatialUnderstandingState.Instance.SpaceQueryDescription = "Bottle Cowboy is Generating World";

        List<PlacementQuery> queries = new List<PlacementQuery>();

        if (DrawObject) {
            queries.AddRange(AddObjects());
          
        }

        GetLocationsFromSolver(queries);
    }

    public List<PlacementQuery> AddObjects() {
        var queries = CreateLocationQueriesForSolver(ObjectCollectionManager.Instance.BigObjectPrefabs.Count, ObjectCollectionManager.Instance.BigObjectSize, ObjectType.House);
        queries.AddRange(CreateLocationQueriesForSolver(ObjectCollectionManager.Instance.SmallPrefabs.Count, ObjectCollectionManager.Instance.SmallSize, ObjectType.Bottle));
        queries.AddRange(CreateLocationQueriesForSolver(ObjectCollectionManager.Instance.MediumObjectPrefabs.Count, ObjectCollectionManager.Instance.MediumObjectsSize, ObjectType.Table));
        queries.AddRange(CreateLocationQueriesForSolver(ObjectCollectionManager.Instance.WallPrefabs.Count, ObjectCollectionManager.Instance.WallObjectSize, ObjectType.Shield));
        return queries;
    }

    private int _placeBigObjects;
    private int _placedMidObjcts;
    private int _placedSmallObjects;
    private int _placedWallObjects;


    private void ProcessPlacementResults() {
        if (_results.Count > 0) {
            var toPlace = _results.Dequeue();
            // Output
            if (DrawDebugBoxes) {
                DrawBox(toPlace, Color.red);
            }

            var rotation = Quaternion.LookRotation(toPlace.Normal, Vector3.up);


            switch (toPlace.ObjType) {
                case ObjectType.Bottle:
                    ObjectCollectionManager.Instance.CreateSmallPrefabs(_placedSmallObjects++, toPlace.Position, rotation);
                    break;
                case ObjectType.House:
                    ObjectCollectionManager.Instance.CreateBigObjects(_placeBigObjects++, toPlace.Position, rotation);
                    break;
                case ObjectType.Table:
                    ObjectCollectionManager.Instance.CreateMediumObjects(_placedMidObjcts++, toPlace.Position, rotation);
                    break;
                case ObjectType.Shield:
                    ObjectCollectionManager.Instance.CreateWallObjects(_placedWallObjects++, toPlace.Position, rotation);
                    break;
            }
        }
    }

    private void DrawBox(PlacementResult boxLocation, Color color) {
        if (boxLocation != null) {
            _lineBoxList.Add(
                new BoxDrawer.Box(
                    boxLocation.Position,
                    Quaternion.LookRotation(boxLocation.Normal, Vector3.up),
                    color,
                    boxLocation.Dimensions * 0.5f)
            );
        }
    }

    private void GetLocationsFromSolver(List<PlacementQuery> placementQueries) {
#if UNITY_WSA && !UNITY_EDITOR
        System.Threading.Tasks.Task.Run(() =>
        {
            // Go through the queries in the list
            for (int i = 0; i < placementQueries.Count; ++i)
            {
                var result = PlaceObject(placementQueries[i].ObjType.ToString() + i,
                                         placementQueries[i].PlacementDefinition,
                                         placementQueries[i].Dimensions,
                                         placementQueries[i].ObjType,
                                         placementQueries[i].PlacementRules,
                                         placementQueries[i].PlacementConstraints);
                if (result != null)
                {
                    _results.Enqueue(result);
                }
            }

            _timeToHideMesh = true;
        });
#else
        _timeToHideMesh = true;
#endif
    }

    private PlacementResult PlaceObject(string placementName,
        SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition,
        Vector3 boxFullDims,
        ObjectType objType,
        List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null,
        List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null) {

        // New query
        if (SpatialUnderstandingDllObjectPlacement.Solver_PlaceObject(
                placementName,
                SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementDefinition),
                (placementRules != null) ? placementRules.Count : 0,
                ((placementRules != null) && (placementRules.Count > 0)) ? SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementRules.ToArray()) : IntPtr.Zero,
                (placementConstraints != null) ? placementConstraints.Count : 0,
                ((placementConstraints != null) && (placementConstraints.Count > 0)) ? SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementConstraints.ToArray()) : IntPtr.Zero,
                SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResultPtr()) > 0) {
            SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult placementResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResult();

            return new PlacementResult(placementResult.Clone() as SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult, boxFullDims, objType);
        }

        return null;
    }

    private List<PlacementQuery> CreateLocationQueriesForSolver(int desiredLocationCount, Vector3 boxFullDims, ObjectType objType) {
        List<PlacementQuery> placementQueries = new List<PlacementQuery>();

        var halfBoxDims = boxFullDims * 0.5f;

        var disctanceFromOtherObjects = halfBoxDims.x > halfBoxDims.z ? halfBoxDims.x * 2.2f : halfBoxDims.z * 2.2f;

        for (int i = 0; i < desiredLocationCount; ++i) {
            var placementRules = new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>{

                SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(disctanceFromOtherObjects)
                        
            };

            var placementConstraints = new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint>();

            if (objType == ObjectType.Bottle) {

                int randomPlace = UnityEngine.Random.Range(1,3);
                if (randomPlace == 1) {
                    SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloor(halfBoxDims);

                    placementQueries.Add(
                    new PlacementQuery(placementDefinition,
                        boxFullDims,
                        objType,
                        placementRules,
                        placementConstraints
                    ));
                } else if(randomPlace == 2) {
                    SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnShape(halfBoxDims, "Couch", 1);

                    placementQueries.Add(
                    new PlacementQuery(placementDefinition,
                        boxFullDims,
                        objType,
                        placementRules,
                        placementConstraints
                    ));
                } else {
                    SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnShape(halfBoxDims, "All Surfaces", 1);

                    placementQueries.Add(
                    new PlacementQuery(placementDefinition,
                        boxFullDims,
                        objType,
                        placementRules,
                        placementConstraints
                    ));
                }
                
            } else if (objType == ObjectType.Table) {
                SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloor(halfBoxDims);

                placementQueries.Add(
                new PlacementQuery(placementDefinition,
                    boxFullDims,
                    objType,
                    placementRules,
                    placementConstraints
                ));

            } else if (objType == ObjectType.House) {
                // SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromWalls(0.1f, 2.0f);
        
                SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnWall(halfBoxDims,-0.2f, 1.5f);

                placementQueries.Add(
                new PlacementQuery(placementDefinition,
                    boxFullDims,
                    objType,
                    placementRules,
                    placementConstraints
                ));

            } else if (objType == ObjectType.Shield) {
                SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnWall(halfBoxDims,1.7f,2.0f);

                placementQueries.Add(
                new PlacementQuery(placementDefinition,
                    boxFullDims,
                    objType,
                    placementRules,
                    placementConstraints
                ));

            }



        }

        return placementQueries;
    }

}