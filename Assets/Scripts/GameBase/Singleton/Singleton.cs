/// <summary>
/// 通过Instance属性获取唯一实例
/// - 相比于静态初始化, 这种写法可以在使用时再创建实例
/// - 无法将构造函数写成private, 但是大家共同遵守就好
/// - 自己的单例类只需要继承一下即可
/// </summary>
/// <typeparam name="T">具有public构造函数的类, 不能是MonoBehaviour</typeparam>
public class Singleton<T> where T : new()
{
    private static T instance = default(T);
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
}
