using UnityEngine;

public class Sphere : MonoBehaviour
{
    [Header("Sphere Settings")]
    public Material material;
    public float sphereRadius = 1f;
    public int latSegments = 10;
    public int lonSegments = 20;

    [Header("Position")]
    public Vector2 spherePos;
    public float zPos;

    [Header("Rotation")]
    public float zRot = 0;
    public float xRot = 0;
    public float yRot = 0;

    private void OnPostRender()
    {
        DrawSphere();
    }

    public void DrawSphere()
    {
        if (!material)
        {
            Debug.LogError("You need to add a material!");
            return;
        }

        GL.PushMatrix();
        material.SetPass(0);
        GL.Begin(GL.LINES);

        DrawLatitudeRings();
        DrawLongitudeArcs();

        GL.End();
        GL.PopMatrix();
    }

    private void DrawLatitudeRings()
    {
        for (int lat = 1; lat < latSegments; lat++)
        {
            float theta = Mathf.PI * lat / latSegments;
            float y = Mathf.Cos(theta) * sphereRadius;
            float ringRadius = Mathf.Sin(theta) * sphereRadius;

            Vector2[] ring = new Vector2[lonSegments];

            for (int lon = 0; lon < lonSegments; lon++)
            {
                float phi = 2f * Mathf.PI * lon / lonSegments;
                float x = Mathf.Cos(phi) * ringRadius;
                float z = Mathf.Sin(phi) * ringRadius;

                Vector3[] point3D = new Vector3[1] { new Vector3(x, y, z) };
                RotationalMatrixComputation(ref point3D);

                Vector3 rotated = point3D[0];
                float perspective = PerspectiveCamera.Instance.GetPerspective(zPos + rotated.z);
                ring[lon] = (spherePos + new Vector2(rotated.x, rotated.y)) * perspective;
            }

            RenderConnectedLine(ring);
        }
    }

    private void DrawLongitudeArcs()
    {
        for (int lon = 0; lon < lonSegments; lon++)
        {
            float phi = 2f * Mathf.PI * lon / lonSegments;
            Vector2[] arc = new Vector2[latSegments + 1];

            for (int lat = 0; lat <= latSegments; lat++)
            {
                float theta = Mathf.PI * lat / latSegments;
                float y = Mathf.Cos(theta) * sphereRadius;
                float ringRadius = Mathf.Sin(theta) * sphereRadius;

                float x = Mathf.Cos(phi) * ringRadius;
                float z = Mathf.Sin(phi) * ringRadius;

                Vector3[] point3D = new Vector3[1] { new Vector3(x, y, z) };
                RotationalMatrixComputation(ref point3D);

                Vector3 rotated = point3D[0];
                float perspective = PerspectiveCamera.Instance.GetPerspective(zPos + rotated.z);
                arc[lat] = (spherePos + new Vector2(rotated.x, rotated.y)) * perspective;
            }

            RenderConnectedLine(arc);
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

    private void RenderConnectedLine(Vector2[] points)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            GL.Vertex(points[i]);
            GL.Vertex(points[i + 1]);
        }

        GL.Vertex(points[points.Length - 1]);
        GL.Vertex(points[0]);
    }
}