/**
 * Author: Christopher Cola
 * Created on 04/11/2015
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mobo
{
    // This class is used to transform the positon of the cameraSpriteBatch to follow the player
    class Camera2D
    {
        public static Matrix transform; // Matrix Transform
        public static Vector2 cameraPos; // Camera Position

        // Get set position
        public static Vector2 Pos
        {
            get { return cameraPos; }
            set { cameraPos = value; }
        }

        public static Matrix getTransformation(GraphicsDevice graphicsDevice)
        {
            // Used to offset the camera slightly to look in the direction of where the crosshair is relative to the screen
            Vector2 mouseVector = ScreenManager.cursorPos - ScreenManager.screenCenter + new Vector2(16, 16);

            transform = Matrix.CreateTranslation(new Vector3(-cameraPos.X, -cameraPos.Y, 0)) *
                                         Matrix.CreateTranslation(new Vector3(ScreenManager.screenCenter.X - mouseVector.X/6, ScreenManager.screenCenter.Y - mouseVector.Y/6, 0));
            return transform;
        }
    }
}
