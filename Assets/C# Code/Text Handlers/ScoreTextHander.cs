using UnityEngine;
using UnityEngine.UI;

public class ScoreTextHander : MonoBehaviour
{
    private Text _text;

    public void Start()
    {
        _text = GetComponent<Text>();
        if (!_text)
        {
            Debug.Log("No text available!");
        }
    }

    public void Update()
    {
        _text.text = "Score : " + (int)Globals.GameScore;
    }


}
