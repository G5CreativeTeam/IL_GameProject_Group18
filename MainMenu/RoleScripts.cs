using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class RoleScripts : MonoBehaviour
{
    public string personName;
    public string role;
    private TextMeshProUGUI textMeshPro;
    private void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = $"{role} {personName}";
    }
}
