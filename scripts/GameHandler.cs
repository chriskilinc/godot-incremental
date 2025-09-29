using Godot;
using System;
using System.Collections.Generic;

public partial class GameHandler : Node
{
    public static GameHandler Instance;
    public Dictionary<string, double> Resources = new()
    {
        { "food", 0.0 },
        // { "wood", 0.0 },
        // { "stone", 0.0 }
    };
    public List<Generator> Generators = new();

    private double _tickAccumulator = 0.0;
    private const double _tickInterval = 1.0; // 1 second per tick

    [Signal]
    public delegate void ResourceUpdatedEventHandler(string resource, double value);

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            GD.Print("GameHandler instance created.");
        }
        else
        {
            GD.PrintErr("GameHandler instance already exists!");
            QueueFree();
        }

        // Get main scene called "main"
        var gameScene = GetTree().CurrentScene;
        if (gameScene == null || gameScene.Name != "main")
        {
            GD.PrintErr("Main scene 'main' not found!");
        }
        else
        {
            GD.Print("Main scene 'main' loaded successfully.");
        }
    }

    public override void _Process(double delta)
    {
        _tickAccumulator += delta;
        if (_tickAccumulator >= _tickInterval)
        {
            _tickAccumulator -= _tickInterval;
            OnTick();
        }
    }

    private void OnTick()
    {
        foreach (var g in Generators)
        {
            var prod = g.GetProduction();
            foreach (var kv in prod)
            {
                // if (!Resources.ContainsKey(kv.Key))
                //     Resources[kv.Key] = 0;

                Resources[kv.Key] += kv.Value;
                EmitSignal(SignalName.ResourceUpdated, kv.Key, Resources[kv.Key]);
            }
        }
    }

    public void AddFood(double amount)
    {
        Resources["food"] += amount;
        EmitSignal(SignalName.ResourceUpdated, "food", Resources["food"]);
    }


    private void LoadGeneratorsFromJson(string path)
    {
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"Failed to open file: {path}");
            return;
        }

        var json = file.GetAsText();
        var result = Json.ParseString(json);
        if (result.VariantType != Variant.Type.Array)
        {
            GD.PrintErr("Generator JSON must be an array.");
            return;
        }

        var array = result.AsGodotArray();

        foreach (var item in array)
        {
            var dict = item.AsGodotDictionary();
            var gen = new Generator()
            {
                Name = dict["name"].ToString(),
                BaseCost = ToDictionary(dict["base_cost"].AsGodotDictionary()),
                Production = ToDictionary(dict["production"].AsGodotDictionary())
            };
            Generators.Add(gen);
        }

        GD.Print($"Loaded {Generators.Count} generators from {path}.");
    }

    public void SaveGame()
    {
        var godotResources = new Godot.Collections.Dictionary();
        foreach (var kvp in Resources)
        {
            godotResources[kvp.Key] = kvp.Value;
        }

        var saveData = new Godot.Collections.Dictionary
        {
            { "resources", godotResources },
        };

        using var file = FileAccess.Open("user://save.json", FileAccess.ModeFlags.Write);
        file.StoreString(Json.Stringify(saveData));
    }

    public void LoadGame()
    {
        if (!FileAccess.FileExists("user://save.json"))
        {
            GD.Print("No save file found.");
            return;
        }

        using var file = FileAccess.Open("user://save.json", FileAccess.ModeFlags.Read);
        var json = file.GetAsText();
        var result = Json.ParseString(json).AsGodotDictionary();


        var res = result["resources"].AsGodotDictionary();
        foreach (var key in res.Keys)
            Resources[key.ToString()] = (double)res[key];

        GD.Print("Game loaded.");
    }

    // helper: convert Godot dict -> C# dict
    private Dictionary<string, double> ToDictionary(Godot.Collections.Dictionary godotDict)
    {
        var result = new Dictionary<string, double>();
        foreach (var key in godotDict.Keys)
            result[key.ToString()] = (double)godotDict[key];
        return result;
    }
}
