using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adjust : MonoBehaviour
{
    private MeshFilter filter;
    private Mesh mesh;
    private Camera cameraMain;
    private Vector3 lastPoint;
    private bool isSwell = false;
    private MeshCollider collider;

    private void Awake()
    {
        filter = GetComponent<MeshFilter>();
        mesh = filter.mesh;
        cameraMain = Camera.main;
        lastPoint = Vector3.zero;
        collider = GetComponent<MeshCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.up, 20);
        var vertices = mesh.vertices;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cameraMain.ScreenPointToRay(Input.mousePosition), out RaycastHit hitinfo))
            {
                lastPoint = hitinfo.point;
                isSwell = true;
            }
        }

        if (!isSwell)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            if(Physics.Raycast(cameraMain.ScreenPointToRay(Input.mousePosition), out RaycastHit hitinfo))
            {
                var swellDir = 1;
                var thisPoint = hitinfo.point;
                if(thisPoint.x < transform.position.x)
                {
                    swellDir = -1;
                }
                var swellScale = thisPoint.x - lastPoint.x;
                var stretchScale = thisPoint.y - lastPoint.y;
                
                Vector3 collidePoint = filter.transform.InverseTransformPoint(hitinfo.point);
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].y >= collidePoint.y - 0.02f && vertices[i].y < collidePoint.y + 0.02f)
                    {
                        var dir = new Vector3(vertices[i].x, 0, vertices[i].z);
                        dir = Vector3.Normalize(dir);
                        vertices[i] += dir * swellScale * 0.1f * swellDir;
                    }

                    vertices[i].y += stretchScale * vertices[i].y * 0.5f;
                }

                lastPoint = thisPoint;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastPoint = Vector3.zero;
            isSwell = false;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        collider.sharedMesh = mesh;
    }
}
