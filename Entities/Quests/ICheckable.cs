namespace Grimore.Entities.Quests;

interface ICheckable<in T>
{
    bool Check(T against);
    bool Met { get; set; }
}