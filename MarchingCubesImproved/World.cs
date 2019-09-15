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
        private const int ChunkSize = 16;

        private const int WorldWidth = 4;
        private const int WorldHeight = 4;
        private const int WorldDepth = 4;

        public const float Isolevel = 0.5f;

        public const int Seed = 485972938;

        public Dictionary<Vector3, Chunk> Chunks;

        private Material _chunkMaterial;
        private Stack<Entity> _chunkPoolInactive = new Stack<Entity>();
        private HashSet<Chunk> _chunksToUpdate = new HashSet<Chunk>();

        public Prefab PlayerPrefab;

        public DensityGenerator DensityGenerator;

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
                _chunkPoolInactive.Push(chunkEntity);
            }

            // Make the player and spawn it
            List<Entity> player = PlayerPrefab.Instantiate();
            player[0].Transform.Position = new Vector3(0f, 0f, 64f);
            player[0].Get<BasicCameraController>().world = this;
            var terrainEditorScript = player[0].GetOrCreate<EditTerrain>();
            terrainEditorScript.World = this;
            terrainEditorScript.CamComp = player[0].Get<CameraComponent>();
            SceneSystem.SceneInstance.RootScene.Entities.AddRange(player);

            _chunkMaterial = Content.Load<Material>("Ground Material");
            DensityGenerator = new DensityGenerator(Seed);

            Chunks = new Dictionary<Vector3, Chunk>(WorldWidth * WorldHeight * WorldDepth);
            CreateChunks();

            while (Game.IsRunning)
            {
                foreach (Chunk chunk in _chunksToUpdate)
                {
                    chunk.Generate();
                }

                _chunksToUpdate.Clear();

                await Script.NextFrame();
            }
        }

        Chunk GetChunkFromPool()
        {
            if (_chunkPoolInactive.Count > 0)
            {
                var chunk = _chunkPoolInactive.Pop().Get<Chunk>();
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
            for (int x = -WorldWidth; x < WorldWidth; x++)
            {
                for (int y = -WorldHeight; y < WorldHeight; y++)
                {
                    for (int z = -WorldDepth; z < WorldDepth; z++)
                    {
                        CreateChunk(x * ChunkSize, y * ChunkSize, z * ChunkSize);
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
            int newX = MathHelpers.FloorToNearestX(x, ChunkSize);
            int newY = MathHelpers.FloorToNearestX(y, ChunkSize);
            int newZ = MathHelpers.FloorToNearestX(z, ChunkSize);

            Chunks.TryGetValue(new Vector3(newX, newY, newZ), out Chunk chunk);
            return chunk;
        }

        public float GetDensity(int x, int y, int z)
        {
            Point p = GetPoint(x, y, z);

            return p.Density;
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

            Point p = chunk.GetPoint(x.Mod(ChunkSize),
                y.Mod(ChunkSize),
                z.Mod(ChunkSize));

            return p;
        }

        public void SetDensity(float density, int worldPosX, int worldPosY, int worldPosZ, bool setReadyForUpdate)
        {
            Vector3Int dp = new Vector3Int(worldPosX, worldPosY, worldPosZ);

            Vector3 lastChunkPos = dp.FloorToNearestX(ChunkSize);

            for (int i = 0; i < 8; i++)
            {
                Vector3Int chunkPos = (dp - MarchingCubes.CubePoints[i]).FloorToNearestX(ChunkSize);

                if (i != 0 && chunkPos == lastChunkPos)
                {
                    continue;
                }

                Chunk chunk = GetChunk(chunkPos);
                if (chunk == null)
                    return;

                lastChunkPos = chunk.Position;

                Vector3Int localPos = (dp - chunk.Position).Mod(ChunkSize + 1);

                chunk.SetDensity(density, localPos);

                if (setReadyForUpdate)
                    _chunksToUpdate.Add(chunk);
            }
        }

        public void SetDensity(float density, Vector3 pos, bool setReadyForUpdate)
        {
            SetDensity(density, (int) pos.X, (int) pos.Y, (int) pos.Z, setReadyForUpdate);
        }

        private void CreateChunk(int x, int y, int z)
        {
            Vector3Int position = new Vector3Int(x, y, z);

            Chunk newChunk = GetChunkFromPool();

            newChunk.Entity.Transform.Position = position;
            newChunk.Material = _chunkMaterial;
            newChunk.Initialize(this, ChunkSize, position);

            Chunks.Add(position, newChunk);
        }
    }
}