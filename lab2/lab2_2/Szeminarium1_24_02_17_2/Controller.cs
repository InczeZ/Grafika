using System;
using Silk.NET.Input;
using Silk.NET.Maths;


namespace Szeminarium1_24_02_17_2
{
    public class Controller
    {
        private Character character;

        public Controller(Character character)
        {
            this.character = character;
        }

        internal static void Update(object keyboard)
        {
            throw new NotImplementedException();
        }

        public void Update(IKeyboard keyboard)
        {
            float movementSpeed = character.MoveSpeed;

            if (keyboard.IsKeyPressed(Key.W))
            {
                // Mozgas előre
                character.Move(new Vector3D<float>(0, 0, -0.1f) * movementSpeed);
            }
            if (keyboard.IsKeyPressed(Key.S))
            {
                // Mozgas hátra
                character.Move(new Vector3D<float>(0, 0, 0.1f) * movementSpeed);
            }
            if (keyboard.IsKeyPressed(Key.A))
            {
                // Mozgas balra
                character.Move(new Vector3D<float>(-0.1f, 0, 0) * movementSpeed);
            }
            if (keyboard.IsKeyPressed(Key.D))
            {
                // Mozgas jobbra
                character.Move(new Vector3D<float>(0.1f, 0, 0) * movementSpeed);
            }
            if (keyboard.IsKeyPressed(Key.ShiftLeft))
            {
                // Mozgas fel
                character.Move(new Vector3D<float>(0, 0.1f, 0) * movementSpeed);
            }
            if (keyboard.IsKeyPressed(Key.ControlLeft))
            {
                // Mozgas le
                character.Move(new Vector3D<float>(0, -0.1f, 0) * movementSpeed);
            }

        }
    }

}