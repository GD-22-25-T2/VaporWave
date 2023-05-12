namespace UDK.API.Features.Core.Framework
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using UDK.API.Features.Core.Framework.WorldManager;

    /// <summary>
    /// The base class of all in-game entities.
    /// </summary>
    [AddComponentMenu("UDK/Core/Framework/Pawn")]
    [DisallowMultipleComponent]
    public abstract class Pawn : Actor
    {
        protected static readonly HashSet<Pawn> pawns = new();

        protected Controller playerController;
        protected GameStateBase gameState;
        protected PlayerState playerState;
        protected AudioSource audioSource;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pawn"/> containing all the entities.
        /// </summary>
        public static IEnumerable<Pawn> List => pawns;

        /// <summary>
        /// Gets the <see cref="Framework.Controller"/>.
        /// </summary>
        public Controller Controller => playerController;

        /// <summary>
        /// Gets the <see cref="GameStateBase"/>.
        /// </summary>
        public GameStateBase GameState => gameState;

        /// <summary>
        /// Gets the <see cref="Framework.PlayerState"/>.
        /// </summary>
        public PlayerState PlayerState => playerState;

        /// <summary>
        /// Gets the <see cref="UnityEngine.AudioSource"/>.
        /// </summary>
        public AudioSource AudioSource => audioSource;

        /// <summary>
        /// Gets the pawn belonging to the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="gameObject">The pawn's <see cref="GameObject"/>.</param>
        /// <returns>The corresponding <see cref="Pawn"/>, or <see langword="null"/> if not found.</returns>
        public static Pawn Get(GameObject gameObject) =>
            !gameObject ? null :
            List.FirstOrDefault(pawn => pawn.GameObject == gameObject) ??
            GetComponentSafe<Pawn>(gameObject) ??
            GetComponentSafe<Pawn>(gameObject.transform.parent.gameObject) ??
            GetComponentSafe<Pawn>(gameObject.transform.root.gameObject);

        /// <summary>
        /// Gets the <see cref="Pawn"/> belonging to the specified id.
        /// </summary>
        /// <param name="id">The pawn's id.</param>
        /// <returns>The corresponding <see cref="Pawn"/>, or <see langword="null"/> if not found.</returns>
        public static Pawn Get(uint id) => List.FirstOrDefault(pawn => pawn.PlayerState.PlayerId == id);

        /// <summary>
        /// Gets the <see cref="Pawn"/> belonging to the specified name.
        /// </summary>
        /// <param name="name">The pawn's name.</param>
        /// <returns>The corresponding <see cref="Pawn"/>, or <see langword="null"/> if not found.</returns>
        public static Pawn Get(string name) => List.FirstOrDefault(pawn => pawn.PlayerState.PlayerName == name);

        /// <summary>
        /// Tries to get a <see cref="Pawn"/> given the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="gameObject">The pawn's <see cref="GameObject"/>.</param>
        /// <param name="pawn">The corresponding <see cref="Pawn"/>, or <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a <see cref="Pawn"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(GameObject gameObject, out Pawn pawn) => pawn = Get(gameObject);

        /// <summary>
        /// Tries to get a <see cref="Pawn"/> given the specified id.
        /// </summary>
        /// <param name="id">The pawn's id.</param>
        /// <param name="pawn">The corresponding <see cref="Pawn"/>, or <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a <see cref="Pawn"/>  was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint id, out Pawn pawn) => pawn = Get(id);

        /// <summary>
        /// Tries to get a pawn given the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="name">The pawn's name.</param>
        /// <param name="pawn">The corresponding <see cref="Pawn"/>, or <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a <see cref="Pawn"/>  was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out Pawn pawn) => pawn = Get(name);

        /// <summary>
        /// Gets the <see cref="Framework.Controller"/> cast as the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The player controller type.</typeparam>
        /// <returns>The <see cref="Framework.Controller"/> cast as the specified type <typeparamref name="T"/>.</returns>
        public T GetPlayerController<T>()
            where T : Controller => Controller?.Cast<T>();

        /// <summary>
        /// Sets the <see cref="Framework.Controller"/>.
        /// </summary>
        /// <typeparam name="T">The player controller type.</typeparam>
        public T SetPlayerController<T>()
            where T : Controller
        {
            Destroy(playerController);
            return (playerController = AddComponent<T>())?.Cast<T>();
        }

        /// <summary>
        /// Gets the <see cref="GameStateBase"/> cast as the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The game state type.</typeparam>
        /// <returns>The <see cref="GameStateBase"/> cast as the specified type <typeparamref name="T"/>.</returns>
        public T GetGameState<T>()
            where T : GameStateBase => GameState?.Cast<T>();

        /// <summary>
        /// Sets the <see cref="GameStateBase"/>.
        /// </summary>
        /// <typeparam name="T">The game state type.</typeparam>
        public T SetGameState<T>()
            where T : GameStateBase
        {
            Destroy(gameState);
            return (gameState = AddComponent<T>())?.Cast<T>();
        }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            playerController = GetComponentSafe<Controller>();
            playerState = GetComponentSafe<PlayerState>() ?? AddComponent<PlayerState>();
            gameState = GetComponentSafe<GameState>();
            audioSource = GetComponentSafe<AudioSource>() ?? AddComponent<AudioSource>();
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            pawns.Add(this);
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            pawns.Remove(this);
        }

#if UNITY_EDITOR
        /// <inheritdoc/>
        protected override void OnValidating()
        {
            playerController = playerController ?? GetComponentSafe<Controller>();
            playerState = playerState ?? GetComponentSafe<PlayerState>();
            gameState = gameState ?? GetComponentSafe<GameState>();
            audioSource = audioSource ?? GetComponentSafe<AudioSource>();
        }
#endif
    }
}
