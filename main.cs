using Godot;
using System;

public partial class main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }
	
	[Export]
	public PackedScene Heart { get; set; }
	
	[Export]
	public PackedScene PowerUp { get; set; }

	private int _score;
	private int _health;
	private bool _powerUp;
	private int _multiplier;
	private float _arielPlayback;
	
	public override void _Ready(){
	}
	
	public override void _Process(double delta){
		var HUD = GetNode<HUD>("HUD");
		Timer powerUpTime = GetNode<Timer>("PowerUpTimer");
		HUD.DisplayPowerUpTimer(powerUpTime.TimeLeft);
	}
	
	private void gameOver(){
	var player = GetNode<Player>("Player");
	player.Hide();
	player.GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	GetNode<Timer>("MobTimer").Stop();
	GetNode<Timer>("ScoreTimer").Stop();
	GetNode<Timer>("SpawnPowerUpTimer").Stop();
	GetNode<Timer>("HeartTimer").Stop();
	GetNode<HUD>("HUD").ShowGameOver();
	GetNode<AudioStreamPlayer>("Ariel").Stop();
	GetNode<AudioStreamPlayer>("DeathSound").Play();
	}

	public void NewGame(){
	_score = 0;
	_health = 3;
	_powerUp = false;
	_multiplier = 1;
	_arielPlayback = 0;
	

	var player = GetNode<Player>("Player");
	var startPosition = GetNode<Marker2D>("StartPosition");
	player.Start(startPosition.Position);

	GetNode<Timer>("StartTimer").Start();
	var hud = GetNode<HUD>("HUD");
	hud.UpdateScore(_score);
	hud.UpdateHealth(_health);
	hud.ShowMessage("Get Ready!");
	GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
	GetTree().CallGroup("hearts", Node.MethodName.QueueFree);
	GetTree().CallGroup("powerups", Node.MethodName.QueueFree);
	GetNode<AudioStreamPlayer>("Ariel").Play();
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
		_score += (1 *_multiplier);
		GetNode<HUD>("HUD").UpdateScore(_score);
	}
	
	private void OnSpawnPowerUpTimerTimeout(){
		PowerUp powerUp = PowerUp.Instantiate<PowerUp>();
		// Set the mob's position to a random location.
		int x,y;
		x = (int)GD.RandRange(50,1000);
		y = (int)GD.RandRange(50,600);
		powerUp.Position = new Vector2(x,y);

		AddChild(powerUp);
	}
	
	private void OnStartTimerTimeout(){
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
		GetNode<Timer>("HeartTimer").Start();
		GetNode<Timer>("SpawnPowerUpTimer").Start();
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
	
	private void OnPowerUpTimeout(){
		_powerUp = false;
		GetNode<HUD>("HUD").UpdateHealth(_health);
		GetNode<Timer>("PowerUpTimer").Stop();
		GetNode<AudioStreamPlayer>("House").Stop();
		GetNode<AudioStreamPlayer>("Ariel").Play(_arielPlayback);
	}
	
	public void MobHit(){
		if(_powerUp){
			DestroyEnemy();
			GetNode<AudioStreamPlayer>("ScoreSound").Play();
		}else{
			DecreaseHealth();
		}
	}
	
	public void DestroyEnemy(){
		_score = _score + (_multiplier * 5);
		GetNode<HUD>("HUD").UpdateScore(_score);
		//GetNode<Timer>("PowerUpTimer").WaitTime += 3;
	}
	
	public void DecreaseHealth(){
		_health--;
		GetNode<HUD>("HUD").UpdateHealth(_health);
		if(_health == 0){
			gameOver();
		}
		GetNode<AudioStreamPlayer>("HurtSound").Play();
	}
	
	public void IncreaseHealth(){
		if(_health == 9){
		}else if(_powerUp){
			_score = _score + _multiplier;
			GetNode<HUD>("HUD").UpdateScore(_score);
		}else{
			_health++;
			_score++;
			GetNode<HUD>("HUD").UpdateHealth(_health);
			GetNode<HUD>("HUD").UpdateScore(_score);
		}
		GetNode<AudioStreamPlayer>("HealthPickUp").Play();
	}
	
	public void StartPowerUp(){
		GetNode<Timer>("PowerUpTimer").Start();
		_powerUp = true;
		GetNode<HUD>("HUD").UpdateHealth("INF");
		_arielPlayback = GetNode<AudioStreamPlayer>("Ariel").GetPlaybackPosition();
		GetNode<AudioStreamPlayer>("Ariel").Stop();
		GetNode<AudioStreamPlayer>("House").Play();
	}

}

