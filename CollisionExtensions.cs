using System.Collections.Generic;
using Godot;

namespace Grimore;

public static class CollisionExtensions
{
    public static IEnumerable<GodotObject> EnumerateCollisions(this KinematicCollision3D collisions)
    {
        var count = collisions?.GetCollisionCount() ?? 0;

        for (int idx = 0; idx < count; idx++)
        {
            yield return collisions!.GetCollider(idx);
        }
    }
}