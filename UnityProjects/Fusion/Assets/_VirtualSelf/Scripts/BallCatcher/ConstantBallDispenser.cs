using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantBallDispenser : MonoBehaviour {

	public Transform prefab;
	public int ballsPerBatch = 10;

	private float timeSinceLastDrop = 5.0f;

	void Update() {
		timeSinceLastDrop += Time.deltaTime;

		if(timeSinceLastDrop >= 5.0f){
			timeSinceLastDrop = 0.0f;
			StartCoroutine("SpawnBall");
		}
	}

	IEnumerator SpawnBall(){
		for(int i = 0; i < ballsPerBatch; i++){
			var ball = Instantiate(prefab, transform.position, transform.rotation);
			var velocity = new Vector3(Random.Range(-0.1f, 0.1f),0.0f, Random.Range(-0.1f, 0.1f));
			var rb = ball.GetComponent<Rigidbody>();
			rb.velocity = velocity;
			yield return new WaitForSecondsRealtime(0.1f);
		}
	}
}
