using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class MathUtility
    {
        #region Round
        public static Vector2 Round(Vector2 a, int precision)
        {
            //Takes two translations x1,y1 and x2,y2 and interpolates linearly between them using a factor    
            return new Vector2(
                (float) Math.Round(a.X, precision), 
                (float) Math.Round(a.Y, precision)
            );
        }

        public static Vector3 Round(Vector3 a, int precision)
        {
            //Takes two translations x1,y1 and x2,y2 and interpolates linearly between them using a factor    
            return new Vector3(
                (float)Math.Round(a.X, precision),
                (float)Math.Round(a.Y, precision),
                (float)Math.Round(a.Z, precision)
            );
        }
        #endregion

        #region Lerp
        public static Vector2 Lerp(Vector2 a, Vector2 b, float lerpFactor)
        {
            //Takes two translations x1,y1 and x2,y2 and interpolates linearly between them using a factor    
            return new Vector2(
                MathHelper.Lerp(a.X, b.X, lerpFactor), 
                MathHelper.Lerp(a.Y, b.Y, lerpFactor)
            );
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float lerpFactor)
        {
            //Takes two translations x1,y1,z1 and x2,y2,z2 and interpolates linearly between them using a factor    
            return new Vector3(
                MathHelper.Lerp(a.X, b.X, lerpFactor),
                MathHelper.Lerp(a.Y, b.Y, lerpFactor), 
                MathHelper.Lerp(a.Z, b.Z, lerpFactor)
            );
        }

        public static Color Lerp(Color a, Color b, float lerpFactor)
        {
            //Lerp between R, G, B, and A channels for each color
            return new Color(
                (int)MathHelper.Lerp(a.R, b.R, lerpFactor),
                (int)MathHelper.Lerp(a.G, b.G, lerpFactor),
                (int)MathHelper.Lerp(a.B, b.B, lerpFactor),
                (int)MathHelper.Lerp(a.A, b.A, lerpFactor)
            );
        }

        public static float SineLerpByElapsedTime(TrigonometricParameters trigonometricParameters, float totalElapsedTime)
        {
            //Range: - Max Amplitude -> + Max Amplitude
            float lerpFactor = (float) (trigonometricParameters.MaxAmplitude
                * Math.Sin(trigonometricParameters.AngularFrequency
                * MathHelper.ToRadians(totalElapsedTime) + trigonometricParameters.PhaseAngle)
            );
            
            //Range 0 -> 2 * Max Amplitude
            lerpFactor += trigonometricParameters.MaxAmplitude;
            
            //Range 0 -> Max Amplitude
            lerpFactor /= 2.0f;

            return lerpFactor;
        }
        #endregion

        #region Trig
        //Lerps along a sine wave with properties defined by TrigonometricParameters
        //i.e. max amplitude, phase, speed (see UISineLerpController)
        public static float Sin(TrigonometricParameters trigonometricParameters, float totalElapsedTime)
        {
            //Range - max amplitude -> + max amplitude
            float sin = (float)(trigonometricParameters.MaxAmplitude
                * Math.Sin(trigonometricParameters.AngularFrequency
                * MathHelper.ToRadians(totalElapsedTime) + trigonometricParameters.PhaseAngle));

            //Range 0 -> 2* max amplitude
            sin += trigonometricParameters.MaxAmplitude;

            //Range 0 -> max amplitude
            sin /= 2.0f;

            return sin;
        }
        #endregion

        #region Camera Related
        public static float GetDistanceFromCamera(Actor3D actor, Camera3D activeCamera)
        {
            return Vector3.Distance(actor.Transform.Translation, activeCamera.Transform.Translation);
        }

        //Sets actor::distanceToCamera value
        //i.e. Sets how far object is from camera (used to sort semi-transparent objects by distance from camera)
        public static void SetDistanceFromCamera(Actor3D actor, Camera3D activeCamera)
        {
            actor.Transform.DistanceToCamera = Vector3.Distance(actor.Transform.Translation, activeCamera.Transform.Translation);
        }

        //Object to target vector, no distance
        public static Vector3 GetNormalizedObjectToTargetVector(Transform3D start, Transform3D target)
        {
            //Camera to target object vector
            return Vector3.Normalize(target.Translation - start.Translation);
        }

        //Object to target vector, also provides access to distance from object to target
        public static Vector3 GetNormalizedObjectToTargetVector(Transform3D start, Transform3D target, out float distance)
        {
            //Camera to target object vector
            Vector3 vectorToTarget = target.Translation - start.Translation;

            //Distance from camera to target
            distance = vectorToTarget.Length();

            //Camera to target object vector
            vectorToTarget.Normalize();

            return vectorToTarget;
        }
        #endregion
    }
}