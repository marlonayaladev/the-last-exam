using UnityEngine;

public class AIPerception : MonoBehaviour
{
	[Range(0f, 10f)] public float viewRange;
	[Range(5f, 180f)] public float viewAngle;
	[Range(0f, 10f)] public float hearingRange;
	[Range(3, 50)] public int numRays;

	Mesh viewCone;
	MeshFilter meshFilter;
	MeshRenderer meshRenderer;

	Vector3[] viewConeVertices;
	Vector2[] viewConeUVs;
	int[] viewConeIndices;

	Color viewConeColor;

	GameObject player;
	Vector3 lastPosition;
	float viewThreshold;
	int viewLayerMask;
	bool targetConfirmed;
	bool targetSuspected;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");

		viewCone = new Mesh();
		viewConeVertices = new Vector3[numRays + 1];
		viewConeUVs = new Vector2[numRays + 1];
		viewConeIndices = new int[(numRays - 1) * 3];

		viewConeVertices[0].Set(0f, 1f, 0f);
		viewConeUVs[0].Set(0f, 1f);

		for (int i = 0; i < numRays - 1; ++i)
		{
			viewConeIndices[(i * 3)] = 0;
			viewConeIndices[(i * 3) + 1] = i + 1;
			viewConeIndices[(i * 3) + 2] = i + 2;
		}

		meshFilter = GetComponent<MeshFilter>();
		if (meshFilter != null)
			meshFilter.sharedMesh = viewCone;

		meshRenderer = GetComponent<MeshRenderer>();

		viewThreshold = Mathf.Cos(Mathf.Deg2Rad * viewAngle * 0.5f);
		viewLayerMask = 1 << LayerMask.NameToLayer("Obstacle");
		targetConfirmed = false;
	}

	void Update()
	{
		UpdateVision();
		UpdateHearing();
		if (meshFilter != null && meshRenderer != null)
		{
			UpdateViewCone();
			UpdateTintColor();
		}
	}

	void UpdateViewCone()
	{
		float currentAngle = transform.rotation.eulerAngles.y;

		float angleStep = viewAngle / (numRays - 1);
		for (int i = 0; i < numRays; ++i)
		{
			float localAngle = (viewAngle * -0.5f) + (i * angleStep);
			float range = viewRange;

			Ray ray = new Ray(transform.position, Quaternion.AngleAxis(localAngle + currentAngle, Vector3.up) * Vector3.forward);

			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, viewRange, viewLayerMask))
			{
				range = hitInfo.distance;
			}

			viewConeVertices[i + 1].Set(Mathf.Sin(Mathf.Deg2Rad * localAngle) * range, 1f, Mathf.Cos(Mathf.Deg2Rad * localAngle) * range);

			float v = 1f;
			if (i == 0 || i == numRays - 1)
			{
				v = 0f;
			}
			viewConeUVs[i + 1].Set(0.99f * (range / viewRange), v);
		}

		viewCone.vertices = viewConeVertices;
		viewCone.uv = viewConeUVs;
		viewCone.triangles = viewConeIndices;
		viewCone.RecalculateBounds();
	}

	void UpdateVision()
	{
		targetConfirmed = false;

		if (player == null) return;

		Vector3 agentToPlayer = player.transform.position - transform.position;

		if (agentToPlayer.magnitude > viewRange) return;

		float angleToPlayer = Vector3.Angle(transform.forward, agentToPlayer);
		if (angleToPlayer > viewAngle * 0.5f) return;

		RaycastHit hitInfo;
		Ray ray = new Ray(transform.position + Vector3.up, agentToPlayer.normalized);
		if (Physics.Raycast(ray, out hitInfo, viewRange, viewLayerMask))
		{
			if (hitInfo.transform.gameObject != player)
				return;
		}

		lastPosition = player.transform.position;
		targetConfirmed = true;
	}

	void UpdateHearing()
	{
		targetSuspected = false;

		if (targetConfirmed)
		{
			targetSuspected = true;
			return;
		}

		if (player == null) return;

		Vector3 agentToPlayer = player.transform.position - transform.position;
		if (agentToPlayer.magnitude > hearingRange) return;

		if (!player.GetComponent<PlayerController>().IsRunning()) return;

		lastPosition = player.transform.position;
		targetSuspected = true;
	}

	void UpdateTintColor()
	{
		Color currentColor = meshRenderer.material.GetColor("_TintColor");
		meshRenderer.material.SetColor("_TintColor", Color.Lerp(currentColor, viewConeColor, Time.deltaTime * 5f));
	}

	public void SetViewConeColor(Color color)
	{
		viewConeColor = color;
	}

	public bool HasConfirmedTarget()
	{
		return targetConfirmed;
	}

	public bool HasSuspectedTarget()
	{
		return targetSuspected;
	}

	public GameObject GetTarget()
	{
		return player;
	}

	public Vector3 GetLastPosition()
	{
		return lastPosition;
	}
}
