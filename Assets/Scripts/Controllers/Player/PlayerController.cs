using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.Player
{
    /// <summary>
    /// Player controller class. For now only thing it does is a small floating effect.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {        
        [Tooltip("Float position offset value along the Y axis.")]
        public float floatSpeed = 0.6f;
        
        [Tooltip("Interval duration in seconds. The float direction changes after the interval has run out.")]
        public float floatTime = 0.5f;

        /// <summary>
        /// The float direction flag. 
        /// </summary>
        private bool _floatUp = true;

        void Update()
        {
            if (_floatUp)
            {
                StartCoroutine(FloatingUp());
            }
            else if (!_floatUp)
            {
                StartCoroutine(FloatingDown());
            }
        }

        /// <summary>
        /// Coroutine which updates in time position along the Y axis UP.
        /// </summary>    
        IEnumerator FloatingUp()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + floatSpeed * Time.deltaTime, transform.position.z);

            yield return new WaitForSeconds(floatTime);

            _floatUp = false;
        }

        /// <summary>
        /// Coroutine which updates in time position along the Y axis DOWN.
        /// </summary>    
        IEnumerator FloatingDown()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - floatSpeed * Time.deltaTime, transform.position.z);

            yield return new WaitForSeconds(floatTime);

            _floatUp = true;
        }
    }
}