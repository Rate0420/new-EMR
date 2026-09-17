using UnityEngine;
using System.Collections.Generic;

public class SequencePlayer : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    public string folderName = "Liselotte_CutinFrames";
    [SerializeField] private int fps = 60;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private Material cutinShaderMaterial;

    // キャッシュ：一度読んだフォルダは再読み込みしない
    private Dictionary<string, Texture2D[]> frameCache = new Dictionary<string, Texture2D[]>();

    private Texture2D[] frames;
    private int currentFrame = 0;
    private float timer = 0f;
    private bool isPlaying = false;

    void Start()
    {
        if (cutinShaderMaterial != null)
            targetRenderer.material = cutinShaderMaterial;

        // 起動時に使いそうなフォルダを全部先読みしておく
        PreloadAll();

        if (playOnStart) Play(folderName);
    }

    // 使う予定のパスを起動時にまとめて読み込む
    void PreloadAll()
    {
        Preload("Liselotte_CutinFrames");
        Preload("ReachFrames");
        Preload("Sayo_CutinFrames");
        Preload("Karu_CutinFrames");
        Preload("Vermilion_CutinFrames");
        Preload("Lilith_CutinFrames");
        Preload("Mion_CutinFrames");
        Preload("Hinata_CutinFrames");
        Preload("Mei_CutinFrames");
        // 増えたらここに追加
    }

    void Preload(string path)
    {
        if (frameCache.ContainsKey(path)) return;

        Object[] loaded = Resources.LoadAll(path, typeof(Texture2D));
        Texture2D[] textures = new Texture2D[loaded.Length];
        for (int i = 0; i < loaded.Length; i++)
            textures[i] = (Texture2D)loaded[i];

        frameCache[path] = textures;
        //Debug.Log($"[Preload] {path}：{textures.Length}枚");
    }

    void Update()
    {
        if (!isPlaying || frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / fps)
        {
            timer = 0f;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                    currentFrame = 0;
                else
                {
                    isPlaying = false;
                    currentFrame = frames.Length - 1;
                    return;
                }
            }

            cutinShaderMaterial.mainTexture = frames[currentFrame];
        }
    }

    public float Play(string path)
    {
        // キャッシュから取得、なければその場で読む
        if (!frameCache.ContainsKey(path))
        {
            Debug.LogWarning($"[SequencePlayer] '{path}'がキャッシュにないので読み込みます");
            Preload(path);
        }

        frames = frameCache[path];

        if (frames == null || frames.Length == 0)
        {
            Debug.LogError($"[SequencePlayer] '{path}'のフレームが0枚です");
            return 0f;
        }

        targetRenderer.enabled = true;
        currentFrame = 0;
        timer = 0f;
        ShaderSetting(path);
        isPlaying = true;
        cutinShaderMaterial.mainTexture = frames[0];

        Debug.Log($"[SequencePlayer] Play: {path} ({frames.Length}枚)");
        return frames.Length / (float)fps;
    }

    public void Stop()
    {
        isPlaying = false;
        targetRenderer.enabled = false;
    }

    void ShaderSetting(string path)
    {
        if (cutinShaderMaterial == null) return;

        switch (path)
        {
            case "Sayo_CutinFrames":
            case "Liselotte_CutinFrames":
            case "Vermilion_CutinFrames":
                cutinShaderMaterial.SetColor("_KeyColor", new Color(1f, 0f, 1f));
                cutinShaderMaterial.SetFloat("_HueThreshold", 0.02f);
                cutinShaderMaterial.SetFloat("_SatThreshold", 1f);
                break;
            case "Lilith_CutinFrames":
            case "Mion_CutinFrames":
            case "Hinata_CutinFrames":
            case "Mei_CutinFrames":
                cutinShaderMaterial.SetColor("_KeyColor", new Color(0, 1f, 1f));
                cutinShaderMaterial.SetFloat("_HueThreshold", 0.02f);
                cutinShaderMaterial.SetFloat("_SatThreshold", 1f);
                break;
            case "Karu_CutinFrames":
                cutinShaderMaterial.SetColor("_KeyColor", new Color(0.5f, 0f, 1f));
                cutinShaderMaterial.SetFloat("_HueThreshold", 0.003f);
                cutinShaderMaterial.SetFloat("_SatThreshold", 1f);
                cutinShaderMaterial.SetFloat("_Softness", 0f);
                break;
            case "ReachFrames":
                cutinShaderMaterial.SetColor("_KeyColor", new Color(1f, 0f, 1f));
                cutinShaderMaterial.SetFloat("_HueThreshold", 0.07f);
                cutinShaderMaterial.SetFloat("_SatThreshold", 1f);
                break;
        }
    }
}