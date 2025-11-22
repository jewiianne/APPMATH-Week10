using UnityEngine;

public class LineGen : MonoBehaviour
{
    [Header("Cube Settings")]
    public Material material;
    public float cubeSize = 1f;

    [Header("Position")]
    public Vector2 cubePos;
    public float zPos;

    [Header("Rotation")]
    public float zRot;
    public float yRot;
    public float xRot;

    private void OnPostRender()
    {
        DrawCube();
    }

    private void DrawCube()
    {
        if (!material)
        {
            Debug.LogError("You need to add a material");
            return;
        }

        GL.PushMatrix();
        material.SetPass(0);
        GL.Begin(GL.LINES);

        Vector3[] front = GetFace(+cubeSize * 0.5f);
        Vector3[] back  = GetFace(-cubeSize * 0.5f);

        RotationalMatrixComputation(ref front);
        RotationalMatrixComputation(ref back);

        Vector2[] front2D = ProjectTo2D(front);
        Vector2[] back2D  = ProjectTo2D(back);

        DrawSquare(front2D);
        DrawSquare(back2D);

        for (int i = 0; i < 4; i++)
        {
            GL.Vertex(front2D[i]);
            GL.Vertex(back2D[i]);
        }

        GL.End();
        GL.PopMatrix();
    }

    private Vector3[] GetFace(float zOffset)
    {
        return new Vector3[]
        {
            new Vector3(+cubeSize, +cubeSize, zPos + zOffset),
            new Vector3(-cubeSize, +cubeSize, zPos + zOffset),
            new Vector3(-cubeSize, -cubeSize, zPos + zOffset),
            new Vector3(+cubeSize, -cubeSize, zPos + zOffset)
        };
    }

    private Vector2[] ProjectTo2D(Vector3[] verts)
    {
        Vector2[] r = new Vector2[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            float p = PerspectiveCamera.Instance.GetPerspective(verts[i].z);
            r[i] = (cubePos + new Vector2(verts[i].x, verts[i].y)) * p;
        }

        return r;
    }

    private void DrawSquare(Vector2[] v)
    {
        for (int i = 0; i < 4; i++)
        {
            GL.Vertex(v[i]);
            GL.Vertex(v[(i + 1) % 4]);
        }
    }

    private void RotationalMatrixComputation(ref Vector3[] points)
    {
        float radX = xRot * Mathf.Deg2Rad;
        float radY = yRot * Mathf.Deg2Rad;
        float radZ = zRot * Mathf.Deg2Rad;

        float cosX = Mathf.Cos(radX);
        float sinX = Mathf.Sin(radX);

        float cosY = Mathf.Cos(radY);
        float sinY = Mathf.Sin(radY);

        float cosZ = Mathf.Cos(radZ);
        float sinZ = Mathf.Sin(radZ);

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 p = points[i];

            float y1 = p.y * cosX - p.z * sinX;
            float z1 = p.y * sinX + p.z * cosX;
            p = new Vector3(p.x, y1, z1);

            float x2 = p.x * cosY + p.z * sinY;
            float z2 = -p.x * sinY + p.z * cosY;
            p = new Vector3(x2, p.y, z2);

            float x3 = p.x * cosZ - p.y * sinZ;
            float y3 = p.x * sinZ + p.y * cosZ;
            p = new Vector3(x3, y3, p.z);

            points[i] = p;
        }
    }
}
