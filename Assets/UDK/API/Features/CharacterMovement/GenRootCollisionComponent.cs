namespace UDK.API.Features.CharacterMovement
{
    using UDK.API.Features.Core;
    using UDK.API.Features.Core.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using static UDK.API.Features.Constants;
    using UDK.API.Features.Enums;

    /// <summary>
    /// The component which handles pawn collisions.
    /// </summary>
    public class GenRootCollisionComponent : UObject, ITransform
    {
        private static readonly IReadOnlyDictionary<Type, ECollisionShape> collisionShapes = new Dictionary<Type, ECollisionShape>()
        {
            { typeof(BoxCollider), ECollisionShape.Box },
            { typeof(SphereCollider), ECollisionShape.Sphere },
            { typeof(CapsuleCollider), ECollisionShape.VerticalCapsule },
        };

        private float halfHeight, radius;

        /// <inheritdoc/>
        protected GenRootCollisionComponent(Collider newCollision, GameObject go)
            : base(go) => CollisionBase = newCollision;

        /// <summary>
        /// Gets the root collision's <see cref="Collider"/>.
        /// </summary>
        public Collider CollisionBase { get; private set; }

        /// <summary>
        /// Gets the root collision's shape.
        /// </summary>
        public ECollisionShape CollisionShape =>
            collisionShapes.TryGetValue(CollisionBase.GetType(), out ECollisionShape collisionShape) ? collisionShape : ECollisionShape.Invalid;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Vector3"/> containing all the vertex positions.
        /// <br>Supported by <see cref="ECollisionShape.Box"/> only.</br>
        /// </summary>
        public IEnumerable<Vector3> Vertex
        {
            get
            {
                BoxCollider rootBox = GetRootCollision<BoxCollider>();
                Vector3 min = rootBox.center - rootBox.size * 0.5f;
                Vector3 max = rootBox.center + rootBox.size * 0.5f;

                yield return Transform.TransformPoint(new(min.x, min.y, min.z));
                yield return Transform.TransformPoint(new(min.x, min.y, max.z));
                yield return Transform.TransformPoint(new(min.x, max.y, min.z));
                yield return Transform.TransformPoint(new(min.x, max.y, max.z));
                yield return Transform.TransformPoint(new(max.x, min.y, min.z));
                yield return Transform.TransformPoint(new(max.x, min.y, max.z));
                yield return Transform.TransformPoint(new(max.x, max.y, min.z));
                yield return Transform.TransformPoint(new(max.x, max.y, max.z));
            }
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Vector3"/> containing all the bottom vertex positions.
        /// <br>Supported by <see cref="ECollisionShape.Box"/> only.</br>
        /// </summary>
        public IEnumerable<Vector3> BottomVertex => Vertex.OrderBy(v => v.y);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Vector3"/> containing all the front-bottom vertex positions on the X axis.
        /// <br>Supported by <see cref="ECollisionShape.Box"/> only.</br>
        /// </summary>
        public IEnumerable<Vector3> FrontXBottomVertex => BottomVertex.OrderBy(v => v.x).Take(2);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Vector3"/> containing all the back-bottom vertex positions on the X axis.
        /// <br>Supported by <see cref="ECollisionShape.Box"/> only.</br>
        /// </summary>
        public IEnumerable<Vector3> BackXBottomVertex => BottomVertex.OrderByDescending(v => v.x).Take(2);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Vector3"/> containing all the front-bottom vertex positions on the Z axis.
        /// <br>Supported by <see cref="ECollisionShape.Box"/> only.</br>
        /// </summary>
        public IEnumerable<Vector3> FrontZBottomVertex => BottomVertex.OrderBy(v => v.z).Take(2);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Vector3"/> containing all the front-bottom vertex positions on the Z axis.
        /// <br>Supported by <see cref="ECollisionShape.Box"/> only.</br>
        /// </summary>
        public IEnumerable<Vector3> BackZBottomVertex => BottomVertex.OrderByDescending(v => v.z).Take(2);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Vector3"/> containing all the top vertex positions.
        /// <br>Supported by <see cref="ECollisionShape.Box"/> only.</br>
        /// </summary>
        public IEnumerable<Vector3> TopVertex
        {
            get
            {
                float maxY = Vertex.Max(v => v.y);
                return Vertex.Where(v => v.y == maxY);
            }
        }

        /// <summary>
        /// Gets or sets the center of the root collision, measured in the object's local space.
        /// </summary>
        public Vector3 Center
        {
            get => CollisionShape switch
            {
                ECollisionShape.Box => GetRootCollision<BoxCollider>().center,
                ECollisionShape.Sphere => GetRootCollision<SphereCollider>().center,
                ECollisionShape.VerticalCapsule => GetRootCollision<CapsuleCollider>().center,
                _ => Vector3.zero,
            };
            set
            {
                switch (CollisionShape)
                {
                    case ECollisionShape.Box:
                        GetRootCollision<BoxCollider>().center = value;
                        break;
                    case ECollisionShape.Sphere:
                        GetRootCollision<SphereCollider>().center = value;
                        break;
                    case ECollisionShape.VerticalCapsule:
                        GetRootCollision<CapsuleCollider>().center = value;
                        break;
                    default:
                        return;
                }
            }
        }


        /// <summary>
        /// Gets or sets the center of the root collision, measured in the world space.
        /// </summary>
        public Vector3 LocalCenterToWorld
        {
            get => Transform.TransformPoint(Center);
            set => Center = Transform.InverseTransformPoint(value);
        }

        /// <summary>
        /// Gets a value indicating whether the collision has a horizontal collision shape.
        /// </summary>
        public bool HasVerticalCollision => CollisionShape is ECollisionShape.VerticalCapsule;

        /// <summary>
        /// Gets a value indicating whether the collision has a box collision shape.
        /// </summary>
        public bool HasBoxCollision => CollisionShape is ECollisionShape.Box;

        /// <summary>
        /// Gets a value indicating whether the collision has a sphere collision shape.
        /// </summary>
        public bool HasSphereCollision => CollisionShape is ECollisionShape.Sphere;

        /// <summary>
        /// Gets a value indicating whether the collision has a valid shape.
        /// </summary>
        public bool HasValidCollision => CollisionShape < ECollisionShape.Invalid;

        /// <inheritdoc/>
        public Transform Transform => CollisionBase.transform;

        /// <inheritdoc/>
        public Vector3 Position
        {
            get => Transform.position;
            set => Transform.position = value;
        }

        /// <inheritdoc/>
        public Quaternion Rotation
        {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }

        /// <inheritdoc/>
        public Vector3 Scale
        {
            get => Extent;
            set => Extent = value;
        }

        /// <summary>
        /// Gets the root collision's bounds.
        /// </summary>
        public Bounds Bounds => CollisionBase.bounds;

        /// <summary>
        /// Get the root collision's size.
        /// </summary>
        public Vector3 Size => Bounds.extents;

        /// <summary>
        /// Gets or sets the root collision's extent.
        /// </summary>
        public virtual Vector3 Extent
        {
            get
            {
                if (CollisionShape is ECollisionShape.Invalid)
                    return Vector3.zero;

                switch (CollisionShape)
                {
                    case ECollisionShape.VerticalCapsule:
                    {
                        CapsuleCollider rootCapsule = GetRootCollision<CapsuleCollider>();
                        return rootCapsule.bounds.extents;
                    }
                    case ECollisionShape.Box:
                    {
                        BoxCollider rootBox = GetRootCollision<BoxCollider>();
                        return rootBox.size;
                    }
                    case ECollisionShape.Sphere:
                    {
                        SphereCollider sphereCollider = GetRootCollision<SphereCollider>();
                        return sphereCollider.bounds.extents;
                    }
                }

                return Vector3.zero;
            }
            set
            {
                if (CollisionShape is ECollisionShape.Invalid)
                    return;

                ECollisionShape collisionShape = CollisionShape;
                Vector3 validExtent = GetValidExtent(collisionShape, value);

                switch (collisionShape)
                {
                    case ECollisionShape.VerticalCapsule:
                    {
                        CapsuleCollider rootCapsule = GetRootCollision<CapsuleCollider>();
                        rootCapsule.radius = validExtent.x;
                        rootCapsule.height = validExtent.y;
                        return;
                    }
                    case ECollisionShape.Box:
                    {
                        BoxCollider rootBox = GetRootCollision<BoxCollider>();
                        rootBox.size = validExtent;
                        return;
                    }
                    case ECollisionShape.Sphere:
                    {
                        SphereCollider rootSphere = GetRootCollision<SphereCollider>();
                        rootSphere.radius = validExtent.x;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the root collision's radius.
        /// </summary>
        public float Radius
        {
            get => radius;
            set
            {
                Extent = new(value, HalfHeight, value);
                radius = value;
            }
        }

        /// <summary>
        /// Gets or sets the root collision's height.
        /// </summary>
        public float HalfHeight
        {
            get => halfHeight;
            set
            {
                Extent = new(Radius, value, Radius);
                halfHeight = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="CollisionBase"/> as the specified collision shape.
        /// </summary>
        /// <typeparam name="T">The type of the collision shape.</typeparam>
        /// <returns>The cast <see cref="CollisionBase"/> or a <see cref="InvalidCastException"/> if the specified cast is not valid.</returns>
        /// <exception cref="InvalidCastException">Thrown when the specified cast is not valid.</exception>
        public virtual T GetRootCollision<T>()
            where T : Collider =>
            CollisionBase is T shapedCollision ? shapedCollision : throw new InvalidCastException($"Couldn't cast root collision as ({typeof(T).Name}).");

        /// <summary>
        /// Evaluates data to get a valid extent.
        /// </summary>
        /// <param name="collisionShape">The collision shape to evaluate.</param>
        /// <param name="extent">The extent to process.</param>
        /// <returns>A valid extent or <see cref="Vector3.zero"/> if not valid.</returns>
        public Vector3 GetValidExtent(ECollisionShape collisionShape, Vector3 extent)
        {
            if (collisionShape >= ECollisionShape.Invalid)
                return Vector3.zero;

            Vector3 validExtent = Vector3.zero;
            switch (CollisionShape)
            {
                case ECollisionShape.VerticalCapsule:
                {
                    validExtent.x = validExtent.z = Mathf.Max(KINDA_SMALL_NUMBER, extent.x, extent.z);
                    validExtent.y = Mathf.Max(KINDA_SMALL_NUMBER, extent.x, extent.y);
                    break;
                }
                case ECollisionShape.Box:
                {
                    validExtent.x = Mathf.Max(KINDA_SMALL_NUMBER, extent.x);
                    validExtent.y = Mathf.Max(KINDA_SMALL_NUMBER, extent.y);
                    validExtent.z = Mathf.Max(KINDA_SMALL_NUMBER, extent.z);
                    break;
                }
                case ECollisionShape.Sphere:
                {
                    validExtent.x = Mathf.Max(extent.x, extent.y, extent.z);
                    validExtent.x = validExtent.y = validExtent.z = Mathf.Max(KINDA_SMALL_NUMBER, validExtent.x);
                    break;
                }
            }

            return validExtent;
        }

        /// <summary>
        /// Linearly interpolates the <see cref="HalfHeight"/> given the specified params.
        /// </summary>
        /// <param name="targetHalfHeight">The target height.</param>
        /// <param name="interpSpeed">The interpolation speed.</param>
        /// <param name="interpTolerance">The interpolation tolerance.</param>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="adjustPosition">Whether the position should be adjusted.</param>
        /// <returns>The interpolated half height.</returns>
        public float LerpRootCollisionHalfHeight(
            float targetHalfHeight,
            float interpSpeed,
            float interpTolerance,
            float deltaTime,
            bool adjustPosition)
        {
            if (deltaTime < MIN_DELTA_TIME)
                return 0f;

            targetHalfHeight = Mathf.Clamp(targetHalfHeight, 2f * KINDA_SMALL_NUMBER, BIG_NUMBER);
            ECollisionShape collisionShape = CollisionShape;
            Vector3 extent = Extent;
            float initialHalfHeight = Extent.y;

            if (Mathf.Approximately(initialHalfHeight, targetHalfHeight))
                return 0f;

            float currentVelocity = 0f;
            float newHalfHeight = Mathf.SmoothDamp(initialHalfHeight, targetHalfHeight, ref currentVelocity, interpSpeed, interpTolerance, deltaTime);
            switch (collisionShape)
            {
                case ECollisionShape.VerticalCapsule:
                {
                    newHalfHeight = Mathf.Max(extent.x, newHalfHeight);
                    break;
                }
                case ECollisionShape.Box:
                case ECollisionShape.Sphere:
                    break;
            }

            if (Mathf.Approximately(newHalfHeight, initialHalfHeight))
                return 0f;

            float SetNewExtent()
            {
                switch (collisionShape)
                {
                    case ECollisionShape.VerticalCapsule:
                    case ECollisionShape.Box:
                    {
                        Extent = new(newHalfHeight, extent.y, newHalfHeight);
                        return Mathf.Abs(Extent.y - initialHalfHeight);
                    }
                    case ECollisionShape.Sphere:
                    {
                        Extent = new(newHalfHeight, newHalfHeight, newHalfHeight);
                        return Mathf.Abs(Extent.x - initialHalfHeight);
                    }
                }

                return 0f;
            }

            if (!adjustPosition)
                return SetNewExtent();

            float halfHeightChange = 0f;
            if (newHalfHeight > initialHalfHeight)
            {
                halfHeightChange = SetNewExtent();
                if (halfHeightChange == 0f)
                    return 0f;

                return halfHeightChange;
            }

            return halfHeightChange;
        }
    }
}
