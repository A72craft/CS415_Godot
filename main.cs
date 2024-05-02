using Godot;
using System;

public partial class main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }
	
	[Export]
	public PackedScene Heart { get; set; }

	private int _score;
	private int _health;
	
	public override void _Ready(){
	}
	
	private void gameOver(){
	var player = GetNode<Player>("Player");
	player.Hide();
	player.GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	GetNode<Timer>("MobTimer").Stop();
	GetNode<Timer>("ScoreTimer").Stop();
	GetNode<HUD>("HUD").ShowGameOver();
	GetNode<AudioStreamPlayer>("Music").Stop();
	GetNode<AudioStreamPlayer>("DeathSound").Play();
	}

	public void NewGame(){
	_score = 0;
	_health = 3;

	var player = GetNode<Player>("Player");
	var startPosition = GetNode<Marker2D>("StartPosition");
	player.Start(startPosition.Position);

	GetNode<Timer>("StartTimer").Start();
	var hud = GetNode<HUD>("HUD");
	hud.UpdateScore(_score);
	hud.UpdateHealth(_health);
	hud.ShowMessage("Get Ready!");
	GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
	GetNode<AudioStreamPlayer>("Music").Play();
	}
	
	private void OnMobTimerTimeout(){
		// Create a new instance of the Mob scene.
		Mob mob = MobScene.Instantiate<Mob>();

		// Choose a random location on Path2D.
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		// Set the mob's direction perpendicular to the path direction.
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		// Set the mob's position to a random location.
		mob.Position = mobSpawnLocation.Position;

		// Add some randomness to the direction.
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		// Choose the velocity.
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}
	
	private void OnScoreTimerTimeout(){
		_score++;
		GetNode<HUD>("HUD").UpdateScore(_score);
	}
	
	private void OnStartTimerTimeout(){
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
		GetNode<Timer>("HeartTimer").Start();
	}
	
	private void OnStartHeartTimeout(){
		Heart heart = Heart.Instantiate<Heart>();
		// Set the mob's position to a random location.
		int x,y;
		x = (int)GD.RandRange(50,1000);
		y = (int)GD.RandRange(50,600);
		heart.Position = new Vector2(x,y);

		AddChild(heart);
	}
	
	public void DecreaseHealth(){
		_health--;
		GetNode<HUD>("HUD").UpdateHealth(_health);
		if(_health == 0){
			gameOver();
		}
	}
	
	public void IncreaseHealth(){
		if(_health == 9){
		}else{
			_health++;
			_score++;
			GetNode<HUD>("HUD").UpdateHealth(_health);
			GetNode<HUD>("HUD").UpdateScore(_score);
		}
	}

}
