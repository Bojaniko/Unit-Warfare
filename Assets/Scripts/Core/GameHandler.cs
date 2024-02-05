using System.Collections;

using UnityEngine;

using UnitWarfare.Core.Enums;

using Studio28.Utility;

namespace UnitWarfare.Core
{
    public abstract class GameHandler
    {
        protected readonly IGameStateHandler gameStateHandler;

        protected GameHandler(IGameStateHandler game_state_handler)
        {
            GameObject go = new($"GAME_HANDLER: {GetType().Name.ToUpper()}");
            monoBehaviour = new(go);
            monoBehaviour.OnUpdate += OnUpdate;

            gameStateHandler = game_state_handler;
            gameStateHandler.OnLoadGameStateChanged += HandleLoadGameState;
        }

        protected GameHandler(IGameStateHandler game_state_handler, GameObject game_object)
        {
            monoBehaviour = new(game_object);
            monoBehaviour.OnUpdate += OnUpdate;

            gameStateHandler = game_state_handler;
            gameStateHandler.OnLoadGameStateChanged += HandleLoadGameState;
        }

        private void HandleLoadGameState(LoadingGameState state)
        {
            switch (state)
            {
                case LoadingGameState.LOAD:
                    Initialize();
                    break;

                case LoadingGameState.POST:
                    OnPostLoad();
                    break;

                case LoadingGameState.FINAL:
                    OnFinalLoad();
                    break;
            }
        }

        protected abstract void Initialize();
        protected virtual void OnPostLoad() { }
        protected virtual void OnFinalLoad() { }

        // ##### MONO BEHAVIOUR ##### \\

        protected readonly EncapsulatedMonoBehaviour monoBehaviour;

        protected virtual void OnUpdate() { }

        public GameObject gameObject => monoBehaviour.gameObject;
        public Transform transform => monoBehaviour.transform;

        protected Coroutine StartCoroutine(IEnumerator routine) =>
            monoBehaviour.StartCoroutine(routine);

        protected void StopCoroutine(Coroutine coroutine) =>
            monoBehaviour.StopCoroutine(coroutine);
    }
}