namespace Day22.Model.Reactor;

public interface IReactor
{
    public long CountOn();

    public void Mark(Instruction instruction);
}