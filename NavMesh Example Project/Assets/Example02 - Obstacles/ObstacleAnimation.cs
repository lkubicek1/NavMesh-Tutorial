using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAnimation : MonoBehaviour {

	public float speed = .2f;
	public float strength = 9f;

	private float randomOffset;
    private Vector3 initialPosition;

    // Use this for initialization
    void Start () {
		randomOffset = Random.Range(0f, 2f);
        initialPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;

        // Calculate the sine wave offset
        float offset = Mathf.Sin(Time.time * speed + randomOffset) * strength;

        // Update the wall's position
        transform.position = initialPosition + Vector3.right * offset;
	}
}
