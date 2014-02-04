using UnityEngine;
using System.Collections;

public class CameraSetup : MonoBehaviour 
{
	[SerializeField] private Camera cam;
	
	void Start () 
	{
		if(cam != null)
		{
			float orthoHeight = Screen.height / 2f / Constants.PIXEL_PER_UNIT; 
			float orthoWidth = Screen.width / 2f / Constants.PIXEL_PER_UNIT; 
			cam.orthographicSize = orthoHeight;
			cam.transform.position = new Vector3(orthoWidth, orthoHeight, -10);
		}
	}
}
