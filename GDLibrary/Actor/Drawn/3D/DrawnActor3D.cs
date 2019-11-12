/*
Function: 		Represents the parent class for all updateable AND drawn 3D game objects. Notice that Effect has been added.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class DrawnActor3D : Actor3D, ICloneable
    {
        #region Fields
        private EffectParameters effectParameters;
        #endregion

        #region Properties
        public EffectParameters EffectParameters
        {
            get
            {
                return this.effectParameters;
            }
            set
            {
                this.effectParameters = value;
            }
        }

        public float Alpha
        {
            get
            {
                return this.EffectParameters.Alpha;
            }
            set
            {
                //Opaque to transparent AND valid (i.e. 0 <= x < 1)
                if (this.EffectParameters.Alpha == 1 && value < 1)
                {
                    EventDispatcher.Publish(new EventData("OpTr", this, EventActionType.OnOpaqueToTransparent, EventCategoryType.Opacity));
                }

                //Transparent to opaque
                else if (this.EffectParameters.Alpha < 1 && value == 1)
                {
                    EventDispatcher.Publish(new EventData("TrOp", this, EventActionType.OnTransparentToOpaque, EventCategoryType.Opacity));
                }

                this.EffectParameters.Alpha = value;
            }
        }
        #endregion

        #region Constructors
        public DrawnActor3D(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters
        ) : this(id, actorType, StatusType.Drawn | StatusType.Update, transform, effectParameters) {
        }

        public DrawnActor3D(
            string id, 
            ActorType actorType, 
            StatusType statusType, 
            Transform3D transform, 
            EffectParameters effectParameters
        ) : base(id, actorType, statusType, transform) {
            this.effectParameters = effectParameters;
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (!(obj is DrawnActor3D other))
                return false;
            else if (this == other)
                return true;

            return this.effectParameters.Equals(other.EffectParameters) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = hash * 31 + this.effectParameters.GetHashCode();
            hash = hash * 43 + base.GetHashCode();
            return hash;
        }

        public new object Clone()
        {
            IActor actor = new DrawnActor3D(
                "Clone - " + ID,                                //Deep
                this.ActorType,                                 //Deep
                this.StatusType,                                //Deep - a simple numeric type
                (Transform3D)this.Transform.Clone(),            //Deep - calls the clone for Transform3D explicitly
                (EffectParameters)this.EffectParameters.Clone() //Hybrid - shallow (texture and effect) and deep (all other fields)
            );

            //Clone each of the (behavioural) controllers
            if (this.ControllerList != null)
                foreach (IController controller in this.ControllerList)
                    actor.AttachController((IController)controller.Clone());

            return actor;
        }

        public override bool Remove()
        {
            this.effectParameters = null;
            return base.Remove();
        }
        #endregion
    }
}