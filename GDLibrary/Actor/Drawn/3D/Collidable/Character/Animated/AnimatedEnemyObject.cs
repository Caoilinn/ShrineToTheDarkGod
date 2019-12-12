using GDLibrary;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDApp
{
    public class AnimatedEnemyObject : AnimatedCharacterObject
    {
        private readonly float moveSpeed;
        private readonly float rotateSpeed;

        public AnimatedEnemyObject(
            string id,
            ActorType actorType,            
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            float accelerationRate,
            float decelerationRate,
            Vector3 movementVector,
            Vector3 rotationVector,
            float moveSpeed,
            float rotateSpeed,
            float health,
            float attack,
            float defence,
            ManagerParameters managerParameters
        ) : base (
            id,
            actorType,
            transform,
            effectParameters,
            model,
            accelerationRate,
            decelerationRate,
            movementVector,
            rotationVector,
            moveSpeed,
            rotateSpeed,
            health,
            attack,
            defence,
            managerParameters
        ) {
            //Add extra constructor parameters like health, inventory etc...
            this.moveSpeed = moveSpeed;
            this.rotateSpeed = rotateSpeed;
        }

        //This methods defines how your player interacts with ALL collidable objects in the world - its really the players complete behaviour
        private bool CollisionSkin_callbackFn(CollisionSkin collider, CollisionSkin collidee)
        {
            HandleCollisions(collider.Owner.ExternalData as CollidableObject, collidee.Owner.ExternalData as CollidableObject);
            return true;
        }

        //want do we want to do now that we have collided with an object?
        private void HandleCollisions(CollidableObject collidableObjectCollider, CollidableObject collidableObjectCollidee)
        {
            //did the "as" typecast return a valid object?
            if (collidableObjectCollidee != null)
            {
                if (collidableObjectCollidee.ActorType == ActorType.CollidablePickup)
                {
                    #region demo removed from nialls code
                    //do whatever you want here when you hit a collidable pickup...
                    #endregion
                }
                //add else if statements here for all the responses that you want your player to have
               // else if (collidableObjectCollidee.ActorType == ActorType.CollidableDoor)
               // {
               //
               // }
               // else if (collidableObjectCollidee.ActorType == ActorType.CollidableAmmo)
               // {
               //
               // }
            }
        }

        protected override void SetAnimationByInput()
        {
            switch (this.AnimationState)
            {
                case AnimationStateType.Attacking:
                    //call SetAnimation() with your Running FBX file name and Take 001
                    SetAnimation("Take 001", "dude");
                    break;

                case AnimationStateType.Defending:
                    SetAnimation("Take 001", "dude");
                    break;

                case AnimationStateType.Idle:
                    //call SetAnimation() with your Jumping FBX file name and Take 001
                    break;

            }
        }
    }
}