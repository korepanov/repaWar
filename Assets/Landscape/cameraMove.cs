using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class cameraMove : MonoBehaviour
{
    [SerializeField] private CameraFollow cameraFollow; 
	//[SerializeField] private GameObject gameMenu; 

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


	/*private void DestroyMenu(){
		GameObject tile = GameObject.Find("userMenu");

		if (null != tile){
			Destroy(tile);
		}
	}
	private void MakeMenu(){
		GameObject tile = GameObject.Find("userMenu");

		if(null == tile){
			GameObject local_tile_group = new GameObject("userMenu");
			local_tile_group.transform.parent = gameObject.transform;
			local_tile_group.transform.localPosition = new Vector3(0, 0, 0);
			tile = Instantiate(gameMenu, local_tile_group.transform);

			tile.name = "userMenu";
			
			 
			var screenBottomCenter = new Vector3(0, 0, 0);
			tile.transform.localPosition = cameraFollow.myCamera.ScreenToWorldPoint(screenBottomCenter);
		}
	}*/

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
			//DestroyMenu();
		}   
		if ((Input.mousePosition.x < edgeSize) && (cameraFollowPosition.x > 0f)){
			cameraFollowPosition.x -= moveAmount * Time.deltaTime;
			//DestroyMenu();
		} 
		if ((Input.mousePosition.y > Screen.height - edgeSize) && (cameraFollowPosition.y < height)){
			cameraFollowPosition.y += moveAmount * Time.deltaTime;
			//DestroyMenu();
		} 
		if ((Input.mousePosition.y < edgeSize) && (cameraFollowPosition.y > 0f)){
			cameraFollowPosition.y -= moveAmount * Time.deltaTime;
			//DestroyMenu();
		} 
	
		//MakeMenu(); 
    }
}
