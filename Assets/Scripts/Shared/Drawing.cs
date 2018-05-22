using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

abstract class Drawing : MonoBehaviour
{
    public enum ColorModes { RGB, HSB };

    static Texture2D _texture;
    static Stack<Matrix4x4> _stack;
    static Matrix4x4 _matrix;
    static Matrix4x4 _reset;
    static Matrix4x4 _draw;
    static float _z = 0;
    static float _strokeWeight;
    static Material _fillMaterial;
    static Material _strokeMaterial;
    static bool _drawFill;
    static bool _drawStroke;
    static Mesh _quad;
    static Mesh _circle;
    static Mesh _box;
    static Mesh _sphere;
    static ColorModes _colorMode;
    static float _maxColorValue;
    static GameObject _light;
    static Dictionary<Color, Material> _fillMaterials;
    static Dictionary<Color, Material> _strokeMaterials;
    static GUIStyle _style;
    static bool _is3D = false;

    static List<Mesh> _meshPool = new List<Mesh>();
    static int _meshPoolIndex = 0;

    static List<Tuple<string, Vector2>> _texts = new List<Tuple<string, Vector2>>();

    public static int Width { get; private set; }
    public static int Height { get; private set; }

    static Drawing()
    {
        _quad = Quad();
        _circle = Circle();

        var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _box = box.GetComponent<MeshFilter>().sharedMesh;
        Destroy(box);

        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _sphere = sphere.GetComponent<MeshFilter>().sharedMesh;
        Destroy(sphere);

        _style = new GUIStyle
        {
            fontSize = 18,
            alignment = TextAnchor.LowerLeft
        };

        Defaults();
    }

    public static void Defaults()
    {
        if (_light != null)
        {
            Destroy(_light);
            _light = null;
        }

        _meshPool.Clear();
        _meshPoolIndex = 0;

        _drawFill = true;
        _fillMaterials = new Dictionary<Color, Material>();
        _fillMaterial = new Material(Shader.Find("Unlit/Transparent"));
        Fill(Color.black);

        _drawStroke = true;
        _strokeMaterials = new Dictionary<Color, Material>();
        _strokeMaterial = new Material(Shader.Find("Unlit/Transparent"));
        Stroke(Color.black);
        StrokeWeight(1);

        _stack = new Stack<Matrix4x4>();

        ViewSize(500, 500);
        Camera.main.backgroundColor = Color.black;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        ColorMode(ColorModes.RGB, 255);
        _texture = null;
    }

    public void Size(int width, int height, bool is3D = false)
    {
        ViewSize(width, height, is3D);
        Application.targetFrameRate = 60;

        var collider = gameObject.GetComponent<BoxCollider2D>();

        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();

        collider.size = new Vector2(width, height);
        collider.offset = new Vector2(width / 2, height / 2);
    }

    static void ViewSize(int width, int height, bool is3D = false)
    {
        Width = width;
        Height = height;
        _is3D = is3D;

        _reset.SetTRS(
                    new Vector3(0, height, 0),
                    Quaternion.Euler(180, 0, 0),
                    Vector3.one);

        _matrix = _reset;
        _z = 0;

        Screen.SetResolution(width, height, false);
        Camera.main.transform.position = new Vector3(width / 2, height / 2, -height);

        if (is3D)
        {
            Camera.main.orthographic = false;
        }
        else
        {
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = height / 2;
        }

#if UNITY_EDITOR
        var size = new Vector2(width, height);
        var windowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");
        var window = UnityEditor.EditorWindow.GetWindow(windowType);
        window.position = new Rect(window.position.x, window.position.y, width, height + 17);
        window.minSize = size;
        window.maxSize = size;
#endif
    }

    public static void Lights()
    {
        _fillMaterials.Clear();
        _fillMaterial = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        Fill(Color.white);

        _light = new GameObject("Light", typeof(Light));
        var light = _light.GetComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 0.85f;
        light.shadows = LightShadows.None;
        light.transform.parent = Camera.main.transform;
    }

    // pixel

    public static void Set(int x, int y, Color color)
    {
        if (_texture == null)
        {
            _texture = new Texture2D(Width, Height)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
        }

        _texture.SetPixel(x, Height - 1 - y, color);
    }

    public static Color Get(int x, int y)
    {
        return _texture.GetPixel(x, Height - 1 - y);
    }

    // attributes

    public static void ColorMode(ColorModes mode, float maxValue)
    {
        _colorMode = mode;
        _maxColorValue = maxValue;
    }

    public static void NoStroke() => _drawStroke = false;
    public static void NoFill() => _drawFill = false;
    public static void StrokeWeight(float weight) => _strokeWeight = weight;

    public static void Fill(Color color)
    {
        Material material;
        if (!_fillMaterials.TryGetValue(color, out material))
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();

            material = new Material(_fillMaterial)
            {
                mainTexture = tex
            };

            _fillMaterials.Add(color, material);
        }

        _fillMaterial = material;
    }

    public static void Fill(byte gray, float alpha)
    {
        float t = gray / _maxColorValue;
        float a = alpha / _maxColorValue;
        var color = new Color(t, t, t, a);
        Fill(color);
    }

    public static void Fill(int gray)
    {
        byte n = Convert.ToByte(gray);
        Fill(n, _maxColorValue);
    }

    public static void Fill(float v1, float v2, float v3)
    {
        Color color = _colorMode == ColorModes.RGB ?
                       new Color(v1 / 255, v2 / 255, v3 / 255, 1) :
                       Color.HSVToRGB(v1, v2, v3);

        Fill(color);
    }

    public static void Stroke(Color color)
    {
        Material material;
        if (!_strokeMaterials.TryGetValue(color, out material))
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();

            material = new Material(_strokeMaterial)
            {
                mainTexture = tex
            };

            _strokeMaterials.Add(color, material);
        }

        _strokeMaterial = material;
    }

    public static void Stroke(float gray)
    {
        float t = gray / _maxColorValue;
        var color = new Color(t, t, t, 1);
        Stroke(color);
    }

    public static void Stroke(byte gray, float alpha)
    {
        float t = gray / _maxColorValue;
        float a = alpha / _maxColorValue;
        var color = new Color(t, t, t, a);
        Stroke(color);
    }

    public static void Stroke(float v1, float v2, float v3)
    {
        Color color = _colorMode == ColorModes.RGB ?
                       new Color(v1 / 255, v2 / 255, v3 / 255, 1) :
                       Color.HSVToRGB(v1, v2, v3);

        Stroke(color);
    }

    public static void Background(Color color)
    {
        Camera.main.backgroundColor = color;
    }

    public static void Background(int r, int g, int b)
    {
        var color = new Color32((byte)r, (byte)g, (byte)b, 255);
        Background(color);
    }

    public static void Background(byte gray)
    {
        var color = new Color32(gray, gray, gray, 255);
        Background(color);
    }

    // draw methods

    public static void Point(float x, float y)
    {
        if (!_drawStroke) return;

        _input.SetTRS(
              new Vector2(x, y),
              Quaternion.identity,
              new Vector2(_strokeWeight, _strokeWeight)
              );

        DrawMultiply(ref _input);

        Graphics.DrawMesh(_circle, _draw, _strokeMaterial, 0, null, 0);
    }

    public static void Line(float x1, float y1, float x2, float y2)
    {
        if (!_drawStroke) return;

        var start = new Vector2(x1, y1);
        var end = new Vector2(x2, y2);
        var vector = end - start;

        _input.SetTRS(
               start + vector * 0.5f,
               Quaternion.LookRotation(Vector3.forward, vector),
               new Vector2(_strokeWeight, vector.magnitude)
               );

        DrawMultiply(ref _input);

        Graphics.DrawMesh(_quad, _draw, _strokeMaterial, 0, null, 0);
    }

    public static void Rect(float x, float y, float w, float h)
    {
        _input.SetTRS(
               new Vector2(x, y),
               Quaternion.identity,
               new Vector2(w, h)
               );

        DrawMultiply(ref _input);

        if (_drawFill)
        {
            Graphics.DrawMesh(_quad, _draw, _fillMaterial, 0, null, 0);
        }

        if (_drawStroke)
        {
            _draw.m23 -= 0.001f;
            Graphics.DrawMesh(_quad, _draw, _strokeMaterial, 0, null, 1);
        }
    }

    public static void Ellipse(float x, float y, float w, float h)
    {
        _input.SetTRS(
               new Vector2(x, y),
               Quaternion.identity,
               new Vector2(w, h)
               );

        DrawMultiply(ref _input);

        if (_drawFill)
        {
            Graphics.DrawMesh(_circle, _draw, _fillMaterial, 0, null, 0);
        }

        if (_drawStroke)
        {
            _draw.m23 -= 0.001f;
            Graphics.DrawMesh(_circle, _draw, _strokeMaterial, 0, null, 1);
        }
    }

    public static void Box(float w, float h, float d)
    {
        _input.SetTRS(
               Vector3.zero,
               Quaternion.identity,
               new Vector3(w, h, d)
               );

        DrawMultiply(ref _input);
        Graphics.DrawMesh(_box, _draw, _fillMaterial, 0, null, 0);
    }

    public static void Box(float size)
    {
        Box(size, size, size);
    }

    public static void Sphere(float r)
    {
        _input.SetTRS(
               Vector3.zero,
               Quaternion.identity,
               new Vector3(r, r, r)
               );

        DrawMultiply(ref _input);
        Graphics.DrawMesh(_sphere, _draw, _fillMaterial, 0);
    }

    static int[] _trisFaces = new[] { 0, 1, 2, 5, 4, 3 };
    static int[] _quadsFaces = new[] { 0, 1, 2, 3, 7, 6, 5, 4 };
    static Vector3[] _tris = new Vector3[6];
    static Vector3[] _quads = new Vector3[8];

    public static void Shape(params Vector3[] v)
    {
        int size = v.Length;

        if (size != 3 && size != 4)
            throw new ArgumentException(" Shape should have 3 or 4 vertices.");

        Mesh mesh;

        if (_meshPoolIndex > _meshPool.Count - 1)
        {
            mesh = new Mesh();
            mesh.MarkDynamic();
            _meshPool.Add(mesh);
        }
        else
        {
            mesh = _meshPool[_meshPoolIndex++];
        }

        bool isTri = size == 3;
        bool updateFaces = mesh.vertexCount != size * 2;

        var arr = isTri ? _tris : _quads;

        for (int i = 0; i < size; i++)
        {
            arr[i] = v[i];
            arr[size + i] = v[i];
        }

        mesh.vertices = arr;

        if (isTri)
        {
            var n = Vector3.Cross(v[1] - v[0], v[2] - v[0]).normalized;

            for (int i = 0; i < 3; i++)
            {
                arr[i] = n;
                arr[i + 3] = -n;
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                int prev = i == 0 ? 3 : i - 1;
                int next = i == 3 ? 0 : i + 1;
                var n = Vector3.Cross(v[next] - v[i], v[prev] - v[i]).normalized;
                arr[i] = n;
                arr[i + 4] = -n;
            }
        }

        mesh.normals = arr;

        if (updateFaces)
        {
            mesh.SetIndices(
                isTri ? _trisFaces : _quadsFaces,
                isTri ? MeshTopology.Triangles : MeshTopology.Quads,
                0,
                true
                );
        }

        Graphics.DrawMesh(mesh, _matrix, _fillMaterial, 0);
    }

    public static void Text(string text, float x, float y)
    {
        _texts.Add(Tuple.Create(text, new Vector2(x, y)));
    }

    // transform

    public static void PushMatrix() => _stack.Push(_matrix);
    public static void PopMatrix() => _matrix = _stack.Pop();

    public static void ApplyMatrix(ref Matrix4x4 matrix)
    {
        Multiply(ref matrix);
    }

    public static void Translate(Vector3 v, ref Matrix4x4 matrix)
    {
        v = matrix.MultiplyPoint3x4(v);
        matrix.m03 = v.x;
        matrix.m13 = v.y;
        matrix.m23 = v.z;
    }

    public static void Translate(float x, float y, float z = 0)
    {
        Translate(new Vector3(x, y, z), ref _matrix);
    }

    static Matrix4x4 _input = Matrix4x4.identity;

    public static void Rotate(Quaternion q, ref Matrix4x4 matrix)
    {
        _input.SetTRS(Vector3.zero, q, Vector3.one);

        Matrix4x4 result;
        Multiply(ref matrix, ref _input, out result);
        matrix = result;
    }

    static void Rotate(float a, float b, float c)
    {
        Rotate(Quaternion.Euler(a, b, c), ref _matrix);
    }

    public static void Rotate(float r) => Rotate(0, 0, r * Rad2Deg);
    public static void RotateX(float r) => Rotate(r * Rad2Deg, 0, 0);
    public static void RotateY(float r) => Rotate(0, r * Rad2Deg, 0);

    public static void Scale(float x, float y, float z)
    {
        _input.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(x, y, z));
        Multiply(ref _input);
    }

    public static void Scale(float s) => Scale(s, s, s);

    static void Multiply(ref Matrix4x4 input)
    {
        Matrix4x4 result;
        Multiply(ref _matrix, ref input, out result);
        _matrix = result;
    }

    static void DrawMultiply(ref Matrix4x4 input)
    {
        Multiply(ref _matrix, ref input, out _draw);

        if (!_is3D)
        {
            _draw.m23 = _z;
            _z -= 0.01f;
        }
    }

    public static void Multiply(ref Matrix4x4 a, ref Matrix4x4 b, out Matrix4x4 result)
    {
        result.m00 = a.m00 * b.m00 + a.m01 * b.m10 + a.m02 * b.m20 + a.m03 * b.m30;
        result.m01 = a.m00 * b.m01 + a.m01 * b.m11 + a.m02 * b.m21 + a.m03 * b.m31;
        result.m02 = a.m00 * b.m02 + a.m01 * b.m12 + a.m02 * b.m22 + a.m03 * b.m32;
        result.m03 = a.m00 * b.m03 + a.m01 * b.m13 + a.m02 * b.m23 + a.m03 * b.m33;
        result.m10 = a.m10 * b.m00 + a.m11 * b.m10 + a.m12 * b.m20 + a.m13 * b.m30;
        result.m11 = a.m10 * b.m01 + a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31;
        result.m12 = a.m10 * b.m02 + a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32;
        result.m13 = a.m10 * b.m03 + a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33;
        result.m20 = a.m20 * b.m00 + a.m21 * b.m10 + a.m22 * b.m20 + a.m23 * b.m30;
        result.m21 = a.m20 * b.m01 + a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31;
        result.m22 = a.m20 * b.m02 + a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32;
        result.m23 = a.m20 * b.m03 + a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33;
        result.m30 = a.m30 * b.m00 + a.m31 * b.m10 + a.m32 * b.m20 + a.m33 * b.m30;
        result.m31 = a.m30 * b.m01 + a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31;
        result.m32 = a.m30 * b.m02 + a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32;
        result.m33 = a.m30 * b.m03 + a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33;
    }

    // math

    public static float Sq(float n) => n * n;

    public static float Dist(float x1, float y1, float x2, float y2)
    {
        float x = x2 - x1;
        float y = y2 - y1;
        return Sqrt(x * x + y * y);
    }

    // primitives

    static Mesh Quad()
    {
        const float s = 0.5f;

        var v = new[]
        {
            new Vector3(-s,-s),
            new Vector3(s,-s),
            new Vector3(s,s),
            new Vector3(-s,s)
        };

        var quad = new Mesh()
        {
            vertices = v,
            normals = Enumerable.Repeat(-Vector3.up, 4).ToArray(),
            subMeshCount = 2
        };

        quad.SetIndices(new[] { 0, 1, 2, 3 }, MeshTopology.Quads, 0);
        quad.SetIndices(new[] { 0, 1, 2, 3, 0 }, MeshTopology.LineStrip, 1);
        return quad;
    }

    static Mesh Circle()
    {
        const int sides = 32;
        const float step = (PI * 2) / sides;
        var vertices = new Vector3[sides + 1];

        for (int i = 0; i < sides; i++)
        {
            float angle = step * i;
            float x = Cos(angle);
            float y = Sin(angle);
            vertices[i + 1] = new Vector3(x, y, 0) * 0.5f;
        }

        var triangles = new int[sides * 3];

        for (int i = 0; i < sides; i++)
        {
            int j = i == sides - 1 ? 0 : i + 1;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = j + 1;
        }

        var circle = new Mesh()
        {
            vertices = vertices,
            triangles = triangles,
            normals = Enumerable.Repeat(-Vector3.up, sides + 1).ToArray(),
            subMeshCount = 2
        };

        var perimeter = Enumerable.Range(1, sides + 1).ToArray();
        perimeter[sides] = 1;

        circle.SetIndices(perimeter, MeshTopology.LineStrip, 1);
        return circle;
    }

    // other

    public static byte[] LoadBytes(string file)
    {
        string path = Path.Combine(Application.streamingAssetsPath, file);
        return File.ReadAllBytes(path);
    }

    // MonoBehaviour

    void LateUpdate()
    {
        _meshPoolIndex = 0;
        _matrix = _reset;
        _z = 0;

        if (_texture != null)
            _texture.Apply();

        StartCoroutine(EndFrame());
    }

    void OnGUI()
    {
        foreach (var text in _texts)
            GUI.Label(new Rect(text.Item2.x, text.Item2.y - Height, Width, Height), text.Item1, _style);

        if (_texture != null)
            GUI.DrawTexture(new Rect(0, 0, Width, Height), _texture);
    }

    IEnumerator EndFrame()
    {
        yield return new WaitForEndOfFrame();
        _texts.Clear();
    }
}
