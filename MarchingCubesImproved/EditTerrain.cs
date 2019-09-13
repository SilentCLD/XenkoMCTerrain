using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.Input;
using Xenko.Physics;

namespace MarchingCubesImproved
{
    public class EditTerrain : AsyncScript
    {
        public CameraComponent camComp;
        public World world;

        private int _range = 3;
        private float _force = 0.1f;

        public override async Task Execute()
        {
            while (Game.IsRunning)
            {
                ProcessInput();

                await Script.NextFrame();
            }
        }

        private void ProcessInput()
        {
            if (Input.HasKeyboard && Input.HasMouse)
            {
                if (Input.IsMouseButtonDown(MouseButton.Left) && !Input.IsKeyDown(Keys.LeftCtrl))
                {
                    RaycastToTerrain(false);
                }
                else if (Input.IsMouseButtonDown(MouseButton.Left) && Input.IsKeyDown(Keys.LeftCtrl))
                {
                    RaycastToTerrain(true);
                }
            }
        }

        private void RaycastToTerrain(bool addTerrain)
        {
            var result =
                MathHelpers.ScreenPositionToWorldPositionRaycast(Input.MousePosition, camComp, this.GetSimulation());
            if (result.Succeeded)
            {
                if (result.Collider.Entity != null)
                {
                    Chunk chunk = result.Collider.Entity.Get<Chunk>();
                    if (chunk == null)
                        return;

                    ModifyTerrain(result.Point, addTerrain, _force, _range);
                }
            }
        }

        private void ModifyTerrain(Vector3 point, bool addTerrain, float force, float range)
        {
            int buildModifier = addTerrain ? 1 : -1;

            int hitX = point.X.Round();
            int hitY = point.Y.Round();
            int hitZ = point.Z.Round();
            
            for (int x = -_range; x <= _range; x++)
            {
                for (int y = -_range; y <= _range; y++)
                {
                    for (int z = -_range; z <= _range; z++)
                    {
                        int offsetX = hitX - x;
                        int offsetY = hitY - y;
                        int offsetZ = hitZ - z;

                        float distance = MathHelpers.Distance(offsetX, offsetY, offsetZ, point);
                        if (!(distance <= range)) continue;

                        float modificationAmount = force / distance * 0.5f * buildModifier;

                        float oldDensity = world.GetDensity(offsetX, offsetY, offsetZ);
                        float newDensity = oldDensity - modificationAmount;

                        newDensity = newDensity.Clamp01();

                        world.SetDensity(newDensity, offsetX, offsetY, offsetZ, true);
                    }
                }
            }
        }
    }
}