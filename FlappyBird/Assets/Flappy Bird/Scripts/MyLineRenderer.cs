using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLineRenderer : MonoBehaviour
{
    static Material lineMaterial;
    public static List<List<Vector3>> lineStrips;

	void Awake ()
    {
        CreateLineMaterial();
        lineStrips = new List<List<Vector3>>();
    }

    public static void Init()
    {
        foreach (List<Vector3> strip in lineStrips)
            strip.Clear();
        lineStrips.Clear();
    }

    static void CreateLineMaterial()
    {
        // Unity has a built-in shader that is useful for drawing
        // simple colored things.
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        // Turn on alpha blending
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn backface culling off
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        // Turn off depth writes
        lineMaterial.SetInt("_ZWrite", 0);
    }

    private void OnRenderObject()
    {
        lineMaterial.SetPass(0);
        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        foreach (List<Vector3> strip in lineStrips)
        {
            GL.Begin(GL.LINE_STRIP);
            foreach (Vector3 point in strip)
            {
                // Vertex colors change from red to green
                GL.Color(Color.red);
                // One vertex at transform position
                GL.Vertex3(point.x, point.y, 5.0f);
            }
            GL.End();
        }
        GL.PopMatrix();
    }
}