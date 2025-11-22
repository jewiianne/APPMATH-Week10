using UnityEngine;

public class Rectangle : MonoBehaviour
{
    [Header("Rectangle")]
    public Material material;
    public float width = 2f;   
    public float height = 6f;  
    public float depth = 2f; 

    [Header("Position")]
    public Vector2 rectPos;
    public float zPos;

    [Header("Rotation")]
    public float zRot = 0;
    public float xRot = 0;
    public float yRot = 0;

    private void OnPostRender()
    {
        DrawRectangle();
    }

    public void DrawRectangle()
    {
        if (material == null)
        {
            Debug.LogError("You need to add a material!");
            return;
        }

        GL.PushMatrix();
        material.SetPass(0);
        GL.Begin(GL.LINES);

        Vector3[] frontFace = GetRectangleFace(zPos + depth * 0.5f);
        Vector3[] backFace = GetRectangleFace(zPos - depth * 0.5f);

        RotationalMatrixComputation(ref frontFace);
        RotationalMatrixComputation(ref backFace);

        Vector2[] front2D = ProjectTo2D(frontFace);
        Vector2[] back2D = ProjectTo2D(backFace);

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

    private Vector3[] GetRectangleFace(float z)
    {
        float w = width * 0.5f;
        float h = height * 0.5f;

        return new Vector3[]
        {
            new Vector3(rectPos.x + w, rectPos.y + h, z),
            new Vector3(rectPos.x - w, rectPos.y + h, z),
            new Vector3(rectPos.x - w, rectPos.y - h, z),
            new Vector3(rectPos.x + w, rectPos.y - h, z),
        };
    }

    private Vector2[] ProjectTo2D(Vector3[] points)
    {
        Vector2[] projected = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            float perspective = PerspectiveCamera.Instance.GetPerspective(points[i].z);
            projected[i] = new Vector2(points[i].x, points[i].y) * perspective;
        }
        return projected;
    }

    private void DrawSquare(Vector2[] verts)
    {
        for (int i = 0; i < 4; i++)
        {
            int next = (i + 1) % 4;
            GL.Vertex(verts[i]);
            GL.Vertex(verts[next]);
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
