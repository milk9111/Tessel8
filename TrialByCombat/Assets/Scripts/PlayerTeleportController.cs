using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Collections;
using UnityEngine.Tilemaps;

public class PlayerTeleportController : MonoBehaviour
{

	public float teleportRangeRadius = 4f;
	[UnityEngine.Range(0.1f, 1)]
	public float teleportCircleThickness = 0.5f;

	public ParticleSystem teleportRange;

	public ParticleSystem teleportSignal;
	
	private WorldTile _tile;
	private Camera _mainCamera;
	private WorldTile _lastTile;
	private Vector3 _mousePos;
	private Collider2D _collider;
	private WorldTile _lastTeleportedBottomTile;
	private PlayerPlatformerController _platformerController;
	private bool _isTeleportRangeActivated;

	void Awake()
	{
		_mainCamera = Camera.main;

		_collider = GetComponent<Collider2D>();

		_platformerController = GetComponent<PlayerPlatformerController>();
		
		teleportSignal.gameObject.SetActive(false);
		teleportRange.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			_platformerController.StartPlayerMotionPause();
			_isTeleportRangeActivated = true;
			teleportRange.gameObject.SetActive(true);
		}
		else if (Input.GetKeyUp(KeyCode.Mouse1))
		{
			_platformerController.StopPlayerMotionPause();
			teleportSignal.gameObject.SetActive(false);
			_isTeleportRangeActivated = false;
			teleportRange.gameObject.SetActive(false);
		}
		
		if (_isTeleportRangeActivated)
		{
			var worldPoint = GetMousePosition();
			if (IsPointWithinRange(worldPoint))
			{
				if (IsValidTeleportLocation(worldPoint) && Input.GetMouseButtonDown(0))
				{
					TeleportPlayer();
				}
			}
			else
			{
			    teleportSignal.gameObject.SetActive(false);
				ClearTileColor(_lastTile);
			}
		}
		else
		{
			ClearTileColor(_lastTile);
		}
	}

	private bool IsValidTeleportLocation(Vector3Int worldPoint)
	{
		if (HoveringOverTile(worldPoint))
		{
			teleportSignal.gameObject.SetActive(false);
			return false;
		}

		_lastTeleportedBottomTile = IsAboveTile(worldPoint);
		if (_lastTeleportedBottomTile == null)
		{
			teleportSignal.gameObject.SetActive(false);
			return false;
		}

		teleportSignal.gameObject.SetActive(true);
		teleportSignal.gameObject.transform.SetPositionAndRotation(
			new Vector3(_mousePos.x, worldPoint.y), 
			teleportSignal.gameObject.transform.rotation);
		if (!teleportSignal.isPlaying)
		{
			teleportSignal.Play();
		}

		return true;
	}

	private bool HoveringOverTile(Vector3Int worldPoint)
	{
		var tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

		ClearTileColor(_lastTile);

		if (tiles.TryGetValue(worldPoint, out _tile)) 
		{
			_tile.TilemapMember.SetTileFlags(_tile.LocalPlace, TileFlags.None);
			_tile.TilemapMember.SetColor(_tile.LocalPlace, Color.red);
			_lastTile = _tile;

			return true;
		}

		return false;
	}

	private static void ClearTileColor(WorldTile tile)
	{
		if (tile == null) return;
		tile.TilemapMember.SetColor(tile.LocalPlace, Color.white);
	}

	private bool IsPointWithinRange(Vector3Int worldPoint)
	{
		var xDiff = Math.Pow(worldPoint.x - transform.position.x, 2);
		var yDiff = Math.Pow(worldPoint.y - transform.position.y, 2);
		
		var distance = Math.Sqrt(xDiff + yDiff);
		
		return distance <= teleportRangeRadius;
	}

	private void TeleportPlayer()
	{
		var tiles = GameTiles.instance.tiles;

		transform.SetPositionAndRotation(new Vector3(_mousePos.x, 
			_lastTeleportedBottomTile.LocalPlace.y + 1.1f), transform.rotation);

		var foundTileOnSide = CollidingWithSurroundingTile(tiles);
		
		var tries = 0;
		while (tries < 100 && foundTileOnSide != Side.None)
		{
			AttemptTileCollisionResolution(foundTileOnSide);
			foundTileOnSide = CollidingWithSurroundingTile(tiles);
			tries++;
		}

		if (tries >= 100)
		{
			print("reached max tries");
		}
	}

	private void AttemptTileCollisionResolution(Side side)
	{
		var x = transform.position.x;
		var y = transform.position.y;
		switch (side)
		{
			case Side.Left:
				x += _collider.bounds.extents.x;
				break;
			case Side.Top:
				y += _collider.bounds.extents.y;
				break;
			case Side.Right:
				x -= _collider.bounds.extents.x;
				break; 
		}
			
		transform.SetPositionAndRotation(new Vector3(x, y), transform.rotation);
	}

	private WorldTile IsAboveTile(Vector3Int worldPoint)
	{
		var tiles = GameTiles.instance.tiles;		

		var tilePosToCheck = new Vector3(
			Mathf.FloorToInt(worldPoint.x),
			Mathf.FloorToInt(worldPoint.y - tiles.First().Value.TilemapMember.cellSize.y));
		
		return tiles.ContainsKey(tilePosToCheck) ?
			tiles[tilePosToCheck] : null;
	}

	private Side CollidingWithSurroundingTile(IDictionary<Vector3, WorldTile> tiles)
	{
		var tileOnTop = tiles.ContainsKey(new Vector3(
			Mathf.FloorToInt(transform.position.x),
			Mathf.FloorToInt(transform.position.y + _collider.bounds.extents.y)));
		if (tileOnTop)
		{
			return Side.Top;
		}
		var tileOnLeft = tiles.ContainsKey(new Vector3(
			Mathf.FloorToInt(transform.position.x - _collider.bounds.extents.x),
			Mathf.FloorToInt(transform.position.y)));
		if (tileOnLeft)
		{
			return Side.Left;
		}		
		
		var tileOnRight = tiles.ContainsKey(new Vector3(
			Mathf.FloorToInt(transform.position.x + _collider.bounds.extents.x),
			Mathf.FloorToInt(transform.position.y)));
		if (tileOnRight)
		{
			return Side.Right;
		}

		return Side.None;
	}

	private Vector3Int GetMousePosition()
	{
		_mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
		return new Vector3Int(Mathf.FloorToInt(_mousePos.x), Mathf.FloorToInt(_mousePos.y), 0);
	}
	
	private void DrawCircle(LineRenderer lineRenderer)
	{
		const int segments = 360;
		var line = lineRenderer;
		line.useWorldSpace = false;
		line.startWidth = teleportCircleThickness;
		line.endWidth = teleportCircleThickness;
		line.positionCount = segments + 1;

		const int pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
		var points = new Vector3[pointCount];

		for (var i = 0; i < pointCount; i++)
		{
			var rad = Mathf.Deg2Rad * (i * 360f / segments);
			points[i] = new Vector3(Mathf.Sin(rad) * teleportRangeRadius, Mathf.Cos(rad) * teleportRangeRadius, -1.02f);
		}

		line.SetPositions(points);
		line.startColor = Color.red;
		line.endColor = Color.red;
	}

	enum Side
	{
		None = 0,
		Left = 1,
		Top = 2,
		Right = 3,
		Bottom = 4
	}
}
