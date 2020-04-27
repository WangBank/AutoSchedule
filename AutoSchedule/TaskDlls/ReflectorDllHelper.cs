using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoSchedule.TaskDlls
{
    public static class ReflectorDllHelper
    {
        public static Object ReturnObjType(string dllPath,string DllOrUrl)
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);
            Type type = assembly.GetType(DllOrUrl.Split(',')[1]);
            object obj = Activator.CreateInstance(type);
            return obj;
        }
        
    }
}
