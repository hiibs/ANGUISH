using UnityEngine;
using UnityEngine.UI;

public class FramerateText : MonoBehaviour
{
    private int fps;
    private Text text;

    private void Start() {
        text = GetComponent<Text>();
    }

    private void Update() {
        fps = (int)(1f / Time.smoothDeltaTime);
    }

    private void OnGUI() {
        text.text = fps.ToString();
    }
}
