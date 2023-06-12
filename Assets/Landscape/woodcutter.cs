using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WoodcutterClass : MonoBehaviour
{
    public GameObject WoodcutterPattern;
    public GameObject WoodcutterRight;
    public GameObject WoodcutterLeft;
    public GameObject WoodcutterDown;
    public GameObject WoodcutterUp;
    public GameObject WoodcutterUpLeft;
    public GameObject WoodcutterUpRight;
    public GameObject WoodcutterDownLeft;
    public GameObject WoodcutterDownRight;
    public GameObject HighlightPattern;
    public string ObjName; 

    private float WoodCutterX;
    private float WoodCutterY;

    List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    // Start is called before the first frame update
    
    public void Setup(float x, float y){
    	WoodCutterX = x;
	    WoodCutterY = y;
    }	

    public Vector3 GetCoord(){
    	return new Vector3(WoodCutterX, WoodCutterY, 0);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
