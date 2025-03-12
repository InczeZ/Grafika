namespace Szeminarium1_24_02_17_2
{
    internal class CubeArrangementModel
    {
        private double Time { get; set; } = 0;

        /// <summary>
        /// The value by which the center cube is scaled. It varies between 0.8 and 1.2 with respect to the original size.
        /// </summary>
        public double CubeRotation { get; private set; } = 0;

        /// <summary>
        /// The angle with which the diamond cube is rotated around the diagonal from bottom right front to top left back.
        /// </summary>

        private double targetRotation = 0;

        internal void RotateLeft()
        {
            targetRotation += Math.PI / 2;
        }

        internal void RotateRight()
        {
            targetRotation -= Math.PI / 2;
        }

        internal void AdvanceTime(double deltaTime)
        {

            Time += deltaTime;
            if (CubeRotation < targetRotation)
            {
                CubeRotation += Math.PI * deltaTime;
                if (CubeRotation >= targetRotation)
                {
                    CubeRotation = targetRotation;
                }
            }

            if (CubeRotation > targetRotation)
            {
                CubeRotation -= Math.PI * deltaTime;
                if (CubeRotation <= targetRotation)
                {
                    CubeRotation = targetRotation;
                }
            }
        }
    }
}
