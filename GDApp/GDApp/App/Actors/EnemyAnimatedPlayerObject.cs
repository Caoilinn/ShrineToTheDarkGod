using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using JigLibX.Collision;

namespace GDApp
{
    public class EnemyAnimatedPlayerObject : AnimatedPlayerObject
    {
        private float moveSpeed, rotationSpeed;
        private readonly float DefaultMinimumMoveVelocity = 1;


        public EnemyAnimatedPlayerObject(
            string id, 
            ActorType actorType, 
            Transform3D transform,
            EffectParameters effectParameters, 
            Keys[] moveKeys, 
            float radius, 
            float height,
            float accelerationRate, 
            float decelerationRate,
            float moveSpeed, 
            float rotationSpeed,
            Vector3 translationOffset,
            KeyboardManager keyboardManager)

            : base(id, actorType, transform, effectParameters, moveKeys, radius, height,
                  accelerationRate, decelerationRate, translationOffset, keyboardManager)
        {
            //add extra constructor parameters like health, inventory etc...
            this.moveSpeed = moveSpeed;
            this.rotationSpeed = rotationSpeed;

            //register for callback on CDCR
            this.CharacterBody.CollisionSkin.callbackFn += CollisionSkin_callbackFn;
        }


        //this methods defines how your player interacts with ALL collidable objects in the world - its really the players complete behaviour
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
