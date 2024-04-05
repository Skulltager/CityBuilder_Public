using System.IO;
using UnityEditor;
using UnityEngine;

public class TextureAtlasGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Texture Atlas")]
    public static void ShowWindow()
    {
        TextureAtlasGenerator textureAtlasWindow = GetWindow<TextureAtlasGenerator>();
        textureAtlasWindow.Show();
    }

    private const int THREADS = 16;
    private const int TEXTURE_SIZE = 256;
    private string EDITOR_KEY_WIDTH => projectName + "TEXTURE ATLAS WIDTH";

    private string EDITOR_KEY_TEXTURE_FILE_NAME => projectName + "TEXTURE ATLAS FILE NAME";
    private string EDITOR_KEY_TEXTURE_COUNT => projectName + "TEXTURE ATLAS TEXTURE COUNT";
    private string EDITOR_KEY_TEXTURE => projectName + "TEXTURE ATLAS TEXTURE {0}";

    private string EDITOR_KEY_NORMAL_FILE_NAME => projectName + "NORMAL ATLAS FILE NAME";
    private string EDITOR_KEY_NORMAL_COUNT => projectName + "NORMAL ATLAS TEXTURE COUNT";
    private string EDITOR_KEY_NORMAL => projectName + "TEXTURE ATLAS NORMAL {0}";

    private string projectName;

    [SerializeField] private string fileNameTextureAtlas;
    [SerializeField] private string fileNameNormalsAtlas;
    [SerializeField] private Texture[] texturesAtlas;
    [SerializeField] private Texture[] normalsAtlas;
    [SerializeField] private int width;
    [SerializeField] private ComputeShader textureResizerShader;

    private void Awake()
    {
        projectName = PlayerSettings.productName;
        fileNameTextureAtlas = EditorPrefs.GetString(EDITOR_KEY_TEXTURE_FILE_NAME, "");
        fileNameNormalsAtlas = EditorPrefs.GetString(EDITOR_KEY_NORMAL_FILE_NAME, "");
        width = EditorPrefs.GetInt(EDITOR_KEY_WIDTH, 1);

        int textureCount = EditorPrefs.GetInt(EDITOR_KEY_TEXTURE_COUNT, 0);
        texturesAtlas = new Texture[textureCount];
        normalsAtlas = new Texture[textureCount];
        for (int i = 0; i < textureCount; i++)
        {
            string key = string.Format(EDITOR_KEY_TEXTURE, i);
            string texturePath = EditorPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(texturePath))
                continue;

            texturesAtlas[i] = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
        }

        for (int i = 0; i < textureCount; i++)
        {
            string key = string.Format(EDITOR_KEY_NORMAL, i);
            string texturePath = EditorPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(texturePath))
                continue;

            normalsAtlas[i] = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
        }
    }

    private void OnGUI()
    {
        EditorWindow target = this;
        SerializedObject serializedObject = new SerializedObject(target);

        EditorGUILayout.BeginVertical();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fileNameTextureAtlas"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fileNameNormalsAtlas"), false);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("width"), false);
        width = Mathf.Max(1, width);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("texturesAtlas"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("normalsAtlas"), true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("textureResizerShader"), true);

        if (GUILayout.Button("Change Directory Texture Atlas"))
        {
            string directory = string.IsNullOrEmpty(fileNameTextureAtlas) ? "" : Path.GetDirectoryName(fileNameTextureAtlas);
            string file = string.IsNullOrEmpty(fileNameTextureAtlas) ? "" : Path.GetFileName(fileNameTextureAtlas);
            string result = EditorUtility.SaveFilePanelInProject("what", file, "jpg", "message", directory);
            if (!string.IsNullOrEmpty(result))
                fileNameTextureAtlas = result;
        }

        if (GUILayout.Button("Change Directory Normal Atlas"))
        {
            string directory = string.IsNullOrEmpty(fileNameNormalsAtlas) ? "" : Path.GetDirectoryName(fileNameNormalsAtlas);
            string file = string.IsNullOrEmpty(fileNameNormalsAtlas) ? "" : Path.GetFileName(fileNameNormalsAtlas);
            string result = EditorUtility.SaveFilePanelInProject("what", file, "jpg", "message", directory);
            if (!string.IsNullOrEmpty(result))
                fileNameNormalsAtlas = result;
        }

        GUI.enabled = !string.IsNullOrEmpty(fileNameTextureAtlas) && !string.IsNullOrEmpty(fileNameNormalsAtlas);
        if (GUILayout.Button("Generate Atlas"))
        {
            GenerateTextureAtlas();
            GenerateNormalsAtlas();
        }
        GUI.enabled = true;

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void GenerateTextureAtlas()
    {
        int resizeTextureKernel = textureResizerShader.FindKernel("ResizeTexture");
        int height = Mathf.CeilToInt((float)texturesAtlas.Length / width);
        RenderTexture renderTexture = new RenderTexture(width * TEXTURE_SIZE, height * TEXTURE_SIZE, 0, RenderTextureFormat.ARGB32, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.filterMode = FilterMode.Point;
        textureResizerShader.SetInt("targetWidth", TEXTURE_SIZE);
        textureResizerShader.SetInt("targetHeight", TEXTURE_SIZE);

        for (int i = 0; i < texturesAtlas.Length; i++)
        {
            if (texturesAtlas[i] == null)
                continue;

            int xIndex = i % width;
            int yIndex = (height - 1) - (i - xIndex) / width;
            Texture texture = texturesAtlas[i];
            RenderTexture resizedTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0, RenderTextureFormat.ARGB32, 0);
            resizedTexture.enableRandomWrite = true;
            resizedTexture.wrapMode = TextureWrapMode.Clamp;
            resizedTexture.filterMode = FilterMode.Point;
            float widthRatio = (float)texture.width / TEXTURE_SIZE;
            float heightRatio = (float)texture.height / TEXTURE_SIZE;

            textureResizerShader.SetInt("sourceWidth", texture.width);
            textureResizerShader.SetInt("sourceHeight", texture.height);
            textureResizerShader.SetFloat("widthRatio", widthRatio);
            textureResizerShader.SetFloat("heightRatio", heightRatio);
            textureResizerShader.SetTexture(resizeTextureKernel, "targetTexture", resizedTexture);
            textureResizerShader.SetTexture(resizeTextureKernel, "sourceTexture", texture);
            textureResizerShader.Dispatch(resizeTextureKernel, Mathf.CeilToInt((float)TEXTURE_SIZE / THREADS), Mathf.CeilToInt((float)TEXTURE_SIZE / THREADS), 1);

            Graphics.CopyTexture(resizedTexture, 0, 0, 0, 0, TEXTURE_SIZE, TEXTURE_SIZE, renderTexture, 0, 0, xIndex * TEXTURE_SIZE, yIndex * TEXTURE_SIZE);
        }

        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(fileNameTextureAtlas, bytes);
        AssetDatabase.ImportAsset(fileNameTextureAtlas);

        TextureImporter textureImporter = TextureImporter.GetAtPath(fileNameTextureAtlas) as TextureImporter;

        TextureImporterSettings test = new TextureImporterSettings();
        test.filterMode = FilterMode.Point;
        test.wrapMode = TextureWrapMode.Repeat;
        test.textureType = TextureImporterType.Default;
        test.textureShape = TextureImporterShape.Texture2DArray;
        test.mipmapEnabled = true;
        test.readable = true;
        test.flipbookColumns = width;
        test.flipbookRows = height;
        test.alphaSource = TextureImporterAlphaSource.FromInput;

        textureImporter.SetTextureSettings(test);
        textureImporter.SaveAndReimport();
    }

    private void GenerateNormalsAtlas()
    {
        int resizeTextureKernel = textureResizerShader.FindKernel("ResizeNormal");
        int height = Mathf.CeilToInt((float)normalsAtlas.Length / width);
        RenderTexture renderTexture = new RenderTexture(width * TEXTURE_SIZE, height * TEXTURE_SIZE, 0, RenderTextureFormat.ARGB32, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.filterMode = FilterMode.Point;
        textureResizerShader.SetInt("targetWidth", TEXTURE_SIZE);
        textureResizerShader.SetInt("targetHeight", TEXTURE_SIZE);

        for (int i = 0; i < normalsAtlas.Length; i++)
        {
            if (normalsAtlas[i] == null)
                continue;

            int xIndex = i % width;
            int yIndex = (height - 1) - (i - xIndex) / width;
            Texture texture = normalsAtlas[i];
            RenderTexture resizedTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0, RenderTextureFormat.ARGB32, 0);
            resizedTexture.enableRandomWrite = true;
            resizedTexture.wrapMode = TextureWrapMode.Clamp;
            resizedTexture.filterMode = FilterMode.Point;
            float widthRatio = (float)texture.width / TEXTURE_SIZE;
            float heightRatio = (float)texture.height / TEXTURE_SIZE;

            textureResizerShader.SetInt("sourceWidth", texture.width);
            textureResizerShader.SetInt("sourceHeight", texture.height);
            textureResizerShader.SetFloat("widthRatio", widthRatio);
            textureResizerShader.SetFloat("heightRatio", heightRatio);
            textureResizerShader.SetTexture(resizeTextureKernel, "targetTexture", resizedTexture);
            textureResizerShader.SetTexture(resizeTextureKernel, "sourceTexture", texture);
            textureResizerShader.Dispatch(resizeTextureKernel, Mathf.CeilToInt((float)TEXTURE_SIZE / THREADS), Mathf.CeilToInt((float)TEXTURE_SIZE / THREADS), 1);

            Graphics.CopyTexture(resizedTexture, 0, 0, 0, 0, TEXTURE_SIZE, TEXTURE_SIZE, renderTexture, 0, 0, xIndex * TEXTURE_SIZE, yIndex * TEXTURE_SIZE);
        }

        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(fileNameNormalsAtlas, bytes);
        AssetDatabase.ImportAsset(fileNameNormalsAtlas);

        TextureImporter textureImporter = TextureImporter.GetAtPath(fileNameNormalsAtlas) as TextureImporter;

        TextureImporterSettings test = new TextureImporterSettings();
        test.filterMode = FilterMode.Point;
        test.wrapMode = TextureWrapMode.Repeat;
        test.textureType = TextureImporterType.NormalMap;
        test.textureShape = TextureImporterShape.Texture2DArray;
        test.mipmapEnabled = true;
        test.readable = true;
        test.flipbookColumns = width;
        test.flipbookRows = height;
        test.alphaSource = TextureImporterAlphaSource.FromInput;

        textureImporter.SetTextureSettings(test);
        textureImporter.SaveAndReimport();
    }

    private void OnDestroy()
    {
        EditorPrefs.SetString(EDITOR_KEY_TEXTURE_FILE_NAME, fileNameTextureAtlas);
        EditorPrefs.SetString(EDITOR_KEY_NORMAL_FILE_NAME, fileNameNormalsAtlas);
        EditorPrefs.SetInt(EDITOR_KEY_WIDTH, width);
        EditorPrefs.SetInt(EDITOR_KEY_TEXTURE_COUNT, texturesAtlas.Length);
        EditorPrefs.SetInt(EDITOR_KEY_NORMAL_COUNT, normalsAtlas.Length);

        for (int i = 0; i < texturesAtlas.Length; i++)
        {
            string key = string.Format(EDITOR_KEY_TEXTURE, i);
            string texturePath = texturesAtlas[i] == null ? "" : AssetDatabase.GetAssetPath(texturesAtlas[i]);
            EditorPrefs.SetString(key, texturePath);
        }

        for (int i = 0; i < normalsAtlas.Length; i++)
        {
            string key = string.Format(EDITOR_KEY_NORMAL, i);
            string texturePath = normalsAtlas[i] == null ? "" : AssetDatabase.GetAssetPath(normalsAtlas[i]);
            EditorPrefs.SetString(key, texturePath);
        }
    }
}
