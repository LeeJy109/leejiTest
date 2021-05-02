using UnityEngine;

using Cinemachine;

public class CameraMoveByInput : MonoBehaviour
{
	public CinemachineFreeLook cinemachineFreeLook = null;

	// Rotate
	public float touchSensitivity = 1f;
	public float xRotSpeed = 100f;
	public float yRotSpeed = 1f;

	// Zoom
	public float touchZoomSpeed = 1f;
	public float minMiddleHeight = 0.1f;
	public float maxMiddleHeight = 10f;

	private bool isZoom = false;
	private Vector2[] startZoomPos;


	private float[] ratioRadius;
	private float ratioTopHeight;


	private void Start()
	{
		if (cinemachineFreeLook == null)
			cinemachineFreeLook = this.GetComponent<CinemachineFreeLook>();

		InitializeRatio();
	}

	private void InitializeRatio()
	{
		var orbits = cinemachineFreeLook.m_Orbits;

		ratioRadius = new float[3];

		// Set by MiddleRig.Height
		ratioTopHeight = orbits[0].m_Height / orbits[1].m_Height;

		for (int i = 0; i < 3; i++) // Top, Middle, Bottom
		{
			ratioRadius[i] = orbits[i].m_Radius / orbits[1].m_Height;
		}
	}

	private void Update()
	{
		if (cinemachineFreeLook == null) return;

		if (Input.touchCount == 1)  // Drag Rotate
		{			
			Touch touch = Input.GetTouch(0);
			
			if (Mathf.Abs(touch.deltaPosition.x) > touchSensitivity)
			{
				cinemachineFreeLook.m_XAxis.Value += touch.deltaPosition.x * xRotSpeed;
			}

			if (Mathf.Abs(touch.deltaPosition.y) > touchSensitivity)
			{
				cinemachineFreeLook.m_YAxis.Value += touch.deltaPosition.y * yRotSpeed;
			}
		}
		else if (Input.touchCount > 1)	// Zoom
		{
			Vector2[] newPosition = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };

			if(isZoom == false)
			{
				startZoomPos = newPosition;
				isZoom = true;
			}
			else
			{
				float newDist = Vector2.Distance(newPosition[0], newPosition[1]);
				float oldDist = Vector2.Distance(startZoomPos[0], startZoomPos[1]);
				float offset = newDist - oldDist;

				cinemachineFreeLook.m_Orbits[1].m_Height -= offset + touchZoomSpeed;

				SetFreeLookRigAndRadius();

				startZoomPos = newPosition;
			}
		}
	}



	private void SetFreeLookRigAndRadius()
	{
		cinemachineFreeLook.m_Orbits[1].m_Height = Mathf.Clamp(cinemachineFreeLook.m_Orbits[1].m_Height, minMiddleHeight, maxMiddleHeight);

		// MiddleRig
		var middleHeight = cinemachineFreeLook.m_Orbits[1].m_Height;

		// Set Top Rig Height by Middle Rig Height
		cinemachineFreeLook.m_Orbits[0].m_Height = middleHeight * ratioTopHeight;

		// Set Radius by Middle Rig Height
		for (int i = 0; i < 3; i++)
		{
			cinemachineFreeLook.m_Orbits[i].m_Radius = middleHeight * ratioRadius[i];
		}
	}
}
