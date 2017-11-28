using UnityEngine;
 
public class MeshDrawer : MonoBehaviour {
    float width;
    float height;

    Mesh mesh;
    public MeshFilter MeshFilter;
    public Material Material;
    public Material Material2;

    void Start()
    {
        Rect camRect = Camera.main.pixelRect;

        MeshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        MeshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(camRect.xMin, camRect.yMin, 0);
        vertices[1] = new Vector3(camRect.xMax, camRect.yMin, 0);
        vertices[2] = new Vector3(camRect.xMin, camRect.yMax, 0);
        vertices[3] = new Vector3(camRect.xMax, camRect.yMax, 0);

        mesh.vertices = vertices;

        int[] tri = new int[6];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;
    }

    private void Update()
    {
        Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, Material, 0);
        Graphics.DrawMesh(mesh, Vector3.one, Quaternion.identity, Material2, 0);
    }

}
