using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Tilemaps;
using UserInterface;

public class PlayerTeleportController : MonoBehaviour
{
	private const int TOTAL_RECHARGE_TIME_IN_SEC = 10;
	
	public int stamina = 100;
	public int staminaRechargeRatePerSecond = 4;
	public int staminaCostPerTeleport = 15;
	public float waitTimeInSecTillStaminaRecharge = 1;
	
	public float teleportRangeRadius = 4f;
	[Range(0.1f, 1)]
	public float teleportCircleThickness = 0.5f;
	[Tooltip("The Y offset amount to add to player position on teleport (so player doesn't get stuck in tile collider)")]
	public float teleportLandYOffset = 1.5f;

	public ParticleSystem teleportRange;

	public ParticleSystem teleportSignal;
		
	public StaminaBar staminaBar;
	
	private WorldTile _tile;

	private IDictionary<Vector3, WorldTile> _tiles;
	private Camera _mainCamera;
	private WorldTile _lastTile;
	private Vector3 _mousePos;
	private Collider2D _collider;
	private WorldTile _lastTeleportedBottomTile;
	private PlayerPlatformerController _platformerController;
	private bool _isTeleportRangeActivated;
	private bool _isDisabled;

	private float _currStamina;

	private float _remainingSecondsOnTimer;

	private Coroutine _staminaRechargeWaitCoroutine;
	private Coroutine __rechargeStaminaCoroutine;

	void Awake()
	{
		_tiles = null;
		
		_mainCamera = Camera.main;

		_collider = GetComponent<Collider2D>();

		_platformerController = GetComponent<PlayerPlatformerController>();
		
		teleportSignal.gameObject.SetActive(false);
		teleportRange.gameObject.SetActive(false);

		_isDisabled = false;

		_currStamina = stamina;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_isDisabled) return;
		
		if (_tiles == null)
		{
			_tiles = GameTiles.instance.tiles;
			foreach (var tile in GameTiles.instance.borderTiles)
			{
				if (_tiles.ContainsKey(tile.Key)) continue;
				_tiles.Add(tile.Key, tile.Value);
			}
		}
		
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
				if (IsValidTeleportLocation(worldPoint) && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Q))
				    && _currStamina - staminaCostPerTeleport >= 0)
				{
					if (_staminaRechargeWaitCoroutine != null && __rechargeStaminaCoroutine != null)
					{
						StopCoroutine(_staminaRechargeWaitCoroutine);
						StopCoroutine(__rechargeStaminaCoroutine);
					}

					TeleportPlayer();
					_currStamina -= staminaCostPerTeleport;
					staminaBar.OnAction(staminaCostPerTeleport / (float)stamina);
					_staminaRechargeWaitCoroutine = StartCoroutine(WaitForStaminaRechargeStart());
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

	public void ResetStamina()
	{
		_currStamina = stamina;
		staminaBar.ResetStaminaBar();
	}

	public void DisableTeleport()
	{
		_isDisabled = true;
	}
	
	public void EnableTeleport()
	{
		_isDisabled = false;
	}

	public bool IsTeleportRangeActivated()
	{
		return _isTeleportRangeActivated;
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
		ClearTileColor(_lastTile);

		if (_tiles.TryGetValue(worldPoint, out _tile)) 
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
		transform.SetPositionAndRotation(new Vector3(_mousePos.x, 
			_lastTeleportedBottomTile.LocalPlace.y + teleportLandYOffset), transform.rotation);

		var foundTileOnSide = CollidingWithSurroundingTile(_tiles);
		
		var tries = 0;
		while (tries < 100 && foundTileOnSide != Side.None)
		{
			AttemptTileCollisionResolution(foundTileOnSide);
			foundTileOnSide = CollidingWithSurroundingTile(_tiles);
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
		var tilePosToCheck = new Vector3(
			Mathf.FloorToInt(worldPoint.x),
			Mathf.FloorToInt(worldPoint.y - _tiles.First().Value.TilemapMember.cellSize.y));
		
		return _tiles.ContainsKey(tilePosToCheck) ?
			_tiles[tilePosToCheck] : null;
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

	private IEnumerator WaitForStaminaRechargeStart()
	{
		yield return new WaitForSeconds(waitTimeInSecTillStaminaRecharge);
		__rechargeStaminaCoroutine = StartCoroutine(RechargeStamina());
	}

	private IEnumerator RechargeStamina()
	{
		var timerLength = _remainingSecondsOnTimer > 0 ? _remainingSecondsOnTimer : TOTAL_RECHARGE_TIME_IN_SEC;
		for (_remainingSecondsOnTimer = timerLength;
			_remainingSecondsOnTimer > 0;
			_remainingSecondsOnTimer -= Time.deltaTime)
		{
			var amountToRecharge = _currStamina + staminaRechargeRatePerSecond < stamina
				? staminaRechargeRatePerSecond
				: stamina - _currStamina;

			_currStamina = _currStamina + amountToRecharge;
			
			if (_currStamina <= stamina)
			{
				staminaBar.OnAction(-(amountToRecharge / stamina));
			}
			else
			{
				break;
			}

			yield return null;
		}
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

	
}
