namespace BEAM.Docking;

public interface IDockBase
{
    string Name { get; }
    public void OnClose();
}