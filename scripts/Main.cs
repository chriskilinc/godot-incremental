using Godot;
using System;

// Main should handle UI and other non-gameplay elements
public partial class Main : Node2D
{
    private Label _foodLabel;
    private Button _clickButton;

    public override void _Ready()
    {
        GD.Print("Main scene is ready.");
        GD.Print(GameHandler.Instance != null ? "GameHandler instance is accessible." : "GameHandler instance is null.");

        _foodLabel = GetNodeOrNull<Label>("UI/MarginContainer/HBoxContainer/FoodLabel");
        _clickButton = GetNodeOrNull<Button>("UI/MarginContainer/HBoxContainer/ClickButton");

        if (_clickButton != null)
        {
            _clickButton.Pressed += OnHarvestButtonPressed;
        }

        if (GameHandler.Instance != null)
        {
            GameHandler.Instance.ResourceUpdated += OnResourceUpdated;
        }
        else
        {
            GD.PushError("GameHandler instance is null in Main._Ready()");
        }

        SetUI();
    }

    private void OnHarvestButtonPressed()
    {
        GameHandler.Instance.AddFood(1);
    }

    private void OnResourceUpdated(string resource, double value)
    {
        if (resource == "food")
        {
            _foodLabel.Text = $"Food: {value}";
        }
    }

    private void SetUI()
    {
        if (_foodLabel == null || _clickButton == null)
        {
            GD.PushError("UI elements not found in Main._Ready()");
            return;
        }

        _foodLabel.Text = $"Food: {GameHandler.Instance.Resources["food"]}";
        _clickButton.Text = "Harvest Food";
    }
}
