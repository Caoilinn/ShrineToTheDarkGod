using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class Enemy : CharacterObject
    {

        #region Fields
        private float health;
        private float attack;
        private float defence;
        #endregion

        #region Properties
        public float Health
        {
            get {
                return health;
            }
        }

        public float Attack
        {
            get {
                return attack;
            }
        }

        public float Defence
        {
            get {
                return defence;
            }
        }
        #endregion

        public Enemy(
            string id, 
            ActorType actorType,
            StatusType statusType,
            Transform3D transform, 
            EffectParameters effectParameters,
            Model model, 
            float radius, 
            float height,
            float health, 
            float attack, 
            float defence
        ) :  base(id, actorType, statusType, transform, effectParameters, model, radius, height, 0, 0) {
            this.health = health;
            this.attack = attack;
            this.defence = defence;
        }

        public void takeDamage(float damage)
        {
            this.health -= damage;
        }
    }
}
