namespace Assets._CrossyroadTest.Scripts.Games.Common.ValueType
{
    public struct CollisionPlayerMessage
    {
        public CollisionPlayerMessage(CollisionType collision)
        {
            TypeCollision = collision;
        }
        public CollisionType TypeCollision { get; }
    }
}
