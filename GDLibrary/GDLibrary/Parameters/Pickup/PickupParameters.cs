/*
Function: 		Encapsulates the parameters for a collectable collidable object (e.g. "ammo", 10)
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/

namespace GDLibrary
{
    public enum PickupType : sbyte
    {
        Health,
        Ammo,
        Key,
        Inventory,
        Sword
    }

    public class PickupParameters
    {
        #region Fields
        private string description;
        private float value;
        private PickupType pickupType;

        //An optional array to store multiple parameters (used for play with sound/video when we pickup this object)
        private object[] additionalParameters;
        #endregion

        #region Properties
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = (value.Length != 0) ? value : "no description specified";
            }
        }

        public float Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (value >= 0) ? value : 0;
            }
        }

        public PickupType PickupType
        {
            get
            {
                return this.pickupType;
            }
            set
            {
                this.pickupType = value;
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
        #endregion

        #region Constructors
        public PickupParameters(
            string description, 
            float value, 
            PickupType pickupType
        ) : this(description, value, pickupType, null) {

        }

        public PickupParameters(
            string description, 
            float value, 
            PickupType pickupType, 
            object[] additionalParameters
        ) {
            this.value = value;
            this.description = description;
            this.pickupType = pickupType;
            this.additionalParameters = additionalParameters;
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            PickupParameters other = obj as PickupParameters;
            bool bEquals = this.description.Equals(other.Description)
                && this.value == other.Value
                && this.pickupType == other.PickupType;

            return bEquals && ((this.additionalParameters != null && this.additionalParameters.Length != 0) ? this.additionalParameters.Equals(other.additionalParameters) : true);
        }

        public override int GetHashCode()
        {
            int hash = 11 + this.description.GetHashCode();
            hash = hash * 17 + this.value.GetHashCode();
            hash = hash * 47 + this.pickupType.GetHashCode();

            if (this.additionalParameters != null && this.additionalParameters.Length != 0)
                hash = hash * 31 + this.additionalParameters.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return "Desc.:" + this.description + ", Value: " + this.value + ", Type: " + this.pickupType;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
