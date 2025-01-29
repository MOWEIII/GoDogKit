using Godot;

namespace GoDogKit;

public static class MathUtility
{
    public static Vector2 RandomCircle(float radius)
    {
        var biasX = (float)GD.RandRange(-radius, radius);
        var biasY = (float)GD.RandRange(-radius, radius);
        return new Vector2(biasX, biasY);
    }

    public static Vector3 RandomSphere(float radius)
    {
        var biasX = (float)GD.RandRange(-radius, radius);
        var biasY = (float)GD.RandRange(-radius, radius);
        var biasZ = (float)GD.RandRange(-radius, radius);
        return new Vector3(biasX, biasY, biasZ);
    }
}