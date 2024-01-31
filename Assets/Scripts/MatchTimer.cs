using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
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
        timerText.text = "<mspace=0.75em>" + ts.ToString("mm\\:ss\\:ff");
    }
}
