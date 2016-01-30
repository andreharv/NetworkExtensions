
namespace Transit.Framework.Texturing
{
    public partial class TextureBlender
    {
        private abstract class ComponentBase
        {
            protected readonly Point Position;
            protected readonly bool PositionRelativeFromPrevious;
            protected readonly bool IncreaseXOffset;
            protected readonly bool IncreaseYOffset;

            protected ComponentBase(
                Point position = null,
                bool positionRelativeFromPrevious = true,
                bool increaseXOffset = true,
                bool increaseYOffset = false)
            {
                Position = position ?? new Point(0, 0);
                PositionRelativeFromPrevious = positionRelativeFromPrevious;
                IncreaseXOffset = increaseXOffset;
                IncreaseYOffset = increaseYOffset;
            }
        }
    }
}
