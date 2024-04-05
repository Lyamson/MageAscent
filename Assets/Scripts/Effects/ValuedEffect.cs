using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ValuedEffect : Effect
{
    public Dictionary<Stats.StatType, int> Values = new();
    public override string Description { get => ParseDescription(description); protected set => base.Description = value; }

    public override void Apply()
    {
        foreach (var item in Values)
        {
            GameManager.Singleton.player.AddStatsChange(item.Key, Id, item.Value);
        }
    }

    public override void Cancel()
    {
        GameManager.Singleton.player.RemoveStatsChange(this);
    }

    public ValuedEffect() { }

    protected ValuedEffect(string title, string description, uint pointsCost, Dictionary<Stats.StatType, int> values)
    {
        Id = Guid.NewGuid();
        this.title = title;
        this.description = description;
        this.PointsCost = pointsCost;
        this.Values = values.ToDictionary(entry => entry.Key, entry => entry.Value);
    }

    public Effect AddValueChange(Stats.StatType statType, int value)
    {
        Values.Add(statType, value);
        return this;
    }

    private string ParseDescription(string description)
    {
        string parsedDescription = description;
        foreach (var item in Values)
        {
            parsedDescription = parsedDescription.Replace($"%{item.Key}%", item.Value.ToString());
        }
        return parsedDescription;
    }

    public override Effect Clone()
    {
        return new ValuedEffect(this.title, this.description, this.PointsCost, this.Values);
    }
}
