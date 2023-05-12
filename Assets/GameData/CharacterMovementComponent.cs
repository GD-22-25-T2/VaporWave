namespace GameData
{
    using UDK.API.Features.CharacterMovement;
    using UDK.API.Features.Enums;
    using UDK.API.Features.Input;
    using UDK.MEC;
    using UnityEngine;
    using static UDK.API.Features.Constants;

    /// <inheritdoc/>
    internal class CharacterMovementComponent : GenMovementComponent
    {
        #region Editor

        [SerializeField]
        private float _chainedUpJumpForce;

        [SerializeField]
        private byte _chainedUpJumps;

        [SerializeField]
        private float _timeBeforeNextChainedUpJump;

        [SerializeField]
        private LayerMask _wallMask;

        [SerializeField]
        private float _wallSlidingSpeed;

        [SerializeField]
        private Vector3 _wallJumpingForce;

        #endregion

        #region BackingFields

        private bool _isWalled;
        private bool _isWallSliding;

        #endregion

        /// <summary>
        /// Gets a value indicating whether the pawn is facing right.
        /// </summary>
        internal bool IsFacingRight { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the pawn is wall jumping.
        /// </summary>
        internal bool IsWallJumping { get; private set; }

        /// <summary>
        /// Gets the wall jumping direction.
        /// </summary>
        internal float WallJumpingDirection { get; private set; }

        /// <summary>
        /// Gets the wall jumping time.
        /// </summary>
        internal float WallJumpingTime { get; private set; }

        /// <summary>
        /// Gets the wall jumping counter.
        /// </summary>
        internal float WallJumpingCounter { get; private set; }

        /// <summary>
        /// Gets the wall jumping duration.
        /// </summary>
        internal float WallJumpingDuration { get; private set; }

        /// <summary>
        /// Gets the amount of remaining chained up jumps.
        /// </summary>
        internal byte RemainingJumps { get; private set; }

        /// <inheritdoc/>
        protected override bool CanJump => IsAirborne ?
            GetMovementState<EMovementState>() == EMovementState.Vaulting ? WallJumpingCounter > 0f :
            isJumpAvailable && RemainingJumps > 0 :
            isJumpAvailable && !JustJumped;

        /// <inheritdoc/>
        protected override void DefineInputBindings()
        {
            InputBinding.Create("Horizontal");
            InputBinding.Create("Jump", KeyCode.Space);
            InputBinding.Create("Hook", KeyCode.LeftShift);
            InputBinding.Create("Pause", KeyCode.Escape);
        }

        /// <inheritdoc/>
        protected override void BindInputActions()
        {
            InputActionComponent.BindInputAxis(InputBinding.Get("Horizontal"), MoveRight);

            InputActionComponent.BindInputAction(InputBinding.Get("Jump"), Jump);
            InputActionComponent.BindInputAction(InputBinding.Get("Hook"), HookAction);
            InputActionComponent.BindInputAction(InputBinding.Get("Pause"), PauseAction);
        }

        /// <inheritdoc/>
        protected override void PreMovementUpdate(float deltaTime)
        {
            base.PreMovementUpdate(deltaTime);

            _isWalled = Physics2D.OverlapCircle(Position, 0.2f, _wallMask);
            _isWallSliding = _isWalled && IsAirborne && Horizontal != 0f;
        }

        /// <inheritdoc/>
        protected override void MovementUpdate(float deltaTime)
        {
            base.MovementUpdate(deltaTime);

            WallSlide();
            WallJump(deltaTime);

            if (!IsWallJumping)
                TryFlip();
        }

        /// <inheritdoc/>
        protected override void ResolveMovementState()
        {
            SetMovementState(IsAirborne ? EMovementState.Airborne :
            !WantsToMove || SpeedXZ <= 0.1f ? EMovementState.None :
            _isWallSliding ? EMovementState.Vaulting : EMovementState.Jogging);
        }

        /// <inheritdoc/>
        protected override void Jump(bool wantsToJump)
        {
            WantsToJump = wantsToJump;

            if (!WantsToJump)
            {
                canJump = CanJump;
                return;
            }

            if (!canJump)
                return;

            isJumpAvailable = false;
            JustJumped = true;

            if (!IsAirborne)
            {
                Rigidbody.AddForce(Transform.up * jumpForce, ForceMode.Impulse);
                return;
            }

            if (GetMovementState<EMovementState>() == EMovementState.Vaulting)
            {
                IsWallJumping = true;
                Velocity = new(WallJumpingDirection * _wallJumpingForce.x, _wallJumpingForce.y, Velocity.z);
                WallJumpingCounter = 0f;

                if (Transform.localScale.x != WallJumpingDirection)
                    Flip();

                Timing.CallDelayed(WallJumpingDuration, () =>
                {
                    if (GetMovementState<EMovementState>() != EMovementState.Vaulting)
                        IsWallJumping = false;
                });

                return;
            }

            RemainingJumps -= 1;
            Rigidbody.AddForce(Transform.up * _chainedUpJumpForce, ForceMode.Impulse);
            Timing.CallDelayed(_timeBeforeNextChainedUpJump, () => isJumpAvailable = true);
        }

        /// <inheritdoc/>
        protected override void OnLanded()
        {
            base.OnLanded();

            RemainingJumps = _chainedUpJumps;
        }

        #region Internal

        private void WallSlide()
        {
            if (GetMovementState<EMovementState>() == EMovementState.Vaulting)
                Velocity = new(Velocity.x, Mathf.Clamp(Velocity.y, -_wallSlidingSpeed, BIG_NUMBER), Velocity.z);
        }

        private void WallJump(float deltaTime)
        {
            if (GetMovementState<EMovementState>() != EMovementState.Vaulting)
            {
                WallJumpingCounter -= deltaTime;
                return;
            }

            IsWallJumping = false;
            WallJumpingDirection = -Transform.localScale.x;
            WallJumpingCounter = WallJumpingTime;
        }

        private void TryFlip()
        {
            if ((!IsFacingRight || Horizontal >= 0f) && (IsFacingRight || Horizontal <= 0f))
                return;

            Flip();
        }

        private void Flip()
        {
            IsFacingRight = !IsFacingRight;
            Vector3 localScale = Transform.localScale;
            localScale.x *= -1;
            Transform.localScale = localScale;
        }

        #endregion

        #region InputActions

        private void HookAction(bool wantsToHook)
        {

        }

        private void PauseAction(bool wantsToPause)
        {

        }
        #endregion
    }
}