using System;

namespace NUSHPOS.Helpers;

public class NavigationService
{
    public event Action<Type>? NavigationRequested;
    public event Action<Type, object?>? NavigationWithParamRequested;
    
    public void NavigateTo<T>() where T : class
    {
        NavigationRequested?.Invoke(typeof(T));
    }
    
    public void NavigateTo<T>(object? parameter) where T : class
    {
        NavigationWithParamRequested?.Invoke(typeof(T), parameter);
    }
}
