using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        int fps = Mathf.CeilToInt(1.0f / deltaTime);
        string fpsText = $"{fps} FPS";

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;

        Color originalColor = GUI.color;
        GUI.color = new Color(0.01f, 0.01f, 0.01f, 0.99f);
        Rect rect = new Rect(10, 10, 100, 40);
        GUI.Box(rect, "");
        GUI.color = Color.white;

        string statsText = fpsText;
        GUI.Label(new Rect(20, 20, 230, 60), statsText, style);
    }
}