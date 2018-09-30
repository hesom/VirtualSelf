using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour {

	public List<Transform> bodyParts = new List<Transform>();

	public float speed = 3.5f;
	public float currentRotation = 0.0f;
	public float rotationSensitivity = 50.0f;

	public Transform bodyObject;

	[Range(0.0f, 1.0f)]
	public float smoothTime = 0.5f;

	private Vector3 pointInWorld;
	private Vector3 mousePosition;
	public float radius = 3.0f;
	private Vector3 direction;

	public Transform laserDecal;

	private Vector3 startPos;
	private Quaternion startRot;

	public bool useLeap = true;
	public Transform palmPosition;
    public bool useMainCameraAsAnchor;
    public Transform anchorPoint;

	public Transform orbPrefab;

	private bool indexExtended = false;
    private Transform mainCamera;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		startRot = transform.rotation;
        mainCamera = Camera.main.transform;

		var spawnLocations = GameObject.FindGameObjectsWithTag("OrbSpawn");
		foreach(var spawn in spawnLocations){
			spawn.GetComponent<Renderer>().enabled = false;
		}
		SpawnNewOrb();
	}
	
	// Update is called once per frame
	void Update () {

		if(!useLeap && indexExtended){
			MouseRotationSnake();
		}else{
			IndexRotationSnake();
		}

		//InputRotation();
		
	}

	void MouseRotationSnake()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		CastRay(ray);
	}

	void IndexRotationSnake()
	{
        if (useMainCameraAsAnchor)
        {
            anchorPoint = mainCamera;
        }
        Vector3 shoulderAnchor = anchorPoint.position - 0.1f * Vector3.down + 0.2f*Vector3.right;
		Ray ray = new Ray(shoulderAnchor, palmPosition.position - shoulderAnchor);
		CastRay(ray);
	}

	void CastRay(Ray ray)
	{
		RaycastHit hit;
		int layerMask = 1 << LayerMask.NameToLayer("BackGround");
		Physics.Raycast(ray, out hit, 1000.0f, layerMask);
		laserDecal.position = hit.point + 0.001f*hit.normal;
		//laserDecal.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);

		mousePosition = new Vector3(hit.point.x, hit.point.y, 0);

		direction = Vector3.Slerp(direction, mousePosition - transform.position, Time.deltaTime * 4.0f);
		direction.z = 0;

		pointInWorld = direction.normalized * radius + transform.position;

		transform.LookAt(pointInWorld);
	}

	void InputRotation()
	{
		if(Input.GetKey(KeyCode.A)){
			currentRotation += rotationSensitivity * Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.D)){
			currentRotation -= rotationSensitivity * Time.deltaTime;
		}
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		MoveForward();
		Rotation();

		if(!useLeap){
			CameraFollow();
		}
	}

	void MoveForward()
	{
		transform.position += transform.forward * speed * Time.deltaTime;
	}

	void Rotation()
	{
		transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation));
	}

	void CameraFollow()
	{
		Transform camera = Camera.main.transform;
		Vector3 cameraVelocity = Vector3.zero;
		camera.transform.position = Vector3.SmoothDamp(camera.position,
			new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, camera.position.z), ref cameraVelocity, smoothTime);
	}

	/// <summary>
	/// OnCollisionEnter is called when this collider/rigidbody has begun
	/// touching another rigidbody/collider.
	/// </summary>
	/// <param name="other">The Collision data associated with this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		if(other.transform.tag == "Orb"){
			other.gameObject.SetActive(false);
			Destroy(other.gameObject);
			SpawnNewOrb();

			if(bodyParts.Count == 0){
				Vector3 currentPos = transform.position;
				Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity);
				bodyParts.Add(newBodyPart);
			}else{
				Vector3 currentPos = bodyParts[bodyParts.Count-1].position;
				Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity);
				bodyParts.Add(newBodyPart);
			}
		}

		if(other.transform.tag == "SnakeBody" && other.transform != bodyParts[0]){
			Vector3 collisionDirection = transform.position - other.transform.position;
			if(Vector3.Dot(collisionDirection, transform.forward) < 0.0f){
				DestroySnake();
			}
		}

		if(other.transform.tag == "SawBlade"){
			DestroySnake();
		}
	}

	/// <summary>
	/// Spawn new orb at random location
	/// </summary>
	void SpawnNewOrb()
	{
		var spawnLocations = GameObject.FindGameObjectsWithTag("OrbSpawn");
		var length = spawnLocations.Length;

		int spawnIndex = Random.Range(0, length-1);

		Instantiate(orbPrefab, spawnLocations[spawnIndex].transform.position, spawnLocations[spawnIndex].transform.rotation);
	}

	/// <summary>
	/// Destroy every part of the snake and respawn at start location
	/// </summary>
	void DestroySnake()
	{
		for(int i = 0; i < bodyParts.Count; i++){
				Destroy(bodyParts[i].gameObject);
			}
		bodyParts.Clear();
		transform.position = startPos;
		transform.rotation = startRot;
	}

	public void SetExtended()
	{
		indexExtended = true;
	}

	public void UnsetExtended()
	{
		indexExtended = false;
	}
}
