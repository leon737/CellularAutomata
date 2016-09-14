namespace Cellular.Models
{
    public interface ITrackingField
    {
        int GetNumberOfActiveCells();

        double GetPatternSimilarity(PatternMemo memo);

        PatternMemo TakeMemo(CellStates state);

        void SetBinaryField(CellStates[] binaryField, int width, int height);
    }
}