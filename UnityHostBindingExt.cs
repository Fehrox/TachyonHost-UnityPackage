using System;
using System.Linq;
using TachyonCommon;
using TachyonServerCore;

public static class UnityHostBindingExt 
{
    public static void Bind<TService>(this HostCore host, TService serviceToBind) where TService : class {
        
        var serviceTypes = typeof(TService).Assembly.GetTypes();
        var serviceType = serviceTypes
            .First(t => {
                    
                    var bindingAttr = Attribute.GetCustomAttribute(t,
                        typeof(HostBindingAttribute));
                    if (bindingAttr is HostBindingAttribute binding)
                        return binding.ServiceType.IsAssignableFrom(typeof(TService));
                    
                    return false;
                }
            );
        
        var bindMethod = serviceType.GetMethod("Bind");
        var binderInstance = Activator.CreateInstance(serviceType);
        bindMethod?.Invoke(binderInstance, new object[] {host, serviceToBind, host._endPoints});
        
    }
}
