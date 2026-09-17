using System.Collections;
using UnityEngine;

public class SkinColorChanger : MonoBehaviour
{
    public Color color1;
    public Color color2;
    public Color originColor1;
    public Color originColor2;

    public Material targetMaterial;

    private void Start()
    {
        targetMaterial.SetColor("_Color", originColor1);
        targetMaterial.SetColor("_ShadeColor", originColor2);
    }

    public void StartColorChange(float t)
    {
        // t•b‚©‚¯‚ÄtargetMaterial‚Ìlitcolor‚Æshadecolor‚ðcolor1‚Æcolor2‚É•Ï‰»‚³‚¹‚é
        StartCoroutine(ChangeColorCoroutine(t,color1,color2));
    }

    public void StartColorChangeReverse(float t)
    {
        // t•b‚©‚¯‚ÄtargetMaterial‚Ìlitcolor‚Æshadecolor‚ðoriginColor1‚ÆoriginColor2‚É•Ï‰»‚³‚¹‚é
        StartCoroutine(ChangeColorCoroutine(t,originColor1,originColor2));
    }

    public void ResetColor()
    {
        targetMaterial.SetColor("_Color", originColor1);
        targetMaterial.SetColor("_ShadeColor", originColor2);
    }

    IEnumerator ChangeColorCoroutine(float duration,Color colorA,Color colorB)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsed / duration);
            targetMaterial.SetColor("_Color", Color.Lerp(targetMaterial.GetColor("_Color"), colorA, lerpFactor));
            targetMaterial.SetColor("_ShadeColor", Color.Lerp(targetMaterial.GetColor("_ShadeColor"), colorB, lerpFactor));

            yield return null;
        }
        // ÅŒã‚ÉŠmŽÀ‚É–Ú“I‚ÌF‚É‚·‚é
        targetMaterial.SetColor("_Color", colorA);
        targetMaterial.SetColor("_ShadeColor", colorB);
    }

    private void OnApplicationQuit()
    {
        ResetColor();
    }
}
