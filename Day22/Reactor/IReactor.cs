namespace Day22.Reactor;

public interface IReactor
{
    public long CountOn();

    public void Mark(Instruction instruction);
}