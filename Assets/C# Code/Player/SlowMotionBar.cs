using UnityEngine;

public class SlowMotionBar : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public Color DangerColor = Color.red;

    public void Start()
    {
        if (!Renderer)
            Debug.LogError("Renderer is null!");
    }


    public void Update()
    {
        var max = Globals.SlowMotionTimeData.MaxTime;
        var current = Globals.SlowMotionTimeData.Current;

        var fraction = current / max;
        transform.localScale = new Vector3(transform.localScale.x, fraction, transform.localScale.z);

        SetColor();
    }

    private void SetColor()
    {
        if (Globals.DangerSlowMotion)
        {
            Renderer.color = DangerColor;
        } 
        else
        {
            Renderer.color = Color.white;
        }
    }
}
