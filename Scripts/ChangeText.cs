using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeText : MonoBehaviour
{
    public Text myText;
    public GameObject changeMyText;
    
    public void TextChange()
    {
        if(GetComponent<UIStuff>().POPoints == 12)
        {
            myText.text = "Teal team Won!";
            changeMyText.GetComponent<Text>().text = "Teal team Won!";
        }

    }
}
