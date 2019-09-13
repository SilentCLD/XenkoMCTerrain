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
        public int chunkSize = 8;

        public int worldWidth = 4;
        public int worldHeight = 4;
        public int worldDepth = 4;

        public float isolevel = 0.5f;

        public int seed = 485972938;

        public Dictionary<Vector3, Chunk> chunks;

        private Material chunkMaterial;
        Stack<Entity> chunkPoolInactive = new Stack<Entity>();
        HashSet<Chunk> chunksToUpdate = new HashSet<Chunk>();
        private int chunksToUpdatePerFrame = 100;

        public Prefab PlayerPrefab;

        //private Bounds worldBounds;

        public DensityGenerator densityGenerator;

        public override async Task Execute()
        {
            // TODO This should be moved into a settings script
            (Game).Window.AllowUserResizing = true;
            
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
            player[0].Transform.Position = new Vector3(0.5f, 0.5f, -2f);
            player[0].Get<BasicCameraController>().world = this;
            SceneSystem.SceneInstance.RootScene.Entities.AddRange(player);
            //player[0].Transform.Rotation = new Quaternion(new Vector3(0, -180, 0), 1);

            chunkMaterial = Content.Load<Material>("Ground Material");
            densityGenerator = new DensityGenerator(seed);
            //worldBounds = new Bounds();
            UpdateBounds();

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
                chunk.GetOrCreate<Chunk>();
                Entity.AddChild(chunk);
                return chunk.Get<Chunk>();
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
            int newX = Utils.FloorToNearestX(x, chunkSize);
            int newY = Utils.FloorToNearestX(y, chunkSize);
            int newZ = Utils.FloorToNearestX(z, chunkSize);

            chunks.TryGetValue(new Vector3(newX, newY, newZ), out Chunk chunk);
            return chunk;
            //return chunks[new Vector3(newX, newY, newZ)];
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

        public void SetDensity(float density, int worldPosX, int worldPosY, int worldPosZ, bool setReadyForUpdate,
            Chunk[] initChunks)
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

        public void SetDensity(float density, Vector3 pos, bool setReadyForUpdate, Chunk[] initChunks)
        {
            SetDensity(density, (int) pos.X, (int) pos.Y, (int) pos.Z, setReadyForUpdate, initChunks);
        }

        private void UpdateBounds()
        {
            float middleX = worldWidth * chunkSize / 2f;
            float middleY = worldHeight * chunkSize / 2f;
            float middleZ = worldDepth * chunkSize / 2f;

            Vector3 midPos = new Vector3(middleX, middleY, middleZ);

            Vector3 size = new Vector3(
                worldWidth * chunkSize,
                worldHeight * chunkSize,
                worldDepth * chunkSize);

            //worldBounds.center = midPos;
            //worldBounds.size = size;
        }

        /*public bool IsPointInsideWorld(int x, int y, int z)
        {
            return IsPointInsideWorld(new Vector3(x, y, z));
        }

        public bool IsPointInsideWorld(Vector3 point)
        {
            return worldBounds.Contains(point);
        }*/

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