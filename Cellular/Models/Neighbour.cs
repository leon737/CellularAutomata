namespace Cellular.Models
{
    public class Neighbour
    {
        public Neighbour(int dx, int dy)
        {
            Dx = dx;
            Dy = dy;
        }

        public int Dx { get; }

        public int Dy { get; }
    }
}