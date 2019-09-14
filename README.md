# **Xenko Marching Cubes Terrain**

This is an example project for Marching Cubes terrain in the [Xenko Engine](https://www.xenko.com) built on version 3.1.0.1-beta2-0674 

The texturing method isn't the greatest (currently looping through 3 UV coordinates), a better method would be necessary for real world usage (triplanar mapping or something like that (PRs welcome!)).  

The original implementation is written by Eldemarkki which is available [here](https://github.com/Eldemarkki/Marching-Cubes-Improved).

#### Usage
1) Download / Clone the repo
2) Open the .sln and run

There are a couple of different terrain generation types (found in [DensityGenerator.cs](MarchingCubesImproved/TerrainGen/DensityGenerator.cs)) which can be changed on Line 68 in [Chunk.cs](MarchingCubesImproved/Chunk.cs).