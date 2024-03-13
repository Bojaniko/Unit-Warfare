using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Core.Global;

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
                gameStateHandler.GetHandler<Territories.TerritoryManager>(),
                gameStateHandler.GetHandler<Input.InputHandler>().MoveInput,
                gameStateHandler.GetHandler<Input.InputHandler>().PintchInput,
                gameStateHandler));
        }

        protected override void OnPostLoad()
        {
            _mainCamera.Initialize();
        }

        protected override void Enable()
        {
            if (_mainCamera != null)
                _mainCamera.Enable();
        }

        protected override void Disable()
        {
            if (_mainCamera != null)
                _mainCamera.Disable();
        }
    }
}