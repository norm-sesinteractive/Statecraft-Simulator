namespace Statecraft.Sim.Politics
{
    /// <summary>
    /// The political systems of politics v1 (Norm's list, D-0021). The order indexes the traits table and
    /// alignment arrays; never reorder. Meritocracy is held back (D-0023).
    /// </summary>
    public enum GovernmentType
    {
        Democracy = 0,
        Monarchy = 1,
        Communism = 2,
        MilitaryJunta = 3,
        Theocracy = 4,
    }
}
