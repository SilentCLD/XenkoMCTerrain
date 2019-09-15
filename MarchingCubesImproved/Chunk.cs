using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.Graphics;
using Xenko.Physics;
using Xenko.Rendering;

namespace MarchingCubesImproved
{
    public class Chunk : StartupScript
    {
        public Point[,,] points;
        public int chunkSize;
        public Vector3Int position;

        private VertexBufferBinding _vertexBufferBinding;
        private IndexBufferBinding _indexBufferBinding;
        private CommandList _commandList;

        public Material material;
        private Mesh mesh;
        ModelComponent modelComponent;
        StaticColliderComponent colliderComponent;


        private float _isolevel;
        private int _seed;

        private MarchingCubes _marchingCubes;
        private DensityGenerator _densityGenerator;

        private VertexPositionNormalTexture[] verts;
        private int[] tris;
        private Vector3[] colVerts;

        public void Initialize(World world, int chunkSize, Vector3Int position)
        {
            _commandList = Game.GraphicsContext.CommandList;

            this.chunkSize = chunkSize;
            this.position = position;
            _isolevel = world.isolevel;

            _densityGenerator = world.densityGenerator;

            points = new Point[chunkSize + 1, chunkSize + 1, chunkSize + 1];

            _seed = world.seed;
            _marchingCubes = new MarchingCubes(points, _isolevel, _seed);

            for (int x = 0; x < points.GetLength(0); x++)
            {
                for (int y = 0; y < points.GetLength(1); y++)
                {
                    for (int z = 0; z < points.GetLength(2); z++)
                    {
                        points[x, y, z] = new Point(
                            new Vector3(x, y, z),
                            //_densityGenerator.CalculateDensity(x + worldPosX, y + worldPosY, z + worldPosZ)
                            _densityGenerator.SphereDensity(x + position.X, y + position.Y, z + position.Z, 32)
                        );
                    }
                }
            }

            Generate();
        }

        public void Generate()
        {
            (verts, tris) = _marchingCubes.CreateMeshData(points);

            if ((verts == null || verts.Length == 0) && _vertexBufferBinding.Buffer != null)
            {
                _vertexBufferBinding.Buffer.Dispose();
                _indexBufferBinding.Buffer.Dispose();
                return;
            }

            if (verts == null || verts.Length == 0)
                return;

            // Copying the generated verts for the collider
            // TODO Could probably do this a better way..
            colVerts = new Vector3[verts.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                colVerts[i] = verts[i].Position;
            }

            if (mesh == null)
            {
                CreateMesh();
                return;
            }

            // Only remake the buffers if we're trying to put in more than it can handle
            if (_vertexBufferBinding.Buffer.IsDisposed || verts.Length >= _vertexBufferBinding.Buffer.ElementCount)
            {
                _vertexBufferBinding.Buffer.Dispose();
                _indexBufferBinding.Buffer.Dispose();
                Entity.Remove(modelComponent);
                Entity.Remove(colliderComponent);
                CreateMesh();
            }
            else
            {
                _vertexBufferBinding.Buffer.SetData(_commandList, verts);
                _indexBufferBinding.Buffer.SetData(_commandList, tris);
                mesh.Draw.DrawCount = tris.Length;
                Entity.Remove(colliderComponent);
                CreateCollider();
            }
        }

        private void CreateMesh()
        {
            var vbo = Xenko.Graphics.Buffer.Vertex.New(
                GraphicsDevice,
                verts,
                GraphicsResourceUsage.Dynamic
            );

            var ibo = Xenko.Graphics.Buffer.Index.New(
                GraphicsDevice,
                tris,
                GraphicsResourceUsage.Dynamic
            );

            _vertexBufferBinding =
                new VertexBufferBinding(vbo, VertexPositionNormalTexture.Layout, verts.Length);
            _indexBufferBinding = new IndexBufferBinding(ibo, is32Bit: true, count: tris.Length);
            mesh = new Mesh()
            {
                Draw = new MeshDraw()
                {
                    PrimitiveType = PrimitiveType.TriangleList,
                    VertexBuffers = new[]
                    {
                        _vertexBufferBinding
                    },
                    IndexBuffer = _indexBufferBinding,
                    DrawCount = tris.Length
                }
            };

            modelComponent = new ModelComponent()
            {
                Model = new Model()
                {
                    mesh,
                    material
                }
            };

            Entity.Add(modelComponent);

            // Bounding box for culling (stops rendering the mesh when the camera isn't looking at it)
            mesh.BoundingBox = MathHelpers.FromPoints(verts);
            mesh.BoundingSphere = BoundingSphere.FromBox(mesh.BoundingBox);
            CreateCollider();
        }

        void CreateCollider()
        {
            colliderComponent = new StaticColliderComponent();

            var shape = new StaticMeshColliderShape(colVerts, tris,
                Vector3.One);

            colliderComponent.ColliderShape = shape;
            colliderComponent.CanSleep = true;
            Entity.Add(colliderComponent);
        }

        public Point GetPoint(int x, int y, int z)
        {
            return points[x, y, z];
        }

        public void SetDensity(float density, int x, int y, int z)
        {
            points[x, y, z].density = density;
        }

        public void SetDensity(float density, Vector3Int pos)
        {
            SetDensity(density, pos.X, pos.Y, pos.Z);
        }
    }
}