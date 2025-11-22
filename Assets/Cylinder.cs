using UnityEngine;

public class Cylinder : MonoBehaviour
{
    [Header("Cylinder Settings")]
    public Material material;
    public float cylinderRadius = 1f;
    public float cylinderHeight = 2f;
    public int heightSegments = 10;
    public int radialSegments = 24;

    [Header("Position")]
    public Vector2 cylinderPos;
    public float zPos;

    [Header("Rotation")]
    public float zRot = 0;
    public float xRot = 0;
    public float yRot = 0;

    private void OnPostRender()
    {
        DrawCylinder();
    }

    public void DrawCylinder()
    {
        if (material == null)
        {
            Debug.LogError("You need to add a material");
            return;
        }

        GL.PushMatrix();
        material.SetPass(0);
        GL.Begin(GL.LINES);

        float halfHeight = cylinderHeight * 0.5f;

        for (int i = 0; i < radialSegments; i++)
        {
            float angle = 2f * Mathf.PI * i / radialSegments;

            float x = Mathf.Cos(angle) * cylinderRadius;
            float z = Mathf.Sin(angle) * cylinderRadius;

            Vector3[] topArr = new Vector3[1] { new Vector3(x, halfHeight, z) };
            RotationalMatrixComputation(ref topArr);
            Vector3 top3D = topArr[0];

            Vector3[] bottomArr = new Vector3[1] { new Vector3(x, -halfHeight, z) };
            RotationalMatrixComputation(ref bottomArr);
            Vector3 bottom3D = bottomArr[0];

            float pTop = PerspectiveCamera.Instance.GetPerspective(zPos + top3D.z);
            float pBottom = PerspectiveCamera.Instance.GetPerspective(zPos + bottom3D.z);

            Vector2 top2D = new Vector2(top3D.x + cylinderPos.x, top3D.y + cylinderPos.y) * pTop;
            Vector2 bottom2D = new Vector2(bottom3D.x + cylinderPos.x, bottom3D.y + cylinderPos.y) * pBottom;

            GL.Vertex(top2D);
            GL.Vertex(bottom2D);
        }

        for (int h = 1; h < heightSegments; h++)
        {
            float t = (float)h / heightSegments;
            float y = Mathf.Lerp(-halfHeight, halfHeight, t);
            DrawCircle3D(y);
        }

        GL.End();
        GL.PopMatrix();
    }

    private void DrawCircle3D(float y)
    {
        Vector2[] points = new Vector2[radialSegments];

        for (int i = 0; i < radialSegments; i++)
        {
            float angle = 2f * Mathf.PI * i / radialSegments;

            float x = Mathf.Cos(angle) * cylinderRadius;
            float z = Mathf.Sin(angle) * cylinderRadius;

            Vector3[] arr = new Vector3[1] { new Vector3(x, y, z) };
            RotationalMatrixComputation(ref arr);
            Vector3 rotated = arr[0];

            float perspective = PerspectiveCamera.Instance.GetPerspective(zPos + rotated.z);
            points[i] = new Vector2(rotated.x + cylinderPos.x, rotated.y + cylinderPos.y) * perspective;
        }

        for (int i = 0; i < radialSegments; i++)
        {
            GL.Vertex(points[i]);
            GL.Vertex(points[(i + 1) % radialSegments]);
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