using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Rendering;

namespace MarchingCubesImproved
{
    public class World : AsyncScript
    {
        private int chunkSize = 16;

        private int worldWidth = 4;
        private int worldHeight = 4;
        private int worldDepth = 4;

        public float isolevel = 0.5f;

        public int seed = 485972938;

        public Dictionary<Vector3, Chunk> chunks;

        private Material chunkMaterial;
        Stack<Entity> chunkPoolInactive = new Stack<Entity>();
        HashSet<Chunk> chunksToUpdate = new HashSet<Chunk>();

        public Prefab PlayerPrefab;

        public DensityGenerator densityGenerator;

        public override async Task Execute()
        {
            // TODO This should be moved into a settings script
            (Game).Window.AllowUserResizing = true;

            // Preload a few chunks
            for (int i = 0; i < 10; i++)
            {
                var chunkEntity = new Entity();
                chunkEntity.GetOrCreate<Chunk>();
                Entity.AddChild(chunkEntity);
                chunkEntity.EnableAll(false, true);
                chunkPoolInactive.Push(chunkEntity);
            }

            // Make the player and spawn it
            List<Entity> player = PlayerPrefab.Instantiate();
            player[0].Transform.Position = new Vector3(0f, 0f, 64f);
            player[0].Get<BasicCameraController>().world = this;
            var terrainEditorScript = player[0].GetOrCreate<EditTerrain>();
            terrainEditorScript.world = this;
            terrainEditorScript.camComp = player[0].Get<CameraComponent>();
            SceneSystem.SceneInstance.RootScene.Entities.AddRange(player);

            chunkMaterial = Content.Load<Material>("Ground Material");
            densityGenerator = new DensityGenerator(seed);

            chunks = new Dictionary<Vector3, Chunk>(worldWidth * worldHeight * worldDepth);
            CreateChunks();

            while (Game.IsRunning)
            {
                foreach (Chunk chunk in chunksToUpdate)
                {
                    chunk.Generate();
                }

                chunksToUpdate.Clear();

                await Script.NextFrame();
            }
        }

        Chunk GetChunkFromPool()
        {
            if (chunkPoolInactive.Count > 0)
            {
                var chunk = chunkPoolInactive.Pop().Get<Chunk>();
                chunk.Entity.EnableAll(true, true);
                return chunk;
            }
            else
            {
                var chunk = new Entity();
                Entity.AddChild(chunk);
                return chunk.GetOrCreate<Chunk>();
            }
        }

        private void CreateChunks()
        {
            for (int x = -worldWidth; x < worldWidth; x++)
            {
                for (int y = -worldHeight; y < worldHeight; y++)
                {
                    for (int z = -worldDepth; z < worldDepth; z++)
                    {
                        CreateChunk(x * chunkSize, y * chunkSize, z * chunkSize);
                    }
                }
            }
        }

        private Chunk GetChunk(Vector3 pos)
        {
            return GetChunk((int) pos.X, (int) pos.Y, (int) pos.Z);
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            int newX = MathHelpers.FloorToNearestX(x, chunkSize);
            int newY = MathHelpers.FloorToNearestX(y, chunkSize);
            int newZ = MathHelpers.FloorToNearestX(z, chunkSize);

            chunks.TryGetValue(new Vector3(newX, newY, newZ), out Chunk chunk);
            return chunk;
        }

        public float GetDensity(int x, int y, int z)
        {
            Point p = GetPoint(x, y, z);

            return p.density;
        }

        public float GetDensity(Vector3 pos)
        {
            return GetDensity((int) pos.X, (int) pos.Y, (int) pos.Z);
        }

        public Point GetPoint(int x, int y, int z)
        {
            Chunk chunk = GetChunk(x, y, z);
            if (chunk == null)
                return new Point(Vector3.Zero, 0);

            Point p = chunk.GetPoint(x.Mod(chunkSize),
                y.Mod(chunkSize),
                z.Mod(chunkSize));

            return p;
        }

        public void SetDensity(float density, int worldPosX, int worldPosY, int worldPosZ, bool setReadyForUpdate)
        {
            Vector3 dp = new Vector3(worldPosX, worldPosY, worldPosZ);

            Vector3 lastChunkPos = dp.FloorToNearestX(chunkSize);

            for (int i = 0; i < 8; i++)
            {
                Vector3 chunkPos = (dp - MarchingCubes.CubePoints[i]).FloorToNearestX(chunkSize);

                if (i != 0 && chunkPos == lastChunkPos)
                {
                    continue;
                }

                Chunk chunk = GetChunk(chunkPos);
                if (chunk == null)
                    return;

                lastChunkPos = chunk.position;

                Vector3 localPos = (dp - chunk.position).Mod(chunkSize + 1);

                chunk.SetDensity(density, localPos);

                if (setReadyForUpdate)
                    chunksToUpdate.Add(chunk);
            }
        }

        public void SetDensity(float density, Vector3 pos, bool setReadyForUpdate)
        {
            SetDensity(density, (int) pos.X, (int) pos.Y, (int) pos.Z, setReadyForUpdate);
        }

        private void CreateChunk(int x, int y, int z)
        {
            Vector3 position = new Vector3(x, y, z);

            Chunk newChunk = GetChunkFromPool();

            newChunk.Entity.Transform.Position = position;
            newChunk.material = chunkMaterial;
            newChunk.Initialize(this, chunkSize, position);

            chunks.Add(position, newChunk);
        }
    }
}