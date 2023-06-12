using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class cameraMove : MonoBehaviour
{
    [SerializeField] private CameraFollow cameraFollow;

    private Vector3 cameraFollowPosition;
    // Start is called before the first frame update

    private float zoom = 10f; 
    private int width = 160;
    private int height = 160;

    private void Start()
    {
	cameraFollowPosition.x = 40;
	cameraFollowPosition.y = 40; 
        cameraFollow.Setup(() => cameraFollowPosition, () => zoom);
    }

    // Update is called once per frame
    void Update()
    {
		HandleZoom();
		HandleManualMovement();
    }

    private void HandleZoom(){
	float zoomChangeAmount = 10f;
	if (Input.mouseScrollDelta.y > 0){
		zoom -= zoomChangeAmount * Time.deltaTime * 10f;
	}    	
	if (Input.mouseScrollDelta.y < 0){
		zoom += zoomChangeAmount * Time.deltaTime * 10f;
	}

	zoom = Mathf.Clamp(zoom, 1f, 10f);
    }
    private void HandleManualMovement(){
    	float moveAmount = 20f;
		float edgeSize = 30f;
		
		if ((Input.mousePosition.x > Screen.width - edgeSize) && (cameraFollowPosition.x < width)){
			cameraFollowPosition.x += moveAmount * Time.deltaTime;
		}   
		if ((Input.mousePosition.x < edgeSize) && (cameraFollowPosition.x > 0f)){
			cameraFollowPosition.x -= moveAmount * Time.deltaTime;
		} 
		if ((Input.mousePosition.y > Screen.height - edgeSize) && (cameraFollowPosition.y < height)){
			cameraFollowPosition.y += moveAmount * Time.deltaTime;
		} 
		if ((Input.mousePosition.y < edgeSize) && (cameraFollowPosition.y > 0f)){
			cameraFollowPosition.y -= moveAmount * Time.deltaTime;
		} 
	
	
    }
}
