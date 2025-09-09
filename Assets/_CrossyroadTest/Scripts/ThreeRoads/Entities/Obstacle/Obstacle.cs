namespace Assets._CrossyroadTest.Scripts.ThreeRoads.Entities.Obstacle
{
    internal class Obstacle : IObstacleData, IMoveObstacle
    {
        private readonly float _speed;

        public Obstacle(float speed, int roadPosition, float startPosition)
        {
            _speed = speed;
            Road = roadPosition;
            Position = startPosition;
        }

        public float Position { get; private set; }
        public int Road { get; private set; }

        public void Move(float deltaTime)
        {
            Position += deltaTime * _speed;
            if (Position > 1.5f)
            {
                Position = Position % 1.5f - 0.5f;
            }
        }
    }
}