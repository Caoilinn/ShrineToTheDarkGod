using Microsoft.Xna.Framework;
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
        #endregion

        #region Constructors
        public Enemy(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            float width,
            float height,
            float depth,
            Vector3 movementVector,
            Vector3 rotationVector,
            float moveSpeed,
            float rotateSpeed,
            float health,
            float attack,
            float defence
        ) : base(id, actorType, transform, effectParameters, model, width, height, depth, 0, 0, movementVector, rotationVector, moveSpeed, rotateSpeed, health, attack, defence) {

        }
        #endregion

        #region Methods
        public void TrackPlayer(GameTime gameTime, Transform3D playerPos)
        {
            //If it is not the enemy's turn, return
            if (!StateManager.enemyTurn) return;

            //If the enemy is in motion, return
            if (this.InMotion) return;

            #region Old Code
            Vector3 playerPosition = playerPos.Translation;
            Vector3 enemyPosition = this.Transform.Translation;

            Vector3 movementVector = new Vector3(254, 254, 254);
            Vector3 rotationVector = new Vector3(90, 90, 90);

            if (!this.InMotion)
            {
                //Forward, Back
                Vector3 adjacentCellAhead = enemyPosition + (movementVector * this.Transform.Right);
                Vector3 adjacentCellBehind = enemyPosition + (movementVector * -this.Transform.Right);

                //Left, Right
                Vector3 adjacentCellLeft = enemyPosition + (movementVector * this.Transform.Look);
                Vector3 adjacentCellRight = enemyPosition + (movementVector * -this.Transform.Look);

                if (!this.InMotion)
                {
                    //Forward
                    if (Vector3.Distance(enemyPosition, playerPosition) > Vector3.Distance(adjacentCellAhead, playerPosition))
                    {
                        //Calculate target position, relative to the enemy
                        this.TargetPosition = (this.Transform.Right * movementVector);
                        this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Right);
                    }

                    //Back
                    else if (Vector3.Distance(enemyPosition, playerPosition) > Vector3.Distance(adjacentCellBehind, playerPosition))
                    {
                        //Calculate target position, relative to the enemy
                        this.TargetPosition = -(this.Transform.Right * movementVector);
                        this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotateSpeed * this.Transform.Right);
                    }

                    //Left
                    else if (Vector3.Distance(enemyPosition, playerPosition) > Vector3.Distance(adjacentCellLeft, playerPosition))
                    {
                        this.TargetHeading = -(this.Transform.Up * rotationVector);
                        this.Rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotateSpeed * this.Transform.Up);
                    }

                    //Right
                    else if (Vector3.Distance(enemyPosition, playerPosition) > Vector3.Distance(adjacentCellRight, playerPosition))
                    {
                        this.TargetHeading = (this.Transform.Up * rotationVector);
                        this.Rotation = (gameTime.ElapsedGameTime.Milliseconds * this.RotateSpeed * this.Transform.Up);
                    }

                    //At required position
                    else
                    {
                        //Update game state
                        EventDispatcher.Publish(
                            new EventData(
                                EventActionType.PlayerTurn,
                                EventCategoryType.Game
                            )
                        );
                    }
                }
            }
            #endregion

            //Need to create a function that generates a translation and a rotation, based on the players position
            //Function should feed the HandleMovement method

            //Very simple test code
            //Vector3 currentPosition = this.Transform.Translation;
            //this.TargetPosition = new Vector3(254, 0, 0);
            //this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Right);
        }

        public void TakeDamage(float damage)
        {
            this.Health -= damage;
        }

        public override void HandleMovement()
        {
            if (!StateManager.enemyTurn) return;

            #region Translation
            if (this.Translation != Vector3.Zero)
            {
                //If the current positon is near the target position
                if (Vector3.Distance(this.CurrentPosition, this.TargetPosition) <= 10)
                {
                    //Move to the target position
                    this.Transform.TranslateBy((this.CurrentPosition - this.TargetPosition) * -Vector3.One);

                    //Update collision
                    this.CharacterBody.Position = this.Transform.Translation;
                    JigLibX.Math.Transform transform = this.Body.Transform;
                    this.Body.CollisionSkin.SetNewTransform(ref transform);

                    //Reset vectors
                    this.Translation = Vector3.Zero;
                    this.CurrentPosition = Vector3.Zero;

                    //Update motion state
                    this.InMotion = false;

                    //Update game state
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.PlayerTurn, 
                            EventCategoryType.Game
                        )
                    );
                }
                else
                {
                    //Translate actor
                    this.Transform.TranslateBy(this.Translation);

                    //Update current position
                    this.CurrentPosition += this.Translation;

                    //Update collision
                    this.CharacterBody.Position = this.Transform.Translation;
                    JigLibX.Math.Transform transform = this.Body.Transform;
                    this.Body.CollisionSkin.SetNewTransform(ref transform);

                    //Update motion state
                    this.InMotion = true;
                }
            }
            #endregion

            #region Rotation
            if (this.Rotation != Vector3.Zero)
            {
                //If the current heading is near the target heading
                if (Vector3.Distance(this.CurrentHeading, this.TargetHeading) <= 5)
                {
                    //Rotate to the target heading
                    this.Transform.RotateBy((this.CurrentHeading - this.TargetHeading) * -Vector3.One);

                    //Reset vectors
                    this.Rotation = Vector3.Zero;
                    this.CurrentHeading = Vector3.Zero;

                    //Update motion state
                    this.InMotion = false;
                }
                else
                {
                    //Rotate actor
                    this.Transform.RotateBy(this.Rotation);

                    //Update current heading
                    this.CurrentHeading += this.Rotation;

                    //Update motion state
                    this.InMotion = true;
                }
            }
            #endregion
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion
    }
}