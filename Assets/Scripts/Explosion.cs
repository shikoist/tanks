using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
    public Transform cameraTransform;

    private Transform t;

	// Use this for initialization
	void Start () {
        t = transform;
	}
	
	// Update is called once per frame
	void Update () {
        t.LookAt(cameraTransform);
	}
}
