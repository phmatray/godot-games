using Godot;

namespace PlatformerBrackeys;

public partial class GameManager : Node
{
    public int Score { get; private set; } = 0;
    
    private Label _scoreLabel = null!;

    public override void _Ready()
    {
        // Initialize the score label by finding the node
        _scoreLabel = GetNode<Label>("%ScoreLabel");
            
        // Ensure the label node exists to avoid runtime errors
        if (_scoreLabel == null)
        {
            GD.PrintErr("ScoreLabel node not found. Please ensure it exists and is correctly named.");
            return;
        }

        UpdateScoreLabel();
    }

    // Method to add a point to the score
    public void AddPoint()
    {
        Score++;
        UpdateScoreLabel();
        GD.Print("You collected a coin!");
    }
    
    // Method to update the score label's text
    private void UpdateScoreLabel()
    {
        _scoreLabel.Text = $"You collected {Score} coins!";
    }
}