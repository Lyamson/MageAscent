using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public Guid Id
    {
        get;
        protected set;
    }
    protected string title;
    public string Title
    {
        get => title; 
        protected set
        {
            if (title != value)
            {
                title = value;
            }
        }
    }
    protected string description;
    public virtual string Description
    {
        get => description; 
        protected set
        {
            if (description != value)
            {
                description = value;
            }
        }
    }
    public uint PointsCost;

    public abstract void Apply();

    public abstract void Cancel();

    public static T CreateEffect<T>(string title, string description, uint pointsCost = 1) where T : Effect, new()
    {
        T effect = new T();
        effect.Id = Guid.NewGuid();
        effect.Title = title;
        effect.Description = description;
        effect.PointsCost = pointsCost;
        return effect;
    }

    public abstract Effect Clone();
}
