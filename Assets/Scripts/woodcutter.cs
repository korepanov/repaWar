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
    public GameObject peasantLarge;
    public GameObject mainBase;
    public GameObject mainBaseMap;
    public GameObject mainBaseToBuild;
    public string ObjName; 
    private GameObject skin;

    private float WoodCutterX;
    private float WoodCutterY;

    public enum Buildings{
        undefined,
        mainBase,
        barracks 
    }

    public Buildings selectedBuilding = Buildings.undefined; 

    List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    // Start is called before the first frame update
    
    public void Setup(float x, float y){
    	WoodCutterX = x;
	    WoodCutterY = y;
    }	

    public GameObject GetSkin(){
        return skin;
    }

    public void SetSkin(GameObject pattern){
        skin = pattern; 
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
