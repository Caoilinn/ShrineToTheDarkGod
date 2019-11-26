using System;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class InteractableGate : MoveablePickupObject
    {
        public InteractableGate(
            string id, 
            ActorType actorType, 
            Transform3D transform, 
            EffectParameters effectParameters, 
            Model model, 
            PickupParameters pickupParameters
        ) : base(id, actorType, transform, effectParameters, model, pickupParameters) {
        }

        public void OpenGate()
        {
            //Transform the gate for an open
            //Transform.TranslateBy();

            Console.WriteLine("Open Gate - GATE");
        }
    }
}