using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText1;
    public TextMeshProUGUI timerText2;
    public TextMeshProUGUI timerText3;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        UpdateTimer();
    }


    private void UpdateTimer()
    {
        var ts = System.TimeSpan.FromSeconds(elapsedTime);
        timerText1.text = "<mspace=0.8em>" + ts.ToString("mm");
        timerText2.text = "<mspace=0.8em>" + ts.ToString("ss");
        timerText3.text = "<mspace=0.8em>" + ts.ToString("ff");
    }
}
