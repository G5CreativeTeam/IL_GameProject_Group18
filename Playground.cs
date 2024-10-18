using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Playground : MonoBehaviour
{
    
    private int speed = 1;
    private string messageTextView = "This space for rent";
    private const int PlayerScore = 100;
    
    public string firstName ;
    public string lastName;
    private TextMeshProUGUI textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = $"Hello {firstName} {lastName}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
