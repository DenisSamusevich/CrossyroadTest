namespace Assets._CrossyroadTest.Scripts.Services.EntityService.Interfaces
{
    public interface IEntityModel<out TData>
    {
        public TData EntityData { get; }
    }
    public interface IEntityModel<out TData, out TBehavior>
    {
        public TData EntityData { get; }
        public TBehavior EntityBehavior { get; }
    }
}
