using Xenko.Engine;

namespace MarchingCubesImproved.Windows
{
    class MarchingCubesImprovedApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
