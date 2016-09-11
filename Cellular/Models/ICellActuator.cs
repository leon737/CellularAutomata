namespace Cellular.Models
{
    public interface ICellActuator
    {
        bool Actuate(ITrackingField trackingField);
        
        bool RecognizeMemo(ITrackingField trackingField, PatternMemo memo);

        bool TestActuate();
    }
}