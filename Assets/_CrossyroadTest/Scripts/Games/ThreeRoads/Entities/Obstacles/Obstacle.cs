using Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles.Interfaces;

namespace Assets._CrossyroadTest.Scripts.Games.ThreeRoads.Entities.Obstacles
{
    public class Obstacle : IObstacleData, IMoveObstacle
    {
        private readonly float _speed;
        private float _position;
        private int _areaPosition;
        private float _maxPos;
        private float _minPos;

        public Obstacle(float speed, int roadPosition, float startPosition, int levelWidth)
        {
            _speed = speed;
            _areaPosition = roadPosition;
            _position = startPosition;
            _maxPos = levelWidth * 1.5f;
            _minPos = levelWidth * -0.5f;
        }

        float IObstacleData.Position { get => _position; }
        int IObstacleData.AreaPosition { get => _areaPosition; }

        void IMoveObstacle.Move(float deltaTime)
        {
            _position += deltaTime * _speed;
            if (_position > _maxPos)
            {
                _position = _position % _maxPos + _minPos;
            }
        }
    }
}
