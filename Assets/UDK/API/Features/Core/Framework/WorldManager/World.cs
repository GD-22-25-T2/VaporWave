namespace UDK.API.Features.Core.Framework.WorldManager
{
    using System;
    using UnityEngine;

    /// <summary>
    /// The class which handles the current game context, including entities and game modes.
    /// </summary>
    [AddComponentMenu("UDK/Core/Framework/WorldManager/World")]
    [DisallowMultipleComponent]
    [Serializable]
    [ExecuteAlways]
    public class World : Actor
    {
        private static World _instance;

        [SerializeField]
        private TSubclassOf<GameModeBase> _defaultGameMode = new();

        private GameModeBase _gameManager;

        /// <summary>
        /// Gets the world context.
        /// </summary>
        public static World Context => _instance;

        /// <summary>
        /// Gets the default game mode.
        /// </summary>
        public TSubclassOf<GameModeBase> DefaultGameMode => _defaultGameMode;

        /// <summary>
        /// Gets the current <see cref="GameModeBase"/>.
        /// </summary>
        public GameModeBase GameManager
        {
            get => _gameManager ? _gameManager : _gameManager = GetComponentSafe<GameModeBase>();
            set => _gameManager = value;
        }

        /// <summary>
        /// Gets the world context.
        /// </summary>
        /// <returns>The world context.</returns>
        public static World GetWorld() => _instance;

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            _instance = this;
            GameManager = GetComponentSafe<GameModeBase>();
        }

        /// <summary>
        /// Gets the current <see cref="GameManager"/> as the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The cast type.</typeparam>
        /// <returns>The current <see cref="GameManager"/> cast as the specified type <typeparamref name="T"/>.</returns>
        public T GetGameMode<T>()
            where T : GameModeBase => GameManager.Cast<T>();

        /// <summary>
        /// Sets a new game mode.
        /// </summary>
        /// <param name="newGameMode">The game mode to be set.</param>
        public void SetGameMode(GameModeBase newGameMode) => GameManager = newGameMode;

        /// <summary>
        /// Starts the match.
        /// </summary>
        /// <param name="forceStart">Whether match start should be forced.</param>
        public void StartMatch(bool forceStart = false)
        {
            if (GameManager.Cast(out GameMode gameManager) && !gameManager.HasMatchStarted)
            {
                if (forceStart)
                {
                    gameManager.StartMatch();
                    return;
                }

                if (!gameManager.IsReadyToStartMatch)
                    return;

                gameManager.StartMatch();
            }
        }

        /// <summary>
        /// Ends the match.
        /// </summary>
        /// <param name="forceEnd">Whether match end should be forced.</param>
        public void EndMatch(bool forceEnd = false)
        {
            if (GameManager.Cast(out GameMode gameManager) && !gameManager.HasMatchEnded)
            {
                if (forceEnd)
                {
                    gameManager.EndMatch();
                    return;
                }

                if (!gameManager.IsReadyToEndMatch)
                    return;

                gameManager.EndMatch();
            }
        }
    }
}
