using UnityEngine;

public class Pyramid : MonoBehaviour
{
    [Header("Pyramid")]
    public Material material;
    public float baseSize = 1f;
    public float height = 1.5f;

    [Header("Position")]
    public Vector2 basePos;
    public float zPos;

    [Header("Rotation")]
    public float zRot;
    public float yRot;
    public float xRot;

    private void OnPostRender()
    {
        DrawPyramid();
    }

    private void DrawPyramid()
    {
        if (!material)
        {
            Debug.LogError("You need to add a material");
            return;
        }

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        Vector3[] baseFace = GetBase3D();
        Vector3[] topArr   = { GetTop3D() };

        RotationalMatrixComputation(ref baseFace);
        RotationalMatrixComputation(ref topArr);

        Vector2[] base2D = ProjectTo2D(baseFace);
        Vector2 top2D    = ProjectTo2D(topArr)[0];

        DrawSquare(base2D);

        for (int i = 0; i < 4; i++)
        {
            GL.Vertex(top2D);
            GL.Vertex(base2D[i]);
        }

        GL.End();
        GL.PopMatrix();
    }

    private Vector3[] GetBase3D()
    {
        float h = baseSize * 0.5f;

        return new Vector3[]
        {
            new Vector3(+h, +h, zPos),
            new Vector3(-h, +h, zPos),
            new Vector3(-h, -h, zPos),
            new Vector3(+h, -h, zPos),
        };
    }

    private Vector3 GetTop3D()
    {
        return new Vector3(0, 0, zPos - height);
    }

    private Vector2[] ProjectTo2D(Vector3[] verts)
    {
        Vector2[] result = new Vector2[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            float p = PerspectiveCamera.Instance.GetPerspective(verts[i].z);
            result[i] = (basePos + new Vector2(verts[i].x, verts[i].y)) * p;
        }

        return result;
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