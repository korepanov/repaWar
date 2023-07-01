using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 

public class Landscape : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tile_groups;
    public GameObject grass;
    public GameObject tree;
    public WoodcutterClass woodcutter;
    public BuildingClass building; 
 
    int map_width = 160;
    int map_height = 160;
    private bool toMove = false;
    private bool toBuild = false; 

    private float xMove = 0;
    private float yMove = 0;
    float panelMaxY = 200f;
    private float iconSize = 2f;
 
    List<BuildingClass> buildings = new List<BuildingClass>(); 
    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tile_grid = new List<List<GameObject>>();
 
    // recommend 4 to 20
    float magnification = 7.0f;
 
    int x_offset = 0; // <- +>
    int y_offset = 0; // v- +^
 
    void Start()
    {
        CreateTileset();
        CreateTileGroups();
        GenerateMap();
	    woodcutter.Setup(40f, 40f); 
        woodcutter.ObjName = "mainWoodcutter"; 
	    SpawnWoodcutter(woodcutter);
        InitMenu(); 
    }

    void InitMenu(){
         woodcutter.peasantLarge.SetActive(false); 
         woodcutter.mainBase.SetActive(false); 
    }

    void Update(){

        if (Input.GetMouseButtonDown(0)){
            
            Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mainBaseLargePos = woodcutter.mainBase.transform.position;

            if (pos.x - 1 < woodcutter.GetCoord().x && woodcutter.GetCoord().x < pos.x + 1 && pos.y - 1 < woodcutter.GetCoord().y && woodcutter.GetCoord().y < pos.y + 1){
                if (!(Input.mousePosition.y < panelMaxY)){
                    MakeHighlight(woodcutter);     
                }
            }else{
                var outline = woodcutter.mainBase.GetComponent<UnityEngine.UI.Outline>();
                if ((!(Input.mousePosition.y < panelMaxY)) && (null == outline)){
                    DestroyHighlight(woodcutter);
                }
                if (!(Input.mousePosition.y < panelMaxY) && (null != outline)){
                    
                    if (WoodcutterClass.Buildings.undefined != woodcutter.selectedBuilding){
                        Vector3 posToBuild = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                        toMove = true;
                        xMove = posToBuild.x;
                        yMove = posToBuild.y; 
                        toBuild = true; 
                    }
                }
            }

           
            if ((woodcutter.mainBase.activeSelf) && (pos.x - iconSize / 2 < mainBaseLargePos.x && mainBaseLargePos.x < pos.x + 
                    iconSize / 2 && pos.y - iconSize / 2 < mainBaseLargePos.y && mainBaseLargePos.y < pos.y + iconSize / 2)){
                var outline = woodcutter.mainBase.GetComponent<UnityEngine.UI.Outline>();
                if (null == outline){
                    outline = woodcutter.mainBase.AddComponent<UnityEngine.UI.Outline>();
                    outline.useGraphicAlpha = true;
                    outline.effectColor = new Color(0f, 255f, 0f, 128f);
                    outline.effectDistance = new Vector2(5, -5);
                    woodcutter.selectedBuilding = WoodcutterClass.Buildings.mainBase;
                }else{
                    Destroy(outline); 
                    woodcutter.selectedBuilding = WoodcutterClass.Buildings.undefined;
                }
            }
           
        }

        if (Input.GetMouseButtonDown(1) && HasHighlight(woodcutter) && (!(Input.mousePosition.y < panelMaxY))){
            Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            toMove = true;
            xMove = pos.x;
            yMove = pos.y; 
        }

        if (toMove){
            Move(woodcutter, xMove, yMove);
        }

        if ((!toMove) && (toBuild)){
            
            if (null == GameObject.Find(woodcutter.ObjName)){
                Build(woodcutter);
                woodcutter.selectedBuilding = WoodcutterClass.Buildings.undefined; 
                toBuild = false; 
            }
            
            DestroyHighlight(woodcutter);
            DestroyWoodcutter(woodcutter);
        }

    }
 
    void CreateTileset()
    {
        /** Collect and assign ID codes to the tile prefabs, for ease of access.
            Best ordered to match land elevation. **/
 
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, grass);
	    tileset.Add(1, tree);
    }
 
    void CreateTileGroups()
    {
        /** Create empty gameobjects for grouping tiles of the same type, ie
            forest tiles **/
 
        tile_groups = new Dictionary<int, GameObject>();
        foreach(KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = gameObject.transform;
            tile_group.transform.localPosition = new Vector3(0, 0, 0);
            tile_groups.Add(prefab_pair.Key, tile_group);
        }
    }
    

    void GenerateMap()
    {
        /** Generate a 2D grid using the Perlin noise fuction, storing it as
            both raw ID values and tile gameobjects **/
 
        for(int x = 0; x < map_width; x+=1)
        {
            noise_grid.Add(new List<int>());
            tile_grid.Add(new List<GameObject>());
 
            for(int y = 0; y < map_height; y+=1)
            {
                int tile_id = GetIdUsingPerlin(x, y);
                noise_grid[x].Add(tile_id);
		        CreateTile(0, x, y);
                CreateTile(tile_id, x, y);
            }
        }
	
    }
 
    int GetIdUsingPerlin(int x, int y)
    {
        /** Using a grid coordinate input, generate a Perlin noise value to be
            converted into a tile ID code. Rescale the normalised Perlin value
            to the number of tiles available. **/
 	if ((x >=40) && (x <= 50) && (y >=40) && (y <=50)){
		return 0; 
	}
        float raw_perlin = Mathf.PerlinNoise(
            (x - x_offset) / magnification,
            (y - y_offset) / magnification
        );
        float clamp_perlin = Mathf.Clamp01(raw_perlin); 
        float scaled_perlin = clamp_perlin * tileset.Count;
 
        // Replaced 4 with tileset.Count to make adding tiles easier
        if(scaled_perlin == tileset.Count)
        {
            scaled_perlin = (tileset.Count - 1);
        }
        return Mathf.FloorToInt(scaled_perlin);
    }
 
    void CreateTile(int tile_id, int x, int y)
    {
        /** Creates a new tile using the type id code, group it with common
            tiles, set it's position and store the gameobject. **/
 
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tile_groups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);
 
        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);
 
        tile_grid[x].Add(tile);
    }


    public void SpawnWoodcutter(WoodcutterClass woodcutter){

        woodcutter.SetSkin(woodcutter.WoodcutterPattern);

        int x;
        int y; 

        x = (int)woodcutter.GetCoord().x;
        y = (int)woodcutter.GetCoord().y;

        GameObject local_tile_group = new GameObject(woodcutter.ObjName);
            local_tile_group.transform.parent = gameObject.transform;
            local_tile_group.transform.localPosition = new Vector3(0, 0, 0);
            GameObject tile = Instantiate(woodcutter.WoodcutterPattern, local_tile_group.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);
 	
    }

    public void Build(WoodcutterClass woodcutter){

        int x;
        int y; 

        x = (int)woodcutter.GetCoord().x + 1;
        y = (int)woodcutter.GetCoord().y + 1;

        building.ObjName = string.Format("tile_x{0}_y{1}", x + 1, y + 1); 
        building.buildingType = woodcutter.selectedBuilding;

        if (WoodcutterClass.Buildings.mainBase == woodcutter.selectedBuilding){
            building.skin = woodcutter.mainBaseToBuild;
        }

        buildings.Add(building); 

        GameObject local_tile_group = new GameObject(building.ObjName);
            local_tile_group.transform.parent = gameObject.transform;
            local_tile_group.transform.localPosition = new Vector3(0, 0, 0);
            GameObject tile = Instantiate(building.skin, local_tile_group.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);
 	
    }

    public void setPattern(WoodcutterClass woodcutter, GameObject pattern){

        if (pattern != woodcutter.GetSkin()){
            float x;
            float y; 

            x = woodcutter.GetCoord().x;
            y = woodcutter.GetCoord().y;

            DestroyWoodcutter(woodcutter);

            GameObject local_tile_group = new GameObject(woodcutter.ObjName);

            local_tile_group.transform.parent = gameObject.transform;
            local_tile_group.transform.localPosition = new Vector3(0, 0, 0);
            GameObject tile = Instantiate(pattern, local_tile_group.transform);

            tile.name = string.Format("tile_x{0}_y{1}", (int)x, (int)y);
            tile.transform.localPosition = new Vector3(x, y, 0);

            woodcutter.SetSkin(pattern); 
        }
        
    }


    public void Move(WoodcutterClass woodcutter, float x, float y){
        if (x - 1 < woodcutter.GetCoord().x && woodcutter.GetCoord().x < x + 1 && y - 1 < woodcutter.GetCoord().y && woodcutter.GetCoord().y < y + 1){
            toMove = false; 
            setPattern(woodcutter, woodcutter.WoodcutterPattern); 
        }else if (x < woodcutter.GetCoord().x && y - 1 < woodcutter.GetCoord().y && woodcutter.GetCoord().y < y + 1){
            MoveLeft(woodcutter);
        }else if (x > woodcutter.GetCoord().x && y - 1 < woodcutter.GetCoord().y && woodcutter.GetCoord().y < y + 1){
            MoveRight(woodcutter);
        }else if (x - 1 < woodcutter.GetCoord().x && woodcutter.GetCoord().x < x + 1 && y < woodcutter.GetCoord().y){
            MoveDown(woodcutter);
        }else if (x - 1 < woodcutter.GetCoord().x && woodcutter.GetCoord().x < x + 1 && y > woodcutter.GetCoord().y){
            MoveUp(woodcutter);
        }else if (x > woodcutter.GetCoord().x  && y > woodcutter.GetCoord().y){
            MoveUpRight(woodcutter);
        }else if (x < woodcutter.GetCoord().x  && y < woodcutter.GetCoord().y){
            MoveDownLeft(woodcutter);
        }else if (x > woodcutter.GetCoord().x  && y < woodcutter.GetCoord().y){
            MoveDownRight(woodcutter);
        }else if (x < woodcutter.GetCoord().x  && y > woodcutter.GetCoord().y){
            MoveUpLeft(woodcutter);
        }

    }

    public void MoveRight(WoodcutterClass woodcutter){

        setPattern(woodcutter, woodcutter.WoodcutterRight); 

        float x;
        float y;

        x = woodcutter.GetCoord().x;
        y = woodcutter.GetCoord().y;

        GameObject tile = GameObject.Find(woodcutter.ObjName);

        var delta = Time.deltaTime;

        tile.transform.Translate(Vector3.right * 2 * delta);

        
        tile = GameObject.Find(woodcutter.ObjName + "Highlight");
        if (null != tile){
            tile.transform.Translate(Vector3.right * 2 * delta);
        }
        woodcutter.Setup(x + 2 * delta, y); 
        
    }

    public void MoveLeft(WoodcutterClass woodcutter){

        setPattern(woodcutter, woodcutter.WoodcutterLeft); 

        float x;
        float y;

        x = woodcutter.GetCoord().x;
        y = woodcutter.GetCoord().y;

        GameObject tile = GameObject.Find(woodcutter.ObjName);

        var delta = Time.deltaTime;

        tile.transform.Translate(Vector3.left * 2 * delta);

        tile = GameObject.Find(woodcutter.ObjName + "Highlight");
        if (null != tile){
            tile.transform.Translate(Vector3.left * 2 * delta);
        }
        woodcutter.Setup(x - 2 * delta, y);
    }

    public void MoveUp(WoodcutterClass woodcutter){

        setPattern(woodcutter, woodcutter.WoodcutterUp); 

        float x;
        float y;

        x = woodcutter.GetCoord().x;
        y = woodcutter.GetCoord().y;

        GameObject tile = GameObject.Find(woodcutter.ObjName);

        var delta = Time.deltaTime;

        tile.transform.Translate(Vector3.up * 2 * delta);

        tile = GameObject.Find(woodcutter.ObjName + "Highlight");
        if (null != tile){
            tile.transform.Translate(Vector3.up * 2 * delta);
        }

        woodcutter.Setup(x, y + 2 * delta);
    }

    public void MoveDown(WoodcutterClass woodcutter){

       setPattern(woodcutter, woodcutter.WoodcutterDown); 

        float x;
        float y;

        x = woodcutter.GetCoord().x;
        y = woodcutter.GetCoord().y;

        GameObject tile = GameObject.Find(woodcutter.ObjName);

        var delta = Time.deltaTime;

        tile.transform.Translate(Vector3.down * 2 * delta);

        tile = GameObject.Find(woodcutter.ObjName + "Highlight");

        if (null != tile){
            tile.transform.Translate(Vector3.down * 2 * delta);
        }

        woodcutter.Setup(x, y - 2 * delta);
    }

    public void MoveUpLeft(WoodcutterClass woodcutter){

        setPattern(woodcutter, woodcutter.WoodcutterUpLeft); 

        float x;
        float y;

        x = woodcutter.GetCoord().x;
        y = woodcutter.GetCoord().y;

        GameObject tile = GameObject.Find(woodcutter.ObjName);

        var delta = Time.deltaTime;

        tile.transform.Translate(new Vector3(-1, 1, 0) * 2 * delta);

        tile = GameObject.Find(woodcutter.ObjName + "Highlight");
        if (null != tile){
            tile.transform.Translate(new Vector3(-1, 1, 0) * 2 * delta);
        }
        woodcutter.Setup(x - 2 * delta, y + 2 * delta);
    }

    public void MoveUpRight(WoodcutterClass woodcutter){

        setPattern(woodcutter, woodcutter.WoodcutterUpRight); 

        float x;
        float y;

        x = woodcutter.GetCoord().x;
        y = woodcutter.GetCoord().y;

        GameObject tile = GameObject.Find(woodcutter.ObjName);

        var delta = Time.deltaTime;

        tile.transform.Translate(new Vector3(1, 1, 0) * 2 * delta);

        tile = GameObject.Find(woodcutter.ObjName + "Highlight");
        if (null != tile){
            tile.transform.Translate(new Vector3(1, 1, 0) * 2 * delta);
        }
        woodcutter.Setup(x + 2 * delta, y + 2 * delta); 
    }

    public void MoveDownLeft(WoodcutterClass woodcutter){

        setPattern(woodcutter, woodcutter.WoodcutterDownLeft); 

        float x;
        float y;

        x = woodcutter.GetCoord().x;
        y = woodcutter.GetCoord().y;

        GameObject tile = GameObject.Find(woodcutter.ObjName);

        var delta = Time.deltaTime;

        tile.transform.Translate(new Vector3(-1, -1, 0) * 2 * delta);

        tile = GameObject.Find(woodcutter.ObjName + "Highlight");

        if (null != tile){
            tile.transform.Translate(new Vector3(-1, -1, 0) * 2 * delta);
        }

        woodcutter.Setup(x - 2 * delta, y - 2 * delta); 
    }

    public void MoveDownRight(WoodcutterClass woodcutter){

       setPattern(woodcutter, woodcutter.WoodcutterDownRight); 

        float x;
        float y;

        x = woodcutter.GetCoord().x;
        y = woodcutter.GetCoord().y;

        GameObject tile = GameObject.Find(woodcutter.ObjName);

        var delta = Time.deltaTime;

        tile.transform.Translate(new Vector3(1, -1, 0) * 2 * delta);

        tile = GameObject.Find(woodcutter.ObjName + "Highlight");

        if (null != tile){
            tile.transform.Translate(new Vector3(1, -1, 0) * 2 * delta);
        }

        woodcutter.Setup(x + 2 * delta, y - 2 * delta);  
    }

    public void DestroyWoodcutter(WoodcutterClass woodcutter){
        GameObject worker = GameObject.Find(woodcutter.ObjName);
    
        if (null != worker){
            Destroy(worker); 
        }
    }

    private void MakeHighlight(WoodcutterClass woodcutter){
        
        woodcutter.peasantLarge.SetActive(true); 
        woodcutter.mainBase.SetActive(true); 

    	float x;
        float y; 

        x = woodcutter.GetCoord().x;
        y = woodcutter.GetCoord().y;

        GameObject highlight = GameObject.Find(woodcutter.ObjName + "Highlight");

        if (null == highlight){
            GameObject local_tile_group = new GameObject(woodcutter.ObjName + "Highlight");
                local_tile_group.transform.parent = gameObject.transform;
                local_tile_group.transform.localPosition = new Vector3(0, 0, 0);
                GameObject tile = Instantiate(woodcutter.HighlightPattern, local_tile_group.transform);

            tile.name = string.Format("tile_x{0}_y{1}", x, y);
                tile.transform.localPosition = new Vector3(x, y - 0.2f, 0);
        }

    }

    private bool HasHighlight(WoodcutterClass woodcutter){
        GameObject highlight = GameObject.Find(woodcutter.ObjName + "Highlight");
        return null != highlight; 
    }


    private void DestroyHighlight(WoodcutterClass woodcutter){
        woodcutter.peasantLarge.SetActive(false); 
        woodcutter.mainBase.SetActive(false); 

        GameObject highlight = GameObject.Find(woodcutter.ObjName + "Highlight");
        
        if (null != highlight){
            Destroy(highlight); 
        }
    }

}
