namespace CS2Shared.Manager;

public interface IManager {
    void OnCreate();
    void OnUpdate();
    void OnReset();
    void OnDestroy();
}

public interface IManager<T> : IManager {
    void OnCreate(T t);
}

public interface IManager<T1, T2> : IManager {
    void OnCreate(T1 t1, T2 t2);
}

public interface IManager<T1, T2, T3> : IManager {
    void OnCreate(T1 t1, T2 t2, T3 t3);
}

public interface IManager<T1, T2, T3, T4> : IManager {
    void OnCreate(T1 t1, T2 t2, T3 t3, T4 t4);
}