using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JigLibX.Collision;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    class InteractableGate : MoveablePickupObject
    {
        public InteractableGate(
            string id, 
            ActorType actorType, 
            Transform3D transform, 
            EffectParameters effectParameters, 
            Model model, 
            PickupParameters pickupParameters
        ) : base(id, actorType, transform, effectParameters, model, pickupParameters)
        {
        }

        public void OpenGate()
        {
            //Transform the gate for an open
            //Transform.TranslateBy();

            Console.WriteLine("Open Gate - GATE");
        }
    }
}