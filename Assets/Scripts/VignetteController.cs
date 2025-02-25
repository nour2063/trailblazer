using UnityEngine;
using System.Collections;

public class VignetteController : MonoBehaviour
{
    private Renderer _rend;
    private MaterialPropertyBlock _mbp;

    private static readonly int Color0ID = Shader.PropertyToID("_Color0");
    private static readonly int Color1ID = Shader.PropertyToID("_Color1");
    private static readonly int Color2ID = Shader.PropertyToID("_Color2");
    private static readonly int Gradient0ID = Shader.PropertyToID("_GradientPosition0");
    private static readonly int Gradient1ID = Shader.PropertyToID("_GradientPosition1");
    private static readonly int Gradient2ID = Shader.PropertyToID("_GradientPosition2");

    void Start()
    {
        _rend = GetComponent<Renderer>();
        _mbp = new MaterialPropertyBlock();
        _rend.GetPropertyBlock(_mbp);
    }

    public void SetColor(Color newColor, float duration)
    {
        StartCoroutine(FadeColor(newColor, duration));
    }

    private IEnumerator FadeColor(Color targetColor, float duration)
    {
        _rend.GetPropertyBlock(_mbp);

        Color startColor = _mbp.GetColor(Color0ID);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            Color lerpedColor = Color.Lerp(startColor, targetColor, time / duration);
            _mbp.SetColor(Color0ID, lerpedColor);
            _mbp.SetColor(Color1ID, lerpedColor);
            _mbp.SetColor(Color2ID, lerpedColor);
            _rend.SetPropertyBlock(_mbp);
            yield return null;
        }

        // Ensure final color is set
        _mbp.SetColor(Color0ID, targetColor);
        _mbp.SetColor(Color1ID, targetColor);
        _mbp.SetColor(Color2ID, targetColor);
        _rend.SetPropertyBlock(_mbp);
    }

    public void SetIntensity(float newIntensity, float duration)
    {
        StartCoroutine(FadeIntensity(newIntensity, duration));
    }

    private IEnumerator FadeIntensity(float targetIntensity, float duration)
    {
        _rend.GetPropertyBlock(_mbp);
        float startZ = _mbp.GetVector(Gradient0ID).z;

        Vector4 gradient0 = new Vector4(0.15f, 0.5f, startZ, 1f);
        Vector4 gradient1 = new Vector4(0.85f, 0.5f, startZ, 1f);
        Vector4 gradient2 = new Vector4(0.5f, 0.5f, startZ, 1f);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float lerpedZ = Mathf.Lerp(startZ, targetIntensity, time / duration);

            _mbp.SetVector(Gradient0ID, new Vector4(gradient0.x, gradient0.y, lerpedZ, gradient0.w));
            _mbp.SetVector(Gradient1ID, new Vector4(gradient1.x, gradient1.y, lerpedZ, gradient1.w));
            _mbp.SetVector(Gradient2ID, new Vector4(gradient2.x, gradient2.y, lerpedZ, gradient2.w));

            _rend.SetPropertyBlock(_mbp);
            yield return null;
        }

        _mbp.SetVector(Gradient0ID, new Vector4(gradient0.x, gradient0.y, targetIntensity, gradient0.w));
        _mbp.SetVector(Gradient1ID, new Vector4(gradient1.x, gradient1.y, targetIntensity, gradient1.w));
        _mbp.SetVector(Gradient2ID, new Vector4(gradient2.x, gradient2.y, targetIntensity, gradient2.w));
        _rend.SetPropertyBlock(_mbp);
    }
}
