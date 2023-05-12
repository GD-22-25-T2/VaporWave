namespace UDK.MEC
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class Extensions
    {
        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine) => Timing.RunCoroutine(coroutine);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, GameObject gameObj) => Timing.RunCoroutine(coroutine, gameObj);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, int layer) => Timing.RunCoroutine(coroutine, layer);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, string tag) => Timing.RunCoroutine(coroutine, tag);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, GameObject gameObj, string tag) => Timing.RunCoroutine(coroutine, gameObj, tag);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, int layer, string tag) => Timing.RunCoroutine(coroutine, layer, tag);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, Segment segment) => Timing.RunCoroutine(coroutine, segment);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, Segment segment, GameObject gameObj) => Timing.RunCoroutine(coroutine, segment, gameObj);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, Segment segment, int layer) => Timing.RunCoroutine(coroutine, segment, layer);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, Segment segment, string tag) => Timing.RunCoroutine(coroutine, segment, tag);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, Segment segment, GameObject gameObj, string tag) => Timing.RunCoroutine(coroutine, segment, gameObj, tag);

        public static CoroutineHandle RunCoroutine(this IEnumerator<float> coroutine, Segment segment, int layer, string tag) => Timing.RunCoroutine(coroutine, segment, layer, tag);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, CoroutineHandle handle, SingletonBehavior behaviorOnCollision) => Timing.RunCoroutineSingleton(coroutine, handle, behaviorOnCollision);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, GameObject gameObj, SingletonBehavior behaviorOnCollision) => !(gameObj == null)
                ? Timing.RunCoroutineSingleton(coroutine, gameObj.GetInstanceID(), behaviorOnCollision)
                : Timing.RunCoroutine(coroutine);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, int layer, SingletonBehavior behaviorOnCollision) => Timing.RunCoroutineSingleton(coroutine, layer, behaviorOnCollision);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, string tag, SingletonBehavior behaviorOnCollision) => Timing.RunCoroutineSingleton(coroutine, tag, behaviorOnCollision);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, GameObject gameObj, string tag, SingletonBehavior behaviorOnCollision) => !(gameObj == null)
                ? Timing.RunCoroutineSingleton(coroutine, gameObj.GetInstanceID(), tag, behaviorOnCollision)
                : Timing.RunCoroutineSingleton(coroutine, tag, behaviorOnCollision);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, int layer, string tag, SingletonBehavior behaviorOnCollision) => Timing.RunCoroutineSingleton(coroutine, layer, tag, behaviorOnCollision);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, CoroutineHandle handle, Segment segment, SingletonBehavior behaviorOnCollision) => Timing.RunCoroutineSingleton(coroutine, handle, segment, behaviorOnCollision);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, Segment segment, GameObject gameObj, SingletonBehavior behaviorOnCollision) => !(gameObj == null)
                ? Timing.RunCoroutineSingleton(coroutine, segment, gameObj.GetInstanceID(), behaviorOnCollision)
                : Timing.RunCoroutine(coroutine, segment);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, Segment segment, int layer, SingletonBehavior behaviorOnCollision) => Timing.RunCoroutineSingleton(coroutine, segment, layer, behaviorOnCollision);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, Segment segment, string tag, SingletonBehavior behaviorOnCollision) => Timing.RunCoroutineSingleton(coroutine, segment, tag, behaviorOnCollision);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, Segment segment, GameObject gameObj, string tag, SingletonBehavior behaviorOnCollision) => !(gameObj == null)
                ? Timing.RunCoroutineSingleton(coroutine, segment, gameObj.GetInstanceID(), tag, behaviorOnCollision)
                : Timing.RunCoroutineSingleton(coroutine, segment, tag, behaviorOnCollision);

        public static CoroutineHandle RunCoroutineSingleton(this IEnumerator<float> coroutine, Segment segment, int layer, string tag, SingletonBehavior behaviorOnCollision) => Timing.RunCoroutineSingleton(coroutine, segment, layer, tag, behaviorOnCollision);

        public static float WaitUntilDone(this IEnumerator<float> newCoroutine) => Timing.WaitUntilDone(newCoroutine);

        public static float WaitUntilDone(this IEnumerator<float> newCoroutine, string tag) => Timing.WaitUntilDone(newCoroutine, tag);

        public static float WaitUntilDone(this IEnumerator<float> newCoroutine, int layer) => Timing.WaitUntilDone(newCoroutine, layer);

        public static float WaitUntilDone(this IEnumerator<float> newCoroutine, int layer, string tag) => Timing.WaitUntilDone(newCoroutine, layer, tag);

        public static float WaitUntilDone(this IEnumerator<float> newCoroutine, Segment segment) => Timing.WaitUntilDone(newCoroutine, segment);

        public static float WaitUntilDone(this IEnumerator<float> newCoroutine, Segment segment, string tag) => Timing.WaitUntilDone(newCoroutine, segment, tag);

        public static float WaitUntilDone(this IEnumerator<float> newCoroutine, Segment segment, int layer) => Timing.WaitUntilDone(newCoroutine, segment, layer);

        public static float WaitUntilDone(this IEnumerator<float> newCoroutine, Segment segment, int layer, string tag) => Timing.WaitUntilDone(newCoroutine, segment, layer, tag);
    }
}
