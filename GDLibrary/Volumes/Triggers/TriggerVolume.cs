namespace GDLibrary
{
    public class TriggerVolume
    {
        #region Fields
        private float x;
        private float y;
        private float z;
        private float width;
        private float height;
        private float depth;
        private TriggerType triggerType;

        private object[] additionalParameters;
        private bool hasFired;
        #endregion

        #region Properties
        public float X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public float Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        public float Z
        {
            get
            {
                return this.z;
            }
            set
            {
                this.z = value;
            }
        }

        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        public float Depth
        {
            get
            {
                return this.depth;
            }
            set
            {
                this.depth = value;
            }
        }

        public TriggerType TriggerType
        {
            get
            {
                return this.triggerType;
            }
        }

        public object[] AdditionalParameters
        {
            get
            {
                return this.additionalParameters;
            }
            set
            {
                this.additionalParameters = value;
            }
        }

        public bool HasFired
        {
            get
            {
                return this.hasFired;
            }
            set
            {
                this.hasFired = value;
            }
        }
        #endregion

        #region Constructor
        public TriggerVolume(
            float x, 
            float y, 
            float z,
            float width,
            float height, 
            float depth,
            TriggerType triggerType,
            object[] additionalParameters
        ) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.triggerType = triggerType;
            this.additionalParameters = additionalParameters;
            this.hasFired = false;
        }
        #endregion

        #region Methods
        public bool isActorWithin(Actor3D actor)
        {
            float minX = this.x;
            float minY = this.y;
            float minZ = this.z;
            float maxX = (this.x + (this.width));
            float maxY = (this.y + (this.height));
            float maxZ = (this.z + (this.depth));

            return (actor.Transform.Translation.X >= minX && actor.Transform.Translation.X <= maxX)
                && (actor.Transform.Translation.Y >= minY && actor.Transform.Translation.Y <= maxY)
                && (actor.Transform.Translation.Z >= minZ && actor.Transform.Translation.Z <= maxZ);
            
            //return ((actor.Transform.Translation.X >= this.x && actor.Transform.Translation.X <= (this.x + (this.width)))
            //    && (actor.Transform.Translation.Y >= this.y && actor.Transform.Translation.Y <= (this.y + (this.height)))
            //    && (actor.Transform.Translation.Z >= this.z && actor.Transform.Translation.Z <= (this.z + (this.depth))));
        }

        public void Test()
        {
            switch (triggerType)
            {
                case TriggerType.PlaySound:
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            this.additionalParameters
                        )
                    );
                    break;

                case TriggerType.PlayAnimation:
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Video,
                            this.additionalParameters
                        )
                    );
                    break;

                case TriggerType.EndLevel:
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            this.additionalParameters
                        )
                    );

                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPause,
                            EventCategoryType.Screen
                        )
                    );
                    break;

                case TriggerType.InitiateBattle:
                    //To Do
                    break;

                case TriggerType.DisplayToast:
                    //To Do
                    break;

                case TriggerType.PickupItem:
                    //To Do
                    break;

                case TriggerType.ActivateTrap:
                    //To Do
                    break;

                case TriggerType.ActivateEnemy:
                    //To Do
                    break;
            }
        }
        #endregion
    }
}