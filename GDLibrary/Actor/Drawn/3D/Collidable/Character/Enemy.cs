using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class Enemy : PlayerObject
    {
        #region Fields
        private bool completeTurn;
        #endregion

        #region Properties
        public bool CompleteTurn
        {
            get
            {
                return this.completeTurn;
            }
            set
            {
                this.completeTurn = value;
            }
        }
        #endregion

        #region Constructors
        public Enemy(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            float radius,
            float height,
            float accelerationRate,
            float decelerationRate,
            Vector3 movementVector,
            Vector3 rotationVector,
            float moveSpeed,
            float rotateSpeed,
            float health,
            float attack,
            float defence,
            Keys[] moveKeys,
            Vector3 translationOffset,
            KeyboardManager keyboardManager,
            float jumpHeight
        ) : base(id, actorType, transform, effectParameters, model, radius, height, accelerationRate, decelerationRate, movementVector, rotationVector, moveSpeed, rotateSpeed, health, attack, defence, moveKeys, translationOffset, keyboardManager, jumpHeight) {    
        }
        #endregion

        #region Methods
        public void TrackPlayer(GameTime gameTime, Transform3D playerPos)
        {
            //If it is not the enemy's turn, return
            if (!StateManager.enemyTurn) return;

            //If the enemy is in combat, return
            if (CombatManager.inCombat) return;

            //If the enemy is in motion, return
            if (this.InMotion) return;

            #region Movement Algorithm
            Vector3 playerPosition = playerPos.Translation;
            Vector3 enemyPosition = this.Transform.Translation;

            Vector3 movementVector = new Vector3(254, 254, 254);
            Vector3 rotationVector = new Vector3(90, 90, 90);

            if (!this.InMotion)
            {
                //Forward, Back
                Vector3 adjacentCellRight = enemyPosition + (movementVector * this.Transform.Look);
                Vector3 adjacentCellLeft = enemyPosition + (movementVector * -this.Transform.Look);

                //Left, Right
                Vector3 adjacentCellAhead = enemyPosition + (movementVector * this.Transform.Right);
                Vector3 adjacentCellBehind = enemyPosition + (movementVector * -this.Transform.Right);

                if (!this.InMotion)
                {
                    //If infront of the player
                    if (Vector3.Distance(enemyPosition, playerPosition) == Vector3.Distance(new Vector3(0, 0, 0), new Vector3(1, 0, 0)))
                    {

                    }
                    
                    //Forward
                    else if (Vector3.Distance(enemyPosition, playerPosition) > Vector3.Distance(adjacentCellAhead, playerPosition))
                    {
                        //Calculate target position, relative to the enemy
                        this.TargetPosition = (this.Transform.Right * movementVector);
                        this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Right);
                    }

                    //Turn Around
                    else if (Vector3.Distance(enemyPosition, playerPosition) > Vector3.Distance(adjacentCellBehind, playerPosition))
                    {
                        //Calculate target position, relative to the enemy
                        this.TargetHeading = -(this.Transform.Up * (2 * rotationVector));
                        this.Rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotateSpeed * this.Transform.Up);
                    }

                    //Turn Left
                    else if (Vector3.Distance(enemyPosition, playerPosition) > Vector3.Distance(adjacentCellLeft, playerPosition))
                    {
                        this.TargetHeading = -(this.Transform.Up * rotationVector);
                        this.Rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotateSpeed * this.Transform.Up);
                    }

                    //Turn Right
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

                    //Reset vectors
                    this.Translation = Vector3.Zero;
                    this.CurrentPosition = Vector3.Zero;

                    //Update motion state
                    this.InMotion = false;

                    //Update game state
                    EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));
                }
                else
                {
                    //Translate actor
                    this.Transform.TranslateBy(this.Translation);

                    //Update current position
                    this.CurrentPosition += this.Translation;

                    //Update collision
                    this.CharacterBody.Position = this.Transform.Translation;

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