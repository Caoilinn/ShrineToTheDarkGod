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
        public float Health
        {
            get
            {
                return health;
            }
        }

        public float Attack
        {
            get
            {
                return attack;
            }
        }

        public float Defence
        {
            get
            {
                return defence;
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
        ) : base(id, actorType, transform, effectParameters, model, width, height, depth, 0, 0, movementVector, rotationVector, moveSpeed, rotateSpeed) {
            this.health = health;
            this.attack = attack;
            this.defence = defence;
        }
        #endregion

        #region Methods
        public void TrackPlayer(GameTime gameTime)
        {
            Vector3 playerPosition = new Vector3(889, 381, 1143);
            Vector3 enemyPosition = this.Transform.Translation;

            Vector3 movementVector = new Vector3(254, 254, 254);
            Vector3 rotationVector = new Vector3(90, 90, 90);

            if (!this.InMotion)
            {
                //Forward, Back
                Vector3 adjacentCellAhead = enemyPosition + (movementVector * this.Transform.Look);
                Vector3 adjacentCellBehind = enemyPosition + (movementVector * -this.Transform.Look);

                //Left, Right
                Vector3 adjacentCellLeft = enemyPosition + (movementVector * -this.Transform.Right);
                Vector3 adjacentCellRight = enemyPosition + (movementVector * this.Transform.Right);

                if (!this.InMotion)
                {
                    ////Forward
                    if (Vector3.Distance(enemyPosition, playerPosition) > Vector3.Distance(adjacentCellAhead, playerPosition))
                    {
                        //Calculate target position, relative to the enemy
                        this.TargetPosition = (this.Transform.Look * movementVector);
                        this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Look);
                    }

                    ////Back
                    else if (Vector3.Distance(enemyPosition, playerPosition) > Vector3.Distance(adjacentCellBehind, playerPosition))
                    {
                        //Calculate target position, relative to the enemy
                        this.TargetPosition = -(this.Transform.Look * movementVector);
                        this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotateSpeed * this.Transform.Look);
                    }

                    //Left
                    if (Vector3.Distance(enemyPosition, playerPosition) > Vector3.Distance(adjacentCellLeft, playerPosition))
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
                }
            }
        }

        public void TakeDamage(float damage)
        {
            this.health -= damage;
        }

        public override void Update(GameTime gameTime)
        {
            TrackPlayer(gameTime);
            base.Update(gameTime);
        }
        #endregion
    }
}