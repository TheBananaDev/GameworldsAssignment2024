using AbstractGameEntities;
using System;
using UnityEngine;

namespace BaseEntities
{
    public class BasicEntity : DamageableEntity<BaseEntityStats, BaseEntityTemplate>
    {
        public override T GetStat<T>(BaseEntityStats stats)
        {
            switch (stats)
            {
                case BaseEntityStats.currHealth:
                    return (T)Convert.ChangeType(currHealth, typeof(T));
                case BaseEntityStats.baseMaxHealth:
                    return (T)Convert.ChangeType(attributes.baseMaxHealth, typeof(T));
                case BaseEntityStats.takesDamage:
                    return (T)Convert.ChangeType(attributes.takesDamage, typeof(T));
                default:
                    return default;
            }
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "EntityStats", menuName = "Game Entities/Generic Entity", order = 1)]
    public class BaseEntityTemplate : ScriptableObject
    {
        public int baseMaxHealth { get; private set; }
        public bool takesDamage { get; private set; }

        public BaseEntityTemplate()
        {
            baseMaxHealth = 10;
            takesDamage = true;
        }
    }

    public enum BaseEntityStats
    {
        currHealth,
        baseMaxHealth,
        takesDamage
    }
}
