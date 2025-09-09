namespace Assets._CrossyroadTest.Scripts.Common.Player
{
    public struct CollisionPlayerMessage
    {
        public CollisionPlayerMessage(TypeCollision collision)
        {
            TypeCollision = collision;
        }
        public TypeCollision TypeCollision { get; }
    }
}
