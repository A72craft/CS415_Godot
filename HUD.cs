using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();
	
	public void ShowMessage(string text){
	var message = GetNode<Label>("Message");
	message.Text = text;
	message.Show();

	GetNode<Timer>("MessageTimer").Start();
	}

	async public void ShowGameOver(){
		ShowMessage("Game Over");

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		var message = GetNode<Label>("Message");
		message.Text = "Avoid The Monsters!";
		message.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<Button>("StartButton").Show();
	}
	
	public void UpdateScore(int score){
		GetNode<Label>("ScoreLabel").Text = score.ToString();
	}
	
	private void OnStartButtonPressed(){
		GetNode<Button>("StartButton").Hide();
	EmitSignal(SignalName.StartGame);
	}

	private void OnMessageTimerTimeout(){
		GetNode<Label>("Message").Hide();
	}
	
	public void UpdateHealth(int health){
		GetNode<Label>("HealthLabel").Text = health.ToString();
	}
	
	public void UpdateHealth(string health){
		GetNode<Label>("HealthLabel").Text = health;
	}
	
	public void DisplayPowerUpTimer(double time){
		var powerUpLabel = GetNode<Label>("PowerUpLabel");
		if(time <= 0){
			powerUpLabel.Hide();
		}else{
			powerUpLabel.Show();
			powerUpLabel.Text = time.ToString("F2");
		}
	}
	
	public void DisplayHighScore(bool isShown){
		Label highScore = GetNode<Label>("HighScore");
		if(isShown){
			highScore.Show();
		}else{
			highScore.Hide();
		}
	}
	
	public void SetHighScoreDisplay(int score){
		Label highScore = GetNode<Label>("HighScore");
		string str = "Highest Score:\n" + score;
		highScore.Text = str;
	}
}

