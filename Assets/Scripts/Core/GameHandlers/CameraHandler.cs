using UnityEngine;

using UnitWarfare.Core;

namespace UnitWarfare.Cameras
{
    public class CameraHandler : GameHandler
    {
        private CameraController _mainCamera;
        public CameraController MainCamera => _mainCamera;

        private readonly CameraData _data;

        public CameraHandler(CameraData data, IGameStateHandler game_state_handler)
            : base(game_state_handler)
        {
            _data = data;
        }

        protected override void Initialize()
        {
            _mainCamera = new(new CameraController.CameraConfig(Camera.main,
                _data,
                gameStateHandler.GetHandler<Territories.TerritoryManager>().MapCenter,
                gameStateHandler.GetHandler<Input.InputHandler>().MoveInput,
                gameStateHandler.GetHandler<Input.InputHandler>().PintchInput));
        }
    }
}