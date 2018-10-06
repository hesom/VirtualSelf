using UnityEngine;

namespace VirtualSelf.Ballmaze
{
    public class BallConstraint : MonoBehaviour
    {

        public GameObject Maze;
        
        private Rigidbody mazeRb;
        private Rigidbody rb;

        private Vector3 lastLocalPosition;
        // die drei werte müssen hier bei scalierfactor 1 noch genauer bestimmt werden, dann private machen
        // sollte iwie nen gewisser spielraum drin sein von wenigen float nachkommastellen
        // ebenso dann mit dem model scalieren in der Start methode
        public float Valid2DRadius;
        public float MinYPosition;
        public float MaxYPosition;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            lastLocalPosition = Maze.transform.worldToLocalMatrix.MultiplyPoint3x4(rb.position);
            if (lastLocalPosition.y < MinYPosition
                || lastLocalPosition.y > MaxYPosition
                || new Vector2(lastLocalPosition.x, lastLocalPosition.z).sqrMagnitude > Valid2DRadius * Valid2DRadius)
                throw new UnityException(
                    "You positioned the ball wrong inside the maze!/n" +
                    "Y-Position: " + lastLocalPosition.y +
                    ", Radius: " + new Vector2(lastLocalPosition.x, lastLocalPosition.z).magnitude);
            mazeRb = Maze.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var localPosition = mazeRb.transform.worldToLocalMatrix
                .MultiplyPoint3x4(rb.position);
            bool wasAdjusted = false;

            if (localPosition.y < MinYPosition || localPosition.y > MaxYPosition)
            {
                Debug.Log("Y Collision: " + localPosition.y);
                localPosition = CorrectYPosition(localPosition);
                wasAdjusted = true;
            }

            var local2DPos = new Vector2(localPosition.x, localPosition.z);

            if (local2DPos.sqrMagnitude > Valid2DRadius * Valid2DRadius)
            {
                Debug.Log("Radius Collision." + local2DPos.magnitude);
                localPosition = CorrectDistanceToCenter(localPosition);
                wasAdjusted = true;
            }

            lastLocalPosition = localPosition;

            if (wasAdjusted)
            {
                // TODO adjust force of rigidbody somewhat. maybe reflect instead zero? dunno test
                rb.position = Maze.transform.localToWorldMatrix.MultiplyPoint3x4(localPosition);
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    
        private Vector3 CorrectYPosition(Vector3 localPosition)
        {
            // Check to which y position we have to cap
            float yConstrain = localPosition.y > MaxYPosition ? MaxYPosition : MinYPosition;
            var lineDirection = localPosition - lastLocalPosition;
            if (lineDirection.y < 0.000001f)
                return new Vector3(localPosition.x, yConstrain, localPosition.z);

            float alpha = (yConstrain - localPosition.y) / lineDirection.y;
            return lastLocalPosition + lineDirection * alpha;
        }

        /// <summary>
        /// Corrects the distance to center along the direction to lastLocalPosition.
        ///
        /// Project the direction vector onto the normalized local2DPos.
        /// This gives us the scalar projection, which defines how much distance in direction
        /// of local2DPos do i go for every lineDir2D vector addition.
        /// Then we calculate the overlapping distance to know how much we overshoot the radius.
        /// After that simply divide the overlap distance with the scalar projection to get
        /// the alpha, which we have to multiply lineDirection with to bridge the overlap.
        /// Then the return is simply the localPos subtracted by the lineDirection times alpha.
        ///
        /// Note to alpha calculation:
        /// scalarProjection should never really be too near to zero.
        /// Only with minimalistic changes around the border this could go wrong i guess.
        /// If i test for minimalistic changes I could maybe
        /// </summary>
        /// <returns></returns>
        private Vector3 CorrectDistanceToCenter(Vector3 localPos)
        {
            var lineDirection = localPos - lastLocalPosition;
            var lineDir2D = new Vector2(lineDirection.x, lineDirection.z);
            var local2DPos = new Vector2(localPos.x, localPos.z);
            if (lineDir2D.sqrMagnitude < 0.000001f)
                return lastLocalPosition;
        
            float scalarProjection = Vector2.Dot(lineDir2D, local2DPos.normalized);
            float overlappingDistance = local2DPos.magnitude - Valid2DRadius;
            float alpha = overlappingDistance / scalarProjection;
            return localPos - alpha * lineDirection;
        }
    }
}
