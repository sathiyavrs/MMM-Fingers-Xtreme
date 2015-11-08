using UnityEngine;
using UnityEngine.UI;


public class GameOverTextHander : MonoBehaviour
{
    private Text _text;

    public void Start()
    {
        _text = GetComponent<Text>();
        if (_text == null)
            Debug.LogError("Text is null");
    }

    public void Update()
    {
        if (Globals.GameOver)
            _text.text = "Game Over! Press R to Restart!";
        else
            _text.text = "";

    }



}
