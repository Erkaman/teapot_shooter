using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SurfaceManager : MonoBehaviour {

    private List<TrackedPlane> newPlanes = new List<TrackedPlane>();

    // Use this for initialization
    void Awake () {
	}
	
	// Update is called once per frame
	void Update () {
        // Check that motion tracking is tracking.
        if (Frame.TrackingState != TrackingState.Tracking)
        {
            return;
        }
        
        // Iterate over planes found in this frame. Create a game object with a surface component for these new planes,
        // so that the teapots collide with them.
        Frame.GetPlanes(newPlanes, TrackableQueryFilter.New);
        for (int i = 0; i < newPlanes.Count; i++)
        {
            TrackedPlane p = newPlanes[i];

            GameObject go = new GameObject();
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;

            go.AddComponent<Surface>().SetPlane(p);
        }

    }

    private class Surface : MonoBehaviour
    {
        private TrackedPlane plane;

        private Mesh mesh;  
        private MeshCollider meshCollider;

        // previous and current plane vertices.
        private List<Vector3> prevFrameVertices = new List<Vector3>();
        private List<Vector3> curFrameVertices = new List<Vector3>();

        List<int> indices;

        public void SetPlane(TrackedPlane plane)
        {
            this.plane = plane;
        }

        void Awake()
        {
            this.meshCollider = this.gameObject.AddComponent<MeshCollider>();         
            this.mesh = new Mesh();

            indices = new List<int>();
        }

        void Update()
        {
            if(plane == null)
            {
                return;
            }
            else if (plane.SubsumedBy != null)
            {
                // subsumed by other plane, so destroy.
                Destroy(gameObject);
            }

            else if (Frame.TrackingState != TrackingState.Tracking)
            {
                meshCollider.enabled = false;
                return;
            }

            meshCollider.enabled = true;

            plane.GetBoundaryPolygon(curFrameVertices);
     
            if (AreVerticesListsEqual(prevFrameVertices, curFrameVertices))
            {
                // the boundary polygon is unchanged, so no need to update this frame.
                return;
            }
            
            // we need to triangulate the boundary polygon, before giving it to the unity physics engine
            // we assume here that the boundary polygon is convex, and this seems to work very well.
            {
                Mesh m = mesh;
                m.Clear();

                indices.Clear();
                int firstIndex = 0;     
                for (int i = 1; i < curFrameVertices.Count - 1; ++i)
                {
                    indices.Add(firstIndex);
                    indices.Add(i + 0);
                    indices.Add(i + 1);
                    
                }

                m.SetVertices(curFrameVertices);
                m.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
                m.RecalculateBounds();
                
                meshCollider.sharedMesh = m;
            }

            prevFrameVertices.Clear();
            prevFrameVertices.AddRange(curFrameVertices);
        }

        private bool AreVerticesListsEqual(List<Vector3> firstList, List<Vector3> secondList)
        {
            if (firstList.Count != secondList.Count)
            {
                return false;
            }

            for (int i = 0; i < firstList.Count; i++)
            {
                if (firstList[i] != secondList[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}


