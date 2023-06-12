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
 
    int map_width = 160;
    int map_height = 160;
 
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
	    woodcutter.Setup(40, 40); 
	    SpawnWoodcutter(woodcutter);
    }

    void Update(){
        
        if (Input.GetMouseButtonDown(0)){
            
            Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (pos.x - 1 < woodcutter.GetCoord().x && woodcutter.GetCoord().x < pos.x + 1 && pos.y - 1 < woodcutter.GetCoord().y && woodcutter.GetCoord().y < pos.y + 1){
                MakeHighlight(woodcutter);     
            }else{
                DestroyHighlight();
            }
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

        int x;
        int y; 

        x = (int)woodcutter.GetCoord().x;
        y = (int)woodcutter.GetCoord().y;

        GameObject local_tile_group = new GameObject("mainWoodcutter");
            local_tile_group.transform.parent = gameObject.transform;
            local_tile_group.transform.localPosition = new Vector3(0, 0, 0);
            GameObject tile = Instantiate(woodcutter.WoodcutterPattern, local_tile_group.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);
 	
    }

    private void MakeHighlight(WoodcutterClass woodcutter){
    	int x;
        int y; 

        x = (int)woodcutter.GetCoord().x;
        y = (int)woodcutter.GetCoord().y;

        GameObject highlight = GameObject.Find("mainWoodcutterHighlight");

        if (null == highlight){
            GameObject local_tile_group = new GameObject("mainWoodcutterHighlight");
                local_tile_group.transform.parent = gameObject.transform;
                local_tile_group.transform.localPosition = new Vector3(0, 0, 0);
                GameObject tile = Instantiate(woodcutter.HighlightPattern, local_tile_group.transform);

            tile.name = string.Format("tile_x{0}_y{1}", x, y);
                tile.transform.localPosition = new Vector3(x, y - 0.3f, 0);
            }
    }

    private void DestroyHighlight(){
        GameObject highlight = GameObject.Find("mainWoodcutterHighlight");
        
        if (null != highlight){
            Destroy(highlight); 
        }
    }

}