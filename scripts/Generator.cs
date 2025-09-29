using System;
using System.Collections.Generic;

public class Generator
{
    public string Name { get; set; }
    public Dictionary<string, double> BaseCost { get; set; }
    public Dictionary<string, double> Production { get; set; }
    public int Quantity { get; set; } = 0;

    private const double CostMultiplier = 1.15;

    // public Generator(string name, Dictionary<string, double> baseCost, Dictionary<string, double> production, int quantity = 0)
    // {
    //     Name = name;
    //     BaseCost = baseCost;
    //     Production = production;
    //     Quantity = quantity;
    // }

    public Dictionary<string, double> GetCurrentCost()
    {
        var currentCost = new Dictionary<string, double>();
        foreach (var kvp in BaseCost)
        {
            currentCost[kvp.Key] = kvp.Value * Math.Pow(CostMultiplier, Quantity);
        }
        return currentCost;
    }

    public Dictionary<string, double> GetProduction()
    {
        var totalProduction = new Dictionary<string, double>();
        foreach (var kvp in Production)
        {
            totalProduction[kvp.Key] = kvp.Value * Quantity;
        }
        return totalProduction;
    }
}