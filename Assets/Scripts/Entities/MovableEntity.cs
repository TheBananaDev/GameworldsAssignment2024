using BaseEntities;
using AbstractGameEntities;
using System;
using UnityEngine;

namespace MovableEntities
{
    public class MovingEntity : MovableEntity<MovingEntityStats, MovingEntityTemplate>
    {
        public override T GetStat<T>(MovingEntityStats stats)
        {
            switch (stats)
            {
                case MovingEntityStats.currHealth:
                    return (T)Convert.ChangeType(currHealth, typeof(T));
                case MovingEntityStats.baseMaxHealth:
                    return (T)Convert.ChangeType(attributes.baseMaxHealth, typeof(T));
                case MovingEntityStats.takesDamage:
                    return (T)Convert.ChangeType(attributes.takesDamage, typeof(T));
                case MovingEntityStats.currMovementSpeed:
                    return (T)Convert.ChangeType(currMovementSpeed, typeof(T));
                case MovingEntityStats.baseMaxMovementSpeed:
                    return (T)Convert.ChangeType(attributes.baseMaxMovementSpeed, typeof(T));
                default:
                    return default;
            }
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "EntityStats", menuName = "Game Entities/Moving Entity", order = 1)]
    public class MovingEntityTemplate : BaseEntityTemplate
    {
        public float baseMaxMovementSpeed { get; private set; }

        public MovingEntityTemplate() : base()
        {
            baseMaxMovementSpeed = 5;
        }
    }

    public enum MovingEntityStats
    {
        currHealth,
        baseMaxHealth,
        takesDamage,
        currMovementSpeed,
        baseMaxMovementSpeed
    }
}
