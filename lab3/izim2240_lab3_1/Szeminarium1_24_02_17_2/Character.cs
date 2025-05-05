using Silk.NET.Maths;

namespace Szeminarium1_24_02_17_2
{
    public class Character
    {
        private Vector3D<float> position = new Vector3D<float>(5, 0, 3);
        private Vector3D<float> rotation = new Vector3D<float>(0, 0, 0);

        public Vector3D<float> Position
        {
            get
            {
                return position;
            }
        }
        public Vector3D<float> Rotation
        {
            get
            {
                return rotation;
            }
        }

        public float MoveSpeed { get; set; } = 5.0f;

        public void Move(Vector3D<float> movement)
        {
            position += movement * MoveSpeed;
        }

        public void Rotate(Vector3D<float> rotation)
        {
            this.rotation += rotation;
        }
    }

}
