# GoDogKit

`Godot 4.0`
`.NET 8.0`

## 介绍

一个轻量级弱框架类库，提供可选工具助力 Godot C# 开发。

<!-- 一个用于Godot开发的工具集，主要内容是个人开发时积累下来的轮子，后续 **“可能”** 还会继续更新。直接把addons文件夹复制粘贴到Godot项目中就可以使用了。 -->

还有，名字虽然叫 **“走狗工具集”**，但不是贬义，只是个谐音梗哈哈。

## 核心特性
### *可有可无：*
GoDogKit中的所有功能都只有在用到它们的时候才会初始化，所以不用担心程序结构变得臃肿。
### *轻巧的协程支持：*
GoDogKit中的协程和Unity中的协程很相似，但和Godot原生的协程差别较大，所以如果你熟悉Unity的话，很容易上手。

    // 编写一个协程方法
    IEnumerator Attack()
    {        
        GD.Print("Wait for input!");
        yield return new WaitForInputActionJustPressed("ui_left");
        yield return new WaitForInputActionJustPressed("ui_up");
        yield return new WaitForInputActionJustPressed("ui_right");
        yield return new WaitForInputActionJustPressed("ui_down");
        GD.Print("Attack!");        
    }

    // 调用协程
    public override void _Ready()
    {       
        this.StartCoroutine(Attack);
    }
你还可以通过继承`Coroutine`类实现自己的协程。
### *全局管理类与全局节点：*
全局类在GoDogKit中代表其对象的生命周期和游戏相同。在此基础上全局类通过衍生出不同功能的节点加入场景树中，从而参与游戏流程。

鉴于全局类和[Godot最佳实践](https://docs.godotengine.org/zh-cn/4.x/tutorials/best_practices/index.html)的冲突。全局功能全部设计为可选功能，只要它们在被使用时才会加载对应的节点到场景树中。可以视为一种“延迟”的[`Autoload`](https://docs.godotengine.org/zh-cn/4.x/tutorials/scripting/singletons_autoload.html)。

    [Export] public PackedScene scene;

    public override void _Ready()
    {
        // 这会激活全局场景管理器
        scene.RegisterToGlobalScenes("Level 1");
    }

    public override void _Input(InputEvent @event)
	{
		if(Input.IsActionJustReleased("ui_accept"))
		{
			SceneKit.GoToScene("Level 1");
		}
	}
### *丰富的（其实不然）工具类和工具节点：*

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

## 最近更新与计划
### 最近更新
* 加入更完备的对象池`ManagedPool`
### 计划
* 进一步简化，轻量化