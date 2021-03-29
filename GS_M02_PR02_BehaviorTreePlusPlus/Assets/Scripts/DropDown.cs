using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDown : MonoBehaviour
{
    public Toggle closed;
    public Toggle locked;
    public Text text;

    // Update is called once per frame
    void Update()
    {
        if (closed.isOn && locked.isOn)
        {
            this.transform.GetChild(0).GetComponent<Text>().enabled = true;
            this.transform.GetChild(1).GetComponent<Image>().enabled = true;
            GetComponent<Image>().enabled = true;
            text.gameObject.SetActive(false);
        }
        else
        {
            this.transform.GetChild(0).GetComponent<Text>().enabled = false;
            this.transform.GetChild(1).GetComponent<Image>().enabled = false;
            GetComponent<Image>().enabled = false;
            text.gameObject.SetActive(true);
        }
    }
}
