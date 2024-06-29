using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

public class TargetLocation : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;

    // change target location for new behaviors
    void Start(){
        input.onValueChanged.AddListener((position) => {

            // pattern to detect a tuple of two floats
            string pattern = @"\((-?\d+(\,\d*)?)\/(-?\d+(\,\d*)?)\)";
            try {
                Match match = Regex.Match(position, pattern);

                if (match.Success)
                {
                    input.image.color = Color.white;
                    float x = float.Parse(match.Groups[1].Value);
                    float z = float.Parse(match.Groups[3].Value);
                    GameManagement.selectedPos = new Vector3(x,0,z);
                } else {
                    input.image.color = Color.red;
                }
            } catch (Exception) {
                input.image.color = Color.red;
            }
        });
    }
}
