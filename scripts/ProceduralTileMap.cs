using System;
using Godot;
using Godot.Collections;
using static System.Int32;

namespace ToasterGame.scripts;

public partial class ProceduralTileMap : TileMap {
	[Export] private int _seed;
	[Export] private float _width = 10;
	[Export] private float _height = 10;
	[Export] private float _smoothness = 1;
	[Export] private Player _player;
	[Export] private PackedScene _enemy;
	[Export] private PackedScene _milk;

	private const int CellSize = 32;
	private const int HalfCellSize = CellSize / 2;

	private FastNoiseLite _fastNoiseLite = new() {
		NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin
	};

	private float _lastPlayerX;
	private float _lastCellX;
	private float _lastEnemy;

	public override void _Ready() {
		if (_seed == 0) _seed = new Random().Next(MinValue, MaxValue);
		_lastPlayerX = _player.GlobalPosition.X;
		GenerateMap();
	}

	public override void _Process(double delta) {
		var halfWidth = Mathf.FloorToInt(_width * CellSize / 2);
		// GD.Print($"Player is at {_player.GlobalPosition.X}, last player X was {_lastPlayerX} (half width: {halfWidth})");
		if (!(_player.GlobalPosition.X + 10 > _lastPlayerX + halfWidth) &&
		    !(_player.GlobalPosition.X - 10 < _lastPlayerX - halfWidth)) return;
		GD.Print($"Player moved from {_lastPlayerX} to {_player.GlobalPosition.X}, generating new map");
		_lastPlayerX = Mathf.RoundToInt(_player.GlobalPosition.X);
		GenerateMap();
	}

	private void GenerateMap() {
		_fastNoiseLite.Seed = _seed;
		var cells = new Array<Vector2I>();
		var lastX = 0f;
		GD.Print($"Generating map from cellX {_lastCellX} to {_lastCellX + _width}");
		var random = new Random();
		for (var x = 0; x < _width; x++) {
			var lastPlayerXCell = _lastCellX + x;
			var lastPlayerXWorld = lastPlayerXCell * CellSize;
			SetCell(0, new Vector2I(x, 0), 0, new Vector2I(0, 0));
			var perlinHeight = Mathf.RoundToInt(_fastNoiseLite.GetNoise1D(lastPlayerXWorld / _smoothness) * _height) + 10;
			var perlinHeightWorld = perlinHeight * CellSize;
			for (var y = 1; y < perlinHeight; y++) {
				var cell = new Vector2I(Mathf.RoundToInt(lastPlayerXCell), -y);
				cells.Add(cell);
				SetCell(0, new Vector2I(Mathf.RoundToInt(lastPlayerXCell), -y), 0, new Vector2I(0, 0));
			}

			lastX = _lastCellX + x;
			var generateEnemy = random.NextDouble() > 0.5;
			var generateTwoEnemies = random.NextDouble() > 0.5;
			var generateMilk = random.NextDouble() < 0.1;
			var enemyIsFarEnough = Mathf.RoundToInt(lastPlayerXWorld) > _lastEnemy + 5 * CellSize;
			var enemyIsFarEnoughFromPlayer = Mathf.RoundToInt(lastPlayerXWorld + CellSize) > _player.GlobalPosition.X + 5 * CellSize;
			if (!generateEnemy || !enemyIsFarEnough || !enemyIsFarEnoughFromPlayer) continue;
			var enemy = _enemy.Instantiate<Enemy>();
			enemy.GlobalPosition = new Vector2(Mathf.RoundToInt(lastPlayerXWorld), -perlinHeightWorld - CellSize - HalfCellSize);
			GetParent().CallDeferred("add_child", enemy);
			if (generateTwoEnemies) {
				var enemy2 = _enemy.Instantiate<Enemy>();
				enemy2.GlobalPosition = new Vector2(Mathf.RoundToInt(lastPlayerXWorld) - HalfCellSize, -perlinHeightWorld - CellSize - CellSize - HalfCellSize);
				GetParent().CallDeferred("add_child", enemy2);
			}
			GD.Print($"setting lastenemy to {lastPlayerXWorld}, next enemy should spawn after {lastPlayerXWorld + 10 * CellSize}");
			_lastEnemy = Mathf.RoundToInt(lastPlayerXWorld);
			if (!generateMilk) continue;
			var milk = _milk.Instantiate<Milk>();
			milk.GlobalPosition = new Vector2(Mathf.RoundToInt(lastPlayerXWorld), -perlinHeightWorld - CellSize - HalfCellSize);
			GetParent().CallDeferred("add_child", milk);
		}

		_lastCellX = lastX;
		SetCellsTerrainConnect(0, cells, 0, 0);
	}
}