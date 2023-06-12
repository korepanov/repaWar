using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WoodcutterClass : MonoBehaviour
{
    public GameObject WoodcutterPattern;
    public GameObject HighlightPattern;

    private int WoodCutterX;
    private int WoodCutterY;

    List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    // Start is called before the first frame update
    
    public void Setup(int x, int y){
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
