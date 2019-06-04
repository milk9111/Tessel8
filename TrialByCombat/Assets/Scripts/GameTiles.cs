using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTiles : MonoBehaviour {
	public static GameTiles instance;
	
	public Tilemap Tilemap;
	public Tilemap BorderTilemap;

	public Dictionary<Vector3, WorldTile> tiles;
	public Dictionary<Vector3, WorldTile> borderTiles;

	private void Awake() 
	{
		if (instance == null) 
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		tiles = new Dictionary<Vector3, WorldTile>();
		borderTiles = new Dictionary<Vector3, WorldTile>();
		GetWorldTiles(tiles, Tilemap);
		GetWorldTiles(borderTiles, BorderTilemap);
	}

	// Use this for initialization
	private void GetWorldTiles (IDictionary<Vector3, WorldTile> tiles, Tilemap tilemap) 
	{
		foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
		{
			var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

			if (!tilemap.HasTile(localPlace)) continue;
			var tile = new WorldTile
			{
				LocalPlace = localPlace,
				WorldLocation = tilemap.CellToWorld(localPlace),
				TileBase = tilemap.GetTile(localPlace),
				TilemapMember = tilemap,
				Name = localPlace.x + "," + localPlace.y,
				Cost = 1 // TODO: Change this with the proper cost from ruletile
			};
			
			tiles.Add(tile.WorldLocation, tile);
		}
	}
}