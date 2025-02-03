# GoDogKit

[中文点这里](README_zh.md)

`Godot 4.0`
`.NET 8.0`

## Introduction

A lightweight, weak framework, library that provides optional tools to assist Godot C # development.

## Core Features

### *Optional:*
All features in GoDogKit are initialized only when they are used, so there is no need to worry about the program structure becoming bloated.
### *Lightweight coroutine support:*
The coroutines in GoDogKit are very similar to those in Unity, but differ significantly from Godot's native coroutines, so if you are familiar with Unity, it is easy to get started.

```csharp
    // Write a coroutine.
    IEnumerator Attack()
    {        
        GD.Print("Wait for input!");
        yield return new WaitForInputActionJustPressed("ui_left");
        yield return new WaitForInputActionJustPressed("ui_up");
        yield return new WaitForInputActionJustPressed("ui_right");
        yield return new WaitForInputActionJustPressed("ui_down");
        GD.Print("Attack!");        
    }

    // Call it.
    public override void _Ready()
    {       
        this.StartCoroutine(Attack);
    }
```

You can also implement your own coroutine by inheriting from the 'Coronine' class.

### *Global management classes and global nodes:*
Global classes in GoDogKit represent the same lifecycle as their objects and games. On this basis, the global class generates nodes with different functions and adds them to the scene tree to participate in the game process.

Due to the confliction between global classes and [Godot best practices](https://docs.godotengine.org/zh-cn/4.x/tutorials/best_practices/index.html). All global functions are designed as optional features, and their corresponding nodes will only be loaded into the scene tree when they are used. Can be regarded as a type of 'defered' [`Autoload`](https://docs.godotengine.org/zh-cn/4.x/tutorials/scripting/singletons_autoload.html).

```csharp
    [Export] public PackedScene scene;

    public override void _Ready()
    {
        // This will apply the GlobalSceneManager.
        scene.RegisterToGlobalScenes("Level 1");
    }

    public override void _Input(InputEvent @event)
	{
		if(Input.IsActionJustReleased("ui_accept"))
		{
			SceneKit.GoToScene("Level 1");
		}
	}
```

### *Plenty of (To be) Kits support:*

```csharp
public override void _Input(InputEvent @event)
{
    if(InputKit.IsActionJustReleasedContinuous("ui_accept", 3))
    {
        GD.Print("Tab me in three times!");
    }

    if(InputKit.IsActionJustPressedForSeconds("ui_accept", 3.0f))
    {
        GD.Print("Hold me at least 3 seconds!");
    }
}
```

## Recent updates and further plans
### Recent updates
* Join a more complete object pool `ManagedPool`
### Plan
* Further simplification and lightweighting