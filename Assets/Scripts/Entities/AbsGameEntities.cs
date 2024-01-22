using BaseEntities;
using MovableEntities;
using System;
using UnityEngine;

namespace AbstractGameEntities
{
    public abstract class GameEntity<E, A> : MonoBehaviour where E : IConvertible
                                                           where A : ScriptableObject, new()
    {
        [SerializeField]
        protected A attributes;

        private void Awake()
        {
            InitVars();
        }

        public abstract T GetStat<T>(E stats);

        protected virtual void InitVars()
        {
            if (attributes == null)
            {
                attributes = new A();
            }
        }
    }

    public abstract class DamageableEntity<E, A> : GameEntity<E, A> where E : IConvertible
                                                                    where A : BaseEntityTemplate, new()
    {
        public event Action onEntityDie;

        protected int currHealth;

        protected override void InitVars()
        {
            base.InitVars();
            currHealth = attributes.baseMaxHealth;
        }

        public virtual void TakeDamage(int healthChange)
        {
            if (attributes.takesDamage)
            {
                currHealth -= healthChange;
            }

            if (currHealth <= 0)
            {
                OnDeath();
            }
        }

        public virtual void OnDeath()
        {
            onEntityDie?.Invoke();
            Destroy(this);
        }
    } 

    public abstract class MovableEntity<E, A> : DamageableEntity<E, A> where E : IConvertible
                                                                       where A : MovingEntityTemplate, new()
    {
        protected float currMovementSpeed;

        protected override void InitVars()
        {
            base.InitVars();
            currMovementSpeed = attributes.baseMaxMovementSpeed;
        }
    }
}
