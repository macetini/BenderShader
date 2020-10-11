using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.Curve.Meta
{
    /// <summary>
    /// The meta data for the <b>CurveWorld</b> shader. Every material that references this shader will have it's properties set by this class.
    /// </summary>    
    public class CurveShaderMeta
    {
        /// <summary>
        /// The value used to turn the integer bend amounts to decimal numbers. The amount vector passed to the shader is multiplied with this 
        /// number so that the values are not too high. Used so that the bend definition parameters are easily defined integer numbers.
        /// </summary>
        public static float BEND_AMOUNT_DILUTER = 0.1f;

        //Shader property IDs.
        private int bendAmountId;
        private int bendOriginId;
        private int bendAmountDiluterId;
        private int bendFalloffId;
        //

        public CurveShaderMeta()
        {
            BindProperties();
        }

        /// <summary>
        /// Bind the shader ids to structure global variables.
        /// </summary>
        protected void BindProperties()
        {
            bendAmountId = Shader.PropertyToID("_BendAmount");
            bendOriginId = Shader.PropertyToID("_BendOrigin");
            bendAmountDiluterId = Shader.PropertyToID("_BendAmountDiluter");
            bendFalloffId = Shader.PropertyToID("_BendFalloff");                        
        }

        /// <summary>
        /// Sets the curvature bend amount in XYZ plane along X and Y axis (up and down).
        /// </summary>
        /// <param name="bendAmount">Vector3 value that holds the amount of bend in X and Y variables.</param>
        /// <param name="origin">The top vertex of the curve parabola.</param>
        public void BendAroundOrigin(Vector3 bendAmount, Vector3 origin)
        {
            Shader.SetGlobalVector(bendAmountId, bendAmount);
            Shader.SetGlobalVector(bendOriginId, origin);

            Shader.SetGlobalFloat(bendAmountDiluterId, BEND_AMOUNT_DILUTER);
        }

        /// <summary>
        /// Sets the curve deformation cut off from the curve origin. The items are not deformed beyond this point.
        /// </summary>
        /// <param name="fallOff">Curve deformation cut off from the curve origin. The items are not deformed beyond this point.</param>
        public void SetFallOff(float fallOff)
        {
            Shader.SetGlobalFloat(bendFalloffId, fallOff);
        }
    }
}
