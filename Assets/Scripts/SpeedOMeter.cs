using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedOMeter : MonoBehaviour {

    public PodRacer pod;

    private Text t;
	// Use this for initialization
	void Start () {
        t = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        t.text = pod.currentSpeed.ToString() + " m/s";
	}
}
