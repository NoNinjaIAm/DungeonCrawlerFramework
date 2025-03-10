using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField xSizeInputField;
    public TMP_InputField ySizeInputField;
    public TMP_InputField extraDoorInputField;
    public TMP_InputField obstacleInputField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryStartGame()
    {
        int xValue;
        int yValue;
        float roomProb;
        float obsProb;

        if (int.TryParse(xSizeInputField.text, out xValue))
        {
            Debug.Log("xValue: " + xValue);
        }
        if (int.TryParse(ySizeInputField.text, out yValue))
        {
            Debug.Log("yValue: " + yValue);
        }
        if (float.TryParse(extraDoorInputField.text, out roomProb))
        {
            Debug.Log("roomProb: " + roomProb);
        }
        if (float.TryParse(obstacleInputField.text, out obsProb))
        {
            Debug.Log("obsProb: " + obsProb);
        }


        GlobalOptions.Instance.SetOptions(xValue, yValue, roomProb, obsProb);

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private int LogWarning()
    {
        Debug.Log("Invalid field");
        return -1;
    }
}
