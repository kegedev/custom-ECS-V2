using Unity.Collections;

namespace Game.ECS.Base.Component
{
    internal struct ComponentContainer<T> where T : struct
    {
        internal NativeArray<int> EntityIds;//bu componente sahip entityIDleri
        internal NativeArray<T> Components;//EntityId indexine uyumlu Componentler
        internal int EntityCount;//aktifENtityComponentsayısı
    }
}

